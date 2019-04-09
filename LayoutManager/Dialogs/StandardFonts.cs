using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;

using LayoutManager.Model;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for StadardFonts.
    /// </summary>
    public class StandardFonts : Form {
        private ListView listViewFonts;
        private ColumnHeader columnHeaderTitle;
        private ColumnHeader columnHeaderDescription;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonDelete;
        private Button buttonClose;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private ContextMenu contextMenuEdit;
        private MenuItem menuItemEditSettings;
        private MenuItem menuItemEditTitle;
        private MenuItem menuItemFontID;

        public StandardFonts() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            listViewFonts.MultiSelect = false;
            listViewFonts.FullRowSelect = true;

            fill();
        }

        private void fill() {
            XmlNode fonts = LayoutModel.Instance.XmlInfo.DocumentElement.SelectSingleNode("Fonts");

            if (fonts != null) {
                foreach (XmlElement fontElement in fonts)
                    listViewFonts.Items.Add(new FontItem(fontElement));
            }
        }

        private class FontItem : ListViewItem {
            private readonly XmlElement fontElement;

            public FontItem(XmlElement fontElement) {
                this.fontElement = fontElement;

                LayoutFontInfo fontProvider = new LayoutFontInfo(fontElement);
                String[] items = new String[1] { fontProvider.Description };

                this.Text = fontProvider.GetAttribute("Title");
                SubItems.AddRange(items);
            }

            public LayoutFontInfo FontProvider => new LayoutFontInfo(fontElement);

            public void Update() {
                LayoutFontInfo fontProvider = new LayoutFontInfo(fontElement);

                SubItems[1].Text = fontProvider.Description;
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.columnHeaderTitle = new ColumnHeader();
            this.buttonDelete = new Button();
            this.buttonEdit = new Button();
            this.buttonClose = new Button();
            this.buttonAdd = new Button();
            this.listViewFonts = new ListView();
            this.columnHeaderDescription = new ColumnHeader();
            this.contextMenuEdit = new ContextMenu();
            this.menuItemEditSettings = new MenuItem();
            this.menuItemEditTitle = new MenuItem();
            this.menuItemFontID = new MenuItem();
            this.SuspendLayout();
            // 
            // columnHeaderTitle
            // 
            this.columnHeaderTitle.Text = "Title";
            this.columnHeaderTitle.Width = 80;
            // 
            // buttonDelete
            // 
            this.buttonDelete.Location = new System.Drawing.Point(168, 224);
            this.buttonDelete.Name = "buttonDelete";
            this.buttonDelete.Size = new System.Drawing.Size(72, 23);
            this.buttonDelete.TabIndex = 4;
            this.buttonDelete.Text = "&Delete";
            this.buttonDelete.Click += new System.EventHandler(this.buttonDelete_Click);
            // 
            // buttonEdit
            // 
            this.buttonEdit.Location = new System.Drawing.Point(88, 224);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(72, 23);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit";
            this.buttonEdit.Click += new System.EventHandler(this.buttonEdit_Click);
            // 
            // buttonClose
            // 
            this.buttonClose.Location = new System.Drawing.Point(168, 256);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(72, 23);
            this.buttonClose.TabIndex = 5;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(8, 224);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(72, 23);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&New";
            this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
            // 
            // listViewFonts
            // 
            this.listViewFonts.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderTitle,
            this.columnHeaderDescription});
            this.listViewFonts.HideSelection = false;
            this.listViewFonts.LabelEdit = true;
            this.listViewFonts.Location = new System.Drawing.Point(8, 16);
            this.listViewFonts.Name = "listViewFonts";
            this.listViewFonts.Size = new System.Drawing.Size(240, 200);
            this.listViewFonts.TabIndex = 0;
            this.listViewFonts.View = System.Windows.Forms.View.Details;
            this.listViewFonts.DoubleClick += new System.EventHandler(this.buttonEdit_Click);
            this.listViewFonts.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listViewFonts_AfterLabelEdit);
            // 
            // columnHeaderDescription
            // 
            this.columnHeaderDescription.Text = "Description";
            this.columnHeaderDescription.Width = 188;
            // 
            // contextMenuEdit
            // 
            this.contextMenuEdit.MenuItems.AddRange(new MenuItem[] {
            this.menuItemEditSettings,
            this.menuItemEditTitle,
            this.menuItemFontID});
            // 
            // menuItemEditSettings
            // 
            this.menuItemEditSettings.Index = 0;
            this.menuItemEditSettings.Text = "Settings...";
            this.menuItemEditSettings.Click += new System.EventHandler(this.menuItemEditSettings_Click);
            // 
            // menuItemEditTitle
            // 
            this.menuItemEditTitle.Index = 1;
            this.menuItemEditTitle.Text = "Title...";
            this.menuItemEditTitle.Click += new System.EventHandler(this.menuItemEditTitle_Click);
            // 
            // menuItemFontID
            // 
            this.menuItemFontID.Index = 2;
            this.menuItemFontID.Text = "Font Ref...";
            this.menuItemFontID.Click += new System.EventHandler(this.menuItemFontID_Click);
            // 
            // StadardFonts
            // 
            this.AcceptButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(258, 288);
            this.ControlBox = false;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.buttonDelete);
            this.Controls.Add(this.buttonEdit);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.listViewFonts);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "StadardFonts";
            this.ShowInTaskbar = false;
            this.Text = "Standard Fonts";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonClose_Click(object sender, System.EventArgs e) {
            this.Close();
        }

        private void buttonEdit_Click(object sender, System.EventArgs e) {
            contextMenuEdit.Show(this, new Point(buttonEdit.Left, buttonEdit.Bottom));
        }

        private void buttonAdd_Click(object sender, System.EventArgs e) {
            XmlElement fontElement = LayoutInfo.CreateProviderElement(LayoutModel.Instance.XmlInfo, "Font", "Fonts");
            LayoutFontInfo fontProvider = new LayoutFontInfo(fontElement);

            fontProvider.SetAttribute("Title", "New font");
            FontItem item = new FontItem(fontElement);

            listViewFonts.Items.Add(item);
            item.BeginEdit();
        }

        private void listViewFonts_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e) {
            FontItem item = (FontItem)listViewFonts.Items[e.Item];

            LayoutFontInfo fontProvider = item.FontProvider;

            fontProvider.SetAttribute("Title", e.Label);
            item.Text = e.Label;
            item.Update();
        }

        private void buttonDelete_Click(object sender, System.EventArgs e) {
            if (listViewFonts.SelectedItems.Count == 1) {
                if (MessageBox.Show("Do you really want to delete a style font. " +
                    "This will cause problems if this font is still being used!", "Warning",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.OK) {
                    FontItem item = (FontItem)listViewFonts.SelectedItems[0];
                    XmlNode fonts = LayoutModel.Instance.XmlInfo.DocumentElement.SelectSingleNode("Fonts");

                    fonts.RemoveChild(item.FontProvider.Element);
                    item.Remove();
                }
            }
        }

        private void menuItemEditSettings_Click(object sender, System.EventArgs e) {
            if (listViewFonts.SelectedItems.Count == 1) {
                FontItem item = (FontItem)listViewFonts.SelectedItems[0];
                FontDialog fontDialog = new FontDialog {
                    ShowColor = true,
                    Color = item.FontProvider.Color,
                    Font = item.FontProvider.Font
                };

                if (fontDialog.ShowDialog() == DialogResult.OK) {
                    item.FontProvider.Font = fontDialog.Font;
                    item.FontProvider.Color = fontDialog.Color;
                    item.Update();
                }
            }
        }

        private void menuItemFontID_Click(object sender, System.EventArgs e) {
            if (listViewFonts.SelectedItems.Count == 1) {
                FontItem item = (FontItem)listViewFonts.SelectedItems[0];
                CommonUI.Dialogs.InputBox fontIDbox = new CommonUI.Dialogs.InputBox("Font Ref", "Font Ref:") {
                    Input = item.FontProvider.Ref
                };
                if (fontIDbox.ShowDialog() == DialogResult.OK) {
                    if (fontIDbox.Input.Trim() == "")
                        item.FontProvider.Ref = null;
                    else
                        item.FontProvider.Ref = fontIDbox.Input;
                }
            }
        }

        private void menuItemEditTitle_Click(object sender, System.EventArgs e) {
            if (listViewFonts.SelectedItems.Count == 1) {
                FontItem item = (FontItem)listViewFonts.SelectedItems[0];
                CommonUI.Dialogs.InputBox titleBox = new CommonUI.Dialogs.InputBox("Title", "Title:") {
                    Input = item.FontProvider.GetAttribute("Title")
                };
                if (titleBox.ShowDialog() == DialogResult.OK) {
                    item.FontProvider.SetAttribute("Title", titleBox.Input);
                    item.Update();
                }
            }
        }
    }
}
