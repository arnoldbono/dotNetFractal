using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace dotNetFractal
{

	public class RectangleForm : System.Windows.Forms.Form
	{
		private DisplayArea _area;
		private string _text1;
		private string _text2;
		private string _text3;
		private string _text4;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ComboBox comboBoxPlates;
		private System.Windows.Forms.RadioButton _radioButtonMinMax;
        private RadioButton radioButtonCenter;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RectangleForm(DisplayArea area)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			_area = area;
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonCenter = new System.Windows.Forms.RadioButton();
            this._radioButtonMinMax = new System.Windows.Forms.RadioButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.comboBoxPlates = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(13, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Min &X:";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(13, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Min &Y:";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(13, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 24);
            this.label3.TabIndex = 4;
            this.label3.Text = "&Max X:";
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(13, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 23);
            this.label4.TabIndex = 6;
            this.label4.Text = "M&ax Y:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonCenter);
            this.groupBox1.Controls.Add(this._radioButtonMinMax);
            this.groupBox1.Location = new System.Drawing.Point(13, 175);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 119);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Meth&od of Specification";
            // 
            // radioButtonCenter
            // 
            this.radioButtonCenter.Location = new System.Drawing.Point(26, 70);
            this.radioButtonCenter.Name = "radioButtonCenter";
            this.radioButtonCenter.Size = new System.Drawing.Size(268, 26);
            this.radioButtonCenter.TabIndex = 2;
            this.radioButtonCenter.Text = "&Center, Maginification";
            this.radioButtonCenter.CheckedChanged += new System.EventHandler(this.radioButtonCenter_CheckedChanged);
            // 
            // _radioButtonMinMax
            // 
            this._radioButtonMinMax.Checked = true;
            this._radioButtonMinMax.Location = new System.Drawing.Point(26, 35);
            this._radioButtonMinMax.Name = "_radioButtonMinMax";
            this._radioButtonMinMax.Size = new System.Drawing.Size(243, 26);
            this._radioButtonMinMax.TabIndex = 0;
            this._radioButtonMinMax.TabStop = true;
            this._radioButtonMinMax.Text = "M&inimum and Maximum";
            this._radioButtonMinMax.CheckedChanged += new System.EventHandler(this._radioButtonMinMax_CheckedChanged);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(179, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(217, 26);
            this.textBox1.TabIndex = 1;
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(179, 47);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(217, 26);
            this.textBox2.TabIndex = 3;
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(179, 94);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(217, 26);
            this.textBox3.TabIndex = 5;
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBox4
            // 
            this.textBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox4.Location = new System.Drawing.Point(179, 129);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(217, 26);
            this.textBox4.TabIndex = 7;
            this.textBox4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // comboBoxPlates
            // 
            this.comboBoxPlates.Location = new System.Drawing.Point(230, 303);
            this.comboBoxPlates.Name = "comboBoxPlates";
            this.comboBoxPlates.Size = new System.Drawing.Size(154, 28);
            this.comboBoxPlates.TabIndex = 10;
            this.comboBoxPlates.SelectedIndexChanged += new System.EventHandler(this.comboBoxPlates_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(13, 305);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(115, 24);
            this.label5.TabIndex = 9;
            this.label5.Text = "Pla&te:";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(141, 403);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(120, 34);
            this.buttonOK.TabIndex = 11;
            this.buttonOK.Text = "&OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(269, 403);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(115, 34);
            this.buttonCancel.TabIndex = 12;
            this.buttonCancel.Text = "&Cancel";
            // 
            // RectangleForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(409, 466);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.comboBoxPlates);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RectangleForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "RectangleForm";
            this.Load += new System.EventHandler(this.RectangleForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void RectangleForm_Load(object sender, System.EventArgs e)
		{
			comboBoxPlates.Items.Clear();
			foreach(FractalPlate plate in plates)
			{
				comboBoxPlates.Items.Add(plate.name);
			}

			this._text1 = this.label1.Text;
			this._text2 = this.label2.Text;
			this._text3 = this.label3.Text;
			this._text4 = this.label4.Text;

			// PRE: this._radioButtonMinMax.Checked = true;
			// PRE: this._radioButtonPosWidthHeight.Checked = false;

			setTexts();
		}

		public static FractalPlate[] plates =
		{
			new FractalPlate( "4",  -2.0,      -1.25,      0.5,      1.25),
			new FractalPlate( "5",  -0.702973,  0.374785, -0.642879, 0.395415),
			new FractalPlate( "5a", -0.691594,  0.386608, -0.690089, 0.387494),
			new FractalPlate( "6",  -0.691060,  0.387103, -0.690906, 0.387228),
			new FractalPlate( "7",  -0.793114,  0.037822, -0.723005, 0.140974),
			new FractalPlate( "7a", -0.749337,  0.109349, -0.744948, 0.115851),
			new FractalPlate( "8",  -0.745465,  0.112896, -0.745387, 0.113034),
			new FractalPlate( "9",  -0.745464,  0.112967, -0.745388, 0.113030)
		};

		private void comboBoxPlates_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string name = this.comboBoxPlates.SelectedItem.ToString();
			foreach(FractalPlate plate in plates)
			{
				if (name.CompareTo(plate.name) == 0)
				{
					setTexts(plate.minX, plate.minY, plate.maxX - plate.minX, plate.maxY - plate.minY);
					break;
				}
			}
			getTexts(); // store in this._area
		}

		private void setTexts(double Left, double Bottom, double Width, double Height)
		{
			double value1 = Left;
			double value2 = Bottom;
            double value3 = Width;
            double value4 = Height;

			if (this._radioButtonMinMax.Checked)
			{
				value3 = Left + Width;
				value4 = Bottom + Height;
			}
            else
            {
                value1 += Width / 2.0;
                value2 += Height / 2.0;
            }

			this.textBox1.Text = value1.ToString();
			this.textBox2.Text = value2.ToString();
			this.textBox3.Text = value3.ToString();
			this.textBox4.Text = value4.ToString();
		}

		private void setTexts()
		{
			setTexts(this._area.Left, this._area.Bottom, this._area.Width, this._area.Height);
		}

		private void getTexts()
		{
			double value1 = Convert.ToDouble(this.textBox1.Text);
			double value2 = Convert.ToDouble(this.textBox2.Text);
			double value3 = Convert.ToDouble(this.textBox3.Text);
			double value4 = Convert.ToDouble(this.textBox4.Text);
            if (this._radioButtonMinMax.Checked)
            {
                this._area =
                    new DisplayArea((value1 + value3) / 2.0, (value2 + value4) / 2.0,
                        value3 - value1, value4 - value2,
                        this._area.PixelsHorizontal, this._area.PixelsVertical);
            }
            else
            {
                this._area =
                    new DisplayArea(value1, value2, value3, value4,
                        this._area.PixelsHorizontal, this._area.PixelsVertical);
            }
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			getTexts();
		}

		private void _radioButtonMinMax_CheckedChanged(object sender, System.EventArgs e)
		{
			if (this._radioButtonMinMax.Checked)
			{
				this.label1.Text = this._text1;
				this.label2.Text = this._text2;
				this.label3.Text = this._text3;
				this.label4.Text = this._text4;
			    setTexts();
			}
		}

		public DisplayArea area
		{
			get { return this._area; }
		}

        private void radioButtonCenter_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCenter.Checked)
            {
                this.label1.Text = "Pos &X:";
                this.label2.Text = "Pos &Y:";
                this.label3.Text = "&Width:";
                this.label4.Text = "&Height:";
                setTexts();
            }
        }
	}
}
