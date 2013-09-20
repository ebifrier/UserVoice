using System;
using System.Collections.Generic;
using System.Text;

namespace UserVoice.Dialect
{
    class DialectTagList
    {
        private static readonly Dictionary<DialectType, string[]> dialectTagList =
           new Dictionary<DialectType, string[]>()
            {
                {
                    DialectType.None,
                    new string[]
                    {
                        "無", "なし", "む", "むへんかん", "MU", "NASI",
                    }
                },
                {
                    DialectType.Samurai,
                    new string[]
                    {
                        "ぶし", "BUSI", "BUSHI",
                    }
                },
            };
    }
}
