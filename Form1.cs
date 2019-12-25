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
			controls = new Dictionary<string, NewControl>();
			ReadTheme();
			NewControl control;
			control = new NewControl("");
			control.MouseDownLeft += StartRelocaing;
			control.MouseUpLeft += StopRelocaing;
			controls.Add("control_box", control);
			control = new NewControl("");
			control.MouseEnter += () => Cursor.Current = Cursors.SizeNS;
			control.MouseLeave += () => Cursor.Current = Cursors.Default;
			control.MouseDownLeft += () => resize = 5;
			control.MouseUpLeft += () => resize = 0;
			controls.Add("bottom_grip", control);
			control = new NewControl("");
			control.MouseEnter += () => Cursor.Current = Cursors.SizeWE;
			control.MouseLeave += () => Cursor.Current = Cursors.Default;
			control.MouseDownLeft += () => resize = 3;
			control.MouseUpLeft += () => resize = 0;
			controls.Add("right_grip", control);
			control = new NewControl("");
			control.MouseEnter += () => Cursor.Current = Cursors.SizeNWSE;
			control.MouseLeave += () => Cursor.Current = Cursors.Default;
			control.MouseDownLeft += () => resize = 4;
			control.MouseUpLeft += () => resize = 0;
			controls.Add("bottom_right_grip", control);
			control = new NewControl("CloseButton");
			control.MouseClickLeft += CloseForm;
			controls.Add("corner_exit", control);
			controlBoxHeight = 25;
			minWidth = 50;
			minHeight = 50 + controlBoxHeight;
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

		private void MouseStateChanged(object sender, MouseEventArgs e)
		{
			cursor = e.Location;
			leftMouse = e.Button.HasFlag(MouseButtons.Left);
			if (moving) MoveForm();
			if (resize != 0) ResizeForm();
			UpdateControls();
		}

		private void Viewport_MouseLeave(object sender, EventArgs e)
		{
			cursor.X = -100;
			UpdateControls();
		}
	}
}
