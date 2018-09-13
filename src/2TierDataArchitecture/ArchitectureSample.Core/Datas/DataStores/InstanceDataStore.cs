using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.EC2;
using Amazon.EC2.Model;
using ArchitectureSample.Core.AwsApi;
using ArchitectureSample.Core.Datas.DataStores.AwsApi;
using ArchitectureSample.Core.Datas.Entities;

namespace ArchitectureSample.Core.Datas.DataStores
{
    internal class Ec2DataStore
    {
        private static IEc2Instance[] cachedDescribeInstancesAsync;
        private static ISpotFleetConfig[] cachedDescribeSpotFleetAsync;
        private static ISpotFleetHistory[] cachedDescribeSpotFleetHistoryAsync;

        #region Ec2

        public static async Task<IEc2Instance[]> DescribeInstancesAsync(bool useCache)
        {
            if (useCache && cachedDescribeInstancesAsync != null && cachedDescribeInstancesAsync.Any())
            {
                return cachedDescribeInstancesAsync;
            }

            (bool success, List<Instance> response) response = await InstanceApi.DescribeAsync();
            var success = response.success;
            IEc2Instance[] result = response.response.Select(x => new Ec2Instance().Bind(success, x))
                .OrderBy(x => Version.Parse(x.PrivateIpAddress))
                .ToArray();

            cachedDescribeInstancesAsync = result;
            return result;
        }

        public static async Task<IEc2Instance[]> DescribeInstancesAsync(IEnumerable<string> instanceIds, bool useCache)
        {
            if (useCache && cachedDescribeInstancesAsync != null && cachedDescribeInstancesAsync.Any())
            {
                return cachedDescribeInstancesAsync;
            }

            (bool success, List<Instance> response) response = await InstanceApi.DescribeAsync(instanceIds);
            var success = response.success;
            IEc2Instance[] result = response.response.Select(x => new Ec2Instance().Bind(success, x))
                .OrderBy(x => Version.Parse(x.PrivateIpAddress))
                .ToArray();

            cachedDescribeInstancesAsync = result;
            return result;
        }

        public static async Task<IEc2Instance[]> DescribeInstancesAsync(IEnumerable<ILoadBalancerInstance> elbInstances, bool useCache)
        {
            if (useCache && cachedDescribeInstancesAsync != null && cachedDescribeInstancesAsync.Any())
            {
                return cachedDescribeInstancesAsync;
            }

            (bool success, List<Instance> response) response = await InstanceApi.DescribeAsync(elbInstances.Select(x => x.InstanceId)).ConfigureAwait(false);
            IEc2Instance[] result = response.response.Join(elbInstances, inner => inner.InstanceId, outer => outer.InstanceId, (inner, outer) => new Ec2Instance().Bind(true, inner))
                .OrderBy(x => Version.Parse(x.PrivateIpAddress))
                .ToArray();

            cachedDescribeInstancesAsync = result;
            return result;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async Task<IEc2Instance[]> DescribeInstancesAsync(IEnumerable<IEc2Instance> instances, IEnumerable<ILoadBalancerInstance> elbInstances, bool useCache)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (useCache && cachedDescribeInstancesAsync != null && cachedDescribeInstancesAsync.Any())
            {
                return cachedDescribeInstancesAsync;
            }

            IEc2Instance[] result = instances.Join(elbInstances, inner => inner.InstanceId, outer => outer.InstanceId, (inner, outer) => new Ec2Instance().Bind(true, inner))
                .OrderBy(x => Version.Parse(x.PrivateIpAddress))
                .ToArray();

            cachedDescribeInstancesAsync = result;
            return result;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public static async Task<IEc2Instance[]> DescribeInstancesAsync(IEnumerable<IEc2Instance> instances, IEnumerable<string> instanceIds, bool useCache)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (useCache && cachedDescribeInstancesAsync != null && cachedDescribeInstancesAsync.Any())
            {
                return cachedDescribeInstancesAsync;
            }

            IEc2Instance[] result = instances.Join(instanceIds, inner => inner.InstanceId, outer => outer, (inner, outer) => new Ec2Instance().Bind(true, inner))
                .OrderBy(x => Version.Parse(x.PrivateIpAddress))
                .ToArray();

            cachedDescribeInstancesAsync = result;
            return result;
        }

        public static async Task<IEc2Instance[]> RequestInstancesAsync(string launchTemplateId, string templateVersion, string clientToken, string instanceType, int maxCount, int minCount, string subnetId)
        {
            var spec = new LaunchTemplateSpecification()
            {
                LaunchTemplateId = launchTemplateId,
                Version = templateVersion,
            };
            var t = InstanceType.FindValue(instanceType);
            (bool success, List<Instance> response) response = await InstanceApi.RequestAsync(spec, clientToken, maxCount, minCount, t, subnetId);
            var success = response.success;
            IEc2Instance[] result = response.response.Select(x => new Ec2Instance().Bind(success, x)).ToArray();
            return result;
        }

        public static async Task<IEc2InstanceState[]> TerminateInstancesAsync(IEnumerable<string> instanceIds)
        {
            return await TerminateInstancesAsync(instanceIds, false);
        }

        public static async Task<IEc2InstanceState[]> TerminateInstancesAsync(IEnumerable<string> instanceIds, bool force)
        {
            (bool success, List<InstanceStateChange> response) response = await InstanceApi.TerminateAsync(instanceIds, force);
            var success = response.success;
            Ec2InstanceState[] result = response.response.Select(x => new Ec2InstanceState
            {
                Success = success,
                InstanceId = x.InstanceId,
                CurrentState = x.CurrentState?.Name,
                PreviousState = x.PreviousState?.Name,
            })
            .ToArray();
            return result;
        }

        #endregion

        #region SpotFleet

        public static async Task<ISpotFleetConfig[]> DescribeSpotFleetAsync(bool useCache)
        {
            if (useCache && cachedDescribeSpotFleetAsync != null && cachedDescribeSpotFleetAsync.Any())
            {
                return cachedDescribeSpotFleetAsync;
            }

            (bool success, List<SpotFleetRequestConfig> response) response = await SpotFleetApi.DescribeRequestsAsync();
            var success = response.success;
            ISpotFleetConfig[] result = response.response.Select(x => new SpotFleetConfig().Bind(success, x)).ToArray();

            cachedDescribeSpotFleetAsync = result;
            return result;
        }

        public static async Task<ISpotFleetConfig[]> DescribeSpotFleetAsync(IEnumerable<string> spotfleetRequestIds, bool useCache)
        {
            if (useCache && cachedDescribeSpotFleetAsync != null && cachedDescribeSpotFleetAsync.Any())
            {
                return cachedDescribeSpotFleetAsync;
            }

            (bool success, List<SpotFleetRequestConfig> response) response = await SpotFleetApi.DescribeRequestsAsync(spotfleetRequestIds);
            var success = response.success;
            ISpotFleetConfig[] result = response.response.Select(x => new SpotFleetConfig().Bind(success, x)).ToArray();

            cachedDescribeSpotFleetAsync = result;
            return result;
        }

        public static async Task<ISpotFleetInstance[]> DescribeSpotFleetInstancesAsync(string spotFleetRequestId)
        {
            (bool success, List<ActiveInstance> response) response = await SpotFleetApi.DescribeInstancesAsync(spotFleetRequestId);
            var success = response.success;
            ISpotFleetInstance[] result = response.response.Select(x => new SpotFleetInstance().Bind(success, x)).ToArray();
            return result;
        }

        public static async Task<ISpotFleetInstanceLookup[]> DescribeSpotFleetInstanceLookupsAsync(bool useCache)
        {
            ISpotFleetConfig[] spotFleets = await DescribeSpotFleetAsync(useCache);
            (ISpotFleetConfig config, ISpotFleetInstance[] spotInstances)[] dic = await spotFleets.Select(async x =>
            {
                ISpotFleetInstance[] result = await DescribeSpotFleetInstancesAsync(x.SpotFleetRequestId);
                return (config: x, spotInstances: result);
            }).WhenAll();
            ISpotFleetInstanceLookup[] lookup = dic.SelectMany(x => x.spotInstances.Select(y => new SpotFleetInstanceLookup().Bind(x.config.SpotFleetRequestId, y))).ToArray();
            return lookup;
        }

        public static async Task<ISpotFleetHistory[]> DescribeSpotFleetHistoryAsync(string spotFleetRequestId, DateTime starttime, bool useCache)
        {
            if (useCache && cachedDescribeSpotFleetHistoryAsync != null && cachedDescribeSpotFleetHistoryAsync.Any())
            {
                return cachedDescribeSpotFleetHistoryAsync;
            }

            (bool success, List<HistoryRecord> response) response = await SpotFleetApi.DescribeRequestHistoryAsync(spotFleetRequestId, starttime);
            var success = response.success;
            ISpotFleetHistory[] result = response.response.Select(x => new SpotFleetHistory().Bind(success, x))
                .OrderBy(x => x.InstanceId)
                .ThenBy(x => x.Timestamp)
                .ToArray();

            cachedDescribeSpotFleetHistoryAsync = result;
            return result;
        }

        public static async Task<ISpotFleetRequest> RequestSpotFleetAsync(string launchTemplateId, string templateVersion, string clientToken, string iamFeetRole, string[] instanceTypes, int targetCapacity, string subnetId, string allocationStorage)
        {
            var spec = new FleetLaunchTemplateSpecification()
            {
                LaunchTemplateId = launchTemplateId,
                Version = templateVersion,
            };
            var overwrites = instanceTypes.Select(x => new LaunchTemplateOverrides
            {
                InstanceType = InstanceType.FindValue(x),
                SubnetId = subnetId,
                WeightedCapacity = 1,
            })
            .ToList();
            var storategy = AllocationStrategy.FindValue(allocationStorage);

            (bool success, string spotFleetRequestId) response = await SpotFleetApi.RequestAsync(spec, overwrites, clientToken, iamFeetRole, targetCapacity, storategy);
            var success = response.success;
            var result = new SpotFleetRequest() { Success = success, SpotFleetRequestId = response.spotFleetRequestId };
            return result;
        }

        public static async Task<bool> ModifySpotFleetCapacityAsync(string spotFleetRequestId, int capacity)
        {
            (bool success, ModifySpotFleetRequestResponse response) response = await SpotFleetApi.ModifyRequestAsync(spotFleetRequestId, capacity);
            var result = response.success && response.response.Return;

            return result;
        }

        public static async Task<ISpotFleetCancel> CancelSpotFleetAsync(IEnumerable<string> spotFleetRequestIds)
        {
            (bool success, CancelSpotFleetRequestsResponse response) response = await SpotFleetApi.CancelRequestAsync(spotFleetRequestIds);
            var success = response.success;
            var result = new SpotFleetCancel()
            {
                Success = success,
                SuccessSpotFleetRequestStates = response.response.SuccessfulFleetRequests.Select(x => new SpotFleetCancelState()
                {
                    SpotFleetRequestId = x.SpotFleetRequestId,
                    CurrentState = x.CurrentSpotFleetRequestState,
                    PreviousState = x.PreviousSpotFleetRequestState,
                }).ToArray(),
            };

            return result;
        }

        #endregion
    }
}
