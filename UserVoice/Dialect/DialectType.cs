using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace UserVoice.Dialect
{
    /// <summary>
    /// 言葉・方言の種類です。
    /// </summary>
    public enum DialectType
    {
        /// <summary>
        /// 変換無し
        /// </summary>
        [Dialect("無変換", "無")]
        None = 0,
        /// <summary>
        /// 武士語
        /// </summary>
        [Dialect("武士語", "武士")]
        Samurai = 42,
        /// <summary>
        /// 龍馬語
        /// </summary>
        [Dialect("龍馬語", "龍馬")]
        Ryoma = 57,
        /// <summary>
        /// オネェ語
        /// </summary>
        [Dialect("オネェ語", "オネェ")]
        Onee = 56,
        /// <summary>
        /// マイケル語
        /// </summary>
        [Dialect("マイケル語", "マイケル")]
        Michael = 53,
        /// <summary>
        /// ジョジョ語風味
        /// </summary>
        [Dialect("ジョジョ語風味", "ジョジョ")]
        JoJo = 48,
        /// <summary>
        /// ノリピー語
        /// </summary>
        [Dialect("ノリピー語", "ノリピー")]
        Noripi = 54,
        /// <summary>
        /// 業界語
        /// </summary>
        [Dialect("業界語", "業界")]
        Trade = 45,
        /// <summary>
        /// 沖縄弁
        /// </summary>
        [Dialect("沖縄弁", "沖縄")]
        Okinawa = 20,
        /// <summary>
        /// 博多弁
        /// </summary>
        [Dialect("博多弁", "博多")]
        Hakata = 1,
        /// <summary>
        /// 宮崎弁
        /// </summary>
        [Dialect("宮崎弁", "宮崎")]
        Miyazaki = 38,
        /// <summary>
        /// 京都弁
        /// </summary>
        [Dialect("京都弁", "京都")]
        Kyoto = 19,
        /// <summary>
        /// 大阪弁
        /// </summary>
        [Dialect("大阪弁", "大阪")]
        Osaka = 2,
        /// <summary>
        /// 津軽弁
        /// </summary>
        [Dialect("津軽弁", "津軽")]
        Tsugaru = 18,
        /// <summary>
        /// 練馬ザ語
        /// </summary>
        [Dialect("練馬ザ語", "練馬")]
        Nerima = 10,
        /// <summary>
        /// 死語
        /// </summary>
        [Dialect("死語", "氏語")]
        Sigo = 11,
        /// <summary>
        /// 2ちゃん風味
        /// </summary>
        [Dialect("2ちゃん風味", "2ちゃん")]
        TwoChan = 28,
        /// <summary>
        /// ギャル語
        /// </summary>
        [Dialect("ギャル語", "ギャル")]
        Gyaru = 33,
        /// <summary>
        /// ナベアツ風味
        /// </summary>
        [Dialect("ナベアツ風味", "ナベアツ")]
        Nabeatu = 41,
        /// <summary>
        /// ヤンキー語
        /// </summary>
        [Dialect("ヤンキー語", "ヤンキー")]
        Yankee = 44,
        /// <summary>
        /// ルー語風味
        /// </summary>
        [Dialect("ルー語風味", "ルー")]
        Ruu = 39,
        /// <summary>
        /// よしお
        /// </summary>
        [Dialect("よしお", "よしお")]
        Yoshio = 4,
        /// <summary>
        /// ランダム
        /// </summary>
        [Dialect("ランダム", "ランダム")]
        Random = 7,
    }
}
