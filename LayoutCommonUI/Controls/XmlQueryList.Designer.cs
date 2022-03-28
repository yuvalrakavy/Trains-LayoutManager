using System.ComponentModel;

namespace LayoutManager.CommonUI.Controls {
    partial class XmlQueryList {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XmlQueryList));
            this.listBox = new System.Windows.Forms.ListBox();
            this.imageListArrows = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.listBox.ItemHeight = 32;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(260, 246);
            this.listBox.TabIndex = 0;
            this.listBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.OnDrawItem);
            this.listBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.OnMeasureItem);
            this.listBox.DoubleClick += new System.EventHandler(this.OnDoubleClick);
            // 
            // imageListArrows
            // 
            this.imageListArrows.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageListArrows.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListArrows.ImageStream")));
            this.imageListArrows.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListArrows.Images.SetKeyName(0, "ArrowCollapse.ico");
            this.imageListArrows.Images.SetKeyName(1, "ArrowExpand.ico");
            // 
            // XmlQueryList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 32F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listBox);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.Name = "XmlQueryList";
            this.Size = new System.Drawing.Size(260, 246);
            this.ResumeLayout(false);

        }

        #endregion

        private ListBox listBox;
        private ImageList imageListArrows;
    }
}
