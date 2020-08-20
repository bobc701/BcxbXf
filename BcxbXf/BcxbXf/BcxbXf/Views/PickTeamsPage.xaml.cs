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


      public string[] SelectedTeams { get; set; } = new string[2]; //<-- This is how selections are passed back to MainPage.

      //public Action Dismiss;


      public PickTeamsPage(List<string> teamList) {
      // ---------------------------------------------------------------
         InitializeComponent();
         BindingContext = new PickTeamsViewModel();

         pickerVis.ItemsSource = teamList;
         pickerHome.ItemsSource = teamList;
         //pickerVis.ItemDisplayBinding = new Binding("TeamTag");
         //pickerHome.ItemDisplayBinding = new Binding("TeamTag");
      }


      private void btnUse_Clicked(object sender, EventArgs e) {
      // -------------------------------------------------------
         //DisplayAlert("", "Use these teams", "OK");
         SelectedTeams[0] = (string)pickerVis.SelectedItem;
         SelectedTeams[1] = (string)pickerHome.SelectedItem;
         //Dismiss?.Invoke();
         Navigation.PopModalAsync();

      }


      private void btnCanc_Clicked(object sender, EventArgs e) {
      // -------------------------------------------------------
         //DisplayAlert("", "Cancel", "OK");
         SelectedTeams[0] = "";
         SelectedTeams[1] = "";
         Navigation.PopModalAsync();
      }

      private void picker_IndexChanged(object sender, EventArgs e) {
      // ------------------------------------------------------------------
         btnUse.IsEnabled = (pickerVis.SelectedIndex >= 0 && pickerHome.SelectedIndex >= 0);

      }


   }


}