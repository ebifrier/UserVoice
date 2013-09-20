using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserVoice.Voice
{
    /// <summary>
    /// 声質情報を保持します。
    /// </summary>
    [Serializable()]
    public class VoiceType
    {
        /// <summary>
        /// 声質のＩＤを取得します。
        /// </summary>
        public int Id
        {
            get;
            private set;
        }

        /// <summary>
        /// 声質の名前を取得します。
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        public VoiceType(int id, string name)
        {
            this.Id = id;
            this.Name = name;
        }
    }
}
