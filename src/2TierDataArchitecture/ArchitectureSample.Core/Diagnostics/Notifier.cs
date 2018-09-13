using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ArchitectureSample.Core
{
    public class Notifier
    {
        private static readonly ExponentialBackoff backoff = ExponentialBackoff.Preset.StandardOperation();
        private static readonly int RetryCount = 10;
        private static ILogger _logger;
        private static bool _dryRun;
        private static string _project;

        public static Lazy<Notifier> Instance = new Lazy<Notifier>(() => instance);
        private static Notifier instance = null;

        public string Channel { get; }
        public string Text { get; private set; }
        public string UserName { get; } = "Azure Functions Bot";
        public string IconUrl { get; } = "https://pbs.twimg.com/media/CfAg9HtVIAAbKD8.jpg";

        private readonly string _functionUrl;

        public Notifier(string channel, string functionUrl)
        {
            Channel = channel;
            _functionUrl = functionUrl;
        }

        public Notifier(string channel, string functionUrl, string userName)
            : this(channel, functionUrl)
        {
            UserName = userName;
        }

        public Notifier(string channel, string functionUrl, string userName, string iconUrl)
            : this(channel, functionUrl, userName)
        {
            IconUrl = iconUrl;
        }

        public static void CreateClient(string channel, string functionUrl)
        {
            var notifier = new Notifier(channel, functionUrl);
            instance = notifier;
        }

        public void SetLogger(ILogger logger, bool dryRun, string project)
        {
            _logger = logger;
            _dryRun = dryRun;
            _project = project;
        }

        /// <summary>
        /// メッセージを送信します。
        /// </summary>
        /// <returns></returns>
        public async Task<Notifier> SendAsync(string text)
        {
            Text = text;

            Notifier result = null;
            var current = 0;
            do
            {
                using (var sendTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                using (var retryTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(5)))
                using (var client = new HttpClient())
                {
                    var payload = new
                    {
                        reciever = "slack",
                        channel = Channel,
                        text = Text,
                        username = UserName,
                        icon_url = IconUrl,
                    };
                    var jsonString = JsonConvert.SerializeObject(payload);

                    try
                    {
                        using (var res = await client.PostAsync(_functionUrl, new StringContent(jsonString, Encoding.UTF8, "application/json"), sendTokenSource.Token))
                        {
                            if (res.IsSuccessStatusCode)
                                break;

                            // retry on fail
                            current++;
                            await System.Threading.Tasks.Task.Delay(backoff.GetNextDelay(), retryTokenSource.Token);
                        }
                    }
                    catch (Exception)
                    {
                        current++;
                    }
                }
            } while (current < RetryCount);
            return result;
        }
    }
}
