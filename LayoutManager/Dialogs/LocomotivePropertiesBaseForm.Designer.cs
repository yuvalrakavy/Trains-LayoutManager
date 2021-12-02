using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using System.Linq;

using LayoutManager.Model;
using LayoutManager.CommonUI.Controls;

namespace LayoutManager.Dialogs {
    /// <summary>
    /// Base form for the locomotive & locomotive type properties
    /// </summary>
    partial class LocomotiveBasePropertiesForm : Form {
        private ImageGetter imageGetter;

        private ListView listViewFunctions;
        private AttributesEditor attributesEditor;
    }
}

