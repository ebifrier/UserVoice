using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

using UserVoice;

namespace UnitTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Assert.AreEqual(
                "わこつ",
                UserVoiceCore.GetReadOutTextCore("/press show green わこつ @ 宣誓"));
            Assert.AreEqual(
                "わこつ だお",
                UserVoiceCore.GetReadOutTextCore("/press show green わこつ だお @テスト"));

            Assert.AreEqual(
                "zunko)",
                UserVoiceCore.GetVoicePrefix("声 zunko)"));
            Assert.AreEqual(
                "zunko)",
                UserVoiceCore.GetVoicePrefix("声 zunko) これでいい？"));
            Assert.AreEqual(
                "こういうのは zunko)",
                UserVoiceCore.GetVoicePrefix("声 こういうのは zunko)"));
            Assert.AreEqual(
                "",
                UserVoiceCore.GetVoicePrefix("声"));

            /*var converter = new UserVoice.Dialect.DialectConverterWeb();
            converter.BeginConvertText(
                "教育(野武士=www) 武士) 音量(200) 教育(野武士=www) おはようございます ノリピー) おはようございます",
                UserVoice.Dialect.DialectType.Noripi,
                TimeSpan.FromSeconds(10));*/
        }

        /// <summary>
        /// ショウタを高速で読み上げさせると２回読み上げるというバグを発生させます。
        /// </summary>
        static void RepeatTest()
        {
            for (int i = 0; i < 10; ++i)
            {
            }
        }
    }
}
