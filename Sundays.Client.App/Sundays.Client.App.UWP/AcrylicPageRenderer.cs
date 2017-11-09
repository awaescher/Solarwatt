using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(Sundays.Client.App.MainPage), typeof(Sundays.Client.App.UWP.AcrylicPageRenderer))]
namespace Sundays.Client.App.UWP
{
	public class AcrylicPageRenderer : PageRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Page> e)
		{
			base.OnElementChanged(e);

			if (e.NewElement == null)
				return;

			if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent(typeof(XamlCompositionBrushBase).FullName))
			{
				var acrylicBrush = new AcrylicBrush();
				acrylicBrush.BackgroundSource = AcrylicBackgroundSource.HostBackdrop;

				Children.OfType<LayoutRenderer>().Single().Background = acrylicBrush;
			}
		}
	}
}
