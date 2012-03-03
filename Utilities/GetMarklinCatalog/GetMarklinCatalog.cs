using System;
using System.Drawing;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Data;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace GetMarklinCatalog
{
	public interface ILocomotiveItemsContainer {
		XmlElement ContainerElement {
			get;
		}

		int Version {
			get;
		}
	}

	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class GetMarklinCatalog : System.Windows.Forms.Form, ILocomotiveItemsContainer
	{
		private System.Windows.Forms.ListView listViewLocomotives;
		private System.Windows.Forms.ColumnHeader columnHeaderName;
		private CroppablePictureBox pictureBoxImage;
		private System.Windows.Forms.Label labelImageDetails;
		private System.Windows.Forms.MenuItem menuItemFile;
		private System.Windows.Forms.MenuItem menuItemLoad;
		private System.Windows.Forms.MenuItem menuItemSave;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemExit;
		private System.Windows.Forms.OpenFileDialog openFileDialogLoad;
		private System.Windows.Forms.SaveFileDialog saveFileDialogSave;
		private System.Windows.Forms.MenuItem menuItemDownload;
		private System.Windows.Forms.RadioButton radioButtonElectric;
		private System.Windows.Forms.RadioButton radioButtonSteam;
		private System.Windows.Forms.RadioButton radioButtonDiesel;
		private System.Windows.Forms.GroupBox groupBoxKind;
		private System.Windows.Forms.Button buttonSetImageUrl;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItemTranslationsEdit;
		private System.Windows.Forms.MenuItem menuItemTranslationsSave;
		private System.Windows.Forms.MenuItem menuItemTranslationsLoad;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.TextBox textBoxName;
		private System.Windows.Forms.OpenFileDialog openFileDialogTranslations;
		private System.Windows.Forms.SaveFileDialog saveFileDialogTranslations;
		private System.Windows.Forms.MainMenu mainMenu;
		private System.Windows.Forms.SaveFileDialog saveFileDialogCatalog;
		private System.Windows.Forms.MenuItem menuItemGenerateCatalog;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.GroupBox groupBoxOrigin;
		private System.Windows.Forms.RadioButton radioButtonOriginUS;
		private System.Windows.Forms.RadioButton radioButtonOriginEurope;
		private System.Windows.Forms.PictureBox pictureBoxProcessedImage;

		private void endOfDesignerVariables() { }

		string		baseURL = "http://www.maerklin.de";
		const int	fileVersion = 1;
		const int	itemsPerPage = 20;

		XmlDocument	xmlDocument = new XmlDocument();
		XmlElement	containerElement;
		int			currentFileVersion = fileVersion;
		XmlDocument	translationsDocument = new XmlDocument();

		Image		processedImage = null;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public GetMarklinCatalog()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			pictureBoxImage.CroppingRectangleChanged += new EventHandler(this.onCroppingRectangleChange);
			
			containerElement = xmlDocument.CreateElement("LocomotiveCatalog");
			xmlDocument.AppendChild(containerElement);

			translationsDocument.LoadXml("<Translations />");
		}

		public XmlElement ContainerElement {
			get {
				return containerElement;
			}
		}

		public int Version {
			get {
				return currentFileVersion;
			}
		}

		string getIndexPage(int pageNumber) {
			WebClient			web = new WebClient();
			NameValueCollection	fields = new NameValueCollection();
			int	start = pageNumber * itemsPerPage;

			web.BaseAddress = baseURL;
			fields.Add("start", start.ToString());
			fields.Add("page", "list");
			fields.Add("spur", "2");		// HO
			fields.Add("gruppe", "1");		// Locomotives
			fields.Add("untergruppe", "all");
			fields.Add("epoche", "all");
			fields.Add("merkmale", "all");
			fields.Add("show", "list");
			fields.Add("wishednumber", "");

			byte[]	responseBuffer = web.UploadValues("produkte/produkte_cdrom/index.php", "POST", fields);

			return System.Text.Encoding.UTF8.GetString(responseBuffer);
		}

		int parseIndexPage(string indexPageContent) {
			Regex	rTable = new Regex("<table.*>(?s:.*?)</table>", RegexOptions.IgnoreCase);

			Match mTable = rTable.Match(indexPageContent);
			mTable = mTable.NextMatch();
			mTable = mTable.NextMatch();

			Regex	rRow = new Regex("<tr>(?s:.*?)</tr>", RegexOptions.IgnoreCase);
			Match	mRow = rRow.Match(mTable.Value);

			Regex	rIndex = new Regex(".*href=\"(.*)\">(.*)</a>", RegexOptions.IgnoreCase);
			int		nItems = 0;

			while(mRow.Success) {
				Match	mIndex = rIndex.Match(mRow.Value);
				string	itemUrl = mIndex.Groups[1].Value;
				string	itemName = mIndex.Groups[2].Value;

				listViewLocomotives.Items.Add(new LocomotiveItem(this, itemName, itemUrl));
				nItems++;

				mRow = mRow.NextMatch();
			}

			return nItems;
		}

		Image GetProcessedImage(LocomotiveItem item) {
			if(processedImage == null)
				processedImage = item.CreateProcessedImage();
			return processedImage;
		}

		private void onCroppingRectangleChange(object sender, EventArgs e) {
			if(listViewLocomotives.SelectedItems.Count > 0) {
				LocomotiveItem	item = (LocomotiveItem)listViewLocomotives.SelectedItems[0];

				item.CropRectangle = pictureBoxImage.CroppingRectangle;
				item.UpdateProcessedImage(GetProcessedImage(item));
				pictureBoxProcessedImage.Invalidate();
			}
		}

		void showItem(LocomotiveItem item) {
			if(item.Image != null) {
				pictureBoxImage.Image = item.Image;
				pictureBoxImage.CroppingRectangle = item.CropRectangle;
				item.UpdateProcessedImage(GetProcessedImage(item));
				pictureBoxProcessedImage.Image = GetProcessedImage(item);
			}
			else {
				pictureBoxImage.Image = null;
				pictureBoxProcessedImage.Image = null;
			}

			textBoxName.Text = item.TypeName;

			switch(item.Kind) {
				case "Electric":
					radioButtonElectric.Checked = true;
					break;

				case "Steam":
					radioButtonSteam.Checked = true;
					break;

				case "Diesel":
					radioButtonDiesel.Checked = true;
					break;
			}

			switch(item.Origin) {

				case "Europe":
					radioButtonOriginEurope.Checked = true;
					break;

				case "US":
					radioButtonOriginUS.Checked = true;
					break;
			}
		}

		void showSelectedItem() {
			if(listViewLocomotives.SelectedItems.Count > 0) {
				LocomotiveItem	selected = (LocomotiveItem)listViewLocomotives.SelectedItems[0];

				showItem(selected);
			}
		}

		public void SaveFile(string filename) {
			try {
				if(File.Exists(filename)) {
					if(File.Exists(filename + ".Backup"))
						File.Delete(filename + ".Backup");
					File.Move(filename, filename + ".Backup");
				}

				using(FileStream s = new FileStream(filename, FileMode.Create)) {
					using(BinaryWriter w = new BinaryWriter(s)) {

						w.Write(fileVersion);

						foreach(LocomotiveItem item in listViewLocomotives.Items)
							item.Save(w);
					}
				}
			} catch(IOException ex) {
				MessageBox.Show(this, "Error while saving file: " + ex.Message, "Error while saving file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		public void LoadFile(string filename) {
			try {
				using(FileStream s = new FileStream(filename, FileMode.Open)) {
					using(BinaryReader r = new BinaryReader(s)) {
						currentFileVersion = r.ReadInt32();

						listViewLocomotives.Items.Clear();

						try {
							while(true)
								listViewLocomotives.Items.Add(new LocomotiveItem(this, r));
						} catch(EndOfStreamException) {
						}
					}
				}
			} catch(IOException ex) {
				MessageBox.Show(this, "Error while loading file: " + ex.Message, "Error while loading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void setKind(string kind) {
			foreach(LocomotiveItem item in listViewLocomotives.SelectedItems)
				item.Kind = kind;
		}

		private void setOrigin(string origin) {
			foreach(LocomotiveItem item in listViewLocomotives.SelectedItems)
				item.Origin = origin;
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
			this.listViewLocomotives = new System.Windows.Forms.ListView();
			this.columnHeaderName = new System.Windows.Forms.ColumnHeader();
			this.pictureBoxImage = new CroppablePictureBox();
			this.labelImageDetails = new System.Windows.Forms.Label();
			this.pictureBoxProcessedImage = new System.Windows.Forms.PictureBox();
			this.mainMenu = new System.Windows.Forms.MainMenu();
			this.menuItemFile = new System.Windows.Forms.MenuItem();
			this.menuItemLoad = new System.Windows.Forms.MenuItem();
			this.menuItemSave = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemDownload = new System.Windows.Forms.MenuItem();
			this.menuItemGenerateCatalog = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItemExit = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItemTranslationsEdit = new System.Windows.Forms.MenuItem();
			this.menuItemTranslationsLoad = new System.Windows.Forms.MenuItem();
			this.menuItemTranslationsSave = new System.Windows.Forms.MenuItem();
			this.openFileDialogLoad = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialogSave = new System.Windows.Forms.SaveFileDialog();
			this.groupBoxKind = new System.Windows.Forms.GroupBox();
			this.radioButtonElectric = new System.Windows.Forms.RadioButton();
			this.radioButtonSteam = new System.Windows.Forms.RadioButton();
			this.radioButtonDiesel = new System.Windows.Forms.RadioButton();
			this.buttonSetImageUrl = new System.Windows.Forms.Button();
			this.labelName = new System.Windows.Forms.Label();
			this.textBoxName = new System.Windows.Forms.TextBox();
			this.openFileDialogTranslations = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialogTranslations = new System.Windows.Forms.SaveFileDialog();
			this.saveFileDialogCatalog = new System.Windows.Forms.SaveFileDialog();
			this.groupBoxOrigin = new System.Windows.Forms.GroupBox();
			this.radioButtonOriginEurope = new System.Windows.Forms.RadioButton();
			this.radioButtonOriginUS = new System.Windows.Forms.RadioButton();
			this.groupBoxKind.SuspendLayout();
			this.groupBoxOrigin.SuspendLayout();
			this.SuspendLayout();
			// 
			// listViewLocomotives
			// 
			this.listViewLocomotives.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																								  this.columnHeaderName});
			this.listViewLocomotives.FullRowSelect = true;
			this.listViewLocomotives.HideSelection = false;
			this.listViewLocomotives.Location = new System.Drawing.Point(8, 8);
			this.listViewLocomotives.Name = "listViewLocomotives";
			this.listViewLocomotives.Size = new System.Drawing.Size(408, 320);
			this.listViewLocomotives.TabIndex = 0;
			this.listViewLocomotives.View = System.Windows.Forms.View.Details;
			this.listViewLocomotives.SelectedIndexChanged += new System.EventHandler(this.listViewLocomotives_SelectedIndexChanged);
			// 
			// columnHeaderName
			// 
			this.columnHeaderName.Text = "Name";
			this.columnHeaderName.Width = 386;
			// 
			// pictureBoxImage
			// 
			this.pictureBoxImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxImage.CroppingRectangle = new System.Drawing.Rectangle(0, 0, 100, 100);
			this.pictureBoxImage.Location = new System.Drawing.Point(8, 368);
			this.pictureBoxImage.Name = "pictureBoxImage";
			this.pictureBoxImage.Size = new System.Drawing.Size(250, 178);
			this.pictureBoxImage.TabIndex = 2;
			this.pictureBoxImage.TabStop = false;
			// 
			// labelImageDetails
			// 
			this.labelImageDetails.Location = new System.Drawing.Point(280, 352);
			this.labelImageDetails.Name = "labelImageDetails";
			this.labelImageDetails.Size = new System.Drawing.Size(152, 16);
			this.labelImageDetails.TabIndex = 3;
			this.labelImageDetails.Text = "Processed Image:";
			// 
			// pictureBoxProcessedImage
			// 
			this.pictureBoxProcessedImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBoxProcessedImage.Location = new System.Drawing.Point(280, 368);
			this.pictureBoxProcessedImage.Name = "pictureBoxProcessedImage";
			this.pictureBoxProcessedImage.Size = new System.Drawing.Size(98, 48);
			this.pictureBoxProcessedImage.TabIndex = 4;
			this.pictureBoxProcessedImage.TabStop = false;
			// 
			// mainMenu
			// 
			this.mainMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuItemFile,
																					 this.menuItem2});
			// 
			// menuItemFile
			// 
			this.menuItemFile.Index = 0;
			this.menuItemFile.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItemLoad,
																						 this.menuItemSave,
																						 this.menuItem1,
																						 this.menuItemDownload,
																						 this.menuItemGenerateCatalog,
																						 this.menuItem4,
																						 this.menuItemExit});
			this.menuItemFile.Text = "File";
			// 
			// menuItemLoad
			// 
			this.menuItemLoad.Index = 0;
			this.menuItemLoad.Text = "Load...";
			this.menuItemLoad.Click += new System.EventHandler(this.menuItemLoad_Click);
			// 
			// menuItemSave
			// 
			this.menuItemSave.Index = 1;
			this.menuItemSave.Text = "&Save...";
			this.menuItemSave.Click += new System.EventHandler(this.menuItemSave_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 2;
			this.menuItem1.Text = "-";
			// 
			// menuItemDownload
			// 
			this.menuItemDownload.Index = 3;
			this.menuItemDownload.Text = "&Download from Web";
			this.menuItemDownload.Click += new System.EventHandler(this.menuItemDownload_Click);
			// 
			// menuItemGenerateCatalog
			// 
			this.menuItemGenerateCatalog.Index = 4;
			this.menuItemGenerateCatalog.Text = "&Generate Catalog...";
			this.menuItemGenerateCatalog.Click += new System.EventHandler(this.menuItemGenerateCatalog_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 5;
			this.menuItem4.Text = "-";
			// 
			// menuItemExit
			// 
			this.menuItemExit.Index = 6;
			this.menuItemExit.Text = "E&xit";
			this.menuItemExit.Click += new System.EventHandler(this.menuItemExit_Click);
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItemTranslationsEdit,
																					  this.menuItemTranslationsLoad,
																					  this.menuItemTranslationsSave});
			this.menuItem2.Text = "&Translations";
			// 
			// menuItemTranslationsEdit
			// 
			this.menuItemTranslationsEdit.Index = 0;
			this.menuItemTranslationsEdit.Text = "&Edit...";
			this.menuItemTranslationsEdit.Click += new System.EventHandler(this.menuItemTranslationsEdit_Click);
			// 
			// menuItemTranslationsLoad
			// 
			this.menuItemTranslationsLoad.Index = 1;
			this.menuItemTranslationsLoad.Text = "Load...";
			this.menuItemTranslationsLoad.Click += new System.EventHandler(this.menuItemTranslationsLoad_Click);
			// 
			// menuItemTranslationsSave
			// 
			this.menuItemTranslationsSave.Index = 2;
			this.menuItemTranslationsSave.Text = "Save...";
			this.menuItemTranslationsSave.Click += new System.EventHandler(this.menuItemTranslationsSave_Click);
			// 
			// openFileDialogLoad
			// 
			this.openFileDialogLoad.DefaultExt = "RawCatalog";
			this.openFileDialogLoad.Filter = "Raw Catalog Files|*.RawCatalog|All files|*.*";
			this.openFileDialogLoad.Title = "Load Raw Catalog Data";
			// 
			// saveFileDialogSave
			// 
			this.saveFileDialogSave.DefaultExt = "RawCatalog";
			this.saveFileDialogSave.FileName = "marklinCatalog";
			this.saveFileDialogSave.Filter = "Raw Catalog Files|*.RawCatalog|All files|*.*";
			this.saveFileDialogSave.Title = "Save Raw Catalog Data";
			// 
			// groupBoxKind
			// 
			this.groupBoxKind.Controls.AddRange(new System.Windows.Forms.Control[] {
																					   this.radioButtonElectric,
																					   this.radioButtonSteam,
																					   this.radioButtonDiesel});
			this.groupBoxKind.Location = new System.Drawing.Point(272, 456);
			this.groupBoxKind.Name = "groupBoxKind";
			this.groupBoxKind.Size = new System.Drawing.Size(112, 96);
			this.groupBoxKind.TabIndex = 5;
			this.groupBoxKind.TabStop = false;
			this.groupBoxKind.Text = "Locomotive Kind:";
			// 
			// radioButtonElectric
			// 
			this.radioButtonElectric.Location = new System.Drawing.Point(8, 16);
			this.radioButtonElectric.Name = "radioButtonElectric";
			this.radioButtonElectric.Size = new System.Drawing.Size(88, 24);
			this.radioButtonElectric.TabIndex = 0;
			this.radioButtonElectric.Text = "Electric";
			this.radioButtonElectric.CheckedChanged += new System.EventHandler(this.radioButtonElectric_CheckedChanged);
			// 
			// radioButtonSteam
			// 
			this.radioButtonSteam.Location = new System.Drawing.Point(8, 40);
			this.radioButtonSteam.Name = "radioButtonSteam";
			this.radioButtonSteam.Size = new System.Drawing.Size(88, 24);
			this.radioButtonSteam.TabIndex = 0;
			this.radioButtonSteam.Text = "Steam";
			this.radioButtonSteam.CheckedChanged += new System.EventHandler(this.radioButtonSteam_CheckedChanged);
			// 
			// radioButtonDiesel
			// 
			this.radioButtonDiesel.Location = new System.Drawing.Point(8, 64);
			this.radioButtonDiesel.Name = "radioButtonDiesel";
			this.radioButtonDiesel.Size = new System.Drawing.Size(88, 24);
			this.radioButtonDiesel.TabIndex = 0;
			this.radioButtonDiesel.Text = "Diesel";
			this.radioButtonDiesel.CheckedChanged += new System.EventHandler(this.radioButtonDiesel_CheckedChanged);
			// 
			// buttonSetImageUrl
			// 
			this.buttonSetImageUrl.Location = new System.Drawing.Point(8, 336);
			this.buttonSetImageUrl.Name = "buttonSetImageUrl";
			this.buttonSetImageUrl.Size = new System.Drawing.Size(112, 23);
			this.buttonSetImageUrl.TabIndex = 6;
			this.buttonSetImageUrl.Text = "Set Image URL...";
			this.buttonSetImageUrl.Click += new System.EventHandler(this.buttonSetImageUrl_Click);
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(267, 427);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(40, 16);
			this.labelName.TabIndex = 7;
			this.labelName.Text = "Name:";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBoxName
			// 
			this.textBoxName.Location = new System.Drawing.Point(312, 425);
			this.textBoxName.Name = "textBoxName";
			this.textBoxName.Size = new System.Drawing.Size(165, 20);
			this.textBoxName.TabIndex = 8;
			this.textBoxName.Text = "";
			this.textBoxName.TextChanged += new System.EventHandler(this.textBoxName_TextChanged);
			// 
			// openFileDialogTranslations
			// 
			this.openFileDialogTranslations.DefaultExt = "Translations";
			this.openFileDialogTranslations.Filter = "Translations Files|*.Translations|All files|*.*";
			this.openFileDialogTranslations.Title = "Load Translations File";
			// 
			// saveFileDialogTranslations
			// 
			this.saveFileDialogTranslations.DefaultExt = "Translations";
			this.saveFileDialogTranslations.FileName = "marklonCatalog";
			this.saveFileDialogTranslations.Filter = "Translations Files|*.Translations|All files|*.*";
			this.saveFileDialogTranslations.Title = "Save Translations";
			// 
			// saveFileDialogCatalog
			// 
			this.saveFileDialogCatalog.DefaultExt = "LocomotiveCatalog";
			this.saveFileDialogCatalog.FileName = "marklinCatalog";
			this.saveFileDialogCatalog.Filter = "Layout Manager Locomotive Catalog|*.LocomotiveCatalog|All files|*.*";
			this.saveFileDialogCatalog.Title = "Save Catalog";
			// 
			// groupBoxOrigin
			// 
			this.groupBoxOrigin.Controls.AddRange(new System.Windows.Forms.Control[] {
																						 this.radioButtonOriginEurope,
																						 this.radioButtonOriginUS});
			this.groupBoxOrigin.Location = new System.Drawing.Point(392, 456);
			this.groupBoxOrigin.Name = "groupBoxOrigin";
			this.groupBoxOrigin.Size = new System.Drawing.Size(88, 96);
			this.groupBoxOrigin.TabIndex = 9;
			this.groupBoxOrigin.TabStop = false;
			this.groupBoxOrigin.Text = "Origin";
			// 
			// radioButtonOriginEurope
			// 
			this.radioButtonOriginEurope.Location = new System.Drawing.Point(8, 16);
			this.radioButtonOriginEurope.Name = "radioButtonOriginEurope";
			this.radioButtonOriginEurope.Size = new System.Drawing.Size(64, 24);
			this.radioButtonOriginEurope.TabIndex = 0;
			this.radioButtonOriginEurope.Text = "Europe";
			this.radioButtonOriginEurope.CheckedChanged += new System.EventHandler(this.radioButtonOriginEurope_CheckedChanged);
			// 
			// radioButtonOriginUS
			// 
			this.radioButtonOriginUS.Location = new System.Drawing.Point(8, 40);
			this.radioButtonOriginUS.Name = "radioButtonOriginUS";
			this.radioButtonOriginUS.Size = new System.Drawing.Size(64, 24);
			this.radioButtonOriginUS.TabIndex = 0;
			this.radioButtonOriginUS.Text = "US";
			this.radioButtonOriginUS.CheckedChanged += new System.EventHandler(this.radioButtonOriginUS_CheckedChanged);
			// 
			// GetMarklinCatalog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(488, 566);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.groupBoxOrigin,
																		  this.textBoxName,
																		  this.labelName,
																		  this.buttonSetImageUrl,
																		  this.groupBoxKind,
																		  this.pictureBoxProcessedImage,
																		  this.labelImageDetails,
																		  this.pictureBoxImage,
																		  this.listViewLocomotives});
			this.Menu = this.mainMenu;
			this.Name = "GetMarklinCatalog";
			this.Text = "Get Marklin Catalog";
			this.groupBoxKind.ResumeLayout(false);
			this.groupBoxOrigin.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new GetMarklinCatalog());
		}

		private void buttonGetIndex_Click(object sender, System.EventArgs e) {
			for(int page = 0; parseIndexPage(getIndexPage(page)) > 0; page++)
				;
		}

		private void buttonGetDetails_Click(object sender, System.EventArgs e) {
			WebClient	web = new WebClient();

			web.BaseAddress = baseURL;

			foreach(LocomotiveItem item in listViewLocomotives.SelectedItems) {
				item.DownloadHtml(web);
				item.DownloadImage(web);

				showItem(item);
			}	
		}

		private void listViewLocomotives_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(listViewLocomotives.SelectedItems.Count > 0) {
				LocomotiveItem	item = (LocomotiveItem)listViewLocomotives.SelectedItems[0];

				showItem(item);
			}
		}

		private void menuItemExit_Click(object sender, System.EventArgs e) {
			Close();
		}

		private void menuItemLoad_Click(object sender, System.EventArgs e) {
			if(openFileDialogLoad.ShowDialog(this) == DialogResult.OK)
				LoadFile(openFileDialogLoad.FileName);
		}

		private void menuItemSave_Click(object sender, System.EventArgs e) {
			if(saveFileDialogSave.ShowDialog(this) == DialogResult.OK)
				SaveFile(saveFileDialogSave.FileName);
		}

		private void menuItemDownload_Click(object sender, System.EventArgs e) {
			DownloadProgress	progress = new DownloadProgress();

			progress.Show();
			this.Enabled = false;

			if(listViewLocomotives.Items.Count == 0) {
				progress.Description = "Downloading index...";
				Application.DoEvents();

				for(int page = 0; parseIndexPage(getIndexPage(page)) > 0; page++)
					Application.DoEvents();
			}

			if(!progress.Aborted) {
				WebClient	web = new WebClient();

				web.BaseAddress = baseURL;

				progress.ProgressBar.Minimum = 0;
				progress.ProgressBar.Maximum = listViewLocomotives.Items.Count;

				int		iItem = 0;
				foreach(LocomotiveItem item in listViewLocomotives.Items) {
					Application.DoEvents();
					progress.ProgressBar.Value = iItem++;

					if(item.DetailsHtml == null) {
						progress.Description = "Downloading " + item.Text + " details...";
						Application.DoEvents();
						item.DownloadHtml(web);
					}

					if(progress.Aborted)
						break;

					Application.DoEvents();
					if(item.ImageBytes == null) {
						progress.Description = "Downloading " + item.Text + " image...";
						Application.DoEvents();
						item.DownloadImage(web);
					}

					if(progress.Aborted)
						break;

					progress.Description = "";
				}
			}

			progress.Close();
			this.Enabled = true;
		}

		private void radioButtonElectric_CheckedChanged(object sender, System.EventArgs e) {
			setKind("Electric");
		}

		private void radioButtonSteam_CheckedChanged(object sender, System.EventArgs e) {
			setKind("Steam");
		}

		private void radioButtonDiesel_CheckedChanged(object sender, System.EventArgs e) {
			setKind("Diesel");
		}

		private void buttonSetImageUrl_Click(object sender, System.EventArgs e) {
			string		imageUrl = InputBox.Show("Image URL", "Enter Image URL");

			if(imageUrl != null) {
				WebClient	web = new WebClient();

				web.BaseAddress = baseURL;

				foreach(LocomotiveItem item in listViewLocomotives.SelectedItems) {
					item.CustomImageUrl = imageUrl;
					item.DownloadImage(web);
				}
			}
		}

		private void menuItemTranslationsEdit_Click(object sender, System.EventArgs e) {
			EditTranslations	d = new EditTranslations(translationsDocument.DocumentElement);

			d.ShowDialog(this);

			foreach(LocomotiveItem item in listViewLocomotives.Items) {
				item.Translate(translationsDocument);
				item.Text = item.TypeName;
			}

			showSelectedItem();
		}

		private void menuItemTranslationsLoad_Click(object sender, System.EventArgs e) {
			if(openFileDialogTranslations.ShowDialog(this) == DialogResult.OK) {
				translationsDocument.Load(openFileDialogTranslations.FileName);

				foreach(LocomotiveItem item in listViewLocomotives.Items) {
					item.Translate(translationsDocument);
					item.Text = item.TypeName;
				}

				showSelectedItem();
			}
		}

		private void menuItemTranslationsSave_Click(object sender, System.EventArgs e) {
			if(saveFileDialogTranslations.ShowDialog(this) == DialogResult.OK)
				translationsDocument.Save(saveFileDialogTranslations.FileName);
		}

		private void textBoxName_TextChanged(object sender, System.EventArgs e) {
			if(listViewLocomotives.SelectedItems.Count > 0) {
				LocomotiveItem	item = (LocomotiveItem)listViewLocomotives.SelectedItems[0];

				item.TypeName = textBoxName.Text;
				item.Text = textBoxName.Text;
			}
		}

		private void menuItemGenerateCatalog_Click(object sender, System.EventArgs e) {
			if(saveFileDialogCatalog.ShowDialog(this) == DialogResult.OK) {
				foreach(LocomotiveItem item in listViewLocomotives.Items)
					item.PrepareCatalog();

				ContainerElement.OwnerDocument.Save(saveFileDialogCatalog.FileName);

				foreach(LocomotiveItem item in listViewLocomotives.Items)
					item.UnprepareCatalog();
			}
		}

		private void radioButtonOriginEurope_CheckedChanged(object sender, System.EventArgs e) {
			setOrigin("Europe");
		}

		private void radioButtonOriginUS_CheckedChanged(object sender, System.EventArgs e) {
			setOrigin("US");
		}

		class LocomotiveItem : ListViewItem {
			ILocomotiveItemsContainer	container;
			string						detailsUrl;
			string						detailsHtml = null;
			string						imageUrl = null;
			byte[]						imageBytes = null;
			static readonly Size		processedImageSize = new Size(98, 48);
			Rectangle					cropRectangle;
			XmlElement					element;

			public LocomotiveItem(ILocomotiveItemsContainer container, string name, string detailsUrl) {
				this.container = container;
				Text = name;
				this.detailsUrl = detailsUrl;
				element = container.ContainerElement.OwnerDocument.CreateElement("LocomotiveType");
				container.ContainerElement.AppendChild(element);
			}

			public LocomotiveItem(ILocomotiveItemsContainer container, BinaryReader r) {
				this.container = container;
				Load(r);
			}

			public string DetailsUrl {
				get {
					return detailsUrl;
				}
			}

			public string DetailsHtml {
				get {
					return detailsHtml;
				}

				set {
					detailsHtml = value;
					ParseDetailsHtml();		// This will set ImageUrl and other properties such as Kind etc.
				}
			}

			public void DownloadHtml(WebClient web) {
				DetailsHtml = System.Text.Encoding.UTF8.GetString(web.DownloadData("produkte/produkte_cdrom/" + DetailsUrl));
			}

			public string ImageUrl {
				get {
					if(element.HasAttribute("CustomImageUrl"))
						return element.GetAttribute("CustomImageUrl");
					return imageUrl;
				}

				set {
					imageUrl = value;
				}
			}

			public string CustomImageUrl {
				get {
					if(element.HasAttribute("CustomImageUrl"))
						return element.GetAttribute("CustomImageUrl");
					else
						return imageUrl;
				}

				set {
					if(value == "-")
						element.RemoveAttribute("CustomImageUrl");
					else
						element.SetAttribute("CustomImageUrl", value);
				}
			}

			public void DownloadImage(WebClient web) {
				if(ImageUrl == null)
					throw new ArgumentException("Locomotive item " + Text + " image URL is not known");

				ImageBytes = web.DownloadData(ImageUrl);
				GetDefaultCroppingRectangle();
			}

			public byte[] ImageBytes {
				get {
					return imageBytes;
				}

				set {
					imageBytes = value;
				}
			}

			public Image Image {
				get {
					using(MemoryStream m = new MemoryStream(ImageBytes))
						return Image.FromStream(m);
				}
			}

			public Rectangle CropRectangle {
				get {
					return cropRectangle;
				}

				set {
					cropRectangle = value;
				}
			}

			public string TypeName {
				get {
					XmlElement	typeNameElement = element["TypeName"];

					if(typeNameElement == null)
						return Text;
					else
						return typeNameElement.InnerText;
				}

				set {
					XmlElement	typeNameElement = element["TypeName"];

					if(typeNameElement == null) {
						typeNameElement = element.OwnerDocument.CreateElement("TypeName");
						element.AppendChild(typeNameElement);
					}

					typeNameElement.InnerText = value;
				}
			}

			public void Translate(XmlDocument translationsDocument) {
				string	name = TypeName;

				foreach(XmlElement translateElement in translationsDocument.DocumentElement)
					name = Regex.Replace(name, translateElement.GetAttribute("From"), translateElement.GetAttribute("To"));

				TypeName = name;
			}

			public void ParseDetailsHtml() {
				Regex	rImageUrl = new Regex("image\\.php.*?img src=\"(.*?)\"", RegexOptions.IgnoreCase);
				Match	mImageUrl = rImageUrl.Match(DetailsHtml);

				ImageUrl = mImageUrl.Groups[1].Value;

				if(!element.HasAttribute("Kind")) {
					// Get the kind
					Regex	rKind = new Regex("<b>Untergruppe:</b>&nbsp;(.*?)<", RegexOptions.IgnoreCase);
					Match	mKind = rKind.Match(DetailsHtml);

					switch(mKind.Groups[1].Value) {

						case "E-Lok":
							element.SetAttribute("Kind", "Electric");
							break;

						case "Dampflok":
							element.SetAttribute("Kind", "Steam");
							break;

						case "Diesellok":
						case "Triebwagen":
							element.SetAttribute("Kind", "Diesel");
							break;

						default:
							Trace.WriteLine("Unknown locomotive type " + mKind.Groups[1].Value + " for item " + Text);
							break;
					}
				}
			}

			public void UpdateProcessedImage(Image imageToUpdate) {
				if(ImageBytes == null)
					throw new ArgumentException("No image is set for this item (" + Text + ")");

				if(imageToUpdate.Height != processedImageSize.Height || imageToUpdate.Width != processedImageSize.Width)
					throw new ArgumentException("Invalid image to update");

				using(Graphics g = Graphics.FromImage(imageToUpdate)) {
					g.DrawImage(Image, new Rectangle(new Point(0, 0), processedImageSize), cropRectangle, GraphicsUnit.Pixel);
				}
			}

			public Image CreateProcessedImage() {
				return new Bitmap(Image, processedImageSize);
			}

			public void GetDefaultCroppingRectangle() {
				if(ImageBytes == null)
					throw new ArgumentException("No image is set for this item (" + Text + ")");

				Bitmap	bm = (Bitmap)Image;
				Color	background = bm.GetPixel(0, 0);
				int		t, b, l, r;

				for(t = 0; t < bm.Height && bm.GetPixel(bm.Width / 2, t) == background; t++)
					;

				for(b = bm.Height-1; b >= 0 && bm.GetPixel(bm.Width / 2, b) == background; b--)
					;

				for(l = 0; l < bm.Width && bm.GetPixel(l, bm.Height / 2) == background; l++)
					;

				for(r = bm.Width-1; r >= 0 && bm.GetPixel(r, bm.Height / 2) == background; r--)
					;

				// Fix some margins
				l = Math.Max(0, l - 5);
				r = Math.Min(bm.Width - 1, r + 5);
				t = Math.Max(0, t - 5);
				b = Math.Min(bm.Height - 1, b + 5);

				if((r - l) > (b - t)) {
					int	vSize = (r - l) * processedImageSize.Height / processedImageSize.Width;
					int	y = (b + t) / 2;

					CropRectangle = Rectangle.FromLTRB(l, y - vSize / 2, r, y + vSize / 2);
				}
				else {
					int hSize = (b - t) * processedImageSize.Width / processedImageSize.Height;
					int	x = (l + r) / 2;

					CropRectangle = Rectangle.FromLTRB(x - hSize / 2, t, x + hSize / 2, b);
				}
			}

			public string Kind {
				get {
					if(element.HasAttribute("Kind"))
						return element.GetAttribute("Kind");
					return "";
				}

				set {
					element.SetAttribute("Kind", value);
				}
			}

			public string Origin {
				get {
					if(!element.HasAttribute("Origin"))
						element.SetAttribute("Origin", "Europe");
					return element.GetAttribute("Origin");
				}

				set {
					element.SetAttribute("Origin", value);
				}
			}

			// Save/Load item to raw file, format is as follows
			//
			// <Item Text> (string)
			// <DetailsUrl> (string)
			// <HasHtml) (bool) -- If false end
			// <Html> (string)
			// <Xml Information> (string)
			// <HasImage> (bool) -- If false end
			// <Cropping rectangle> (4 ints)
			// <Image bytes>
			//
			public void Save(BinaryWriter w) {
				w.Write(Text);
				w.Write(DetailsUrl);

				w.Write(DetailsHtml != null);
				if(DetailsHtml != null) {
					w.Write(DetailsHtml);
					w.Write(element.OuterXml);

					w.Write(ImageBytes != null);
					if(ImageBytes != null) {
						w.Write((long)ImageBytes.Length);
						w.Write(ImageBytes, 0, ImageBytes.Length);

						w.Write(CropRectangle.Left);
						w.Write(CropRectangle.Top);
						w.Write(CropRectangle.Right);
						w.Write(CropRectangle.Bottom);
					}
				}
			}

			public void Load(BinaryReader r) {
				Text = r.ReadString();
				detailsUrl = r.ReadString();

				if(r.ReadBoolean()) {
					string	detailsHtml = r.ReadString();

					string			xmlText = r.ReadString();
					XmlTextReader	xmlReader = new XmlTextReader(xmlText, XmlNodeType.Element, null);
		
					element = (XmlElement)container.ContainerElement.OwnerDocument.ReadNode(xmlReader);
					container.ContainerElement.AppendChild(element);

					DetailsHtml = detailsHtml;

					if(r.ReadBoolean()) {
						long	bufferLength = r.ReadInt64();
		
						ImageBytes = r.ReadBytes((int)bufferLength);

						int		left, top, right, bottom;

						left = r.ReadInt32();
						top = r.ReadInt32();
						right = r.ReadInt32();
						bottom = r.ReadInt32();

						CropRectangle = Rectangle.FromLTRB(left, top, right, bottom);
					}
				}
			}

			public void PrepareCatalog() {
				XmlElement	imageElement = element.OwnerDocument.CreateElement("Image");
				string		origin = Origin;	// Side effect is to set the origin if not already set

				element.AppendChild(imageElement);
				using(MemoryStream m = new MemoryStream()) {
					using(Image processedImage = CreateProcessedImage()) {
						UpdateProcessedImage(processedImage);

						processedImage.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
						imageElement.InnerText = Convert.ToBase64String(m.GetBuffer());
					}
				}
			}

			public void UnprepareCatalog() {
				XmlElement	imageElement = element["Image"];

				if(imageElement != null)
					element.RemoveChild(imageElement);
			}
		}
	}
}
