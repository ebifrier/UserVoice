using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Text;

namespace UserVoice.Dialect
{
    /// <summary>
    /// 文字列を方言に変換します。
    /// </summary>
    public sealed class DialectConverterWeb : IDialectConverter
    {
        List<TextTagInfo> tagInfoList;
        List<IAsyncResult> asyncResultList;
        DateTime convertEndTime;

        /// <summary>
        /// もんじろうで適切な言葉・方言への変換を行います。
        /// </summary>
        private List<IAsyncResult> ConvertMonjiro(List<TextTagInfo> tagInfoList,
                                                 DialectType defaultDialect)
        {
            DialectType dialact = defaultDialect;
            List<IAsyncResult> asyncResultList = new List<IAsyncResult>();

            foreach (TextTagInfo info in tagInfoList)
            {
                if (info.TagType == TextTag.Dialect)
                {
                    // 変換する方言を変更します。
                    dialact = info.DialectType;
                }
                else if (info.TagType == TextTag.Unknown)
                {
                    // 空白のみの場合は変換作業を行いません。
                    if (!DialectUtil.IsStringWhitespace(info.Text) &&
                        dialact != DialectType.None)
                    {
                        IAsyncResult result = MonjiroUtil.ConvertAsync(
                            info.Text,
                            dialact,
                            (convertedText, ex) =>
                                ConvertMonjiroDone(info, convertedText, ex));

                        if (result != null)
                        {
                            asyncResultList.Add(result);
                        }
                    }
                }
            }

            return asyncResultList;
        }

        /// <summary>
        /// もんじろうでの変換後に呼ばれます。
        /// </summary>
        private void ConvertMonjiroDone(TextTagInfo tagInfo,
                                        string convertedText,
                                        Exception ex)
        {
            if (ex != null)
            {
                // 必要ならログ出力。
                //FNF.BouyomiChanApp.Pub.Information.Add
                return;
            }

            // 原則的にnullのことは無いはずなのだけど、
            // まあ一応チェックしておく。
            if (!string.IsNullOrEmpty(convertedText))
            {
                tagInfo.ConvertedText = convertedText;
            }
        }

        /// <summary>
        /// 発声用の文字列の非同期的な処理を開始します。
        /// </summary>
        /// <remarks>
        /// サイトにアクセスして変換する作業なので、時間がかかることが
        /// あります。そのため、タイムアウト時間(<param name="processTime" />)を
        /// 設定して、その時間内のみ変換作業を行うようにします。
        /// 
        /// 間に合わなかった処理はすべて無かったことになります。
        /// </remarks>
        public void BeginConvertText(string text, DialectType defaultDialect,
                                     TimeSpan processTime)
        {
            // 変換処理終了時間
            this.convertEndTime = DateTime.Now + processTime;

            // 文字列をタグと通常文字に分解したのち、
            // 通常文字列部分をもんじろうで変換します。
            this.tagInfoList = DialectUtil.SplitText(text);

            this.asyncResultList = ConvertMonjiro(
                this.tagInfoList,
                defaultDialect);
        }

        /// <summary>
        /// 変換後の文字列を取得します。
        /// </summary>
        public string GetConvertedText()
        {
            if (this.tagInfoList == null)
            {
                throw new InvalidOperationException(
                    "変換処理を開始していません。");
            }

            // 最大で処理終了時間までは待ちます。
            // それ以上かかる場合は変換自体をあきらめます。
            foreach (IAsyncResult result in this.asyncResultList)
            {
                TimeSpan elapsed = this.convertEndTime - DateTime.Now;

                // 持ち時間がある間は待ちます。
                if (elapsed >= TimeSpan.Zero)
                {
                    result.AsyncWaitHandle.WaitOne(elapsed);
                }
            }

            return DialectUtil.JoinAll(this.tagInfoList);
        }

        /// <summary>
        /// 変換する言葉・方言の一覧を取得します。
        /// </summary>
        public static List<DialectNamePair> ConvertDialectList
        {
            get;
            private set;
        }

        /// <summary>
        /// Webで変換するときの方言リストを初期化します。
        /// </summary>
        static DialectConverterWeb()
        {
            ConvertDialectList = new List<DialectNamePair>();

            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.None));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Osaka));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Samurai));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Ryoma));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Onee));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Michael));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.JoJo));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Noripi));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Trade));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Okinawa));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Hakata));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Miyazaki));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Kyoto));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Tsugaru));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Nerima));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Sigo));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.TwoChan));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Gyaru));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Nabeatu));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Yankee));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Ruu));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Yoshio));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Random));
        }
    }
}
