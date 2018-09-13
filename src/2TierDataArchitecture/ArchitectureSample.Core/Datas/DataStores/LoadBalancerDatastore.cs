using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchitectureSample.Core.Datas.DataStores.AwsApi;
using ArchitectureSample.Core.Datas.Entities;
using ArchitectureSample.Core.Entities;

namespace ArchitectureSample.Core.Datas.DataStores
{
    internal static class LoadBalancerDatastore
    {
        private static ILoadBalancerInstance[] cachedGetElbInstancesAsync;
        private static ILoadBalancerInstance[] cachedGetAlbInstancesAsync;

        #region CLB
        public static async Task<ILoadBalancerInstance[]> GetClbInstancesAsync(string loadbalancerName, bool useCache = false)
        {
            if (useCache && cachedGetElbInstancesAsync != null && cachedGetElbInstancesAsync.Any())
            {
                return cachedGetElbInstancesAsync;
            }

            (bool success, List<Amazon.ElasticLoadBalancing.Model.InstanceState> response) response = await LoadbalancerApi.DescribeClbInstancesAsync(loadbalancerName);
            LoadBalancerInstance[] result = response.response.Select(x => new LoadBalancerInstance() { InstanceId = x.InstanceId, State = x.State }).ToArray();

            cachedGetElbInstancesAsync = result;
            return result;
        }

        public static async Task<bool> RegisterClbInstancesAsync(string loadbalancerName, IEnumerable<string> instanceIds)
        {
            await LoadbalancerApi.RegisterClbInstancesAsync(loadbalancerName, instanceIds).ConfigureAwait(false);
            return true;
        }

        public static async Task<bool> DeregisterClbInstancesAsync(string loadbalancerName, IEnumerable<string> instanceIds)
        {
            await LoadbalancerApi.DeregisterClbInstancesAsync(loadbalancerName, instanceIds).ConfigureAwait(false);
            return true;
        }


        public static async Task<ILoadbalancerAttribute> DescribeClbAttributesAsync(string loadbalancerName)
        {
            (bool success, Amazon.ElasticLoadBalancing.Model.LoadBalancerAttributes response) response = await LoadbalancerApi.DescribeClbAttributesAsync(loadbalancerName).ConfigureAwait(false);
            return new LoadbalancerAttribute().Bind(response.success, response.response);
        }
        #endregion

        #region Alb
        public static async Task<ILoadBalancerInstance[]> GetAlbInstancesAsync(string loadbalancerName, bool useCache = false)
        {
            if (useCache && cachedGetAlbInstancesAsync != null && cachedGetAlbInstancesAsync.Any())
            {
                return cachedGetAlbInstancesAsync;
            }

            (bool success, (Amazon.ElasticLoadBalancingV2.Model.TargetDescription description, string health)[] response)[] response = await LoadbalancerApi.DescribeAlbInstancesAsync(new[] { loadbalancerName });
            LoadBalancerInstance[] result = response.SelectMany(x => x.response.Select(y => new LoadBalancerInstance() { InstanceId = y.description.Id, State = y.health })).ToArray();

            cachedGetAlbInstancesAsync = result;
            return result;
        }

        public static async Task<bool> RegisterAlbInstancesAsync(string loadbalancerName, IEnumerable<string> instanceIds)
        {
            await LoadbalancerApi.RegisterAlbInstancesAsync(loadbalancerName, instanceIds).ConfigureAwait(false);
            return true;
        }

        public static async Task<bool> DeregisterAlbInstancesAsync(string loadbalancerName, IEnumerable<string> instanceIds)
        {
            await LoadbalancerApi.DeregisterAlbInstancesAsync(loadbalancerName, instanceIds).ConfigureAwait(false);
            return true;
        }

        public static async Task<ILoadbalancerAttribute> DescribeAlbAttributesAsync(string loadbalancerName)
        {
            (bool success, List<Amazon.ElasticLoadBalancingV2.Model.TargetGroupAttribute> response) response = await LoadbalancerApi.DescribeAlbAttributesAsync(loadbalancerName).ConfigureAwait(false);
            return new LoadbalancerAttribute().Bind(response.success, response.response);
        }

        #endregion
    }
}
