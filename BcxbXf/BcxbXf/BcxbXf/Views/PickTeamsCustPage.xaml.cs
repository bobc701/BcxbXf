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
   public partial class PickTeamsCustPage : ContentPage {

      // Constructor ---------
      public PickTeamsCustPage(PickTeamsPrepPage pickPrep) {

         InitializeComponent();
         BindingContext = new PickTeamsCustVM(pickPrep);
      }

   }

}


