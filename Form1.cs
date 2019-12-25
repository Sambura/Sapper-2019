using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sapper_2019
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
			buttons = new Dictionary<string, GUI.Button>();
			ReadTheme();
			buttons.Add("control_box", new GUI.Button(new Action(StartRelocaing), ""));
			buttons.Add("bottom_grip", new GUI.Button(new Action(BottomResize), ""));
			buttons.Add("right_grip", new GUI.Button(new Action(RightResize), ""));
			buttons.Add("bottom_right_grip", new GUI.Button(new Action(BottomRightResize), ""));
			buttons.Add("corner_exit", new GUI.Button(new Action(FormClose), "CloseButton"));
			controlBoxHeight = 25;
			FormResize();
			updater.Start();
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			FormResize();
		}

		private void Updater_Tick(object sender, EventArgs e)
		{
			UpdateFrame();
		}

		private void Viewport_MouseMove(object sender, MouseEventArgs e)
		{
			cursor = e.Location;
			if (moving)
			{
				Left = Cursor.Position.X - grip.X;
				Top = Cursor.Position.Y - grip.Y;
			} else switch (resize)
				{
					case 4:
						Size = new Size(Width, Cursor.Position.Y - Top + 1);
						FormResize();
						break;
					case 3:
						Size = new Size(Cursor.Position.X - Left + 1, Height);
						FormResize();
						break;
					case 7:
						Size = new Size(Cursor.Position.X - Left + 1, Cursor.Position.Y - Top + 1);
						FormResize();
						break;
				}
		}

		private void Viewport_MouseLeave(object sender, EventArgs e)
		{
			cursor.X = -100;
		}

		private void Viewport_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;
			leftMouse = true;
			if (hovered != null)
				hovered.Click();
		}

		private void Viewport_MouseUp(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;
			leftMouse = false;
			moving = false;
			resize = 0;
		}
	}
}
