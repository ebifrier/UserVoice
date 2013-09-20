using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace UserVoice
{
    /// <summary>
    /// アプリにまたがるオブジェクトを管理します。
    /// </summary>
    static class Global
    {
        private static Models.ModelObject model = new Models.ModelObject();
        private static Views.MainForm mainForm;

        /// <summary>
        /// メインの設定フォームを取得または設定します。
        /// </summary>
        public static Views.MainForm MainForm
        {
            get { return mainForm; }
            set { mainForm = value; }
        }

        /// <summary>
        /// モデルオブジェクトを取得します。
        /// </summary>
        public static Models.ModelObject ModelObject
        {
            get { return model; }
        }
    }
}
