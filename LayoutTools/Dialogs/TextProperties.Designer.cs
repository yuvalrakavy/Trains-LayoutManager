using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for LabelProperties.
    /// </summary>
    partial class TextProperties : Form, ILayoutComponentPropertiesDialog {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.textProviderPositionDefinition = new LayoutManager.CommonUI.Controls.TextProviderPositionDefinition();
            this.buttonOK = new Button();
            this.textBoxText = new TextBox();
            this.labelText = new Label();
            this.textProviderFontDefinition = new LayoutManager.CommonUI.Controls.TextProviderFontDefinition();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // TextProviderPositionDefinition
            // 
            this.textProviderPositionDefinition.CustomPositionElementName = "Position";
            this.textProviderPositionDefinition.Location = new System.Drawing.Point(3, 144);
            this.textProviderPositionDefinition.Name = "TextProviderPositionDefinition";
            this.textProviderPositionDefinition.Size = new System.Drawing.Size(320, 176);
            this.textProviderPositionDefinition.TabIndex = 3;
            this.textProviderPositionDefinition.XmlInfo = null;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(160, 323);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 7;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.ButtonOK_Click;
            // 
            // textBoxText
            // 
            this.textBoxText.Location = new System.Drawing.Point(48, 16);
            this.textBoxText.Name = "textBoxText";
            this.textBoxText.Size = new System.Drawing.Size(265, 20);
            this.textBoxText.TabIndex = 1;
            this.textBoxText.Text = "";
            // 
            // labelText
            // 
            this.labelText.Location = new System.Drawing.Point(8, 18);
            this.labelText.Name = "labelText";
            this.labelText.Size = new System.Drawing.Size(40, 16);
            this.labelText.TabIndex = 0;
            this.labelText.Text = "Text:";
            this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textProviderFontDefinition
            // 
            this.textProviderFontDefinition.CustomFontElementName = "Font";
            this.textProviderFontDefinition.Location = new System.Drawing.Point(3, 40);
            this.textProviderFontDefinition.Name = "textProviderFontDefinition";
            this.textProviderFontDefinition.Size = new System.Drawing.Size(320, 112);
            this.textProviderFontDefinition.TabIndex = 2;
            this.textProviderFontDefinition.XmlInfo = null;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(240, 323);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            // 
            // TextProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(332, 380);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonOK,
                                                                          this.buttonCancel,
                                                                          this.textProviderPositionDefinition,
                                                                          this.textProviderFontDefinition,
                                                                          this.textBoxText,
                                                                          this.labelText});
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TextProperties";
            this.ShowInTaskbar = false;
            this.Text = "Text Properties";
            this.ResumeLayout(false);
        }
        #endregion

        private Label labelText;
        private TextBox textBoxText;
        private LayoutManager.CommonUI.Controls.TextProviderFontDefinition textProviderFontDefinition;
        private LayoutManager.CommonUI.Controls.TextProviderPositionDefinition textProviderPositionDefinition;
        private Button buttonOK;
        private Button buttonCancel;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}

