namespace ArchitectureSample.Core
{
    public enum LoggerEventIds
    {
        Trace = 1,
        Info = 2,
        Error = 3,
        Exception = 4,
        TaskForgetUnobservedException = 5,
        UnobservedException = 6,
        UnhandledException = 7,
        Notification = 10,
    }

    public static class LoggerEventIdExtensions
    {
        public static int ToInt(this LoggerEventIds eventId)
        {
            return (int)eventId;
        }
    }
}
