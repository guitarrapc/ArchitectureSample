using System;
using Amazon.EC2.Model;

namespace ArchitectureSample.Core.Datas.Entities
{
    public interface ISpotFleetConfig
    {
        bool Success { get; set; }
        string SpotFleetRequestId { get; set; }
        string SpotFleetRequestState { get; set; }
        string ActivityStatus { get; set; }
        int TargetCapacity { get; set; }
        DateTime CreateTime { get; set; }
    }

    public class SpotFleetConfig : ISpotFleetConfig
    {
        public bool Success { get; set; }
        public string SpotFleetRequestId { get; set; }
        public string SpotFleetRequestState { get; set; }
        public string ActivityStatus { get; set; }
        public int TargetCapacity { get; set; }
        public DateTime CreateTime { get; set; }

        public ISpotFleetConfig Bind(bool success, SpotFleetRequestConfig x)
        {
            Success = success;
            SpotFleetRequestId = x.SpotFleetRequestId;
            SpotFleetRequestState = x.SpotFleetRequestState?.Value;
            ActivityStatus = x.ActivityStatus?.Value;
            CreateTime = x.CreateTime;
            TargetCapacity = x.ConfigData.TargetCapacity;
            return this;
        }
    }
}
