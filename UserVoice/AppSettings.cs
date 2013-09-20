using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace UserVoice
{
    using UserVoice.Dialect;
    using UserVoice.Voice;

    /// <summary>
    /// このアプリの設定情報の保存/復元を行います。
    /// </summary>
    [Serializable()]
    public class AppSettings
    {
        private object syncObject = new object();
        private ObservableCollection<UserVoiceInfo> userVoiceInfoList =
            new ObservableCollection<UserVoiceInfo>();
        private bool useUserVoice = true;
        private bool isReadOfficialNGUserComment = false;
        private bool isReadOfficialNGWordComment = false;
        private bool isReadOfficialNGCommandComment = false;
        private bool isReadOfficialComment = true;
        private bool isReadTelop = true;
        private bool isLimitToCommentLength = true;
        private int commentLimitLength = 256;
        private string commentOmitText = " りゃく";
        private string exclusionRegString = "s?https?://[-_.!~*'()a-zA-Z0-9;/?:@&=+$,%#]+";
        private bool isUseDialect = false;
        private bool isDialectConvertSelf = true;
        private DialectType dialectConvertSelfDefaultType = DialectType.None;
        private bool isDialectConvertWeb = false;
        private DialectType dialectConvertWebDefaultType = DialectType.None;

        /// <summary>
        /// 各ユーザーの声設定リストを取得します。
        /// </summary>
        public ObservableCollection<UserVoiceInfo> UserVoiceInfoList
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.userVoiceInfoList;
                }
            }
        }

        /// <summary>
        /// ユーザーボイスによる読み上げを使用するかどうかを取得または設定します。
        /// </summary>
        public bool UseUserVoice
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.useUserVoice;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.useUserVoice = value;
                }
            }
        }

        /// <summary>
        /// 運営ＮＧユーザーのコメントを読み上げるか取得または設定します。
        /// </summary>
        public bool IsReadOfficialNGUserComment
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.isReadOfficialNGUserComment;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.isReadOfficialNGUserComment = value;
                }
            }
        }

        /// <summary>
        /// 運営ＮＧワードに該当するコメントを読み上げるか取得または設定します。
        /// </summary>
        public bool IsReadOfficialNGWordComment
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.isReadOfficialNGWordComment;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.isReadOfficialNGWordComment = value;
                }
            }
        }

        /// <summary>
        /// 運営ＮＧコマンドに該当するコメントを読み上げるか取得または設定します。
        /// </summary>
        public bool IsReadOfficialNGCommandComment
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.isReadOfficialNGCommandComment;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.isReadOfficialNGCommandComment = value;
                }
            }
        }

        /// <summary>
        /// 運営コメントを読み上げるか取得または設定します。
        /// </summary>
        public bool IsReadOfficialComment
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.isReadOfficialComment;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.isReadOfficialComment = value;
                }
            }
        }

        /// <summary>
        /// テロップコメントを読み上げるか取得または設定します。
        /// </summary>
        public bool IsReadTelop
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.isReadTelop;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.isReadTelop = value;
                }
            }
        }

        /// <summary>
        /// 読み上げるコメントの文字数を制限するかどうかを取得または設定します。
        /// </summary>
        public bool IsLimitToCommentLength
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.isLimitToCommentLength;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.isLimitToCommentLength = value;
                }
            }
        }

        /// <summary>
        /// 文字数制限をかけるときの文字数を取得または設定します。
        /// </summary>
        public int CommentLimitLength
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.commentLimitLength;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.commentLimitLength = value;
                }
            }
        }

        /// <summary>
        /// コメント省略時に最後につけられる文字列を取得または設定します。
        /// </summary>
        public string CommentOmitText
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.commentOmitText;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.commentOmitText = value;
                }
            }
        }

        /// <summary>
        /// 除外文字列(正規表現)を取得または設定します。
        /// </summary>
        public string ExclusionRegString
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.exclusionRegString;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.exclusionRegString = value;
                }
            }
        }

        /// <summary>
        /// 方言変換機能を使うかどうかを取得または設定します。
        /// </summary>
        public bool IsUseDialect
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.isUseDialect;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.isUseDialect = value;
                }
            }
        }

        /// <summary>
        /// 方言変換を自前でやるかを取得または設定します。
        /// </summary>
        public bool IsDialectConvertSelf
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.isDialectConvertSelf;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.isDialectConvertSelf = value;
                }
            }
        }

        /// <summary>
        /// 方言変換を自前で行うときのデフォルトの方言を取得または設定します。
        /// </summary>
        public DialectType DialectConvertSelfDefaultType
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.dialectConvertSelfDefaultType;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.dialectConvertSelfDefaultType = value;
                }
            }
        }
        
        /// <summary>
        /// 方言変換をサイトでやるかどうかを取得または設定します。
        /// </summary>
        public bool IsDialectConvertWeb
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.isDialectConvertWeb;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.isDialectConvertWeb = value;
                }
            }
        }

        /// <summary>
        /// 方言変換をWebで行うときのデフォルトの方言を取得または設定します。
        /// </summary>
        public DialectType DialectConvertWebDefaultType
        {
            get
            {
                lock (this.syncObject)
                {
                    return this.dialectConvertWebDefaultType;
                }
            }
            set
            {
                lock (this.syncObject)
                {
                    this.dialectConvertWebDefaultType = value;
                }
            }
        }

        private static AppSettings instance = new AppSettings();
        private static string dataFilePath = null;

        /// <summary>
        /// クラス唯一のインスタンスを取得します。
        /// </summary>
        [XmlIgnore()]
        public static AppSettings Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// アプリケーション設定ファイルのパスを取得または設定します。
        /// </summary>
        [XmlIgnore()]
        public static string DataFilePath
        {
            get { return dataFilePath; }
            set { dataFilePath = value; }
        }

        /// <summary>
        /// XMLファイルからデータを読み込みます。
        /// </summary>
        public static void Load()
        {
            if (string.IsNullOrEmpty(DataFilePath))
            {
                return;
            }

            // 初回読み込み時にはファイルが無いことがあります。
            if (!File.Exists(DataFilePath))
            {
                return;
            }

            object result;
            using (var fs = new FileStream(DataFilePath, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(AppSettings));
                result = serializer.Deserialize(fs);
            }

            instance = (AppSettings)result;
        }

        /// <summary>
        /// XMLファイルにデータを保存します。
        /// </summary>
        public static void Save()
        {
            if (string.IsNullOrEmpty(DataFilePath))
            {
                return;
            }

            using (var fs = new FileStream(DataFilePath, FileMode.Create))
            {
                var serializer = new XmlSerializer(typeof(AppSettings));
                serializer.Serialize(fs, instance);
            }
        }
    }
}
