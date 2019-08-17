using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;

#pragma warning disable IDE0051
namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for BindBlockEdgeToSignals.
    /// </summary>
    public class BindBlockEdgeToSignals : Form, IModelComponentReceiverDialog {
        private const string E_LinkedSignals = "LinkedSignals";
        private const string E_LinkedSignal = "LinkedSignal";
        private const string A_SignalId = "SignalID";
        private Label label1;
        private ListBox listBoxSignals;
        private Button buttonRemove;
        private Button buttonCancel;
        private Button buttonOk;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private readonly LayoutSelection blockEdgeSelection;
        private readonly LayoutSelection linkedSignalSelection;

        public BindBlockEdgeToSignals(LayoutBlockEdgeBase blockEdge) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.BlockEdge = blockEdge;

            this.Owner = LayoutController.ActiveFrameWindow as Form;

            EventManager.AddObjectSubscriptions(this);

            blockEdgeSelection = new LayoutSelection();
            linkedSignalSelection = new LayoutSelection();

            blockEdgeSelection.Add(blockEdge);
            blockEdgeSelection.Display(new LayoutSelectionLook(Color.LightSeaGreen));
            linkedSignalSelection.Display(new LayoutSelectionLook(Color.DarkGreen));

            foreach (LinkedSignalInfo linkedSignal in blockEdge.LinkedSignals) {
                try {
                    listBoxSignals.Items.Add(new LinkedSignalItem(this, linkedSignal));
                }
                catch (LayoutException ex) {
                    ex.Report();
                }
            }

            updateButtons(null, null);
        }

        private void updateButtons(object sender, EventArgs e) {
            LinkedSignalItem selected = (LinkedSignalItem)listBoxSignals.SelectedItem;

            linkedSignalSelection.Clear();

            if (selected == null)
                buttonRemove.Enabled = false;
            else {
                buttonRemove.Enabled = true;
                linkedSignalSelection.Add(selected.SignalComponent);
            }
        }

        public string DialogName(IModelComponent component) => "Link signals to track contact";

        public LayoutBlockEdgeBase BlockEdge { get; }

        public void AddComponent(IModelComponent component) {
            LayoutSignalComponent signalComponent = (LayoutSignalComponent)component;

            foreach (LinkedSignalItem linkedSignalItem in listBoxSignals.Items)
                if (linkedSignalItem.SignalComponent.Id == signalComponent.Id)
                    return;

            listBoxSignals.Items.Add(new LinkedSignalItem(signalComponent));
        }

        [LayoutEvent("query-bind-signals-dialogs")]
        private void queryBindSignalsDialog(LayoutEvent e) {
            LayoutBlockEdgeBase queryBlockEdge = (LayoutBlockEdgeBase)e.Sender;
            ArrayList list = (ArrayList)e.Info;

            if (queryBlockEdge == null || queryBlockEdge.Id == BlockEdge.Id)
                list.Add((IModelComponentReceiverDialog)this);
        }

        [LayoutEvent("removed-from-model")]
        private void removedFromModel(LayoutEvent e) {
            ModelComponent component = (ModelComponent)e.Sender;

            if (component is LayoutBlockEdgeBase && component.Id == BlockEdge.Id)
                Close();
            else if (component is LayoutSignalComponent) {
                foreach (LinkedSignalItem linkedSignalItem in listBoxSignals.Items)
                    if (linkedSignalItem.SignalComponent.Id == component.Id) {
                        listBoxSignals.Items.Remove(linkedSignalItem);
                        break;
                    }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                EventManager.Subscriptions.RemoveObjectSubscriptions(this);

                if (components != null) {
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
            this.label1 = new Label();
            this.listBoxSignals = new ListBox();
            this.buttonRemove = new Button();
            this.buttonCancel = new Button();
            this.buttonOk = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(176, 56);
            this.label1.TabIndex = 0;
            this.label1.Text = "Right click, and select \"Link signal\" on the signal components that should reflec" +
                "t the signalling state of this track contact:";
            // 
            // listBoxSignals
            // 
            this.listBoxSignals.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom
                | System.Windows.Forms.AnchorStyles.Left
                | System.Windows.Forms.AnchorStyles.Right;
            this.listBoxSignals.Location = new System.Drawing.Point(12, 72);
            this.listBoxSignals.Name = "listBoxSignals";
            this.listBoxSignals.Size = new System.Drawing.Size(170, 108);
            this.listBoxSignals.TabIndex = 1;
            this.listBoxSignals.SelectedIndexChanged += this.updateButtons;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            this.buttonRemove.Location = new System.Drawing.Point(12, 186);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(56, 21);
            this.buttonRemove.TabIndex = 2;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.Click += this.buttonRemove_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonCancel.Location = new System.Drawing.Point(128, 216);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(56, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += this.buttonCancel_Click;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
            this.buttonOk.Location = new System.Drawing.Point(64, 216);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(56, 23);
            this.buttonOk.TabIndex = 2;
            this.buttonOk.Text = "OK";
            this.buttonOk.Click += this.buttonOk_Click;
            // 
            // BindBlockEdgeToSignals
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.ClientSize = new System.Drawing.Size(192, 246);
            this.ControlBox = false;
            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                                                                          this.buttonRemove,
                                                                          this.listBoxSignals,
                                                                          this.label1,
                                                                          this.buttonCancel,
                                                                          this.buttonOk});
            this.Name = "BindBlockEdgeToSignals";
            this.ShowInTaskbar = false;
            this.Text = "Link Signals to Track Contact";
            this.Closing += this.BindBlockEdgeToSignals_Closing;
            this.Closed += this.BindBlockEdgeToSignals_Closed;
            this.ResumeLayout(false);
        }
        #endregion

        private void BindBlockEdgeToSignals_Closed(object sender, System.EventArgs e) {
            blockEdgeSelection.Hide();
            linkedSignalSelection.Hide();

            Dispose();
        }

        private void buttonRemove_Click(object sender, System.EventArgs e) {
            LinkedSignalItem selected = (LinkedSignalItem)listBoxSignals.SelectedItem;

            if (selected != null)
                listBoxSignals.Items.Remove(selected);
        }

        private void buttonOk_Click(object sender, System.EventArgs e) {
            LayoutXmlInfo xmlInfo = new LayoutXmlInfo(BlockEdge);
            XmlElement linkedSignalsElement = xmlInfo.Element[E_LinkedSignals];

            if (linkedSignalsElement == null) {
                linkedSignalsElement = xmlInfo.XmlDocument.CreateElement(E_LinkedSignals);

                xmlInfo.Element.AppendChild(linkedSignalsElement);
            }

            linkedSignalsElement.RemoveAll();

            foreach (LinkedSignalItem linkedSignalItem in listBoxSignals.Items) {
                XmlElement linkedSignalElement = linkedSignalsElement.OwnerDocument.CreateElement(E_LinkedSignal);

                linkedSignalsElement.AppendChild(linkedSignalElement);
                linkedSignalElement.SetAttribute(A_SignalId, linkedSignalItem.SignalComponent.Id);
            }

            LayoutModifyComponentDocumentCommand modifyComponentDocumentCommand =
                new LayoutModifyComponentDocumentCommand(BlockEdge, xmlInfo);

            LayoutController.Do(modifyComponentDocumentCommand);

            Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            Close();
        }

        private void BindBlockEdgeToSignals_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (Owner != null)
                Owner.Activate();
        }

        private class LinkedSignalItem {
            public LinkedSignalItem(BindBlockEdgeToSignals dialog, LinkedSignalInfo linkedSignal) {
                if (LayoutModel.Component<LayoutSignalComponent>(linkedSignal.SignalId, LayoutModel.ActivePhases) == null)
                    throw new LayoutException(dialog.BlockEdge, "A signal linked to this track contact cannot be found");
            }

            public LinkedSignalItem(LayoutSignalComponent signalComponent) {
                this.SignalComponent = signalComponent;
            }

            public LayoutSignalComponent SignalComponent { get; }

            public override string ToString() {
                switch (SignalComponent.Info.SignalType) {
                    case LayoutSignalType.Lights:
                        return "Lights";

                    case LayoutSignalType.Semaphore:
                        return "Semaphore";

                    case LayoutSignalType.Distance:
                        return "Distance Signal";

                    default:
                        return "Unknown signal type";
                }
            }
        }
    }
}
