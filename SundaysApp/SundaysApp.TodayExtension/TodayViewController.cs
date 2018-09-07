using System;

using NotificationCenter;
using Foundation;
using UIKit;
using System.Linq;
using SundaysApp.Services;

namespace SundaysApp.TodayExtension
{
    public partial class TodayViewController : UIViewController, INCWidgetProviding
    {
        protected TodayViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Do any additional setup after loading the view.
        }

        [Export("widgetPerformUpdateWithCompletionHandler:")]
        public void WidgetPerformUpdate(Action<NCUpdateResult> completionHandler)
        {
            // Perform any setup necessary in order to update the view.

            // If an error is encoutered, use NCUpdateResultFailed
            // If there's no update required, use NCUpdateResultNoData
            // If there's an update, use NCUpdateResultNewData

            var result = NCUpdateResult.NewData;

            //var sundayService = new SundaysFunctionService(
            //    new PersistentAuthService(
            //        new AesCryptoService()
            //    )
            //);

            //var from = DateTime.Today.AddDays(-7);
            //var to = DateTime.Today;


            //try
            //{
            //    var sundays = (await sundayService.Get(from, to)).ToList();
            //    result = sundays?.Count > 0 ? NCUpdateResult.NewData : NCUpdateResult.NoData;
            //}
            //catch
            //{
            //    result = NCUpdateResult.Failed;
            //}

            completionHandler(result);
        }
    }
}
