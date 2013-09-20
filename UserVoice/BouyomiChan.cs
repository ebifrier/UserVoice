using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Web;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace UserVoice
{
    using UserVoice.Dialect;

    /// <summary>
    /// 棒読みちゃんに発生文字列を与えます。
    /// </summary>
    /// <remarks>
    /// 音声委譲処理は重いので、別スレッド上で行います。
    /// </remarks>
    internal static class BouyomiChan
    {
        private static Thread thread;
        private static object syncObject = new object();
        private static Queue<TalkTask> taskQueue = new Queue<TalkTask>();

        /// <summary>
        /// 発声用スレッドを初期化します。
        /// </summary>
        static BouyomiChan()
        {
            thread = new Thread(ThreadMain);
            thread.Name = "BouyomiChan Thread";
            thread.IsBackground = true;
            thread.Priority = ThreadPriority.BelowNormal;

            thread.Start();
        }

        /// <summary>
        /// 発声処理を追加します。
        /// </summary>
        public static void EnqueueTalkTask(TalkTask task)
        {
            if (task == null)
            {
                return;
            }

            task.BeginTask();

            lock (syncObject)
            {
                taskQueue.Enqueue(task);

                Monitor.PulseAll(syncObject);
            }
        }

        /// <summary>
        /// 発声処理があればそれを削除した後、返します。
        /// </summary>
        private static TalkTask DequeueTalkTask()
        {
            lock (syncObject)
            {
                if (taskQueue.Count == 0)
                {
                    return null;
                }
                
                return taskQueue.Dequeue();
            }
        }

        /// <summary>
        /// 発声データをすべて破棄します。
        /// </summary>
        public static void ClearTalkTask()
        {
            lock (syncObject)
            {
                taskQueue.Clear();
            }
        }

        /// <summary>
        /// 発声文字列を実際に発声させます。
        /// </summary>
        private static void Talk(string text)
        {
            try
            {
                using (var socket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp))
                {
                    socket.Connect(IPAddress.Loopback, 50001);

                    var msgBuffer = Encoding.UTF8.GetBytes(text);
                    var msgLength = msgBuffer.Length;

                    using (var stream = new NetworkStream(socket))
                    using (var bw = new BinaryWriter(stream))
                    {
                        bw.Write((Int16)1);
                        bw.Write((Int16)(-1));
                        bw.Write((Int16)(-1));
                        bw.Write((Int16)(-1));
                        bw.Write((Int16)0);
                        bw.Write((byte)0);

                        bw.Write((Int32)msgLength);
                        bw.Write(msgBuffer);
                    }
                }

                Util.TraceLog("'{0}'を棒読みちゃんに委譲しました。", text);
            }
            catch (Exception)
            {
                Util.TraceLog("棒読みちゃんへの接続に失敗しました。");
            }
        }

        /// <summary>
        /// 発声用のコメントを必要なら修正します。
        /// </summary>
        private static string ModifyTalkText(string text)
        {
            var model = Global.ModelObject;

            if (model.IsLimitToCommentLength)
            {
                // 必要ならコメントを短くします。
                if (text.Length > model.CommentLimitLength)
                {
                    text = text.Substring(0, model.CommentLimitLength);

                    text += model.CommentOmitText;
                }
            }

            return text;
        }

        /// <summary>
        /// 発声処理を行うための専用スレッドメソッドです。
        /// </summary>
        private static void ThreadMain(object state)
        {
            while (true)
            {
                try
                {
                    TalkTask talkTask;
                    lock (syncObject)
                    {
                        talkTask = DequeueTalkTask();
                        if (talkTask == null)
                        {
                            Monitor.Wait(syncObject, 500);
                            continue;
                        }
                    }

                    if (AppSettings.Instance.UseUserVoice)
                    {
                        // 変換操作後の文字列を取得します。
                        string text = talkTask.GetConvertedText();

                        Talk(ModifyTalkText(text));
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception)
                {
                    // 例外は無視します。
                    Util.TraceLog("ThreadMainで例外が発生しました。");
                }

                Thread.Sleep(100);
            }
        }
    }
}
