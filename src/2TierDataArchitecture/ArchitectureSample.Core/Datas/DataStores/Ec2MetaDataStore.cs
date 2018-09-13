using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ArchitectureSample.Core.Datas.DataStores
{
    internal class Ec2MetaDataStore
    {
        // Detect Ec2 or not
        public static readonly AsyncLazy<bool> IsEc2Instance = AsyncLazy.Create(async () => await TestIsEc2Instance());

        // Lazy Initialization. It's Meta but don't want static initalization ran when app running.
        public static AsyncLazy<string> AmiId = AsyncLazy.Create(async () => await GetMetaInfo(AmiIdUrl));
        public static AsyncLazy<string> AmiLaunchIndex = AsyncLazy.Create(async () => await GetMetaInfo(AmiLaunchIndexUrl));
        public static AsyncLazy<string> AmiManifestPath = AsyncLazy.Create(async () => await GetMetaInfo(AmiManifestPathUrl));
        public static AsyncLazy<string[]> BlockDeviceMapping = AsyncLazy.Create(async () => await GetMetaInfoArray(BlockDeviceMappingUrl));
        public static AsyncLazy<string> HostName = AsyncLazy.Create(async () => await GetMetaInfo(HostNameUrl));
        public static AsyncLazy<string> Iam = AsyncLazy.Create(async () => await GetMetaInfo(IamUrl));
        public static AsyncLazy<string> InstanceAction = AsyncLazy.Create(async () => await GetMetaInfo(InstanceActionUrl));
        public static AsyncLazy<string> InstanceId = AsyncLazy.Create(async () => await GetMetaInfo(InstanceIdUrl));
        public static AsyncLazy<string> InstanceType = AsyncLazy.Create(async () => await GetMetaInfo(InstanceTypeUrl));
        public static AsyncLazy<string> LocalHostName = AsyncLazy.Create(async () => await GetMetaInfo(LocalHostNameUrl));
        public static AsyncLazy<string> LocalIPv4 = AsyncLazy.Create(async () => await GetMetaInfo(LocalIPv4Url));
        public static AsyncLazy<string> Mac = AsyncLazy.Create(async () => await GetMetaInfo(MacUrl));
        public static AsyncLazy<string> Metrics = AsyncLazy.Create(async () => await GetMetaInfo(MetricsUrl));
        public static AsyncLazy<Ec2MetaNetworkDetail[]> Network = AsyncLazy.Create(async () => await GetNetworkInfo());
        public static AsyncLazy<string> Placement = AsyncLazy.Create(async () => await GetMetaInfo(PlacementUrl));
        public static AsyncLazy<string> Profile = AsyncLazy.Create(async () => await GetMetaInfo(ProfileUrl));
        public static AsyncLazy<string> PublicHostName = AsyncLazy.Create(async () => await GetMetaInfo(PublicHostNameUrl));
        public static AsyncLazy<string> PublicIPv4 = AsyncLazy.Create(async () => await GetMetaInfo(PublicIPv4Url));
        public static AsyncLazy<string> PublicKeys = AsyncLazy.Create(async () => await GetMetaInfo(PublicKeysUrl));
        public static AsyncLazy<string> ReservationId = AsyncLazy.Create(async () => await GetMetaInfo(ReservationIdUrl));
        public static AsyncLazy<string[]> SecurityGroups = AsyncLazy.Create(async () => await GetMetaInfoArray(SecurityGroupsUrl));
        public static AsyncLazy<string> Services = AsyncLazy.Create(async () => await GetMetaInfo(ServicesUrl));

        /// <summary>
        /// Termination Notice will call this method.
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetTerminationNoticeAsync()
        {
            return await GetMetaInfo(SpotTerminationUrl);
        }

        // meta URL
        public const string BaseUrl = "http://169.254.169.254/latest/meta-data/";
        private const string AmiIdUrl = BaseUrl + "ami-id";
        private const string AmiLaunchIndexUrl = BaseUrl + "ami-launch-index";
        private const string AmiManifestPathUrl = BaseUrl + "ami-manifest-path";
        private const string BlockDeviceMappingUrl = BaseUrl + "block-device-mapping";
        private const string HostNameUrl = BaseUrl + "hostname";
        private const string IamUrl = BaseUrl + "hostname";
        private const string InstanceActionUrl = BaseUrl + "instance-action";
        private const string InstanceIdUrl = BaseUrl + "instance-id";
        private const string InstanceTypeUrl = BaseUrl + "instance-type";
        private const string LocalHostNameUrl = BaseUrl + "local-hostname";
        private const string LocalIPv4Url = BaseUrl + "local-ipv4";
        private const string MacUrl = BaseUrl + "mac";
        private const string MetricsUrl = BaseUrl + "metrics";
        private const string NetworkUrl = BaseUrl + "network/interfaces/macs";
        private const string PlacementUrl = BaseUrl + "placement";
        private const string ProfileUrl = BaseUrl + "profile";
        private const string PublicHostNameUrl = BaseUrl + "public-hostname";
        private const string PublicIPv4Url = BaseUrl + "public-ipv4";
        private const string PublicKeysUrl = BaseUrl + "public-keys";
        private const string ReservationIdUrl = BaseUrl + "reservation-id";
        private const string SecurityGroupsUrl = BaseUrl + "security-groups";
        private const string ServicesUrl = BaseUrl + "services";
        private const string SpotTerminationUrl = BaseUrl + "spot/termination-time";

        // reuse
        private static readonly HttpClient _client = new HttpClient();

        /// <summary>
        /// detect Ec2 Instance or not
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> TestIsEc2Instance()
        {
            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
                {
                    var response = await _client.GetAsync(BaseUrl, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static async Task<string> GetMetaInfo(string url)
        {
            // don't throw. Ec2 or not will not effect this handling. You should use Throw explictly.
            if (!await IsEc2Instance)
                return null;
            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                {
                    var response = await _client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cts.Token);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    return content;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Get Block Device Mapping
        /// </summary>
        /// <returns></returns>
        private static async Task<string[]> GetMetaInfoArray(string url)
        {
            return (await GetMetaInfo(url))?.Split(new string[] { "\n" }, StringSplitOptions.None);
        }

        /// <summary>
        /// Get Network Interface detail for each MAC Address
        /// </summary>
        /// <returns></returns>
        private static async Task<Ec2MetaNetworkDetail[]> GetNetworkInfo()
        {
            var macs = (await GetMetaInfo(NetworkUrl))?.Split(new string[] { "\n" }, StringSplitOptions.None);

            // Will not pass this path
            if (macs == null || !macs.Any())
                return null;

            // get all together.
            var tasks = macs.Select(async x => new Ec2MetaNetworkDetail
            {
                DeviceNumber = await GetMetaInfo(NetworkUrl + $"/{x}/device-number/"),
                InterfaceId = await GetMetaInfo(NetworkUrl + $"/{x}/interface-id/"),
                IPv4Associations = (await GetMetaInfo(NetworkUrl + $"{x}/ipv4-associations/"))?.Split(new string[] { "\n" }, StringSplitOptions.None),
                LocalHostName = await GetMetaInfo(NetworkUrl + $"/{x}/local-hostname/"),
                LocalIPv4s = (await GetMetaInfo(NetworkUrl + $"/{x}/local-ipv4s/"))?.Split(new string[] { "\n" }, StringSplitOptions.None),
                Mac = await GetMetaInfo(NetworkUrl + $"/{x}/mac/"),
                OwnerId = await GetMetaInfo(NetworkUrl + $"/{x}/owner-id/"),
                PublicHostName = await GetMetaInfo(NetworkUrl + $"/{x}/public-hostname/"),
                PublicIPv4s = (await GetMetaInfo(NetworkUrl + $"/{x}/public-ipv4s/"))?.Split(new string[] { "\n" }, StringSplitOptions.None),
                SecurityGrouIds = (await GetMetaInfo(NetworkUrl + $"/{x}/security-group-ids/"))?.Split(new string[] { "\n" }, StringSplitOptions.None),
                SecurityGroups = (await GetMetaInfo(NetworkUrl + $"/{x}/security-groups/"))?.Split(new string[] { "\n" }, StringSplitOptions.None),
                SubetId = await GetMetaInfo(NetworkUrl + $"/{x}/subnet-id/"),
                SubnetIPv4CidrBlock = await GetMetaInfo(NetworkUrl + $"/{x}/subnet-ipv4-cidr-block/"),
                VpcId = await GetMetaInfo(NetworkUrl + $"/{x}/vpc-id/"),
                VpcIPv4CidrBlock = await GetMetaInfo(NetworkUrl + $"/{x}/vpc-ipv4-cidr-block/"),
            }).ToArray();
            var networkDetail = await Task.WhenAll(tasks);
            return networkDetail;
        }
    }

    public class Ec2MetaNetworkDetail
    {
        public string DeviceNumber { get; set; }
        public string InterfaceId { get; set; }
        public string[] IPv4Associations { get; set; }
        public string LocalHostName { get; set; }
        public string[] LocalIPv4s { get; set; }
        public string Mac { get; set; }
        public string OwnerId { get; set; }
        public string PublicHostName { get; set; }
        public string[] PublicIPv4s { get; set; }
        public string[] SecurityGrouIds { get; set; }
        public string[] SecurityGroups { get; set; }
        public string SubetId { get; set; }
        public string SubnetIPv4CidrBlock { get; set; }
        public string VpcId { get; set; }
        public string VpcIPv4CidrBlock { get; set; }
    }
}
