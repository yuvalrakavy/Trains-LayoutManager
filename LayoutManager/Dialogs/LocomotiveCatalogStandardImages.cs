using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.Dialogs
{
	/// <summary>
	/// Summary description for LocomotiveCatalogStandardImages.
	/// </summary>
	public class LocomotiveCatalogStandardImages : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.PictureBox pictureBoxES;
		private System.Windows.Forms.PictureBox pictureBoxED;
		private System.Windows.Forms.PictureBox pictureBoxEE;
		private System.Windows.Forms.Button buttonSetES;
		private System.Windows.Forms.Button buttonClearES;
		private System.Windows.Forms.Button buttonClearED;
		private System.Windows.Forms.Button buttonSetED;
		private System.Windows.Forms.Button buttonClearEE;
		private System.Windows.Forms.Button buttonSetEE;
		private System.Windows.Forms.Button buttonSetUS;
		private System.Windows.Forms.PictureBox pictureBoxUS;
		private System.Windows.Forms.PictureBox pictureBoxUD;
		private System.Windows.Forms.PictureBox pictureBoxUE;
		private System.Windows.Forms.Button buttonClearUS;
		private System.Windows.Forms.Button buttonClearUD;
		private System.Windows.Forms.Button buttonSetUD;
		private System.Windows.Forms.Button buttonClearUE;
		private System.Windows.Forms.Button buttonSetUE;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		class ImageEntry {
			public PictureBox	picBox;
			public Button		setButton;
			public Button		clearButton;

			public ImageEntry(PictureBox picBox, Button setButton, Button clearButton) {
				this.picBox = picBox;
				this.setButton = setButton;
				this.clearButton = clearButton;
			}
		}

		LocomotiveCatalogInfo	catalog;
		ImageEntry[,]				imageTable;
		LocomotiveOrigin[]			origins = new LocomotiveOrigin[] { LocomotiveOrigin.Europe, LocomotiveOrigin.US };
		LocomotiveKind[]			kinds = new LocomotiveKind[] { LocomotiveKind.Steam, LocomotiveKind.Diesel, LocomotiveKind.Electric, LocomotiveKind.SoundUnit };
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button buttonSetESU;
		private System.Windows.Forms.PictureBox pictureBoxESU;
		private System.Windows.Forms.Button buttonClearESU;
		private System.Windows.Forms.Button buttonSetUSU;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.PictureBox pictureBoxUSU;
		private System.Windows.Forms.Button buttonClearUSU;
		bool[,]						modified;

		public LocomotiveCatalogStandardImages(LocomotiveCatalogInfo catalog) {
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			this.catalog = catalog;

			imageTable = new ImageEntry[,] { {
				new ImageEntry(pictureBoxES, buttonSetES, buttonClearES),
				new ImageEntry(pictureBoxED, buttonSetED, buttonClearED),
				new ImageEntry(pictureBoxEE, buttonSetEE, buttonClearEE),
				new ImageEntry(pictureBoxESU,buttonSetESU,buttonClearESU),
			 }, {
				new ImageEntry(pictureBoxUS, buttonSetUS, buttonClearUS),
				new ImageEntry(pictureBoxUD, buttonSetUD, buttonClearUD),
				new ImageEntry(pictureBoxUE, buttonSetUE, buttonClearUE),
				new ImageEntry(pictureBoxUSU,buttonSetUSU,buttonClearUSU),
			} };

			modified = new bool[origins.Length, kinds.Length];

			for(int iOrigin = 0; iOrigin < origins.Length; iOrigin++) {
				for(int iKind = 0; iKind < kinds.Length; iKind++) {
					ImageEntry	entry = imageTable[iOrigin, iKind];

					entry.picBox.Image = catalog.GetStandardImage(kinds[iKind], origins[iOrigin]);
				}
			}
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.pictureBoxES = new System.Windows.Forms.PictureBox();
			this.pictureBoxED = new System.Windows.Forms.PictureBox();
			this.pictureBoxEE = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.buttonSetES = new System.Windows.Forms.Button();
			this.buttonClearES = new System.Windows.Forms.Button();
			this.buttonClearED = new System.Windows.Forms.Button();
			this.buttonSetED = new System.Windows.Forms.Button();
			this.buttonClearEE = new System.Windows.Forms.Button();
			this.buttonSetEE = new System.Windows.Forms.Button();
			this.buttonSetUS = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.pictureBoxUS = new System.Windows.Forms.PictureBox();
			this.pictureBoxUD = new System.Windows.Forms.PictureBox();
			this.pictureBoxUE = new System.Windows.Forms.PictureBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.buttonClearUS = new System.Windows.Forms.Button();
			this.buttonClearUD = new System.Windows.Forms.Button();
			this.buttonSetUD = new System.Windows.Forms.Button();
			this.buttonClearUE = new System.Windows.Forms.Button();
			this.buttonSetUE = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonSetESU = new System.Windows.Forms.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.pictureBoxESU = new System.Windows.Forms.PictureBox();
			this.buttonClearESU = new System.Windows.Forms.Button();
			this.buttonSetUSU = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.pictureBoxUSU = new System.Windows.Forms.PictureBox();
			this.buttonClearUSU = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.buttonSetES,
																					this.label1,
																					this.pictureBoxES,
																					this.pictureBoxED,
																					this.pictureBoxEE,
																					this.label2,
																					this.label3,
																					this.buttonClearES,
																					this.buttonClearED,
																					this.buttonSetED,
																					this.buttonClearEE,
																					this.buttonSetEE,
																					this.buttonSetESU,
																					this.label7,
																					this.pictureBoxESU,
																					this.buttonClearESU});
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(480, 112);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "European Locomotives:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.AddRange(new System.Windows.Forms.Control[] {
																					this.buttonSetUSU,
																					this.label8,
																					this.pictureBoxUSU,
																					this.buttonClearUSU,
																					this.buttonSetUS,
																					this.label4,
																					this.pictureBoxUS,
																					this.pictureBoxUD,
																					this.pictureBoxUE,
																					this.label5,
																					this.label6,
																					this.buttonClearUS,
																					this.buttonClearUD,
																					this.buttonSetUD,
																					this.buttonClearUE,
																					this.buttonSetUE});
			this.groupBox2.Location = new System.Drawing.Point(8, 128);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(480, 112);
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "American Locomotives:";
			// 
			// pictureBoxES
			// 
			this.pictureBoxES.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxES.Location = new System.Drawing.Point(8, 32);
			this.pictureBoxES.Name = "pictureBoxES";
			this.pictureBoxES.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxES.TabIndex = 0;
			this.pictureBoxES.TabStop = false;
			// 
			// pictureBoxED
			// 
			this.pictureBoxED.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxED.Location = new System.Drawing.Point(128, 32);
			this.pictureBoxED.Name = "pictureBoxED";
			this.pictureBoxED.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxED.TabIndex = 0;
			this.pictureBoxED.TabStop = false;
			// 
			// pictureBoxEE
			// 
			this.pictureBoxEE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxEE.Location = new System.Drawing.Point(248, 32);
			this.pictureBoxEE.Name = "pictureBoxEE";
			this.pictureBoxEE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxEE.TabIndex = 0;
			this.pictureBoxEE.TabStop = false;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Steam";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(128, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 1;
			this.label2.Text = "Diesel";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(248, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 1;
			this.label3.Text = "Electric";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonSetES
			// 
			this.buttonSetES.Location = new System.Drawing.Point(8, 86);
			this.buttonSetES.Name = "buttonSetES";
			this.buttonSetES.Size = new System.Drawing.Size(48, 19);
			this.buttonSetES.TabIndex = 2;
			this.buttonSetES.Text = "Set";
			this.buttonSetES.Click += new System.EventHandler(this.buttonSet_click);
			// 
			// buttonClearES
			// 
			this.buttonClearES.Location = new System.Drawing.Point(60, 86);
			this.buttonClearES.Name = "buttonClearES";
			this.buttonClearES.Size = new System.Drawing.Size(48, 19);
			this.buttonClearES.TabIndex = 2;
			this.buttonClearES.Text = "Clear";
			this.buttonClearES.Click += new System.EventHandler(this.buttonClear_click);
			// 
			// buttonClearED
			// 
			this.buttonClearED.Location = new System.Drawing.Point(180, 86);
			this.buttonClearED.Name = "buttonClearED";
			this.buttonClearED.Size = new System.Drawing.Size(48, 19);
			this.buttonClearED.TabIndex = 2;
			this.buttonClearED.Text = "Clear";
			this.buttonClearED.Click += new System.EventHandler(this.buttonClear_click);
			// 
			// buttonSetED
			// 
			this.buttonSetED.Location = new System.Drawing.Point(128, 86);
			this.buttonSetED.Name = "buttonSetED";
			this.buttonSetED.Size = new System.Drawing.Size(48, 19);
			this.buttonSetED.TabIndex = 2;
			this.buttonSetED.Text = "Set";
			this.buttonSetED.Click += new System.EventHandler(this.buttonSet_click);
			// 
			// buttonClearEE
			// 
			this.buttonClearEE.Location = new System.Drawing.Point(300, 86);
			this.buttonClearEE.Name = "buttonClearEE";
			this.buttonClearEE.Size = new System.Drawing.Size(48, 19);
			this.buttonClearEE.TabIndex = 2;
			this.buttonClearEE.Text = "Clear";
			this.buttonClearEE.Click += new System.EventHandler(this.buttonClear_click);
			// 
			// buttonSetEE
			// 
			this.buttonSetEE.Location = new System.Drawing.Point(248, 86);
			this.buttonSetEE.Name = "buttonSetEE";
			this.buttonSetEE.Size = new System.Drawing.Size(48, 19);
			this.buttonSetEE.TabIndex = 2;
			this.buttonSetEE.Text = "Set";
			this.buttonSetEE.Click += new System.EventHandler(this.buttonSet_click);
			// 
			// buttonSetUS
			// 
			this.buttonSetUS.Location = new System.Drawing.Point(10, 84);
			this.buttonSetUS.Name = "buttonSetUS";
			this.buttonSetUS.Size = new System.Drawing.Size(48, 19);
			this.buttonSetUS.TabIndex = 12;
			this.buttonSetUS.Text = "Set";
			this.buttonSetUS.Click += new System.EventHandler(this.buttonSet_click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(10, 14);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 8;
			this.label4.Text = "Steam";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pictureBoxUS
			// 
			this.pictureBoxUS.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxUS.Location = new System.Drawing.Point(10, 30);
			this.pictureBoxUS.Name = "pictureBoxUS";
			this.pictureBoxUS.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxUS.TabIndex = 5;
			this.pictureBoxUS.TabStop = false;
			// 
			// pictureBoxUD
			// 
			this.pictureBoxUD.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxUD.Location = new System.Drawing.Point(130, 30);
			this.pictureBoxUD.Name = "pictureBoxUD";
			this.pictureBoxUD.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxUD.TabIndex = 3;
			this.pictureBoxUD.TabStop = false;
			// 
			// pictureBoxUE
			// 
			this.pictureBoxUE.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxUE.Location = new System.Drawing.Point(250, 30);
			this.pictureBoxUE.Name = "pictureBoxUE";
			this.pictureBoxUE.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxUE.TabIndex = 4;
			this.pictureBoxUE.TabStop = false;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(130, 14);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 7;
			this.label5.Text = "Diesel";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(250, 14);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 16);
			this.label6.TabIndex = 6;
			this.label6.Text = "Electric";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonClearUS
			// 
			this.buttonClearUS.Location = new System.Drawing.Point(62, 84);
			this.buttonClearUS.Name = "buttonClearUS";
			this.buttonClearUS.Size = new System.Drawing.Size(48, 19);
			this.buttonClearUS.TabIndex = 13;
			this.buttonClearUS.Text = "Clear";
			this.buttonClearUS.Click += new System.EventHandler(this.buttonClear_click);
			// 
			// buttonClearUD
			// 
			this.buttonClearUD.Location = new System.Drawing.Point(182, 84);
			this.buttonClearUD.Name = "buttonClearUD";
			this.buttonClearUD.Size = new System.Drawing.Size(48, 19);
			this.buttonClearUD.TabIndex = 14;
			this.buttonClearUD.Text = "Clear";
			this.buttonClearUD.Click += new System.EventHandler(this.buttonClear_click);
			// 
			// buttonSetUD
			// 
			this.buttonSetUD.Location = new System.Drawing.Point(130, 84);
			this.buttonSetUD.Name = "buttonSetUD";
			this.buttonSetUD.Size = new System.Drawing.Size(48, 19);
			this.buttonSetUD.TabIndex = 9;
			this.buttonSetUD.Text = "Set";
			this.buttonSetUD.Click += new System.EventHandler(this.buttonSet_click);
			// 
			// buttonClearUE
			// 
			this.buttonClearUE.Location = new System.Drawing.Point(302, 84);
			this.buttonClearUE.Name = "buttonClearUE";
			this.buttonClearUE.Size = new System.Drawing.Size(48, 19);
			this.buttonClearUE.TabIndex = 10;
			this.buttonClearUE.Text = "Clear";
			this.buttonClearUE.Click += new System.EventHandler(this.buttonClear_click);
			// 
			// buttonSetUE
			// 
			this.buttonSetUE.Location = new System.Drawing.Point(250, 84);
			this.buttonSetUE.Name = "buttonSetUE";
			this.buttonSetUE.Size = new System.Drawing.Size(48, 19);
			this.buttonSetUE.TabIndex = 11;
			this.buttonSetUE.Text = "Set";
			this.buttonSetUE.Click += new System.EventHandler(this.buttonSet_click);
			// 
			// buttonOK
			// 
			this.buttonOK.Location = new System.Drawing.Point(331, 248);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(413, 248);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.TabIndex = 1;
			this.buttonCancel.Text = "Cancel";
			// 
			// buttonSetESU
			// 
			this.buttonSetESU.Location = new System.Drawing.Point(368, 86);
			this.buttonSetESU.Name = "buttonSetESU";
			this.buttonSetESU.Size = new System.Drawing.Size(48, 19);
			this.buttonSetESU.TabIndex = 2;
			this.buttonSetESU.Text = "Set";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(368, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 16);
			this.label7.TabIndex = 1;
			this.label7.Text = "Sound Unit";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pictureBoxESU
			// 
			this.pictureBoxESU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxESU.Location = new System.Drawing.Point(368, 32);
			this.pictureBoxESU.Name = "pictureBoxESU";
			this.pictureBoxESU.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxESU.TabIndex = 0;
			this.pictureBoxESU.TabStop = false;
			// 
			// buttonClearESU
			// 
			this.buttonClearESU.Location = new System.Drawing.Point(420, 86);
			this.buttonClearESU.Name = "buttonClearESU";
			this.buttonClearESU.Size = new System.Drawing.Size(48, 19);
			this.buttonClearESU.TabIndex = 2;
			this.buttonClearESU.Text = "Clear";
			// 
			// buttonSetUSU
			// 
			this.buttonSetUSU.Location = new System.Drawing.Point(368, 84);
			this.buttonSetUSU.Name = "buttonSetUSU";
			this.buttonSetUSU.Size = new System.Drawing.Size(48, 19);
			this.buttonSetUSU.TabIndex = 18;
			this.buttonSetUSU.Text = "Set";
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(368, 14);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 16);
			this.label8.TabIndex = 16;
			this.label8.Text = "Sound Unit";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// pictureBoxUSU
			// 
			this.pictureBoxUSU.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxUSU.Location = new System.Drawing.Point(368, 30);
			this.pictureBoxUSU.Name = "pictureBoxUSU";
			this.pictureBoxUSU.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.pictureBoxUSU.TabIndex = 15;
			this.pictureBoxUSU.TabStop = false;
			// 
			// buttonClearUSU
			// 
			this.buttonClearUSU.Location = new System.Drawing.Point(420, 84);
			this.buttonClearUSU.Name = "buttonClearUSU";
			this.buttonClearUSU.Size = new System.Drawing.Size(48, 19);
			this.buttonClearUSU.TabIndex = 17;
			this.buttonClearUSU.Text = "Clear";
			// 
			// LocomotiveCatalogStandardImages
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(493, 278);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.buttonOK,
																		  this.groupBox1,
																		  this.groupBox2,
																		  this.buttonCancel});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "LocomotiveCatalogStandardImages";
			this.ShowInTaskbar = false;
			this.Text = "Standard Locomotive Images";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		protected void buttonSet_click(Object sender, EventArgs e) {
			for(int iOrigin = 0; iOrigin < origins.Length; iOrigin++) {
				for(int iKind = 0; iKind < kinds.Length; iKind++) {
					ImageEntry	entry = imageTable[iOrigin, iKind];

					if(sender == entry.setButton) {
						FileDialog		fileDialog = new OpenFileDialog();

						fileDialog.AddExtension = true;
						fileDialog.CheckFileExists = true;
						fileDialog.Filter = "Image files (*.jpg,*.bmp,*.gif)|*.jpg;*.bmp;*.gif|All files|*.*";

						if(fileDialog.ShowDialog(this) == DialogResult.OK) {
							try {
								Image	image = Image.FromFile(fileDialog.FileName);

								entry.picBox.Image = image;
								modified[iOrigin, iKind] = true;
							} catch(Exception ex) {
								MessageBox.Show(this, "Error loading image: " + ex.Message, "Image load error", MessageBoxButtons.OK, MessageBoxIcon.Error);
							}
						}
						break;
					}
				}
			}
		}

		protected void buttonClear_click(Object sender, EventArgs e) {
			for(int iOrigin = 0; iOrigin < origins.Length; iOrigin++) {
				for(int iKind = 0; iKind < kinds.Length; iKind++) {
					ImageEntry	entry = imageTable[iOrigin, iKind];

					if(sender == entry.clearButton) {
						entry.picBox.Image = null;
						modified[iOrigin, iKind] = true;
						break;
					}
				}
			}
		}

		private void buttonOK_Click(object sender, System.EventArgs e) {
			for(int iOrigin = 0; iOrigin < origins.Length; iOrigin++) {
				for(int iKind = 0; iKind < kinds.Length; iKind++) {
					ImageEntry	entry = imageTable[iOrigin, iKind];

					if(modified[iOrigin, iKind])
						catalog.SetStandardImage(kinds[iKind], origins[iOrigin], entry.picBox.Image);
				}
			}

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
