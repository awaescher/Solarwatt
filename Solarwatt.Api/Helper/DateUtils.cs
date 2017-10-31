using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solarwatt.Api.Helper
{
	internal static class DateUtils
	{
		internal static DateTime ToBeginOfDay(this DateTime value)
		{
			return value.Date;
		}

		internal static DateTime ToEndOfDay(this DateTime value)
		{
			return value.Date.AddDays(1).AddSeconds(-1);
		}

		internal static long ToUnixTimeStamp(this DateTime value) => ToUnixTimeStamp(value, DateRange.None);

		internal static long ToUnixTimeStamp(this DateTime value, DateRange range)
		{
			if (range == DateRange.Begin)
				value = value.ToBeginOfDay();
			else if (range == DateRange.End)
				value = value.ToEndOfDay();

			long epochTicks = new DateTime(1970, 1, 1).Ticks;
			long unixTime = ((value.ToUniversalTime().Ticks - epochTicks) / TimeSpan.TicksPerSecond);

			return unixTime;
		}
	}
}
