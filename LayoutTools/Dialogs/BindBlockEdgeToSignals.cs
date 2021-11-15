using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;
using System.Collections.Generic;

namespace LayoutManager.Tools.Dialogs {
    /// <summary>
    /// Summary description for BindBlockEdgeToSignals.
    /// </summary>
    public partial class BindBlockEdgeToSignals : Form, IModelComponentReceiverDialog {
        private const string E_LinkedSignals = "LinkedSignals";
        private const string E_LinkedSignal = "LinkedSignal";
        private const string A_SignalId = "SignalID";

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

            updateButtons(null, EventArgs.Empty);
        }

        private void updateButtons(object? sender, EventArgs e) {
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
            var queryBlockEdge = (LayoutBlockEdgeBase?)e.Sender;
            var list = Ensure.NotNull<IList<IModelComponentReceiverDialog>>(e.Info);

            if (queryBlockEdge == null || queryBlockEdge.Id == BlockEdge.Id)
                list.Add((IModelComponentReceiverDialog)this);
        }

        [LayoutEvent("removed-from-model")]
        private void removedFromModel(LayoutEvent e) {
            var component = Ensure.NotNull<ModelComponent>(e.Sender);

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

        private void BindBlockEdgeToSignals_Closed(object? sender, System.EventArgs e) {
            blockEdgeSelection.Hide();
            linkedSignalSelection.Hide();

            Dispose();
        }

        private void buttonRemove_Click(object? sender, System.EventArgs e) {
            LinkedSignalItem selected = (LinkedSignalItem)listBoxSignals.SelectedItem;

            if (selected != null)
                listBoxSignals.Items.Remove(selected);
        }

        private void buttonOk_Click(object? sender, System.EventArgs e) {
            LayoutXmlInfo xmlInfo = new(BlockEdge);
            var linkedSignalsElement = xmlInfo.Element[E_LinkedSignals];

            if (linkedSignalsElement == null) {
                linkedSignalsElement = xmlInfo.XmlDocument.CreateElement(E_LinkedSignals);

                xmlInfo.Element.AppendChild(linkedSignalsElement);
            }

            linkedSignalsElement.RemoveAll();

            foreach (LinkedSignalItem linkedSignalItem in listBoxSignals.Items) {
                XmlElement linkedSignalElement = linkedSignalsElement.OwnerDocument.CreateElement(E_LinkedSignal);

                linkedSignalsElement.AppendChild(linkedSignalElement);
                linkedSignalElement.SetAttributeValue(A_SignalId, linkedSignalItem.SignalComponent.Id);
            }

            LayoutModifyComponentDocumentCommand modifyComponentDocumentCommand =
                new(BlockEdge, xmlInfo);

            LayoutController.Do(modifyComponentDocumentCommand);

            Close();
        }

        private void buttonCancel_Click(object? sender, System.EventArgs e) {
            Close();
        }

        private void BindBlockEdgeToSignals_Closing(object? sender, System.ComponentModel.CancelEventArgs e) {
            if (Owner != null)
                Owner.Activate();
        }

        private class LinkedSignalItem {
            public LinkedSignalItem(BindBlockEdgeToSignals dialog, LinkedSignalInfo linkedSignal) {
                this.SignalComponent = LayoutModel.Component<LayoutSignalComponent>(linkedSignal.SignalId, LayoutModel.ActivePhases) ??
                    throw new LayoutException(dialog.BlockEdge, "A signal linked to this track contact cannot be found");
            }

            public LinkedSignalItem(LayoutSignalComponent signalComponent) {
                this.SignalComponent = signalComponent;
            }

            public LayoutSignalComponent SignalComponent { get; }

            public override string ToString() => SignalComponent.Info.SignalType switch
            {
                LayoutSignalType.Lights => "Lights",
                LayoutSignalType.Semaphore => "Semaphore",
                LayoutSignalType.Distance => "Distance Signal",
                _ => "Unknown signal type",
            };
        }
    }
}
