using ArchitectureSample.Core;
using Microsoft.Extensions.Logging;
using Mono.Options;
using System;
using System.Linq;

namespace ArchitectureSample.ConsoleApp
{
    /// <summary>
    /// All parameters are not required for arguments.
    /// </summary>
    public class CommandlineOptions : IOptions
    {
        public bool Help { get; set; }
        public bool DryRun { get; set; }

        public string Project { get; set; }

        public string Loadbalancer { get; set; }
        public bool IsAlb { get; set; }

        // Chat Notification
        public string FunctionUrl { get; set; }
        public string Channel { get; set; }

        // No EC2 Instance only parameter
        public string Profile { get; set; }
        public string Region { get; set; }

        private static readonly string format = "Map value {0}. with: {1} by: {2}";
        private ILogger _logger;

        public CommandlineOptions() { }

        public CommandlineOptions(string[] args)
        {
            if (!args.Any())
            {
                return;
            }

            var options = new OptionSet()
            {
                { "h|help", "Show help", v => Help = v != null },
                { "d|dryrun|dryRun", "Run as dry-run", v => DryRun = v != null },
                { "p|project=", "Project Name to notify.", v => Project = v },
                { "loadbalancer=", "Loadbalancer name to attach on Scale-out.", v => Loadbalancer = v },
                { "isAlb", "Input if your LoadBalancer is ALB", v => IsAlb = v != null},
                { "functionUrl=", "AzureFunctions url for notify trigger.", v => FunctionUrl = v },
                { "channel=", "Slack Channel to send message.", v => Channel = v },
                { "profile=", "AWS Profile Name to use in local. (Not in use on EC2)", v => Profile = v },
                { "region=", "AWS region to use in local. (Not in use on EC2)", v => Region = v },
            };

            var result = options.Parse(args);
            if (Help)
            {
                options?.WriteOptionDescriptions(Console.Out);
            }
        }

        public void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Map value
        /// </summary>
        /// <param name="overrideOption"></param>
        /// <returns></returns>
        public IOptions Map(IOptions overrideOption)
        {
            return new CommandlineOptions()
            {
                Help = MapCore(nameof(Help), Help, overrideOption.Help),
                DryRun = MapCore(nameof(DryRun), DryRun, overrideOption.DryRun),
                Project = MapCore(nameof(Project), Project, overrideOption.Project),
                FunctionUrl = MapCore(nameof(FunctionUrl), FunctionUrl, overrideOption.FunctionUrl),
                Loadbalancer = MapCore(nameof(Loadbalancer), Loadbalancer, overrideOption.Loadbalancer),
                IsAlb = MapCore(nameof(IsAlb), IsAlb, overrideOption.IsAlb),
                Channel = MapCore(nameof(Channel), Channel, overrideOption.Channel),
                Profile = MapCore(nameof(Profile), Profile, overrideOption.Profile),
                Region = MapCore(nameof(Region), Region, overrideOption.Region),
            };
        }
        private string MapCore(string name, string option, string overrideOption)
        {
            if (string.IsNullOrWhiteSpace(overrideOption))
            {
                return option;
            }
            _logger?.LogDebug(LoggerEventIds.Trace.ToInt(), format, name, option, overrideOption);
            return overrideOption;
        }
        private int MapCore(string name, int option, int overrideOption)
        {
            if (overrideOption == 0)
            {
                return option;
            }
            _logger?.LogDebug(LoggerEventIds.Trace.ToInt(), format, name, option, overrideOption);
            return overrideOption;
        }
        private bool MapCore(string name, bool option, bool overrideOption)
        {
            if (!overrideOption)
            {
                return option;
            }
            _logger?.LogDebug(LoggerEventIds.Trace.ToInt(), format, name, option, overrideOption);
            return overrideOption;
        }

        public string GetStatus()
        {
            return $@"Overridable prameters
----------------------------------
{nameof(DryRun)}               : {DryRun}
{nameof(Project)}              : {Project}
{nameof(FunctionUrl)}          : {FunctionUrl}
{nameof(Loadbalancer)}         : {Loadbalancer}
{nameof(IsAlb)}                : {IsAlb}
{nameof(Channel)}              : {Channel}
{nameof(Profile)}              : {Profile}
{nameof(Region)}               : {Region}";
        }
    }
}
