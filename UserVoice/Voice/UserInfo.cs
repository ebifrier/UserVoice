using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UserVoice.Voice
{
    /// <summary>
    /// ユーザークラスです。184とそれ以外を区別するために使います。
    /// </summary>
    [Serializable()]
    public class UserInfo : IEquatable<UserInfo>, IComparable<UserInfo>, IComparable
    {
        /// <summary>
        /// ユーザーＩＤを取得します。
        /// </summary>
        public string UserId
        {
            get;
            set;
        }

        /// <summary>
        /// 184ユーザーか取得します。
        /// </summary>
        public bool IsAnonymous
        {
            get
            {
                if (string.IsNullOrEmpty(this.UserId))
                {
                    return true;
                }

                return !this.UserId.All(c => char.IsDigit(c));
            }
        }

        /// <summary>
        /// オブジェクトの比較を行います。
        /// </summary>
        public int CompareTo(object other)
        {
            var obj = other as UserInfo;
            if (obj == null)
            {
                return -1;
            }

            return CompareTo(obj);
        }

        /// <summary>
        /// オブジェクトの比較を行います。
        /// </summary>
        public int CompareTo(UserInfo other)
        {
            if (this.IsAnonymous != other.IsAnonymous)
            {
                return (this.IsAnonymous ? +1 : -1);
            }

            return this.UserId.CompareTo(other.UserId);
        }

        /// <summary>
        /// 文字列に変換します。
        /// </summary>
        public override string ToString()
        {
            return this.UserId;
        }

        /// <summary>
        /// オブジェクトの等値性を判断します。
        /// </summary>
        public override bool Equals(object obj)
        {
            var other = obj as UserInfo;
            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        /// <summary>
        /// オブジェクトの等値性を判断します。
        /// </summary>
        public bool Equals(UserInfo other)
        {
            if (other == null)
            {
                return false;
            }

            return this.UserId.Equals(other.UserId);
        }

        /// <summary>
        /// ハッシュコードを取得します。
        /// </summary>
        public override int GetHashCode()
        {
            if (this.UserId == null)
            {
                return "".GetHashCode();
            }

            return this.UserId.GetHashCode();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserInfo(string userId)
        {
            this.UserId = userId;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public UserInfo()
        {
        }
    }
}
