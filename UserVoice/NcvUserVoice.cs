using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

using NicoLibrary.NicoLiveData;
using Plugin;

namespace UserVoice
{
    using UserVoice.Voice;

    public class NcvUserVoice : DispatcherObject, IPlugin
    {
        private UserVoiceCore core = new UserVoiceCore();
        private IPluginHost host;
        private DateTime startTime;
        private DateTime lastCommentTime = DateTime.Now;

        /// <summary>
        /// プラグインに与えられるホストを取得します。
        /// </summary>
        public IPluginHost Host
        {
            get { return this.host; }
            set { this.host = value; }
        }

        /// <summary>
        /// プラグイン名を取得します。
        /// </summary>
        public string Name
        {
            get { return "ユーザーボイス"; }
        }

        /// <summary>
        /// プラグインの説明を取得します。
        /// </summary>
        public string Description
        {
            get { return "ユーザーごとに声設定を行うためのプラグインです。"; }
        }

        /// <summary>
        /// プラグインのバージョンを取得します。
        /// </summary>
        public string Version
        {
            get { return "1.2.0"; }
        }

        /// <summary>
        /// 最初に自動起動するかどうかを取得します。
        /// </summary>
        public bool IsAutoRun
        {
            get { return true; }
        }

        /// <summary>
        /// 自動起動時に呼ばれます。
        /// </summary>
        public void AutoRun()
        {
            try
            {
                var logpath = Path.Combine(
                    this.host.DirectoryPathSettingFile,
                    "UserVoiceLog.txt");
                Trace.Listeners.Add(new TextWriterTraceListener(logpath));

                this.startTime = DateTime.Now;

                this.host.BroadcastConnected += host_BroadcastConnected;
                this.host.BroadcastDisConnected += host_BroadcastDisConnected;
                this.host.ReceivedComment += host_ReceivedComment;
                this.host.MainForm.FormClosed += MainForm_FormClosed;

                //MessageBox.Show(this.host.DirectoryPathSettingFile);
                //MessageBox.Show(this.host.DirectoryPathAppSetting);

                var path = Path.Combine(this.host.DirectoryPathSettingFile, "UserVoice.xml");
                AppSettings.DataFilePath = path;

                AppSettings.Load();
            }
            catch (Exception)
            {
                //MessageBox.Show(e.Message + e.StackTrace);
            }
        }

        /// <summary>
        /// メニューからの選択時に呼ばれます。
        /// </summary>
        public void Run()
        {
            var mainForm = Global.MainForm;

            if (mainForm == null)
            {
                mainForm = new Views.MainForm();
            }

            mainForm.Show();
            mainForm.Activate();
        }

        void host_BroadcastConnected(object sender, EventArgs e)
        {
            this.startTime = DateTime.Now;

            // 再接続の可能性があるので、
            // 一度棒読みちゃんのデータを全部消去します。
            BouyomiChan.ClearTalkTask();
        }

        void host_BroadcastDisConnected(object sender, EventArgs e)
        {
            BouyomiChan.ClearTalkTask();
        }

        /// <summary>
        /// コメントを処理します。
        /// </summary>
        private void HandleComment(List<LiveCommentData> commentDataList)
        {
            Util.TraceLog("HandleComment開始");

            foreach (var comment in commentDataList)
            {
                Util.TraceLog("コメント: {0}({1})",
                    comment.Comment, comment.IndexOfUserSetting);

                try
                {
                    // ＮＧコメントの場合、時刻がないことがあります。
                    if (string.IsNullOrEmpty(comment.Date))
                    {
                        continue;
                    }

                    // コメント投稿時刻がプラグインの開始時刻よりも前の場合は、
                    // そのコメントは解析しません。
                    var date = Util.UnixTimeToDateTime(comment.Date);
                    if (date < this.startTime)
                    {
                        continue;
                    }

                    // オーナーコメントなら
                    /*if (comment.Premium >= 2 && comment.)
                    {
                    }*/

                    if (this.core.ParseComment(comment.Comment, comment.UserId))
                    {
                        this.core.ReadOut("声を登録したよ～", comment.UserId);
                    }
                    else if (Global.ModelObject.IsReadComment(
                        comment.Comment, comment.Mail, comment.IsBSP, comment.Premium))
                    {
                        this.core.ReadOut(comment.Comment, comment.UserId);
                    }
                    else if (!comment.Comment.StartsWith("/keepalive"))
                    {
                        this.core.ReadInfo(comment.Comment, comment.Premium);
                    }
                }
                catch (Exception e)
                {
                    MessageUtil.ErrorMessage(comment.Date + e.Message + e.StackTrace);
                }

                // コメント受信時刻を更新します。
                this.lastCommentTime = DateTime.Now;
            }
        }

        void host_ReceivedComment(object sender, ReceivedCommentEventArgs e)
        {
            Util.TraceLog("コメントを受信しました。");

            // コメントリストはNCV内部で再利用されることがあるようで、
            // 一度コピーしないと中身が変わることがあります。
            var commentList = new List<LiveCommentData>(e.CommentDataList);
            
            if (this.Dispatcher.CheckAccess())
            {
                HandleComment(commentList);
            }
            else
            {
                Dispatcher.BeginInvoke(
                    (Action<List<LiveCommentData>>)HandleComment,
                    commentList);
            }
        }

        void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Global.MainForm != null)
            {
                Global.MainForm.Close();
            }

            AppSettings.Save();
        }
    }
}
