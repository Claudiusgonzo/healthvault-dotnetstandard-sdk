﻿// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace SampleIos
{
    [Register ("RootViewController")]
    partial class RootViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIActivityIndicatorView activityIndicator { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton connectButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel statusLabel { get; set; }

        [Action ("ConnectButtonPressed")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ConnectButtonPressed ();

        void ReleaseDesignerOutlets ()
        {
            if (activityIndicator != null) {
                activityIndicator.Dispose ();
                activityIndicator = null;
            }

            if (connectButton != null) {
                connectButton.Dispose ();
                connectButton = null;
            }

            if (statusLabel != null) {
                statusLabel.Dispose ();
                statusLabel = null;
            }
        }
    }
}