using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using BcxbDataAccess;


namespace BcxbXf.Views {

   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class PickTeamsPrepPage : ContentPage {

      //public PickTeamsRealPage fPickReal { get; set; }
      //public PickTeamsCustPage fPickCust { get; set; }

      public CTeamRecord[] SelectedTeams = new CTeamRecord[2];
      public int TypeOfTeam { get; set; }


      public PickTeamsPrepPage() {
         InitializeComponent();

      }

      private async void btnCustomTeams_Clicked(object sender, EventArgs e) {

         this.TypeOfTeam = 2;
         await Navigation.PushAsync(new PickTeamsCustPage(this));
      }

      private async void btnRealTeams_Clicked(object sender, EventArgs e) {

         this.TypeOfTeam = 1;
         await Navigation.PushAsync(new PickTeamsRealPage(this));

      }
   }
}