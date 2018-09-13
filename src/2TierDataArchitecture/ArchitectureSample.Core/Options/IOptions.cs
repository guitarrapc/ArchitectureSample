using Microsoft.Extensions.Logging;

namespace ArchitectureSample
{
    public interface IOptions
    {
        bool Help { get; set; }
        bool DryRun { get; set; }
        string Project { get; set; }
        string Loadbalancer { get; set; }
        bool IsAlb { get; set; }
        string FunctionUrl { get; set; }
        string Channel { get; set; }
        string Profile { get; set; }
        string Region { get; set; }

        void SetLogger(ILogger logger);
        IOptions Map(IOptions overrideOption);
        string GetStatus();
    }
}
