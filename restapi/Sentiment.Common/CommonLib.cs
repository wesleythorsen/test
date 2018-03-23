using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Common
{
    public static class TimeLibrary
    {
        public enum TimeInterval
        {
            Hour,
            Day,
            Week,
            Month,
            Year
        }

        public static TimeInterval StringToTimeInterval(string s)
        {
            TimeInterval interval;
            switch (s)
            {
                case "h":
                    interval = TimeInterval.Hour;
                    break;
                case "d":
                    interval = TimeInterval.Day;
                    break;
                case "w":
                    interval = TimeInterval.Week;
                    break;
                case "m":
                    interval = TimeInterval.Month;
                    break;
                case "y":
                    interval = TimeInterval.Year;
                    break;
                default:
                    interval = TimeInterval.Day; // default to day if string is invalid
                    break;
            }

            return interval;
        }
        
        public static DateTime Floor(this DateTime time, TimeInterval interval)
        {
            var span = interval.ToTimeSpan();
            return new DateTime(time.Ticks - (time.Ticks % span.Ticks));
        }

        public static TimeSpan ToTimeSpan(this TimeInterval interval)
        {
            TimeSpan span;
            switch (interval)
            {
                case TimeInterval.Hour:
                    span = TimeSpan.FromHours(1);
                    break;
                case TimeInterval.Day:
                    span = TimeSpan.FromDays(1);
                    break;
                case TimeInterval.Week:
                    span = TimeSpan.FromDays(7);
                    break;
                case TimeInterval.Month:
                    span = TimeSpan.FromDays(30);
                    break;
                case TimeInterval.Year:
                    span = TimeSpan.FromDays(365);
                    break;
                default:
                    span = TimeSpan.FromDays(1); // default to day if string is invalid
                    break;
            }

            return span;
        }

    }
}
