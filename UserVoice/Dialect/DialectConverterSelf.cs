using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using System.Text;

namespace UserVoice.Dialect
{
    /// <summary>
    /// 文字列を方言に変換します。
    /// </summary>
    public sealed class DialectConverterSelf : IDialectConverter
    {
        private static List<string[]> osaka = new List<string[]>();
        private string convertedText;

        /// <summary>
        /// 大阪弁用のテーブルを初期化します。
        /// </summary>
        private static void InitOsaka()
        {
            /* 変換前の単語, 変化後の単語 */
            osaka.Add(new[] { "こんばんわ", "おこんばんわ" });
            osaka.Add(new[] { "ありがとうございました", "おおきに" });
            osaka.Add(new[] { "本当", "ほんま" });
            osaka.Add(new[] { "あなた", "あんさん" });
            osaka.Add(new[] { "あんな", "あないな" });
            osaka.Add(new[] { "りますので", "るさかいに" });
            osaka.Add(new[] { "りますから", "るさかいに" });
            osaka.Add(new[] { "あります", "あるんや" });
            osaka.Add(new[] { "あるいは", "せやなかったら" });
            osaka.Add(new[] { "或いは", "せやなかったら" });
            osaka.Add(new[] { "ありません", "おまへん" });
            osaka.Add(new[] { "ありました", "おました" });
            osaka.Add(new[] { "いない", "おらへん" });
            osaka.Add(new[] { "いままでの", "ムカシからの" });
            osaka.Add(new[] { "いままで", "本日この時まで" });
            osaka.Add(new[] { "今まで", "本日この時まで" });
            osaka.Add(new[] { "今までの", "ムカシからの" });
            osaka.Add(new[] { "いまどき", "きょうび" });
            osaka.Add(new[] { "いわゆる なんちうか，", "ようみなはんいわはるとこの" });
            osaka.Add(new[] { "思いますが", "思うんやが" });
            osaka.Add(new[] { "思います", "思うで" });
            osaka.Add(new[] { "いただいた", "もろた" });
            osaka.Add(new[] { "いただきます", "もらうで" });
            osaka.Add(new[] { "いただきました", "もろた" });
            osaka.Add(new[] { "いくら", "なんぼ" });
            osaka.Add(new[] { "いつも", "毎日毎晩壱年中" });
            osaka.Add(new[] { "いるか", "おるか" });
            osaka.Add(new[] { "いますので", "おるさかいに" });
            osaka.Add(new[] { "いますから", "おるさかいに" });
            osaka.Add(new[] { "いちど", "いっぺん" });
            osaka.Add(new[] { "一度", "いっぺん" });
            osaka.Add(new[] { "いますが", "おるけどダンさん" });
            osaka.Add(new[] { "いました", "おったんや" });
            osaka.Add(new[] { "います", "い ますわ" });
            osaka.Add(new[] { "エラー", "アヤマチ" });
            osaka.Add(new[] { "えない", "えへん" });
            osaka.Add(new[] { "おかしな", "ケッタイな" });
            osaka.Add(new[] { "おきました", "おいたんや" });
            osaka.Add(new[] { "おっと", "おっとドッコイたこやきはうまいで...あかん,脱線や" });
            osaka.Add(new[] { "かなあ", "かいな" });
            osaka.Add(new[] { "かならず", "じぇったい" });
            osaka.Add(new[] { "かわいい", "メンコイ" });
            osaka.Add(new[] { "おそらく", "ワイが思うには" });
            osaka.Add(new[] { "恐らく", "ワイが思うには" });
            osaka.Add(new[] { "おもしろい", "オモロイ" });
            osaka.Add(new[] { "面白い", "おもろい" });
            osaka.Add(new[] { "ください", "おくんなはれ" });
            osaka.Add(new[] { "詳しく", "ねちっこく" });
            osaka.Add(new[] { "くわしく", "ねちっこく" });
            osaka.Add(new[] { "けない", "けへん" });
            osaka.Add(new[] { "ございます", "おます" });
            osaka.Add(new[] { "ございました", "おました" });
            osaka.Add(new[] { "こちら", "ウチ" });
            osaka.Add(new[] { "僕", "ワテ" });
            osaka.Add(new[] { "俺", "わて" });
            osaka.Add(new[] { "こんな", "こないな" });
            osaka.Add(new[] { "この頃", "きょうび" });
            osaka.Add(new[] { "こども", "ガキ" });
            osaka.Add(new[] { "このごろ", "きょうび" });
            osaka.Add(new[] { "コロン", "てんてん" });
            osaka.Add(new[] { "下さい", "くれへんかの" });
            osaka.Add(new[] { "さようなら", "ほなさいなら" });
            osaka.Add(new[] { "さん", "はん" });
            osaka.Add(new[] { "しかし", "せやけどダンさん" });
            osaka.Add(new[] { "おはよう", "おはようさん" });
            osaka.Add(new[] { "しかたない", "しゃあない" });
            osaka.Add(new[] { "仕方ない", "しゃあない" });
            osaka.Add(new[] { "しなければ", "せな" });
            osaka.Add(new[] { "しない", "せん" });
            osaka.Add(new[] { "しばらく", "ちーとの間" });
            osaka.Add(new[] { "している", "しとる" });
            osaka.Add(new[] { "しました", "したんや" });
            osaka.Add(new[] { "しまいました", "しもたんや" });
            osaka.Add(new[] { "しますか", "しまっか" });
            osaka.Add(new[] { "しますと", "すやろ、ほしたら" });
            osaka.Add(new[] { "しまった", "しもた" });
            osaka.Add(new[] { "しますので", "するさかいに" });
            osaka.Add(new[] { "じゃ", "や" });
            osaka.Add(new[] { "するとき", "するっちうとき" });
            osaka.Add(new[] { "すべて", "ずぅぇえええぇぇええんぶ" });
            osaka.Add(new[] { "すくなくとも", "なんぼなんでも" });
            osaka.Add(new[] { "少なくとも", "なんぼなんでも" });
            osaka.Add(new[] { "ずに", "んと" });
            osaka.Add(new[] { "すごい", "どエライ" });
            osaka.Add(new[] { "少し", "ちびっと" });
            osaka.Add(new[] { "スリッパ", "パッスリ" });
            osaka.Add(new[] { "せない", "せへん" });
            osaka.Add(new[] { "そこで", "ほんで" });
            osaka.Add(new[] { "そして", "ほんで" });
            osaka.Add(new[] { "そんな", "そないな" });
            osaka.Add(new[] { "そうだろ", "そうやろ" });
            osaka.Add(new[] { "それから", "ほんで" });
            osaka.Add(new[] { "それでは", "ほなら" });
            osaka.Add(new[] { "たとえば", "例あげたろか，たとえばやなあ" });
            osaka.Add(new[] { "例えば", "例あげたろか，たとえばやなあ" });
            osaka.Add(new[] { "たのです", "たちうワケや" });
            osaka.Add(new[] { "たので", "たさかい" });
            osaka.Add(new[] { "ただし", "せやけど" });
            osaka.Add(new[] { "たぶん", "タブン...たぶんやで，わいもよーしらんがタブン" });
            osaka.Add(new[] { "たくさん", "ようけ" });
            osaka.Add(new[] { "だった", "やった" });
            osaka.Add(new[] { "だけど", "やけど" });
            osaka.Add(new[] { "だから", "やから" });
            osaka.Add(new[] { "だが", "やけど" });
            osaka.Add(new[] { "だろ", "やろ" });
            osaka.Add(new[] { "だね。", "やね。" });
            osaka.Add(new[] { "ちなみに", "余計なお世話やけど" });
            osaka.Add(new[] { "ちょっと", "ちーとばかし" });
            osaka.Add(new[] { "ったし", "ったことやねんし" });
            osaka.Add(new[] { "つまり", " ゴチャゴチャゆうとる場合やあれへん，要は" });
            osaka.Add(new[] { "つまらない", "しょーもない" });
            osaka.Add(new[] { "であった", "やった" });
            osaka.Add(new[] { "ている", "とる" });
            osaka.Add(new[] { "ていただいた", "てもろた" });
            osaka.Add(new[] { "ていただきます", "てもらうで" });
            osaka.Add(new[] { "ていただく", "てもらうで" });
            osaka.Add(new[] { "ていただ", "ていただ" });
            osaka.Add(new[] { "ていた", "とった" });
            osaka.Add(new[] { "多く", "ようけ" });
            osaka.Add(new[] { "ですか", "やろか" });
            osaka.Add(new[] { "ですよ", "や" });
            osaka.Add(new[] { "ですが", "やけどアンタ" });
            osaka.Add(new[] { "ですね", "やね" });
            osaka.Add(new[] { "でした", "やった" });
            osaka.Add(new[] { "でしょう", "でっしゃろ" });
            osaka.Add(new[] { "できない", "でけへん" });
            osaka.Add(new[] { "ではない", "ではおまへん" });
            osaka.Add(new[] { "です", "や" });
            osaka.Add(new[] { "てない", "てへん" });
            osaka.Add(new[] { "どういうわけか", "なんでやろかわいもよーしらんが" });
            osaka.Add(new[] { "どうだ", "どや" });
            osaka.Add(new[] { "どこか", "どこぞ" });
            osaka.Add(new[] { "どんな", "どないな" });
            osaka.Add(new[] { "という", "ちう" });
            osaka.Add(new[] { "とすれば", "とするやろ、ほしたら" });
            osaka.Add(new[] { "ところが", "トコロが" });
            osaka.Add(new[] { "ところ", "トコ" });
            osaka.Add(new[] { "とても", "どエライ" });
            osaka.Add(new[] { "なぜか", "なんでやろかわいもよーしらんが" });
            osaka.Add(new[] { "なった", "なりよった" });
            osaka.Add(new[] { "なのですが", "なんやけど" });
            osaka.Add(new[] { "なのです", "なんやこれがホンマに" });
            osaka.Add(new[] { "なので", "やので" });
            osaka.Add(new[] { "なぜ", "なんでやねん" });
            osaka.Add(new[] { "など", "やらなんやら" });
            osaka.Add(new[] { "ならない", "ならへん" });
            osaka.Add(new[] { "なりました", "なったんや" });
            osaka.Add(new[] { "のちほど", "ノチカタ" });
            osaka.Add(new[] { "のです", "のや" });
            osaka.Add(new[] { "はじめまして", "はじめてお目にかかりまんなあ" });
            osaka.Add(new[] { "はじめて", "この世におぎゃあいうて生まれてはじめて" });
            osaka.Add(new[] { "びっくり仰天", "クリビツテンギョー" });
            osaka.Add(new[] { "ひとたち", "ヤカラ" });
            osaka.Add(new[] { "人たち", "ヤカラ" });
            osaka.Add(new[] { "人達", "ヤカラ" });
            osaka.Add(new[] { "ヘルプ", "助け船" });
            osaka.Add(new[] { "ほんとう", "ホンマ" });
            osaka.Add(new[] { "ほんと", "ホンマ" });
            osaka.Add(new[] { "まいますので", "まうさかいに" });
            osaka.Add(new[] { "まったく", "まるっきし" });
            osaka.Add(new[] { "全く", "まるっきし" });
            osaka.Add(new[] { "ません", "まへん" });
            osaka.Add(new[] { "ました", "たんや" });
            osaka.Add(new[] { "ますか", "まっしゃろか" });
            osaka.Add(new[] { "ますが", "まっけど" });
            osaka.Add(new[] { "ましょう", "まひょ" });
            osaka.Add(new[] { "ますので", "よるさかいに" });
            osaka.Add(new[] { "むずかしい", "ややこしい" });
            osaka.Add(new[] { "めない", "めへん" });
            osaka.Add(new[] { "メッセージ", "文句" });
            osaka.Add(new[] { "もらった", "もろた" });
            osaka.Add(new[] { "もらって", "もろて" });
            osaka.Add(new[] { "よろしく", "シブロクヨンキュー" });
            osaka.Add(new[] { "ります", "るんや" });
            osaka.Add(new[] { "らない", "りまへん" });
            osaka.Add(new[] { "りない", "りまへん" });
            osaka.Add(new[] { "れない", "れへん" });
            osaka.Add(new[] { "ます", "まんねん" });
            osaka.Add(new[] { "もっとも", "もっとも" });
            osaka.Add(new[] { "もっと", "もっともっともっと" });
            osaka.Add(new[] { "ようやく", "ようやっと" });
            osaka.Add(new[] { "よろしく", "よろしゅう" });
            osaka.Add(new[] { "るのです", "るちうワケや" });
            osaka.Add(new[] { "だ。", "や。" });
            osaka.Add(new[] { "りました", "ったんや" });
            osaka.Add(new[] { "る。", "るちうわけや。" });
            osaka.Add(new[] { "い。", "いちうわけや。" });
            osaka.Add(new[] { "た。", "たちうわけや。" });
            osaka.Add(new[] { "う。", "うわ。" });
            osaka.Add(new[] { "わがまま", "ワガママ" });
            osaka.Add(new[] { "まま", "まんま" });
            osaka.Add(new[] { "われわれ", "ウチら" });
            osaka.Add(new[] { "わたし", "わい" });
            osaka.Add(new[] { "私", "ウチ" });
            osaka.Add(new[] { "アタシ", "ウチ" });
            osaka.Add(new[] { "わない", "いまへん" });
            osaka.Add(new[] { "本当", "ホンマ" });
            osaka.Add(new[] { "全て", "みな" });
            osaka.Add(new[] { "全部", "ぜええんぶひとつのこらず" });
            osaka.Add(new[] { "全然", "さらさら" });
            osaka.Add(new[] { "ぜんぜん", "サラサラ" });

            osaka.Add(new[] { "日本語", "祖国語" });
            osaka.Add(new[] { "日本", "大日本帝国" });
            osaka.Add(new[] { "便利", "便器...おっとちゃうわ，便利" });
            osaka.Add(new[] { "当局", "わい" });
            osaka.Add(new[] { "大変な", "エライ" });
            osaka.Add(new[] { "大変", "エライ" });
            osaka.Add(new[] { "非常に", "どエライ" });
            osaka.Add(new[] { "違う", "ちゃう" });
            osaka.Add(new[] { "ANK", "アンコ.......ウソやウソ,ANKやわ,はっはっ," });
            osaka.Add(new[] { "古い", "古くさい" });
            osaka.Add(new[] { "最近", "きょうび" });
            osaka.Add(new[] { "以前", "よりどエライ昔" });
            osaka.Add(new[] { "無効", "チャラ" });
            osaka.Add(new[] { "中止", "ヤメ" });
            osaka.Add(new[] { "外国", "異国" });
            osaka.Add(new[] { "海外", "アチラ" });
            osaka.Add(new[] { "難しい", "ややこしい" });
            osaka.Add(new[] { "面倒", "難儀" });
            osaka.Add(new[] { "遅い", "とろい" });
            osaka.Add(new[] { "良い", "ええ" });
            osaka.Add(new[] { "入れる", "ぶちこむ" });
            osaka.Add(new[] { "コギャル", "セーラー服のねえちゃん" });
            osaka.Add(new[] { "女子高生", "セーラー服のねえちゃん" });
            osaka.Add(new[] { "来た", "攻めて来よった" });
            osaka.Add(new[] { "同時", "いっぺん" });
            osaka.Add(new[] { "先頭", "アタマ" });
            osaka.Add(new[] { "破壊", "カンペキに破壊" });
            osaka.Add(new[] { "挿入", "ソーニュー(うひひひ...おっとカンニンや)" });
            osaka.Add(new[] { "置換", "とっかえ" });
            osaka.Add(new[] { "無視", "シカト" });
            osaka.Add(new[] { "注意", "用心" });
            osaka.Add(new[] { "最後", "ケツ" });
            osaka.Add(new[] { "我々", "うちら" });
            osaka.Add(new[] { "初心者", "どシロウト" });
            osaka.Add(new[] { "付属", "オマケ" });
            osaka.Add(new[] { "誤って", "あかーんいうて誤って" });
            osaka.Add(new[] { "商人", "あきんど" });
            osaka.Add(new[] { "商売", "ショーバイ" });
            osaka.Add(new[] { "商業", "ショーバイ" });
            osaka.Add(new[] { "誰", "どなたはん" });
            osaka.Add(new[] { "再度", "もっかい" });
            osaka.Add(new[] { "再び", "もっかい" });
            osaka.Add(new[] { "自動的に", "なあんもせんとホッタラかしといても" });
            osaka.Add(new[] { "無料", "タダ" });
            osaka.Add(new[] { "変化", "変身" });
            //osaka.Add(new[] { "右", "右翼" });
            //osaka.Add(new[] { "左", "左翼" });
            osaka.Add(new[] { "自分", "オノレ" });
            osaka.Add(new[] { "とても", "ごっつ" });
            osaka.Add(new[] { "成功", "性交...ひひひ,ウソや,成功" });
            osaka.Add(new[] { "失敗", "シッパイ" });
            osaka.Add(new[] { "優先", "ヒイキ" });
            osaka.Add(new[] { "タクシー", "タク" });
            osaka.Add(new[] { "カレンダー", "日メクリ" });
            osaka.Add(new[] { "たばこ", "モク" });
            osaka.Add(new[] { "特長", "ええトコ" });
            osaka.Add(new[] { "概要", "おーまかなトコ" });
            osaka.Add(new[] { "概念", "能書き" });
            osaka.Add(new[] { "アルゴリズム", "理屈" });
            osaka.Add(new[] { "実用的", "アホでも使えるよう" });
            osaka.Add(new[] { "何も", "なあんも" });
            osaka.Add(new[] { "何か", "何ぞ" });
            osaka.Add(new[] { "子供", "ボウズ" });
            osaka.Add(new[] { "いい", "ええ" });
            osaka.Add(new[] { "マクドナルド", "マクド" });
            osaka.Add(new[] { "なのかな", "やろか" });
            osaka.Add(new[] { "かな", "やろか" });
            osaka.Add(new[] { "こんにちは", "もうかってまっか？" });
            osaka.Add(new[] { "どうも", "もうかってまっか？" });
            osaka.Add(new[] { "クライアント", "客" });
            osaka.Add(new[] { "素人", "トーシロ" });
        }

        /// <summary>
        /// 文字列を大阪弁に変換します。
        /// </summary>
        private string ConvertDialect(string text, List<string[]> convertTable)
        {
            if (string.IsNullOrEmpty(text))
            {
                return "";
            }

            for (var i = 0; i < convertTable.Count; i++)
            {
                // text = text.Replace(convertTable[i][0], convertTable[i][1]);
                // ↑無限変換地獄にはまる可能性がある。
                var at = 0;

                while (true)
                {
                    // 置き換え対象となる文字列があるか調べます。
                    var strpos = text.IndexOf(convertTable[i][0], at);
                    if (strpos < 0)
                    {
                        break;
                    }

                    // 置き換えます。
                    var pre = text.Substring(0, strpos);
                    var post = text.Substring(strpos + convertTable[i][0].Length);

                    text = pre + convertTable[i][1] + post;
                    at = strpos + convertTable[i][1].Length;
                }
            }

            return text;
        }

        /// <summary>
        /// 方言に変換します。
        /// </summary>
        private string ConvertDialect(string text, DialectType dialect)
        {
            switch (dialect)
            {
                case DialectType.None:
                    return text;
                case DialectType.Osaka:
                    return ConvertDialect(text, osaka);
            }

            return text;
        }

        /// <summary>
        /// 言葉・方言変換を自分のPC上で行います。
        /// </summary>
        public void BeginConvertText(string text, DialectType defaultDialect,
                                     TimeSpan processTime)
        {
            if (string.IsNullOrEmpty(text))
            {
                this.convertedText = text;
                return;
            }

            // 文字列をタグと通常文字に分解します。
            List<TextTagInfo> tagInfoList = DialectUtil.SplitText(text);
            DialectType dialect = defaultDialect;

            // 各文字列を方言変換します。
            foreach (TextTagInfo info in tagInfoList)
            {
                if (info.TagType == TextTag.Dialect)
                {
                    // 変換する方言を変更します。
                    dialect = info.DialectType;
                }
                else if (info.TagType == TextTag.Unknown)
                {
                    // 空白のみの場合は変換作業を行いません。
                    if (!DialectUtil.IsStringWhitespace(info.Text))
                    {
                        info.ConvertedText = ConvertDialect(info.Text, dialect);
                    }
                }
            }

            // 変換後のテキストを保存します。
            this.convertedText = DialectUtil.JoinAll(tagInfoList);
        }

        /// <summary>
        /// 変換後の文字列を取得します。
        /// </summary>
        public string GetConvertedText()
        {
            return this.convertedText;
        }

        /// <summary>
        /// 変換可能な言葉・方言の一覧を取得します。
        /// </summary>
        public static List<DialectNamePair> ConvertDialectList
        {
            get;
            private set;
        }

        /// <summary>
        /// 自前で変換するときの方言リストを初期化します。
        /// </summary>
        static DialectConverterSelf()
        {
            ConvertDialectList = new List<DialectNamePair>();

            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.None));
            ConvertDialectList.Add(
                DialectUtil.MakeDialectNamePair(DialectType.Osaka));

            InitOsaka();
        }
    }
}
