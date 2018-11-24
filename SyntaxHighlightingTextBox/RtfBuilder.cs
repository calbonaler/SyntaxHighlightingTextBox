using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Color = System.Windows.Media.Color;
using DColor = System.Drawing.Color;

namespace Controls
{
	/// <summary>RTF (Rich Text Format) 形式の文書を作成します。</summary>
	public class RtfBuilder
	{
		const string RtfHeader = @"{\rtf1\ansi\ansicpg932\deff0\deflang1033\deflangfe1041";
		const string DocumentLeading = @"\viewkind4\uc1\pard";

		Dictionary<Color, int> colors = new Dictionary<Color, int>();

		StringBuilder document = new StringBuilder();
		Color currentForeColor = DColor.Empty.ToMedia();
		Color currentBackColor = DColor.Empty.ToMedia();

		/// <summary>タブの停止位置を表すコレクションを取得します。停止位置はTwipで表されます。</summary>
		public Collection<int> TabStops { get; } = new Collection<int>();

		/// <summary>この文書のフォントを取得または設定します。</summary>
		public Font Font { get; set; }

		/// <summary>指定された文字列の指定された部分を指定された前景色と指定された背景色で書き込みます。</summary>
		/// <param name="text">書き込む対象の文字列を指定します。</param>
		/// <param name="startIndex">書き込みを開始する文字列内の位置を指定します。</param>
		/// <param name="count">書き込む文字数を指定します。</param>
		/// <param name="foreColor">文字列の前景色を指定します。</param>
		/// <param name="backColor">文字列の背景色を指定します。</param>
		public void AppendWithColor(string text, int startIndex, int count, Color foreColor, Color backColor)
		{
			if (text == null)
				throw new ArgumentNullException("text");
			if (count > 0)
			{
				if (currentForeColor != foreColor)
				{
					int i;
					if (foreColor == DColor.Empty.ToMedia())
						i = 0;
					else if (!colors.TryGetValue(foreColor, out i))
						colors.Add(foreColor, i = colors.Count + 1);
					document.AppendFormat(@"\cf{0} ", i);
					currentForeColor = foreColor;
				}
				if (currentBackColor != backColor)
				{
					int i;
					if (backColor == DColor.Empty.ToMedia())
						i = 0;
					else if (!colors.TryGetValue(backColor, out i))
						colors.Add(backColor, i = colors.Count + 1);
					document.AppendFormat(@"\highlight{0} ", i);
					currentBackColor = backColor;
				}
				for (int i = 0; i < count; i++)
					Append(text[i + startIndex]);
			}
		}

		/// <summary>指定された文字を書き込みます。</summary>
		/// <param name="ch">書き込む文字を指定します。</param>
		public void Append(char ch)
		{
			if (ch == '\\')
				document.Append(@"\\");
			else if (ch == '{')
				document.Append(@"\{");
			else if (ch == '}')
				document.Append(@"\}");
			else if (ch == '\n')
				document.Append("\\par\n");
			else
			{
				var bytes = Encoding.Default.GetBytes(new[] { ch });
				if (bytes.Length == 1 && bytes[0] <= 0x7F)
					document.Append(ch);
				else
				{
					foreach (var b in bytes)
						document.AppendFormat(@"\'{0:x2}", b);
				}
			}
		}

		/// <summary>生成された文書を表す <see cref="System.IO.Stream"/> を取得します。</summary>
		/// <returns>文書を表す <see cref="System.IO.Stream"/>。</returns>
		public Stream BuildStream()
		{
			Stream tmp = null;
			Stream result = null;
			try
			{
				tmp = new MemoryStream();
				using (var writer = new StreamWriter(tmp, Encoding.Default, 1024, true))
				{
					writer.Write(RtfHeader);
					if (Font != null)
					{
						writer.Write(@"{\fonttbl");
						writer.Write(@"{{\f{0}\fnil\fcharset128 {1};}}", 0, Font.Name);
						writer.Write("}");
					}
					writer.Write(@"{\colortbl ;");
					foreach (var color in colors.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key))
						writer.Write(@"\red{0}\green{1}\blue{2};", color.R, color.G, color.B);
					writer.Write("}");
					writer.Write(DocumentLeading);
					foreach (var tabStop in TabStops)
						writer.Write(@"\tx{0}", tabStop);
					if (Font != null)
						writer.Write(@"\f0\fs{0} ", (int)(Font.Size * 2));
					else
						writer.Write(" ");
					writer.Write(document.ToString());
				}
				tmp.Position = 0;
				result = tmp;
				tmp = null;
			}
			finally
			{
				if (tmp != null)
					tmp.Close();
			}
			return result;
		}

		/// <summary>書き込みを行った部分をすべてクリアします。フォントやタブの停止位置は変更されません。</summary>
		public void Clear()
		{
			currentForeColor = DColor.Empty.ToMedia();
			currentBackColor = DColor.Empty.ToMedia();
			colors.Clear();
			document.Remove(0, document.Length);
		}
	}
}
