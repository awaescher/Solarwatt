using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SundaysApp.Services;
using Xamarin.Forms;
using Microcharts;
using SkiaSharp;
using Sundays.Model;
using System.Threading;

namespace SundaysApp.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            (BindingContext as PageModels.MainPageModel).PropertyChanged += Handle_PropertyChanged;
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            (BindingContext as PageModels.MainPageModel).PropertyChanged -= Handle_PropertyChanged;
            base.OnDisappearing();
        }

        protected void Handle_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals("Sundays"))
                return;

            const int LABEL_SIZE = 60;

            generationChartView.Chart = generationChartView.Chart ?? new BarChart() { LabelTextSize = LABEL_SIZE };
            consumptionChartView.Chart = consumptionChartView.Chart ?? new BarChart() { LabelTextSize = LABEL_SIZE };
            batteryChartView.Chart = batteryChartView.Chart ?? new RadialGaugeChart() { MinValue = 0, MaxValue = 100, LabelTextSize = LABEL_SIZE };

            var dateFormat = Thread.CurrentThread.CurrentUICulture.DateTimeFormat;

            var sundays = (BindingContext as PageModels.MainPageModel)
                .Sundays.Where(d => d.Date <= DateTime.Today)
                .ToList();

            var today = sundays.FirstOrDefault(s => s.Date.Date == DateTime.Today);

            var generationEntries = sundays.Select(d =>
                new Microcharts.Entry(d.PowerGenerationTotalWh)
                {
                    ValueLabel = (d.PowerGenerationTotalWh / 1000).ToString("F1"),
                    Label = dateFormat.GetShortestDayName(d.Date.DayOfWeek),
                    Color = SKColor.Parse(d.Date == DateTime.Today ? "#b0f9a4" : "#90D585")
                }
            );

            var consumptionEntries = sundays.Select(d =>
                new Microcharts.Entry(-1 * d.PowerConsumptionWh)
                {
                    ValueLabel = (d.PowerConsumptionWh / 1000).ToString("F1"),
                    Color = SKColor.Parse(d.Date == DateTime.Today ? "#f9ada4" : "#d58d85")
                }
            );

            generationChartView.Chart.Entries = generationEntries;
            consumptionChartView.Chart.Entries = consumptionEntries;

            batteryChartView.Chart.Entries = new[] { GetBatteryEntry(today?.BatteryChargePercent ?? 0) };

            EnsureSameScale(generationChartView.Chart as BarChart, consumptionChartView.Chart as BarChart);

            // fixes: https://github.com/jamesmontemagno/Xamarin.Forms-PullToRefreshLayout/issues/51
            Task.Delay(100).ContinueWith(t => Device.BeginInvokeOnMainThread(() => scrollView.ScrollToAsync(0, 0, true)));
        }

        protected override void LayoutChildren(double x, double y, double width, double height)
        {
            var charts = new[] { batteryChartView, generationChartView, consumptionChartView };
            var desiredHeight = this.Height / charts.Length;

            foreach (var chart in charts)
                chart.HeightRequest = desiredHeight;

            base.LayoutChildren(x, y, width, height);
        }

        private Microcharts.Entry GetBatteryEntry(float chargePercent)
        {
            var entry = new Microcharts.Entry(chargePercent);

            var color = "#f9ada4"; // Red

            if (chargePercent > 20)
                color = "#c6d585"; // Greenyellow

            if (chargePercent > 60)
                color = "#90D585"; // Green

            if (chargePercent > 80)
                color = "#78ce6b"; // Satisfyingly green green

            entry.Color = SKColor.Parse(color);
            entry.ValueLabel = chargePercent.ToString("N0") + "%";

            return entry;
        }

        private void EnsureSameScale(BarChart generationChart, BarChart consumptionChart)
        {
            if (generationChart.MaxValue > Math.Abs(consumptionChart.MinValue))
                consumptionChart.MinValue = -1 * generationChart.MaxValue;
            else
                generationChart.MaxValue = Math.Abs(consumptionChart.MinValue);
        }
    }
}
