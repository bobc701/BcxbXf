using System;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using SkiaSharp;
using SkiaSharp.Views.Forms;

namespace BCX.BCXB {
   
   public class GProfileDisk {

   // SkiaSharp specific stuff...
      private SKCanvas canvas;
      private SKImageInfo info;

      //private CGContext ctxt;
      private SKPoint p0;
      const float pi = (float)Math.PI;
      private float rInner, rOuter; // Radius of the outside & inside disks
      private SKRect rectOuter, rectInner;

   // Things that need to be set by the caller...
      private float X, Y; //Center of the disc;
      private CParamSet para; //UISearchBarDelegate to extract pcts.
      private double[] pcts;
      private int iFrom, iTo;
      private double pctSpinner = -1.0; //For the spinner. Negative means don't show.
      private CDiceRoll diceRoll;


   // Properties for the label...
      private string profileLabel = ""; //Empty string means don't show label.
      private SKColor profileLabelColor = Color.Blue.ToSKColor();
      private float profileLabelFontSize = 20.0f; //20.0f;

   // Properties for the sub-labels...
      private string subLabel1 = ""; //Used for fielding detail -- good play.
      private string subLabel2 = ""; //Used for fielding detail -- bad play.
      private float subLabelFontSize = 16.0f; //15.0f; //Applies to both sub-labels.

      private SKColor[] segmentColors;
      private string[] segmentLabels;
      private float segmentLabelFontSize = 12f;

   // Public setter properties...
      public double PctSpinner { set { pctSpinner = value; }}
      public string ProfileLabel { set { profileLabel = value; }}
      public string SubLabel1 { set { subLabel1 = value; }}
      public string SubLabel2 { set { subLabel2 = value; }}
      public CDiceRoll DiceRoll { set { diceRoll = value; }}
      public SKColor[] SegmentColors { set { segmentColors = value; }}
      private string[] SegmentLabels { set { SegmentLabels = value; }}
      public float SubLabelFontSize { set { subLabelFontSize = value; }} //Applies to both sub-labels.

      SKPaint paintDef = new SKPaint {
         Style = SKPaintStyle.Fill,
         //TextSize = Wid(0.04f), //45, 
         TextAlign = SKTextAlign.Left,
         StrokeWidth = 1
      };

      public void SetLabelProperties(SKColor color1, float fontSize1) {
         this.profileLabelColor = color1;
         this.profileLabelFontSize = fontSize1;
      }


      public GProfileDisk (double x1, double y1, CParamSet para1, SKPaintSurfaceEventArgs args) {
      // ------------------------------------------------------------------------------
         canvas = args.Surface.Canvas;
         info = args.Info;
         
         para = para1;
         X = (float)x1;
         Y = (float)y1;
         iFrom = 1;
         iTo = para1.SegmentCount;
         pcts = para1.GetWidthArray();
         segmentColors = para1.SegmentColors.Select(c => new SKColor(c)).ToArray();
         segmentLabels = para1.SegmentLabels;
      }


      public GProfileDisk() {
      // ----------------------------
         profileLabel = "";

      }


      public void Draw(bool showColorKey = true) {
         // -----------------------------------------
         float x, y, yDel = 42;
         if (this.pcts == null) return;
         if (showColorKey) 
         // It is batter / pitcher profilr...
            DrawColorKey();
         else {
         // It is fieldeing profile...
            y =60;
            x = 75; //120;
            paintDef.TextSize = Wid(0.04f);
            canvas.DrawText("This play was based on fielding ability.", x, y, paintDef);
            canvas.DrawText("Fielders are rated 0 to 6 at each position.", x, y+yDel, paintDef);
         }

         DrawProfileDisk();
         if (diceRoll.topLevelResult != TLR.none) {
            pctSpinner = diceRoll.pointOverall;
            DrawSpinner();
         }
         WriteDiskLabel();
         //await Task.Delay(100);
         //WriteWedgeLabels();

      }


      private void DrawProfileDisk() {
         // ---------------------------------------------------------------
         //double a1 = 3.0 * pi / 2.0;
         float a1 = -90; // (float)(System.Math.PI / 2.0);
         float a2;

         var p0 = new SKPoint(X, Y);
         rOuter = X - Wid(0.07f); //70.0f;   // Radius of disk so it leaves space on each side.
         rInner = rOuter - Wid(0.132f); //130; 
         rectOuter = new SKRect(p0.X - rOuter, p0.Y - rOuter, p0.X + rOuter, p0.Y + rOuter);
         rectInner = new SKRect(p0.X - rInner, p0.Y - rInner, p0.X + rInner, p0.Y + rInner);

         for (int i=iFrom; i<=iTo; i++) {
            a2 = a1 + (float)pcts[i] * 360.0f; // 2f * pi; 
            BCDrawOneWedge (a1, a2, segmentColors[i], segmentLabels[i]); 
            a1 = a2; 
         }
            
      }


      private void DrawSpinner() {
      // --------------------------------------------------------------------
         if (pctSpinner < 0.0) return; // If negative, don't draw spinner.

         var linePaint = new SKPaint {
            Color = Color.DarkGray.ToSKColor(),
            StrokeWidth = 4,
            StrokeCap = SKStrokeCap.Round
         };

         var path = new SKPath();

         // Draw the small nub in the middle...   
         linePaint.Style = SKPaintStyle.Fill;
         path.AddCircle(X, Y, 9.0f, SKPathDirection.Clockwise);

         // Draw the main spinner shaft...
         //float a = (pi / 2.0f) - ((float)pctSpinner * 2.0f * pi);
         linePaint.Style = SKPaintStyle.Stroke; 
         float a = -90f + (float)pctSpinner * 360f;
         var p0 = new SKPoint(X, Y);
         SKPoint p1 = GetPoint (p0, a, rInner);
         path.MoveTo(p0);
         path.LineTo(p1);
         canvas.DrawPath(path, linePaint);

         // Add the tip on the end of the spinner.
         linePaint.Style = SKPaintStyle.Fill; 
         SKPoint p2 = GetPoint (p0, a - 5f, rInner - 40); 
         SKPoint p3 = GetPoint (p0, a + 5f, rInner - 40);
         path.MoveTo(p1);
         path.LineTo(p2);
         path.LineTo(p3);
         path.LineTo(p1);
         path.Close(); //Needed?
         canvas.DrawPath(path, linePaint);

       }


      private SKPoint GetPoint(SKPoint p1, float a, float r) {
      // -------------------------------------------------------
      // Return a point offset from p1 at angle a, length r.
         float x1 = p1.X + r * (float)Math.Cos(Rad(a));
         float y1 = p1.Y + r * (float)Math.Sin(Rad(a));
         return new SKPoint (x1, y1);
      }

      private float Rad(float a) => a * (float)Math.PI / 180f;
      private float Wid(float pct) => pct * info.Width;


      private void BCDrawOneWedge(float a1, float a2, SKColor color, string lbl) {
         // ----------------------------------------------------------------
         var path = new SKPath();

         var p0 = new SKPoint(X, Y);

         var linePaint = new SKPaint {
            Style = SKPaintStyle.StrokeAndFill,
            Color = color,
            StrokeWidth = 3,
            StrokeCap = SKStrokeCap.Round
         };

         var textPaint = new SKPaint {
            Style = SKPaintStyle.Fill,
            Color = Color.Yellow.ToSKColor(),
            StrokeWidth = 1,
            TextAlign = SKTextAlign.Center,
            TextSize = 30
         };


         SKPoint p1, p2, q1, q2;
         p1 = GetPoint(p0, a1, rOuter);
         p2 = GetPoint(p0, a2, rOuter);
         q1 = GetPoint(p0, a1, rInner);
         q2 = GetPoint(p0, a2, rInner);

         path.MoveTo(q1);
         path.LineTo(p1);
         path.ArcTo(rectOuter, a1, a2 - a1, false);
         path.LineTo(q2);
         path.ArcTo(rectInner, a2, a1 - a2, false);

         path.Close();
         //path.FillType = SKPathFillType.EvenOdd;

         linePaint.Color = Color.Black.ToSKColor();
         linePaint.Style = SKPaintStyle.Stroke;
         canvas.DrawPath(path, linePaint);
         linePaint.Color = color;
         linePaint.Style = SKPaintStyle.Fill;
         canvas.DrawPath(path, linePaint);


         // Write the segment label, if wedge is wide enough...
         // .35 radians is about 20 deg. This is arbitrary cutoff.
         if (lbl != "" && Math.Abs(a2 - a1) >= 15f) {
            var pCenter = GetPoint(p0, (a2 + a1) * 0.5F, (rOuter + rInner) * 0.5F);
            pCenter.X -= 10f;
            canvas.DrawText(lbl, pCenter, textPaint);


         }

      }


      public void BCDrawRectangle(float x1, float y1, float x2, float y2, SKPaint paint) {
   // ----------------------------------------------------------------
   // Not used???
      var path = new SKPath();
      path.AddRect(new SKRect(x1, y1, x2, y2));

            
   }


      private void WriteDiskLabel() {
      // -----------------------------------------
         if (profileLabel == "") return;

         var paint = new SKPaint {
            Style = SKPaintStyle.Fill,
            TextSize = Wid(0.04f), //45, 
            TextAlign = SKTextAlign.Center,
            StrokeWidth = 1
         };

         var paintKey = new SKPaint {
            Style = SKPaintStyle.Fill,
            TextSize = Wid(0.04f), //45,
            TextAlign = SKTextAlign.Left,
            StrokeWidth = 1
         };

         SKPoint pStart = new SKPoint(X, Y - 0.3f * rInner);
         paint.Color = Color.Black.ToSKColor();
         paint.FakeBoldText = true;
         canvas.DrawText(profileLabel, pStart, paint);


      // Write the sub-labels...
         pStart = new SKPoint(X, Y + 0.3f * rInner);
         paint.TextSize = Wid(0.032f); //30f;
         paint.FakeBoldText = false;
         canvas.DrawText(subLabel1, pStart, paint);

         pStart = new SKPoint(X, Y + 0.3f * rInner + 30); ; 
         paint.TextSize = Wid(0.032f); //30f;
         paint.FakeBoldText = false;
         canvas.DrawText(subLabel2, pStart, paint);


         if (para.GetType().Name == "CFieldingParamSet") {
            float y = Y + rOuter + 150;
            float ydel = 42;
            float x = X - rOuter;
            canvas.DrawText("Key to fielding ability:", x, y, paintKey);
            canvas.DrawText(" 6: Gold glove quality at the position", x, y + ydel, paintKey);
            canvas.DrawText(" 5: All-star quality at the position", x, y + 2 * ydel, paintKey);
            canvas.DrawText(" 4: Above average among starters at the position", x, y + 3 * ydel, paintKey);
            canvas.DrawText(" 3: Average among starters at the position", x, y + 4 * ydel, paintKey);
            canvas.DrawText(" 2: Below average among starters at the position", x, y + 5 * ydel, paintKey);
            canvas.DrawText(" 1: Definate liability at the position", x, y + 6 * ydel, paintKey);
            canvas.DrawText(" 0 or blank: Does not play the position", x, y + 7 * ydel, paintKey);
         }
      }


      private void DrawColorKey(){
       // -------------------------------------------------
         var circlePaint = new SKPaint {
            StrokeWidth = 4,
            Style = SKPaintStyle.Fill,
            StrokeCap = SKStrokeCap.Round
         };

         var textPaint = new SKPaint {
            StrokeWidth = 3,
            TextSize = 40f,
            Color = Color.Black.ToSKColor(),
            Style = SKPaintStyle.Fill,
            StrokeCap = SKStrokeCap.Round
         };

         void Draw1Key(string text, float x, float y, SKColor circleColor) {
         // ----------------------------------------------------------------
            var path = new SKPath();
            path.AddCircle(x, y, 16.0f, SKPathDirection.Clockwise);
            circlePaint.Color = circleColor;
            canvas.DrawPath(path, circlePaint);
            canvas.DrawText(text, x+40, y+12, textPaint);
         }

         float x1 = 120;
         float xDelta = Wid(0.2f);

         var path1 = new SKPath();
         //path1.AddRect(new SKRect(x1-20, 50, x2 + 30, 200));
         //canvas.DrawPath(path1, textPaint);
         canvas.DrawText("Key:", x1, 60, textPaint);

         Draw1Key("hr", x1, 120, Color.Red.ToSKColor());
         Draw1Key("3b", x1+xDelta, 120, Color.Yellow.ToSKColor());
         Draw1Key("2b", x1+2*xDelta, 120, Color.Blue.ToSKColor());
         Draw1Key("1b", x1+3*xDelta, 120, Color.Green.ToSKColor());

         Draw1Key("bb", x1, 180, Color.Brown.ToSKColor());
         Draw1Key("so", x1+xDelta, 180, Color.Black.ToSKColor());
         Draw1Key("other", x1+2*xDelta, 180, Color.Gray.ToSKColor());

      }





      //      private void WriteWedgeLabel(string lbl, nfloat x, nfloat y, CGContext ctxt) {
      //      // ----------------------------------------------------------------
      //         if (lbl == "") return;

      //         ctxt.SetLineWidth(1.0f);
      //         ctxt.SetStrokeColor(UIColor.DarkGray.CGColor); 
      //         ctxt.SetFillColor(UIColor.DarkGray.CGColor); 
      ////         UIColor.DarkGray.SetStroke ();
      ////         UIColor.DarkGray.SetFill(); 

      //         ctxt.SetTextDrawingMode(CGTextDrawingMode.FillStroke);
      //         ctxt.SelectFont("Helvetica", this.segmentLabelFontSize, CGTextEncoding.MacRoman);

      //         ctxt.ShowTextAtPoint(x, y, lbl);

      //      } 

   }

}

