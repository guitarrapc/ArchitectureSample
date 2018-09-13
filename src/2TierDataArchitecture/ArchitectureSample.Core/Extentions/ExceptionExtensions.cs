using System;
using System.Diagnostics;

namespace ArchitectureSample.Core
{
    public static class ExceptionExtensions
    {
        public static string LoggerMessage<T>(this T ex) where T: Exception
        {
            var demystify = ex.Demystify();
            return $"{ex.GetType().FullName} {demystify.Message} {demystify.StackTrace}";
        }
    }
}
