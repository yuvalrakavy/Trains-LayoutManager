using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.CommonUI;
using System;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Summary description for SelectLocomotiveType.
    /// </summary>
    partial class SelectLocomotiveType : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.xmlQueryList = new CommonUI.Controls.XmlQueryList();
            this.buttonSelect = new Button();
            this.buttonCancel = new Button();
            this.buttonArrangeBy = new Button();
            this.SuspendLayout();
            // 
            // locomotiveTypeList
            // 
            this.xmlQueryList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.xmlQueryList.ContainerElement = null;
            this.xmlQueryList.CurrentListLayout = null;
            this.xmlQueryList.CurrentListLayoutIndex = -1;
            this.xmlQueryList.DefaultSortField = "TypeName";
            this.xmlQueryList.Location = new Point(8, 8);
            this.xmlQueryList.Name = "xmlQueryList";
            this.xmlQueryList.Size = new Size(376, 376);
            this.xmlQueryList.TabIndex = 0;
            // 
            // buttonSelect
            // 
            this.buttonSelect.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonSelect.Location = new Point(252, 392);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new Size(64, 23);
            this.buttonSelect.TabIndex = 2;
            this.buttonSelect.Text = "&Select";
            this.buttonSelect.Click += new EventHandler(this.ButtonSelect_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new Point(320, 392);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new Size(64, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new EventHandler(this.ButtonCancel_Click);
            // 
            // buttonArrangeBy
            // 
            this.buttonArrangeBy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonArrangeBy.Location = new Point(8, 392);
            this.buttonArrangeBy.Name = "buttonArrangeBy";
            this.buttonArrangeBy.TabIndex = 1;
            this.buttonArrangeBy.Text = "&Arrange by";
            this.buttonArrangeBy.Click += new EventHandler(this.ButtonArrangeBy_Click);
            // 
            // SelectLocomotiveType
            // 
            this.AcceptButton = this.buttonSelect;
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoSize = true;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new Size(392, 422);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
                                                                          this.buttonArrangeBy,
                                                                          this.buttonSelect,
                                                                          this.xmlQueryList,
                                                                          this.buttonCancel});
            this.Name = "SelectLocomotiveType";
            this.ShowInTaskbar = false;
            this.Text = "Select Locomotive Type";
            this.ResumeLayout(false);
        }
        #endregion

        private CommonUI.Controls.XmlQueryList xmlQueryList;
        private Button buttonSelect;
        private Button buttonCancel;
        private Button buttonArrangeBy;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}
