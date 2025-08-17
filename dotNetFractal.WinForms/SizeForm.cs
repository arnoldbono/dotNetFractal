using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace dotNetFractal
{
	/// <summary>
	/// Summary description for SizeForm.
	/// </summary>
	public class SizeForm : System.Windows.Forms.Form
	{
		private Size _size;
		private Size _clientSize;
		private System.Windows.Forms.GroupBox groupBoxSizes;
		private System.Windows.Forms.RadioButton radioButtonSizeOfWindow;
		private System.Windows.Forms.RadioButton radioButtonSmall;
		private System.Windows.Forms.RadioButton radioButtonLarge;
		private System.Windows.Forms.RadioButton radioButtonCustom;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBoxWidth;
		private System.Windows.Forms.TextBox textBoxHeight;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SizeForm(Size size, Size clientSize)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this._size = size;
			this._clientSize = clientSize;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.groupBoxSizes = new System.Windows.Forms.GroupBox();
            this.radioButtonCustom = new System.Windows.Forms.RadioButton();
            this.radioButtonLarge = new System.Windows.Forms.RadioButton();
            this.radioButtonSmall = new System.Windows.Forms.RadioButton();
            this.radioButtonSizeOfWindow = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxWidth = new System.Windows.Forms.TextBox();
            this.textBoxHeight = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.groupBoxSizes.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxSizes
            // 
            this.groupBoxSizes.Controls.Add(this.radioButtonCustom);
            this.groupBoxSizes.Controls.Add(this.radioButtonLarge);
            this.groupBoxSizes.Controls.Add(this.radioButtonSmall);
            this.groupBoxSizes.Controls.Add(this.radioButtonSizeOfWindow);
            this.groupBoxSizes.Location = new System.Drawing.Point(13, 4);
            this.groupBoxSizes.Name = "groupBoxSizes";
            this.groupBoxSizes.Size = new System.Drawing.Size(269, 187);
            this.groupBoxSizes.TabIndex = 0;
            this.groupBoxSizes.TabStop = false;
            this.groupBoxSizes.Text = "&Sizes:";
            // 
            // radioButtonCustom
            // 
            this.radioButtonCustom.Checked = true;
            this.radioButtonCustom.Location = new System.Drawing.Point(13, 140);
            this.radioButtonCustom.Name = "radioButtonCustom";
            this.radioButtonCustom.Size = new System.Drawing.Size(179, 35);
            this.radioButtonCustom.TabIndex = 3;
            this.radioButtonCustom.TabStop = true;
            this.radioButtonCustom.Text = "&Custom";
            this.radioButtonCustom.CheckedChanged += new System.EventHandler(this.radioButtonCustom_CheckedChanged);
            // 
            // radioButtonLarge
            // 
            this.radioButtonLarge.Location = new System.Drawing.Point(13, 102);
            this.radioButtonLarge.Name = "radioButtonLarge";
            this.radioButtonLarge.Size = new System.Drawing.Size(179, 35);
            this.radioButtonLarge.TabIndex = 2;
            this.radioButtonLarge.Text = "&Large";
            this.radioButtonLarge.CheckedChanged += new System.EventHandler(this.radioButtonLarge_CheckedChanged);
            // 
            // radioButtonSmall
            // 
            this.radioButtonSmall.Location = new System.Drawing.Point(13, 64);
            this.radioButtonSmall.Name = "radioButtonSmall";
            this.radioButtonSmall.Size = new System.Drawing.Size(179, 35);
            this.radioButtonSmall.TabIndex = 1;
            this.radioButtonSmall.Text = "S&mall";
            this.radioButtonSmall.CheckedChanged += new System.EventHandler(this.radioButtonSmall_CheckedChanged);
            // 
            // radioButtonSizeOfWindow
            // 
            this.radioButtonSizeOfWindow.Location = new System.Drawing.Point(13, 26);
            this.radioButtonSizeOfWindow.Name = "radioButtonSizeOfWindow";
            this.radioButtonSizeOfWindow.Size = new System.Drawing.Size(179, 35);
            this.radioButtonSizeOfWindow.TabIndex = 0;
            this.radioButtonSizeOfWindow.Text = "&Size of Window";
            this.radioButtonSizeOfWindow.CheckedChanged += new System.EventHandler(this.radioButtonSizeOfWindow_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 210);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "&Width:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 246);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 23);
            this.label2.TabIndex = 3;
            this.label2.Text = "&Height:";
            // 
            // textBoxWidth
            // 
            this.textBoxWidth.Location = new System.Drawing.Point(115, 206);
            this.textBoxWidth.Name = "textBoxWidth";
            this.textBoxWidth.Size = new System.Drawing.Size(160, 26);
            this.textBoxWidth.TabIndex = 2;
            // 
            // textBoxHeight
            // 
            this.textBoxHeight.Location = new System.Drawing.Point(115, 241);
            this.textBoxHeight.Name = "textBoxHeight";
            this.textBoxHeight.Size = new System.Drawing.Size(160, 26);
            this.textBoxHeight.TabIndex = 4;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(166, 316);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(116, 33);
            this.buttonCancel.TabIndex = 6;
            this.buttonCancel.Text = "&Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(38, 316);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(120, 33);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // SizeForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(335, 297);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.textBoxHeight);
            this.Controls.Add(this.textBoxWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBoxSizes);
            this.Name = "SizeForm";
            this.Text = "Resolution";
            this.Load += new System.EventHandler(this.SizeForm_Load);
            this.groupBoxSizes.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void radioButtonSizeOfWindow_CheckedChanged(object sender, System.EventArgs e)
		{
			this.textBoxWidth.Text = this._clientSize.Width.ToString();
			this.textBoxHeight.Text = this._clientSize.Height.ToString();
		}

		private void radioButtonSmall_CheckedChanged(object sender, System.EventArgs e)
		{
			int width = 720;
			int height = 480;
			this.textBoxWidth.Text = width.ToString();
			this.textBoxHeight.Text = height.ToString();
		}

		private void radioButtonLarge_CheckedChanged(object sender, System.EventArgs e)
		{
			int width = 3840;
			int height = 2160;
			this.textBoxWidth.Text = width.ToString();
			this.textBoxHeight.Text = height.ToString();
		}

		private void radioButtonCustom_CheckedChanged(object sender, System.EventArgs e)
		{
			this.textBoxWidth.Text = this._size.Width.ToString();
			this.textBoxHeight.Text = this._size.Height.ToString();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			this._size = new Size(Convert.ToInt32(this.textBoxWidth.Text),
				Convert.ToInt32(this.textBoxHeight.Text));
		}

		private void SizeForm_Load(object sender, System.EventArgs e)
		{
			radioButtonCustom_CheckedChanged(sender, e);
		}

		public Size SizeEntered
		{
			get { return this._size; }
		}
	}
}
