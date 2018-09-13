using System;
using Amazon;
using Amazon.EC2;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancingV2;
using Amazon.Runtime;

namespace ArchitectureSample.Core.Datas.DataStores.AwsApi
{
    internal class SingletonEc2InstanceClient
    {
        public static AmazonEC2Client Instance => instance.Value;
        private static readonly Lazy<AmazonEC2Client> instance = new Lazy<AmazonEC2Client>(() => credential == null || Region == null ? new AmazonEC2Client() : new AmazonEC2Client(credential, Region));
        public static string Endpoint { get; private set; }
        private static AWSCredentials credential;
        public static RegionEndpoint Region { get; private set; }

        private static bool initialized = false;

        // Dont Construct
        private SingletonEc2InstanceClient() { }

        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize()
        {
            if (initialized)
                return;
            initialized = true;
        }

        /// <summary>
        /// Initialize with profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="region"></param>
        public static void Initialize(string profile, string region)
        {
            Initialize();

            credential = AmazonCredential.GetCredential(profile);
            Region = RegionEndpoint.GetBySystemName(region);
        }
    }

    internal class SingletonLoadbalancerClient
    {
        public static AmazonElasticLoadBalancingClient Instance => instance.Value;
        private static readonly Lazy<AmazonElasticLoadBalancingClient> instance = new Lazy<AmazonElasticLoadBalancingClient>(() => credential == null || Region == null ? new AmazonElasticLoadBalancingClient() : new AmazonElasticLoadBalancingClient(credential, Region));
        public static string Endpoint { get; private set; }
        private static AWSCredentials credential;
        public static RegionEndpoint Region { get; private set; }

        private static bool initialized = false;

        // Dont Construct
        private SingletonLoadbalancerClient() { }

        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize()
        {
            if (initialized)
                return;
            initialized = true;
        }

        /// <summary>
        /// Initialize with profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="region"></param>
        public static void Initialize(string profile, string region)
        {
            Initialize();

            credential = AmazonCredential.GetCredential(profile);
            Region = RegionEndpoint.GetBySystemName(region);
        }
    }

    internal class SingletonLoadbalancerV2Client
    {
        public static AmazonElasticLoadBalancingV2Client Instance => instance.Value;
        private static readonly Lazy<AmazonElasticLoadBalancingV2Client> instance = new Lazy<AmazonElasticLoadBalancingV2Client>(() => credential == null || Region == null ? new AmazonElasticLoadBalancingV2Client() : new AmazonElasticLoadBalancingV2Client(credential, Region));
        public static string Endpoint { get; private set; }
        private static AWSCredentials credential;
        public static RegionEndpoint Region { get; private set; }

        private static bool initialized = false;

        // Dont Construct
        private SingletonLoadbalancerV2Client() { }

        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize()
        {
            if (initialized)
                return;
            initialized = true;
        }

        /// <summary>
        /// Initialize with profile
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="region"></param>
        public static void Initialize(string profile, string region)
        {
            Initialize();

            credential = AmazonCredential.GetCredential(profile);
            Region = RegionEndpoint.GetBySystemName(region);
        }
    }
}
