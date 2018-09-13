using Amazon.EC2.Model;

namespace ArchitectureSample.Core.Datas.Entities
{
    public interface ISpotFleetInstance
    {
        string InstanceHealth { get; set; }
        string InstanceId { get; set; }
        string InstanceType { get; set; }
        string SpotInstanceRequestId { get; set; }
    }

    public class SpotFleetInstance : ISpotFleetInstance
    {
        public string InstanceHealth { get; set; }
        public string InstanceId { get; set; }
        public string InstanceType { get; set; }
        public string SpotInstanceRequestId { get; set; }

        public ISpotFleetInstance Bind(bool success, ActiveInstance x)
        {
            InstanceHealth = x.InstanceHealth.Value;
            InstanceId = x.InstanceId;
            InstanceType = x.InstanceType;
            SpotInstanceRequestId = x.SpotInstanceRequestId;
            return this;
        }
    }
}
