using System;
using System.Drawing;
using System.Windows.Forms;
using Controls;

namespace Tester
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			txtCode.LanguageOption = RichTextBoxLanguageOptions.UIFonts;
			txtCode.SetTabStops(16);
			var cmhd = new CompositeHighlightTokenizer();
			cmhd.HighlightDescriptors.Add(new RangeHighlightDescriptor(Color.Green, Color.Empty, "/*", "*/"));
			cmhd.HighlightDescriptors.Add(new RangeHighlightDescriptor(Color.Green, Color.Empty, "//", "\n"));
			cmhd.HighlightDescriptors.Add(new EscapedRangeHighlightDescriptor(Color.FromArgb(163, 21, 21), Color.Empty, '"', '\\', true));
			cmhd.HighlightDescriptors.Add(new EscapedRangeHighlightDescriptor(Color.FromArgb(163, 21, 21), Color.Empty, '\'', '\\', true));
			cmhd.HighlightDescriptors.Add(new WordHighlightDescriptor(Color.Blue, Color.Empty, true, "function", "property", "class", "getter", "setter", "extends", "global", "super", "this"));
			cmhd.HighlightDescriptors.Add(new WordHighlightDescriptor(Color.Blue, Color.Empty, true, "if", "else", "switch", "case", "default", "while", "do", "for", "try", "catch", "throw", "with", "return", "break", "continue"));
			cmhd.HighlightDescriptors.Add(new WordHighlightDescriptor(Color.Blue, Color.Empty, true, "var", "new", "invalidate", "isvalid", "delete", "typeof", "instanceof", "incontextof", "int", "real", "string"));
			cmhd.HighlightDescriptors.Add(new WordHighlightDescriptor(Color.Blue, Color.Empty, true, "void", "null", "true", "false"));
			txtCode.HighlightTokenizer = cmhd;
		}
	}
}
