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
   public partial class OptionsPage : ContentPage {

      public CGame gm { get; set; }
      public CGame.RunMode RunMode { get; set; }
      public bool SpeechOn { get; set; }


      public OptionsPage(CGame g1, bool speech) {
      // -------------------------------------------------------------
         InitializeComponent();
         this.Title = "Options";
         gm = g1;
         RunMode = gm.runMode;
         SpeechOn = speech;

         SetSwitches(RunMode);
         SetSpeech(SpeechOn);

      }


      private void optSpeech_Toggled(object sender, ToggledEventArgs e) {
         // ----------------------------------------------------------------
         this.SpeechOn = e.Value;
         if (e.Value) {
            optFastEOP.IsToggled = false;
            this.RunMode = CGame.RunMode.Normal;
         }
      }


      private void OptFastEOP_Toggled(object sender, ToggledEventArgs e) {
      // ------------------------------------------------------------------
         if (optFastEOP.IsToggled) {
            this.RunMode = CGame.RunMode.FastEOP;
            SpeechOn = optSpeech.IsToggled = false;
         }
         else {
            this.RunMode = CGame.RunMode.Normal;
         }
      }


      private void optAuto_Toggled(object sender, ToggledEventArgs e) {

      }

      private void optFast_Toggled(object sender, ToggledEventArgs e) {

      }

      private void optFastEOG_Toggled(object sender, ToggledEventArgs e) {

      }


      private void SetSwitches(CGame.RunMode runMode1) {
      // -----------------------------------------------
         RunMode = runMode1;
         optAuto.IsToggled = optFast.IsToggled = optFastEOG.IsToggled = false;
         switch (RunMode) {
            case CGame.RunMode.Auto: optAuto.IsToggled = true; break;
            case CGame.RunMode.Fast: optFast.IsToggled = true; break;
            case CGame.RunMode.FastEog: optFastEOG.IsToggled = true; break;
            case CGame.RunMode.FastEOP: optFastEOP.IsToggled = true; break;
         }
      }


      private void SetSpeech(bool speech1) {
      // ---------------------------------------
         SpeechOn = speech1;
         optSpeech.IsToggled = speech1;

      }

   } //end class

} //end namespace