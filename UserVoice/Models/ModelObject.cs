using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Data;

namespace UserVoice.Models
{
    using UserVoice.Dialect;
    using UserVoice.Voice;

    /// <summary>
    /// モデルオブジェクトです。
    /// </summary>
    internal class ModelObject : INotifyPropertyChanged
    {
        private List<UserVoiceInfo> currentUserVoiceInfoList =
            new List<UserVoiceInfo>();

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// ユーザーの声設定リストを取得します。
        /// </summary>
        public ObservableCollection<UserVoiceInfo> UserVoiceInfoList
        {
            get
            {
                return AppSettings.Instance.UserVoiceInfoList;
            }
        }

        /// <summary>
        /// 現在のユーザーの声設定を取得します。
        /// </summary>
        public List<UserVoiceInfo> CurrentUserVoiceInfoList
        {
            get
            {
                return this.currentUserVoiceInfoList;
            }
            set
            {
                if (this.currentUserVoiceInfoList != value)
                {
                    this.currentUserVoiceInfoList = value;

                    OnPropertyChanged("CurrentUserVoiceInfoList");
                }
            }
        }

        /// <summary>
        /// 自前で変換するときの方言リストを取得します。
        /// </summary>
        public List<DialectNamePair> ConvertSelfDialectList
        {
            get
            {
                return DialectConverterSelf.ConvertDialectList;
            }
        }

        /// <summary>
        /// Webで変換するときの方言リストを取得します。
        /// </summary>
        public List<DialectNamePair> ConvertWebDialectList
        {
            get
            {
                return DialectConverterWeb.ConvertDialectList;
            }
        }

        /// <summary>
        /// 指定のユーザーＩＤの声設定があればそれを返します。
        /// </summary>
        public UserVoiceInfo FindUserVoice(string userId)
        {
            return UserVoiceInfoList.FirstOrDefault(
                (info_ => userId == info_.User.UserId));
        }

        /// <summary>
        /// ユーザーボイスによる読み上げを使用するかどうかを取得または設定します。
        /// </summary>
        public bool UseUserVoice
        {
            get { return AppSettings.Instance.UseUserVoice; }
            set
            {
                AppSettings.Instance.UseUserVoice = value;
                this.OnPropertyChanged("UseUserVoice");
            }
        }

        /// <summary>
        /// 運営ＮＧユーザーのコメントを読み上げるか取得または設定します。
        /// </summary>
        public bool IsReadOfficialNGUserComment
        {
            get { return AppSettings.Instance.IsReadOfficialNGUserComment; }
            set
            {
                AppSettings.Instance.IsReadOfficialNGUserComment = value;
                this.OnPropertyChanged("IsReadOfficialNGUserComment");
            }
        }

        /// <summary>
        /// 運営ＮＧワードに該当するコメントを読み上げるか取得または設定します。
        /// </summary>
        public bool IsReadOfficialNGWordComment
        {
            get { return AppSettings.Instance.IsReadOfficialNGWordComment; }
            set
            {
                AppSettings.Instance.IsReadOfficialNGWordComment = value;
                this.OnPropertyChanged("IsReadOfficialNGWordComment");
            }
        }

        /// <summary>
        /// 運営ＮＧコマンドに該当するコメントを読み上げるか取得または設定します。
        /// </summary>
        public bool IsReadOfficialNGCommandComment
        {
            get { return AppSettings.Instance.IsReadOfficialNGCommandComment; }
            set
            {
                AppSettings.Instance.IsReadOfficialNGCommandComment = value;
                this.OnPropertyChanged("IsReadOfficialNGCommandComment");
            }
        }

        /// <summary>
        /// 運営コメントを読み上げるか取得または設定します。
        /// </summary>
        public bool IsReadOfficialComment
        {
            get { return AppSettings.Instance.IsReadOfficialComment; }
            set
            {
                AppSettings.Instance.IsReadOfficialComment = value;
                this.OnPropertyChanged("IsReadOfficialComment");
            }
        }

        /// <summary>
        /// テロップコメントを読み上げるか取得または設定します。
        /// </summary>
        public bool IsReadTelop
        {
            get { return AppSettings.Instance.IsReadTelop; }
            set
            {
                AppSettings.Instance.IsReadTelop = value;
                this.OnPropertyChanged("IsReadTelop");
            }
        }

        /// <summary>
        /// 読み上げるコメントの文字数を制限するかどうかを取得または設定します。
        /// </summary>
        public bool IsLimitToCommentLength
        {
            get { return AppSettings.Instance.IsLimitToCommentLength; }
            set
            {
                AppSettings.Instance.IsLimitToCommentLength = value;
                this.OnPropertyChanged("IsLimitToCommentLength");
            }
        }

        /// <summary>
        /// 文字数制限をかけるときの文字数を取得または設定します。
        /// </summary>
        public int CommentLimitLength
        {
            get { return AppSettings.Instance.CommentLimitLength; }
            set
            {
                AppSettings.Instance.CommentLimitLength = value;
                this.OnPropertyChanged("CommentLimitLength");
            }
        }

        /// <summary>
        /// コメント省略時に最後につけられる文字列を取得または設定します。
        /// </summary>
        public string CommentOmitText
        {
            get { return AppSettings.Instance.CommentOmitText; }
            set
            {
                AppSettings.Instance.CommentOmitText = value;
                this.OnPropertyChanged("CommentOmitText");
            }
        }

        /// <summary>
        /// 除外文字列(正規表現)を取得または設定します。
        /// </summary>
        public string ExclusionRegString
        {
            get { return AppSettings.Instance.ExclusionRegString; }
            set
            {
                AppSettings.Instance.ExclusionRegString = value;
                this.OnPropertyChanged("ExclusionRegString");
            }
        }

        /// <summary>
        /// 方言変換を使うかどうかを取得または設定します。
        /// </summary>
        public bool IsUseDialect
        {
            get { return AppSettings.Instance.IsUseDialect; }
            set
            {
                AppSettings.Instance.IsUseDialect = value;
                this.OnPropertyChanged("IsUseDialect");
            }
        }

        /// <summary>
        /// 変換を自前でやるかを取得または設定します。
        /// </summary>
        public bool IsDialectConvertSelf
        {
            get { return AppSettings.Instance.IsDialectConvertSelf; }
            set
            {
                AppSettings.Instance.IsDialectConvertSelf = value;
                this.OnPropertyChanged("IsDialectConvertSelf");
            }
        }

        /// <summary>
        /// 変換を自前でやるときのデフォルトの方言を取得または設定します。
        /// </summary>
        public DialectType DialectConvertSelfDefaultType
        {
            get { return AppSettings.Instance.DialectConvertSelfDefaultType; }
            set
            {
                AppSettings.Instance.DialectConvertSelfDefaultType = value;
                this.OnPropertyChanged("DialectConvertSelfDefaultType");
            }
        }

        /// <summary>
        /// 変換をWebでやるかを取得または設定します。
        /// </summary>
        public bool IsDialectConvertWeb
        {
            get { return AppSettings.Instance.IsDialectConvertWeb; }
            set
            {
                AppSettings.Instance.IsDialectConvertWeb = value;
                this.OnPropertyChanged("IsDialectConvertWeb");
            }
        }

        /// <summary>
        /// 変換をWebでやるときのデフォルトの方言を取得または設定します。
        /// </summary>
        public DialectType DialectConvertWebDefaultType
        {
            get { return AppSettings.Instance.DialectConvertWebDefaultType; }
            set
            {
                AppSettings.Instance.DialectConvertWebDefaultType = value;
                this.OnPropertyChanged("DialectConvertWebDefaultType");
            }
        }

        /// <summary>
        /// コメントを読み上げるか調べます。
        /// </summary>
        public bool IsReadComment(string comment, string mail, bool isBSP,
                                  int premium)
        {
            if (!UseUserVoice)
            {
                return false;
            }

            if (string.IsNullOrEmpty(comment))
            {
                return false;
            }

            if (comment[0] == '/')
            {
                return false;
            }

            if (mail != null && mail.Contains("NoTalk"))
            {
                return false;
            }

            if (isBSP)
            {
                return IsReadTelop;
            }

            if (4 <= premium && premium <= 6)
            {
                return IsReadOfficialComment;
            }

            return true;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ModelObject()
        {
            /*this.UserVoiceInfoList.Add(new UserVoiceInfo("11test", "MK)"));
            this.UserVoiceInfoList.Add(new UserVoiceInfo("11822", "MJ)"));
            this.UserVoiceInfoList.Add(new UserVoiceInfo("11822test", "MK)"));
            this.UserVoiceInfoList.Add(new UserVoiceInfo("12399", "MK)"));*/
        }
    }
}
