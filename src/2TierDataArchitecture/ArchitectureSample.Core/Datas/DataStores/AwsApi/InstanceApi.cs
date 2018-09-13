using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace ArchitectureSample.Core.Datas.DataStores.AwsApi
{
    internal static class InstanceApi
    {
        public static async Task<(bool success, List<Instance> response)> DescribeAsync()
        {
            var responses = new List<Instance>();
            DescribeInstancesResponse response = null;
            do
            {
                response = await SingletonEc2InstanceClient.Instance.DescribeInstancesAsync();
                // target is only running instances.
                responses.AddRange(response.Reservations.SelectMany(x => x.Instances).Where(x => x.State.Name == "running"));
            }
            while (!string.IsNullOrEmpty(response.NextToken));
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, responses);
        }

        public static async Task<(bool success, List<Instance> response)> DescribeAsync(IEnumerable<string> instanceIds)
        {
            var responses = new List<Instance>();
            DescribeInstancesResponse response = null;
            do
            {
                response = await SingletonEc2InstanceClient.Instance.DescribeInstancesAsync(new DescribeInstancesRequest()
                {
                    Filters = new List<Filter>() { new Filter("instance-id", instanceIds.ToList()) }
                });
                // target is only running instances.
                responses.AddRange(response.Reservations.SelectMany(x => x.Instances).Where(x => x.State.Name == "running"));
            }
            while (!string.IsNullOrEmpty(response.NextToken));
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, responses);
        }

        public static async Task<bool> RebootAsync(IEnumerable<string> instanceIds)
        {
            var response = await SingletonEc2InstanceClient.Instance.RebootInstancesAsync(new RebootInstancesRequest()
            {
                InstanceIds = instanceIds.ToList(),
            });
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public static async Task<(bool success, List<InstanceStateChange> response)> StartAsync(IEnumerable<string> instanceIds)
        {
            var response = await SingletonEc2InstanceClient.Instance.StartInstancesAsync(new StartInstancesRequest()
            {
                InstanceIds = instanceIds.ToList(),
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response.StartingInstances);
        }

        public static async Task<(bool success, List<InstanceStateChange> response)> StopAsync(IEnumerable<string> instanceIds, bool force = false)
        {
            var response = await SingletonEc2InstanceClient.Instance.StopInstancesAsync(new StopInstancesRequest()
            {
                InstanceIds = instanceIds.ToList(),
                Force = force,
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response.StoppingInstances);
        }

        public static async Task<(bool success, List<InstanceStateChange> response)> TerminateAsync(IEnumerable<string> instanceIds, bool force = false)
        {
            var response = await SingletonEc2InstanceClient.Instance.TerminateInstancesAsync(new TerminateInstancesRequest()
            {
                InstanceIds = instanceIds.ToList(),
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response.TerminatingInstances);
        }

        public static async Task<bool> DisableApiTerminationAsync(string instanceId, bool disableApiTermination)
        {
            var request = new ModifyInstanceAttributeRequest()
            {
                InstanceId = instanceId,
                DisableApiTermination = disableApiTermination,
            };
            var response = await SingletonEc2InstanceClient.Instance.ModifyInstanceAttributeAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public static async Task<bool> ChangeInstanceTypeAsync(string instanceId, InstanceType instanceType)
        {
            var request = new ModifyInstanceAttributeRequest()
            {
                InstanceId = instanceId,
                InstanceType = instanceType,
            };
            var response = await SingletonEc2InstanceClient.Instance.ModifyInstanceAttributeAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public static async Task<bool> ChangeSourceDestCheckAsync(string instanceId, bool sourceDestCheck)
        {
            var request = new ModifyInstanceAttributeRequest()
            {
                InstanceId = instanceId,
                SourceDestCheck = sourceDestCheck,
            };
            var response = await SingletonEc2InstanceClient.Instance.ModifyInstanceAttributeAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        public static async Task<bool> ChangeUserDataAsync(string instanceId, string userData)
        {
            var request = new ModifyInstanceAttributeRequest()
            {
                InstanceId = instanceId,
                UserData = userData,
            };
            var response = await SingletonEc2InstanceClient.Instance.ModifyInstanceAttributeAsync(request);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }

        private static async Task<(bool success, ModifyInstanceAttributeResponse response)> ModifyAttributeAsync(string instanceId, ModifyInstanceAttributeRequest request)
        {
            var response = await SingletonEc2InstanceClient.Instance.ModifyInstanceAttributeAsync(request);
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response);
        }

        public static async Task<(bool success, List<Instance> response)> RequestAsync(LaunchTemplateSpecification launchTemplateSpec, string clientToken, int maxCount, int minCount, InstanceType instanceType, string subnetId)
        {
            var response = await SingletonEc2InstanceClient.Instance.RunInstancesAsync(new RunInstancesRequest()
            {
                ClientToken = clientToken,
                LaunchTemplate = launchTemplateSpec,
                InstanceType = instanceType,
                MaxCount = maxCount,
                MinCount = minCount,
                SubnetId = subnetId,
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response.Reservation.Instances);
        }
    }
}
