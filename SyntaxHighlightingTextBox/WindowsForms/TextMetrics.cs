using System.Drawing;
using System.Runtime.InteropServices;

namespace Controls.WindowsForms
{
	/// <summary>フォントに関する情報を表します。</summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public class TextMetrics
	{
		int tmHeight;
		int tmAscent;
		int tmDescent;
		int tmInternalLeading;
		int tmExternalLeading;
		int tmAveCharWidth;
		int tmMaxCharWidth;
		int tmWeight;
		int tmOverhang;
		int tmDigitizedAspectX;
		int tmDigitizedAspectY;
		char tmFirstChar;
		char tmLastChar;
		char tmDefaultChar;
		char tmBreakChar;
		byte tmItalic;
		byte tmUnderlined;
		byte tmStruckOut;
		byte tmPitchAndFamily;
		byte tmCharSet;
		/// <summary>フォントの文字の高さ (アセント + ディセント) を取得します。</summary>
		public int Height => tmHeight;
		/// <summary>フォントの文字のアセント (ベースラインから文字の上部までの高さ) を取得します。</summary>
		public int Ascent => tmAscent;
		/// <summary>フォントの文字のディセント (文字の下部からベースラインまでの高さ) を取得します。</summary>
		public int Descent => tmDescent;
		/// <summary>フォントの内部レディングを取得します。内部レディングにはアクセント記号などが描画されることがあります。</summary>
		public int InternalLeading => tmInternalLeading;
		/// <summary>フォントの外部レディング (行間) を取得します。</summary>
		public int ExternalLeading => tmExternalLeading;
		/// <summary>フォントの平均文字幅 (一般的に英字の x の幅によって定義される値) を取得します。太字や斜体などのためのオーバーハングは含まれません。</summary>
		public int AverageCharacterWidth => tmAveCharWidth;
		/// <summary>フォントの最大文字幅を取得します。</summary>
		public int MaximumCharacterWidth => tmMaxCharWidth;
		/// <summary>フォントの太さを取得します。</summary>
		public int Weight => tmWeight;
		/// <summary>太字や斜体などの場合にフォントに付加する幅を取得します。</summary>
		public int Overhang => tmOverhang;
		/// <summary>フォントのデザイン対象であるデバイスの水平アスペクトを取得します。</summary>
		public int DigitizedAspectX => tmDigitizedAspectX;
		/// <summary>フォントのデザイン対象であるデバイスの垂直アスペクトを取得します。</summary>
		public int DigitizedAspectY => tmDigitizedAspectY;
		/// <summary>フォントで定義される最初の文字を取得します。</summary>
		public char FirstCharacter => tmFirstChar;
		/// <summary>フォントで定義される最後の文字を取得します。</summary>
		public char LastCharacter => tmLastChar;
		/// <summary>文字がフォントに存在しない場合に、代わりに使用する文字を取得します。</summary>
		public char FallbackCharacter => tmDefaultChar;
		/// <summary>テキストを両端揃えする場合に単語の分割の定義に使用する文字を取得します。</summary>
		public char BreakCharacter => tmBreakChar;
		/// <summary>フォントが斜体であるかどうかを示す値を取得します。</summary>
		public bool Italic => tmItalic != 0;
		/// <summary>フォントが下線つきフォントであるかどうかを示す値を取得します。</summary>
		public bool Underlined => tmUnderlined != 0;
		/// <summary>フォントが取り消し線つきフォントであるかどうかを示す値を取得します。</summary>
		public bool StruckOut => tmStruckOut != 0;
		/// <summary>フォントが可変ピッチであるかどうかを示す値を取得します。</summary>
		public bool IsVariablePitch => (tmPitchAndFamily & 1) != 0;
		/// <summary>フォントがベクトルフォントであるかどうかを示す値を取得します。</summary>
		public bool IsVector => (tmPitchAndFamily & 2) != 0;
		/// <summary>フォントが TrueType フォントであるかどうかを示す値を取得します。</summary>
		public bool IsTrueType => (tmPitchAndFamily & 4) != 0;
		/// <summary>フォントがデバイスフォントであるかどうかを示す値を取得します。</summary>
		public bool IsDevice => (tmPitchAndFamily & 8) != 0;
		/// <summary>フォントのフォントファミリーを取得します。</summary>
		public FontFamilyType FontFamily => (FontFamilyType)(tmPitchAndFamily & 0xF0);
		/// <summary>フォントの文字セットを取得します。</summary>
		public FontCharacterSet CharacterSet => (FontCharacterSet)tmCharSet;

		TextMetrics() { }

		/// <summary>指定されたフォントの情報を表す <see cref="TextMetrics"/> クラスのインスタンスを取得します。</summary>
		/// <param name="dc">フォントの情報を取得するために使用するデバイス コンテキストを指定します。</param>
		/// <param name="font">情報を取得するフォントを指定します。</param>
		/// <returns>フォントの情報を格納する <see cref="TextMetrics"/> クラスのインスタンス。</returns>
		public static TextMetrics FromFont(IDeviceContext dc, Font font)
		{
			try
			{
				var result = new TextMetrics();
				var handle = dc.GetHdc();
				var prevFont = NativeMethods.SelectObject(handle, font.ToHfont());
				NativeMethods.GetTextMetrics(handle, result);
				NativeMethods.SelectObject(handle, prevFont);
				return result;
			}
			finally { dc.ReleaseHdc(); }
		}
	}

	/// <summary>フォント ファミリーを表します。</summary>
	public enum FontFamilyType
	{
		/// <summary>既定のフォントを使用します。</summary>
		Default = 0x00,
		/// <summary>可変ストローク幅 (プロポーショナル) でセリフのあるフォントを表します。</summary>
		Serif = 0x10,
		/// <summary>可変ストローク幅 (プロポーショナル) でセリフのないフォントを表します。</summary>
		SansSerif = 0x20,
		/// <summary>固定ストローク幅 (等幅) でセリフはない場合もある場合もあるフォントを表します。等幅フォントは通常 <see cref="Modern"/> です。</summary>
		Modern = 0x30,
		/// <summary>手書きのようにデザインされたフォントを表します。</summary>
		Script = 0x40,
		/// <summary>派手に装飾されたフォントを表します。</summary>
		Decorative = 0x50,
	}

	/// <summary>フォントの文字セットを表します。</summary>
	public enum FontCharacterSet
	{
		/// <summary>英語用 Ansi 文字セットです。</summary>
		Ansi = 0,
		/// <summary>既定の文字セットです。</summary>
		Default = 1,
		/// <summary>シンボルを表す文字セットです。</summary>
		Symbol = 2,
		/// <summary>Macintosh用文字セットです。</summary>
		Mac = 77,
		/// <summary>日本語用 Shift_JIS 文字セットです。</summary>
		ShiftJis = 128,
		/// <summary>韓国語用文字セットです。</summary>
		Hangul = 129,
		/// <summary>韓国語用 Johab 文字セットです。</summary>
		Johab = 130,
		/// <summary>簡体字中国語用 GB2312 文字セットです。</summary>
		GB2312 = 134,
		/// <summary>繁体字中国語用 Big5 文字セットです。</summary>
		ChiniseBig5 = 136,
		/// <summary>ギリシャ語用文字セットです。</summary>
		Greek = 161,
		/// <summary>トルコ語用文字セットです。</summary>
		Turkish = 162,
		/// <summary>ベトナム語用文字セットです。</summary>
		Vietnamese = 163,
		/// <summary>ヘブライ語用文字セットです。</summary>
		Hebrew = 177,
		/// <summary>アラビア語用文字セットです。</summary>
		Arabic = 178,
		/// <summary>バルト語用文字セットです。</summary>
		Baltic = 186,
		/// <summary>ロシア語用文字セットです。</summary>
		Russian = 204,
		/// <summary>タイ語用文字セットです。</summary>
		Thai = 222,
		/// <summary>東ヨーロッパ諸国語用文字セットです。</summary>
		EastEurope = 238,
		/// <summary>OEM 文字セットです。</summary>
		Oem = 255,
	}
}
