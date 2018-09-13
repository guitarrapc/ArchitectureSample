using System.Collections.Generic;
using System.Linq;
using Amazon.ElasticLoadBalancing.Model;
using Amazon.ElasticLoadBalancingV2.Model;

namespace ArchitectureSample.Core.Entities
{
    public class LoadbalancerAttribute : ILoadbalancerAttribute
    {
        public bool Success { get; set; }
        public bool ConnectionDrainingEnabled { get; set; }
        public int ConnectionDrainingTimeout { get; set; }

        public ILoadbalancerAttribute Bind(bool success, LoadBalancerAttributes x)
        {
            Success = success;
            ConnectionDrainingEnabled = x.ConnectionDraining.Enabled;
            ConnectionDrainingTimeout = x.ConnectionDraining.Timeout;
            return this;
        }

        public ILoadbalancerAttribute Bind(bool success, IEnumerable<TargetGroupAttribute> x)
        {
            Success = success;
            ConnectionDrainingEnabled = true;
            ConnectionDrainingTimeout = int.Parse(x.FirstOrDefault(y => y.Key == "deregistration_delay.timeout_seconds")?.Value ?? "15");
            return this;
        }
    }

    public interface ILoadbalancerAttribute
    {
        bool Success { get; set; }
        bool ConnectionDrainingEnabled { get; set; }
        int ConnectionDrainingTimeout { get; set; }
    }
}
