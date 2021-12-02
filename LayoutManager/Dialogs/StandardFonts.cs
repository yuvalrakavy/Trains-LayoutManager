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
    public partial class StandardFonts : Form {

        public StandardFonts() {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            listViewFonts.MultiSelect = false;
            listViewFonts.FullRowSelect = true;

            Fill();
        }

        private void Fill() {
            var fonts = LayoutModel.Instance.XmlInfo.DocumentElement.SelectSingleNode("Fonts");

            if (fonts != null) {
                foreach (XmlElement fontElement in fonts)
                    listViewFonts.Items.Add(new FontItem(fontElement));
            }
        }

        private class FontItem : ListViewItem {
            private readonly XmlElement fontElement;

            public FontItem(XmlElement fontElement) {
                this.fontElement = fontElement;

                LayoutFontInfo fontProvider = new(fontElement);
                String[] items = new String[1] { fontProvider.Description };

                this.Text = fontProvider.GetAttribute("Title");
                SubItems.AddRange(items);
            }

            public LayoutFontInfo FontProvider => new(fontElement);

            public void Update() {
                LayoutFontInfo fontProvider = new(fontElement);

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

        private void ButtonClose_Click(object? sender, EventArgs e) {
            this.Close();
        }

        private void ButtonEdit_Click(object? sender, EventArgs e) {
            contextMenuEdit.Show(this, new Point(buttonEdit.Left, buttonEdit.Bottom));
        }

        private void ButtonAdd_Click(object? sender, EventArgs e) {
            XmlElement fontElement = LayoutInfo.CreateProviderElement(LayoutModel.Instance.XmlInfo, "Font", "Fonts");
            LayoutFontInfo fontProvider = new(fontElement);

            fontProvider.SetAttributeValue("Title", "New font");
            FontItem item = new(fontElement);

            listViewFonts.Items.Add(item);
            item.BeginEdit();
        }

        private void ListViewFonts_AfterLabelEdit(object? sender, LabelEditEventArgs e) {
            FontItem item = (FontItem)listViewFonts.Items[e.Item];

            LayoutFontInfo fontProvider = item.FontProvider;

            fontProvider.SetAttributeValue("Title", e.Label);
            item.Text = e.Label;
            item.Update();
        }

        private void ButtonDelete_Click(object? sender, EventArgs e) {
            if (listViewFonts.SelectedItems.Count == 1) {
                if (MessageBox.Show("Do you really want to delete a style font. " +
                    "This will cause problems if this font is still being used!", "Warning",
                    MessageBoxButtons.OKCancel, MessageBoxIcon.Stop) == DialogResult.OK) {
                    FontItem item = (FontItem)listViewFonts.SelectedItems[0];
                    var fonts = LayoutModel.Instance.XmlInfo.DocumentElement.SelectSingleNode("Fonts");

                    fonts?.RemoveChild(item.FontProvider.Element);
                    item.Remove();
                }
            }
        }

        private void MenuItemEditSettings_Click(object? sender, EventArgs e) {
            if (listViewFonts.SelectedItems.Count == 1) {
                FontItem item = (FontItem)listViewFonts.SelectedItems[0];
                FontDialog fontDialog = new() {
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

        private void MenuItemFontID_Click(object? sender, EventArgs e) {
            if (listViewFonts.SelectedItems.Count == 1) {
                FontItem item = (FontItem)listViewFonts.SelectedItems[0];
                CommonUI.Dialogs.InputBox fontIDbox = new("Font Ref", "Font Ref:") {
                    Input = item.FontProvider.Ref ?? String.Empty
                };
                if (fontIDbox.ShowDialog() == DialogResult.OK) {
                    if (fontIDbox.Input.Trim() == "")
                        item.FontProvider.Ref = null;
                    else
                        item.FontProvider.Ref = fontIDbox.Input;
                }
            }
        }

        private void MenuItemEditTitle_Click(object? sender, EventArgs e) {
            if (listViewFonts.SelectedItems.Count == 1) {
                FontItem item = (FontItem)listViewFonts.SelectedItems[0];
                CommonUI.Dialogs.InputBox titleBox = new("Title", "Title:") {
                    Input = item.FontProvider.GetAttribute("Title")
                };
                if (titleBox.ShowDialog() == DialogResult.OK) {
                    item.FontProvider.SetAttributeValue("Title", titleBox.Input);
                    item.Update();
                }
            }
        }
    }
}
