using System;
using System.Collections.Generic;
using System.Text;

namespace UserVoice.Dialect
{
    /// <summary>
    /// 方言の属性です。
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class DialectAttribute : Attribute
    {
        /// <summary>
        /// 表示名を取得または設定します。
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// タグに使われる名前を取得または設定します。
        /// </summary>
        public string TagName
        {
            get;
            set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DialectAttribute(string displayName, string tagName)
        {
            DisplayName = displayName;
            TagName = tagName;
        }
    }
}
