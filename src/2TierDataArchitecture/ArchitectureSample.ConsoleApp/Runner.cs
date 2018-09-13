using ArchitectureSample.Core;
using ArchitectureSample.Core.Datas.Entities;
using ArchitectureSample.Core.Repositories;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ArchitectureSample.ConsoleApp
{
    public class Runner
    {
        private readonly ILogger<Runner> logger;

        public Runner(ILogger<Runner> logger)
        {
            this.logger = logger;
        }

        public async Task Startup(string[] args)
        {
            logger.LogInformation(LoggerEventIds.Info.ToInt(), "Begin Runner.");

            try
            {
                // Error handle
                ConfigureErrorHandling();

                // Initialize
                IOptions option = InitializeOption(args);

                // Help
                if (option.Help)
                {
                    return;
                }

                // Initiali Client Client
                LoadBalancerRepository loadBalancerClient = await EnvironmentRepository.IsEc2Instance()
                    ? new LoadBalancerRepository(option.Loadbalancer, option.IsAlb)
                    : new LoadBalancerRepository(option.Loadbalancer, option.IsAlb, option.Profile, option.Region);
                InstanceRepository instanceClient = await EnvironmentRepository.IsEc2Instance()
                    ? new InstanceRepository()
                    : new InstanceRepository(option.Profile, option.Region);

                // Obtain Instance list
                var instances = (await instanceClient.DescribeInstances())
                    .Where(x => x.IsTagExists("your_key", "some_value"))
                    .Where(x => x.IsTagExists("your_another_key", "some_other_value"))
                    .ToArray();

                // Obtain Load balancer instance and classify to each state.
                var loadBalancerStates = await loadBalancerClient.GetInstanceState();

                if (option.DryRun)
                {
                    await Notifier.Instance.Value.SendAsync($"[Complete] Your Dry run completed.");
                    return;
                }

                // Execute any your desired code.
            }
            catch (Exception ex)
            {
                logger.LogCritical(LoggerEventIds.Exception.ToInt(), ex.LoggerMessage());
                await Notifier.Instance.Value.SendAsync($"[Complete (ExitCode: {Environment.ExitCode})] Some error happen during task : {ex.LoggerMessage()}");
                throw;
            }
            finally
            {
                logger.LogInformation(LoggerEventIds.Info.ToInt(), "End Runner.");
            }
        }

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="args"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        private IOptions InitializeOption(string[] args)
        {
            // Get Option
            string config = $"app.json";
            IOptions configOption = EvaluateAppConfig(config);
            IOptions argumentOption = EvaluateArguments(args);

            // Mapping
            //configOption.SetLogger(logger);
            IOptions mappedOption = configOption.Map(argumentOption);

            // Show
            logger.LogDebug(LoggerEventIds.Trace.ToInt(), "Show configuration before execute.");
            logger.LogDebug(LoggerEventIds.Trace.ToInt(), mappedOption.GetStatus());

            // Logger
            Notifier.Instance.Value.SetLogger(logger, mappedOption.DryRun, mappedOption.Project);

            return mappedOption;
        }

        /// <summary>
        /// Load option from config file
        /// </summary>
        /// <param name="env"></param>
        /// <returns></returns>
        private IOptions EvaluateAppConfig(string config)
        {
            string currentDir = System.IO.Directory.GetCurrentDirectory();
            string path = Path.Combine(currentDir, config);
            if (!File.Exists(path))
            {
                throw new FileNotFoundException(path);
            }

            string jsonReader = File.ReadAllText(path);
            CommandlineOptions option = JsonConvert.DeserializeObject<CommandlineOptions>(jsonReader);
            return option;
        }

        /// <summary>
        /// Load option from command argument
        /// </summary>
        /// <param name="args"></param>
        /// <param name="option"></param>
        /// <returns></returns>
        private IOptions EvaluateArguments(string[] args)
        {
            CommandlineOptions option = new CommandlineOptions(args);
            return option;
        }

        private void ConfigureErrorHandling()
        {
            // Error Handle
            AppDomain.CurrentDomain.UnhandledException += (_sender, _e) =>
            {
                logger.LogCritical(_e.ExceptionObject as Exception, "UnhandledException");
            };

            TaskScheduler.UnobservedTaskException += (_sender, _e) =>
            {
                // catch Async unobserved Task
                logger.LogCritical(_e.Exception, "UnobservedTask");
                _e.SetObserved();
            };
        }
    }
}
