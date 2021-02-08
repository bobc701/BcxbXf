using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Foundation;
using UIKit;

using BCX.BCXCommon;

namespace BcxbXf.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());
            PrimeTeamCache(); // We can't await this because overridden method is not async. #3000.03
            Thread.Sleep(4500); // Delay toshow splash 
            Debug.WriteLine($"TeamCache.Count after FinishedLaunching: {GFileAccess.TeamCache.Count}");
            return base.FinishedLaunching(app, options);
        }


      private async Task PrimeTeamCache() { // #3000.03
         // ---------------------------------------------------------
         // This routine will do an initial fill of the teamCache using 2000-2020,
         // while the splash screen is being displayed.
         // If no internet, this will fail and do nothing.

         try {
            var url = new Uri(GFileAccess.client.BaseAddress, $"{GFileAccess.WinhostEndPoint}api/team-list/2010/2020");

            List<BCX.BCXCommon.CTeamRecord> yearList10;
            HttpResponseMessage response = await GFileAccess.client.GetAsync(url.ToString());
            if (response.IsSuccessStatusCode) {
               yearList10 = await response.Content.ReadAsAsync<List<BCX.BCXCommon.CTeamRecord>>();
            }
            else {
               throw new Exception($"Error loading initial list of teams\r\nStatus code: {response.StatusCode}");
            }
            GFileAccess.TeamCache.AddRange(yearList10);
            Debug.WriteLine($"Teamcache updated in PrimeTeamcache: {GFileAccess.TeamCache.Count()} teams"); //#3000.04
         }
         catch (Exception ex) {
            // Just do nothing here. Can't show error dialog.
            // CAlert.ShowOkAlert("Error initializing data", ex.Message, "OK", this);
            Debug.WriteLine($"Error in PrimeTeamcache: {ex.Message}"); //#3000.04
         }

      }

   }

}
