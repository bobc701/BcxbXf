using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BcxbXf.Views
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class AboutPage : ContentPage
   {
      public AboutPage() {
      // -----------------------------------------------------------
         InitializeComponent();
         this.Title = "Help / About";
      }

      private void btnHelp_Clicked(object sender, EventArgs e) {
      // ---------------------------------------------------------------

         var addr = "https://www.zeemerixdata.com/baseball_iphone/help/default.html";
         Device.OpenUri(new Uri(addr));

      }

      private void btnVisit_Clicked(object sender, EventArgs e) {
      // -----------------------------------------------------------------
         var addr = "http://www.zeemerix.com/";
         Device.OpenUri(new Uri(addr));

      }
   }

}