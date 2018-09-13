namespace ArchitectureSample.Core.Datas.Entities
{
    public interface ISpotFleetInstanceLookup
    {
        string SpotInstanceRequestId { get; set; }
        ISpotFleetInstance SpotFleetInstance { get; set; }
    }

    public class SpotFleetInstanceLookup : ISpotFleetInstanceLookup
    {
        public string SpotInstanceRequestId { get; set; }
        public ISpotFleetInstance SpotFleetInstance { get; set; }

        public ISpotFleetInstanceLookup Bind(string spotInstanceRequestId, ISpotFleetInstance spotFleetInstance)
        {
            SpotInstanceRequestId = spotInstanceRequestId;
            SpotFleetInstance = spotFleetInstance;
            return this;
        }
    }
}
