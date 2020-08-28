using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using BcxbXf.Extend;
using BCX.BCXCommon;

namespace BcxbXf.Models {

   class PickTeamsViewModel : INotifyPropertyChanged {

      public event PropertyChangedEventHandler PropertyChanged;
      private ObservableCollection<LeftComponent> twoSource;

      public PickTeamsViewModel() {

         twoSource = new ObservableCollection<LeftComponent>();
         InitializationData();
      }


      void InitializationData() {

         //TwoSource.Add(new GroupModel { GroupName = "enterprise", Property = enterprise });
         //TwoSource.Add(new GroupModel { GroupName = "countries", Property = countries });

         TwoSource.Add(new LeftComponent { Name = "Year", RightComponentList = new ObservableCollection<CTeamRecord>() });
         for (int y = 2019; y >= 1901; y--) {
            TwoSource.Add(new LeftComponent { 
               Name = y.ToString(), 
               RightComponentList = new ObservableCollection<CTeamRecord>() 
            });
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
