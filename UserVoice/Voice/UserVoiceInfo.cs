using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserVoice.Voice
{
    /// <summary>
    /// 各ユーザーの声設定を保存します。
    /// </summary>
    [Serializable()]
    public class UserVoiceInfo
    {
        /// <summary>
        /// ユーザーを取得します。
        /// </summary>
        public UserInfo User
        {
            get;
            set;
        }

        /// <summary>
        /// 棒読みちゃんに読ませるコメントのプレフィックスを取得します。
        /// </summary>
        public string VoicePrefix
        {
            get;
            set;
        }

        /// <summary>
        /// 登録時刻を取得します。
        /// </summary>
        public DateTime Date
        {
            get;
            set;
        }

        public UserVoiceInfo(string userId, string voicePrefix)
        {
            this.User = new UserInfo(userId);
            this.VoicePrefix = voicePrefix;
            this.Date = DateTime.Now;
        }

        public UserVoiceInfo()
        {
        }
    }
}
