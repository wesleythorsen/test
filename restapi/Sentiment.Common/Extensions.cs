using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sentiment.Common
{
    public static class Extensions
    {
        public static IEnumerable<IGrouping<DateTime, TSource>> GroupByTime<TSource>(this IEnumerable<TSource> source, 
            Func<TSource, DateTime> selector, TimeSpan interval)
        {
            long t1 = source.Min(e => selector(e).Ticks);

            var groups = source.GroupBy(e =>
            {
                // group# = floor((t - t1) / interval)
                int groupNumber = (int) ((selector(e).Ticks - t1) / interval.Ticks);

                    DateTime dateGroup = new DateTime(t1 + (groupNumber * interval.Ticks));

                    return dateGroup;
                });

            return groups;
        }

        public static IEnumerable<IGrouping<DateTime, TSource>> GroupByTime<TSource>(this IEnumerable<TSource> source,
            Func<TSource, DateTime> selector, TimeLibrary.TimeInterval interval)
        {
            var firstTime = new DateTime(source.Min(e => selector(e).Ticks));

            //var t1 = firstTime.Floor(interval).Ticks;

            var groups = source.GroupBy(e =>
            {
                // group# = floor((t - t1) / interval)
                //int groupNumber = (int)((selector(e).Ticks - t1) / interval.Ticks);

                //DateTime dateGroup = new DateTime(t1 + (groupNumber * interval.Ticks));

                DateTime dateGroup = selector(e).Floor(interval);
                
                return dateGroup;
            });

            return groups;
        }

        public static bool IsValidSearchTerm(this string str)
        {
            foreach(var c in str)
            {
                if (char.IsLetterOrDigit(c)) continue;
                if (c == ' ') continue;
                if (c == '#') continue;
                return false;
            }

            return true;
        }

        public static int CountOccurances(this string str, string substring)
        {
            int count = 0;
            int pos = 0;
            while(true)
            {
                pos = str.IndexOf(substring, pos + substring.Length);
                if (pos == -1) break;
                count++;
            }
            return count;
        }
    }
}
