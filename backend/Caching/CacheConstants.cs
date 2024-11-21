namespace Caching
{
    public class KeyCacheConstants
    {
        public const string Prefix = "v1";

        /// <summary>
        /// 0: keyword
        /// 1: url
        /// 2: browser
        /// </summary>

        public const string SearchKey = "Search_{0}_{1}_{2}";

        public const string SupportBrowsers = "SupportBrowsers";

    }

    public class TimeCacheConstants
    {
        public static TimeSpan FiveMinutes => TimeSpan.FromMinutes(5);

        public static TimeSpan DateHour => TimeSpan.FromHours(1);

        public static TimeSpan DateDay => TimeSpan.FromHours(23);

        public static TimeSpan DateWeek => TimeSpan.FromDays(7);

        public static TimeSpan DateMonth => TimeSpan.FromDays(30);

        public static TimeSpan DateNull => TimeSpan.FromMinutes(5);
    }
}
