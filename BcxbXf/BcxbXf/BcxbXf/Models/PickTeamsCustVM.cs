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
            OnExecute_UserNameChanged();
         }
      }

      public string UserStatus { get; set; } = "";

      public Command CancelCmd { get; private set; }
      public Command GetTeamsCmd { get; private set; }
      public Command UseCmd { get; private set; }
   
      public Command UserNameChangedCmd { get; private set; }
      public Command SelectionChangedVisCmd { get; set; }
      public Command SelectionChangedHomeCmd { get; set; }

      public bool Activity1_IsRunning { get; set; }
      public bool Activity1_IsVisible { get; set; }
      public bool PickerVis_IsEnabled { get; set; }
      public bool PickerHome_IsEnabled { get; set; }



      CTeamRecord selectedTeam_Vis;
      public CTeamRecord SelectedTeam_Vis {
         get => selectedTeam_Vis;
         set {
            //selectedTeam_Vis = value;
            SetProperty(ref selectedTeam_Vis, value);
            OnExecute_SelectionChanged_Vis();
         }
      }

      CTeamRecord selectedTeam_Home;
      public CTeamRecord SelectedTeam_Home {
         get => selectedTeam_Home;
         set {
            //selectedTeam_Home = value;
            SetProperty(ref selectedTeam_Home, value);
            OnExecute_SelectionChanged_Home();
         }
      }


      // Look at this (unrelated) code. Interesting syntax.
      // First, note that it is a property. 
      // And the Command constructor takes an Action of string, as a lambda, i/o named Action!
      // public Command TextChangedCommand => new Command<string>(async (_mobilenumber) => await TextChanged(_mobilenumber));


      // Constructor ---------
      public PickTeamsCustVM(PickTeamsPrepPage pickPrep) {

         CancelCmd = new Command(OnExecute_Cancel);
         GetTeamsCmd = new Command(OnExecute_GetTeams, OnCanExecute_GetTeams);
         UseCmd = new Command(OnExecute_Use, OnCanExecute_Use);

         UserNameChangedCmd = new Command(OnExecute_UserNameChanged);
         SelectionChangedVisCmd = new Command(OnExecute_SelectionChanged_Vis);
         SelectionChangedHomeCmd = new Command(OnExecute_SelectionChanged_Home);

         BindingContext = this;

         fPickPrep = pickPrep;
         


         Debug.WriteLine($"--------- TeamCache.Count in PickTeamsPage constructor: {DataAccess.TeamCache.Count}");

         //pickerVis.ParentPage = this; //#3000.01
         //pickerHome.ParentPage = this;


      }

      //public event PropertyChangedEventHandler PropertyChanged;

      //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) {
      //   PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      //}

      void OnExecute_SelectionChanged_Vis() {
      // Handles SelectionChanged_Vis's event

         UseCmd.ChangeCanExecute();

      }


      void OnExecute_SelectionChanged_Home() {
      // Handles SelectionChanged_Home's event

         UseCmd.ChangeCanExecute();

      }


      async void OnExecute_Use() { 
      // Handles the UseCmd's Execute event...

         //fPickPrep.SelectedTeams[0] = (CTeamRecord)pickerVis.SelectedItem;
         //fPickPrep.SelectedTeams[1] = (CTeamRecord)pickerHome.SelectedItem;
         fPickPrep.SelectedTeams[0] = selectedTeam_Vis; 
         fPickPrep.SelectedTeams[1] = selectedTeam_Home;

         //await Navigation.PopToRootAsync();
         await Application.Current.MainPage.Navigation.PopToRootAsync();

      }

      bool OnCanExecute_Use() {
         // Handels the UseCmd's CanExecute event
         return selectedTeam_Vis.City is not null && selectedTeam_Home.City is not null;

      }
   

      async void OnExecute_Cancel() {

         fPickPrep.SelectedTeams[0] = new BcxbDataAccess.CTeamRecord();
         fPickPrep.SelectedTeams[1] = new BcxbDataAccess.CTeamRecord();
         //await Navigation.PopAsync();
         await Application.Current.MainPage.Navigation.PopAsync();

      }

      async void OnExecute_GetTeams() {

         //UserTeamList = new() {
         //   new CTeamRecord { City = "Sluggers", LineName = "Slg", NickName = "", Year = 0 },
         //   new CTeamRecord { City = "Rocket Man", LineName = "RMn", NickName = "", Year = 0 },
         //   new CTeamRecord { City = "Dodgers", LineName = "LAD", NickName = "", Year = 0 }
         //};

         StartActivity();
         UserTeamList = await DataAccess.GetCustTeamListForUser(userName);
         StopActivity();

         OnPropertyChanged("UserTeamList");

         int num = UserTeamList.Count;
         UserStatus = 
            $"{num switch {0 => "No", _ => num.ToString()}} available teams for {UserName}";
         OnPropertyChanged("UserStatus");

         bool ok = (UserTeamList.Count > 0);
         PickerVis_IsEnabled = ok; OnPropertyChanged(nameof(PickerVis_IsEnabled));
         PickerHome_IsEnabled = ok; OnPropertyChanged(nameof(PickerHome_IsEnabled));

      }


      bool OnCanExecute_GetTeams() => UserName != "";


      void OnExecute_UserNameChanged() => GetTeamsCmd.ChangeCanExecute();


      void OnSelectionChanged() {

         
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
                                 
         Activity1_IsVisible = true; OnPropertyChanged("Activity1_IsVisible");
         Activity1_IsRunning = true; OnPropertyChanged("Activity1_IsRunning");
      }


      public void StopActivity() { // #3000.01
                                  
         Activity1_IsRunning = false; OnPropertyChanged("Activity1_IsRunning");
         Activity1_IsVisible = false; OnPropertyChanged("Activity1_IsVisible");

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

      bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
         // I got this from...
         // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/data-binding/commanding

         if (Object.Equals(storage, value))
            return false;

         storage = value;
         OnPropertyChanged(propertyName);
         return true;

      }
   }

}
