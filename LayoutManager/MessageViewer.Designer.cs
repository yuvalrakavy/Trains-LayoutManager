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
            this.components = new Container();
            ComponentResourceManager resources = new(typeof(MessageViewer));
            this.listViewMessages = new ListView();
            this.columnHeaderMessage = (ColumnHeader)new ColumnHeader();
            this.columnHeaderArea = (ColumnHeader)new ColumnHeader();
            this.imageListSeverity = new ImageList(this.components);
            this.buttonClose = new Button();
            this.buttonNextMessage = new Button();
            this.buttonPrevMessage = new Button();
            this.buttonNextComponent = new Button();
            this.buttonPreviousComponent = new Button();
            this.buttonClear = new Button();
            this.SuspendLayout();
            // 
            // listViewMessages
            // 
            this.listViewMessages.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
            | System.Windows.Forms.AnchorStyles.Left
            | System.Windows.Forms.AnchorStyles.Right);
            this.listViewMessages.Columns.AddRange(new ColumnHeader[] {
            this.columnHeaderMessage,
            this.columnHeaderArea});
            this.listViewMessages.FullRowSelect = true;
            this.listViewMessages.GridLines = true;
            this.listViewMessages.HideSelection = false;
            this.listViewMessages.Location = new Point(0, 31);
            this.listViewMessages.MultiSelect = false;
            this.listViewMessages.Name = "listViewMessages";
            this.listViewMessages.Size = new Size(680, 80);
            this.listViewMessages.SmallImageList = this.imageListSeverity;
            this.listViewMessages.TabIndex = 5;
            this.listViewMessages.UseCompatibleStateImageBehavior = false;
            this.listViewMessages.View = System.Windows.Forms.View.Details;
            this.listViewMessages.SelectedIndexChanged += new EventHandler(this.ListViewMessages_SelectedIndexChanged);
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
            this.imageListSeverity.ImageStream = (ImageListStreamer)resources.GetObject("imageListSeverity.ImageStream");
            this.imageListSeverity.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListSeverity.Images.SetKeyName(0, "");
            this.imageListSeverity.Images.SetKeyName(1, "");
            this.imageListSeverity.Images.SetKeyName(2, "");
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClose.Location = new Point(598, 4);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new Size(75, 22);
            this.buttonClose.TabIndex = 4;
            this.buttonClose.Text = "&Close";
            this.buttonClose.Click += new EventHandler(this.ButtonClose_Click);
            // 
            // buttonNextMessage
            // 
            this.buttonNextMessage.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonNextMessage.Location = new Point(419, 4);
            this.buttonNextMessage.Name = "buttonNextMessage";
            this.buttonNextMessage.Size = new Size(74, 22);
            this.buttonNextMessage.TabIndex = 2;
            this.buttonNextMessage.Text = "↓ message";
            this.buttonNextMessage.Click += new EventHandler(this.ButtonNextMessage_Click);
            // 
            // buttonPrevMessage
            // 
            this.buttonPrevMessage.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonPrevMessage.Location = new Point(499, 4);
            this.buttonPrevMessage.Name = "buttonPrevMessage";
            this.buttonPrevMessage.Size = new Size(74, 22);
            this.buttonPrevMessage.TabIndex = 3;
            this.buttonPrevMessage.Text = "↑ message";
            this.buttonPrevMessage.Click += new EventHandler(this.ButtonPrevMessage_Click);
            // 
            // buttonNextComponent
            // 
            this.buttonNextComponent.Location = new Point(8, 4);
            this.buttonNextComponent.Name = "buttonNextComponent";
            this.buttonNextComponent.Size = new Size(91, 22);
            this.buttonNextComponent.TabIndex = 0;
            this.buttonNextComponent.Text = "→ component";
            this.buttonNextComponent.Click += new EventHandler(this.ButtonNextComponent_Click);
            // 
            // buttonPreviousComponent
            // 
            this.buttonPreviousComponent.Location = new Point(105, 4);
            this.buttonPreviousComponent.Name = "buttonPreviousComponent";
            this.buttonPreviousComponent.Size = new Size(91, 22);
            this.buttonPreviousComponent.TabIndex = 1;
            this.buttonPreviousComponent.Text = "← component";
            this.buttonPreviousComponent.Click += new EventHandler(this.ButtonPreviousComponent_Click);
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = (AnchorStyles)(System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right);
            this.buttonClear.Location = new Point(324, 4);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new Size(74, 22);
            this.buttonClear.TabIndex = 6;
            this.buttonClear.Text = "Clear";
            this.buttonClear.Click += new EventHandler(this.ButtonClear_Click);
            // 
            // MessageViewer
            // 
            this.Controls.Add(this.buttonClear);
            this.Controls.Add(this.buttonPreviousComponent);
            this.Controls.Add(this.buttonNextComponent);
            this.Controls.Add(this.buttonPrevMessage);
            this.Controls.Add(this.buttonNextMessage);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.listViewMessages);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.Name = "MessageViewer";
            this.Size = new Size(680, 112);
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
    }
}

