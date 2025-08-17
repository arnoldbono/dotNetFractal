using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace dotNetFractal
{
	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
	{
        private FractalStitcher m_stitcher = null;
        private Bitmap m_bitmap = null;
        private Point _first;
		private Point _last;
		private bool _selecting = false;

		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItemEdit;
		private System.Windows.Forms.MenuItem menuItemSetup;
		private System.Windows.Forms.MenuItem menuItemNew;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuItemExit;
		private System.Windows.Forms.MenuItem menuItemCopy;
		private System.Windows.Forms.MenuItem menuItemBoundingBox;
		private System.Windows.Forms.MenuItem menuItemResolution;
		private System.Windows.Forms.Panel pnlImage;
		private System.Windows.Forms.PictureBox _pictureBox;
		private System.Windows.Forms.Timer timerUpdate;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
            this.components = new System.ComponentModel.Container();
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItemFile = new System.Windows.Forms.MenuItem();
            this.menuItemNew = new System.Windows.Forms.MenuItem();
            this.menuItemSave = new System.Windows.Forms.MenuItem();
            this.menuItemExit = new System.Windows.Forms.MenuItem();
            this.menuItemEdit = new System.Windows.Forms.MenuItem();
            this.menuItemCopy = new System.Windows.Forms.MenuItem();
            this.menuItemSetup = new System.Windows.Forms.MenuItem();
            this.menuItemBoundingBox = new System.Windows.Forms.MenuItem();
            this.menuItemResolution = new System.Windows.Forms.MenuItem();
            this.pnlImage = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this._pictureBox = new System.Windows.Forms.PictureBox();
            this.timerUpdate = new System.Windows.Forms.Timer(this.components);
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.pnlImage.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemFile,
            this.menuItemEdit,
            this.menuItemSetup});
            // 
            // menuItemFile
            // 
            this.menuItemFile.Index = 0;
            this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemNew,
            this.menuItemSave,
            this.menuItemExit});
            this.menuItemFile.Text = "&File";
            // 
            // menuItemNew
            // 
            this.menuItemNew.Index = 0;
            this.menuItemNew.Text = "&New";
            this.menuItemNew.Click += new System.EventHandler(this.menuItemNew_Click);
            // 
            // menuItemSave
            // 
            this.menuItemSave.Index = 1;
            this.menuItemSave.Text = "&Save...";
            this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
            // 
            // menuItemExit
            // 
            this.menuItemExit.Index = 2;
            this.menuItemExit.Shortcut = System.Windows.Forms.Shortcut.AltF4;
            this.menuItemExit.Text = "&Exit";
            this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
            // 
            // menuItemEdit
            // 
            this.menuItemEdit.Index = 1;
            this.menuItemEdit.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemCopy});
            this.menuItemEdit.Text = "&Edit";
            // 
            // menuItemCopy
            // 
            this.menuItemCopy.Index = 0;
            this.menuItemCopy.Shortcut = System.Windows.Forms.Shortcut.CtrlC;
            this.menuItemCopy.Text = "&Copy";
            this.menuItemCopy.Click += new System.EventHandler(this.menuItemCopy_Click);
            // 
            // menuItemSetup
            // 
            this.menuItemSetup.Index = 2;
            this.menuItemSetup.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemBoundingBox,
            this.menuItemResolution});
            this.menuItemSetup.Text = "&Setup";
            // 
            // menuItemBoundingBox
            // 
            this.menuItemBoundingBox.Index = 0;
            this.menuItemBoundingBox.Text = "&Bounding Box...";
            this.menuItemBoundingBox.Click += new System.EventHandler(this.menuItemBoundingBox_Click);
            // 
            // menuItemResolution
            // 
            this.menuItemResolution.Index = 1;
            this.menuItemResolution.Text = "&Resolution...";
            this.menuItemResolution.Click += new System.EventHandler(this.menuItemResolution_Click);
            // 
            // pnlImage
            // 
            this.pnlImage.AutoScroll = true;
            this.pnlImage.Controls.Add(this.statusStrip1);
            this.pnlImage.Controls.Add(this._pictureBox);
            this.pnlImage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlImage.Location = new System.Drawing.Point(0, 0);
            this.pnlImage.Name = "pnlImage";
            this.pnlImage.Size = new System.Drawing.Size(309, 86);
            this.pnlImage.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 54);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(309, 32);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(75, 25);
            this.toolStripStatusLabel1.Text = "Position";
            // 
            // _pictureBox
            // 
            this._pictureBox.Location = new System.Drawing.Point(0, 0);
            this._pictureBox.Name = "_pictureBox";
            this._pictureBox.Size = new System.Drawing.Size(100, 50);
            this._pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._pictureBox.TabIndex = 0;
            this._pictureBox.TabStop = false;
            this._pictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this._pictureBox_MouseDown);
            this._pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this._pictureBox_MouseMove);
            this._pictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this._pictureBox_MouseUp);
            // 
            // timerUpdate
            // 
            this.timerUpdate.Interval = 20;
            this.timerUpdate.Tick += new System.EventHandler(this.timerUpdate_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(8, 19);
            this.ClientSize = new System.Drawing.Size(309, 86);
            this.Controls.Add(this.pnlImage);
            this.Menu = this.mainMenu1;
            this.Name = "MainForm";
            this.Text = ".NET Fractal";
            this.Closed += new System.EventHandler(this.MainForm_Closed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.pnlImage.ResumeLayout(false);
            this.pnlImage.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._pictureBox)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

        private void NewFractal(bool force)
        {
            if (force || m_stitcher == null)
            {
                m_stitcher =
                    new FractalStitcher(() => new FractalMandelbrot(),
                        new DisplayArea(-0.75, 0.0, 2.5, 2.5, 300, 300));
            }
        }

		private void MainForm_Load(object sender, System.EventArgs e)
		{
            NewFractal(true);
            timerUpdate.Enabled = true;
		}

		private void menuItemNew_Click(object sender, System.EventArgs e)
		{
            NewFractal(true);
            m_stitcher.StartThread();
		}

		private void timerUpdate_Tick(object sender, System.EventArgs e)
		{
            if (m_stitcher == null) return;

            int width = m_stitcher.Area.PixelsHorizontal;
            int height = m_stitcher.Area.PixelsVertical;
            if ((m_bitmap == null) ||
                (m_bitmap.Width != width) ||
                (m_bitmap.Height != height))
            {
                m_bitmap = m_stitcher.GetBitmap(width, height);
                m_stitcher.DefaultFill(m_bitmap);
            }

            if (m_bitmap == null)
            {
                return;
            }

            if ((_pictureBox.Image == null) ||
                (_pictureBox.Image.Width != m_bitmap.Width) ||
                (_pictureBox.Image.Height != m_bitmap.Height))
            {
                _pictureBox.Image = new Bitmap(m_bitmap);
            }

            var rect = m_stitcher.Update(m_bitmap);
            if (rect != null)
            {
                Graphics grfx = Graphics.FromImage(_pictureBox.Image);
                grfx.Clip = new Region(rect);
                grfx.DrawImage(m_bitmap, new Point(0, 0));
                _pictureBox.Invalidate();
            }
		}

		private void menuItemBoundingBox_Click(object sender, System.EventArgs e)
		{
            Debug.Assert(m_stitcher != null);
            m_stitcher.StopThread();
            var dlg = new RectangleForm(m_stitcher.Area);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
                m_stitcher.DefaultFill(m_bitmap);
                m_stitcher.Area = dlg.area;
                m_stitcher.StartThread();
			}
		}

		private void menuItemResolution_Click(object sender, System.EventArgs e)
		{
            Debug.Assert(m_stitcher != null);
            m_stitcher.StopThread();
            var dlg = new SizeForm(new Size(m_stitcher.Area.PixelsHorizontal, m_stitcher.Area.PixelsVertical), ClientSize);
			if (dlg.ShowDialog(this) == DialogResult.OK)
			{
                m_stitcher.DefaultFill(m_bitmap);
                var area = m_stitcher.Area;
                m_stitcher.Area = new DisplayArea(area.CenterX, area.CenterY, area.Width, area.Height, dlg.SizeEntered.Width, dlg.SizeEntered.Height);
                m_stitcher.StartThread();
			}
		}

		private void menuItemExit_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void MainForm_Closed(object sender, System.EventArgs e)
		{
            if (m_stitcher != null)
            {
                m_stitcher.StopThread();
            }
		}

		private void _pictureBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ((_pictureBox.Image != null) && ((e.Button & MouseButtons.Left) != 0))
			{
				_first = new Point(e.X, e.Y);
				_last = _first;
				_selecting = true;
			}
		}

		private void _pictureBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
            if (m_stitcher == null) return;
            if (_selecting)
			{
                // _pictureBox.Image != null

				_last = new Point(e.X, e.Y);

                if (_last.X > _first.X && _last.Y > _first.Y)
                {
                    // Copy the original image (to remove old rectangle)
                    Graphics grfx = Graphics.FromImage(_pictureBox.Image);

                    m_stitcher.LockMutex();
                    grfx.DrawImage(m_bitmap, new Point(0, 0));
                    m_stitcher.UnlockMutex();

                    int width = _last.X - _first.X;
                    int height = _last.Y - _first.Y;
                    if (width < height)
                    {
                        width = height;
                    }
                    grfx.DrawRectangle(new Pen(new SolidBrush(Color.Black), 1),
                        _first.X, _first.Y, width + 1, width + 1);

                    _pictureBox.Invalidate();

                    toolStripStatusLabel1.Text =
                        "[" + e.X.ToString() + "," + e.X.ToString() + "] => (" +
                        m_stitcher.Area.GetWidth(width).ToString() + ")";

                    this.statusStrip1.Invalidate();
                }
            }
            else
            {
                toolStripStatusLabel1.Text =
                    "[" + e.X.ToString() + "," + e.X.ToString() + "] => (" +
                    m_stitcher.Area.GetX(e.X).ToString() + "," +
                    m_stitcher.Area.GetY(e.Y).ToString() + ")";

                this.statusStrip1.Invalidate();
            }
		}

		private void _pictureBox_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (_selecting && _pictureBox.Image != null)
			{
				_last = new Point(e.X, e.Y);
				_selecting = false;

                if (_last.X > _first.X && _last.Y > _first.Y)
				{
					m_stitcher.StopThread();

                    int width = _last.X - _first.X;
                    int height = _last.Y - _first.Y;
                    if (width < height)
                    {
                        width = height;
                    }

                    DisplayArea area = m_stitcher.Area;

                    double x = area.GetX(_first.X);
                    double y = area.GetY(_first.Y);
                    double w = area.GetWidth(width + 1);

                    m_stitcher =
                        new FractalStitcher(() => new FractalMandelbrot(),
                            new DisplayArea(x + w / 2, y - w / 2, w, w, area.PixelsHorizontal, area.PixelsVertical));
                    m_stitcher.StartThread();

				    _pictureBox.Invalidate();
                }
			}
			else if (((e.Button & MouseButtons.Right) != 0) &&
                //					(_Stitcher.FractalFactory is FractalFactoryMandelbrot) &&
                    (MessageBox.Show("Create Julia Set?", ".NET Fractal", MessageBoxButtons.OKCancel) == DialogResult.OK))
			{
				double x, y;
				m_stitcher.Area.GetPosition(e.X, e.Y, out x, out y);

                m_stitcher.StopThread();

				FractalJulia fractal = new FractalJulia();
				fractal.SetStartingPoint(x, y);
				m_stitcher = new FractalStitcher(() => new FractalJulia(), m_stitcher.Area);
                fractal.StartThread();
			}
		}

		private void menuItemCopy_Click(object sender, System.EventArgs e)
		{
            m_stitcher.LockMutex();
			Clipboard.SetDataObject(m_bitmap, true);
            m_stitcher.UnlockMutex();
		}

		private void menuItemSave_Click(object sender, System.EventArgs e)
		{
			saveFileDialog1.Filter = "JPeg Image|*.jpg|Bitmap Image|*.bmp";
			saveFileDialog1.Title = "Save an Image File";
			if (saveFileDialog1.ShowDialog() == DialogResult.OK)
			{
                m_stitcher.LockMutex();
				if(saveFileDialog1.FileName != "")
				{
					// Saves the Image via a FileStream created by the OpenFile method.
					System.IO.FileStream fs = 
						(System.IO.FileStream)saveFileDialog1.OpenFile();
					// Saves the Image in the appropriate ImageFormat based upon the
					// File type selected in the dialog box.
					// NOTE that the FilterIndex property is one-based.
					switch(saveFileDialog1.FilterIndex)
					{
						case 1 : 
							m_bitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Jpeg);
							break;

						case 2 :
                            m_bitmap.Save(fs, System.Drawing.Imaging.ImageFormat.Bmp);
							break;
					}

					fs.Close();
				}
                m_stitcher.UnlockMutex();
			}
		}
    }
}
