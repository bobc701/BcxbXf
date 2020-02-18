/* --------------------------------------------------------------
 * Purpose of this page is very simple:
 * Just set SelectedPlayer property when user clicks 'Choose',
 * or set it to null if user clicks 'Cancel'.
 * --------------------------------------------------------------
 */



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using BCX.BCXB;

namespace BcxbXf.Views
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class AvailablePage : ContentPage
   {

      public CBatter SelectedPlayer = null;
      private CLineupCard card;

      //public Action Dismiss;

      public AvailablePage(CLineupCard card1, string nickName, char crit) {
      // ---------------------------------------------------------------------
      // crit: p=pitchers, n=non-pitchers, a=all
         InitializeComponent();
         card = card1;
         lblTeamNickName.Text = nickName + " " + (crit=='p' ? "Bullpen:" : "Bench:");
         BindingContext = card;

      }


      private void cmdChoose_Clicked(object sender, EventArgs e) {
      // ---------------------------------------------------------
         SelectedPlayer = (CBatter)lstAvail.SelectedItem;
         //Dismiss();
         Navigation.PopModalAsync();

      }


      private void cmdCancel_Clicked(object sender, EventArgs e) {
      // ---------------------------------------------------------
         SelectedPlayer = null;
         Navigation.PopModalAsync();

      }


      private void lstAvail_ItemSelected(object sender, EventArgs e) {
      // ---------------------------------------------------------
         SelectedPlayer = (CBatter)lstAvail.SelectedItem;

      }
   }
}