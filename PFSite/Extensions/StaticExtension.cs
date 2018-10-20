using System;
using System.Collections.Generic;
using System.Linq;

namespace PFSite.Extensions
{
    public static class StaticExtension
    {
        /// <summary>
        /// 计算对应日期的一周开始
        /// </summary>
        /// <param name="startDay">默认一周开始为周一</param>
        public static DateTime StartOfWeek(this DateTime dateTime, DayOfWeek startDay = DayOfWeek.Monday)
        {
            int diff = (7 + (dateTime.DayOfWeek - startDay)) % 7;
            return dateTime.AddDays(-1 * diff).Date;
        }

        /// <summary>
        /// 实现TimeSpan的累加求和
        /// </summary>
        /// <returns>总时间差</returns>
        public static TimeSpan Sum<TSource>(this IEnumerable<TSource> source, Func<TSource, TimeSpan?> selector)
        {
            return source.Select(selector).Aggregate(TimeSpan.Zero, (t1, t2) => t1 + t2 ?? TimeSpan.Zero);
        }
    }
}
