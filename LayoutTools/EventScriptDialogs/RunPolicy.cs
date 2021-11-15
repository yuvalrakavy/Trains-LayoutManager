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
    public partial class RunPolicy : Form {
        private const string A_PolicyID = "PolicyID";
        private readonly XmlElement element;

        public RunPolicy(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            comboBoxPolicy.Items.Add(new GlobalOrLayoutEntry(this, "Policies for all layout"));

            AddPoliciesOfScope(LayoutModel.Instance.GlobalPoliciesElement, "Global", "Layout wide policies");
            AddPoliciesOfScope(LayoutModel.Instance.GlobalPoliciesElement, "TripPlan", "Trip plan policies");
            AddPoliciesOfScope(LayoutModel.Instance.GlobalPoliciesElement, "Block", "Block policies");

            comboBoxPolicy.Items.Add(new GlobalOrLayoutEntry(this, "Policies for this layout only"));

            AddPoliciesOfScope(LayoutModel.StateManager.LayoutPoliciesElement, "Global", "Layout wide policies");
            AddPoliciesOfScope(LayoutModel.StateManager.LayoutPoliciesElement, "TripPlan", "Trip plan policies");
            AddPoliciesOfScope(LayoutModel.StateManager.LayoutPoliciesElement, "Block", "Block policies");

            if (element.HasAttribute(A_PolicyID)) {
                var policyID = (Guid)element.AttributeValue(A_PolicyID);

                foreach (PolicyEntryBase entryBase in comboBoxPolicy.Items) {
                    if (entryBase is PolicyEntry entry && entry.Policy.Id == policyID) {
                        comboBoxPolicy.SelectedItem = entryBase;
                        break;
                    }
                }
            }
        }

        private void AddPoliciesOfScope(XmlElement policiesCollectionElement, string scope, string scopeName) {
            LayoutPoliciesCollection policies = new(policiesCollectionElement, null, scope);

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

        private void ComboBoxPolicy_MeasureItem(object? sender, System.Windows.Forms.MeasureItemEventArgs e) {
            if (e.Index >= 0) {
                PolicyEntryBase entry = (PolicyEntryBase)comboBoxPolicy.Items[e.Index];

                e.ItemWidth = comboBoxPolicy.Width;
                entry.MeasureItem(e);
            }
        }

        private void ComboBoxPolicy_DrawItem(object? sender, System.Windows.Forms.DrawItemEventArgs e) {
            if (e.Index >= 0) {
                PolicyEntryBase entry = (PolicyEntryBase)comboBoxPolicy.Items[e.Index];

                entry.Draw(e);
            }
        }

        private void ButtonOK_Click(object? sender, System.EventArgs e) {
            if (comboBoxPolicy.SelectedItem is PolicyEntry selected) {
                element.SetAttributeValue(A_PolicyID, selected.Policy.Id);
            }

            DialogResult = DialogResult.OK;
        }

        private void ComboBoxPolicy_SelectedIndexChanged(object? sender, System.EventArgs e) {
            buttonOK.Enabled = comboBoxPolicy.SelectedItem != null && comboBoxPolicy.SelectedItem is PolicyEntry;

            if (comboBoxPolicy.SelectedItem is not PolicyEntry)
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
                StringFormat format = new() {
                    LineAlignment = StringAlignment.Center,
                    FormatFlags = StringFormatFlags.NoWrap,
                    Trimming = StringTrimming.EllipsisWord
                };

                var backBrush = BackBrush;

                if (backBrush != null)
                    e.Graphics.FillRectangle(backBrush, e.Bounds);

                if (!IsTitle)
                    e.DrawBackground();

                RectangleF titleRect = new(e.Bounds.Left + Indent, e.Bounds.Top, e.Bounds.Width - Indent, e.Bounds.Height);

                e.Graphics.DrawString(ToString(), Font, FontBrush, titleRect, format);

                var underlinePen = UnderlinePen;

                if (underlinePen != null)
                    e.Graphics.DrawLine(underlinePen, new PointF(e.Bounds.Left, e.Bounds.Bottom - (underlinePen.Width / 2)), new PointF(e.Bounds.Right, e.Bounds.Bottom - (underlinePen.Width / 2)));

                if (!IsTitle)
                    e.DrawFocusRectangle();
            }

            abstract protected Font Font { get; }

            abstract protected int Indent { get; }

            protected virtual Brush FontBrush => Brushes.Black;

            protected virtual Brush? BackBrush => null;

            protected virtual int VerticalMargins => 0;

            protected virtual bool IsTitle => false;

            protected virtual Pen? UnderlinePen => null;
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
