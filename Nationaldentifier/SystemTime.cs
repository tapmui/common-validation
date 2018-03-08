using System;

namespace Collector.Common.Validation.NationalIdentifier
{
    public class SystemTime
    {
        [ThreadStatic] public static DateTimeOffset? TestTime;

        public static DateTimeOffset Current => TestTime ?? DateTimeOffset.Now;
    }
}