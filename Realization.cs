using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Sapper_2019
{
	public partial class MainForm : Form
	{
		private Dictionary<string, GUI.Button> buttons;
		private Dictionary<string, Bitmap> textures;
		private Dictionary<string, Color> colorTheme;

		private Bitmap bitmap;
		private Graphics graphics;

		private Rectangle controlBoxBounds;

		private int controlBoxHeight;

		private void LocateGUI()
		{

		}

		private void FormClose()
		{
			Close();
		}

		private void UpdateFrame()
		{

		}

		private void DrawInitial()
		{
			graphics.Clear(colorTheme["BG"]);
			graphics.DrawRectangle(new Pen(colorTheme["ControlBoxBG"]), new Rectangle(0, 0, Width - 1, Height - 1));
			graphics.FillRectangle(new SolidBrush(colorTheme["ControlBoxBG"]), controlBoxBounds);
		}

		private void FormResize()
		{
			bitmap = new Bitmap(Width, Height);
			graphics = Graphics.FromImage(bitmap);
			controlBoxBounds = new Rectangle(0, 0, Width, controlBoxHeight);
			DrawInitial();
			viewport.Image = bitmap;
		}

		private void ReadTheme()
		{
			string path = "Resources\\Themes\\Classic\\";
			FileInfo di = new FileInfo(path + "colorTheme.txt");
			var reader = di.OpenText();
			colorTheme = new Dictionary<string, Color>();
			while (!reader.EndOfStream)
			{
				var s = reader.ReadLine().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				colorTheme.Add(s[0], ColorTranslator.FromHtml(s[1]));
			}
		}
	}
}
