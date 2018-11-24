using System;
using System.Drawing;
using System.Windows.Forms;

namespace Controls.WindowsForms
{
	/// <summary>構文の強調表示が可能なテキストボックスを表します。</summary>
	public class SyntaxHighlightingTextBox : RichTextBox
	{
		/// <summary>このテキストボックス内の文字列を強調表示する方法を表す <see cref="IHighlightTokenizer"/> を取得または設定します。</summary>
		public IHighlightTokenizer HighlightTokenizer { get; set; }

		int averageCharacterWidth = 0;
		RtfBuilder rtfBuilder = new RtfBuilder();

		/// <summary>タブの停止位置を設定します。停止位置は平均文字幅の1/4単位で表されます。</summary>
		/// <param name="stops">設定する停止位置を指定します。</param>
		public void SetTabStops(params int[] stops)
		{
			if (stops == null)
				throw new ArgumentNullException(nameof(stops));
			if (stops.Length <= 0 || stops.Length > 32)
				throw new ArgumentOutOfRangeException(nameof(stops));
			rtfBuilder.TabStops.Clear();
			if (stops.Length == 1)
			{
				for (int i = 1; i <= 32; i++)
					rtfBuilder.TabStops.Add(averageCharacterWidth * stops[0] * i / 4);
			}
			else
			{
				for (int i = 0; i < stops.Length; i++)
					rtfBuilder.TabStops.Add(averageCharacterWidth * stops[i] / 4);
			}
			HighlightSyntax(false);
		}

		/// <summary>コントロール内にテキストを表示するときに使用するフォントを取得または設定します。</summary>
		public override Font Font
		{
			get { return base.Font; }
			set
			{
				if (base.Font != value)
				{
					base.Font = value;
					rtfBuilder.Font = value;
					using (var graphics = CreateGraphics())
						averageCharacterWidth = (int)(TextMetrics.FromFont(graphics, value).AverageCharacterWidth * 1440 / graphics.DpiX);
					HighlightSyntax(false);
				}
			}
		}

		/// <summary><see cref="System.Windows.Forms.Control.TextChanged"/> イベントを発生させます。</summary>
		/// <param name="e">イベント データを格納している <see cref="EventArgs"/>。</param>
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			HighlightSyntax(true);
		}

		void HighlightSyntax(bool parse)
		{
			NativeMethods.LockWindowUpdate(Handle);
			var position = NativeMethods.GetScrollPosition(Handle);
			var cursorLoc = SelectionStart;

			if (parse)
			{
				rtfBuilder.Clear();
				var text = Text;
				int startIndex = 0;
				if (HighlightTokenizer != null)
				{
					foreach (var item in HighlightTokenizer.GetTokens(text))
					{
						rtfBuilder.AppendWithColor(text, startIndex, item.First - startIndex, Color.Empty.ToMedia(), Color.Empty.ToMedia());
						rtfBuilder.AppendWithColor(text, item.First, item.Length, item.ForeColor, item.BackColor);
						startIndex = item.First + item.Length;
					}
				}
				rtfBuilder.AppendWithColor(text, startIndex, text.Length - startIndex, Color.Empty.ToMedia(), Color.Empty.ToMedia());
			}
			using (var stream = rtfBuilder.BuildStream())
				LoadFile(stream, RichTextBoxStreamType.RichText);

			// Restore cursor and scrollbars location.
			SelectionStart = cursorLoc;
			NativeMethods.SetScrollPosition(Handle, position);
			NativeMethods.LockWindowUpdate(IntPtr.Zero);
		}
	}
}