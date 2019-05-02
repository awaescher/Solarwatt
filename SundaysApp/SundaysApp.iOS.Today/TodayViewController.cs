using System;
using NotificationCenter;
using Foundation;
using UIKit;
using SundaysApp.Services;
using System.Linq;

namespace SundaysApp.iOS.Today
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

            MainLabel.Text = "Hey Beauty";

            Log("result is NewData");

            var cs = new AesCryptoService();
            Log("Got crypto");

            var pas = new PersistentAuthService(cs);
            Log("Got auth");

            var sfs = new SundaysFunctionService(pas);
            Log("Got sundays");

            var a = pas.GetAuth();
            Log($"Got auth. Is valid: {(a?.IsValid ?? false)}");

            //var s = sfs.Get(DateTime.Today, DateTime.Today).GetAwaiter().GetResult();
            //Log($"Got {(s?.Count().ToString() ?? "(null)")} sundays");

            //var sundayService = new SundaysFunctionService(
            //    new PersistentAuthService(
            //        new AesCryptoService()
            //    )
            //);

            //var from = DateTime.Today.AddDays(-7);
            //var to = DateTime.Today;


            //try
            //{
            //    var sundays = sundayService.Get(from, to).GetAwaiter().GetResult().ToList();
            //    result = sundays?.Count > 0 ? NCUpdateResult.NewData : NCUpdateResult.NoData;
            //}
            //catch
            //{
            //    result = NCUpdateResult.Failed;
            //}


            Log($"completing with {result}");

            completionHandler(result);
        }

        private void Log(string value) => Console.WriteLine(value);
    }
}