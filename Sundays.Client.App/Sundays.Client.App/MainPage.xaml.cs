using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SkiaSharp;
using Microcharts;

namespace Sundays.Client.App
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			var entries = new[]
			{
				new Microcharts.Entry(200)
				{
					Label = "January",
					ValueLabel = "200",
					Color = SKColor.Parse("#266489")
				},
				new Microcharts.Entry(400)
				{
					Label = "February",
					ValueLabel = "400",
					Color = SKColor.Parse("#68B9C0")
				},
				new Microcharts.Entry(-100)
					{
					Label = "March",
					ValueLabel = "-100",
					Color = SKColor.Parse("#90D585")
				}
			};

			var chart = new BarChart() { Entries = entries };
			// or: var chart = new PointChart() { Entries = entries };
			// or: var chart = new LineChart() { Entries = entries };
			// or: var chart = new DonutChart() { Entries = entries };
			// or: var chart = new RadialGaugeChart() { Entries = entries };
			// or: var chart = new RadarChart() { Entries = entries };

			this.chartView.Chart = chart;
		}
	}
}
