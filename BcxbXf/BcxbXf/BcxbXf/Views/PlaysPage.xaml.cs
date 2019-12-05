using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using BCX.BCXB;

namespace BcxbXf.Views {

/* --------------------------------------------------------------------
 * The whole purpose of this page is to set the valye of 'Play', which 
 * will be read upon return and used in the model.
 * -------------------------------------------------------------------
 */

   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class PlaysPage : ContentPage  {

      //public enum EGameState { PreGame, Offense, Defense };
      public CGame g;
      public SPECIAL_PLAY Play { get; set; } = SPECIAL_PLAY.AtBat;

      public PlaysPage(CGame g1) {
         InitializeComponent();
         this.Title = "Call a Play...";

         g = g1;

         if (g.PlayState == PLAY_STATE.START || g.PlayState == PLAY_STATE.NONE) {
            EnableButton(cmdSac, false);
            EnableButton(cmdSteal, false);
            EnableButton(cmdIP, false);
         }
         else {
            EnableButton(cmdSac, true);
            EnableButton(cmdSteal, true);
            EnableButton(cmdIP, true);
         }

      // Prevent steal and sacrifice if no base runners...
         if (g.r[1].ix == 0 && g.r[2].ix == 0 && g.r[3].ix == 0) {
            cmdSteal.IsEnabled = false;
            cmdSac.IsEnabled = false;
         }

         Play = g.specialPlay;

         switch (g.specialPlay) {
            case SPECIAL_PLAY.Bunt:
               cmdSac.IsToggled = true;
               cmdSteal.IsToggled = false;
               cmdIP.IsToggled = false;
               break;
            case SPECIAL_PLAY.Steal:
               cmdSac.IsToggled = false;
               cmdSteal.IsToggled = true;
               cmdIP.IsToggled = false;
               break;
            case SPECIAL_PLAY.IP:
               cmdSac.IsToggled = false;
               cmdSteal.IsToggled = false;
               cmdIP.IsToggled = true;
               break;
            default:
               cmdSac.IsToggled = false;
               cmdSteal.IsToggled = false;
               cmdIP.IsToggled = false;
               break;
         }

      }


      private void EnableButton(Switch btn, bool on) {
         // ---------------------------------------------------------------
         if (on) {
            btn.IsEnabled = true;
         }
         else {
            btn.IsEnabled = false;
         }
      }


      private void cmdSteal_Taggled(object sender, ToggledEventArgs e) {
         // ---------------------------------------------------------------
         if (e.Value) {
            cmdSac.IsToggled = false;
            cmdIP.IsToggled = false;
            Play = SPECIAL_PLAY.Steal;
         }
         else Play = SPECIAL_PLAY.AtBat;
      }

      private void cmdSac_Toggled(object sender, ToggledEventArgs e) {
         // ---------------------------------------------------------------
         if (e.Value) {
            cmdSteal.IsToggled = false;
            cmdIP.IsToggled = false;
            Play = SPECIAL_PLAY.Bunt;
         }
         else Play = SPECIAL_PLAY.AtBat;
      }

      private void cmdIP_Toggled(object sender, ToggledEventArgs e) {
         // ---------------------------------------------------------------
         if (e.Value) {
            cmdSteal.IsToggled = false;
            cmdSac.IsToggled = false;
            Play = SPECIAL_PLAY.IP;
         }
         else Play = SPECIAL_PLAY.AtBat;
      }

   }

}