using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;
using ArchitectureSample.Core.Datas.DataStores.AwsApi;

namespace ArchitectureSample.Core.AwsApi
{
    internal static class SpotFleetApi
    {
        public static async Task<(bool success, List<LaunchTemplate> response)> DescribeLaunchTemplatesAsync()
        {
            // Id / Name のフィルタは外でご自由に
            var request = new DescribeLaunchTemplatesRequest();
            return await DescribeLaunchTemplatesAsync(request);
        }

        public static async Task<(bool success, List<LaunchTemplate> response)> DescribeLaunchTemplatesAsync((IEnumerable<string> launchTemplateIds, IEnumerable<string> launchTemplateNames) requestKey)
        {
            var request = new DescribeLaunchTemplatesRequest();
            if (requestKey.launchTemplateIds != null && requestKey.launchTemplateIds.Any())
            {
                request.LaunchTemplateIds = requestKey.launchTemplateIds.ToList();
            }
            if (requestKey.launchTemplateNames != null && requestKey.launchTemplateNames.Any())
            {
                request.LaunchTemplateNames = requestKey.launchTemplateNames.ToList();
            }

            return await DescribeLaunchTemplatesAsync(request);
        }

        private static async Task<(bool success, List<LaunchTemplate> response)> DescribeLaunchTemplatesAsync(DescribeLaunchTemplatesRequest request)
        {
            var responses = new List<LaunchTemplate>();
            DescribeLaunchTemplatesResponse response = null;
            do
            {
                response = await SingletonEc2InstanceClient.Instance.DescribeLaunchTemplatesAsync(request);
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                    break;
                responses.AddRange(response.LaunchTemplates);
            }
            while (!string.IsNullOrEmpty(response.NextToken));
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, responses);
        }

        // INFO: CreateLaunchTemplateAsync はコンソールでどうぞ
        // INFO: CreateLaunchTemplateVersionAsync はコンソールでどうぞ

        public static async Task<(bool success, List<LaunchTemplateVersion> response)> DescribeLaunchTemplateVersionsAsync((string launchTemplateId, string launchTemplateName) requestKey)
        {
            var request = new DescribeLaunchTemplateVersionsRequest();
            if (!string.IsNullOrEmpty(requestKey.launchTemplateId))
            {
                request.LaunchTemplateId = requestKey.launchTemplateId;
            }
            if (!string.IsNullOrEmpty(requestKey.launchTemplateName))
            {
                request.LaunchTemplateId = requestKey.launchTemplateName;
            }

            var responses = new List<LaunchTemplateVersion>();
            DescribeLaunchTemplateVersionsResponse response = null;
            do
            {
                response = await SingletonEc2InstanceClient.Instance.DescribeLaunchTemplateVersionsAsync(request);
                responses.AddRange(response.LaunchTemplateVersions);
            }
            while (!string.IsNullOrEmpty(response.NextToken));
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, responses);
        }

        public static async Task<(bool success, List<SpotFleetRequestConfig> response)> DescribeRequestsAsync()
        {
            var responses = new List<SpotFleetRequestConfig>();
            DescribeSpotFleetRequestsResponse response = null;

            do
            {
                response = await SingletonEc2InstanceClient.Instance.DescribeSpotFleetRequestsAsync(new DescribeSpotFleetRequestsRequest());
                responses.AddRange(response.SpotFleetRequestConfigs);
            }
            while (!string.IsNullOrEmpty(response.NextToken));
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, responses);
        }

        public static async Task<(bool success, List<SpotFleetRequestConfig> response)> DescribeRequestsAsync(IEnumerable<string> spotfleetRequestIds)
        {
            var responses = new List<SpotFleetRequestConfig>();
            DescribeSpotFleetRequestsResponse response = null;

            do
            {
                response = await SingletonEc2InstanceClient.Instance.DescribeSpotFleetRequestsAsync(new DescribeSpotFleetRequestsRequest() { SpotFleetRequestIds = spotfleetRequestIds.ToList() });
                responses.AddRange(response.SpotFleetRequestConfigs);
            }
            while (!string.IsNullOrEmpty(response.NextToken));
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, responses);
        }

        public static async Task<(bool success, List<ActiveInstance> response)> DescribeInstancesAsync(string spotFleetRequestId)
        {
            var responses = new List<ActiveInstance>();
            DescribeSpotFleetInstancesResponse response = null;

            do
            {
                response = await SingletonEc2InstanceClient.Instance.DescribeSpotFleetInstancesAsync(new DescribeSpotFleetInstancesRequest()
                {
                    SpotFleetRequestId = spotFleetRequestId,
                });
                responses.AddRange(response.ActiveInstances);
            }
            while (!string.IsNullOrEmpty(response.NextToken));
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, responses);
        }

        public static async Task<(bool success, string spotFleetRequestId)> RequestAsync(FleetLaunchTemplateSpecification launchTemplateSpec, List<LaunchTemplateOverrides> launchTemplateOverrides, string clientToken, string iamFleetRole, int targetCapacity, AllocationStrategy allocationStrategy)
        {
            RequestSpotFleetResponse response = await SingletonEc2InstanceClient.Instance.RequestSpotFleetAsync(new Amazon.EC2.Model.RequestSpotFleetRequest()
            {
                SpotFleetRequestConfig = new Amazon.EC2.Model.SpotFleetRequestConfigData
                {
                    ClientToken = clientToken,
                    IamFleetRole = iamFleetRole,
                    AllocationStrategy = allocationStrategy,
                    ReplaceUnhealthyInstances = true,
                    TargetCapacity = targetCapacity,
                    TerminateInstancesWithExpiration = true,
                    Type = FleetType.Maintain,
                    LaunchTemplateConfigs = new List<LaunchTemplateConfig>()
                    {
                        new LaunchTemplateConfig()
                        {
                            LaunchTemplateSpecification = launchTemplateSpec,
                            Overrides = launchTemplateOverrides,
                        }
                    },
                },
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response.SpotFleetRequestId);
        }

        public static async Task<(bool success, ModifySpotFleetRequestResponse response)> ModifyRequestAsync(string spotFleetRequestId, int targetCapacity)
        {
            ModifySpotFleetRequestResponse response = await SingletonEc2InstanceClient.Instance.ModifySpotFleetRequestAsync(new ModifySpotFleetRequestRequest()
            {
                SpotFleetRequestId = spotFleetRequestId,
                TargetCapacity = targetCapacity,
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response);
        }

        public static async Task<(bool success, List<HistoryRecord> response)> DescribeRequestHistoryAsync(string spotFleetRequestId, DateTime startTime)
        {
            var responses = new List<HistoryRecord>();
            DescribeSpotFleetRequestHistoryResponse response = null;

            do
            {
                response = await SingletonEc2InstanceClient.Instance.DescribeSpotFleetRequestHistoryAsync(new DescribeSpotFleetRequestHistoryRequest()
                {
                    SpotFleetRequestId = spotFleetRequestId,
                    StartTime = startTime,
                });
                responses.AddRange(response.HistoryRecords);
            }
            while (!string.IsNullOrEmpty(response.NextToken));
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, responses);
        }

        public static async Task<(bool success, CancelSpotFleetRequestsResponse response)> CancelRequestAsync(IEnumerable<string> spotFleetRequestId)
        {
            CancelSpotFleetRequestsResponse response = await SingletonEc2InstanceClient.Instance.CancelSpotFleetRequestsAsync(new CancelSpotFleetRequestsRequest()
            {
                SpotFleetRequestIds = spotFleetRequestId.ToList(),
            });
            return (response.HttpStatusCode == System.Net.HttpStatusCode.OK, response);
        }
    }
}
