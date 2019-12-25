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
		private Dictionary<string, NewControl> controls;
		private Dictionary<string, Bitmap> textures;
		private Dictionary<string, Color> colorTheme;

		private Bitmap bitmap;
		private Graphics graphics;

		private int controlBoxHeight;
		private int minWidth;
		private int minHeight;

		private Point cursor;
		private bool leftMouse;
		private Point grip;
		private NewControl hovered;
		private bool moving;
		private int resize;

		private Point gameFieldLocation;
		private Bitmap gameField;
		private Graphics gfGraphics;

		private void LocateGUI()
		{
			controls["corner_exit"].SetPlace(new Point(Width - controlBoxHeight + 1, 1));
			controls["corner_exit"].SetSize(new Size(controlBoxHeight - 2, controlBoxHeight - 2));
			controls["control_box"].SetPlace(new Point());
			controls["control_box"].SetSize(new Size(Width, controlBoxHeight));

			controls["bottom_grip"].SetPlace(new Point(0, Height - 2));
			controls["bottom_grip"].SetSize(new Size(Width, 2));
			controls["right_grip"].SetPlace(new Point(Width - 2, 0));
			controls["right_grip"].SetSize(new Size(2, Height));
			controls["bottom_right_grip"].SetPlace(new Point(Width - 5, Height - 5));
			controls["bottom_right_grip"].SetSize(new Size(5, 5));
		}

		private void UpdateControls()
		{
			hovered = null;
			foreach (var control in controls)
			{
				control.Value.UpdateMouseState(cursor, leftMouse);
				if (control.Value.Hovered) hovered = control.Value;
			}
		}

		private void MoveForm()
		{
			Left = Cursor.Position.X - grip.X;
			Top = Cursor.Position.Y - grip.Y;
		}

		private void ResizeForm()
		{
			switch (resize)
			{
				case 3: // Right
					Size = new Size(Math.Max(Cursor.Position.X - Left + 1, minWidth), Height);
					FormResize();
					break;
				case 4: // Bottom-right
					Size = new Size(Math.Max(Cursor.Position.X - Left + 1, minWidth),
						Math.Max(Cursor.Position.Y - Top + 1, minHeight));
					FormResize();
					break;
				case 5: // Bottom
					Size = new Size(Width, Math.Max(Cursor.Position.Y - Top + 1, minHeight));
					FormResize();
					break;
			}
		}

		private void StartRelocaing()
		{
			grip = cursor;
			moving = true;
		}

		private void StopRelocaing()
		{
			moving = false;
		}

		private void CloseForm()
		{
			Close();
		}

		private void UpdateFrame()
		{
			foreach (var i in controls)
			{
				if (i.Value.Name == "") continue;
				if (i.Value == hovered)
					if (i.Value.Pressed) graphics.DrawImage(textures[i.Value.Name + "Pressed"], i.Value.Bounds);
					else graphics.DrawImage(textures[i.Value.Name + "Hovered"], i.Value.Bounds);
				else
					graphics.DrawImage(textures[i.Value.Name + "Normal"], i.Value.Bounds);
			}
			UpdateGameField();
			graphics.DrawImage(gameField, gameFieldLocation);

			viewport.Refresh();
		}

		private void DrawInitial()
		{ 
			graphics.Clear(colorTheme["BG"]);
			graphics.DrawRectangle(new Pen(colorTheme["ControlBoxBG"]), new Rectangle(0, 0, Width - 1, Height - 1));
			graphics.FillRectangle(new SolidBrush(colorTheme["ControlBoxBG"]), controls["control_box"].Bounds);
		}

		private void FormResize()
		{
			bitmap = new Bitmap(Width, Height);
			graphics = Graphics.FromImage(bitmap);
			int sz = Math.Min(Width, Height - controlBoxHeight) - 6;
			gameField = new Bitmap(sz, sz);
			gfGraphics = Graphics.FromImage(gameField);
			gameFieldLocation = new Point(3, 3 + controlBoxHeight);
			LocateGUI();
			DrawInitial();
			InitialGameField();
			viewport.Image = bitmap;
		}

		private void UpdateGameField()
		{

		}

		private void InitialGameField()
		{
			gfGraphics.DrawRectangle(new Pen(colorTheme["GridColor"]), new Rectangle(0, 0, gameField.Width - 1, gameField.Height - 1));
		}

		private void ReadTheme()
		{
			colorTheme = new Dictionary<string, Color>();
			textures = new Dictionary<string, Bitmap>();
			string path = "Resources\\Themes\\Classic\\";
			// Color theme reading
			FileInfo di = new FileInfo(path + "colorTheme.txt");
			var reader = di.OpenText();
			while (!reader.EndOfStream)
			{
				var s = reader.ReadLine().Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				colorTheme.Add(s[0], ColorTranslator.FromHtml(s[1]));
			}
			// Textures reading
			di = new FileInfo(path + "Theme.txt");
			reader = di.OpenText();
			while (!reader.EndOfStream)
			{
				var s = reader.ReadLine().Replace('\t', ' ').Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				string prefix = s[0];
				string fileName = s[1];
				for (int i = 2; i < s.Length; i++) if (fileName[fileName.Length - 1] != '"') fileName += s[i];
				fileName = fileName.Substring(1, fileName.Length - 2);
				var bit = new Bitmap(path + fileName);
				textures.Add(prefix + "Source", bit);
				for (int i = 0; i < 3; i++)
				{
					s = reader.ReadLine().Replace('\t', ' ').Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
					textures.Add(prefix + s[0], ImagingPlus.GetClip(bit, 
						int.Parse(s[1]), int.Parse(s[2]), int.Parse(s[3]), int.Parse(s[4])));
				}
			}
		}
	}
}
