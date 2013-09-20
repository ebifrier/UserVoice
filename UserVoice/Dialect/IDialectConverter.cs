using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserVoice.Dialect
{
    /// <summary>
    /// 言葉・方言変換を行うクラスの基底です。
    /// </summary>
    public interface IDialectConverter
    {
        /// <summary>
        /// 言葉・方言変換を開始します。
        /// </summary>
        void BeginConvertText(string text, DialectType defaultDialect,
                              TimeSpan processTime);

        /// <summary>
        /// 言葉・方言変換をした文字列を取得します。
        /// </summary>
        /// <remarks>
        /// メソッド内でwaitされることがあります。
        /// </remarks>
        string GetConvertedText();
    }
}
