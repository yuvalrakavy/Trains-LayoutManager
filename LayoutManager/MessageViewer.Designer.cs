using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics;
using System.Linq;

using LayoutManager.Model;

#pragma warning disable IDE0051, IDE0060, IDE0069

namespace LayoutManager {
    /// <summary>
    /// Summary description for MessageViewer.
    /// </summary>
    partial class MessageViewer : UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MessageViewer));
            this.listViewMessages = new System.Windows.Forms.ListView();
            this.columnHeaderMessage = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderArea = new System.Windows.Forms.ColumnHeader();
            this.imageListSeverity = new System.Windows.Forms.ImageList(this.components);
            this.buttonClose = new System.Windows.Forms.Button();
            this.buttonNextMessage = new System.Windows.Forms.Button();
            this.buttonPrevMessage = new System.Windows.Forms.Button();
            this.buttonNextComponent = new System.Windows.Forms.Button();
            this.buttonPreviousComponent = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.flowLayoutPanelLeftSideButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.flowLayoutPanelRightSideButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanelLeftSideButtons.SuspendLayout();
            this.panelButtons.SuspendLayout();
            this.flowLayoutPanelRightSideButtons.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewMessages
            // 
            this.listViewMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listViewMessages.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderMessage,
            this.columnHeaderArea});
            this.listViewMessages.FullRowSelect = true;
            this.listViewMessages.GridLines = true;
            this.listViewMessages.Location = new System.Drawing.Point(8, 77);
            this.listViewMessages.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.listViewMessages.MultiSelect = false;
            this.listViewMessages.Name = "listViewMessages";
            this.listViewMessages.Size = new System.Drawing.Size(1752, 617);
            this.listViewMessages.SmallImageList = this.imageListSeverity;
            this.listViewMessages.TabIndex = 5;
            this.listViewMessages.UseCompatibleStateImageBehavior = false;
            this.listViewMessages.View = System.Windows.Forms.View.Details;
            this.listViewMessages.SelectedIndexChanged += new System.EventHandler(this.ListViewMessages_SelectedIndexChanged);
            // 
            // columnHeaderMessage
            // 
            this.columnHeaderMessage.Text = "Message";
            this.columnHeaderMessage.Width = 393;
            // 
            // columnHeaderArea
            // 
            this.columnHeaderArea.Text = "Area";
            this.columnHeaderArea.Width = 180;
            // 
            // imageListSeverity
            // 
            this.imageListSeverity.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListSeverity.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListSeverity.ImageStream")));
            this.imageListSeverity.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSeverity.Images.SetKeyName(0, "");
            this.imageListSeverity.Images.SetKeyName(1, "");
            this.imageListSeverity.Images.SetKeyName(2, "");
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(716, 7);
            this.buttonClose.Margin = new System.Windows.Forms.Padding(50, 7, 8, 7);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(195, 54);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new System.EventHandler(this.ButtonClose_Click);
            // 
            // buttonNextMessage
            // 
            this.buttonNextMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonNextMessage.Location = new System.Drawing.Point(466, 7);
            this.buttonNextMessage.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonNextMessage.Name = "buttonNextMessage";
            this.buttonNextMessage.Size = new System.Drawing.Size(192, 54);
            this.buttonNextMessage.TabIndex = 2;
            this.buttonNextMessage.Text = "↓ message";
            this.buttonNextMessage.Click += new System.EventHandler(this.ButtonNextMessage_Click);
            // 
            // buttonPrevMessage
            // 
            this.buttonPrevMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPrevMessage.Location = new System.Drawing.Point(258, 7);
            this.buttonPrevMessage.Margin = new System.Windows.Forms.Padding(50, 7, 8, 7);
            this.buttonPrevMessage.Name = "buttonPrevMessage";
            this.buttonPrevMessage.Size = new System.Drawing.Size(192, 54);
            this.buttonPrevMessage.TabIndex = 3;
            this.buttonPrevMessage.Text = "↑ message";
            this.buttonPrevMessage.Click += new System.EventHandler(this.ButtonPrevMessage_Click);
            // 
            // buttonNextComponent
            // 
            this.buttonNextComponent.AutoSize = true;
            this.buttonNextComponent.Location = new System.Drawing.Point(8, 7);
            this.buttonNextComponent.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonNextComponent.Name = "buttonNextComponent";
            this.buttonNextComponent.Size = new System.Drawing.Size(175, 54);
            this.buttonNextComponent.TabIndex = 0;
            this.buttonNextComponent.Text = "→ component";
            this.buttonNextComponent.Click += new System.EventHandler(this.ButtonNextComponent_Click);
            // 
            // buttonPreviousComponent
            // 
            this.buttonPreviousComponent.AutoSize = true;
            this.buttonPreviousComponent.Location = new System.Drawing.Point(199, 7);
            this.buttonPreviousComponent.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonPreviousComponent.Name = "buttonPreviousComponent";
            this.buttonPreviousComponent.Size = new System.Drawing.Size(175, 54);
            this.buttonPreviousComponent.TabIndex = 1;
            this.buttonPreviousComponent.Text = "← component";
            this.buttonPreviousComponent.Click += new System.EventHandler(this.ButtonPreviousComponent_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClear.Location = new System.Drawing.Point(8, 7);
            this.buttonClear.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(192, 54);
            this.buttonClear.TabIndex = 6;
            this.buttonClear.Text = "Clear";
            this.buttonClear.Click += new System.EventHandler(this.ButtonClear_Click);
            // 
            // flowLayoutPanelLeftSideButtons
            // 
            this.flowLayoutPanelLeftSideButtons.AutoSize = true;
            this.flowLayoutPanelLeftSideButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelLeftSideButtons.Controls.Add(this.buttonNextComponent);
            this.flowLayoutPanelLeftSideButtons.Controls.Add(this.buttonPreviousComponent);
            this.flowLayoutPanelLeftSideButtons.Dock = System.Windows.Forms.DockStyle.Left;
            this.flowLayoutPanelLeftSideButtons.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelLeftSideButtons.Name = "flowLayoutPanelLeftSideButtons";
            this.flowLayoutPanelLeftSideButtons.Size = new System.Drawing.Size(382, 64);
            this.flowLayoutPanelLeftSideButtons.TabIndex = 7;
            this.flowLayoutPanelLeftSideButtons.WrapContents = false;
            // 
            // panelButtons
            // 
            this.panelButtons.Controls.Add(this.flowLayoutPanelLeftSideButtons);
            this.panelButtons.Controls.Add(this.flowLayoutPanelRightSideButtons);
            this.panelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelButtons.Location = new System.Drawing.Point(3, 3);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(1762, 64);
            this.panelButtons.TabIndex = 8;
            // 
            // flowLayoutPanelRightSideButtons
            // 
            this.flowLayoutPanelRightSideButtons.AutoSize = true;
            this.flowLayoutPanelRightSideButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanelRightSideButtons.Controls.Add(this.buttonClear);
            this.flowLayoutPanelRightSideButtons.Controls.Add(this.buttonPrevMessage);
            this.flowLayoutPanelRightSideButtons.Controls.Add(this.buttonNextMessage);
            this.flowLayoutPanelRightSideButtons.Controls.Add(this.buttonClose);
            this.flowLayoutPanelRightSideButtons.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowLayoutPanelRightSideButtons.Location = new System.Drawing.Point(843, 0);
            this.flowLayoutPanelRightSideButtons.Name = "flowLayoutPanelRightSideButtons";
            this.flowLayoutPanelRightSideButtons.Size = new System.Drawing.Size(919, 64);
            this.flowLayoutPanelRightSideButtons.TabIndex = 9;
            this.flowLayoutPanelRightSideButtons.WrapContents = false;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 1;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.listViewMessages, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.panelButtons, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(1768, 701);
            this.tableLayoutPanel.TabIndex = 9;
            // 
            // MessageViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "MessageViewer";
            this.Size = new System.Drawing.Size(1768, 701);
            this.flowLayoutPanelLeftSideButtons.ResumeLayout(false);
            this.flowLayoutPanelLeftSideButtons.PerformLayout();
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            this.flowLayoutPanelRightSideButtons.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private ListView listViewMessages;
        private ColumnHeader columnHeaderMessage;
        private ColumnHeader columnHeaderArea;
        private Button buttonClose;
        private Button buttonPrevMessage;
        private Button buttonNextMessage;
        private Button buttonPreviousComponent;
        private Button buttonNextComponent;
        private ImageList imageListSeverity;
        private Button buttonClear;
        private IContainer components;
        private FlowLayoutPanel flowLayoutPanelLeftSideButtons;
        private Panel panelButtons;
        private FlowLayoutPanel flowLayoutPanelRightSideButtons;
        private TableLayoutPanel tableLayoutPanel;
    }
}

