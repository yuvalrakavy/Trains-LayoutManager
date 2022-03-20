namespace LayoutManager.CommonUI.Controls {
    /// <summary>
    /// Summary description for PositionDefinition.
    /// </summary>
    public partial class PositionDefinition : System.Windows.Forms.UserControl {
        #region Component Designer generated code
        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.layoutPositionInfoPreview1 = new LayoutManager.CommonUI.Controls.LayoutPositionInfoPreview();
            this.labelAlignment = new Label();
            this.numericUpDownWidth = new NumericUpDown();
            this.labelPosition = new Label();
            this.enumComboBoxAlignment = new LayoutManager.CommonUI.Controls.EnumComboBox();
            this.enumComboBoxPosition = new LayoutManager.CommonUI.Controls.EnumComboBox();
            this.label1 = new Label();
            this.labelDistance = new Label();
            this.numericUpDownDistance = new NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownWidth).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownDistance).BeginInit();
            this.SuspendLayout();
            // 
            // layoutPositionInfoPreview1
            // 
            this.layoutPositionInfoPreview1.Alignment = LayoutManager.Model.LayoutDrawingAnchorPoint.Center;
            this.layoutPositionInfoPreview1.Distance = 20;
            this.layoutPositionInfoPreview1.Location = new System.Drawing.Point(168, 5);
            this.layoutPositionInfoPreview1.Name = "layoutPositionInfoPreview1";
            this.layoutPositionInfoPreview1.Side = LayoutManager.Model.LayoutDrawingSide.Top;
            this.layoutPositionInfoPreview1.Size = new System.Drawing.Size(88, 88);
            this.layoutPositionInfoPreview1.TabIndex = 8;
            this.layoutPositionInfoPreview1.Text = "layoutPositionInfoPreview1";
            // 
            // labelAlignment
            // 
            this.labelAlignment.Location = new System.Drawing.Point(8, 78);
            this.labelAlignment.Name = "labelAlignment";
            this.labelAlignment.Size = new System.Drawing.Size(60, 16);
            this.labelAlignment.TabIndex = 1;
            this.labelAlignment.Text = "Alignment:";
            this.labelAlignment.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDownWidth
            // 
            this.numericUpDownWidth.Location = new System.Drawing.Point(72, 53);
            this.numericUpDownWidth.Name = "numericUpDownWidth";
            this.numericUpDownWidth.Size = new System.Drawing.Size(52, 20);
            this.numericUpDownWidth.TabIndex = 5;
            this.numericUpDownWidth.ValueChanged += new EventHandler(this.NumericUpDownWidth_ValueChanged);
            // 
            // labelPosition
            // 
            this.labelPosition.Location = new System.Drawing.Point(8, 6);
            this.labelPosition.Name = "labelPosition";
            this.labelPosition.Size = new System.Drawing.Size(48, 16);
            this.labelPosition.TabIndex = 1;
            this.labelPosition.Text = "Position:";
            this.labelPosition.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // enumComboBoxAlignment
            // 
            this.enumComboBoxAlignment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.enumComboBoxAlignment.DropDownWidth = 84;
            this.enumComboBoxAlignment.FormattingEnabled = true;
            this.enumComboBoxAlignment.Location = new System.Drawing.Point(72, 76);
            this.enumComboBoxAlignment.Name = "enumComboBoxAlignment";
            this.enumComboBoxAlignment.Size = new System.Drawing.Size(84, 21);
            this.enumComboBoxAlignment.TabIndex = 7;
            this.enumComboBoxAlignment.SelectedIndexChanged += new EventHandler(this.EnumComboBoxAlignment_SelectedIndexChanged);
            // 
            // enumComboBoxPosition
            // 
            this.enumComboBoxPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.enumComboBoxPosition.DropDownWidth = 84;
            this.enumComboBoxPosition.FormattingEnabled = true;
            this.enumComboBoxPosition.Location = new System.Drawing.Point(72, 4);
            this.enumComboBoxPosition.Name = "enumComboBoxPosition";
            this.enumComboBoxPosition.Size = new System.Drawing.Size(84, 21);
            this.enumComboBoxPosition.TabIndex = 6;
            this.enumComboBoxPosition.SelectedIndexChanged += new EventHandler(this.EnumComboBoxPosition_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 19);
            this.label1.TabIndex = 9;
            this.label1.Text = "Width:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labelDistance
            // 
            this.labelDistance.Location = new System.Drawing.Point(8, 31);
            this.labelDistance.Name = "labelDistance";
            this.labelDistance.Size = new System.Drawing.Size(52, 16);
            this.labelDistance.TabIndex = 1;
            this.labelDistance.Text = "Distance:";
            this.labelDistance.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // numericUpDownDistance
            // 
            this.numericUpDownDistance.Location = new System.Drawing.Point(72, 29);
            this.numericUpDownDistance.Name = "numericUpDownDistance";
            this.numericUpDownDistance.Size = new System.Drawing.Size(52, 20);
            this.numericUpDownDistance.TabIndex = 5;
            this.numericUpDownDistance.ValueChanged += new EventHandler(this.NumericUpDownDistance_ValueChanged);
            // 
            // PositionDefinition
            // 
            this.Controls.Add(this.numericUpDownWidth);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.layoutPositionInfoPreview1);
            this.Controls.Add(this.enumComboBoxAlignment);
            this.Controls.Add(this.enumComboBoxPosition);
            this.Controls.Add(this.numericUpDownDistance);
            this.Controls.Add(this.labelDistance);
            this.Controls.Add(this.labelAlignment);
            this.Controls.Add(this.labelPosition);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoSize = true;
            this.Name = "PositionDefinition";
            this.Size = new System.Drawing.Size(272, 104);
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownWidth).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.numericUpDownDistance).EndInit();
            this.ResumeLayout(false);
        }
        #endregion
        private NumericUpDown numericUpDownDistance;
        private Label labelDistance;
        private Label labelAlignment;
        private Label labelPosition;
        private LayoutManager.CommonUI.Controls.EnumComboBox enumComboBoxPosition;
        private LayoutManager.CommonUI.Controls.EnumComboBox enumComboBoxAlignment;
        private LayoutManager.CommonUI.Controls.LayoutPositionInfoPreview layoutPositionInfoPreview1;
        private Label label1;
        private NumericUpDown numericUpDownWidth;

        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private readonly System.ComponentModel.Container components = null;
    }
}