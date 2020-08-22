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
using BCX.BCXB;

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

            public string SelectedItem { get; internal set; }

            public PickerSource(DualPickerView pickerView)
            {
                _pickerView = pickerView;

                SelectedIndex = 0;
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


            public override string GetTitle(UIPickerView pickerView, nint row, nint component)
            {
                if (component == 0)
                {
                    if (row == 0) return "Choose year";
                    LeftComponent p = _pickerView.SelectedSource[(int)row];
                    return (string)p.Name;
                }
                else
                {
                    if (row == 0) return "Choose team";
                    LeftComponent p = _pickerView.SelectedSource[SelectedIndex];
                    CTeamRecord team = p.RightComponentList[(int)row];
                    return $"{team.City} {team.NickName}";
                }
            }


            public async override void Selected(UIPickerView pickerView, nint row, nint component)
            {
                if (component == 0)
                {
                    SelectedIndex = (int)pickerView.SelectedRowInComponent(0); // Isn't this same as 'row'???
                    if (SelectedIndex == 0) return;
                    LeftComponent q = _pickerView.SelectedSource[(int)row];
                    string yr = q.Name.ToString().Trim();
                    var teamList = await GFileAccess.GetTeamListForYearFromCache(int.Parse(yr));
                    teamList.Insert(0, new CTeamRecord());
                    q.RightComponentList = new ObservableCollection<CTeamRecord>(teamList);
                    pickerView.Select(row: 1, component: 1, true); // Reset team to row 0 (which is row 1)
                    pickerView.ReloadComponent(1);
                }

                // 获取选中的group
                LeftComponent p = _pickerView.SelectedSource[SelectedIndex];

                if (p.RightComponentList.Count <= 0)
                    return;

                // 获取选中的property
                int index = (int)pickerView.SelectedRowInComponent(1);
                //if (index == 0) return;
                //SelectedItem = p.Name + "-" + p.RightComponentList[index].Name;
                SelectedItem = p.RightComponentList[index].ToString();

            if (!string.IsNullOrEmpty(SelectedItem))
                    _pickerView.OnSelectedPropertyChanged(_pickerView, SelectedItem);
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
