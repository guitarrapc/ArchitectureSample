namespace ArchitectureSample.Core.Datas.Entities
{
    public interface ISpotFleetCancel
    {
        bool Success { get; set; }
        ISpotFleetCancelState[] SuccessSpotFleetRequestStates { get; set; }
        ISpotFleetCancelState[] FailedSpotFleetRequestStates { get; set; }
    }

    public class SpotFleetCancel : ISpotFleetCancel
    {
        public bool Success { get; set; }
        public ISpotFleetCancelState[] SuccessSpotFleetRequestStates { get; set; }
        public ISpotFleetCancelState[] FailedSpotFleetRequestStates { get; set; }
    }
}
