using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Controls
{
	static class Utils
	{
		public static System.Windows.Media.Color ToMedia(this System.Drawing.Color color) { return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B); }
	}
}
