using System;
using System.Collections.Generic;
using System.Text;

namespace ArchitectureSample.Core.Datas.Entities
{
    public interface ILoadBalancerInstance
    {
        string InstanceId { get; set; }
        string State { get; set; }
    }

    public class LoadBalancerInstance : ILoadBalancerInstance
    {
        public string InstanceId { get; set; }
        public string State { get; set; }
    }
}
