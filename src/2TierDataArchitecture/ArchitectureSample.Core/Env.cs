namespace ArchitectureSample.Core
{
    public enum EnvBuild
    {
        Debug,
        Release,
    }
    public static class Env
    {
#if DEBUG
        public static EnvBuild Current = EnvBuild.Debug;
#else
        public static EnvBuild Current = EnvBuild.Release;
#endif
        public static bool IsDebug => Current == EnvBuild.Debug;
    }
}
