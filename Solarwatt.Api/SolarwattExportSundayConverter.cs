using Sundays;
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
			const int PROPERTY_COUNT = 13;

			var values = line.Split(new string[] { SPLITTER }, StringSplitOptions.None);

			if (values.Length != PROPERTY_COUNT)
				throw new NotSupportedException($"ExportRow does not have the expected amount of values ({PROPERTY_COUNT}) when split with '{SPLITTER}'.\nValues: " + line);

			var row = new Sunday()
			{
				Date = System.Convert.ToDateTime(values[0]),
				PowerConsumptionWh = TryParseSingle(values[1]),
				PowerGenerationWh = TryParseSingle(values[2]),
				FeedInWh = TryParseSingle(values[3]),
				PurchaseWh = TryParseSingle(values[4]),
				BatterySupplyWh = TryParseSingle(values[5]),
				BatteryChargeWh = TryParseSingle(values[6]),
				BatteryChargePercent = TryParseSingle(values[7]),
				PrivateUseWh = TryParseSingle(values[8]),
				FeedIn2Wh = TryParseSingle(values[9]),
				PrivateUseFromPvWh = TryParseSingle(values[10]),
				PowerGenerationAtLocationWh = TryParseSingle(values[11]),
				PowerGenerationTotalWh = TryParseSingle(values[12])
			};

			return row;
		}

		private static float TryParseSingle(string value)
		{
			if (Single.TryParse(value, out float parsedValue))
				return parsedValue;

			return 0.0f;
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
