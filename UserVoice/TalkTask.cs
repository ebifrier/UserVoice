using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading;

namespace UserVoice
{
    using UserVoice.Dialect;

    /// <summary>
    /// 言葉・方言変換を非同期的に行うためのオブジェクトです。
    /// </summary>
    public class TalkTask
    {
        private ManualResetEvent taskEvent = new ManualResetEvent(false);
        private IDialectConverter converter;
        private string text;

        /// <summary>
        /// タスクの処理を開始します。
        /// </summary>
        public void BeginTask()
        {
            ThreadPool.QueueUserWorkItem(ConvertTaskAsync);
        }

        /// <summary>
        /// 実際に処理を行うメソッド。
        /// </summary>
        private void ConvertTaskAsync(object state)
        {
            // htmlのデコードを行います。
            string newText = HttpUtility.HtmlDecode(
                Util.EliminateTags(this.text));

            // 変換作業を開始します。
            this.converter = DialectUtil.CreateConverter();

            this.converter.BeginConvertText(
                newText,
                DialectUtil.GetDefaultDialect(),
                TimeSpan.FromMilliseconds(500));

            this.taskEvent.Set();
        }

        /// <summary>
        /// 言葉・方言変換後の文字列を取得します。
        /// </summary>
        public string GetConvertedText()
        {
            this.taskEvent.WaitOne();

            return this.converter.GetConvertedText();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TalkTask(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }

            this.text = text;
        }
    }
}
