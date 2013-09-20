using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace UserVoice
{
    using UserVoice.Voice;

    public sealed class UserVoiceCore : DispatcherObject
    {
        /// <summary>
        /// メニューからの選択時に呼ばれます。
        /// </summary>
        public void Run()
        {
            var mainForm = Global.MainForm;

            if (mainForm == null)
            {
                mainForm = new Views.MainForm();
            }

            mainForm.Show();
            mainForm.Activate();
        }

        #region 声コマンド
        /// <summary>
        /// 声質タグのリストです。
        /// </summary>
        private static string[] voiceTags = 
        {
            "", "ai)", "shou)", "yukari)", "tomoe)", /*"zunko)"*/
        };

        /// <summary>
        /// 方言タグのリストです。
        /// </summary>
        private static string[] dialectTags =
        {
            "", "武士)", "ノリピー)", "京都)", "沖縄)", "大阪)"
        };

        private static Random rand = new Random();

        /// <summary>
        /// プリフィックスタグをランダムに選択します。
        /// </summary>
        private static string MakePrefixRandom()
        {
            var voice = voiceTags[rand.Next(voiceTags.Count())];
            //var dialect = dialectTags[rand.Next(dialectTags.Count())];

            return voice;
        }

        /// <summary>
        /// 声を追加します。
        /// </summary>
        private static void AddVoice(string prefix, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return;
            }

            var voiceInfoList = Global.ModelObject.UserVoiceInfoList;

            // prefixは""も許されます。
            prefix = (prefix ?? "");

            // 同じユーザーがすでに登録されていたら、まずそれを削除します。
            var info = Global.ModelObject.FindUserVoice(userId);
            if (info != null)
            {
                voiceInfoList.Remove(info);
            }

            // 新しい声を設定します。
            voiceInfoList.Add(new UserVoiceInfo(userId, prefix));

            // 設定された声を保存します。
            AppSettings.Save();
        }

        /// <summary>
        /// コメントのパース時に使います。
        /// </summary>
        private static readonly Regex voiceRegex = new Regex(
            @"^\s*/?(voice|koe|声)(\s+(.*))?$",
            RegexOptions.IgnoreCase);

        /// <summary>
        /// 声コマンドのプレフィックス部分のみを切り出します。
        /// </summary>
        public static string GetVoicePrefix(string comment)
        {
            if (string.IsNullOrEmpty(comment))
            {
                return null;
            }

            var m = voiceRegex.Match(comment);
            if (!m.Success)
            {
                return null;
            }

            // 設定する声部分を取り出します。
            var text = comment.Substring(m.Groups[1].Length);

            // "声"や"voice"だけの場合は、無条件で登録します。
            if (m.Groups[3].Length > 1)
            {
                // 一番右側の)までを登録します。
                var index1 = text.LastIndexOf(')');
                var index2 = text.LastIndexOf('）');
                var index = Math.Max(index1, index2);
                if (index < 0)
                {
                    // ')'がなければ登録しません。
                    return null;
                }

                text = text.Substring(0, index + 1);
            }

            return text.Trim();
        }

        /// <summary>
        /// コメントをパースします。
        /// </summary>
        public bool ParseComment(string comment, string userId)
        {
            var prefix = GetVoicePrefix(comment);
            if (prefix == null)
            {
                return false;
            }

            AddVoice(prefix, userId);
            return true;
        }
        #endregion

        #region /infoコメント
        private static readonly Regex InfoRegex = new Regex(
            @"^/info (2|6|8) ""(.+)""",
            RegexOptions.IgnoreCase);

        private DateTime lastInfo = DateTime.Now;

        /// <summary>
        /// /infoコメントを処理します。
        /// </summary>
        public void ReadInfo(string comment, int premium)
        {
            // アリーナや立ち見からコメントを拾うことがありますが、
            // NCVではそれらを区別することができません。
            if (DateTime.Now < lastInfo + TimeSpan.FromSeconds(3))
            {
                return;
            }

            if (premium == 0 || premium == 1)
            {
                return;
            }

            var m = InfoRegex.Match(comment);
            if (!m.Success)
            {
                return;
            }

            var n = int.Parse(m.Groups[1].Value);
            var str = m.Groups[2].Value;

            if (n == 2)
            {
                ReadCommunityInfo(str);
            }
            else if (n == 6)
            {
                ReadEarthquake(str);
            }
            else if (n == 8)
            {
                ReadRankingInfo(str);
            }

            lastInfo = DateTime.Now;
        }

        /// <summary>
        /// 指定のディレクトリ中のSEをランダムで一つ選びます。
        /// </summary>
        private string GetSEPath(string dirName)
        {
            try
            {
                var asm = Assembly.GetExecutingAssembly();
                var basePath = Path.Combine(
                    Path.GetDirectoryName(asm.Location),
                    "Data");
                basePath = Path.Combine(basePath, dirName);

                var files = Directory.GetFiles(basePath, "*.wav");
                return files[rand.Next(files.Count())];
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 別スレッド上で、音声を再生します。
        /// </summary>
        private void PlayComminityInfoSound()
        {
            try
            {
                // SEを再生します。
                var path = GetSEPath("Community");
                if (!string.IsNullOrEmpty(path))
                {
                    var player = new SoundPlayer(path);
                    player.PlaySync();
                }

                // SE後に効果音を流します。
                path = GetSEPath("After");
                if (!string.IsNullOrEmpty(path))
                {
                    var player = new SoundPlayer(path);
                    player.PlaySync();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    "PlayComminityInfoSound: 例外が発生しました。({0})",
                    ex.Message);
            }
        }

        /// <summary>
        /// 地震速報を読み上げます。
        /// </summary>
        private void ReadEarthquake(string text)
        {
            ReadOut(text, "");
        }

        /// <summary>
        /// コミュ参加時のコメントを読み上げます。
        /// </summary>
        private void ReadCommunityInfo(string text)
        {
            ThreadPool.QueueUserWorkItem(
                (_) => PlayComminityInfoSound());

            try
            {
                //this.host.SendOwnerComment("NoTalk", text, "");
            }
            catch
            {
            }
        }

        /// <summary>
        /// ランキング時のコメントを読み上げます。
        /// </summary>
        private void ReadRankingInfo(string text)
        {
            try
            {
                //this.host.SendOwnerComment(text);
            }
            catch
            {
            }
        }
        #endregion

        #region 読み上げ
        private static readonly Regex BspRegex = new Regex(
            @"^/press show (.+?)\s+(.+)\s+[@]\s*(.+)$");

        /// <summary>
        /// 読み上げるコメントの必要な部分のみを取り出します。
        /// </summary>
        public static string GetReadOutTextCore(string comment)
        {
            if (string.IsNullOrEmpty(comment))
            {
                return string.Empty;
            }

            // BSPコメントならコメント部分のみを抜き出します。
            var m = BspRegex.Match(comment);
            return (m.Success ? m.Groups[2].Value : comment);
        }

        /// <summary>
        /// 読み上げコメントのプレフィックスを取得します。
        /// </summary>
        public static string GetReadOutPrefix(string userId)
        {
            var voiceInfo = Global.ModelObject.FindUserVoice(userId);
            var prefix = "";

            // もし設定された声があればそれを付加します。
            if (voiceInfo != null)
            {
                prefix = voiceInfo.VoicePrefix;
            }
            else
            {
                // 声タグが登録されてない場合は、ランダムで勝手に登録します。
                prefix = MakePrefixRandom();
                AddVoice(prefix, userId);
            }

            return prefix;
        }

        /// <summary>
        /// コメントの読み上げを行います。
        /// </summary>
        public void ReadOut(string comment, string userId)
        {
            var core = GetReadOutTextCore(comment);
            if (string.IsNullOrEmpty(core))
            {
                return;
            }

            var prefix = GetReadOutPrefix(userId);

            // 言葉・方言変換を別スレッドで行うためのオブジェクトです。
            // 少し時間のかかる処理のため、同一スレッドでやると
            // NCV本体が止まったり、２回コメントが来るようになったりします。
            var task = new TalkTask(prefix + " " + core);

            BouyomiChan.EnqueueTalkTask(task);
        }
        #endregion
    }
}
