using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;
using LayoutManager.CommonUI;

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
            this.locomotiveTypeList = new CommonUI.Controls.LocomotiveTypeList();
            this.buttonSelect = new Button();
            this.buttonCancel = new Button();
            this.buttonArrangeBy = new Button();
            this.SuspendLayout();
            // 
            // locomotiveTypeList
            // 
            this.locomotiveTypeList.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.locomotiveTypeList.ContainerElement = null;
            this.locomotiveTypeList.CurrentListLayout = null;
            this.locomotiveTypeList.CurrentListLayoutIndex = -1;
            this.locomotiveTypeList.DefaultSortField = "TypeName";
            this.locomotiveTypeList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.locomotiveTypeList.Location = new Point(8, 8);
            this.locomotiveTypeList.Name = "locomotiveTypeList";
            this.locomotiveTypeList.Size = new Size(376, 376);
            this.locomotiveTypeList.TabIndex = 0;
            this.locomotiveTypeList.SelectedIndexChanged += this.LocomotiveTypeList_SelectedIndexChanged;
            // 
            // buttonSelect
            // 
            this.buttonSelect.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonSelect.Location = new Point(252, 392);
            this.buttonSelect.Name = "buttonSelect";
            this.buttonSelect.Size = new Size(64, 23);
            this.buttonSelect.TabIndex = 2;
            this.buttonSelect.Text = "&Select";
            this.buttonSelect.Click += this.ButtonSelect_Click;
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
            this.buttonCancel.Click += this.ButtonCancel_Click;
            // 
            // buttonArrangeBy
            // 
            this.buttonArrangeBy.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonArrangeBy.Location = new Point(8, 392);
            this.buttonArrangeBy.Name = "buttonArrangeBy";
            this.buttonArrangeBy.TabIndex = 1;
            this.buttonArrangeBy.Text = "&Arrange by";
            this.buttonArrangeBy.Click += this.ButtonArrangeBy_Click;
            // 
            // SelectLocomotiveType
            // 
            this.AcceptButton = this.buttonSelect;
            this.AutoScaleDimensions = new SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new Size(392, 422);
            this.ControlBox = false;
            this.Controls.AddRange(new Control[] {
                                                                          this.buttonArrangeBy,
                                                                          this.buttonSelect,
                                                                          this.locomotiveTypeList,
                                                                          this.buttonCancel});
            this.Name = "SelectLocomotiveType";
            this.ShowInTaskbar = false;
            this.Text = "Select Locomotive Type";
            this.ResumeLayout(false);
        }
        #endregion

        private CommonUI.Controls.LocomotiveTypeList locomotiveTypeList;
        private Button buttonSelect;
        private Button buttonCancel;
        private Button buttonArrangeBy;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
    }
}
