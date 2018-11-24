using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Controls.Wpf
{
	/// <summary>構文の強調表示が可能なテキストボックスを表します。</summary>
	public class SyntaxHighlightingTextBox : TextBox
	{
		/// <summary><see cref="SyntaxHighlightingTextBox"/> クラスの新しいインスタンスを初期化します。</summary>
		public SyntaxHighlightingTextBox()
		{
			Background = Brushes.Transparent;
			renderTimer = new System.Windows.Threading.DispatcherTimer();
			renderTimer.IsEnabled = false;
			renderTimer.Tick += (s, ev) =>
			{
				renderTimer.IsEnabled = false;
				InvalidateVisual();
			};
			renderTimer.Interval = TimeSpan.FromMilliseconds(10);
		}

		/// <summary>このテキストボックス内の文字列を強調表示する方法を表す <see cref="IHighlightTokenizer"/> を取得または設定します。</summary>
		public IHighlightTokenizer HighlightTokenizer { get; set; }

		System.Windows.Threading.DispatcherTimer renderTimer;
		RenderInfo renderCache = new RenderInfo();
		bool scrollChangedListening = false;
		bool visibilitySet = false;
		
		/// <summary>
		/// 派生クラスでオーバーライドされると、レイアウト システムから指示されるレンダリング操作に参加します。
		/// この要素に対するレンダリング指示は、このメソッドの呼び出し時に直接使用されるわけではなく、
		/// 後からレイアウト処理や描画処理で非同期に使用されるときまで保存されます。
		/// </summary>
		/// <param name="drawingContext">特定の要素に対する描画命令。 このコンテキストはレイアウト システムに提供されます。</param>
		protected override void OnRender(DrawingContext drawingContext)
		{
			base.OnRender(drawingContext);
			EnsureScroll();
			EnsureSetVisibility();
			if (LineCount == 0)
			{
				drawingContext.DrawText(renderCache.Text, renderCache.RenderPoint);
				renderTimer.IsEnabled = true;
			}
			else
			{
				drawingContext.PushClip(new RectangleGeometry(new System.Windows.Rect(0, 0, ActualWidth, ActualHeight)));
				if (string.IsNullOrEmpty(Text) || VisibleText == null)
					return;
				var firstChar = FirstVisibleCharacterIndex;
				var lastChar = LastVisibleCharacterIndex;
				var ft = new FormattedText(VisibleText, CultureInfo.CurrentCulture, FlowDirection, new Typeface(FontFamily.Source), FontSize, Foreground);
				if (HighlightTokenizer != null)
				{
					foreach (var range in HighlightTokenizer.GetTokens(Text))
					{
						int length = Math.Min(Math.Min(Math.Min(firstChar + ft.Text.Length - range.First, range.First + range.Length - firstChar), ft.Text.Length), range.Length);
						if (length > 0)
							ft.SetForegroundBrush(new SolidColorBrush(range.ForeColor), Math.Max(0, range.First - firstChar), length);
					}
				}
				drawingContext.DrawText(renderCache.Text = ft, renderCache.RenderPoint = GetRectFromCharacterIndex(firstChar).Location);
			}
		}

		/// <summary>この編集コントロールの内容が変更されると呼び出されます。</summary>
		/// <param name="e"><see cref="System.Windows.Controls.Primitives.TextBoxBase.TextChanged"/> イベントに関連付けられている引数。</param>
		protected override void OnTextChanged(TextChangedEventArgs e)
		{
			base.OnTextChanged(e);
			InvalidateVisual();
		}

		void EnsureSetVisibility()
		{
			if (!visibilitySet)
			{
				((System.Windows.FrameworkElement)typeof(TextBoxBase).GetProperty(
					"RenderScope",
					System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
				).GetValue(this, null)).Visibility = System.Windows.Visibility.Hidden;
				visibilitySet = true;
			}
		}

		void EnsureScroll()
		{
			if (!scrollChangedListening)
			{
				var sc = (ScrollViewer)Template.FindName("PART_ContentHost", this);
				sc.ScrollChanged += (s, ev) => InvalidateVisual();
				scrollChangedListening = true;
			}
		}

		int FirstVisibleCharacterIndex
		{
			get
			{
				var firstLineIndex = GetFirstVisibleLineIndex();
				return firstLineIndex == 0 ? 0 : GetCharacterIndexFromLineIndex(firstLineIndex);
			}
		}

		int LastVisibleCharacterIndex
		{
			get
			{
				var lastLineIndex = GetLastVisibleLineIndex();
				return lastLineIndex < 0 ? 0 : GetCharacterIndexFromLineIndex(lastLineIndex) + GetLineLength(lastLineIndex);
			}
		}

		string VisibleText
		{
			get
			{
				if (Text == "") return "";
				try
				{
					int firstChar = FirstVisibleCharacterIndex;
					return Text.Substring(firstChar, Math.Min(Text.Length - firstChar, LastVisibleCharacterIndex - firstChar)) ?? "";
				}
				catch { return null; }
			}
		}

		class RenderInfo
		{
			public FormattedText Text { get; set; }
			public System.Windows.Point RenderPoint { get; set; }
		}
	}
}
