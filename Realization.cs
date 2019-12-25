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
using Stuff;

namespace Sapper_2019
{
	public partial class MainForm : Form
	{
		private Dictionary<string, GUI.Button> buttons;
		private Dictionary<string, Bitmap> textures;
		private Dictionary<string, Color> colorTheme;

		private Bitmap bitmap;
		private Graphics graphics;

		private int controlBoxHeight;

		private Point cursor;
		private Point grip;
		private GUI.Button hovered;
		private bool leftMouse;
		private bool moving;
		private int resize;

		private void LocateGUI()
		{
			buttons["corner_exit"].SetPlace(new Point(Width - controlBoxHeight + 1, 1));
			buttons["corner_exit"].SetSize(new Size(controlBoxHeight - 2, controlBoxHeight - 2));
			buttons["control_box"].SetPlace(new Point());
			buttons["control_box"].SetSize(new Size(Width, controlBoxHeight));

			buttons["bottom_grip"].SetPlace(new Point(0, Height - 2));
			buttons["bottom_grip"].SetSize(new Size(Width, 2));
			buttons["right_grip"].SetPlace(new Point(Width - 2, 0));
			buttons["right_grip"].SetSize(new Size(2, Height));
			buttons["bottom_right_grip"].SetPlace(new Point(Width - 5, Height - 5));
			buttons["bottom_right_grip"].SetSize(new Size(5, 5));
		}

		private void StartRelocaing()
		{
			grip = cursor;
			moving = true;
		}

		private void BottomResize()
		{
			grip = cursor;
			resize = 4;
		}

		private void RightResize()
		{
			grip = cursor;
			resize = 3;
		}

		private void BottomRightResize()
		{
			grip = cursor;
			resize = 7;
		}

		private void FormClose()
		{
			Close();
		}

		private void UpdateFrame()
		{
			GUI.Button temp = null;
			foreach (var i in buttons)
			{
				if (i.Value.IsTriggered(cursor)) temp = i.Value;
			}

			//if (temp != hovered)
			{
				hovered = temp;
				foreach (var i in buttons)
				{
					if (i.Value.GetName() == "") continue;
					if (i.Value == hovered)
						if (leftMouse) graphics.DrawImage(textures[i.Value.GetName() + "Clicked"], i.Value.GetRect());
						else graphics.DrawImage(textures[i.Value.GetName() + "Hovered"], i.Value.GetRect()); else
						graphics.DrawImage(textures[i.Value.GetName() + "Normal"], i.Value.GetRect());
				}
			}

			viewport.Refresh();
		}

		private void DrawInitial()
		{ 
			graphics.Clear(colorTheme["BG"]);
			graphics.DrawRectangle(new Pen(colorTheme["ControlBoxBG"]), new Rectangle(0, 0, Width - 1, Height - 1));
			graphics.FillRectangle(new SolidBrush(colorTheme["ControlBoxBG"]), buttons["control_box"].GetRect());
		}

		private void FormResize()
		{
			bitmap = new Bitmap(Width, Height);
			graphics = Graphics.FromImage(bitmap);
			LocateGUI();
			DrawInitial();
			viewport.Image = bitmap;
		}

		private void ReadTheme()
		{
			string path = "Resources\\Themes\\Classic\\";
			FileInfo di = new FileInfo(path + "colorTheme.txt");
			var reader = di.OpenText();
			colorTheme = new Dictionary<string, Color>();
			textures = new Dictionary<string, Bitmap>();
			while (!reader.EndOfStream)
			{
				var s = reader.ReadLine().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				colorTheme.Add(s[0], ColorTranslator.FromHtml(s[1]));
			}
			var bit = new Bitmap(path + "close_button.png");
			textures.Add("CloseButtonSource", bit);
			textures.Add("CloseButtonNormal", ImagingPlus.GetClip(bit, 0, 0, 50, 50));
			textures.Add("CloseButtonHovered", ImagingPlus.GetClip(bit, 0, 50, 50, 50));
			textures.Add("CloseButtonClicked", ImagingPlus.GetClip(bit, 0, 100, 50, 50));

		}
	}
}
