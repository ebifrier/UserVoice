using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace UserVoice.Dialect
{
    /// <summary>
    /// タグ一覧
    /// </summary>
    internal enum TextTag
    {
        /// <summary>
        /// 不明。主にテキストの場合が多いです。
        /// </summary>
        Unknown,
        /// <summary>
        /// 棒読みちゃんのタグ文字列
        /// </summary>
        Tag,
        /// <summary>
        /// 方言タグ
        /// </summary>
        Dialect,
    }

    /// <summary>
    /// 文字列とそのタグを対で持ちます。
    /// </summary>
    internal class TextTagInfo
    {
        /// <summary>
        /// 対象となるテキストを取得または設定します。
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// 設定されたタグを取得または設定します。
        /// </summary>
        public TextTag TagType
        {
            get;
            set;
        }

        /// <summary>
        /// 方言を取得または設定します。
        /// </summary>
        public DialectType DialectType
        {
            get;
            set;
        }

        /// <summary>
        /// 方言変換後の文字列を取得または設定します。
        /// </summary>
        public string ConvertedText
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TextTagInfo()
        {
        }
    }

    /// <summary>
    /// 言葉・方言関係のユーティリティクラスです。
    /// </summary>
    internal static class DialectUtil
    {
        #region DialectAttribute
        /// <summary>
        /// <see cref="DialectNamePair"/>を作成します。
        /// </summary>
        public static DialectNamePair MakeDialectNamePair(DialectType dialectType)
        {
            var attribute = GetAttribute(dialectType);
            if (attribute == null)
            {
                return new DialectNamePair(dialectType, "エラー");
            }

            return new DialectNamePair(
                dialectType,
                string.Format("{0} ({1})",
                    attribute.DisplayName,
                    attribute.TagName));
        }

        /// <summary>
        /// DialectAttribute属性を取得します。
        /// </summary>
        public static DialectAttribute GetAttribute<T>(T dialectType)
        {
            // 列挙型の値から、その値のリフレクション情報を得ます。
            // なんかめんどくさい。。。
            var enumName = Enum.GetName(typeof(T), dialectType);
            if (string.IsNullOrEmpty(enumName))
            {
                return null;
            }

            // 属性を取得します。
            var enumInfo = typeof(T).GetField(enumName);
            var attributes = enumInfo.GetCustomAttributes(
                typeof(DialectAttribute),
                false);
            if (attributes.Length == 0)
            {
                return null;
            }

            return (DialectAttribute)attributes[0];
        }

        /// <summary>
        /// 表示名を取得します。
        /// </summary>
        public static string GetDisplayName<T>(T dialectType)
        {
            var attribute = GetAttribute(dialectType);
            if (attribute == null)
            {
                return null;
            }

            return attribute.DisplayName;
        }

        /// <summary>
        /// タグ名を取得します。
        /// </summary>
        public static string GetTagName<T>(T dialectType)
        {
            var attribute = GetAttribute(dialectType);
            if (attribute == null)
            {
                return null;
            }

            return attribute.TagName;
        }
        #endregion

        #region 言葉・方言変換
        /// <summary>
        /// 変換すべき/すべきでないタグ一覧を保持します。
        /// </summary>
        private static readonly Regex[] tagRegexList = {
            new Regex(@"(?:やまびこ|エコー)[)）]", RegexOptions.IgnoreCase),
            new Regex(@"(?:(?:声[質優種色]*)|速度|音程|音量)[（(]\d*[)）]", RegexOptions.IgnoreCase),
            //new Regex(@"教育[（(][^=＝]*[=＝][^)）]*[)）]", RegexOptions.IgnoreCase),
            //new Regex(@"忘却[（(][^)）]*[)）]", RegexOptions.IgnoreCase),
            new Regex(@"再生[（(][^)）]*[)）]", RegexOptions.IgnoreCase),
            new Regex(@"残響[（(][\d.]*[\s,]+[\d.]*[\s,]+[\d.]*[)）]", RegexOptions.IgnoreCase),

            CreateDialectRegex(),

            new Regex(@"[a-zA-Z0-9_]+[)）]", RegexOptions.IgnoreCase),
        };

        /// <summary>
        /// 言葉・方言の正規表現パターンを作成します。
        /// </summary>
        private static Regex CreateDialectRegex()
        {
            List<string> dialectStrList = new List<string>();

            foreach (DialectType dialect in Enum.GetValues(typeof(DialectType)))
            {
                dialectStrList.Add(
                    Regex.Escape(DialectUtil.GetDisplayName(dialect)));
                dialectStrList.Add(
                    Regex.Escape(DialectUtil.GetTagName(dialect)));
            }

            dialectStrList.Sort(new Comparison<string>(
                (x, y) => y.Length - x.Length));

            StringBuilder result = new StringBuilder();
            result.Append("[（(]?(");
            result.Append(string.Join(
                "|",
                dialectStrList.ToArray()));
            result.Append(")[)）]");

            return new Regex(
                result.ToString(),
                RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 文字列の正規化を行います。
        /// </summary>
        private static string Normalize(string text)
        {
            return Strings.StrConv(text, VbStrConv.Wide | VbStrConv.Hiragana);
        }

        /// <summary>
        /// 名前からその方言を検索します。
        /// </summary>
        public static DialectType FindDialect(string dialectName)
        {
            foreach (DialectType value in Enum.GetValues(typeof(DialectType)))
            {
                DialectAttribute attribute = DialectUtil.GetAttribute(value);
                if (attribute == null)
                {
                    continue;
                }

                // 方言名の比較時はひらがな/カタカナ、
                // ア/ァ などの区別はしないようにします。
                string normalized = Normalize(dialectName);
                if (normalized == Normalize(attribute.DisplayName) ||
                    normalized == Normalize(attribute.TagName))
                {
                    return value;
                }
            }

            return DialectType.None;
        }

        /// <summary>
        /// 指定の文字列を指定の正規表現で分割します。
        /// </summary>
        private static List<TextTagInfo> SplitText(string text, Regex re)
        {
            List<TextTagInfo> result = new List<TextTagInfo>();

            string piece = text;
            while (true)
            {
                Match m = re.Match(piece);
                if (!m.Success)
                {
                    if (!string.IsNullOrEmpty(piece))
                    {
                        result.Add(new TextTagInfo()
                        {
                            Text = piece,
                            TagType = TextTag.Unknown,
                        });
                    }
                    return result;
                }

                // マッチ部分より前の部分を単なる文字列として登録します。
                if (m.Index > 0)
                {
                    result.Add(new TextTagInfo()
                    {
                        Text = piece.Substring(0, m.Index),
                        TagType = TextTag.Unknown,
                    });
                }

                // 言葉・方言タグの場合は後方参照があります。
                if (m.Groups.Count <= 1)
                {
                    result.Add(new TextTagInfo()
                    {
                        Text = m.Value,
                        TagType = TextTag.Tag,
                    });
                }
                else
                {
                    // タグが見つからない場合は無変換になります。
                    var d = FindDialect(m.Groups[1].Value);

                    result.Add(new TextTagInfo()
                    {
                        Text = m.Value,
                        TagType = TextTag.Dialect,
                        DialectType = d,
                    });
                }

                // マッチ部分以降から、再度調べます。
                piece = piece.Substring(m.Index + m.Length);
            }
        }

        /// <summary>
        /// 文字列をタグなどに分解します。
        /// </summary>
        public static List<TextTagInfo> SplitText(string text)
        {
            // 初期値は全文字列(タグは不明)です。
            List<TextTagInfo> result = new List<TextTagInfo>()
            {
                new TextTagInfo()
                {
                    Text = text,
                    TagType = TextTag.Unknown,
                }
            };

            foreach (Regex re in tagRegexList)
            {
                // リストの各文字列からタグを探します。
                List<TextTagInfo> result_ = result;
                result = new List<TextTagInfo>();

                foreach (TextTagInfo info in result_)
                {
                    // テキスト部分から方言タグを探します。
                    if (info.TagType == TextTag.Unknown)
                    {
                        result.AddRange(SplitText(info.Text, re));
                    }
                    else
                    {
                        result.Add(info);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 文字列がすべて空白文字かどうか調べます。
        /// </summary>
        public static bool IsStringWhitespace(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return true;
            }

            for (int i = 0; i < text.Length; ++i)
            {
                if (!char.IsWhiteSpace(text[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// リスト中の文字列をすべてつなげます。
        /// </summary>
        public static string JoinAll(IEnumerable<TextTagInfo> tagInfoList)
        {
            StringBuilder result = new StringBuilder();

            foreach (TextTagInfo info in tagInfoList)
            {
                if (info.TagType == TextTag.Dialect)
                {
                }
                else if (info.TagType == TextTag.Tag)
                {
                    result.Append(info.Text);
                }
                else
                {
                    // ConvertedTextは別スレッドで書き換えられる可能性が
                    // あるため一応コピーしておく。
                    string convertedText = info.ConvertedText;

                    result.Append(
                        convertedText != null ?
                        convertedText :
                        info.Text);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// 言葉・方言変換を行うオブジェクトを作成します。
        /// </summary>
        public static IDialectConverter CreateConverter()
        {
            IDialectConverter converter;

            if (!AppSettings.Instance.IsUseDialect)
            {
                return new DialectConverterNull();
            }

            if (AppSettings.Instance.IsDialectConvertWeb)
            {
                converter = new DialectConverterWeb();
            }
            else if (AppSettings.Instance.IsDialectConvertSelf)
            {
                converter = new DialectConverterSelf();
            }
            else
            {
                converter = new DialectConverterNull();
            }

            return converter;
        }

        /// <summary>
        /// 言葉・方言変換のデフォルト方言を取得します。
        /// </summary>
        public static DialectType GetDefaultDialect()
        {
            if (!AppSettings.Instance.IsUseDialect)
            {
                return DialectType.None;
            }

            if (AppSettings.Instance.IsDialectConvertWeb)
            {
                return AppSettings.Instance.DialectConvertWebDefaultType;
            }
            else if (AppSettings.Instance.IsDialectConvertSelf)
            {
                return AppSettings.Instance.DialectConvertSelfDefaultType;
            }
            else
            {
                return DialectType.None;
            }
        }
        #endregion
    }
}
