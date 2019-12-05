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

namespace BcxbXf.Views
{
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class FieldingDiskPage : ContentPage {

      public GProfileDisk disk1;
      public CGame g; //Assigned in PrepareForSegue.

      public FieldingDiskPage(CGame g1) {
      // ----------------------------------------
         InitializeComponent();
         Title = "Fielder Profile";
         g = g1;



      }

      private void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args) {
      // ------------------------------------------------------------------------------
         SKImageInfo info = args.Info;
         SKSurface surface = args.Surface;
         SKCanvas canvas = surface.Canvas;

         canvas.Clear();

         var x = info.Width / 2f;

         disk1 = new GProfileDisk(x, x + 100, g.fpara, args) {
            DiceRoll = g.diceRollFielding,
            ProfileLabel = g.fpara.fielderName + " fielding..."
         };

         string[] aText = g.fpara.description.Split('/');
         disk1.SubLabel1 = "Green: " + aText [0] + ", Red: " + aText [1];
         disk1.SubLabel2 = "Fielding ability: " + g.fpara.fielderSkill;
         disk1.Draw(showColorKey: false);

      }
   }
}