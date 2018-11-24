using System;
using System.Drawing;
using System.Windows.Forms;
using Controls;

namespace Tester
{
	class Program
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
	}
}
