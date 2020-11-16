using System;
using System.Collections.Generic;
using System.Linq;

namespace Syinpo.Core.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTime GetDayStart( DateTime date ) {
            return date.Date;
        }

        public static DateTime GetDayEnd(DateTime date )
        {
            return new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, 997);
        }

        // DateTime --> long
        public static long ConvertDataTimeToLong( DateTime dt ) {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime( new DateTime( 1970, 1, 1 ) );
            TimeSpan toNow = dt.Subtract( dtStart );
            long timeStamp = toNow.Ticks;
            timeStamp = long.Parse( timeStamp.ToString().Substring( 0, timeStamp.ToString().Length - 4 ) );
            return timeStamp;
        }


        // long --> DateTime
        public static DateTime ConvertLongToDateTime( long d ) {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime( new DateTime( 1970, 1, 1 ) );
            long lTime = long.Parse( d + "0000" );
            TimeSpan toNow = new TimeSpan( lTime );
            DateTime dtResult = dtStart.Add( toNow );
            return dtResult;
        }

        public static string GetTimeGroup( DateTime now ) {
            var group = now.ToString( "yyyyMMddHHmm" );
            //long.TryParse( group, out long requestGroup );
            return group;
        }


        public static List<DateTime> GetAllDays( DateTime dt1, DateTime dt2 ) {
            List<DateTime> listDays = new List<DateTime>();
            DateTime dtDay = new DateTime();
            for( dtDay = dt1; dtDay.CompareTo( dt2 ) <= 0; dtDay = dtDay.AddDays( 1 ) ) {
                listDays.Add( dtDay );
            }
            return listDays;
        }


        public static List<string> GetLastestDateList(DateTime date, int days)
        {
            var dates = new List<string>();
            for (var i = days; i >= 0; i--)
            {
                var dateStr = GetDate(date.AddDays(-i));
                dates.Add(dateStr);
            }

            return dates;

            string GetDate( DateTime date2 ) {
                return date2.Date.Month + "/" + date2.Date.Day;
            }
        }
    }
}
