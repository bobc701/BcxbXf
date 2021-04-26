using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

using System.ComponentModel;
using System.Runtime.CompilerServices;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using BcxbXf.Models;
using BcxbXf.Views;
using BcxbDataAccess;


namespace BcxbXf.Models {

   class PickTeamsCustVM : BindableObject {

      private PickTeamsPrepPage fPickPrep { get; set; }
      public List<CTeamRecord> UserTeamList { get; set; }

      private string userName = "";
      public string UserName {
         get => userName;
         set {
            userName = value;
            OnUserNameChanged();
         }
      }

      public string UserStatus { get; set; } = "";

      public Command CancelCmd { get; private set; }
      public Command GetTeamsCmd { get; private set; }
      public Command UserNameChangedCmd { get; private set; }

      public bool Activity1_IsRunning;
      public bool Activity1_IsVisible;

      public CTeamRecord pickerVis_SelectedItem;
      public CTeamRecord pickerHome_SelectedItem;

      // Look at this (unrelated) code. Interesting syntax.
      // First, note that it is a property. 
      // And the Command constructor takes an Action of string, as a lambda, i/o named Action!
      // public Command TextChangedCommand => new Command<string>(async (_mobilenumber) => await TextChanged(_mobilenumber));


      // Constructor ---------
      public PickTeamsCustVM(PickTeamsPrepPage pickPrep) {

         CancelCmd = new Command(OnCancel);
         GetTeamsCmd = new Command(OnGetTeams, OnCanExecuteGetTeams);
         UserNameChangedCmd = new Command(OnUserNameChanged);


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
         //fPickPrep.SelectedTeams[0] = (CTeamRecord)pickerVis.SelectedItem;
         //fPickPrep.SelectedTeams[1] = (CTeamRecord)pickerHome.SelectedItem;
         fPickPrep.SelectedTeams[0] = pickerVis_SelectedItem; 
         fPickPrep.SelectedTeams[1] = pickerHome_SelectedItem;
         OnPropertyChanged("pickerVis_SelectedItem");
         OnPropertyChanged("pickerHome_SelectedItem");

         //await Navigation.PopToRootAsync();
         await Application.Current.MainPage.Navigation.PopToRootAsync();

      }



      async void OnCancel() {

         fPickPrep.SelectedTeams[0] = new BcxbDataAccess.CTeamRecord();
         fPickPrep.SelectedTeams[1] = new BcxbDataAccess.CTeamRecord();
         //await Navigation.PopAsync();
         await Application.Current.MainPage.Navigation.PopAsync();

      }

      void OnGetTeams() {

         UserTeamList = new() {
            new CTeamRecord { City = "Sluggers", LineName = "Slg", NickName = "" },
            new CTeamRecord { City = "Rocket Man", LineName = "RMn", NickName = "" },
            new CTeamRecord { City = "Dodgers", LineName = "LAD", NickName = "" }
         };
         OnPropertyChanged("UserTeamList");

         this.UserStatus = $"{UserTeamList.Count} teams found for {UserName}";
         OnPropertyChanged("UserStatus");

      }

      void OnUserNameChanged() => GetTeamsCmd.ChangeCanExecute();

      bool OnCanExecuteGetTeams() => UserName != "";



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
                                 
         Activity1_IsVisible = true; OnPropertyChanged("Activity_IsRunning");
         Activity1_IsRunning = true; OnPropertyChanged("Activity_IsVisible");
      }


      public void StopActivity() { // #3000.01
                                  
         Activity1_IsRunning = false; OnPropertyChanged("Activity_IsRunning");
         Activity1_IsVisible = false; OnPropertyChanged("Activity_IsVisible");

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

         //UserTeamList = new() {
         //   new CTeamRecord { City = "Sluggers", LineName = "Slg", NickName = "" },
         //   new CTeamRecord { City = "Rocket Man", LineName = "RMn", NickName = "" },
         //   new CTeamRecord { City = "Dodgers", LineName = "LAD", NickName = "" }
         //};
         //OnPropertyChanged("UserTeamList");

         //this.UserStatus = $"{UserTeamList.Count} teams found for {UserName}";
         //OnPropertyChanged("UserStatus");
      }

      //private void txtUserName_Changed(object sender, TextChangedEventArgs e) d{

      //   //OnUserNameChanged();

   }

}
