using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace UserVoice
{
    /// <summary>
    /// 便利屋クラスです。
    /// </summary>
    public static class Util
    {
        /// <summary>
        /// Unix時間の基準時刻です。
        /// </summary>
        private static readonly DateTime Epoch =
            new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Unix時間をDateTimeに変換します。
        /// </summary>
        public static DateTime UnixTimeToDateTime(string timeText)
        {
            double seconds = double.Parse(
                timeText,
                CultureInfo.InvariantCulture);

            // 基準時刻に加算し、ローカル時刻に直します。
            var date = Epoch.AddSeconds(seconds);
            return date.ToLocalTime();
        }

        /// <summary>
        /// htmlのタグ文字を解析します。
        /// </summary>
        private static readonly Regex htmlTagRegex =
            new Regex(@"</?\w+[^>]*?>", RegexOptions.Compiled);

        /// <summary>
        /// 与えられた文字列からタグ部分を削除します。
        /// </summary>
        public static string EliminateTags(string text)
        {
            return htmlTagRegex.Replace(text, "");
        }

        /// <summary>
        /// UIスレッドに関連づけられたディスパッチャーを取得します。
        /// </summary>
        public static Dispatcher UIDispatcher
        {
            get
            {
                if (Application.Current == null)
                {
                    return null;
                }

                return Application.Current.Dispatcher;
            }
        }

        /// <summary>
        /// 与えられた手続きをUIスレッド上で実行します。
        /// </summary>
        public static void UIProcess(Action func)
        {
            var dispatcher = UIDispatcher;

            if (dispatcher == null || dispatcher.CheckAccess())
            {
                func();
            }
            else
            {
                dispatcher.BeginInvoke(func);
            }
        }

        /// <summary>
        /// トレースログを出力します。
        /// </summary>
        public static void TraceLog(string message)
        {
            /*Trace.TraceInformation(message);
            Trace.Flush();*/
        }

        /// <summary>
        /// トレースログを出力します。
        /// </summary>
        public static void TraceLog(string message, params object[] args)
        {
            /*Trace.TraceInformation(message, args);
            Trace.Flush();*/
        }
    }
}
