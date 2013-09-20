using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace UserVoice.Views
{
    /// <summary>
    /// NumericUpDown.xaml の相互作用ロジック
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(int), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    ValueChanged, CoerceValue));

        public static readonly DependencyProperty ValueStringProperty =
            DependencyProperty.Register(
                "ValueString", typeof(string), typeof(NumericUpDown),
                new FrameworkPropertyMetadata("0",
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    ValueStringChanged),
                ValidateValueString);

        public static readonly DependencyProperty MinimumProperty =
            DependencyProperty.Register(
                "Minimum", typeof(int), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    MinimumChanged, CoerceMinimum));

        public static readonly DependencyProperty MaximumProperty =
            DependencyProperty.Register(
                "Maximum", typeof(int), typeof(NumericUpDown),
                new FrameworkPropertyMetadata(100,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender,
                    MaximumChanged, CoerceMaximum));

        /// <summary>
        /// 値を取得または設定します。
        /// </summary>
        [Bindable(true)]
        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 設定できる最小値を取得または設定します。
        /// </summary>
        [Bindable(true)]
        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        /// <summary>
        /// 設定できる最大値を取得または設定します。
        /// </summary>
        [Bindable(true)]
        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public string ValueString
        {
            get { return (string)GetValue(ValueStringProperty); }
            set { SetValue(ValueStringProperty, value); }
        }

        private static void ValueChanged(DependencyObject d,
                                         DependencyPropertyChangedEventArgs e)
        {
            var self = (NumericUpDown)d;

            self.OnValueChanged((int)e.OldValue, (int)e.NewValue);
            self.ValueString = self.Value.ToString();
        }

        private void OnValueChanged(int oldValue, int newValue)
        {
            //base.RaiseP
        }

        /// <summary>
        /// 値を[MinValue, MaxValue]の間に納めます。
        /// </summary>
        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var self = (NumericUpDown)d;
            var value = (int)baseValue;

            return self.Median(value);
        }

        /// <summary>
        /// 最小値が変わったときに呼ばれます。
        /// </summary>
        private static void MinimumChanged(DependencyObject d,
                                            DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(MaximumProperty);
            d.CoerceValue(ValueProperty);
        }

        /// <summary>
        /// 必要なら設定された最小値を修正します。
        /// </summary>
        private static object CoerceMinimum(DependencyObject d, object baseValue)
        {
            var self = (NumericUpDown)d;
            var value = (int)baseValue;

            // 当然、最大値よりも小さくないとダメです。
            return Math.Min(value, self.Maximum);
        }

        /// <summary>
        /// 最大値が変わったときに呼ばれます。
        /// </summary>
        private static void MaximumChanged(DependencyObject d,
                                           DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(MinimumProperty);
            d.CoerceValue(ValueProperty);
        }

        /// <summary>
        /// 必要なら設定された最大値を修正します。
        /// </summary>
        private static object CoerceMaximum(DependencyObject d, object baseValue)
        {
            var self = (NumericUpDown)d;
            var value = (int)baseValue;

            // 当然、最小値よりも小さくないとダメです。
            return Math.Max(value, self.Minimum);
        }

        /// <summary>
        /// 値文字列が有効か確認をします。
        /// </summary>
        private static bool ValidateValueString(object value)
        {
            var text = (string)value;

            int result;
            return int.TryParse(text, out result);
        }

        /// <summary>
        /// 値文字列が変更されたときに呼ばれます。
        /// </summary>
        private static void ValueStringChanged(DependencyObject d,
                                               DependencyPropertyChangedEventArgs e)
        {
            var self = (NumericUpDown)d;
            var text = (string)e.NewValue;

            self.Value = int.Parse(text);
        }

        /// <summary>
        /// 値文字列の修正を行います。(054 -> 54 など)
        /// </summary>
        private static object CoerceValueString(DependencyObject d, object baseValue)
        {
            var text = (string)baseValue;

            var value = int.Parse(text);
            return value.ToString();
        }

        /// <summary>
        /// 最小値と最大値を加味した値を取得します。
        /// </summary>
        private int Median(int value)
        {
            return Math.Min(Maximum, Math.Max(value, Minimum));
        }

        public NumericUpDown()
        {
            InitializeComponent();

            this.layoutBase.DataContext = this;
        }

        private void scrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var scrollBar = (ScrollBar)sender;

            // スクロールバーの初期値が0のままだと、上スクロールが
            // 出来なくなるため通常設定値をは0.5とし、
            // ここからの変移でどちらにスクロールしたか調べます。
            // また、スクロール後は設定値を0.5に再設定しています。
            if (e.NewValue == 0.5)
            {
                return;
            }

            if (e.NewValue > e.OldValue)
            {
                Value -= 1;
            }
            else
            {
                Value += 1;
            }

            scrollBar.Value = 0.5;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var text = textBox.Text;

            int result;
            if (!int.TryParse(text, out result))
            {
                textBox.Text = Value.ToString();
                return;
            }

            Value = result;
        }
    }
}
