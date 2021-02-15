using System;
using System.Collections.Generic;
using Foundation;
using BcxbXf.Extend;
using BcxbXf.iOS.extend;
using BCX.BCXCommon;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using System.Collections.ObjectModel;
using System.Diagnostics;
using BCX.BCXB;
using BcxbDataAccess;

[assembly: ExportRenderer(typeof(DualPickerView), typeof(DualPickerViewRenderer))]
namespace BcxbXf.iOS.extend
{
    public class DualPickerViewRenderer : PickerRenderer
    {
        public static int DisplayWidth = (int)UIScreen.MainScreen.Bounds.Width;

        private DualPickerView _pickerView;


        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderStyle = UITextBorderStyle.RoundedRect;

                _pickerView = e.NewElement as DualPickerView;

                //if(e.OldElement != null || e.NewElement != null)
                //SetPlaceholderColor(_pickerView);

                var _picker = new UIPickerView
                {
                    Model = new PickerSource(_pickerView)
                };

                SelectPickerValue(_picker, _pickerView);

                Control.InputView = _picker;
            }
        }


        private void SelectPickerValue(UIPickerView customModelPickerView, DualPickerView pickerView)
        {
            if (pickerView == null)
                return;
        }


        protected override void OnElementPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (Control == null)
                return;

            if (e.PropertyName == Picker.SelectedIndexProperty.PropertyName)
            {
                var picker = (UIPickerView)Control.InputView; 

                SelectPickerValue(picker, _pickerView);
            }
        }

        void SetPlaceholderColor(DualPickerView picker)
        {
            string placeholderColor = picker.PlaceholderColor;
            UIColor color = UIColor.FromRGB(GetRed(placeholderColor), GetGreen(placeholderColor), GetBlue(placeholderColor));

            var placeholderAttributes = new NSAttributedString(picker.Title, new UIStringAttributes()
            { ForegroundColor = color });

            if (Control != null)
                Control.AttributedPlaceholder = placeholderAttributes;
        }


        float GetRed(string color)
        {
            Color c = Color.FromHex(color);
            return (float)c.R;
        }

        float GetGreen(string color)
        {
            Color c = Color.FromHex(color);
            return (float)c.G;
        }

        float GetBlue(string color)
        {
            Color c = Color.FromHex(color);
            return (float)c.B;
        }

    //  ---------------------------------------------
        public class PickerSource : UIPickerViewModel
    //  ---------------------------------------------
        {
            private DualPickerView _pickerView { get; }

            public int SelectedIndex { get; internal set; } 

            public CTeamRecord SelectedItem { get; internal set; }

            public PickerSource(DualPickerView pickerView)
            {
                _pickerView = pickerView;

            //SelectedIndex = 0;
                pickerView.SelectedItem = 0;
         }

            public override nint GetComponentCount(UIPickerView pickerView)
            {
                return 2;
            }

            public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
            {
                if (component == 0)
                {
                    return _pickerView.SelectedSource.Count;
                }
                else
                {
                    LeftComponent p = _pickerView.SelectedSource[SelectedIndex]; //<-- Is this where we would get teams?
                    return p.RightComponentList.Count;
                }
            }


            public override string GetTitle(UIPickerView pickerView, nint row, nint component) {

            //  Made this into a switch //#3005.05
                LeftComponent p;
                switch (component) { 
                    case 0:
                       if (row == 0) return "Year"; //#3000.05
                       p = _pickerView.SelectedSource[(int)row];
                       return (string)p.Name;
                    case 1:
                       if (row == 0) return "Team"; //#3000.05
                       p = _pickerView.SelectedSource[SelectedIndex];
                       CTeamRecord team = p.RightComponentList[(int)row];
                       return $"{team.City} {team.NickName}";
                    default:
                       return "Invalid component";
                }
            }


            public async override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                try { 
                   if (component == 0)
                   {   
                       SelectedIndex = (int)pickerView.SelectedRowInComponent(0); // Isn't this same as 'row'???
                       //if (SelectedIndex == 0) return; Out #3000.05
                       LeftComponent q = _pickerView.SelectedSource[(int)row];
                       string yr = q.Name.ToString().Trim();
                       if (yr == "Year") yr = "0"; //#3000.05

                    // #3000.01... (See E/N note for how to improve this.)
                       _pickerView.ParentPage.StartActivity();
                       Debug.WriteLine("Will call GetTeamListForYearFromCache in PickerSource.Selected"); //3000.04
                       var teamList = await DataAccess.GetTeamListForYearFromCache(int.Parse(yr));
                       _pickerView.ParentPage.StopActivity();

                       //teamList.Insert(0, new CTeamRecord());
                       q.RightComponentList = new ObservableCollection<CTeamRecord>(teamList);
                       pickerView.ReloadComponent(1);
                       pickerView.Select(row: 0, component: 1, true); // Reset team to row 0 (which is row 1)
                   }

                   // 获取选中的group
                   LeftComponent p = _pickerView.SelectedSource[SelectedIndex];

                   if (p.RightComponentList.Count <= 0) return;

                   // 获取选中的property
                   int index = (int)pickerView.SelectedRowInComponent(1);
                   //if (index == 0) return; Out #3000.05
                   //SelectedItem = p.Name + "-" + p.RightComponentList[index].Name;
                   SelectedItem = p.RightComponentList[index];
                   _pickerView.SelectedItem = SelectedItem;

                   if (SelectedItem.Year != 0)
                          _pickerView.OnSelectedPropertyChanged(_pickerView, SelectedItem);
                }
                           
                catch (Exception ex) {
               //CAlert.ShowOkAlert("Error selecting year", ex.Message, "OK", ctlr);
                   _pickerView.ParentPage.StopActivity();
                   await _pickerView.ParentPage.DisplayAlert("Error loading data", ex.Message, "OK");
                }

            }


   public override nfloat GetComponentWidth(UIPickerView pickerView, nint component)
            {
                var screenWidth = DisplayWidth;

                if (component == 0)
                {
                    return screenWidth * 0.3f;
                }
                else
                {
                    return screenWidth * 0.6f;
                }
            }
        }
    }
}
