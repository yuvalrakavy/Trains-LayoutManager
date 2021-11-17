using System.ComponentModel;
using System.Windows.Forms;

using LayoutManager;

namespace LayoutEventDebugger {
    /// <summary>
    /// Summary description for EventTrace.
    /// </summary>
    partial class EventTrace : Form {
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.treeViewEventTrace = new TreeView();
            this.buttonTraceState = new Button();
            this.buttonClose = new Button();
            this.buttonClear = new Button();
            this.SuspendLayout();
            // 
            // treeViewEventTrace
            // 
            this.treeViewEventTrace.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.treeViewEventTrace.ImageIndex = -1;
            this.treeViewEventTrace.Location = new System.Drawing.Point(8, 8);
            this.treeViewEventTrace.Name = "treeViewEventTrace";
            this.treeViewEventTrace.SelectedImageIndex = -1;
            this.treeViewEventTrace.Size = new System.Drawing.Size(280, 344);
            this.treeViewEventTrace.TabIndex = 0;
            // 
            // buttonTraceState
            // 
            this.buttonTraceState.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonTraceState.Location = new System.Drawing.Point(8, 360);
            this.buttonTraceState.Name = "buttonTraceState";
            this.buttonTraceState.Size = new System.Drawing.Size(72, 24);
            this.buttonTraceState.TabIndex = 1;
            this.buttonTraceState.Text = "Start trace";
            this.buttonTraceState.Click += this.ButtonTraceState_Click;
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonClose.Location = new System.Drawing.Point(216, 360);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.TabIndex = 3;
            this.buttonClose.Text = "Close";
            this.buttonClose.Click += this.ButtonClose_Click;
            // 
            // buttonClear
            // 
            this.buttonClear.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonClear.Location = new System.Drawing.Point(88, 360);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(75, 24);
            this.buttonClear.TabIndex = 2;
            this.buttonClear.Text = "Clear";
            this.buttonClear.Click += this.ButtonClear_Click;
            // 
            // EventTrace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(296, 389);
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonClear,
                                                                          this.buttonClose,
                                                                          this.buttonTraceState,
                                                                          this.treeViewEventTrace});
            this.Name = "EventTrace";
            this.Text = "Event Trace";
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Closed += this.EventTrace_Closed;
            this.ResumeLayout(false);
        }
        #endregion

        private TreeView treeViewEventTrace;
        private Button buttonTraceState;
        private Button buttonClose;
        private Button buttonClear;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

    }
}
