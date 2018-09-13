using System.Linq;
using System.Threading.Tasks;
using ArchitectureSample.Core.Datas.DataStores;
using ArchitectureSample.Core.Datas.DataStores.AwsApi;
using ArchitectureSample.Core.Datas.Entities;
using ArchitectureSample.Core.Entities;

namespace ArchitectureSample.Core.Repositories
{
    public class LoadBalancerRepository
    {
        public string LoadBalancerName { get; private set; }
        public bool IsAlb { get; private set; }

        public LoadBalancerRepository(string loadBalancerName, bool isAlb)
        {
            LoadBalancerName = loadBalancerName;
            IsAlb = isAlb;
            SingletonLoadbalancerClient.Initialize();
            SingletonLoadbalancerV2Client.Initialize();
        }

        public LoadBalancerRepository(string loadBalancerName, bool isAlb, string profile, string region)
        {
            LoadBalancerName = loadBalancerName;
            IsAlb = isAlb;
            SingletonLoadbalancerClient.Initialize(profile, region);
            SingletonLoadbalancerV2Client.Initialize(profile, region);
        }

        public async Task<ILoadBalancerInstanceState> GetInstanceState(bool useCache = false)
        {
            if (IsAlb)
            {
                var current = await LoadBalancerDatastore.GetAlbInstancesAsync(LoadBalancerName, useCache);
                var inServiceInstances = current.Where(x => x.State == "healthy").ToArray();
                var outofServiceInstances = current.Except(inServiceInstances).ToArray();
                return new LoadBalancerInstanceState
                {
                    All = current,
                    InService = inServiceInstances,
                    OutofService = outofServiceInstances,
                };
            }
            else
            {
                var current = await LoadBalancerDatastore.GetClbInstancesAsync(LoadBalancerName, useCache);
                var inServiceInstances = current.Where(x => x.State == "InService").ToArray();
                var outofServiceInstances = current.Except(inServiceInstances).ToArray();
                return new LoadBalancerInstanceState
                {
                    All = current,
                    InService = inServiceInstances,
                    OutofService = outofServiceInstances,
                };
            }
        }

        public async Task<ILoadbalancerAttribute> DescribeAttributes()
        {
            if (IsAlb)
            {
                var response = await LoadBalancerDatastore.DescribeAlbAttributesAsync(LoadBalancerName);
                return response;
            }
            else
            {
                var response = await LoadBalancerDatastore.DescribeClbAttributesAsync(LoadBalancerName);
                return response;
            }
        }
    }
}
