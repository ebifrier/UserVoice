using System;//
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Net;

namespace UserVoice.Dialect
{
    /// <summary>
    /// もんじろうでの失敗を通知する例外です。
    /// </summary>
    public class MonjiroException : Exception
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MonjiroException(string message)
            : base(message)
        {
        }
    }

    /// <summary>
    /// サイト もんじろうにアクセスするためのクラスです。
    /// </summary>
    public static class MonjiroUtil
    {
        /// <summary>
        /// 扱うエンコーディングを取得します。
        /// </summary>
        private static readonly Encoding defaultEncoding =
            Encoding.GetEncoding("EUC-JP");

        /// <summary>
        /// もんじろうの変換後の文字列を取得します。
        /// </summary>
        /// <remarks>
        /// サイト構成が変わった場合、速やかに修正する必要があります。
        /// </remarks>
        private static readonly Regex afterRegex = new Regex(
            "<p class=\"text\"><small>" +
            "▼<a href=\"http://monjiro[.]net/hougen/conv/[\\d]+/\">[^<]+</a> " +
            "by <a href=\"http://monjiro[.]net/\">http://monjiro[.]net/</a></small><br />" +
            "(.+?)</p>",
            RegexOptions.Compiled);

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static MonjiroUtil()
        {
            DefaultTimeout = TimeSpan.FromSeconds(1.0);
        }

        /// <summary>
        /// タイムアウト時間を取得または設定します。
        /// </summary>
        public static TimeSpan DefaultTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// ストリームの内容を読み出します。
        /// </summary>
        private static byte[] ReadToEnd(Stream stream)
        {
            using (MemoryStream result = new MemoryStream())
            {
                byte[] buffer = new byte[1024];
                int size = 0;

                while ((size = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    result.Write(buffer, 0, size);
                }

                result.Flush();
                return result.ToArray();
            }
        }

        /// <summary>
        /// リクエストを行います。
        /// </summary>
        private static HttpWebRequest MakeRequest(string text, DialectType dialect)
        {
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            string data = string.Format(
                "gn_id={0}&mode=conv&before={1}",
                (int)dialect,
                HttpUtility.UrlEncode(text, defaultEncoding));
            byte[] deData = defaultEncoding.GetBytes(data);

            // リクエストの作成。
            HttpWebRequest request = (HttpWebRequest)
                HttpWebRequest.Create("http://monjiro.net/");
            request.Method = "POST";
            request.Referer = "http://monjiro.net/";
            request.Timeout = (int)DefaultTimeout.TotalMilliseconds;

            // 送信データの設定。
            request.ContentLength = deData.Length;
            request.ContentType = "application/x-www-form-urlencoded";
            using (Stream reqStream = request.GetRequestStream())
            {
                reqStream.Write(deData, 0, deData.Length);
            }

            return request;
        }

        /// <summary>
        /// もんじろうでの変換後の文字列を受け取ります。
        /// </summary>
        public delegate void ConvertHandler(string text, Exception ex);

        /// <summary>
        /// 非同期的な変換を行います。
        /// </summary>
        public static IAsyncResult ConvertAsync(string text, DialectType dialect,
                                                ConvertHandler callback)
        {
            if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentNullException("text");
            }
            if (callback == null)
            {
                throw new ArgumentNullException("callback");
            }

            try
            {
                HttpWebRequest request = MakeRequest(text, dialect);

                // リクエストを送ります。
                return request.BeginGetResponse(
                    result => ConvertAsyncDone(result, callback),
                    request);
            }
            catch (Exception ex)
            {
                callback(null, ex);

                return null;
            }
        }

        /// <summary>
        /// レスポンス受信時に呼ばれます。
        /// </summary>
        private static void ConvertAsyncDone(IAsyncResult result,
                                             ConvertHandler callback)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                WebResponse response = request.EndGetResponse(result);

                // レスポンスから変換後文字列を取り出します。
                using (Stream stream = response.GetResponseStream())
                {
                    byte[] resData = ReadToEnd(stream);
                    string resText = defaultEncoding.GetString(resData);

                    // 変換後文字列を取得します。
                    Match m = afterRegex.Match(resText);
                    if (m.Success)
                    {
                        string afterText = m.Groups[1].Value;

                        // タグを消す&エスケープされた文字列を元に戻します。
                        callback(
                            HttpUtility.HtmlDecode(
                                Util.EliminateTags(afterText)),
                            null);
                    }
                    else
                    {
                        throw new MonjiroException(
                            "もんじろうの変換後の文字列取得に失敗しました。");
                    }
                }
            }
            catch (Exception ex)
            {
                // エラーの場合は、例外オブジェクトを送ります。
                callback(null, ex);
            }
        }
    }
}
