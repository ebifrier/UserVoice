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

using ankoPlugin2;
using LibAnko;

namespace UserVoice
{
    using UserVoice.Voice;

    [CLSCompliant(false)]
    public sealed class ankoUserVoice : DispatcherObject, IPlugin
    {
        private readonly object syncObject = new object();
        private readonly Thread readOutThread;
        private readonly Queue<chat> readOutTaskQueue = new Queue<chat>();

        private readonly UserVoiceCore core = new UserVoiceCore();
        private IPluginHost vHost;
        private DateTime startTime;
        private DateTime lastCommentTime = DateTime.Now;
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ankoUserVoice()
        {
            this.readOutThread = new Thread(ReadOutThreadMain)
            {
                IsBackground = true,
                Name = "ReadOut",
                Priority = ThreadPriority.BelowNormal,
            };

            this.readOutThread.Start();
        }

        /// <summary>
        /// プラグインに与えられるホストを取得します。
        /// </summary>
        public IPluginHost host
        {
            get { return this.vHost; }
            set
            {
                this.vHost = value;

                if (value != null)
                {
                    Initialize(value);
                }
            }
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
        /// 生きているかどうかを取得します。
        /// </summary>
        public bool IsAlive
        {
            get { return true; }
        }

        /// <summary>
        /// ホスト設定時に呼ばれる初期化メソッドです。
        /// </summary>
        private void Initialize(IPluginHost host)
        {
            try
            {
                var logpath = Path.Combine(
                    host.ApplicationDataFolder,
                    "UserVoiceLog.txt");
                Trace.Listeners.Add(new TextWriterTraceListener(logpath));

                this.startTime = DateTime.Now;

                host.ConnectedServer += host_BroadcastConnected;
                host.DisconnectedServer += host_BroadcastDisConnected;
                host.ReceiveChat += host_ReceivedChat;
                host.PluginDispose += PluginDisposed;

                //MessageBox.Show(this.host.DirectoryPathSettingFile);
                //MessageBox.Show(this.host.DirectoryPathAppSetting);

                var path = Path.Combine(host.ApplicationDataFolder, "UserVoice.xml");
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
        /// 読み上げ処理が遅れるとコメントを２回読むなどの不具合がでるため、
        /// 専用スレッドを用意して処理速度を上げます。
        /// </summary>
        private void ReadOutThreadMain()
        {
            while (true)
            {
                try
                {
                    chat task = null;
                    lock (syncObject)
                    {
                        // 処理するタスクを一つ取得します。
                        if (!this.readOutTaskQueue.Any())
                        {
                            // ウェイト時間は最大でも300msとします。
                            Monitor.Wait(this.syncObject, 300);
                            continue;
                        }

                        task = this.readOutTaskQueue.Dequeue();
                    }

                    if (task != null)
                    {
                        HandleChat(task);
                    }
                }
                catch (ThreadAbortException)
                {
                    // スレッド終了時に呼ばれます。
                    break;
                }
                catch (Exception ex)
                {
                    // 例外は無視します。
                    Util.TraceLog(
                        "{0}: ThreadMainで例外が発生しました。",
                        ex.Message);
                }
            }
        }

        /// <summary>
        /// コメントを処理します。
        /// </summary>
        private void HandleChat(chat chat)
        {
            Util.TraceLog("コメント: {0}({1})",
                chat.Message, chat.No);

            try
            {
                // ＮＧコメントの場合、時刻がないことがあります。
                if (string.IsNullOrEmpty(chat.Message))
                {
                    return;
                }

                // コメント投稿時刻がプラグインの開始時刻よりも前の場合は、
                // そのコメントは解析しません。
                if (chat.Date < this.startTime)
                {
                    return;
                }

                // オーナーコメントなら
                /*if (comment.Premium >= 2 && comment.)
                {
                }*/

                if (this.core.ParseComment(chat.Message, chat.UserId))
                {
                    this.core.ReadOut("声を登録したよ～", chat.UserId);
                }
                else if (Global.ModelObject.IsReadComment(
                    chat.Message, chat.Mail, chat.IsBSP, chat.Premium))
                {
                    this.core.ReadOut(chat.Message, chat.UserId);
                }
                else if (!chat.Message.StartsWith("/keepalive"))
                {
                    this.core.ReadInfo(chat.Message, chat.Premium);
                }
            }
            catch (Exception e)
            {
                MessageUtil.ErrorMessage(chat.Date + e.Message + e.StackTrace);
            }

            // コメント受信時刻を更新します。
            this.lastCommentTime = DateTime.Now;
        }

        /// <summary>
        /// コメント受信時に呼ばれます。
        /// </summary>
        void host_ReceivedChat(object sender, ReceiveChatEventArgs e)
        {
            Util.TraceLog("コメントを受信しました。");

            if (e.Chat == null)
            {
                return;
            }

            lock(this.syncObject)
            {
                // 別スレッドで処理するためのタスクを追加します。
                this.readOutTaskQueue.Enqueue(e.Chat);

                Monitor.PulseAll(this.syncObject);
            }
        }

        void PluginDisposed(object sender, EventArgs e)
        {
            if (Global.MainForm != null)
            {
                Global.MainForm.Close();
            }

            AppSettings.Save();
        }
    }
}
