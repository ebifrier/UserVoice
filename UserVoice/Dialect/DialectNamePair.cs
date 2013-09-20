using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserVoice.Dialect
{
    /// <summary>
    /// 方言の名前とタイプを保持します。
    /// </summary>
    public class DialectNamePair
    {
        /// <summary>
        /// 方言を取得します。
        /// </summary>
        public DialectType DialectType
        {
            get;
            private set;
        }

        /// <summary>
        /// 方言の表示名を取得します。
        /// </summary>
        public string DisplayName
        {
            get;
            private set;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DialectNamePair(DialectType dialectType, string displayName)
        {
            this.DialectType = dialectType;
            this.DisplayName = displayName;
        }
    }
}
