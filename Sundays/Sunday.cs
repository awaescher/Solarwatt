using System;
using System.Collections.Generic;
using System.Linq;

namespace Sundays.Model
{
	[System.Diagnostics.DebuggerDisplay("{Date}, Consumption:{PowerConsumptionWh} Generation:{PowerGenerationWh}")]
	public class Sunday
	{
		public DateTime Date { get; set; }

		public float PowerConsumptionWh { get; set; }

		public float PowerGenerationWh { get; set; }

		public float FeedInWh { get; set; }

		public float PurchaseWh { get; set; }

		public float BatterySupplyWh { get; set; }

		public float BatteryChargeWh { get; set; }

		public float? BatteryChargePercent { get; set; }

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
				BatteryChargePercent?.ToString() ?? "null",
				PrivateUseWh,
				FeedIn2Wh,
				PrivateUseFromPvWh,
				PowerGenerationAtLocationWh,
				PowerGenerationTotalWh
			});
		}
	}
}
