using System;
using System.Windows.Forms;

using LayoutManager.Model;

namespace LayoutManager.CommonUI.Controls {
    public partial class TrackGuageSelector : ComboBox {
        public TrackGuageSelector() {
            InitializeComponent();
            DropDownStyle = ComboBoxStyle.DropDownList;

            IncludeGuageSet = false;
        }

        public void Init() {
            Items.Clear();

            foreach (int v in Enum.GetValues(typeof(TrackGauges)))
                if (IncludeGuageSet || !IsSet(v))
                    Items.Add(Enum.ToObject(typeof(TrackGauges), v));
        }

        public TrackGauges Value {
            set {
                if (!DesignMode) {
                    SelectedItem = null;

                    foreach (object item in Items)
                        if ((TrackGauges)item == value) {
                            SelectedItem = item;
                            break;
                        }
                }
            }

            get {
                return SelectedItem == null ? TrackGauges.HO : (TrackGauges)SelectedItem;
            }
        }

        /// <summary>
        /// Check if a given value is a single bit value or a combination of more than 1 bit
        /// </summary>
        /// <param name="v">The value</param>
        /// <returns>false = v represent a single value, true = v represet a set of values</returns>
        private bool IsSet(int v) {
            int mask = 1;

            while (mask != 0) {
                if ((v & mask) != 0) {
                    return (v & mask) != v;
                }

                mask <<= 1;
            }

            return false;
        }

        public bool IncludeGuageSet {
            get;
            set;
        }
    }
}
