namespace Sapper_2019
{
	partial class MainForm
	{
		/// <summary>
		/// Обязательная переменная конструктора.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Освободить все используемые ресурсы.
		/// </summary>
		/// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Код, автоматически созданный конструктором форм Windows

		/// <summary>
		/// Требуемый метод для поддержки конструктора — не изменяйте 
		/// содержимое этого метода с помощью редактора кода.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			this.viewport = new System.Windows.Forms.PictureBox();
			this.updater = new System.Windows.Forms.Timer(this.components);
			((System.ComponentModel.ISupportInitialize)(this.viewport)).BeginInit();
			this.SuspendLayout();
			// 
			// viewport
			// 
			this.viewport.Dock = System.Windows.Forms.DockStyle.Fill;
			this.viewport.Location = new System.Drawing.Point(0, 0);
			this.viewport.Name = "viewport";
			this.viewport.Size = new System.Drawing.Size(421, 337);
			this.viewport.TabIndex = 0;
			this.viewport.TabStop = false;
			this.viewport.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MouseStateChanged);
			this.viewport.MouseLeave += new System.EventHandler(this.Viewport_MouseLeave);
			this.viewport.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MouseStateChanged);
			this.viewport.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MouseStateChanged);
			// 
			// updater
			// 
			this.updater.Interval = 10;
			this.updater.Tick += new System.EventHandler(this.Updater_Tick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(421, 337);
			this.Controls.Add(this.viewport);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.Name = "MainForm";
			this.Text = "Form1";
			this.ResizeEnd += new System.EventHandler(this.MainForm_ResizeEnd);
			((System.ComponentModel.ISupportInitialize)(this.viewport)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.PictureBox viewport;
		private System.Windows.Forms.Timer updater;
	}
}

