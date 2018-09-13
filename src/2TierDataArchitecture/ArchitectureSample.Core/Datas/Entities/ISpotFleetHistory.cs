using System;
using Amazon.EC2.Model;

namespace ArchitectureSample.Core.Datas.Entities
{
    public interface ISpotFleetHistory
    {
        bool Success { get; set; }
        string EventDescription { get; set; }
        string EventSubType { get; set; }
        string InstanceId { get; set; }
        string EventType { get; set; }
        DateTime Timestamp { get; set; }
    }

    public class SpotFleetHistory : ISpotFleetHistory
    {
        public bool Success { get; set; }
        public string EventDescription { get; set; }
        public string EventSubType { get; set; }
        public string InstanceId { get; set; }
        public string EventType { get; set; }
        public DateTime Timestamp { get; set; }

        public ISpotFleetHistory Bind(bool success, HistoryRecord x)
        {
            Success = success;
            EventDescription = x.EventInformation.EventDescription;
            EventSubType = x.EventInformation.EventSubType;
            InstanceId = x.EventInformation.InstanceId;
            EventType = x.EventType?.Value;
            Timestamp = x.Timestamp;
            return this;
        }
    }
}
