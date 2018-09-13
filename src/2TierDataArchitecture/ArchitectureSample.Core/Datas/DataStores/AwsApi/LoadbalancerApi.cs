using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.ElasticLoadBalancing.Model;
using Amazon.ElasticLoadBalancingV2.Model;

namespace ArchitectureSample.Core.Datas.DataStores.AwsApi
{
    internal static class LoadbalancerApi
    {
        // CLB
        public static async Task<(bool success, List<LoadBalancerDescription> response)> DescribeClbLoadBalancer(IEnumerable<string> loadBalancerName)
        {
            var responses = new List<LoadBalancerDescription>();
            Amazon.ElasticLoadBalancing.Model.DescribeLoadBalancersResponse response = null;
            do
            {
                response = response = await SingletonLoadbalancerClient.Instance.DescribeLoadBalancersAsync(new Amazon.ElasticLoadBalancing.Model.DescribeLoadBalancersRequest()
                {
                    LoadBalancerNames = loadBalancerName.ToList(),
                });
                responses.AddRange(response.LoadBalancerDescriptions);
            }
            while (!string.IsNullOrEmpty(response.NextMarker));
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, responses);
        }

        public static async Task<(bool success, List<Amazon.ElasticLoadBalancing.Model.InstanceState> response)> DescribeClbInstancesAsync(string loadbalancerName)
        {
            DescribeInstanceHealthResponse response = await SingletonLoadbalancerClient.Instance.DescribeInstanceHealthAsync(new Amazon.ElasticLoadBalancing.Model.DescribeInstanceHealthRequest()
            {
                LoadBalancerName = loadbalancerName,
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response.InstanceStates);
        }

        public static async Task<(bool success, Amazon.ElasticLoadBalancing.Model.LoadBalancerAttributes response)> DescribeClbAttributesAsync(string loadbalancerName)
        {
            Amazon.ElasticLoadBalancing.Model.DescribeLoadBalancerAttributesResponse response = await SingletonLoadbalancerClient.Instance.DescribeLoadBalancerAttributesAsync(new Amazon.ElasticLoadBalancing.Model.DescribeLoadBalancerAttributesRequest()
            {
                LoadBalancerName = loadbalancerName,
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response.LoadBalancerAttributes);
        }

        public static async Task<(bool success, List<Amazon.ElasticLoadBalancing.Model.Instance> instances)> RegisterClbInstancesAsync(string loadbalancerName, IEnumerable<string> instanceIds)
        {
            RegisterInstancesWithLoadBalancerResponse response = await SingletonLoadbalancerClient.Instance.RegisterInstancesWithLoadBalancerAsync(new Amazon.ElasticLoadBalancing.Model.RegisterInstancesWithLoadBalancerRequest()
            {
                LoadBalancerName = loadbalancerName,
                Instances = instanceIds.Select(x => new Amazon.ElasticLoadBalancing.Model.Instance() { InstanceId = x }).ToList(),
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response.Instances);
        }

        public static async Task<(bool success, List<Amazon.ElasticLoadBalancing.Model.Instance> instances)> DeregisterClbInstancesAsync(string loadbalancerName, IEnumerable<string> instanceIds)
        {
            DeregisterInstancesFromLoadBalancerResponse response = await SingletonLoadbalancerClient.Instance.DeregisterInstancesFromLoadBalancerAsync(new Amazon.ElasticLoadBalancing.Model.DeregisterInstancesFromLoadBalancerRequest()
            {
                LoadBalancerName = loadbalancerName,
                Instances = instanceIds.Select(x => new Amazon.ElasticLoadBalancing.Model.Instance() { InstanceId = x }).ToList(),
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response.Instances);
        }

        // ALB
        public static async Task<List<LoadBalancer>> DescribeAlbLoadBalancer(IEnumerable<string> loadBalancerName)
        {
            Amazon.ElasticLoadBalancingV2.Model.DescribeLoadBalancersResponse response = await SingletonLoadbalancerV2Client.Instance.DescribeLoadBalancersAsync(new Amazon.ElasticLoadBalancingV2.Model.DescribeLoadBalancersRequest()
            {
                Names = loadBalancerName.ToList(),
            });
            return response.LoadBalancers;
        }

        public static async Task<(bool success, (TargetDescription description, string health)[] response)[]> DescribeAlbInstancesAsync(IEnumerable<string> loadbalancerNames)
        {
            List<LoadBalancer> loadBalancers = await DescribeAlbLoadBalancer(loadbalancerNames);

            (bool success, (TargetDescription description, string health)[] response)[][] instances = await loadBalancers.Select(async x =>
            {
                (var success, List<TargetGroup> response) = await DescribeAllTargetGroupsAsync(x.LoadBalancerArn);
                IEnumerable<Task<(bool success, (TargetDescription description, string health)[] response)>> tasks = response.Select(async y => await DescribeAllInstancesAsync(y.TargetGroupArn));
                (bool success, (TargetDescription description, string health)[] response)[] result = await tasks.WhenAll();
                return result;
            })
            .WhenAll();

            return instances.SelectMany(x => x).ToArray();
        }

        public static async Task<(bool success, List<Amazon.ElasticLoadBalancingV2.Model.TargetGroup> response)> DescribeAllTargetGroupsAsync(string loadbalancerArn)
        {
            var responses = new List<Amazon.ElasticLoadBalancingV2.Model.TargetGroup>();
            DescribeTargetGroupsResponse response = null;
            do
            {
                response = await SingletonLoadbalancerV2Client.Instance.DescribeTargetGroupsAsync(new Amazon.ElasticLoadBalancingV2.Model.DescribeTargetGroupsRequest()
                {
                    LoadBalancerArn = loadbalancerArn,
                });
                responses.AddRange(response.TargetGroups);
            }
            while (!string.IsNullOrEmpty(response.NextMarker));
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, responses);
        }

        public static async Task<(bool success, (TargetDescription description, string health)[] response)> DescribeAllInstancesAsync(string targetgroupArn)
        {
            DescribeTargetHealthResponse response = await SingletonLoadbalancerV2Client.Instance.DescribeTargetHealthAsync(new DescribeTargetHealthRequest()
            {
                TargetGroupArn = targetgroupArn,
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response.TargetHealthDescriptions.Select(x => (x.Target, x.TargetHealth.State.Value)).ToArray());
        }

        public static async Task<(bool success, List<Amazon.ElasticLoadBalancingV2.Model.TargetGroupAttribute> response)> DescribeAlbAttributesAsync(string loadbalancerName)
        {
            Amazon.ElasticLoadBalancingV2.Model.DescribeLoadBalancersResponse albs = await SingletonLoadbalancerV2Client.Instance.DescribeLoadBalancersAsync(new Amazon.ElasticLoadBalancingV2.Model.DescribeLoadBalancersRequest()
            {
                Names = new[] { loadbalancerName }.ToList(),
            });

            DescribeTargetGroupsResponse targetGroups = await SingletonLoadbalancerV2Client.Instance.DescribeTargetGroupsAsync(new DescribeTargetGroupsRequest()
            {
                LoadBalancerArn = albs.LoadBalancers.FirstOrDefault().LoadBalancerArn,
            });

            DescribeTargetGroupAttributesResponse attribute = await SingletonLoadbalancerV2Client.Instance.DescribeTargetGroupAttributesAsync(new DescribeTargetGroupAttributesRequest()
            {
                TargetGroupArn = targetGroups.TargetGroups.First().TargetGroupArn,
            });
            return (attribute.HttpStatusCode == System.Net.HttpStatusCode.OK, attribute.Attributes);
        }

        public static async Task<bool> RegisterAlbInstancesAsync(IEnumerable<string> loadbalancerNames, IEnumerable<string> instanceIds)
        {
            List<LoadBalancer> loadBalancers = await DescribeAlbLoadBalancer(loadbalancerNames);

            (bool success, List<TargetGroup> response)[] targetGroups = await loadBalancers.Select(async x => await DescribeAllTargetGroupsAsync(x.LoadBalancerArn)).WhenAll();

            var result = await targetGroups
                .Select(x => x.response)
                .SelectMany(x => x)
                .Select(x => RegisterAlbInstancesAsync(x.TargetGroupArn, instanceIds.ToList()))
                .WhenAll();
            return result.All(x => x);
        }

        public static async Task<bool> RegisterAlbInstancesAsync(string targetGroupName, IEnumerable<string> loadbalancerNames, IEnumerable<string> instanceIds)
        {
            List<LoadBalancer> loadBalancers = await DescribeAlbLoadBalancer(loadbalancerNames);

            (bool success, List<TargetGroup> response)[] targetGroups = await loadBalancers.Select(async x => await DescribeAllTargetGroupsAsync(x.LoadBalancerArn)).WhenAll();
            var result = await targetGroups
                .Select(x => x.response)
                .SelectMany(x => x)
                .Where(x => x.TargetGroupName == targetGroupName)
                .Select(x => RegisterAlbInstancesAsync(x.TargetGroupArn, instanceIds.ToList()))
                .WhenAll();
            return result.All(x => x);
        }

        public static async Task<bool> RegisterAlbInstancesAsync(string targetGroupArn, IEnumerable<string> instanceIds)
        {
            RegisterTargetsResponse response = await SingletonLoadbalancerV2Client.Instance.RegisterTargetsAsync(new RegisterTargetsRequest()
            {
                TargetGroupArn = targetGroupArn,
                Targets = instanceIds.Select(x => new TargetDescription() { Id = x }).ToList(),
            });
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public static async Task<bool> DeregisterAlbInstancesAsync(IEnumerable<string> loadbalancerNames, IEnumerable<string> instanceIds)
        {
            List<LoadBalancer> loadBalancers = await DescribeAlbLoadBalancer(loadbalancerNames);

            (bool success, List<TargetGroup> response)[] targetGroups = await loadBalancers.Select(async x => await DescribeAllTargetGroupsAsync(x.LoadBalancerArn)).WhenAll();
            var result = await targetGroups
                .Select(x => x.response)
                .SelectMany(x => x)
                .Select(x => DeregisterAlbInstancesAsync(x.TargetGroupArn, instanceIds.ToList()))
                .WhenAll();
            return result.All(x => x);
        }

        public static async Task<bool> DeregisterAlbInstancesAsync(string targetGroupName, IEnumerable<string> loadbalancerNames, IEnumerable<string> instanceIds)
        {
            List<LoadBalancer> loadBalancers = await DescribeAlbLoadBalancer(loadbalancerNames);

            (bool success, List<TargetGroup> response)[] targetGroups = await loadBalancers.Select(async x => await DescribeAllTargetGroupsAsync(x.LoadBalancerArn)).WhenAll();
            var result = await targetGroups
                .Select(x => x.response)
                .SelectMany(x => x)
                .Where(x => x.TargetGroupName == targetGroupName)
                .Select(x => DeregisterAlbInstancesAsync(x.TargetGroupArn, instanceIds.ToList()))
                .WhenAll();
            return result.All(x => x);
        }

        public static async Task<bool> DeregisterAlbInstancesAsync(string targetGroupArn, IEnumerable<string> instanceIds)
        {
            DeregisterTargetsResponse response = await SingletonLoadbalancerV2Client.Instance.DeregisterTargetsAsync(new DeregisterTargetsRequest()
            {
                TargetGroupArn = targetGroupArn,
                Targets = instanceIds.Select(x => new TargetDescription() { Id = x }).ToList(),
            });
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
    }
}
