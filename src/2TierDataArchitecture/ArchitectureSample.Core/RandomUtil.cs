namespace ArchitectureSample.Core
{
    using System;

    public static class RandomUtil
    {
        [ThreadStatic]
        private static Random random;

        static RandomUtil()
        {
            RandomFactory = () =>
            {
                using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
                {
                    var buffer = new byte[sizeof(int)];
                    rng.GetBytes(buffer);
                    var seed = BitConverter.ToInt32(buffer, 0);
                    return new Random(seed);
                }
            };
        }

        /// <summary>
        /// Generate Random instance Factory
        /// </summary>
        public static Func<Random> RandomFactory { get; set; }

        /// <summary>
        /// Generate Thread Unique Random instance
        /// </summary>
        public static Random ThreadRandom
        {
            get
            {
                if (random == null)
                {
                    random = RandomFactory();
                }

                return random;
            }
        }
    }
}
