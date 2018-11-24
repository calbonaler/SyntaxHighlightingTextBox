using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Markup;

namespace Controls
{
	/// <summary>文字列内の強調表示するすべての範囲を検索する方法を提供します。</summary>
	public interface IHighlightTokenizer
	{
		/// <summary>指定された文字列の中で強調表示するすべての範囲を取得します。</summary>
		/// <param name="text">強調表示する範囲を含む文字列を指定します。</param>
		/// <returns>強調表示される文字列内の複数の範囲を表す <see cref="HighlightToken"/> のシーケンス。</returns>
		IEnumerable<HighlightToken> GetTokens(string text);
	}

	/// <summary>複数の <see cref="IHighlightDescriptor"/> から得られる強調表示範囲を1つにまとめる方法を提供します。</summary>
	[ContentProperty("HighlightDescriptors")]
	public class CompositeHighlightTokenizer : IHighlightTokenizer
	{
		/// <summary><see cref="CompositeHighlightTokenizer"/> クラスの新しいインスタンスを初期化します。</summary>
		public CompositeHighlightTokenizer() { }

		/// <summary>強調表示するコンテンツを示す <see cref="IHighlightDescriptor"/> のコレクションを取得します。</summary>
		public Collection<IHighlightDescriptor> HighlightDescriptors { get; set; } = new Collection<IHighlightDescriptor>();

		/// <summary>指定された文字列の中で強調表示するすべての範囲を取得します。</summary>
		/// <param name="text">強調表示する範囲を含む文字列を指定します。</param>
		/// <returns>強調表示される文字列内の複数の範囲を表す <see cref="HighlightToken"/> のシーケンス。</returns>
		public virtual IEnumerable<HighlightToken> GetTokens(string text)
		{
			int startIndex = 0;
			while (true)
			{
				var minimum = HighlightToken.Empty;
				int rangeStartLimit = text.Length;
				foreach (var item in HighlightDescriptors)
				{
					var range = item.GetToken(text, startIndex, rangeStartLimit);
					if (range.Length > 0 && (minimum.Length == 0 || range.First < minimum.First))
					{
						minimum = range;
						if ((rangeStartLimit = range.First) <= startIndex)
							break;
					}
				}
				if (minimum.Length > 0)
				{
					startIndex = minimum.First + minimum.Length;
					yield return minimum;
				}
				else
					yield break;
			}
		}
	}
}
