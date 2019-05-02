// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SundaysApp.iOS.Today
{
    [Register ("TodayViewController")]
    partial class TodayViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel MainLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (MainLabel != null) {
                MainLabel.Dispose ();
                MainLabel = null;
            }
        }
    }
}