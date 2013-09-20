using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Text;

namespace UserVoice.Dialect
{
    /// <summary>
    /// 方言変換をしないコンバータークラスです。
    /// </summary>
    public sealed class DialectConverterNull : IDialectConverter
    {
        private string text;

        /// <summary>
        /// 言葉・方言変換をしません。
        /// </summary>
        public void BeginConvertText(string text, DialectType defaultDialect,
                                     TimeSpan processTime)
        {
            this.text = text;
        }

        /// <summary>
        /// 変換後の文字列を取得します。
        /// </summary>
        public string GetConvertedText()
        {
            return this.text;
        }
    }
}
