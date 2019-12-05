using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using BCX.BCXB;


namespace BcxbXf.Views {

   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class ProfileDiskPage : ContentPage {

      public GProfileDisk disk1, disk2, disk3, draw4;
      public CGame g; //Assigned in PrepareForSegue.
      CBatter b;
      CPitcher p;



      public ProfileDiskPage() {

         InitializeComponent();
         Title = "Profile Disks";
      }


      void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args) {
         
         SKImageInfo info = args.Info;
         SKSurface surface = args.Surface;
         SKCanvas canvas = surface.Canvas;

         canvas.Clear();

      // Get name of batter & pitcher for labels on the disks...
         string sBatter = b.bname;
         string sPitcher = p.pname;

         disk1 = new GProfileDisk(150, 200, g.cmean, args);
         disk1.DiceRoll = g.diceRollBatting;
         disk1.ProfileLabel = "League Norm:";
         disk1.Draw(new SKRect(0,0,0,0));

         disk2 = new GProfileDisk(150, 200, b.par, args);
         disk2.DiceRoll = g.diceRollBatting;
         disk2.ProfileLabel = sBatter + " vs. League Norm:";
         disk2.SubLabel1 = BatterStatsString(1);
         disk2.SubLabel2 = BatterStatsString(2);
         disk2.Draw(new SKRect(0, 0, 0, 0));

         disk3 = new GProfileDisk(150, 200, p.par, args); 
         disk3.DiceRoll = g.diceRollBatting;
         disk3.ProfileLabel = sPitcher + " vs. League Norm:";
         disk3.SubLabel1 = PitcherStatsString(1);
         disk3.SubLabel2 = PitcherStatsString(2);
         disk3.Draw(new SKRect(0, 0, 0, 0));

         draw4 = new GProfileDisk(150, 200, g.cpara, args);
         draw4.DiceRoll = g.diceRollBatting;
         draw4.ProfileLabel = sBatter + " vs. " + sPitcher + ":";
         draw4.Draw(new SKRect(0, 0, 0, 0));
      }


      private string BatterStatsString(int i) {
         // ----------------------------------------------
         string s = "";
         switch (i) {
            case 1:
               s = string.Format(
                  "ab:{0}, ave:{1:#.000}, hr:{2}, 3b:{3}, 2b:{4}",
                  b.br.ab, b.br.ave, b.br.hr, b.br.b3, b.br.b2);
               break;
            case 2:
               s = string.Format(
                  "rbi:{0}, bb:{1}, so{2}",
                  b.br.bi, b.br.bb, b.br.so);
               break;
         }
         return s;

      }


      private string PitcherStatsString(int i) {
         // ----------------------------------------------
         string s = "";
         CPitRealSet r = p.pr;
         switch (i) {
            case 1:
               double whip = r.ip3 > 0 ? (r.h + r.bb) / (r.ip3 / 3.0) : 0.0;
               s = string.Format(
                  "ip:{0:#0.0}, era:{1:#0.00}, whip:{2:#0.00}",
                  r.ip3 / 3.0, r.era, whip);
               break;
            case 2:
               s = string.Format(
                  "h:{0}, hr:{1}, so:{2}, bb:{3}",
                  r.h, r.hr, r.so, r.bb);
               break;
         }
         return s;

      }


   }

}