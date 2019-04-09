using System;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for RunPolicy.
    /// </summary>
    public class RunPolicy : Form {
        private const string A_PolicyID = "PolicyID";
        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private ComboBox comboBoxPolicy;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private readonly XmlElement element;

        public RunPolicy(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            comboBoxPolicy.Items.Add(new GlobalOrLayoutEntry(this, "Policies for all layout"));

            addPoliciesOfScope(LayoutModel.Instance.GlobalPoliciesElement, "Global", "Layout wide policies");
            addPoliciesOfScope(LayoutModel.Instance.GlobalPoliciesElement, "TripPlan", "Trip plan policies");
            addPoliciesOfScope(LayoutModel.Instance.GlobalPoliciesElement, "Block", "Block policies");

            comboBoxPolicy.Items.Add(new GlobalOrLayoutEntry(this, "Policies for this layout only"));

            addPoliciesOfScope(LayoutModel.StateManager.LayoutPoliciesElement, "Global", "Layout wide policies");
            addPoliciesOfScope(LayoutModel.StateManager.LayoutPoliciesElement, "TripPlan", "Trip plan policies");
            addPoliciesOfScope(LayoutModel.StateManager.LayoutPoliciesElement, "Block", "Block policies");

            if (element.HasAttribute(A_PolicyID)) {
                var policyID = (Guid)element.AttributeValue(A_PolicyID);

                foreach (PolicyEntryBase entry in comboBoxPolicy.Items) {
                    if (entry is PolicyEntry && ((PolicyEntry)entry).Policy.Id == policyID) {
                        comboBoxPolicy.SelectedItem = entry;
                        break;
                    }
                }
            }
        }

        private void addPoliciesOfScope(XmlElement policiesCollectionElement, string scope, string scopeName) {
            LayoutPoliciesCollection policies = new LayoutPoliciesCollection(policiesCollectionElement, null, scope);

            if (policies.Count > 0) {
                comboBoxPolicy.Items.Add(new PolicyUsageEntry(this, scopeName));

                foreach (LayoutPolicyInfo policy in policies)
                    comboBoxPolicy.Items.Add(new PolicyEntry(this, policy));
            }
        }

        private Font PolicyNameFont => comboBoxPolicy.Font;

        private Font GlobalOrLayoutFont { get; } = new Font("Arial", 10F);

        private Font PolicyUsageFont { get; } = new Font("Arial", 9F);

        private Pen GlobalOrLayoutUnderlinePen { get; } = new Pen(Brushes.DarkCyan, 2F);

        private Pen PolicyUsageUnderlinePen { get; } = new Pen(Brushes.Blue, 1F);

        private Brush GlobalOrLayoutBackBrush { get; } = Brushes.Khaki;

        private Brush PolicyUsageBackBrush { get; } = Brushes.LemonChiffon;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null) {
                    GlobalOrLayoutFont.Dispose();
                    PolicyUsageFont.Dispose();
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.comboBoxPolicy = new ComboBox();
            this.label1 = new Label();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // comboBoxPolicy
            // 
            this.comboBoxPolicy.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPolicy.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.comboBoxPolicy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPolicy.Location = new System.Drawing.Point(8, 26);
            this.comboBoxPolicy.MaxDropDownItems = 15;
            this.comboBoxPolicy.Name = "comboBoxPolicy";
            this.comboBoxPolicy.Size = new System.Drawing.Size(272, 21);
            this.comboBoxPolicy.TabIndex = 0;
            this.comboBoxPolicy.SelectedIndexChanged += new System.EventHandler(this.comboBoxPolicy_SelectedIndexChanged);
            this.comboBoxPolicy.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(this.comboBoxPolicy_MeasureItem);
            this.comboBoxPolicy.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.comboBoxPolicy_DrawItem);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Run policy:";
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(128, 58);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(208, 58);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            // 
            // RunPolicy
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(292, 86);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxPolicy);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.Name = "RunPolicy";
            this.ShowInTaskbar = false;
            this.Text = "Run Policy";
            this.ResumeLayout(false);
        }
        #endregion

        private void comboBoxPolicy_MeasureItem(object sender, System.Windows.Forms.MeasureItemEventArgs e) {
            if (e.Index >= 0) {
                PolicyEntryBase entry = (PolicyEntryBase)comboBoxPolicy.Items[e.Index];

                e.ItemWidth = comboBoxPolicy.Width;
                entry.MeasureItem(e);
            }
        }

        private void comboBoxPolicy_DrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e) {
            if (e.Index >= 0) {
                PolicyEntryBase entry = (PolicyEntryBase)comboBoxPolicy.Items[e.Index];

                entry.Draw(e);
            }
        }

        private void buttonOK_Click(object sender, System.EventArgs e) {
            if (comboBoxPolicy.SelectedItem is PolicyEntry selected) {
                element.SetAttribute(A_PolicyID, selected.Policy.Id);
            }

            DialogResult = DialogResult.OK;
        }

        private void comboBoxPolicy_SelectedIndexChanged(object sender, System.EventArgs e) {
            buttonOK.Enabled = comboBoxPolicy.SelectedItem != null && comboBoxPolicy.SelectedItem is PolicyEntry;

            if (!(comboBoxPolicy.SelectedItem is PolicyEntry))
                comboBoxPolicy.SelectedItem = null;
        }

        private abstract class PolicyEntryBase {
            protected PolicyEntryBase(RunPolicy form) {
                this.RunPolicy = form;
            }

            protected RunPolicy RunPolicy { get; }

            public virtual void MeasureItem(MeasureItemEventArgs e) {
                SizeF titleSize = e.Graphics.MeasureString(ToString(), Font);

                e.ItemHeight = (int)Math.Ceiling(titleSize.Height + VerticalMargins);
                if (titleSize.Width + Indent > e.ItemWidth)
                    e.ItemWidth = (int)Math.Ceiling(titleSize.Width + Indent);
            }

            public virtual void Draw(DrawItemEventArgs e) {
                StringFormat format = new StringFormat {
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Trimming = StringTrimming.EllipsisWord
                };

                Brush backBrush = BackBrush;

                if (backBrush != null)
                    e.Graphics.FillRectangle(backBrush, e.Bounds);

                if (!IsTitle)
                    e.DrawBackground();

                RectangleF titleRect = new RectangleF(e.Bounds.Left + Indent, e.Bounds.Top, e.Bounds.Width - Indent, e.Bounds.Height);

                e.Graphics.DrawString(ToString(), Font, FontBrush, titleRect, format);

                Pen underlinePen = UnderlinePen;

                if (underlinePen != null)
                    e.Graphics.DrawLine(underlinePen, new PointF(e.Bounds.Left, e.Bounds.Bottom - underlinePen.Width / 2), new PointF(e.Bounds.Right, e.Bounds.Bottom - underlinePen.Width / 2));

                if (!IsTitle)
                    e.DrawFocusRectangle();
            }

            abstract protected Font Font { get; }

            abstract protected int Indent { get; }

            protected virtual Brush FontBrush => Brushes.Black;

            protected virtual Brush BackBrush => null;

            protected virtual int VerticalMargins => 0;

            protected virtual bool IsTitle => false;

            protected virtual Pen UnderlinePen => null;
        }

        private class PolicyEntry : PolicyEntryBase {
            public PolicyEntry(RunPolicy form, LayoutPolicyInfo policy) : base(form) {
                this.Policy = policy;
            }

            public LayoutPolicyInfo Policy { get; }

            protected override int Indent => 16;

            protected override Font Font => RunPolicy.PolicyNameFont;

            protected override int VerticalMargins => 2;

            public override string ToString() => Policy.Name;
        }

        private class GlobalOrLayoutEntry : PolicyEntryBase {
            private readonly string title;

            public GlobalOrLayoutEntry(RunPolicy form, string title) : base(form) {
                this.title = title;
            }

            protected override Font Font => RunPolicy.GlobalOrLayoutFont;

            protected override int Indent => 0;

            protected override bool IsTitle => true;

            protected override int VerticalMargins => 8;

            protected override Pen UnderlinePen => RunPolicy.GlobalOrLayoutUnderlinePen;

            public override string ToString() => title;

            protected override Brush BackBrush => RunPolicy.GlobalOrLayoutBackBrush;
        }

        private class PolicyUsageEntry : PolicyEntryBase {
            private readonly string title;

            public PolicyUsageEntry(RunPolicy form, string title) : base(form) {
                this.title = title;
            }

            protected override Font Font => RunPolicy.PolicyUsageFont;

            protected override int Indent => 8;

            protected override bool IsTitle => true;

            protected override int VerticalMargins => 6;

            protected override Pen UnderlinePen => RunPolicy.PolicyUsageUnderlinePen;

            public override string ToString() => title;

            protected override Brush BackBrush => RunPolicy.PolicyUsageBackBrush;
        }
    }
}
