using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using BcxbXf.Models;
using BcxbXf.Views;
using BcxbDataAccess;


namespace BcxbXf.Views {

   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class PickTeamsCustPage : ContentPage, INotifyPropertyChanged {

      private PickTeamsPrepPage fPickPrep { get; set; }
      public List<CTeamRecord> UserTeamList { get; set; }
      public string UserName { get; set; } = "";
      public string UserStatus { get; set; } = "";

   // ------------ Constructor -----------------------------
      public PickTeamsCustPage(PickTeamsPrepPage pickPrep) {

         InitializeComponent();

         CancelCmd = new Command(OnCancel);

         BindingContext = this;

         fPickPrep = pickPrep;


         pickerVis.SelectedIndexChanged +=
            (object sender, EventArgs e) => {
               btnUse.IsEnabled =
               pickerVis.SelectedItem is not null && pickerHome.SelectedItem is not null;
            };

         pickerHome.SelectedIndexChanged +=
            (object sender, EventArgs e) => {
               btnUse.IsEnabled =
               pickerVis.SelectedItem is not null && pickerHome.SelectedItem is not null;

            };

         Debug.WriteLine($"--------- TeamCache.Count in PickTeamsPage constructor: {DataAccess.TeamCache.Count}");

         //pickerVis.ParentPage = this; //#3000.01
         //pickerHome.ParentPage = this;

      }
         

      //public event PropertyChangedEventHandler PropertyChanged;

      //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
      //   PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      //}

      
      private async void btnUse_Clicked(object sender, EventArgs e) {
         // -------------------------------------------------------
         fPickPrep.SelectedTeams[0] = (CTeamRecord)pickerVis.SelectedItem;
         fPickPrep.SelectedTeams[1] = (CTeamRecord)pickerHome.SelectedItem;

         await Navigation.PopToRootAsync();

      }


      public Command CancelCmd { get; private set; }

      async void OnCancel() {

         fPickPrep.SelectedTeams[0] = (CTeamRecord)pickerVis.SelectedItem;
         fPickPrep.SelectedTeams[1] = (CTeamRecord)pickerHome.SelectedItem;

         await Navigation.PopToRootAsync();

      }

      //private void btnCanc_Clicked(object sender, EventArgs e) {
      //   // -------------------------------------------------------
      //   //DisplayAlert("", "Cancel", "OK");
      //   fPickPrep.SelectedTeams[0] = new BcxbDataAccess.CTeamRecord();
      //   fPickPrep.SelectedTeams[1] = new BcxbDataAccess.CTeamRecord();
      //   Navigation.PopAsync();
      //}

      //private void picker_IndexChanged(object sender, EventArgs e) {
      //   // ------------------------------------------------------------------
      //   //btnUse.IsEnabled =
      //   //   pickerVis.NewPickedTeam.Year != 0 &&
      //   //   pickerHome.NewPickedTeam.Year != 0;
      //   //(pickerVis.SelectedItem != null) && ((CTeamRecord)(pickerVis.SelectedItem)).Year != 0 &&
      //   //(pickerHome.SelectedItem != null) && ((CTeamRecord)(pickerHome.SelectedItem)).Year != 0;
      //}

      public void StartActivity() { // #3000.01
                                    // --------------------------------
         Activity1.IsVisible = true;
         Activity1.IsRunning = true; ;
      }


      public void StopActivity() { // #3000.01
                                    // -------------------------------
         Activity1.IsRunning = false;
         Activity1.IsVisible = false;

      }

      // #3000.01
      // This is not yet wired in, but reccommend doing so.
      // Call this from Selected in the custom renderer.
      // So that renderer is not coupled with GFileAccess.

      public async Task<List<BcxbDataAccess.CTeamRecord>> GetTeamList(int yr) {
         // ---------------------------------------------------------
         StartActivity();
         var teamList = await DataAccess.GetTeamListForYearFromCache(yr);
         StopActivity();
         return teamList;

      }

      private void btnGetTeams_Clicked(object sender, EventArgs e) {

         UserTeamList = new() {
            new CTeamRecord { City = "Sluggers", LineName = "Slg", NickName = "" },
            new CTeamRecord { City = "Rocket Man", LineName = "RMn", NickName = "" },
            new CTeamRecord { City = "Dodgers", LineName = "LAD", NickName = "" }
         };
         OnPropertyChanged("UserTeamList");

         this.UserStatus = $"{UserTeamList.Count} teams found for {UserName}";
         OnPropertyChanged("UserStatus");
     }
   }

}


