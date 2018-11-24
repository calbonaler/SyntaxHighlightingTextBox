using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Color = System.Windows.Media.Color;
using DColor = System.Drawing.Color;

namespace Controls
{
	/// <summary>強調表示する文字列内の範囲を検索する方法を提供します。</summary>
	public interface IHighlightDescriptor
	{
		/// <summary>指定された文字列の中から強調表示する範囲を検索します。</summary>
		/// <param name="text">強調表示する範囲を検索する文字列を指定します。</param>
		/// <param name="startIndex">文字列内の範囲の検索を開始する位置を指定します。</param>
		/// <param name="rangeStartLimit">結果として得られる範囲の開始位置の排他的上限を指定します。</param>
		/// <returns>強調表示される文字列内の範囲を表す <see cref="HighlightToken"/>。</returns>
		HighlightToken GetToken(string text, int startIndex, int rangeStartLimit);
	}

	/// <summary>強調表示する文字列内の範囲を表します。</summary>
	public struct HighlightToken
	{
		/// <summary>指定された開始位置、長さ、前景色、背景色を使用して、<see cref="HighlightToken"/> クラスの新しいインスタンスを初期化します。</summary>
		/// <param name="first">範囲の開始位置を指定します。</param>
		/// <param name="length">範囲の長さを指定します。</param>
		/// <param name="foreColor">範囲を強調表示するときに使用する前景色を指定します。</param>
		/// <param name="backColor">範囲を強調表示するときに使用する背景色を指定します。</param>
		public HighlightToken(int first, int length, Color foreColor, Color backColor) : this()
		{
			First = first;
			Length = length;
			ForeColor = foreColor;
			BackColor = backColor;
		}

		/// <summary>指定された開始位置、長さ、前景色、背景色を使用して、<see cref="HighlightToken"/> クラスの新しいインスタンスを初期化します。</summary>
		/// <param name="first">範囲の開始位置を指定します。</param>
		/// <param name="length">範囲の長さを指定します。</param>
		/// <param name="foreColor">範囲を強調表示するときに使用する前景色を指定します。</param>
		/// <param name="backColor">範囲を強調表示するときに使用する背景色を指定します。</param>
		public HighlightToken(int first, int length, DColor foreColor, DColor backColor) : this(first, length, foreColor.ToMedia(), backColor.ToMedia()) { }

		/// <summary>文字列内のどの場所も表さない <see cref="HighlightToken"/> を取得します。</summary>
		public static readonly HighlightToken Empty = new HighlightToken();

		/// <summary>範囲の開始位置を取得します。</summary>
		public int First { get; private set; }

		/// <summary>範囲の長さを取得します。</summary>
		public int Length { get; private set; }

		/// <summary>範囲の表示に使用される前景色を取得します。</summary>
		public Color ForeColor { get; private set; }

		/// <summary>範囲の表示に使用される背景色を取得します。</summary>
		public Color BackColor { get; private set; }
	}

	/// <summary>2つの文字列によって囲まれた範囲を強調表示します。</summary>
	public class RangeHighlightDescriptor : IHighlightDescriptor
	{
		/// <summary><see cref="RangeHighlightDescriptor"/> の新しいインスタンスを初期化します。</summary>
		public RangeHighlightDescriptor() { }

		/// <summary>指定された前景色、背景色、開始文字列、終了文字列を使用して、<see cref="RangeHighlightDescriptor"/> クラスの新しいインスタンスを初期化します。</summary>
		/// <param name="foreColor">強調表示に使用する前景色を指定します。</param>
		/// <param name="backColor">強調表示に使用する背景色を指定します。</param>
		/// <param name="openString">範囲の開始を表す文字列を指定します。</param>
		/// <param name="closeString">範囲の終了を表す文字列を指定します。</param>
		public RangeHighlightDescriptor(DColor foreColor, DColor backColor, string openString, string closeString)
		{
			ForeColor = foreColor.ToMedia();
			BackColor = backColor.ToMedia();
			OpenString = openString;
			CloseString = closeString;
		}

		/// <summary>強調表示に使用する前景色を取得または設定します。</summary>
		public Color ForeColor { get; set; }
		/// <summary>強調表示に使用する背景色を取得または設定します。</summary>
		public Color BackColor { get; set; }
		/// <summary>強調表示する範囲の開始を表す文字列を取得または設定します。</summary>
		public string OpenString { get; set; }
		/// <summary>強調表示する範囲の終了を表す文字列を取得または設定します。</summary>
		public string CloseString { get; set; }

		/// <summary>指定された文字列の中から強調表示する範囲を検索します。</summary>
		/// <param name="text">強調表示する範囲を検索する文字列を指定します。</param>
		/// <param name="startIndex">文字列内の範囲の検索を開始する位置を指定します。</param>
		/// <param name="rangeStartLimit">結果として得られる範囲の開始位置の排他的上限を指定します。</param>
		/// <returns>強調表示される文字列内の範囲を表す <see cref="HighlightToken"/>。</returns>
		public virtual HighlightToken GetToken(string text, int startIndex, int rangeStartLimit)
		{
			var open = text.IndexOf(OpenString, startIndex, StringComparison.Ordinal);
			if (open >= 0 && open < rangeStartLimit)
			{
				var close = text.IndexOf(CloseString, open + OpenString.Length, StringComparison.Ordinal);
				if (close >= 0)
					return new HighlightToken(open, close + CloseString.Length - open, ForeColor, BackColor);
				else
					return new HighlightToken(open, text.Length - open, ForeColor, BackColor);
			}
			return HighlightToken.Empty;
		}
	}

	/// <summary>正規表現によって指定された範囲を強調表示します。</summary>
	public class RegexHighlightDescriptor : IHighlightDescriptor
	{
		/// <summary><see cref="RegexHighlightDescriptor"/> クラスの新しいインスタンスを初期化します。</summary>
		public RegexHighlightDescriptor() { }

		/// <summary>指定された前景色、背景色、正規表現パターンを使用して、<see cref="RegexHighlightDescriptor"/> クラスの新しいインスタンスを初期化します。</summary>
		/// <param name="foreColor">強調表示に使用する前景色を指定します。</param>
		/// <param name="backColor">強調表示に使用する背景色を指定します。</param>
		/// <param name="pattern">強調表示する範囲を示す正規表現パターンを指定します。</param>
		public RegexHighlightDescriptor(DColor foreColor, DColor backColor, string pattern)
		{
			ForeColor = foreColor.ToMedia();
			BackColor = backColor.ToMedia();
			Pattern = pattern;
		}
		
		/// <summary>強調表示に使用する前景色を取得または設定します。</summary>
		public Color ForeColor { get; set; }
		/// <summary>強調表示に使用する背景色を取得または設定します。</summary>
		public Color BackColor { get; set; }
		/// <summary>強調表示する範囲を示す正規表現パターンを取得または設定します。</summary>
		public string Pattern { get; set; }

		/// <summary>指定された文字列の中から強調表示する範囲を検索します。</summary>
		/// <param name="text">強調表示する範囲を検索する文字列を指定します。</param>
		/// <param name="startIndex">文字列内の範囲の検索を開始する位置を指定します。</param>
		/// <param name="rangeStartLimit">結果として得られる範囲の開始位置の排他的上限を指定します。</param>
		/// <returns>強調表示される文字列内の範囲を表す <see cref="HighlightToken"/>。</returns>
		public virtual HighlightToken GetToken(string text, int startIndex, int rangeStartLimit)
		{
			var match = new Regex(Pattern, RegexOptions.ExplicitCapture).Match(text, startIndex);
			if (match.Success && match.Index < rangeStartLimit)
				return new HighlightToken(match.Index, match.Length, ForeColor, BackColor);
			return HighlightToken.Empty;
		}
	}

	/// <summary>指定された単語を強調表示します。</summary>
	public class WordHighlightDescriptor : IHighlightDescriptor
	{
		/// <summary><see cref="WordHighlightDescriptor"/> クラスの新しいインスタンスを初期化します。</summary>
		public WordHighlightDescriptor() { }

		/// <summary>指定された前景色、背景色、単語を使用して、<see cref="WordHighlightDescriptor"/> クラスの新しいインスタンスを初期化します。</summary>
		/// <param name="foreColor">強調表示に使用する前景色を指定します。</param>
		/// <param name="backColor">強調表示に使用する背景色を指定します。</param>
		/// <param name="caseSensitive">単語の一致で大文字と小文字を区別するかどうかを示す値を指定します。</param>
		/// <param name="words">強調表示する単語を指定します。複数指定できます。</param>
		public WordHighlightDescriptor(DColor foreColor, DColor backColor, bool caseSensitive, params string[] words)
		{
			ForeColor = foreColor.ToMedia();
			BackColor = backColor.ToMedia();
			Words.AddRange(words);
			IsCaseSensitive = caseSensitive;
		}

		/// <summary>強調表示に使用する前景色を取得または設定します。</summary>
		public Color ForeColor { get; set; }
		/// <summary>強調表示に使用する背景色を取得または設定します。</summary>
		public Color BackColor { get; set; }
		/// <summary>強調表示する単語のリストを取得または設定します。</summary>
		[TypeConverter(typeof(MultiWordTypeConverter))]
		public System.Collections.Specialized.StringCollection Words { get; set; } = new System.Collections.Specialized.StringCollection();
		/// <summary>単語の一致で大文字と小文字を区別するかどうかを示す値を取得または設定します。</summary>
		public bool IsCaseSensitive { get; set; }

		/// <summary>指定された文字列の中から強調表示する範囲を検索します。</summary>
		/// <param name="text">強調表示する範囲を検索する文字列を指定します。</param>
		/// <param name="startIndex">文字列内の範囲の検索を開始する位置を指定します。</param>
		/// <param name="rangeStartLimit">結果として得られる範囲の開始位置の排他的上限を指定します。</param>
		/// <returns>強調表示される文字列内の範囲を表す <see cref="HighlightToken"/>。</returns>
		public virtual HighlightToken GetToken(string text, int startIndex, int rangeStartLimit)
		{
			StringBuilder sb = new StringBuilder();
			var words = new HashSet<string>(Words.Cast<string>(), IsCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);
			for (int i = startIndex; i < rangeStartLimit; i++)
			{
				if (char.IsLetter(text, i) || text[i] == '_')
				{
					int baseIndex = i;
					do
					{
						sb.Append(text[i++]);
					} while (i < text.Length && (char.IsLetterOrDigit(text, i) || text[i] == '_'));
					if (Words.Contains(sb.ToString()))
						return new HighlightToken(baseIndex, sb.Length, ForeColor, BackColor);
					sb.Remove(0, sb.Length);
				}
			}
			return HighlightToken.Empty;
		}

		class MultiWordTypeConverter : TypeConverter
		{
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
			{
				if (sourceType == typeof(string))
					return true;
				return base.CanConvertFrom(context, sourceType);
			}

			public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
			{
				if (destinationType == typeof(string))
					return true;
				return base.CanConvertTo(context, destinationType);
			}

			public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
			{
				if (value is string text)
				{
					var collection = new System.Collections.Specialized.StringCollection();
					foreach (Match match in Regex.Matches(text, @"[\S-[,]]+", RegexOptions.Singleline))
					{
						if (match.Success)
							collection.Add(match.Value);
					}
					return collection;
				}
				return base.ConvertFrom(context, culture, value);
			}

			public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
			{
				if (value is System.Collections.Specialized.StringCollection collection && CanConvertTo(context, destinationType))
					return string.Join(", ", collection.Cast<string>().ToArray());
				return base.ConvertTo(context, culture, value, destinationType);
			}
		}
	}

	/// <summary>指定された文字によって囲まれたエスケープ可能な範囲を強調表示します。</summary>
	public class EscapedRangeHighlightDescriptor : IHighlightDescriptor
	{
		/// <summary><see cref="EscapedRangeHighlightDescriptor"/> クラスの新しいインスタンスを初期化します。</summary>
		public EscapedRangeHighlightDescriptor() { }

		/// <summary>指定された前景色、背景色、境界文字、エスケープ文字を使用して、<see cref="EscapedRangeHighlightDescriptor"/> クラスの新しいインスタンスを初期化します。</summary>
		/// <param name="foreColor">強調表示に使用する前景色を指定します。</param>
		/// <param name="backColor">強調表示に使用する背景色を指定します。</param>
		/// <param name="boundary">範囲の開始と終了を表す文字を指定します。</param>
		/// <param name="escape">エスケープのために使用する文字を指定します。</param>
		/// <param name="multiline">範囲が複数行に及ぶかどうかを示す値を指定します。</param>
		public EscapedRangeHighlightDescriptor(DColor foreColor, DColor backColor, char boundary, char escape, bool multiline)
		{
			ForeColor = foreColor.ToMedia();
			BackColor = backColor.ToMedia();
			BoundaryCharacter = boundary;
			EscapeCharacter = escape;
			IsMultiline = multiline;
		}

		/// <summary>強調表示に使用する前景色を取得または設定します。</summary>
		public Color ForeColor { get; set; }
		/// <summary>強調表示に使用する背景色を取得または設定します。</summary>
		public Color BackColor { get; set; }
		/// <summary>強調表示する範囲の開始と終了を表す文字を取得または設定します。</summary>
		public char BoundaryCharacter { get; set; }
		/// <summary>エスケープのために使用する文字を取得または設定します。</summary>
		public char EscapeCharacter { get; set; }
		/// <summary>範囲が複数行に及ぶかどうかを示す値を取得または設定します。</summary>
		public bool IsMultiline { get; set; }

		/// <summary>指定された文字列の中から強調表示する範囲を検索します。</summary>
		/// <param name="text">強調表示する範囲を検索する文字列を指定します。</param>
		/// <param name="startIndex">文字列内の範囲の検索を開始する位置を指定します。</param>
		/// <param name="rangeStartLimit">結果として得られる範囲の開始位置の排他的上限を指定します。</param>
		/// <returns>強調表示される文字列内の範囲を表す <see cref="HighlightToken"/>。</returns>
		public virtual HighlightToken GetToken(string text, int startIndex, int rangeStartLimit)
		{
			var foundIndex = text.IndexOf(BoundaryCharacter, startIndex);
			for (int i = foundIndex + 1; foundIndex >= 0 && foundIndex < rangeStartLimit; i++)
			{
				if (i >= text.Length - 1 || text[i] == BoundaryCharacter || !IsMultiline && text[i] == '\n')
				{
					if (i >= text.Length)
						i = text.Length - 1;
					return new HighlightToken(foundIndex, i - foundIndex + 1, ForeColor, BackColor);
				}
				else if (text[i] == EscapeCharacter)
					i++;
			}
			return HighlightToken.Empty;
		}
	}
}
