using System;

namespace ArchitectureSample.Core
{
    public class ExponentialBackoff
    {
        private readonly Random random;
        private readonly double minBackoffMilliseconds;
        private readonly double maxBackoffMilliseconds;
        private readonly double deltaBackoffMilliseconds;
        private int currentPower;

        public ExponentialBackoff(TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff)
        {
            random = new Random();
            minBackoffMilliseconds = minBackoff.TotalMilliseconds;
            maxBackoffMilliseconds = maxBackoff.TotalMilliseconds;
            deltaBackoffMilliseconds = deltaBackoff.TotalMilliseconds;
        }

        public TimeSpan GetNextDelay()
        {
            var delta = (int)((System.Math.Pow(2.0, currentPower) - 1.0) * random.Next((int)(deltaBackoffMilliseconds * 0.8), (int)(deltaBackoffMilliseconds * 1.2)));
            var interval = (int)System.Math.Min(checked(minBackoffMilliseconds + delta), maxBackoffMilliseconds);

            if (interval < maxBackoffMilliseconds)
            {
                currentPower++;
            }

            return TimeSpan.FromMilliseconds(interval);
        }

        public static class Preset
        {
            /// <summary>
            /// Preset for AWS Retry : 00:00:01, 00:00:03, 00:00:07, 00:00:15, 00:00:30...
            /// </summary>
            /// <returns></returns>
            public static ExponentialBackoff AwsOperation()
            {
                return new ExponentialBackoff(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(2));
            }

            /// <summary>
            /// バックグラウンド動作用のプリセット。00:00:30, 00:01:00, 00:02:00, 00:04:30, 00:5:00...
            /// </summary>
            public static ExponentialBackoff Background()
            {
                return new ExponentialBackoff(TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(5), TimeSpan.FromSeconds(30));
            }

            /// <summary>
            /// 通常ネットワークのリトライ用プリセット。00:00:01, 00:00:02, 00:00:03, 00:00:07, 00:00:10...
            /// </summary>
            /// <returns></returns>
            public static ExponentialBackoff StandardOperation()
            {
                return new ExponentialBackoff(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1));
            }
        }
    }
}
