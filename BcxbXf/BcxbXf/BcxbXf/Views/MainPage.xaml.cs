using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Xamarin.Forms;

using BCX.BCXB;
using BCX.BCXCommon;
using BcxbXf.Views;
using BcxbDataAccess;


namespace BcxbXf
{

   public partial class MainPage : ContentPage  {
   
      CGame mGame;

      public bool SpeechOn = true;
      private bool IsFieldingPlay = false;
      //private GProfileDisk disk1 = null;

      //public string[] selectedTeams = new string[2];
      private PickTeamsRealPage fPickTeamsReal { get; set; } = null;
      private PickTeamsCustPage fPickTeamsCust { get; set; } = null;
      private PickTeamsPrepPage fPickTeamsPrep { get; set; } = null;
      private LineupCardPage fLineup { get; set; } = null;
      private PlaysPage fPlays { get; set; } = null;
      private OptionsPage fOptions { get; set; } = null;

      private bool pinchHitter, pinchRunner, nwPitcher;
      //private LineupCardController fLineup;
      //private OptionsController fOptions;
      //private SpecialPlaysController fPlays;

      private string returningFrom = "";

      // Structures that map to form elements...
      Label[] txtAbbrev = new Label[2];
      Label[,] grdLinescore = new Label[2, 13];
      Label[,] grdRHE = new Label[2, 3];
      Label[] grdInning = new Label[13];

      Label[] lblRunner = new Label[4];
      Label[] lblFielder = new Label[10];



      public MainPage() {
         
         InitializeComponent();
         ViewDidLoad();
         EnableControls();


         Appearing += async delegate (object sender, EventArgs e) {
          
            Debug.WriteLine("We've returned from " + returningFrom);

            switch (sender) {

               case MainPage f:
                  break;

               case OptionsPage f:
                  int i = 1;
                  f.SpeechOn = true;
                  break;

               case PlaysPage f:
                  int y = 7;
                  f.Play = SPECIAL_PLAY.AtBat;
                  break;

            }

            switch (this.returningFrom) {

               case "PickTeamsPage":

                  try {
                     for (int i = 0; i <= 1; i++) {
                        if (fPickTeamsPrep?.SelectedTeams[i].Year == 0 &&
                            fPickTeamsPrep?.SelectedTeams[1].UserTeamID == 0) return; //User did not pick
                     }

                     Debug.WriteLine("Vititing team: " + fPickTeamsPrep.SelectedTeams[0]);
                     Debug.WriteLine("Home team: " + fPickTeamsPrep.SelectedTeams[1]);

                     txtResults.Text = "\nLoading player stats. Please wait..."; //#3000.02... note 'async delegate' above.
                     Activity2.IsVisible = true;
                     Activity2.IsRunning = true;
                     await SetupNewGame(fPickTeamsPrep.SelectedTeams);
                     txtResults.Text =
                        "\nTap 'Mng' above to change starting lineups." +
                        "\nWhen done, tap 'Start' below." +
                        "\n\nMake sure phone is not in silent mode" +
                        "\nto hear audio play-by-play.";
                     Activity2.IsRunning = false;
                     Activity2.IsVisible = false;

                     fPickTeamsReal = null;

                  }
                  catch (Exception ex) {
                     DisplayAlert("Error", ex.Message, "Dismiss");
                     Activity2.IsRunning = false;
                     Activity2.IsVisible = false;

                  }
                  break;

               case "LineupCardPage":
                  if (fLineup.pinchHitter || fLineup.pinchRunner) mGame.InitBatter();
                  if (fLineup.fieldingChange) ShowFielders(mGame.fl);
                  fLineup = null;
                  break;

               case "PlaysPage":
                  mGame.specialPlay = fPlays.Play;
                  fPlays = null;
                  break;

               case "OptionsPage":
                  mGame.runMode = fOptions.RunMode;
                  this.SpeechOn = fOptions.SpeechOn;
                  fOptions = null;
                  break;

               case "FieldingDiskPage":
                  this.IsFieldingPlay = false;
                  btnProfileDisks.Source = "bat_img1a.png";
                  break;

               case "AboutPage":
                  break;

            };
            returningFrom = "";

         };

      }


      private void ViewDidLoad() {
         
         //GFileAccess.SetFolders();
         try {
            mGame = new CGame();
            AssignEventHandlers();
            //mGame.SetupEngineAndModel();
            EnginePrep.SetupSimEngine(mGame);
            SetupScreen();
            txtResults.Text =
              "\nTap 'New Game' above to get started.";
            //imgDiamond.Source = ImageSource.FromFile("test9.png");
         }
         catch (Exception ex) {
            DisplayAlert("Error", "Error loading Main Page:\r\n" + ex.Message, "OK");
         }

      }


      public void btnGo_Clicked(object sender, EventArgs e) {
      
         bool clocking = false;
         //BCX.BCXCommon.CActivityIndicator acty = null;
         btnGo.Text = "";
         btnGo.IsEnabled = false;
         //cmdInfo.Enabled = false;

         if (mGame.IsFastRunMode) {
            clocking = true;
         }


         int result = mGame.Go1();
         //if (clocking) acty.Hide(); //Can't rely on IsFatRunMode at end of game.

      // Important not to normally call EnableControls here, because and 'await' in
      // EShowResults will pop the control back here and so btnGo will be enabled while
      // the results are still rolling. But if there's a validity error, namely like
      // defense not correct after a pinch hit, then we *do* need to enable control here
      // or else it will never be done.
      // ------------------------------------------------------------------------------
         if (result != 0) 
            EnableControls(); //1907.01: Enable Play after defence message.
      }


      private async Task SetupNewGame(CTeamRecord[] newTeams) {
         // --------------------------------------------------------
         // Have just returned from PickTeams, and have the selected teams
         // in newTeams[] which is array of string...

         //mGame.UsingDh = selectedTeams[1].UsesDh; 

         if ((newTeams[0].Year == 0 && newTeams[0].UserTeamID == 0) ||       
             (newTeams[1].Year == 0 && newTeams[1].UserTeamID == 0)) return; //User did not pick. 

         // User has selected 2 teams for new game.
         // So set up the two CTeam objects for the 2 teams...
         mGame.t = new CTeam[2];
         mGame.t[0] = new CTeam(mGame);
         mGame.t[1] = new CTeam(mGame);

         mGame.cmean = new CHittingParamSet(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);
         mGame.PlayState = PLAY_STATE.START;

         try {
            string tm = newTeams[1].TeamTag.Trim();
            int yr = newTeams[1].Year;
            int teamID = newTeams[1].UserTeamID; //For custom teams
            DTO_TeamRoster ros;
            switch (teamID) {
               case 0:  ros = await DataAccess.GetTeamRosterOnLine(tm, yr); break;
               default: ros = await DataAccess.GetCustTeamRoster(teamID); break;
            }  
            if (ros == null) throw new Exception($"Error: Could not load data for team, {newTeams[1].NickName}");
            mGame.t[1].ReadTeam(ros, 1);
         }
         catch (Exception ex) {
            throw new Exception($"Error loading data for team, {newTeams[1]}\r\n{ex.Message}");
         }

         try {
            string tm = newTeams[0].TeamTag.Trim(); 
            int yr = newTeams[0].Year;
            int teamID = newTeams[0].UserTeamID; //For custom teams
            DTO_TeamRoster ros;
            switch (teamID) {
               case 0: ros = await DataAccess.GetTeamRosterOnLine(tm, yr); break;
               default: ros = await DataAccess.GetCustTeamRoster(teamID); break;
            }
            if (ros == null) throw new Exception($"Error: Could not load data for team, {newTeams[0].NickName}");
            mGame.t[0].ReadTeam(ros, 0);
         }
         catch (Exception ex) {
            throw new Exception($"Error loading data for team, {newTeams[0]}\r\n{ex.Message}");
         }

         //This is now obs due to each B & P having own lgMean's... #2101.01
         //mGame.cmean.CombineLeagueMeans(mGame.t[0].lgMean, mGame.t[1].lgMean);

         mGame.InitGame();
         ShowRHE();               //<---- Put these back in!
         InitLinescore();
         ShowFielders(1);
         ShowRunners();

         EnableControls();



      }

      private void SetupScreen() {
         

         // =====================================
         // Fielding and base runner display area
         // =====================================
         lblRunner[1] = lblRunner1;
         lblRunner[2] = lblRunner2;
         lblRunner[3] = lblRunner3;

         lblFielder[1] = lblFielder1;
         lblFielder[2] = lblFielder2;
         lblFielder[3] = lblFielder3;
         lblFielder[4] = lblFielder4;
         lblFielder[5] = lblFielder5;
         lblFielder[6] = lblFielder6;
         lblFielder[7] = lblFielder7;
         lblFielder[8] = lblFielder8;
         lblFielder[9] = lblFielder9;


         // ================================
         // Linescore setup
         // ================================
         // Point internal structures at form structures for line score...
         txtAbbrev[0] = txtAbbrev0;
         txtAbbrev[1] = txtAbbrev1;

         grdLinescore[0, 1] = grdLinescore001;
         grdLinescore[0, 2] = grdLinescore002;
         grdLinescore[0, 3] = grdLinescore003;
         grdLinescore[0, 4] = grdLinescore004;
         grdLinescore[0, 5] = grdLinescore005;
         grdLinescore[0, 6] = grdLinescore006;
         grdLinescore[0, 7] = grdLinescore007;
         grdLinescore[0, 8] = grdLinescore008;
         grdLinescore[0, 9] = grdLinescore009;
         //grdLinescore[0, 10] = grdLinescore010;
         //grdLinescore[0, 11] = grdLinescore011;
         //grdLinescore[0, 12] = grdLinescore012;

         grdLinescore[1, 1] = grdLinescore101;
         grdLinescore[1, 2] = grdLinescore102;
         grdLinescore[1, 3] = grdLinescore103;
         grdLinescore[1, 4] = grdLinescore104;
         grdLinescore[1, 5] = grdLinescore105;
         grdLinescore[1, 6] = grdLinescore106;
         grdLinescore[1, 7] = grdLinescore107;
         grdLinescore[1, 8] = grdLinescore108;
         grdLinescore[1, 9] = grdLinescore109;
         //grdLinescore[1, 10] = grdLinescore110;
         //grdLinescore[1, 11] = grdLinescore111;
         //grdLinescore[1, 12] = grdLinescore112;

         grdRHE[0, 0] = grdRHE00;
         grdRHE[0, 1] = grdRHE01;
         grdRHE[0, 2] = grdRHE02;
         grdRHE[1, 0] = grdRHE10;
         grdRHE[1, 1] = grdRHE11;
         grdRHE[1, 2] = grdRHE12;

         grdInning[1] = grdInning01;
         grdInning[2] = grdInning02;
         grdInning[3] = grdInning03;
         grdInning[4] = grdInning04;
         grdInning[5] = grdInning05;
         grdInning[6] = grdInning06;
         grdInning[7] = grdInning07;
         grdInning[8] = grdInning08;
         grdInning[9] = grdInning09;
         //grdInning[10] = grdInning10;
         //grdInning[11] = grdInning11;
         //grdInning[12] = grdInning12;

      }


      private void ShowRHE() {
      // -------------------
         grdRHE[0, 0].Text = mGame.rk[0, 0].ToString();
         grdRHE[1, 0].Text = mGame.rk[1, 0].ToString();
         grdRHE[0, 1].Text = mGame.rk[0, 1].ToString();
         grdRHE[1, 1].Text = mGame.rk[1, 1].ToString();
         grdRHE[0, 2].Text = mGame.rk[0, 2].ToString();
         grdRHE[1, 2].Text = mGame.rk[1, 2].ToString();

         //for (int i = 0; i <= 1; i++) for (int j = 0; j <= 2; j++) {
         //      ///grdRHE[i,j].Refresh();
         //   }

      }


      private void ShowRunners() {
         
         //if (mGame.IsFastRunMode) return;
         for (int b = 1; b <= 3; b++) {
            lblRunner[b].Text = mGame.r[b].name;
            ///lblRunner[b].Refresh();
         }
         lblBatter.Text = mGame.r[0].name; lblBatter.IsVisible = true;
         ///lblBatter.Refresh();
         ///this.Refresh();
      }


      private void ShowRunnersOnly() {
         
         // This version of ShowRunners leaves the batter blank.
         //if (mGame.IsFastRunMode) return;
         for (int b = 1; b <= 3; b++) { lblRunner[b].Text = mGame.r[b].name; lblRunner[b].IsVisible = true; }
         lblBatter.Text = ""; lblBatter.IsVisible = true;
         ///lblBatter.Refresh();
         ///this.Refresh();
      }


      private void ShowFielders(int fl) {
         
         for (int p = 1; p <= 9; p++) {
            if (mGame.t[fl].who[p] > 0)
               lblFielder[p].Text = mGame.t[fl].bat[mGame.t[fl].who[p]].bname;
            else
               lblFielder[p].Text = "???";
            lblFielder[p].IsVisible = true;
            ///lblFielder[p].Refresh();
         }
      }



      private void AssignEventHandlers() {
         // ---------------------------------------------------------------   

         // These are the Stream fetching events. Needed because, while a PCL can 
         // process Streams, it can't open one based on file name.
         //

         // REBUILD THIS FROM IOS APP

         //mGame.ERequestModelFile += delegate (short n) {
         //   // --------------------------------------------------------------------
         //   var rdr = GFileAccess.GetModelFile(n);
         //   return rdr;
         //};

         //mGame.ERequestEngineFile += delegate (string fName) {
         //   // ------------------------------------------------------------------------
         //   var rdr = GFileAccess.GetModelFile(fName);
         //   return rdr;
         //};

         ////mGame.ERequestTeamFileReader += delegate (string fName) {
         ////   // --------------------------------------------------------------------
         ////   var rdr = GFileAccess.GetTeamFileReader(fName);
         ////   return rdr;
         ////};

         //mGame.ERequestTeamFileWriter += delegate (string fName) {
         //   // --------------------------------------------------------------------
         //   var rdr = GFileAccess.GetTeamFileWriter(fName);
         //   return rdr;
         //};

         //mGame.ERequestBoxFileWriter += delegate (string fName) {
         //   // --------------------------------------------------------------------
         //   var rdr = new StreamWriter(fName);
         //   return rdr;
         //};

         mGame.EShowFielders += ShowFielders;
         mGame.EShowLinescore += ShowLinescoreOne;
         mGame.EShowLinescoreFull += ShowLinescoreFull;
         mGame.EInitLinescore += InitLinescore;
         mGame.EShowRHE += ShowRHE;


         mGame.EShowResults += async delegate (int scenario) {
            /* -------------------------------------------------------------------
                  *  There are 2 parts to ShowResults -- The part in Cgame, whatever it 
                  *  wants to do with the msg, like keep a scrolling log, and then the 
                  *  hand-off to the client, namely the form, which will display it in 
                  *  the text box. This is the client part.
                  *  
                  *  We might add a scrolling store of whole-game play by play here.
                  *  Possibly a List of string.
                  */
            //SpeechSynthesizer s = new SpeechSynthesizer();
            //s.SelectVoiceByHints(VoiceGender.Male, VoiceAge.Adult)

            //if (mGame.IsFastRunMode) {
            //   mGame.lstResults.Clear();
            //   return;
            //}

            TextToSay[] list1 = mGame.lstResults.ToArray();

            if (!mGame.IsFastRunMode) {
               foreach (var txt in list1) {
                  if (txt.action == 'X') {
                     txtResults.Text = "";
                     await Task.Delay(100);
                  }
                  else {
                     if (txtResults.Text == "") txtResults.Text = txt.msg;
                     else txtResults.Text += txt.delim + txt.msg;
                     if (SpeechOn && !mGame.IsFastRunMode && txt.msg != "...") {
                        //tts.Speak(txt.msg); //#1604.03
                        var s = new System.Text.StringBuilder(txt.msg);
                        s.Replace("0 out", "none out");
                        s.Replace(" 0", " nothing");
                        if (s[1] == '.') s.Remove(0, 2); //Don't say player's initial
                        //tts.Speak(s.ToString());
                        DependencyService.Get<BcxbXf.Services.ITextToSpeach>().Speak(s.ToString()); //-1906.01:
                     }
                     if (txt.delay)
                        await Task.Delay(mGame.runMode == CGame.RunMode.FastEOP ? 100 : 1200);
                     else
                        await Task.Delay(100);
                  }
               }
            }
            else {
               string s = "";
               foreach (var txt in list1) {
                  if (txt.action == 'X') {
                     s = "";
                  }
                  else {
                     if (s == "") s = txt.msg;
                     else s += txt.delim + txt.msg;
                  }
               }
               txtResults.Text = s;
               await Task.Delay(100);
            }
            mGame.lstResults.Clear();

            // #1510.01:
            // Run these here
            switch (scenario) {
               case 1: ShowRunners(); /*segProfileDisks.SetEnabled(false, 1); cmdInfo.Enabled = true;*/ break;
               case 2: ShowRunnersOnly(); break;
            }
            PostOuts();
            await Task.Delay(100);

            // Here is where to show the disk...
            string sBatter = mGame.CurrentBatterName;
            string sPitcher = mGame.CurrentPitcherName;

            //disk1.Init(150, 220, mGame.cpara);
            //disk1.DiceRoll = mGame.diceRollBatting;
            //disk1.ProfileLabel = sBatter + " vs. " + sPitcher + ":";
            //disk1.SubLabel1 = "";
            //disk1.SetLabelProperties(UIColor.Black.CGColor, 18f); // 18f);
            //disk1.Hidden = false;
            //disk1.ClearsContextBeforeDrawing = true;
            //disk1.SetNeedsDisplay();
            //cmdInfo.Enabled = true;

            EnableControls(); //*1

            //            var s = txtResults.Text;
            //            if (s == "") {
            //               txtResults.Text = s; 
            //               Task.Delay(1500);
            //            }
            //            else 
            //               txtResults.Text += delim + s;
            //            if (SpeechOn && !mGame.IsFastRunMode) voice.Speak(msg); //#1505.02

            ///txtResults.Refresh();
         };


         mGame.EFmtFieldingBar += delegate (
            CFieldingParamSet fPar, string labels, string fielderName) {
            
               if (mGame.runMode != CGame.RunMode.Normal && mGame.runMode != CGame.RunMode.FastEOP) return;
               this.IsFieldingPlay = true;
               btnProfileDisks.Source = "glove_img1a.png";
            };
  

         mGame.EClearResults += async () => {
            
            //if (mGame.IsFastRunMode) return;
            txtResults.Text = "";
            await Task.Yield();
            ///txtResults.Refresh();

         };


         mGame.ENotifyUser += delegate (string s) {
         
            DisplayAlert("", s, "OK");

         };

      }


      public void PostOuts() {
         
         lblOuts1.Source = mGame.ok > 0 ? "redball" : "whtball";
         lblOuts2.Source = mGame.ok > 1 ? "redball" : "whtball";
         lblOuts3.Source = mGame.ok > 2 ? "redball" : "whtball";
      }




   // -----------------------------------------------------------------------
   // This section has routines for displaying the line score, including what
   // happens when there are extra innings, and the left & right shift buttons.
   // -----------------------------------------------------------------------


      private int LinescoreStartInning = 1;


      private void InitLinescore() {
      // ---------------------------------------------------------
      // This just blanks out the whole linescore, and posts inning
      // numbers (not scores!) up to 9, or the current inning if more.

         for (int ab1 = 0; ab1 <= 1; ab1++) {
            txtAbbrev[ab1].Text = mGame.t[ab1].lineName;
            for (int i = 1; i <= 9; i++) grdLinescore[ab1, i].Text = "";
         }
         int iOff = LinescoreStartInning - 1;
         for (int i = 1; i <= 9; i++) grdInning[i].Text = (i + iOff).ToString();

         // Visibility of the 2 shift buttons...
         cmdShiftLeft.IsEnabled = cmdShiftLeft.IsVisible = mGame.inn > 9;   
         cmdShiftRight.IsEnabled = cmdShiftRight.IsVisible = mGame.inn > 9; 
         
      }



      private void ShowLinescoreOne() {
      // ---------------------------------------------------------------
      // This just posts current inning (mGame.inn) for team at bat (mGame.ab).
      // If the inning is not in current range, it adjusts the display.
      // 1706.23

         if (mGame.inn > LinescoreStartInning + 8) {
         // Inning is not in current range, must adjust...
            do LinescoreStartInning += 9; while (mGame.inn > LinescoreStartInning + 8);
            ShowLinescoreFull();
         }
         else {
         // Just post single half-inning score...  
            int iOff = LinescoreStartInning - 1;
            grdLinescore[mGame.ab, mGame.inn - iOff].Text = mGame.lines[mGame.ab, mGame.inn].ToString();
            ShowRHE();
         }

      }


      private void ShowLinescoreFull() {
      // --------------------------------------------------------
      // This posts the entire linescore for game so far...
      // But just showing visible innings (based on LinescoreStartInning).

         int maxDisplayedInn;
         int iOff = LinescoreStartInning - 1;

         int actualInn(int displayedInn) => displayedInn + iOff;

         // Blank out existiing...
         InitLinescore();

         // Visitors...
         maxDisplayedInn = Math.Min(9, mGame.inn - iOff);
         for (int i = 1; i <= maxDisplayedInn; i++)
            grdLinescore[0, i].Text = mGame.lines[0, actualInn(i)].ToString();

         // Home...
         // Back off 1 inning unless 9th displayed inning is less than actual inn.
         if (mGame.ab == 0 && !(maxDisplayedInn == 9 && (actualInn(9) <  mGame.inn))) maxDisplayedInn--; // <--Not tested!
         for (int i = 1; i <= maxDisplayedInn; i++) {
            grdLinescore[1, i].Text = mGame.lines[1, actualInn(i)].ToString();
         }

         ShowRHE();


      }


      private void cmdShiftLeft_Clicked(object sender, EventArgs e) {
         
         if (LinescoreStartInning > 9) {
            LinescoreStartInning -= 9;
            ShowLinescoreFull();
         }

      }


      private void cmdShiftRight_Clicked(object sender, EventArgs e) {
         
         if (mGame.inn > LinescoreStartInning + 8) {
            LinescoreStartInning += 9;
            ShowLinescoreFull();
         }

      }


      private void EnableControls() {
         
         switch (mGame.PlayState) {

            case PLAY_STATE.PLAY:
               cmdManageV.IsEnabled= true;
               cmdPlays.IsEnabled = cmdOptions.IsEnabled = true;
               btnGo.IsEnabled /*= cmdInfo.Enabled*/ = true;
               btnGo.Text="PLAY";
               btnBoxScore.IsEnabled = true;
               btnProfileDisks.IsEnabled = true;

            // This is needed here, as well as in 'Appearing', in case
            // user does not open FieldingProfile page when the glove 
            // appears.
            // -------------------------------------------------------
               this.IsFieldingPlay = false;
               btnProfileDisks.Source = "bat_img1a.png";

               break;

            case PLAY_STATE.NEXT:
               cmdManageV.IsEnabled = false;
               cmdPlays.IsEnabled = false; 
               cmdOptions.IsEnabled = true;
               btnGo.IsEnabled = true;
               btnGo.Text = "NEXT";
               btnBoxScore.IsEnabled = true;
               btnProfileDisks.IsEnabled = true;
               break;

            case PLAY_STATE.START:
               cmdManageV.IsEnabled = true;
               cmdPlays.IsEnabled = cmdOptions.IsEnabled = false;
               btnGo.IsEnabled = true;
               btnGo.Text = "START";
               btnBoxScore.IsEnabled = true;
               btnProfileDisks.IsEnabled = false;
               break;

            case PLAY_STATE.NONE:
               cmdManageV.IsEnabled = false;
               cmdPlays.IsEnabled = cmdOptions.IsEnabled = false;
               btnGo.IsEnabled = false;
               btnGo.Text = "";
               btnBoxScore.IsEnabled = false;
               btnProfileDisks.IsEnabled = false;
               break;

            case PLAY_STATE.OVER:
               cmdManageV.IsEnabled = false;
               cmdPlays.IsEnabled = cmdOptions.IsEnabled = false;
               btnGo.IsEnabled = false;
               btnGo.Text = "";
               btnBoxScore.IsEnabled = true;
               btnProfileDisks.IsEnabled = true;
               break;

         }

      }


      async void mnuPickTeams_OnClick(object sender, EventArgs e) {
      
         try {

            fPickTeamsPrep = new PickTeamsPrepPage();

            returningFrom = "PickTeamsPage";
            await Navigation.PushAsync(fPickTeamsPrep);
         }
         catch (Exception ex) {
            string msg = ex.Message +
               "\r\nNote: You must be connected to the Internet to start a new game";
            DisplayAlert("Error", msg, "Dismiss");
         }

      }

      async void mnuMngVis_OnClick(object sender, EventArgs e) {
      
         fLineup = new LineupCardPage(mGame, 0);
         returningFrom = "LineupCardPage";

         await Navigation.PushAsync(fLineup);
      }


      async private void btnBoxScore_Clicked(object sender, EventArgs e) {
      
         var fBoxScore = new BoxScorePage(mGame, 0);
         returningFrom = "BoxScorePage";
         await Navigation.PushAsync(fBoxScore);

      }

      async private void btnProfileDisks_Clicked(object sender, EventArgs e) {
         
         if (!this.IsFieldingPlay) {
            var fDisks = new ProfileDisk2Page(mGame);
            returningFrom = "ProfileDiskPage";
            await Navigation.PushAsync(fDisks);
         }
         else {
            var fDisks = new FieldingDiskPage(mGame);
            returningFrom = "FieldingDiskPage";
            await Navigation.PushAsync(fDisks);

         }

      }

      async private void mnuHelp_OnClick(object sender, EventArgs e) {
         
         var fAbout = new AboutPage();
         returningFrom = "AboutPage";
         await Navigation.PushAsync(fAbout);

      }

      async private void mnuPlays_OnClick(object sender, EventArgs e) {
      
         fPlays = new PlaysPage(mGame);
         returningFrom = "PlaysPage";
         await Navigation.PushAsync(fPlays);
      }

      async void mnuMngHome_OnClick(object sender, EventArgs e) {

      }
      async void mnuOptions_OnClick(object sender, EventArgs e) {
      
         fOptions = new OptionsPage(mGame, this.SpeechOn);
         returningFrom = "OptionsPage";
         await Navigation.PushAsync(fOptions);

      }



   }

}