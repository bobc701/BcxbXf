using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using BCX.BCXB;

namespace BcxbXf.Models {

   class BoxScoreListViewModel : INotifyPropertyChanged { 

      public ObservableCollection<CBatBoxSet> BatterBoxVis { get; set; }
      //public ObservableCollection<CBatBoxSet> BatterBoxHome { get; set; }
      public ObservableCollection<CPitBoxSet> PitcherBoxVis { get; set; }
      //public ObservableCollection<CPitBoxSet> PitcherBoxHome { get; set; }

      //public CBatBoxSet BatterBoxVis_tot { get; set; }
      private CGame mGame;
      public string VisName { get { return mGame.t[0].nick; } }
      public string HomeName { get { return mGame.t[1].nick; } }

      public BoxScoreListViewModel(CGame g, int side = 0) {
      // ------------------------------------------------------
         mGame = g;
         CBatter bat;
         CPitcher pit;
         int bx, px;

         BatterBoxVis = new ObservableCollection<CBatBoxSet>();
         var bsTot = new CBatBoxSet() { boxName = "Total"};
         for (int i = 1; i <= CGame.SZ_BAT-1; i++) {
            bx = g.t[side].xbox[i];
            if (bx == 0) break;
            bat = g.t[side].bat[bx];
            BatterBoxVis.Add(bat.bs);
            bsTot += bat.bs; //Operator overload! 
         }
         BatterBoxVis.Add(g.t[side].btot); //This is the totals row.


         //BatterBoxHome = new ObservableCollection<CBatBoxSet>();
         //for (int i = 1; i <= CGame.SZ_BAT; i++) {
         //   bx = g.t[1].xbox[i];
         //   if (bx == 0) break;
         //   bat = g.t[1].bat[bx];
         //   BatterBoxHome.Add(bat.bs);
         //}


         PitcherBoxVis = new ObservableCollection<CPitBoxSet>();
         for (int i = 1; i <= CGame.SZ_PIT-1; i++) {
            px = g.t[side].ybox[i];
            if (px == 0) break;
            pit = g.t[side].pit[px];
            pit.ps.boxName = pit.pname2;
            PitcherBoxVis.Add(pit.ps);
         }


         //PitcherBoxHome = new ObservableCollection<CPitBoxSet>();
         //for (int i = 1; i <= CGame.SZ_PIT; i++) {
         //   px = g.t[1].xbox[i];
         //   if (px == 0) break;
         //   pit = g.t[1].pit[px];
         //   PitcherBoxHome.Add(pit.ps);
         //}

         //var bs1 = new CBatBoxSet();
         //foreach (var bs in BatterBoxVis) bsTot = bsTot + bs; //Operator overload!
         //BatterBoxVis_tot = bsTot;

      }

      public event PropertyChangedEventHandler PropertyChanged;
      void OnPropertyChanged([CallerMemberName] string propertyName = "") {

         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

   }
}
