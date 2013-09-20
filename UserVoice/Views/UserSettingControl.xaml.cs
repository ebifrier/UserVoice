using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UserVoice.Views
{
    using UserVoice.Voice;

    /// <summary>
    /// UserSettingControl.xaml の相互作用ロジック
    /// </summary>
    public partial class UserSettingControl : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserSettingControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// DataGridの選択内容が変更されたときに呼ばれます。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems)
                {
                    var voiceInfo = (UserVoiceInfo)item;

                    Global.ModelObject.CurrentUserVoiceInfoList.Add(voiceInfo);
                }
            }

            if (e.RemovedItems != null)
            {
                foreach (var item in e.RemovedItems)
                {
                    var voiceInfo = (UserVoiceInfo)item;

                    Global.ModelObject.CurrentUserVoiceInfoList.Remove(voiceInfo);
                }
            }
        }
    }
}
