using Sundays;
using Sundays.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solarwatt.Api
{
	public class SolarwattExportSundayConverter : SundayConverter
	{
		const string SPLITTER = ";";

		public override IEnumerable<Sunday> Convert(object value)
		{
			var stringRows = (IEnumerable<string>)value;

			return stringRows
				.Where(IsDataRow)
				.Select(FromCsv);
		}

		public static Sunday FromCsv(string line)
		{
			var culture = new System.Globalization.CultureInfo("de-DE");

			const int PROPERTY_COUNT = 13;

			var values = line.Split(new string[] { SPLITTER }, StringSplitOptions.None);

			if (values.Length != PROPERTY_COUNT)
				throw new NotSupportedException($"ExportRow does not have the expected amount of values ({PROPERTY_COUNT}) when split with '{SPLITTER}'.\nValues: " + line);

			var row = new Sunday()
			{
				Date = System.Convert.ToDateTime(values[0], culture),
				PowerConsumptionWh = TryParseSingle(values[1], culture),
				PowerGenerationWh = TryParseSingle(values[2], culture),
				FeedInWh = TryParseSingle(values[3], culture),
				PurchaseWh = TryParseSingle(values[4], culture),
				BatterySupplyWh = TryParseSingle(values[5], culture),
				BatteryChargeWh = TryParseSingle(values[6], culture),
				BatteryChargePercent = TryParseNullableSingle(values[7], culture), // these values are not summed up, the lastest one is the correct one -> don't take "--" as 0 here, we need to differentiate.
				PrivateUseWh = TryParseSingle(values[8], culture),
				FeedIn2Wh = TryParseSingle(values[9], culture),
				PrivateUseFromPvWh = TryParseSingle(values[10], culture),
				PowerGenerationAtLocationWh = TryParseSingle(values[11], culture),
				PowerGenerationTotalWh = TryParseSingle(values[12], culture)
			};

			return row;
		}

		private static float TryParseSingle(string value, IFormatProvider formatProvider)
		{
			if (Single.TryParse(value, System.Globalization.NumberStyles.Any, formatProvider, out float parsedValue))
				return parsedValue;

			return 0.0f;
		}

		private static float? TryParseNullableSingle(string value, IFormatProvider formatProvider)
		{
			if (Single.TryParse(value, System.Globalization.NumberStyles.Any, formatProvider, out float parsedValue))
				return parsedValue;

			return null;
		}


		public static bool IsDataRow(string line)
		{
			return !string.IsNullOrWhiteSpace(line)
				&& line.Contains(SPLITTER)
				&& line.IndexOf("Date", StringComparison.OrdinalIgnoreCase) == -1
				&& line.IndexOf("Wh", StringComparison.OrdinalIgnoreCase) == -1;
		}
		
	}
}
