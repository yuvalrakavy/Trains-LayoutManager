﻿using LayoutManager;
using System;
using System.Windows.Forms;

namespace TrainDetector.Dialogs {
    public partial class TrainDetectorProperties : Form {
        public LayoutXmlInfo XmlInfo { get; }

        readonly TrainDetectorsComponent component;

        public TrainDetectorProperties(TrainDetectorsComponent component, bool initialPlacment) {
            InitializeComponent();
            this.component = component;
            this.XmlInfo = new LayoutXmlInfo(component);
            nameDefinition.XmlInfo = this.XmlInfo;

            if (!initialPlacment)
                checkBoxAutoDetect.Visible = false;
        }

        public bool AutoDetect {
            get => checkBoxAutoDetect.Checked;
            set => checkBoxAutoDetect.Checked = value;
        }

        private void ButtonOK_Click(object? sender, EventArgs e) {
            if (!nameDefinition.Commit())
                return;

            var _ = new TrainDetectorsInfo(component, XmlInfo.Element) {
                AutoDetect = checkBoxAutoDetect.Checked,
            };

            DialogResult = DialogResult.OK;
        }

        private void NameDefinition_Load(object? sender, EventArgs e) {

        }
    }
}
