using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using BCX.BCXCommon;
using BcxbXf.Models;

namespace BcxbXf {
   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class PickTeamsPage : ContentPage {


      public CTeamRecord[] SelectedTeams { get; set; } = new CTeamRecord[2]; //<-- This is how selections are passed back to MainPage.

      //public Action Dismiss;


      public PickTeamsPage() {
      // ---------------------------------------------------------------
         InitializeComponent();
         BindingContext = new PickTeamsViewModel(); 

         pickerVis.SelectedIndexChanged +=
            (object sender, EventArgs e) => {
               btnUse.IsEnabled =
                  pickerVis.NewPickedTeam.Year != 0 && pickerHome.NewPickedTeam.Year != 0;
            };

         pickerHome.SelectedIndexChanged +=
            (object sender, EventArgs e) => {
               btnUse.IsEnabled =
                  pickerVis.NewPickedTeam.Year != 0 && pickerHome.NewPickedTeam.Year != 0;
            };

         pickerVis.ParentPage = this; //#3000.01
         pickerHome.ParentPage = this;
         //pickerVis.ItemsSource = teamList;
         //pickerHome.ItemsSource = teamList;
         ////pickerVis.ItemDisplayBinding = new Binding("TeamTag");
         ////pickerHome.ItemDisplayBinding = new Binding("TeamTag");
      }


      private void btnUse_Clicked(object sender, EventArgs e) {
         // -------------------------------------------------------
         //DisplayAlert("", "Use these teams", "OK");
         SelectedTeams[0] = pickerVis.NewPickedTeam; //(CTeamRecord)pickerVis.SelectedItem; 
         SelectedTeams[1] = pickerHome.NewPickedTeam;//(CTeamRecord)pickerHome.SelectedItem;

         //Dismiss?.Invoke();
         Navigation.PopModalAsync();

      }


      private void btnCanc_Clicked(object sender, EventArgs e) {
      // -------------------------------------------------------
         //DisplayAlert("", "Cancel", "OK");
         SelectedTeams[0] = new CTeamRecord();
         SelectedTeams[1] = new CTeamRecord();
         Navigation.PopModalAsync();
      }

      private void picker_IndexChanged(object sender, EventArgs e) {
         // ------------------------------------------------------------------
         //btnUse.IsEnabled =
         //   pickerVis.NewPickedTeam.Year != 0 &&
         //   pickerHome.NewPickedTeam.Year != 0;
            //(pickerVis.SelectedItem != null) && ((CTeamRecord)(pickerVis.SelectedItem)).Year != 0 &&
            //(pickerHome.SelectedItem != null) && ((CTeamRecord)(pickerHome.SelectedItem)).Year != 0;
      }

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

      public async Task<List<CTeamRecord>> GetTeamList(int yr) {
      // ---------------------------------------------------------
         StartActivity();
         var teamList = await GFileAccess.GetTeamListForYearFromCache(yr);
         StopActivity();
         return teamList;


      }


   }


}