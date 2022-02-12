﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using BcxbXf.Models;
using BCX.BCXB;
using System.Diagnostics;

namespace BcxbXf.Views {

   [XamlCompilation(XamlCompilationOptions.Compile)]
   public partial class BoxScorePage : ContentPage {

      CGame mGame;
      BoxScoreListViewModel _boxModel;

      public BoxScorePage(CGame g, int side = 0) {
         // ---------------------------------------------------
         InitializeComponent();
         mGame = g;
         _boxModel = new BoxScoreListViewModel(g, side);
         BindingContext = _boxModel;
         Debug.WriteLine($"------------ BatteBoxVis[1].h: {_boxModel.BatterBoxVis[1].h} -------------------------");
         this.Title = "Box Score";

      // This here is for testing...
         //lstBox.ItemTapped += (object sender, ItemTappedEventArgs e) => {
         //   DisplayAlert("Item Tapped", ((CBatBoxSet)e.Item).BoxName, "OK");
         //};
         //lstBox.ItemSelected += (object sender, SelectedItemChangedEventArgs e) => {
         //   DisplayAlert("Item Selected", ((CBatBoxSet)e.SelectedItem).BoxName, "OK");
         //};
      }

      private void btnHomeBox_Clicked(object sender, EventArgs e) {
      // ----------------------------------------------------------
         BindingContext = new BoxScoreListViewModel(mGame, 1);
         btnHomeBox.BackgroundColor = Color.White;
         btnVisBox.BackgroundColor = Color.Gray;
      }

      private void btnVisBox_Clicked(object sender, EventArgs e) {
      // ---------------------------------------------------------
         BindingContext = new BoxScoreListViewModel(mGame, 0);
         btnVisBox.BackgroundColor = Color.White;
         btnHomeBox.BackgroundColor = Color.Gray;

      }

      //private void lstBox_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
      //   var item = (CBatBoxSet)e.SelectedItem;
      //   var x = (ListView)sender;
      //}
   }
}