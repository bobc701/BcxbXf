using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

using BcxbXf.Extend;
using BCX.BCXCommon;
using BcxbDataAccess;

using System.Linq;

namespace BcxbXf.Models {

   class PickTeamsViewModel : INotifyPropertyChanged {

      public event PropertyChangedEventHandler PropertyChanged;
      private ObservableCollection<LeftComponent> twoSource;

      public PickTeamsViewModel() {

         twoSource = new ObservableCollection<LeftComponent>();
         InitializationData();
      }


      async void InitializationData() {

         //TwoSource.Add(new GroupModel { GroupName = "enterprise", Property = enterprise });
         //TwoSource.Add(new GroupModel { GroupName = "countries", Property = countries });

         TwoSource.Add(new LeftComponent { Name = "Year", RightComponentList = new ObservableCollection<CTeamRecord>() });
         for (int y = 2020; y >= 1901; y--) {
            TwoSource.Add(new LeftComponent { 
               Name = y.ToString(), 
               RightComponentList = new ObservableCollection<CTeamRecord>() 
            });
         }
         try {
            //throw new Exception("Don't forget to remove this exception"); //For testing
            Debug.WriteLine("Will call GetTeamListForYearFromCache in InitializationData"); //3000.04
            var list0 = await DataAccess.GetTeamListForYearFromCache(2020);
            TwoSource[0].RightComponentList = new ObservableCollection<CTeamRecord>(list0);
         }
         catch (Exception ex) {
            TwoSource[0].RightComponentList = new ObservableCollection<CTeamRecord>();
         }

      }


      public ObservableCollection<LeftComponent> TwoSource {
         get { return twoSource; }
         set { SetProperty(ref twoSource, value); }
      }


      bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null) {
         if (Object.Equals(storage, value))
            return false;

         storage = value;
         OnPropertyChanged(propertyName);
         return true;
      }

      protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
         PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
   }


}
