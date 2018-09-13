using System;
using System.Collections.Generic;
using System.Text;

namespace ArchitectureSample.Core.Datas.Entities
{
    public interface ILoadBalancerInstanceState
    {
        ILoadBalancerInstance[] All { get; set; }
        ILoadBalancerInstance[] InService { get; set; }
        ILoadBalancerInstance[] OutofService { get; set; }
    }

    public class LoadBalancerInstanceState : ILoadBalancerInstanceState
    {
        public ILoadBalancerInstance[] All { get; set; }
        public ILoadBalancerInstance[] InService { get; set; }
        public ILoadBalancerInstance[] OutofService { get; set; }
    }
}
