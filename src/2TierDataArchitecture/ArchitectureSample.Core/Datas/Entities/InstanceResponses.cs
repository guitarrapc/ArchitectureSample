namespace ArchitectureSample.Core.Datas.Entities
{
    public class SpotFleetCancelState : ISpotFleetCancelState
    {
        public string SpotFleetRequestId { get; set; }
        public string CurrentState { get; set; }
        public string PreviousState { get; set; }
    }

    public interface ISpotFleetCancelState
    {
        string SpotFleetRequestId { get; set; }
        string CurrentState { get; set; }
        string PreviousState { get; set; }
    }


}
