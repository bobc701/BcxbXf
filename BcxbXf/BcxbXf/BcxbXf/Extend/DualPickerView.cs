using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace BcxbXf.Extend
{
    public class DualPickerView : Picker
    {
        public static readonly BindableProperty PlaceholderColorProperty =
            BindableProperty.Create(nameof(PlaceholderColor),
                                    typeof(string),
                                    typeof(DualPickerView), defaultValue: "#CCCCCC",
                                    defaultBindingMode: BindingMode.TwoWay);

        public static readonly BindableProperty SelectedSourceProperty =
            BindableProperty.Create(nameof(SelectedSource),
                                    typeof(ObservableCollection<LeftComponent>),
                                    typeof(DualPickerView), defaultValue: null,
                                    defaultBindingMode: BindingMode.TwoWay);


        public DualPickerView()
        {
            Items.Add("");
            SelectedIndex = 0;
        }

        public string PlaceholderColor
        {
            get { return (string)GetValue(PlaceholderColorProperty); }
            set { SetValue(PlaceholderColorProperty, value); }
        }

        public ObservableCollection<LeftComponent> SelectedSource
        {
            get { return (ObservableCollection<LeftComponent>)GetValue(SelectedSourceProperty); }
            set { SetValue(SelectedSourceProperty, value); }
        }


        public void OnSelectedPropertyChanged(BindableObject bindable, object newValue)
        {
            var picker = (DualPickerView)bindable;
            // Update value
            picker.Items[0] = newValue.ToString();

            picker.SelectedIndex = 0;
        }
    }
}
