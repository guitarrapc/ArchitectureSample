using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchitectureSample.Core.Datas.DataStores;
using ArchitectureSample.Core.Datas.DataStores.AwsApi;
using ArchitectureSample.Core.Datas.Entities;

namespace ArchitectureSample.Core.Repositories
{
    public class InstanceRepository
    {
        public InstanceRepository()
        {
            SingletonEc2InstanceClient.Initialize();
        }

        public InstanceRepository(string profile, string region)
        {
            SingletonEc2InstanceClient.Initialize(profile, region);
        }

        #region Instance
        public async Task<IEc2Instance[]> DescribeInstances(bool useCache = false)
        {
            return await Ec2DataStore.DescribeInstancesAsync(useCache);
        }

        public async Task<IEc2Instance[]> DescribeInstances(IEnumerable<string> instanceIds, bool useCache = false)
        {
            return await Ec2DataStore.DescribeInstancesAsync(instanceIds, useCache);
        }

        public async Task<IEc2Instance[]> DescribeInstances(IEnumerable<ILoadBalancerInstance> loadBalancerInstances, bool useCache = false)
        {
            return await Ec2DataStore.DescribeInstancesAsync(loadBalancerInstances, useCache);
        }

        public async Task<IEc2Instance[]> DescribeInstances(IEnumerable<IEc2Instance> instances, IEnumerable<ILoadBalancerInstance> loadBalancerInstances, bool useCache = false)
        {
            return await Ec2DataStore.DescribeInstancesAsync(instances, loadBalancerInstances, useCache);
        }

        public async Task<IEc2Instance[]> DescribeInstances(IEnumerable<IEc2Instance> instances, IEnumerable<string> instanceIds, bool useCache = false)
        {
            return await Ec2DataStore.DescribeInstancesAsync(instances, instanceIds, useCache);
        }

        public async Task<IEc2Instance[]> RequestInstance(string launchTemplateId, string templateVersion, string clientToken, string instanceType, int maxCount, int minCount, string subnetId)
        {
            return await Ec2DataStore.RequestInstancesAsync(launchTemplateId, templateVersion, clientToken, instanceType, maxCount, minCount, subnetId);
        }

        public async Task<IEc2InstanceState[]> CancelInstance(IEnumerable<string> instanceIds, bool force = false)
        {
            return await Ec2DataStore.TerminateInstancesAsync(instanceIds, force);
        }

        #endregion

        #region SpotFleet
        public async Task<ISpotFleetConfig[]> DescribeSpotFleet(bool cache = false)
        {
            return await Ec2DataStore.DescribeSpotFleetAsync(cache);
        }

        public async Task<ISpotFleetConfig[]> DescribeSpotFleet(IEnumerable<string> spotfleetRequestIds, bool cache = false)
        {
            return await Ec2DataStore.DescribeSpotFleetAsync(spotfleetRequestIds, cache);
        }

        public async Task<ISpotFleetInstance[]> DescribeSpotFleetInstances(string spotFleetRequestId)
        {
            return await Ec2DataStore.DescribeSpotFleetInstancesAsync(spotFleetRequestId);
        }

        public async Task<ISpotFleetInstanceLookup[]> DescribeSpotFleetInstanceLookups(bool cache = false)
        {
            return await Ec2DataStore.DescribeSpotFleetInstanceLookupsAsync(cache);
        }

        public async Task<ISpotFleetHistory[]> DescribeSpotFleetHistory(string spotFleetRequestId, DateTime datetime, bool useCache = false)
        {
            return await Ec2DataStore.DescribeSpotFleetHistoryAsync(spotFleetRequestId, datetime, useCache);
        }

        public async Task<ISpotFleetHistory[]> DescribeSpotFleetHistory(string spotFleetRequestId, DateTime datetime, string eventSubType, bool useCache = false)
        {
            ISpotFleetHistory[] history = await Ec2DataStore.DescribeSpotFleetHistoryAsync(spotFleetRequestId, datetime, useCache);
            ISpotFleetHistory[] result = history.Where(x => x.Success).Where(x => x.EventSubType == eventSubType).ToArray();
            return result;
        }

        public async Task<bool> ModifySpotFleetCapacity(string spotFleetRequestId, int capacity)
        {
            return await Ec2DataStore.ModifySpotFleetCapacityAsync(spotFleetRequestId, capacity);
        }

        public async Task<ISpotFleetRequest> RequestSpotFleet(string launchTemplateId, string templateVersion, string clientToken, string iamFleetId, string[] instanceTypes, int targetCapcity, string subnetId, string allocationStrategy)
        {
            return await Ec2DataStore.RequestSpotFleetAsync(launchTemplateId, templateVersion, clientToken, iamFleetId, instanceTypes, targetCapcity, subnetId, allocationStrategy);
        }

        public async Task<ISpotFleetCancel> CancelSpotFleet(IEnumerable<string> spotfleetRequestId)
        {
            return await Ec2DataStore.CancelSpotFleetAsync(spotfleetRequestId);
        }

        #endregion
    }

    public class InstanceCleanupParameter
    {
        public string[] OndemandIstanceIds { get; set; }
        public string SpotFleetRequestId { get; set; }
        public int SpotFleetTargetCapacity { get; set; }
    }
}
