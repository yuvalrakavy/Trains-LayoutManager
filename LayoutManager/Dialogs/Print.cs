#region Using directives

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

#endregion

namespace LayoutManager {
    public enum PrintViewScope {
        BestFit, Active, All
    };
}

namespace LayoutManager.Dialogs {
    internal partial class Print : Form {
        private readonly PrintDocument printDoc;
        //private read-only Dictionary<string, IntPtr> devModes = new();

        public Print(PrintDocument printDoc) {
            InitializeComponent();

            string? defaultPrinterName = null;

            this.printDoc = printDoc;

            foreach (string printerName in PrinterSettings.InstalledPrinters) {
                comboBoxPrinters.Items.Add(printerName);
                printDoc.PrinterSettings.PrinterName = printerName;

                if (printDoc.PrinterSettings.IsDefaultPrinter)
                    defaultPrinterName = printerName;
            }

            if (defaultPrinterName != null)
                comboBoxPrinters.SelectedItem = defaultPrinterName;
        }

        public bool AllAreas => radioButtonAllAreas.Checked;

        public PrintViewScope PrintViewScope {
            get {
                if (radioButtonBestFit.Checked)
                    return PrintViewScope.BestFit;
                else return radioButtonAllViews.Checked ? PrintViewScope.All : PrintViewScope.Active;
            }
        }

        public bool GridLines => checkBoxGridLines.Checked;

        private void ButtonOK_Click(object? sender, EventArgs e) {
            printDoc.PrinterSettings.PrinterName = comboBoxPrinters.SelectedItem.ToString() ?? String.Empty;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void ButtonProperties_Click(object? sender, EventArgs e) {
            string printerName = comboBoxPrinters.SelectedItem.ToString() ?? String.Empty;
            if (printDoc.PrinterSettings.PrinterName != printerName)
                printDoc.PrinterSettings.PrinterName = printerName;

            IntPtr devModeHandle = printDoc.PrinterSettings.GetHdevmode();
            IntPtr devModePtr = NativeMethods.GlobalLock(new HandleRef(null, devModeHandle));
            int result = NativeMethods.DocumentProperties(new HandleRef(this, this.Handle), NativeMethods.nullHandleRef, printerName, devModePtr, new HandleRef(null, devModePtr),
                NativeMethods.DocumentPropertyOptions.InBuffer | NativeMethods.DocumentPropertyOptions.Prompt | NativeMethods.DocumentPropertyOptions.OutBuffer);

            _ = NativeMethods.GlobalUnlock(new HandleRef(null, devModeHandle));

            int error = Marshal.GetLastWin32Error();

            if (result < 0)
                MessageBox.Show("DocumentProperties returned error " + error + " (result " + result + ")");
            else if (result == 1) {
                printDoc.PrinterSettings.SetHdevmode(devModeHandle);
                printDoc.DefaultPageSettings.SetHdevmode(devModeHandle);
            }
        }

        private class NativeMethods {
            private NativeMethods() {
            }

            static internal HandleRef nullHandleRef = new();

            [DllImport("kernel32.dll", PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern IntPtr GlobalAlloc(uint flags, int size);

            [DllImport("kernel32.dll", PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern IntPtr GlobalLock(HandleRef hGlobal);

            [DllImport("kernel32.dll", PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern int GlobalUnlock(HandleRef hGlobal);

            [DllImport("winspool.drv", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern int DocumentProperties(HandleRef hwnd, HandleRef hPrinter, string pDeviceName, IntPtr pDevModeOutput, HandleRef pDevModeInput, DocumentPropertyOptions fMode);

            [DllImport("winspool.drv", SetLastError = true, CharSet = CharSet.Unicode)]
            static internal extern int OpenPrinter(string pPrinterName, out IntPtr phPrinter, int pDefault);

            [DllImport("winspool.drv", SetLastError = true)]
            static internal extern int ClosePrinter(IntPtr hPrinter);

            [Flags]
            internal enum DocumentPropertyOptions : uint {
                None = 0,
                InBuffer = 8,
                Prompt = 4,
                OutBuffer = 2,
                OutDefault = 1,
            }
        }
    }
}