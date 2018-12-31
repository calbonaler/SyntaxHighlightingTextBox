using System;
using System.Drawing;
using System.Windows.Forms;

namespace Controls.WindowsForms
{
	/// <summary>�\���̋����\�����\�ȃe�L�X�g�{�b�N�X��\���܂��B</summary>
	public class SyntaxHighlightingTextBox : RichTextBox
	{
		/// <summary><see cref="SyntaxHighlightingTextBox"/> �N���X�̐V�����C���X�^���X�����������܂��B</summary>
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

		/// <summary><see cref="SyntaxHighlightingTextBox"/> �Ƃ��̎q�R���g���[�����g�p���Ă���A���}�l�[�W ���\�[�X��������܂��B�I�v�V�����ŁA�}�l�[�W ���\�[�X��������܂��B</summary>
		/// <param name="disposing">�}�l�[�W ���\�[�X�ƃA���}�l�[�W ���\�[�X�̗������������ꍇ�� <c>true</c>�B�A���}�l�[�W ���\�[�X�������������ꍇ�� <c>false</c>�B</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && autoHighlightTimer != null)
			{
				autoHighlightTimer.Dispose();
				autoHighlightTimer = null;
			}
			base.Dispose(disposing);
		}

		/// <summary>�^�u�̒�~�ʒu��ݒ肵�܂��B��~�ʒu�͕��ϕ�������1/4�P�ʂŕ\����܂��B</summary>
		/// <param name="stops">�ݒ肷���~�ʒu���w�肵�܂��B</param>
		/// <exception cref="ArgumentNullException"><paramref name="stops"/> �� <c>null</c> �ł��B</exception>
		/// <exception cref="ArgumentOutOfRangeException"><paramref name="stops"/> �̗v�f���� 0 �ȉ��ł��邩 32 ���傫�Ȓl�ł��B</exception>
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

		/// <summary>���̃e�L�X�g�{�b�N�X���̕�����������\��������@��\�� <see cref="IHighlightTokenizer"/> ���擾�܂��͐ݒ肵�܂��B</summary>
		public IHighlightTokenizer HighlightTokenizer { get; set; }

		/// <summary>�R���g���[�����Ƀe�L�X�g��\������Ƃ��Ɏg�p����t�H���g���擾�܂��͐ݒ肵�܂��B</summary>
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
		/// �Ō�̃e�L�X�g���͂��狭���\�����s����܂ł̑҂����Ԃ��w�肵�܂��B
		/// ���ԓ��ɕʂ̃e�L�X�g���͂��������ꍇ�͋����\���͎��s����܂���B
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		/// �l�� 0 �����ł��邩�A<see cref="int.MaxValue"/> ���傫�Ȓl�ł��B
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

		/// <summary><see cref="System.Windows.Forms.Control.TextChanged"/> �C�x���g�𔭐������܂��B</summary>
		/// <param name="e">�C�x���g �f�[�^���i�[���Ă��� <see cref="EventArgs"/>�B</param>
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