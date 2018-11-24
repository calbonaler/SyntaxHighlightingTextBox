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
	/// <summary>�����\�����镶������͈̔͂�����������@��񋟂��܂��B</summary>
	public interface IHighlightDescriptor
	{
		/// <summary>�w�肳�ꂽ������̒����狭���\������͈͂��������܂��B</summary>
		/// <param name="text">�����\������͈͂��������镶������w�肵�܂��B</param>
		/// <param name="startIndex">��������͈̔͂̌������J�n����ʒu���w�肵�܂��B</param>
		/// <param name="rangeStartLimit">���ʂƂ��ē�����͈͂̊J�n�ʒu�̔r���I������w�肵�܂��B</param>
		/// <returns>�����\������镶������͈̔͂�\�� <see cref="HighlightToken"/>�B</returns>
		HighlightToken GetToken(string text, int startIndex, int rangeStartLimit);
	}

	/// <summary>�����\�����镶������͈̔͂�\���܂��B</summary>
	public struct HighlightToken
	{
		/// <summary>�w�肳�ꂽ�J�n�ʒu�A�����A�O�i�F�A�w�i�F���g�p���āA<see cref="HighlightToken"/> �N���X�̐V�����C���X�^���X�����������܂��B</summary>
		/// <param name="first">�͈͂̊J�n�ʒu���w�肵�܂��B</param>
		/// <param name="length">�͈͂̒������w�肵�܂��B</param>
		/// <param name="foreColor">�͈͂������\������Ƃ��Ɏg�p����O�i�F���w�肵�܂��B</param>
		/// <param name="backColor">�͈͂������\������Ƃ��Ɏg�p����w�i�F���w�肵�܂��B</param>
		public HighlightToken(int first, int length, Color foreColor, Color backColor) : this()
		{
			First = first;
			Length = length;
			ForeColor = foreColor;
			BackColor = backColor;
		}

		/// <summary>�w�肳�ꂽ�J�n�ʒu�A�����A�O�i�F�A�w�i�F���g�p���āA<see cref="HighlightToken"/> �N���X�̐V�����C���X�^���X�����������܂��B</summary>
		/// <param name="first">�͈͂̊J�n�ʒu���w�肵�܂��B</param>
		/// <param name="length">�͈͂̒������w�肵�܂��B</param>
		/// <param name="foreColor">�͈͂������\������Ƃ��Ɏg�p����O�i�F���w�肵�܂��B</param>
		/// <param name="backColor">�͈͂������\������Ƃ��Ɏg�p����w�i�F���w�肵�܂��B</param>
		public HighlightToken(int first, int length, DColor foreColor, DColor backColor) : this(first, length, foreColor.ToMedia(), backColor.ToMedia()) { }

		/// <summary>��������̂ǂ̏ꏊ���\���Ȃ� <see cref="HighlightToken"/> ���擾���܂��B</summary>
		public static readonly HighlightToken Empty = new HighlightToken();

		/// <summary>�͈͂̊J�n�ʒu���擾���܂��B</summary>
		public int First { get; private set; }

		/// <summary>�͈͂̒������擾���܂��B</summary>
		public int Length { get; private set; }

		/// <summary>�͈͂̕\���Ɏg�p�����O�i�F���擾���܂��B</summary>
		public Color ForeColor { get; private set; }

		/// <summary>�͈͂̕\���Ɏg�p�����w�i�F���擾���܂��B</summary>
		public Color BackColor { get; private set; }
	}

	/// <summary>2�̕�����ɂ���Ĉ͂܂ꂽ�͈͂������\�����܂��B</summary>
	public class RangeHighlightDescriptor : IHighlightDescriptor
	{
		/// <summary><see cref="RangeHighlightDescriptor"/> �̐V�����C���X�^���X�����������܂��B</summary>
		public RangeHighlightDescriptor() { }

		/// <summary>�w�肳�ꂽ�O�i�F�A�w�i�F�A�J�n������A�I����������g�p���āA<see cref="RangeHighlightDescriptor"/> �N���X�̐V�����C���X�^���X�����������܂��B</summary>
		/// <param name="foreColor">�����\���Ɏg�p����O�i�F���w�肵�܂��B</param>
		/// <param name="backColor">�����\���Ɏg�p����w�i�F���w�肵�܂��B</param>
		/// <param name="openString">�͈͂̊J�n��\����������w�肵�܂��B</param>
		/// <param name="closeString">�͈͂̏I����\����������w�肵�܂��B</param>
		public RangeHighlightDescriptor(DColor foreColor, DColor backColor, string openString, string closeString)
		{
			ForeColor = foreColor.ToMedia();
			BackColor = backColor.ToMedia();
			OpenString = openString;
			CloseString = closeString;
		}

		/// <summary>�����\���Ɏg�p����O�i�F���擾�܂��͐ݒ肵�܂��B</summary>
		public Color ForeColor { get; set; }
		/// <summary>�����\���Ɏg�p����w�i�F���擾�܂��͐ݒ肵�܂��B</summary>
		public Color BackColor { get; set; }
		/// <summary>�����\������͈͂̊J�n��\����������擾�܂��͐ݒ肵�܂��B</summary>
		public string OpenString { get; set; }
		/// <summary>�����\������͈͂̏I����\����������擾�܂��͐ݒ肵�܂��B</summary>
		public string CloseString { get; set; }

		/// <summary>�w�肳�ꂽ������̒����狭���\������͈͂��������܂��B</summary>
		/// <param name="text">�����\������͈͂��������镶������w�肵�܂��B</param>
		/// <param name="startIndex">��������͈̔͂̌������J�n����ʒu���w�肵�܂��B</param>
		/// <param name="rangeStartLimit">���ʂƂ��ē�����͈͂̊J�n�ʒu�̔r���I������w�肵�܂��B</param>
		/// <returns>�����\������镶������͈̔͂�\�� <see cref="HighlightToken"/>�B</returns>
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

	/// <summary>���K�\���ɂ���Ďw�肳�ꂽ�͈͂������\�����܂��B</summary>
	public class RegexHighlightDescriptor : IHighlightDescriptor
	{
		/// <summary><see cref="RegexHighlightDescriptor"/> �N���X�̐V�����C���X�^���X�����������܂��B</summary>
		public RegexHighlightDescriptor() { }

		/// <summary>�w�肳�ꂽ�O�i�F�A�w�i�F�A���K�\���p�^�[�����g�p���āA<see cref="RegexHighlightDescriptor"/> �N���X�̐V�����C���X�^���X�����������܂��B</summary>
		/// <param name="foreColor">�����\���Ɏg�p����O�i�F���w�肵�܂��B</param>
		/// <param name="backColor">�����\���Ɏg�p����w�i�F���w�肵�܂��B</param>
		/// <param name="pattern">�����\������͈͂��������K�\���p�^�[�����w�肵�܂��B</param>
		public RegexHighlightDescriptor(DColor foreColor, DColor backColor, string pattern)
		{
			ForeColor = foreColor.ToMedia();
			BackColor = backColor.ToMedia();
			Pattern = pattern;
		}
		
		/// <summary>�����\���Ɏg�p����O�i�F���擾�܂��͐ݒ肵�܂��B</summary>
		public Color ForeColor { get; set; }
		/// <summary>�����\���Ɏg�p����w�i�F���擾�܂��͐ݒ肵�܂��B</summary>
		public Color BackColor { get; set; }
		/// <summary>�����\������͈͂��������K�\���p�^�[�����擾�܂��͐ݒ肵�܂��B</summary>
		public string Pattern { get; set; }

		/// <summary>�w�肳�ꂽ������̒����狭���\������͈͂��������܂��B</summary>
		/// <param name="text">�����\������͈͂��������镶������w�肵�܂��B</param>
		/// <param name="startIndex">��������͈̔͂̌������J�n����ʒu���w�肵�܂��B</param>
		/// <param name="rangeStartLimit">���ʂƂ��ē�����͈͂̊J�n�ʒu�̔r���I������w�肵�܂��B</param>
		/// <returns>�����\������镶������͈̔͂�\�� <see cref="HighlightToken"/>�B</returns>
		public virtual HighlightToken GetToken(string text, int startIndex, int rangeStartLimit)
		{
			var match = new Regex(Pattern, RegexOptions.ExplicitCapture).Match(text, startIndex);
			if (match.Success && match.Index < rangeStartLimit)
				return new HighlightToken(match.Index, match.Length, ForeColor, BackColor);
			return HighlightToken.Empty;
		}
	}

	/// <summary>�w�肳�ꂽ�P��������\�����܂��B</summary>
	public class WordHighlightDescriptor : IHighlightDescriptor
	{
		/// <summary><see cref="WordHighlightDescriptor"/> �N���X�̐V�����C���X�^���X�����������܂��B</summary>
		public WordHighlightDescriptor() { }

		/// <summary>�w�肳�ꂽ�O�i�F�A�w�i�F�A�P����g�p���āA<see cref="WordHighlightDescriptor"/> �N���X�̐V�����C���X�^���X�����������܂��B</summary>
		/// <param name="foreColor">�����\���Ɏg�p����O�i�F���w�肵�܂��B</param>
		/// <param name="backColor">�����\���Ɏg�p����w�i�F���w�肵�܂��B</param>
		/// <param name="caseSensitive">�P��̈�v�ő啶���Ə���������ʂ��邩�ǂ����������l���w�肵�܂��B</param>
		/// <param name="words">�����\������P����w�肵�܂��B�����w��ł��܂��B</param>
		public WordHighlightDescriptor(DColor foreColor, DColor backColor, bool caseSensitive, params string[] words)
		{
			ForeColor = foreColor.ToMedia();
			BackColor = backColor.ToMedia();
			Words.AddRange(words);
			IsCaseSensitive = caseSensitive;
		}

		/// <summary>�����\���Ɏg�p����O�i�F���擾�܂��͐ݒ肵�܂��B</summary>
		public Color ForeColor { get; set; }
		/// <summary>�����\���Ɏg�p����w�i�F���擾�܂��͐ݒ肵�܂��B</summary>
		public Color BackColor { get; set; }
		/// <summary>�����\������P��̃��X�g���擾�܂��͐ݒ肵�܂��B</summary>
		[TypeConverter(typeof(MultiWordTypeConverter))]
		public System.Collections.Specialized.StringCollection Words { get; set; } = new System.Collections.Specialized.StringCollection();
		/// <summary>�P��̈�v�ő啶���Ə���������ʂ��邩�ǂ����������l���擾�܂��͐ݒ肵�܂��B</summary>
		public bool IsCaseSensitive { get; set; }

		/// <summary>�w�肳�ꂽ������̒����狭���\������͈͂��������܂��B</summary>
		/// <param name="text">�����\������͈͂��������镶������w�肵�܂��B</param>
		/// <param name="startIndex">��������͈̔͂̌������J�n����ʒu���w�肵�܂��B</param>
		/// <param name="rangeStartLimit">���ʂƂ��ē�����͈͂̊J�n�ʒu�̔r���I������w�肵�܂��B</param>
		/// <returns>�����\������镶������͈̔͂�\�� <see cref="HighlightToken"/>�B</returns>
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

	/// <summary>�w�肳�ꂽ�����ɂ���Ĉ͂܂ꂽ�G�X�P�[�v�\�Ȕ͈͂������\�����܂��B</summary>
	public class EscapedRangeHighlightDescriptor : IHighlightDescriptor
	{
		/// <summary><see cref="EscapedRangeHighlightDescriptor"/> �N���X�̐V�����C���X�^���X�����������܂��B</summary>
		public EscapedRangeHighlightDescriptor() { }

		/// <summary>�w�肳�ꂽ�O�i�F�A�w�i�F�A���E�����A�G�X�P�[�v�������g�p���āA<see cref="EscapedRangeHighlightDescriptor"/> �N���X�̐V�����C���X�^���X�����������܂��B</summary>
		/// <param name="foreColor">�����\���Ɏg�p����O�i�F���w�肵�܂��B</param>
		/// <param name="backColor">�����\���Ɏg�p����w�i�F���w�肵�܂��B</param>
		/// <param name="boundary">�͈͂̊J�n�ƏI����\���������w�肵�܂��B</param>
		/// <param name="escape">�G�X�P�[�v�̂��߂Ɏg�p���镶�����w�肵�܂��B</param>
		/// <param name="multiline">�͈͂������s�ɋy�Ԃ��ǂ����������l���w�肵�܂��B</param>
		public EscapedRangeHighlightDescriptor(DColor foreColor, DColor backColor, char boundary, char escape, bool multiline)
		{
			ForeColor = foreColor.ToMedia();
			BackColor = backColor.ToMedia();
			BoundaryCharacter = boundary;
			EscapeCharacter = escape;
			IsMultiline = multiline;
		}

		/// <summary>�����\���Ɏg�p����O�i�F���擾�܂��͐ݒ肵�܂��B</summary>
		public Color ForeColor { get; set; }
		/// <summary>�����\���Ɏg�p����w�i�F���擾�܂��͐ݒ肵�܂��B</summary>
		public Color BackColor { get; set; }
		/// <summary>�����\������͈͂̊J�n�ƏI����\���������擾�܂��͐ݒ肵�܂��B</summary>
		public char BoundaryCharacter { get; set; }
		/// <summary>�G�X�P�[�v�̂��߂Ɏg�p���镶�����擾�܂��͐ݒ肵�܂��B</summary>
		public char EscapeCharacter { get; set; }
		/// <summary>�͈͂������s�ɋy�Ԃ��ǂ����������l���擾�܂��͐ݒ肵�܂��B</summary>
		public bool IsMultiline { get; set; }

		/// <summary>�w�肳�ꂽ������̒����狭���\������͈͂��������܂��B</summary>
		/// <param name="text">�����\������͈͂��������镶������w�肵�܂��B</param>
		/// <param name="startIndex">��������͈̔͂̌������J�n����ʒu���w�肵�܂��B</param>
		/// <param name="rangeStartLimit">���ʂƂ��ē�����͈͂̊J�n�ʒu�̔r���I������w�肵�܂��B</param>
		/// <returns>�����\������镶������͈̔͂�\�� <see cref="HighlightToken"/>�B</returns>
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
