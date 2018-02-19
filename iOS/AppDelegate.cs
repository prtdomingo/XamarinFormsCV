using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using Xam.Plugins.OnDeviceCustomVision;

namespace XamarinFormsCV.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            CrossImageClassifier.Current.Init("imageclassifier", ModelType.General);
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }
    }
}
