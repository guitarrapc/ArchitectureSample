namespace ArchitectureSample.Core.Datas.Entities
{
    public interface IEc2InstanceState
    {
        bool Success { get; set; }
        string InstanceId { get; set; }
        string CurrentState { get; set; }
        string PreviousState { get; set; }
    }

    public class Ec2InstanceState : IEc2InstanceState
    {
        public bool Success { get; set; }
        public string InstanceId { get; set; }
        public string CurrentState { get; set; }
        public string PreviousState { get; set; }
    }
}
