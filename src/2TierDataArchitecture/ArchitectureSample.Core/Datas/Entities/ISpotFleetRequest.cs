namespace ArchitectureSample.Core.Datas.Entities
{
    public interface ISpotFleetRequest
    {
        bool Success { get; set; }
        string SpotFleetRequestId { get; set; }
    }

    public class SpotFleetRequest : ISpotFleetRequest
    {
        public bool Success { get; set; }
        public string SpotFleetRequestId { get; set; }
    }
}
