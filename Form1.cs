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
			textures = new Dictionary<string, Bitmap>();
			ReadTheme();
			buttons.Add("corner_exit", new GUI.Button(new Action(FormClose)));
			controlBoxHeight = 25;
			FormResize();
			LocateGUI();
		}

		private void MainForm_ResizeEnd(object sender, EventArgs e)
		{
			FormResize();
		}
	}
}
