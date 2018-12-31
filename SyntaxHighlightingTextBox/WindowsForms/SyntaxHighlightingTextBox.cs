using System;
using System.Drawing;
using System.Windows.Forms;

namespace Controls.WindowsForms
{
	/// <summary>構文の強調表示が可能なテキストボックスを表します。</summary>
	public class SyntaxHighlightingTextBox : RichTextBox
	{
		/// <summary><see cref="SyntaxHighlightingTextBox"/> クラスの新しいインスタンスを初期化します。</summary>
		public SyntaxHighlightingTextBox()
		{
			autoHighlightTimer.SynchronizingObject = this;
			autoHighlightTimer.AutoReset = true;
			autoHighlightTimer.Elapsed += AutoHighlightTimer_Elapsed;
		}

		int averageCharacterWidth = 0;
		RtfBuilder rtfBuilder = new RtfBuilder();
		string lastChangedText = null;
		string lastHighlightedText = null;
		System.Timers.Timer autoHighlightTimer = new System.Timers.Timer();
		double highlightWaitTime = 0;

		/// <summary><see cref="SyntaxHighlightingTextBox"/> とその子コントロールが使用しているアンマネージ リソースを解放します。オプションで、マネージ リソースも解放します。</summary>
		/// <param name="disposing">マネージ リソースとアンマネージ リソースの両方を解放する場合は <c>true</c>。アンマネージ リソースだけを解放する場合は <c>false</c>。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && autoHighlightTimer != null)
			{
				autoHighlightTimer.Dispose();
				autoHighlightTimer = null;
			}
			base.Dispose(disposing);
		}

		/// <summary>タブの停止位置を設定します。停止位置は平均文字幅の1/4単位で表されます。</summary>
		/// <param name="stops">設定する停止位置を指定します。</param>
		/// <exception cref="ArgumentNullException"><paramref name="stops"/> が <c>null</c> です。</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="stops"/> の要素数が 0 以下であるか 32 より大きな値です。</exception>
		public void SetTabStops(params int[] stops)
		{
			if (stops == null)
				throw new ArgumentNullException(nameof(stops));
			if (stops.Length <= 0 || stops.Length > 32)
				throw new ArgumentOutOfRangeException(nameof(stops));
			rtfBuilder.TabStops.Clear();
			if (stops.Length == 1)
			{
				for (var i = 1; i <= 32; i++)
					rtfBuilder.TabStops.Add(averageCharacterWidth * stops[0] * i / 4);
			}
			else
			{
				for (var i = 0; i < stops.Length; i++)
					rtfBuilder.TabStops.Add(averageCharacterWidth * stops[i] / 4);
			}
			HighlightSyntax(null);
		}

		/// <summary>このテキストボックス内の文字列を強調表示する方法を表す <see cref="IHighlightTokenizer"/> を取得または設定します。</summary>
		public IHighlightTokenizer HighlightTokenizer { get; set; }

		/// <summary>コントロール内にテキストを表示するときに使用するフォントを取得または設定します。</summary>
		public override Font Font
		{
			get => base.Font;
			set
			{
				if (base.Font != value)
				{
					base.Font = value;
					rtfBuilder.Font = value;
					using (var graphics = CreateGraphics())
						averageCharacterWidth = (int)(TextMetrics.FromFont(graphics, value).AverageCharacterWidth * 1440 / graphics.DpiX);
					HighlightSyntax(null);
				}
			}
		}

		/// <summary>
		/// 最後のテキスト入力から強調表示が行われるまでの待ち時間を指定します。
		/// 時間内に別のテキスト入力があった場合は強調表示は実行されません。
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// 値が 0 未満であるか、<see cref="int.MaxValue"/> より大きな値です。
		/// </exception>
		public double HighlightWaitTime
		{
			get => highlightWaitTime;
			set
			{
				if (value < 0 || value > int.MaxValue)
					throw new ArgumentOutOfRangeException(nameof(value));
				highlightWaitTime = value;
			}
		}

		/// <summary><see cref="System.Windows.Forms.Control.TextChanged"/> イベントを発生させます。</summary>
		/// <param name="e">イベント データを格納している <see cref="EventArgs"/>。</param>
		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			lastChangedText = Text;
			autoHighlightTimer.Stop();
			if (HighlightWaitTime == 0)
				AutoHighlightTimer_Elapsed(autoHighlightTimer, EventArgs.Empty);
			else
			{
				autoHighlightTimer.Interval = HighlightWaitTime;
				autoHighlightTimer.Start();
			}
		}

		void HighlightSyntax(string text)
		{
			NativeMethods.LockWindowUpdate(Handle);
			var position = NativeMethods.GetScrollPosition(Handle);
			var cursorLoc = SelectionStart;

			if (text != null)
			{
				rtfBuilder.Clear();
				var startIndex = 0;
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

		void AutoHighlightTimer_Elapsed(object sender, EventArgs e)
		{
			if (lastChangedText != lastHighlightedText)
			{
				HighlightSyntax(lastChangedText);
				lastHighlightedText = lastChangedText;
			}
		}
	}
}