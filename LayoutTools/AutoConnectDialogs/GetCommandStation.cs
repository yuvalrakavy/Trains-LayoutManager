using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using LayoutManager.Model;

namespace LayoutManager.Tools.AutoConnectDialogs {
    /// <summary>
    /// Summary description for GetCommandStation.
    /// </summary>
    public class GetCommandStation : Form {
        private Label label1;
        private Button buttonOK;
        private Button buttonCancel;
        private ComboBox comboBoxCommandStations;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;

        public GetCommandStation(IList<IModelComponentIsCommandStation> commandStations) {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            comboBoxCommandStations.DisplayMember = "Name";

            foreach (IModelComponentIsCommandStation commandStation in commandStations)
                comboBoxCommandStations.Items.Add(new Item(commandStation));

            if (comboBoxCommandStations.Items.Count > 0)
                comboBoxCommandStations.SelectedIndex = 0;
        }

        public IModelComponentIsCommandStation CommandStation {
            get {
                return comboBoxCommandStations.SelectedItem == null ? null : ((Item)comboBoxCommandStations.SelectedItem).CommandStation;
            }
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
            this.label1 = new Label();
            this.comboBoxCommandStations = new ComboBox();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 48);
            this.label1.TabIndex = 0;
            this.label1.Text = "This layout has more than one command station. Please select the command station " +
                "to which you wish to connect the component:";
            // 
            // comboBoxCommandStations
            // 
            this.comboBoxCommandStations.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxCommandStations.Location = new System.Drawing.Point(8, 64);
            this.comboBoxCommandStations.Name = "comboBoxCommandStations";
            this.comboBoxCommandStations.Size = new System.Drawing.Size(168, 21);
            this.comboBoxCommandStations.TabIndex = 1;
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(88, 96);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "Continue";
            this.buttonOK.Click += this.buttonOK_Click;
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(168, 96);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            // 
            // GetCommandStation
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(5, 13);
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(248, 126);
            this.ControlBox = false;
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxCommandStations);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "GetCommandStation";
            this.ShowInTaskbar = false;
            this.Text = "Select Command Station";
            this.ResumeLayout(false);
        }
        #endregion

        private void buttonOK_Click(object sender, System.EventArgs e) {
            DialogResult = DialogResult.OK;
        }

        private class Item {
            public Item(IModelComponentIsCommandStation commandStation) {
                this.CommandStation = commandStation;
            }

            public IModelComponentIsCommandStation CommandStation { get; }

            public override string ToString() => CommandStation.NameProvider.Name.ToString();
        }
    }
}
