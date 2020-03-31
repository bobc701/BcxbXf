using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using BcxbXf.Models;
using BcxbXf.Views;
using BCX.BCXB;


namespace BcxbXf.Views {

   public struct CLineupChange
   {
      public char type;
      public int x, y; // x replaced by y
      public int p; // Position being replaced, if applicable.
      public int q; // New position (x being moved from p to q.)
      public side abMng;

   }


   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class LineupCardPage : ContentPage {
   // ----------------------------------------------------


      public enum EGameState { PreGame, Offense, Defense };
      public EGameState gameState;
      public CGame g;
      public side abMng; //The side (home or vis) being managed here.
      public side abGame;    //The side currently at bat in the game
      public CLineupCard lineupCard;


   // Since multiple changes are enabled, more than one of these can
   // be set simultabeously. These are so that on returning to the main 
   // screen, screen can be refreshed as needed...
      public bool pinchHitter = false;
      public bool pinchRunner = false;
      public bool newPitcher = false;
      public bool fieldingChange = false;

      public CLineupChange operation;
      private AvailablePage fAvail;


      public LineupCardPage(CGame g1, int side1 = 0) {

         InitializeComponent();
         g = g1;
         lineupCard = new CLineupCard(g1, (side)side1);
         this.BindingContext = lineupCard;
         this.Title = "Lineup Card";

         if (g.UsingDh && gameState == EGameState.PreGame) {
            // Allow to choose dh...
            pkrPosn.ItemsSource = new string[] { "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf", "dh", "Cancel" };
         }
         else {
            // Cannot choose dh...
            pkrPosn.ItemsSource = new string[] { "c", "1b", "2b", "3b", "ss", "lf", "cf", "rf", "Cancel" };
         }

         ConfigureControls();

         Appearing += delegate (object sender, EventArgs e) {
            // ---------------------------------------------------------
            Console.WriteLine("We've returned from Available!");
            HandleChangeRequest();

         };

      }


         async private void HandleChangeRequest() {
         // -------------------------------------------------------------

            string msg = "";
            switch (this.operation.type) {

               case 'h': // Pinch hit...
                  if (this.fAvail.SelectedPlayer == null) return;
                  operation.x = ((CBatter)lstCard.SelectedItem).bx;
                  operation.y = fAvail.SelectedPlayer.bx;
                  msg =
                     "Pinch hit " + g.t[(int)abMng].bat[operation.y].bname +
                     " for " + g.t[(int)abMng].bat[operation.x].bname;
                  //EnableControl(cmdDoIt, true);
                  //cmdDoIt.IsVisible = true;
                  break;

               case 'r':
                  if (this.fAvail.SelectedPlayer == null) return;
                  operation.x = ((CBatter)lstCard.SelectedItem).bx;
                  operation.y = fAvail.SelectedPlayer.bx;
                  msg =
                     g.t[(int)abMng].bat[operation.y].bname + " pinch runner for " +
                     g.t[(int)abMng].bat[operation.x].bname;
                  //cmdDoIt.IsVisible = true;
                  break;

               case 's':
                  // Defensive replacement...
                  if (this.fAvail.SelectedPlayer == null) return;
                  CBatter batx = (CBatter)lstCard.SelectedItem;
                  CBatter baty = fAvail.SelectedPlayer;
                  operation.x = batx.bx;
                  operation.y = fAvail.SelectedPlayer.bx;

                  if (gameState != EGameState.PreGame && batx.where == 10) {
                     await DisplayAlert("Lineup change", "Replacing DH is not supported. (Pinch hit instead.)", "OK");
                  }
                  else {
                  // New player (y) goes at same posn as old (x), unless he is a pitcher,
                  // then he goes in as a pitcher...
                     if (baty.px != 0) //new player is a pitcher, use p
                        operation.p = 1;
                     else if (batx.px == 0 && baty.px == 0) //both are non-pitchers, use curr pos
                        operation.p = batx.where;
                     else
                        operation.p = 0; //new is non-p, old is p, use 0.

                     msg = "Replace " + batx.bname + " with " + baty.bname;
                     if (operation.p != 0) msg += " as " + CGame.posAbbr[operation.p];
                        //cmdDoIt.IsVisible = true;
                  }

                  break;
               ;
               default:
                  break;

            }

            if (msg != "") {
               bool ans = await DisplayAlert("", msg + "?", "Do it!", "No");
               if (ans) {
                  MakeTheChange();

               }
            }

         }


      

      private void btnHomeCard_Clicked(object sender, EventArgs e) {
      // --------------------------------------------------------------
         BindingContext = lineupCard = new CLineupCard(g, side.home);
         this.abMng = side.home;
         this.lstCard.SelectedItem = null;
         btnHomeCard.BackgroundColor = Color.White;
         btnVisCard.BackgroundColor = Color.LightGray;
         ConfigureControls();
      }


      private void btnVisCard_Clicked(object sender, EventArgs e) {
      // --------------------------------------------------------------
         BindingContext = lineupCard = new CLineupCard(g, side.vis);
         this.abMng = side.vis;
         this.lstCard.SelectedItem = null;
         btnVisCard.BackgroundColor = Color.White;
         btnHomeCard.BackgroundColor = Color.LightGray;
         ConfigureControls();
      }


      private void EnableControl(VisualElement ctl, bool on) {
         // ---------------------------------------------------------------
         if (on) {
            ctl.IsEnabled = true;
            //btn.TitleColor(UIControlState.Normal) = UIColor.Blue;
         }
         else {
            ctl.IsEnabled = false;
            //btn.TitleColor(UIControlState.Disabled) = UIColor.DarkGray;
         }
      }


      private void ConfigureControls() {
         // ----------------------------

         abGame = (side)g.ab;

         if (g.PlayState == PLAY_STATE.START || g.PlayState == PLAY_STATE.NONE) {
            gameState = EGameState.PreGame;
         }
         else {
            if (abGame == abMng) gameState = EGameState.Offense;
            else gameState = EGameState.Defense;
         }

         ////dgvLineupCard.Source = new CLineupCardSource((int)abMng, this);

         ////EnableControl(cmdEditFielding, false);
         ////EnableControl(cmdSaveLineup, false);

         ////cmdEditFielding.Hidden = true;
         ////cmdSaveLineup.Hidden = true;

         ////SetButtonColors(cmdMoveUp);
         ////SetButtonColors(cmdMoveDown);
         ////SetButtonColors(cmdPinchHit);
         ////SetButtonColors(cmdPinchRun);
         ////SetButtonColors(cmdReplace);
         ////SetButtonColors(cmdChangePos);
         ////SetButtonColors(cmdEditFielding);
         ////SetButtonColors(cmdSaveLineup);
         ////SetButtonColors(cmdDoIt);
         ////SetButtonColors(cmdDone);

         if (lstCard.SelectedItem == null) { 
            EnableControl(cmdMoveUp, false);
            EnableControl(cmdMoveDown, false); 
            EnableControl(cmdPinchHit, false);
            EnableControl(cmdPinchRun, false); 
            EnableControl(cmdReplace, false);
            //EnableControl(cmdDoIt, false);
            EnableControl(pkrPosn, false);
            return;
         }

         ////SetLabelBorder(lblAction, UIColor.Brown, 1);

         switch (gameState) {
            case EGameState.PreGame:
               EnableControl(cmdMoveUp, true);
               EnableControl(cmdMoveDown, true);
               EnableControl(cmdPinchHit, false);
               EnableControl(cmdPinchRun, false);
               EnableControl(cmdReplace, true);
               EnableControl(pkrPosn, true);
               //EnableControl(cmdDoIt, false);
               break;

            case EGameState.Offense:
               EnableControl(cmdMoveUp, false);
               EnableControl(cmdMoveDown, false);
               EnableControl(cmdPinchHit, false);
               EnableControl(cmdPinchRun, false);
               EnableControl(cmdReplace, false);
               EnableControl(pkrPosn, false);
               //EnableControl(cmdDoIt, false);
               break;

            case EGameState.Defense:
               EnableControl(cmdMoveUp, false);
               EnableControl(cmdMoveDown, false);
               EnableControl(cmdPinchHit, false);
               EnableControl(cmdPinchRun, false);
               EnableControl(cmdReplace, true);
               EnableControl(pkrPosn, true);
               //EnableControl(cmdDoIt, false);
               break;

            default:
               break;

         }

      }

      async private void cmdPinchHit_Clicked(object sender, EventArgs e) {
      // -----------------------------------------------------------

         lineupCard.GetAvailable('n');
         this.operation.type = 'h';
         this.operation.x = ((CBatter)lstCard.SelectedItem).bx;
         this.fAvail = new AvailablePage(lineupCard, g.t[(int)abMng].nick, 'n');
         await Navigation.PushModalAsync(fAvail);


      }

      private void cmdMoveUp_Clicked(object sender, EventArgs e) {
      // ------------------------------------------------------------
         if (lstCard.SelectedItem == null) return;
         CBatter bat = (CBatter)lstCard.SelectedItem;
         lineupCard.MovePlayerUpDown(bat.bx, -1); //-1 is up
         lineupCard.SetLineupCard();
         if (bat.when >= 1 && bat.when <= 9)
            lstCard.SelectedItem = lineupCard.CurrentLineup[bat.when-1];

      }


      private void cmdMoveDown_Clicked(object sender, EventArgs e) {
      // ------------------------------------------------------------
         if (lstCard.SelectedItem == null) return;
         CBatter bat = (CBatter)lstCard.SelectedItem;
         lineupCard.MovePlayerUpDown(bat.bx, 1); //+1 is down
         lineupCard.SetLineupCard();
         if (bat.when >= 1 && bat.when <= 9)
            lstCard.SelectedItem = lineupCard.CurrentLineup[bat.when - 1];

      }

      async private void cmdPinchRun_Clicked(object sender, EventArgs e) {
      // ------------------------------------------------------------------
         lineupCard.GetAvailable('n');
         this.operation.type = 'r';
         this.operation.x = ((CBatter)lstCard.SelectedItem).bx;
         this.fAvail = new AvailablePage(lineupCard, g.t[(int)abMng].nick, 'n');
         await Navigation.PushModalAsync(fAvail);

      }


      async private void cmdReplace_Clicked(object sender, EventArgs e) {
      // -----------------------------------------------------------------
         char ch = 'a';
         CBatter bat1 = ((CBatter)lstCard.SelectedItem);
         if (g.UsingDh && bat1.px != 0) ch = 'p';
         if (g.UsingDh && bat1.px == 0) ch = 'n';
         lineupCard.GetAvailable(ch);
         operation.type = 's';
         this.operation.x = ((CBatter)lstCard.SelectedItem).bx;
         this.fAvail = new AvailablePage(lineupCard, g.t[(int)abMng].nick, ch);
 
         await Navigation.PushModalAsync(fAvail);

      }


      async private void pkrPosn_SelectedIndexChanged(object sender, EventArgs e) {
         // -----------------------------------------------------------------------
         if (pkrPosn.SelectedIndex == -1) return;
         if (pkrPosn.SelectedIndex + 1 >= pkrPosn.ItemsSource.Count) return; //This s/b 'Cancel'
         int newPos = pkrPosn.SelectedIndex + 2; // dh=10

         CBatter bat = ((CBatter)lstCard.SelectedItem);

         operation.type = 'p';
         operation.x = bat.bx; 
         operation.p = newPos;
         operation.abMng = abMng;

         string msg = "";

         if (bat.where == newPos) 
            await DisplayAlert("", "Fielder is already at position", "OK");
         if (bat.where == 1) 
            await DisplayAlert("", "Moving a pitcher to the field is not supported", "OK");
         else if (gameState != EGameState.PreGame && newPos == 10) 
            await DisplayAlert("", "Can't move player from field to DH", "OK");
         else if (gameState != EGameState.PreGame && bat.where == 10) 
            await DisplayAlert("", "Can't move DH to the field", "OK");
         else {
            msg = "Move " + bat.bname + " to " + CGame.PosName[newPos];
            bool ans = await DisplayAlert("", msg + "?", "Do it!", "No");
            if (ans) {
               MakeTheChange();
            }
            //cmdDoIt.IsVisible = true;
         }
         pkrPosn.SelectedIndex = -1;
      }


      private void MakeTheChange() {
         // ----------------------------------------------------------

         void CleanUp() {
            // --------------------------------------
            //this.cmdDoIt.Text = "";
            //this.cmdDoIt.IsVisible = false;
         }

         try {
            switch (this.operation.type) {
               case 'h':
               case 'r':
                  lineupCard.ReplacePlayer(operation.x, operation.y);
                  lineupCard.SetLineupCard();
                  CleanUp();
                  //DisplayAlert("Lineup change", "Change made!", "OK");
                  break;

               case 's':
                  CBatter bx = g.t[(int)abMng].bat[operation.x];
                  CBatter by = g.t[(int)abMng].bat[operation.y];
                  if (gameState != EGameState.PreGame && bx.where == 10) {
                     DisplayAlert("Lineup change", "Replacing DH is not supported. (You can pinch hit.)", "Got it");
                  }
                  else {
                     lineupCard.ReplacePlayer(operation.x, operation.y);
                     // If new player is a p-type, he goes in as p. Else as old player's pos....
                     lineupCard.AssignPos(operation.y, by.px == 1 ? 1 : operation.p);
                     lineupCard.SetLineupCard();
                     CleanUp();
                     //DisplayAlert("Lineup change", "Change made!", "OK");
                  }
                  break;

               case 'p':
               // Validation was performed in picker changed event handler...
                  lineupCard.AssignPos(operation.x, operation.p);
                  lineupCard.SetLineupCard();
                  CleanUp();
                  //DisplayAlert("Lineup change", "Change made!", "OK");
                  break;

            }

            switch (this.operation.type) {
               case 'h': this.pinchHitter = true; break;
               case 'r': this.pinchRunner = true; break;
               case 's': this.fieldingChange = true; break;
               case 'p': this.fieldingChange = true; break;
            }

         }

         catch (Exception ex) {
         // AssignPos will throw exception if it detects you are putting a
         // non-pitcher in as pitcher. (Based on his .px property being 0.)
         // I guess restricting the available list to pitchers would be
         // cleaner... future change.
         // So do nothing here.

            DisplayAlert("Lineup change", ex.Message, "OK");

         }

      }


      private void lstCard_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
         // -------------------------------------------------------------------------------
         CBatter bat1 = (CBatter)e.SelectedItem;
         if (bat1 == null) return; //Should not happen
         bool isFielder = (bat1.where >= 1 && bat1.where <= 9 || bat1.where == 0);

         switch (gameState) {

            case EGameState.PreGame:
               EnableControl(cmdPinchHit, false);
               EnableControl(cmdPinchRun, false);
               EnableControl(cmdReplace, true);
               EnableControl(pkrPosn, bat1.where != 1);
               EnableControl(lblPosn, bat1.where != 1);
               EnableControl(cmdMoveUp, bat1.when != 0 && bat1.when > 1);
               EnableControl(cmdMoveDown, bat1.when != 0 && bat1.when < 9);
               break;

            case EGameState.Offense:
               EnableControl(cmdPinchHit, bat1.DisplayBase == "ab");
               EnableControl(cmdPinchRun,
                  bat1.DisplayBase == "1st" ||
                  bat1.DisplayBase == "2nd" ||
                  bat1.DisplayBase == "3rd");
               EnableControl(cmdReplace, false);
               EnableControl(pkrPosn, false);
               EnableControl(lblPosn, false);
               EnableControl(cmdMoveUp, false);
               EnableControl(cmdMoveDown, false);
               break;

            case EGameState.Defense:
               EnableControl(cmdReplace, isFielder);
               EnableControl(pkrPosn, isFielder && bat1.where != 1);
               EnableControl(lblPosn, isFielder && bat1.where != 1);
               EnableControl(cmdMoveUp, false);
               EnableControl(cmdMoveDown, false);
               break;
         }

      }

   }
}