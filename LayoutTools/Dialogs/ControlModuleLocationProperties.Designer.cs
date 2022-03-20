using System;
using System.Windows.Forms;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for ControlModuleLocationProperties.
    /// </summary>
    partial class ControlModuleLocationProperties : Form, ILayoutComponentPropertiesDialog {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.nameDefinition = new LayoutManager.CommonUI.Controls.NameDefinition();
            this.groupBox1 = new GroupBox();
            this.buttonEdit = new Button();
            this.buttonAdd = new Button();
            this.listViewDefaults = new ListView();
            this.columnHeaderBusName = new ColumnHeader();
            this.columnHeaderModuleType = new ColumnHeader();
            this.columnHeaderStartAddress = new ColumnHeader();
            this.buttonRemove = new Button();
            this.buttonCancel = new Button();
            this.buttonOK = new Button();
            this.label1 = new Label();
            this.comboBoxCommandStation = new ComboBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // nameDefinition
            // 
            this.nameDefinition.DefaultIsVisible = true;
            this.nameDefinition.ElementName = "Name";
            this.nameDefinition.IsOptional = false;
            this.nameDefinition.Location = new System.Drawing.Point(3, 8);
            this.nameDefinition.Name = "nameDefinition";
            this.nameDefinition.Size = new System.Drawing.Size(373, 64);
            this.nameDefinition.TabIndex = 0;
            this.nameDefinition.XmlInfo = null;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.buttonEdit);
            this.groupBox1.Controls.Add(this.buttonAdd);
            this.groupBox1.Controls.Add(this.listViewDefaults);
            this.groupBox1.Controls.Add(this.buttonRemove);
            this.groupBox1.Location = new System.Drawing.Point(8, 96);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(368, 192);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Defaults:";
            // 
            // buttonEdit
            // 
            this.buttonEdit.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonEdit.Location = new System.Drawing.Point(88, 166);
            this.buttonEdit.Name = "buttonEdit";
            this.buttonEdit.Size = new System.Drawing.Size(75, 20);
            this.buttonEdit.TabIndex = 2;
            this.buttonEdit.Text = "&Edit";
            this.buttonEdit.Click += new EventHandler(this.ButtonEdit_Click);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonAdd.Location = new System.Drawing.Point(8, 166);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 20);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.Click += new EventHandler(this.ButtonAdd_Click);
            // 
            // listViewDefaults
            // 
            this.listViewDefaults.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right);
            this.listViewDefaults.Columns.AddRange(new ColumnHeader[] {
                                                                                               this.columnHeaderBusName,
                                                                                               this.columnHeaderModuleType,
                                                                                               this.columnHeaderStartAddress});
            this.listViewDefaults.FullRowSelect = true;
            this.listViewDefaults.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewDefaults.Location = new System.Drawing.Point(8, 16);
            this.listViewDefaults.MultiSelect = false;
            this.listViewDefaults.Name = "listViewDefaults";
            this.listViewDefaults.Size = new System.Drawing.Size(352, 144);
            this.listViewDefaults.TabIndex = 0;
            this.listViewDefaults.View = System.Windows.Forms.View.Details;
            this.listViewDefaults.SelectedIndexChanged += new EventHandler(this.ListViewDefaults_SelectedIndexChanged);
            // 
            // columnHeaderBusName
            // 
            this.columnHeaderBusName.Text = "Connection";
            this.columnHeaderBusName.Width = 141;
            // 
            // columnHeaderModuleType
            // 
            this.columnHeaderModuleType.Text = "Module";
            this.columnHeaderModuleType.Width = 126;
            // 
            // columnHeaderStartAddress
            // 
            this.columnHeaderStartAddress.Text = "Start Address";
            this.columnHeaderStartAddress.Width = 81;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonRemove.Location = new System.Drawing.Point(168, 166);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 20);
            this.buttonRemove.TabIndex = 3;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += new EventHandler(this.ButtonRemove_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(301, 296);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 5;
            this.buttonCancel.Text = "Cancel";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(221, 296);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new EventHandler(this.ButtonOK_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "By default new modules will be connected to: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBoxCommandStation
            // 
            this.comboBoxCommandStation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCommandStation.Location = new System.Drawing.Point(248, 64);
            this.comboBoxCommandStation.Name = "comboBoxCommandStation";
            this.comboBoxCommandStation.Size = new System.Drawing.Size(128, 21);
            this.comboBoxCommandStation.TabIndex = 2;
            // 
            // ControlModuleLocationProperties
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(386, 325);
            this.ControlBox = false;
            this.Controls.Add(this.comboBoxCommandStation);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.nameDefinition);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ControlModuleLocationProperties";
            this.Text = "Control Modules Location Properties";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private LayoutManager.CommonUI.Controls.NameDefinition nameDefinition;
        private GroupBox groupBox1;
        private ListView listViewDefaults;
        private Button buttonCancel;
        private Button buttonOK;
        private ColumnHeader columnHeaderBusName;
        private ColumnHeader columnHeaderModuleType;
        private ColumnHeader columnHeaderStartAddress;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonRemove;
        private Label label1;
        private ComboBox comboBoxCommandStation;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}
