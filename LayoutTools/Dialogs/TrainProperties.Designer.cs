using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.CommonUI;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for TrainProperties.
    /// </summary>
    partial class TrainProperties : Form, IObjectHasXml {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TrainProperties));
            this.tabControl1 = new TabControl();
            this.tabPageGeneral = new TabPage();
            this.label2 = new Label();
            this.groupBox2 = new GroupBox();
            this.checkBoxLastCarDetected = new CheckBox();
            this.checkBoxMagnetOnLastCar = new CheckBox();
            this.textBoxName = new TextBox();
            this.label1 = new Label();
            this.tabPageLocomotives = new TabPage();
            this.panelLocoImages = new Panel();
            this.buttonLocoMoveDown = new Button();
            this.imageListButttons = new ImageList(this.components);
            this.buttonLocoAdd = new Button();
            this.listViewLocomotives = new ListView();
            this.columnHeaderLocoName = new ColumnHeader();
            this.columnAddress = new ColumnHeader();
            this.columnHeaderLocoOrientation = new ColumnHeader();
            this.columnHeaderLocoType = new ColumnHeader();
            this.buttonLocoRemove = new Button();
            this.buttonLocoEdit = new Button();
            this.buttonLocoMoveUp = new Button();
            this.tabPageDriver = new TabPage();
            this.tabPageAttributes = new TabPage();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.contextMenuEditLocomotive = new ContextMenuStrip();
            this.menuItemLocoOrientation = new ToolStripMenuItem();
            this.menuItemLocoOrientationForward = new ToolStripMenuItem();
            this.menuItemLocoOrientationBackward = new ToolStripMenuItem();
            this.menuItemEditLocoDefinition = new ToolStripMenuItem();
            this.groupBox1 = new GroupBox();
            this.trainLengthDiagram = new LayoutManager.CommonUI.Controls.TrainLengthDiagram();
            this.trainDriverComboBox = new LayoutManager.CommonUI.Controls.TrainDriverComboBox();
            this.drivingParameters = new LayoutManager.CommonUI.Controls.DrivingParameters();
            this.attributesEditor = new LayoutManager.CommonUI.Controls.AttributesEditor();
            this.tabControl1.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPageLocomotives.SuspendLayout();
            this.tabPageDriver.SuspendLayout();
            this.tabPageAttributes.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.tabControl1.Controls.Add(this.tabPageGeneral);
            this.tabControl1.Controls.Add(this.tabPageLocomotives);
            this.tabControl1.Controls.Add(this.tabPageDriver);
            this.tabControl1.Controls.Add(this.tabPageAttributes);
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(296, 272);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.groupBox1);
            this.tabPageGeneral.Controls.Add(this.trainDriverComboBox);
            this.tabPageGeneral.Controls.Add(this.label2);
            this.tabPageGeneral.Controls.Add(this.groupBox2);
            this.tabPageGeneral.Controls.Add(this.textBoxName);
            this.tabPageGeneral.Controls.Add(this.label1);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Size = new System.Drawing.Size(288, 246);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(10, 202);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(264, 16);
            this.label2.TabIndex = 7;
            this.label2.Text = "When driven according to a trip-plan, train is driven:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBoxLastCarDetected);
            this.groupBox2.Controls.Add(this.checkBoxMagnetOnLastCar);
            this.groupBox2.Location = new System.Drawing.Point(8, 132);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(272, 64);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Train last car:";
            // 
            // checkBoxLastCarDetected
            // 
            this.checkBoxLastCarDetected.Location = new System.Drawing.Point(7, 32);
            this.checkBoxLastCarDetected.Name = "checkBoxLastCarDetected";
            this.checkBoxLastCarDetected.Size = new System.Drawing.Size(256, 24);
            this.checkBoxLastCarDetected.TabIndex = 7;
            this.checkBoxLastCarDetected.Text = "Detected by train occupancy detection blocks";
            // 
            // checkBoxMagnetOnLastCar
            // 
            this.checkBoxMagnetOnLastCar.Location = new System.Drawing.Point(7, 15);
            this.checkBoxMagnetOnLastCar.Name = "checkBoxMagnetOnLastCar";
            this.checkBoxMagnetOnLastCar.Size = new System.Drawing.Size(240, 18);
            this.checkBoxMagnetOnLastCar.TabIndex = 6;
            this.checkBoxMagnetOnLastCar.Text = "Triggers track contact sensor";
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(72, 22);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(176, 20);
            this.textBoxName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // tabPageLocomotives
            // 
            this.tabPageLocomotives.Controls.Add(this.panelLocoImages);
            this.tabPageLocomotives.Controls.Add(this.buttonLocoMoveDown);
            this.tabPageLocomotives.Controls.Add(this.buttonLocoAdd);
            this.tabPageLocomotives.Controls.Add(this.listViewLocomotives);
            this.tabPageLocomotives.Controls.Add(this.buttonLocoRemove);
            this.tabPageLocomotives.Controls.Add(this.buttonLocoEdit);
            this.tabPageLocomotives.Controls.Add(this.buttonLocoMoveUp);
            this.tabPageLocomotives.Location = new System.Drawing.Point(4, 22);
            this.tabPageLocomotives.Name = "tabPageLocomotives";
            this.tabPageLocomotives.Size = new System.Drawing.Size(288, 246);
            this.tabPageLocomotives.TabIndex = 1;
            this.tabPageLocomotives.Text = "Locomotives";
            // 
            // panelLocoImages
            // 
            this.panelLocoImages.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.panelLocoImages.Location = new System.Drawing.Point(8, 6);
            this.panelLocoImages.Name = "panelLocoImages";
            this.panelLocoImages.Size = new System.Drawing.Size(272, 42);
            this.panelLocoImages.TabIndex = 5;
            this.panelLocoImages.MouseDown += this.PanelLocoImages_MouseDown;
            this.panelLocoImages.Paint += this.PanelLocoImages_Paint;
            // 
            // buttonLocoMoveDown
            // 
            this.buttonLocoMoveDown.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonLocoMoveDown.ImageIndex = 0;
            this.buttonLocoMoveDown.ImageList = this.imageListButttons;
            this.buttonLocoMoveDown.Location = new System.Drawing.Point(222, 222);
            this.buttonLocoMoveDown.Name = "buttonLocoMoveDown";
            this.buttonLocoMoveDown.Size = new System.Drawing.Size(29, 21);
            this.buttonLocoMoveDown.TabIndex = 4;
            this.buttonLocoMoveDown.Click += this.ButtonLocoMoveDown_Click;
            // 
            // imageListButttons
            // 
            this.imageListButttons.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("imageListButttons.ImageStream");
            this.imageListButttons.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListButttons.Images.SetKeyName(0, "");
            this.imageListButttons.Images.SetKeyName(1, "");
            // 
            // buttonLocoAdd
            // 
            this.buttonLocoAdd.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonLocoAdd.Location = new System.Drawing.Point(8, 222);
            this.buttonLocoAdd.Name = "buttonLocoAdd";
            this.buttonLocoAdd.Size = new System.Drawing.Size(64, 21);
            this.buttonLocoAdd.TabIndex = 1;
            this.buttonLocoAdd.Text = "&Add...";
            this.buttonLocoAdd.Click += this.ButtonLocoAdd_Click;
            // 
            // listViewLocomotives
            // 
            this.listViewLocomotives.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                        | System.Windows.Forms.AnchorStyles.Left
                        | System.Windows.Forms.AnchorStyles.Right);
            this.listViewLocomotives.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderLocoName,
            this.columnAddress,
            this.columnHeaderLocoOrientation,
            this.columnHeaderLocoType});
            this.listViewLocomotives.FullRowSelect = true;
            this.listViewLocomotives.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listViewLocomotives.HideSelection = false;
            this.listViewLocomotives.Location = new System.Drawing.Point(8, 56);
            this.listViewLocomotives.MultiSelect = false;
            this.listViewLocomotives.Name = "listViewLocomotives";
            this.listViewLocomotives.Size = new System.Drawing.Size(272, 162);
            this.listViewLocomotives.TabIndex = 0;
            this.listViewLocomotives.UseCompatibleStateImageBehavior = false;
            this.listViewLocomotives.View = System.Windows.Forms.View.Details;
            this.listViewLocomotives.SelectedIndexChanged += this.UpdateControls;
            // 
            // columnHeaderLocoName
            // 
            this.columnHeaderLocoName.Text = "Name";
            this.columnHeaderLocoName.Width = 78;
            // 
            // columnAddress
            // 
            this.columnAddress.Text = "Address";
            this.columnAddress.Width = 51;
            // 
            // columnHeaderLocoOrientation
            // 
            this.columnHeaderLocoOrientation.Text = "Orientation";
            this.columnHeaderLocoOrientation.Width = 65;
            // 
            // columnHeaderLocoType
            // 
            this.columnHeaderLocoType.Text = "Type";
            this.columnHeaderLocoType.Width = 73;
            // 
            // buttonLocoRemove
            // 
            this.buttonLocoRemove.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonLocoRemove.Location = new System.Drawing.Point(80, 222);
            this.buttonLocoRemove.Name = "buttonLocoRemove";
            this.buttonLocoRemove.Size = new System.Drawing.Size(64, 21);
            this.buttonLocoRemove.TabIndex = 2;
            this.buttonLocoRemove.Text = "&Remove";
            this.buttonLocoRemove.Click += this.ButtonLocoRemove_Click;
            // 
            // buttonLocoEdit
            // 
            this.buttonLocoEdit.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left);
            this.buttonLocoEdit.Location = new System.Drawing.Point(152, 222);
            this.buttonLocoEdit.Name = "buttonLocoEdit";
            this.buttonLocoEdit.Size = new System.Drawing.Size(64, 21);
            this.buttonLocoEdit.TabIndex = 3;
            this.buttonLocoEdit.Text = "&Edit...";
            this.buttonLocoEdit.Click += this.ButtonLocoEdit_Click;
            // 
            // buttonLocoMoveUp
            // 
            this.buttonLocoMoveUp.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonLocoMoveUp.ImageIndex = 1;
            this.buttonLocoMoveUp.ImageList = this.imageListButttons;
            this.buttonLocoMoveUp.Location = new System.Drawing.Point(251, 222);
            this.buttonLocoMoveUp.Name = "buttonLocoMoveUp";
            this.buttonLocoMoveUp.Size = new System.Drawing.Size(29, 21);
            this.buttonLocoMoveUp.TabIndex = 4;
            this.buttonLocoMoveUp.Click += this.ButtonLocoMoveUp_Click;
            // 
            // tabPageDriver
            // 
            this.tabPageDriver.Controls.Add(this.drivingParameters);
            this.tabPageDriver.Location = new System.Drawing.Point(4, 22);
            this.tabPageDriver.Name = "tabPageDriver";
            this.tabPageDriver.Size = new System.Drawing.Size(288, 246);
            this.tabPageDriver.TabIndex = 4;
            this.tabPageDriver.Text = "Driver";
            // 
            // tabPageAttributes
            // 
            this.tabPageAttributes.Controls.Add(this.attributesEditor);
            this.tabPageAttributes.Location = new System.Drawing.Point(4, 22);
            this.tabPageAttributes.Name = "tabPageAttributes";
            this.tabPageAttributes.Size = new System.Drawing.Size(288, 246);
            this.tabPageAttributes.TabIndex = 3;
            this.tabPageAttributes.Text = "Attributes";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonOK.Location = new System.Drawing.Point(111, 280);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 21);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += this.ButtonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = (System.Windows.Forms.AnchorStyles)(System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right);
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(191, 280);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 21);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.ButtonCancel_Click;
            // 
            // contextMenuEditLocomotive
            // 
            this.contextMenuEditLocomotive.Items.AddRange(new ToolStripMenuItem[] {
            this.menuItemLocoOrientation,
            this.menuItemEditLocoDefinition});
            // 
            // menuItemLocoOrientation
            // 
            this.menuItemLocoOrientation.DropDownItems.AddRange(new ToolStripMenuItem[] {
            this.menuItemLocoOrientationForward,
            this.menuItemLocoOrientationBackward});
            this.menuItemLocoOrientation.Text = "Orientation";
            // 
            // menuItemLocoOrientationForward
            // 
            this.menuItemLocoOrientationForward.Text = "Forward";
            this.menuItemLocoOrientationForward.Click += this.MenuItemLocoOrientationForward_Click;
            // 
            // menuItemLocoOrientationBackward
            // 
            this.menuItemLocoOrientationBackward.Text = "Backward";
            this.menuItemLocoOrientationBackward.Click += this.MenuItemLocoOrientationBackward_Click;
            // 
            // menuItemEditLocoDefinition
            // 
            this.menuItemEditLocoDefinition.Text = "Definition...";
            this.menuItemEditLocoDefinition.Click += this.MenuItemEditLocoDefinition_Click;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.trainLengthDiagram);
            this.groupBox1.Location = new System.Drawing.Point(8, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(272, 78);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Train Length:";
            // 
            // trainLengthDiagram
            // 
            this.trainLengthDiagram.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.trainLengthDiagram.Comparison = LayoutManager.Model.TrainLengthComparison.None;
            this.trainLengthDiagram.Length = LayoutManager.Model.TrainLength.Standard;
            this.trainLengthDiagram.Location = new System.Drawing.Point(45, 19);
            this.trainLengthDiagram.Name = "trainLengthDiagram";
            this.trainLengthDiagram.Size = new System.Drawing.Size(180, 52);
            this.trainLengthDiagram.TabIndex = 9;
            // 
            // trainDriverComboBox
            // 
            this.trainDriverComboBox.Location = new System.Drawing.Point(8, 220);
            this.trainDriverComboBox.Name = "trainDriverComboBox";
            this.trainDriverComboBox.Size = new System.Drawing.Size(272, 24);
            this.trainDriverComboBox.TabIndex = 8;
            this.trainDriverComboBox.OptionalTrain = null;
            // 
            // drivingParameters
            // 
            this.drivingParameters.Dock = System.Windows.Forms.DockStyle.Fill;
            this.drivingParameters.OptionalElement = null;
            this.drivingParameters.Location = new System.Drawing.Point(0, 0);
            this.drivingParameters.Name = "drivingParameters";
            this.drivingParameters.Size = new System.Drawing.Size(288, 246);
            this.drivingParameters.TabIndex = 0;
            // 
            // attributesEditor
            // 
            this.attributesEditor.AttributesOwner = null;
            this.attributesEditor.AttributesSource = null;
            this.attributesEditor.Location = new System.Drawing.Point(8, 8);
            this.attributesEditor.Name = "attributesEditor";
            this.attributesEditor.Size = new System.Drawing.Size(272, 224);
            this.attributesEditor.TabIndex = 0;
            this.attributesEditor.ViewOnly = false;
            // 
            // TrainProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 310);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.MinimumSize = new System.Drawing.Size(300, 300);
            this.Name = "TrainProperties";
            this.ShowInTaskbar = false;
            this.Text = "Train Properties";
            this.Closed += this.TrainProperties_Closed;
            this.tabControl1.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageGeneral.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tabPageLocomotives.ResumeLayout(false);
            this.tabPageDriver.ResumeLayout(false);
            this.tabPageAttributes.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
        }
        #endregion
        private TabControl tabControl1;
        private Button buttonOK;
        private Button buttonCancel;
        private TabPage tabPageGeneral;
        private TabPage tabPageLocomotives;
        private Label label1;
        private TextBox textBoxName;
        private ListView listViewLocomotives;
        private ColumnHeader columnHeaderLocoName;
        private ColumnHeader columnAddress;
        private ColumnHeader columnHeaderLocoOrientation;
        private ColumnHeader columnHeaderLocoType;
        private Button buttonLocoAdd;
        private Button buttonLocoRemove;
        private Button buttonLocoEdit;
        private ContextMenuStrip contextMenuEditLocomotive;
        private ToolStripMenuItem menuItemLocoOrientation;
        private ToolStripMenuItem menuItemLocoOrientationForward;
        private ToolStripMenuItem menuItemLocoOrientationBackward;
        private ToolStripMenuItem menuItemEditLocoDefinition;
        private Button buttonLocoMoveDown;
        private ImageList imageListButttons;
        private Button buttonLocoMoveUp;
        private Panel panelLocoImages;
        private TabPage tabPageAttributes;
        private LayoutManager.CommonUI.Controls.AttributesEditor attributesEditor;
        private TabPage tabPageDriver;
        private GroupBox groupBox2;
        private CheckBox checkBoxMagnetOnLastCar;
        private CheckBox checkBoxLastCarDetected;
        private Label label2;
        private LayoutManager.CommonUI.Controls.TrainDriverComboBox trainDriverComboBox;
        private LayoutManager.CommonUI.Controls.DrivingParameters drivingParameters;
        private LayoutManager.CommonUI.Controls.TrainLengthDiagram trainLengthDiagram;
        private GroupBox groupBox1;
        private IContainer components;
    }
}