#region Using directives

using System.Text;

#endregion

namespace LayoutManager.CommonUI.Dialogs {
    public partial class SerialInterfaceParameters : Form {
        public SerialInterfaceParameters(string parametersString) {
            InitializeComponent();

            // Parse parameters
            int[] baudRates = new int[] {
                110, 150, 300, 600, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200
            };

            foreach (int baudRate in baudRates)
                comboBoxBaudrate.Items.Add(baudRate.ToString());
            radioButtonStopBits1.Checked = true;
            radioButtonDTRoff.Checked = true;
            radioButtonRTSoff.Checked = true;
            comboBoxBaudrate.SelectedIndex = 7;
            comboBoxParity.SelectedIndex = 0;

            parametersString = System.Text.RegularExpressions.Regex.Replace(parametersString, "[\\r\\n ]+", " ", System.Text.RegularExpressions.RegexOptions.None);

            string[] parameters = parametersString.Split(' ');

            foreach (string parameter in parameters) {
                string[] parameterParts = parameter.Split('=');
                string name = parameterParts[0].ToLower();
                string value = parameterParts[1].ToLower();

                switch (name) {
                    case "baud": {
                            foreach (string item in comboBoxBaudrate.Items)
                                if (item == value) {
                                    comboBoxBaudrate.SelectedItem = item;
                                    break;
                                }
                        }
                        break;

                    case "parity":
                        switch (value[0]) {
                            case 'n': comboBoxParity.SelectedIndex = 0; break;
                            case 'e': comboBoxParity.SelectedIndex = 1; break;
                            case 'o': comboBoxParity.SelectedIndex = 2; break;
                            case 'm': comboBoxParity.SelectedIndex = 3; break;
                            case 's': comboBoxParity.SelectedIndex = 4; break;
                        }
                        break;

                    case "data":
                        switch (value[0]) {
                            case '8': radioButtonData8.Checked = true; break;
                            case '7': radioButtonData7.Checked = true; break;
                            case '6': radioButtonData6.Checked = true; break;
                            case '5': radioButtonData5.Checked = true; break;
                        }
                        break;

                    case "stop":
                        switch (value) {
                            case "1": radioButtonStopBits1.Checked = true; break;
                            case "1.5": radioButtonStopBits15.Checked = true; break;
                            case "2": radioButtonStopBits2.Checked = true; break;
                        }
                        break;

                    case "to":
                        checkBoxNoTimeout.Checked = value == "on";
                        break;

                    case "xon":
                        checkBoxXonXoff.Checked = value == "on";
                        break;

                    case "odsr":
                        checkBoxDSRhandshake.Checked = value == "on";
                        break;

                    case "octs":
                        checkBoxCTShandshake.Checked = value == "on";
                        break;

                    case "dtr":
                        switch (value) {
                            case "on": radioButtonDTRon.Checked = true; break;
                            case "off": radioButtonDTRoff.Checked = true; break;
                            case "hs": radioButtonDTRhandshake.Checked = true; break;
                        }
                        break;

                    case "rts":
                        switch (value) {
                            case "on": radioButtonRTSon.Checked = true; break;
                            case "off": radioButtonRTSoff.Checked = true; break;
                            case "hs": radioButtonRTShandshake.Checked = true; break;
                            case "tg": radioButtonRTStoggle.Checked = true; break;
                        }
                        break;

                    case "idsr":
                        checkBoxDSR.Checked = value == "on";
                        break;
                }
            }
        }

        public string ModeString {
            get {
                StringBuilder modeString = new("baud=" + comboBoxBaudrate.SelectedItem + " parity=" + ((string)comboBoxParity.SelectedItem)[0] + " data=");

                if (radioButtonData8.Checked)
                    modeString.Append("8");
                else if (radioButtonData7.Checked)
                    modeString.Append("7");
                else if (radioButtonData6.Checked)
                    modeString.Append("6");
                else if (radioButtonData5.Checked)
                    modeString.Append("5");

                modeString.Append(" stop=");
                if (radioButtonStopBits1.Checked)
                    modeString.Append("1");
                else if (radioButtonStopBits15.Checked)
                    modeString.Append("1.5");
                else if (radioButtonStopBits2.Checked)
                    modeString.Append("2");

                modeString.Append(" to=" + (checkBoxNoTimeout.Checked ? "on" : "off"));
                modeString.Append(" xon=" + (checkBoxXonXoff.Checked ? "on" : "off"));
                modeString.Append(" odsr=" + (checkBoxDSRhandshake.Checked ? "on" : "off"));
                modeString.Append(" octs=" + (checkBoxCTShandshake.Checked ? "on" : "off"));

                modeString.Append(" dtr=");
                if (radioButtonDTRoff.Checked)
                    modeString.Append("off");
                else if (radioButtonDTRon.Checked)
                    modeString.Append("on");
                else if (radioButtonDTRhandshake.Checked)
                    modeString.Append("hs");

                modeString.Append(" rts=");
                if (radioButtonRTSoff.Checked)
                    modeString.Append("off");
                else if (radioButtonRTSon.Checked)
                    modeString.Append("on");
                else if (radioButtonRTShandshake.Checked)
                    modeString.Append("hs");
                else if (radioButtonRTStoggle.Checked)
                    modeString.Append("tg");

                modeString.Append(" idsr=" + (checkBoxDSR.Checked ? "on" : "off"));

                return modeString.ToString();
            }
        }
    }
}