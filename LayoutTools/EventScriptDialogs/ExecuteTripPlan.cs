using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using LayoutManager.Model;
using LayoutManager.Components;

namespace LayoutManager.Tools.EventScriptDialogs {
    /// <summary>
    /// Summary description for ExecuteTripPlan.
    /// </summary>
    public class ExecuteTripPlan : Form, IModelComponentReceiverDialog {
        private LayoutManager.CommonUI.Controls.TripPlanList tripPlanList;
        private Splitter splitter1;
        private Panel panel1;
        private Label label1;
        private ComboBox comboBoxTrainSymbol;
        private Button buttonOK;
        private Button buttonCancel;
        private GroupBox groupBox1;
        private LayoutManager.Tools.Controls.TripPlanEditor tripPlanEditor;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        private void EndOfDesignerVariables() { }

        readonly XmlElement element;

        LayoutBlock locomotiveBlock = null;
        LayoutComponentConnectionPoint locomotiveFront = LayoutComponentConnectionPoint.Empty;

        public ExecuteTripPlan(XmlElement element) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.element = element;

            tripPlanList.Initialize();

            tripPlanEditor.Initialize();
            tripPlanEditor.EnablePreview = false;

            EventManager.AddObjectSubscriptions(this);

            TripPlanInfo selectedTripPlan = null;

            if (element.HasAttribute("TripPlanID")) {
                Guid tripPlanID = XmlConvert.ToGuid(element.GetAttribute("TripPlanID"));

                selectedTripPlan = LayoutModel.StateManager.TripPlansCatalog.TripPlans[tripPlanID];
            }

            if (element.HasAttribute("TrainSymbol"))
                comboBoxTrainSymbol.Text = element.GetAttribute("TrainSymbol");

            if (element.HasAttribute("LocomotiveBlockID")) {
                Guid locomotiveBlockID = XmlConvert.ToGuid(element.GetAttribute("LocomotiveBlockID"));

                locomotiveBlock = LayoutModel.Blocks[locomotiveBlockID];
                locomotiveFront = LayoutComponentConnectionPoint.Parse(element.GetAttribute("LocomotiveFront"));
            }

            fillList(locomotiveBlock, locomotiveFront);
            tripPlanList.SelectedTripPlan = selectedTripPlan;

            if (locomotiveBlock != null) {
                tripPlanEditor.LocomotiveBlock = locomotiveBlock;
                tripPlanEditor.Front = locomotiveFront;
                tripPlanEditor.EnablePreview = true;
            }

            tripPlanEditor.TripPlan = selectedTripPlan;
        }

        private void fillList(LayoutBlock block, LayoutComponentConnectionPoint front) {
            XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();
            XmlElement applicableTripPlansElement = workingDoc.CreateElement("ApplicableTripPlans");

            workingDoc.AppendChild(applicableTripPlansElement);

            LayoutEvent getApplicableTripPlansEvent = new LayoutEvent(block, "get-applicable-trip-plans-request", null, applicableTripPlansElement);

            if (front != LayoutComponentConnectionPoint.Empty)
                getApplicableTripPlansEvent.SetOption("Front", front.ToString());

            EventManager.Event(getApplicableTripPlansEvent);

            if (applicableTripPlansElement.HasAttribute("LocomotiveBlockID")) {
                locomotiveBlock = LayoutModel.Blocks[XmlConvert.ToGuid(applicableTripPlansElement.GetAttribute("LocomotiveBlockID"))];
                locomotiveFront = LayoutComponentConnectionPoint.Parse(applicableTripPlansElement.GetAttribute("LocomotiveFront"));
            }

            tripPlanList.ApplicableTripPlansElement = applicableTripPlansElement;

        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing) {
            if (disposing) {
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
            this.tripPlanList = new LayoutManager.CommonUI.Controls.TripPlanList();
            this.splitter1 = new Splitter();
            this.panel1 = new Panel();
            this.groupBox1 = new GroupBox();
            this.tripPlanEditor = new LayoutManager.Tools.Controls.TripPlanEditor();
            this.buttonOK = new Button();
            this.comboBoxTrainSymbol = new ComboBox();
            this.label1 = new Label();
            this.buttonCancel = new Button();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tripPlanList
            // 
            this.tripPlanList.ApplicableTripPlansElement = null;
            this.tripPlanList.Dock = System.Windows.Forms.DockStyle.Top;
            this.tripPlanList.Location = new System.Drawing.Point(0, 0);
            this.tripPlanList.Name = "tripPlanList";
            this.tripPlanList.SelectedTripPlan = null;
            this.tripPlanList.Size = new System.Drawing.Size(632, 216);
            this.tripPlanList.TabIndex = 0;
            this.tripPlanList.Load += new System.EventHandler(this.tripPlanList_Load);
            this.tripPlanList.SelectedTripPlanChanged += new System.EventHandler(this.tripPlanList_SelectedTripPlanChanged);
            // 
            // splitter1
            // 
            this.splitter1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 216);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(632, 4);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Controls.Add(this.comboBoxTrainSymbol);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 220);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(632, 301);
            this.panel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tripPlanEditor);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(616, 256);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Selected Trip-plan:";
            // 
            // tripPlanEditor
            // 
            this.tripPlanEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tripPlanEditor.EnablePreview = true;
            this.tripPlanEditor.Location = new System.Drawing.Point(3, 16);
            this.tripPlanEditor.LocomotiveBlock = null;
            this.tripPlanEditor.Name = "tripPlanEditor";
            this.tripPlanEditor.Size = new System.Drawing.Size(610, 237);
            this.tripPlanEditor.TabIndex = 0;
            this.tripPlanEditor.Train = null;
            this.tripPlanEditor.TrainTargetWaypoint = -1;
            this.tripPlanEditor.TripPlanName = null;
            this.tripPlanEditor.ViewOnly = true;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(472, 272);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // comboBoxTrainSymbol
            // 
            this.comboBoxTrainSymbol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboBoxTrainSymbol.Items.AddRange(new object[] {
                                                                     "Train",
                                                                     "Script:Train"});
            this.comboBoxTrainSymbol.Location = new System.Drawing.Point(240, 273);
            this.comboBoxTrainSymbol.Name = "comboBoxTrainSymbol";
            this.comboBoxTrainSymbol.Size = new System.Drawing.Size(152, 21);
            this.comboBoxTrainSymbol.TabIndex = 2;
            this.comboBoxTrainSymbol.Text = "Train";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Location = new System.Drawing.Point(0, 272);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 23);
            this.label1.TabIndex = 1;
            this.label1.Text = "Trip plan is for train represented by synbol:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(552, 272);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 4;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // ExecuteTripPlan
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(632, 521);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.tripPlanList);
            this.Name = "ExecuteTripPlan";
            this.Text = "Execute Trip-plan";
            this.Closing += new System.ComponentModel.CancelEventHandler(this.ExecuteTripPlan_Closing);
            this.panel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        [LayoutEvent("query-execute-trip-plan-dialog")]
        private void queryExecuteTripPlanDialog(LayoutEvent e0) {
            var e = (LayoutEvent<LayoutBlockDefinitionComponent, List<IModelComponentReceiverDialog>>)e0;

            e.Info.Add(this);
        }

        private void ExecuteTripPlan_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            EventManager.Subscriptions.RemoveObjectSubscriptions(this);
        }

        private void buttonOK_Click(object sender, System.EventArgs e) {
            TripPlanInfo tripPlan = tripPlanList.SelectedTripPlan;

            if (tripPlan == null) {
                MessageBox.Show(this, "You did not select any trip-plan", "No Trip-plan", MessageBoxButtons.OK, MessageBoxIcon.Error);
                tripPlanList.Focus();
                return;
            }

            element.SetAttribute("TripPlanID", XmlConvert.ToString(tripPlan.Id));
            element.SetAttribute("ShouldReverse", XmlConvert.ToString(tripPlanList.ShouldReverseSelectedTripPlan));
            element.SetAttribute("TrainSymbol", comboBoxTrainSymbol.Text);

            if (locomotiveBlock != null) {
                element.SetAttribute("LocomotiveBlockID", XmlConvert.ToString(locomotiveBlock.Id));
                element.SetAttribute("LocomotiveFront", locomotiveFront.ToString());
            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void buttonCancel_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void tripPlanList_SelectedTripPlanChanged(object sender, System.EventArgs e) {
            tripPlanEditor.LocomotiveBlock = locomotiveBlock;
            tripPlanEditor.Front = locomotiveFront;

            TripPlanInfo tripPlan = tripPlanList.SelectedTripPlan;

            if (tripPlan != null && tripPlanList.ShouldReverseSelectedTripPlan) {
                XmlDocument workingDoc = LayoutXmlInfo.XmlImplementation.CreateDocument();

                workingDoc.AppendChild((XmlElement)workingDoc.ImportNode(tripPlan.Element, true));
                tripPlan = new TripPlanInfo(workingDoc.DocumentElement);
                tripPlan.Reverse();
            }

            tripPlanEditor.TripPlan = tripPlan;

            tripPlanEditor.EnablePreview = locomotiveBlock != null;
        }

        #region IModelComponentReceiverComponent Members

        public string DialogName(IModelComponent component) => Text;

        public void AddComponent(IModelComponent component) {
            LayoutBlockDefinitionComponent blockInfo = (LayoutBlockDefinitionComponent)component;

            locomotiveBlock = blockInfo.Block;
            locomotiveFront = LayoutComponentConnectionPoint.Empty;
            fillList(locomotiveBlock, locomotiveFront);
        }

        #endregion

        private void tripPlanList_Load(object sender, System.EventArgs e) {

        }
    }
}
