using System;
using System.Collections.Generic;
using System.Linq;

namespace Solarwatt.Api.Dto
{
	[System.Diagnostics.DebuggerDisplay("{Date}, Consumption:{PowerConsumptionWh} Generation:{PowerGenerationWh}")]
	public class ExportRow
	{
		public DateTime Date { get; set; }

		public float PowerConsumptionWh { get; set; }

		public float PowerGenerationWh { get; set; }

		public float FeedInWh { get; set; }

		public float PurchaseWh { get; set; }

		public float BatterySupplyWh { get; set; }

		public float BatteryChargeWh { get; set; }

		public float BatteryChargePercent { get; set; }

		public float PrivateUseWh { get; set; }

		public float FeedIn2Wh { get; set; }

		public float PrivateUseFromPvWh { get; set; }

		public float PowerGenerationAtLocationWh { get; set; }

		public float PowerGenerationTotalWh { get; set; }

		public override string ToString()
		{
			return string.Join(";", new object[] {
				Date,
				PowerConsumptionWh,
				PowerGenerationWh,
				FeedInWh,
				PurchaseWh,
				BatterySupplyWh,
				BatteryChargeWh,
				BatteryChargePercent,
				PrivateUseWh,
				FeedIn2Wh,
				PrivateUseFromPvWh,
				PowerGenerationAtLocationWh,
				PowerGenerationTotalWh
			});
		}

		public static ExportRow FromCsv(string line, string splitter)
		{
			const int PROPERTY_COUNT = 13;

			var values = line.Split(new string[] { splitter }, StringSplitOptions.None);

			if (values.Length != PROPERTY_COUNT)
				throw new NotSupportedException($"ExportRow does not have the expected amount of values ({PROPERTY_COUNT}) when split with '{splitter}'.\nValues: " + line);

			var row = new ExportRow()
			{
				Date = Convert.ToDateTime(values[0]),
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

		public static bool IsDataRow(string line, string splitter)
		{
			return !string.IsNullOrWhiteSpace(line)
				&& line.Contains(splitter)
				&& line.IndexOf("Date", StringComparison.OrdinalIgnoreCase) == -1
				&& line.IndexOf("Wh", StringComparison.OrdinalIgnoreCase) == -1;
		}

		public static IEnumerable<ExportRow> MergeByDate(IEnumerable<ExportRow> rows)
		{
			var result = new List<ExportRow>();

			foreach (var row in rows.OrderBy(r => r.Date).ToArray())
			{
				row.Date = row.Date.Date;
				var existingRow = result.SingleOrDefault(e => e.Date == row.Date);

				if (existingRow == null)
				{
					result.Add(row);
				}
				else
				{
					//existingRow.Date = Convert.ToDateTime(values[0]),
					existingRow.PowerConsumptionWh += row.PowerConsumptionWh;
					existingRow.PowerGenerationWh += row.PowerGenerationWh;
					existingRow.FeedInWh += row.FeedInWh;
					existingRow.PurchaseWh += row.PurchaseWh;
					existingRow.BatterySupplyWh += row.BatterySupplyWh;
					existingRow.BatteryChargeWh += row.BatteryChargeWh;
					existingRow.BatteryChargePercent += row.BatteryChargePercent;
					existingRow.PrivateUseWh += row.PrivateUseWh;
					existingRow.FeedIn2Wh += row.FeedIn2Wh;
					existingRow.PrivateUseFromPvWh += row.PrivateUseFromPvWh;
					existingRow.PowerGenerationAtLocationWh += row.PowerGenerationAtLocationWh;
					existingRow.PowerGenerationTotalWh += row.PowerGenerationTotalWh;
				}
			}

			return result;
		}
	}
}
