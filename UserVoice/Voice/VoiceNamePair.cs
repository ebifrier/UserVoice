using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace UserVoice.Voice
{
    /// <summary>
    /// 声とその名前の組を保持します。
    /// </summary>
    [Serializable()]
    public class VoiceNamePair
    {
        /// <summary>
        /// 声に関連づけられた別名を取得します。
        /// </summary>
        public string Alias
        {
            get;
            private set;
        }

        /// <summary>
        /// 声質情報を取得します。
        /// </summary>
        public VoiceType VoiceType
        {
            get;
            private set;
        }

        /// <summary>
        /// 声質の名前を取得します。
        /// </summary>
        public string VoiceTypeName
        {
            get
            {
                if (this.VoiceType == null)
                {
                    return "";
                }

                return this.VoiceType.Name;
            }
        }

        public VoiceNamePair(VoiceType voiceType, string alias)
        {
            this.VoiceType = voiceType;
            this.Alias = alias;
        }
    }
}
