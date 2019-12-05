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
   public partial class ProfileDisk2Page : ContentPage {

      public GProfileDisk disk1, disk2, disk3, draw4;
      public CGame g; //Assigned in PrepareForSegue.
      CBatter b;
      CPitcher p;

      public ProfileDisk2Page(CGame g1) {

         InitializeComponent();
         Title = "Batter Profiles";
         g = g1;
         int i = g.t[g.ab].linup[g.t[g.ab].slot];
         int j = g.t[g.fl].curp;
         b = g.t[g.ab].bat[i];
         p = g.t[g.fl].pit[j];

      }


      void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args) {
      // -------------------------------------------------------------------------
         SKImageInfo info = args.Info;
         SKSurface surface = args.Surface;
         SKCanvas canvas = surface.Canvas;

         canvas.Clear();

         // Get name of batter & pitcher for labels on the disks...
         string sBatter = b.bname;
         string sPitcher = p.pname;

         var x = info.Width / 2f;

         draw4 = new GProfileDisk(x, x + 250, g.cpara, args) {
            DiceRoll = g.diceRollBatting,
            ProfileLabel = sBatter + " vs. " + sPitcher
         };
         draw4.Draw();

         disk2 = new GProfileDisk(x, 3*x + 280, b.par, args) {
            DiceRoll = g.diceRollBatting,
            ProfileLabel = sBatter + " vs. League Norm",
            SubLabel1 = BatterStatsString(1),
            SubLabel2 = BatterStatsString(2)
         };
         disk2.Draw();

         disk3 = new GProfileDisk(x, 5*x + 310, p.par, args) {
            DiceRoll = g.diceRollBatting,
            ProfileLabel = sPitcher + " vs. League Norm",
            SubLabel1 = PitcherStatsString(1),
            SubLabel2 = PitcherStatsString(2)
         };
         disk3.Draw();

         disk1 = new GProfileDisk(x, 7*x + 340, g.cmean, args) {
            DiceRoll = g.diceRollBatting,
            ProfileLabel = "League Norm"
         };
         disk1.Draw();

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
                  "rbi:{0}, bb:{1}, so:{2}",
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