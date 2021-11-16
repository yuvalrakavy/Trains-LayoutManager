using System;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

#pragma warning disable IDE0051, IDE0060
namespace LayoutManager.Components {
    [LayoutModule("Layout I/O Services")]
    public class LayoutIOServices : LayoutModuleBase {
        public const string A_Port = "Port";
        public const string A_OverlappedIO = "OverlappedIO";
        public const string A_BufferSize = "BufferSize";
        public const string E_ModeString = "ModeString";
        public const string A_ReadIntervalTimeout = "ReadIntervalTimeout";
        public const string A_ReadTotalTimeoutConstant = "ReadTotalTimeoutConstant";
        public const string A_ReadTotalTimeoutMultiplier = "ReadTotalTimeoutMultiplier";
        public const string A_WriteTotalTimeoutMultiplier = "WriteTotalTimeoutMultiplier";

        /// <summary>
        /// Request opening of a serial communication device. The sender is an XML element providing details and setup information
        /// regarding the device to be opened
        /// </summary>
        /// <remarks>
        /// The following XML attributes are used:
        ///		Port						the port to be opened (e.g. com1)
        ///		OverlappedIO				if True, the stream will be opened in overlapped IO mode
        ///		ReadIntervalTimeout			The timeout between input characters (in milliseconds)
        ///		ReadTotalTimeoutConstant	The total time for read operation
        ///	The following child elements are used:
        ///		ModeString					setup string for setting parameters such as baud rate and parity. The syntax is the same as for the MODE command
        /// </remarks>
        /// <param name="e.Sender">XML element with the attributes specifying various details</param>
        /// <returns>e.Info will be a Stream for the opened device</returns>
        [LayoutEvent("open-serial-communication-device-request")]
        private void openSerialCommunicationDeviceRequest(LayoutEvent e) {
            XmlElement setupElement = (XmlElement)e.Sender;
            string port = setupElement.GetAttribute(A_Port);
            bool overlappedIO = (bool?)setupElement.AttributeValue(A_OverlappedIO) ?? false;
            Win32createFileFlags createFlags = 0;
            FileStream commStream;
            int error;

            if (overlappedIO)
                createFlags |= Win32createFileFlags.Overlapped;

            // Convert COMx to \\.\COMx in order to support COM10 and above
            if (port.StartsWith("COM", StringComparison.InvariantCultureIgnoreCase))
                port = "\\\\.\\" + port;

            IntPtr commHandle = NativeMethods.CreateFile(port, Win32accessModes.GenericRead | Win32accessModes.GenericWrite, 0, 0,
                Win32createDisposition.OpenExisting, createFlags, (IntPtr)0);
            error = Marshal.GetLastWin32Error();

            if (commHandle.ToInt32() == -1)
                throw new IOException("Could not open " + port + " (error " + String.Format("{0:x}", error) + ")");

            int bufferSize = (int?)setupElement.AttributeValue(A_BufferSize) ?? 4;

            commStream = new FileStream(new SafeFileHandle(commHandle, true), FileAccess.ReadWrite, bufferSize, overlappedIO);

            string modeString = setupElement[E_ModeString].InnerText;
            DCB dcb = new();

            // Fix mode string (remove CR/LF etc.)
            modeString = System.Text.RegularExpressions.Regex.Replace(modeString, "[\\r\\n ]+", " ", System.Text.RegularExpressions.RegexOptions.None);

            if (!NativeMethods.BuildCommDCB(modeString, dcb))
                throw new ArgumentException("Invalid setting for serial port " + port + ": " + modeString);

            dcb.flags = (uint)DCBflags.OutxCtsFlow;

            if (!NativeMethods.SetCommState(commStream.SafeFileHandle.DangerousGetHandle(), dcb))
                throw new IOException("Failed to set communication port " + port + " parameters");

            CommunicationTimeouts commTimeouts = new() {
                ReadIntervalTimeout = (uint?)setupElement.AttributeValue(A_ReadIntervalTimeout) ?? 0,
                ReadTotalTimeoutConstant = (uint?)setupElement.AttributeValue(A_ReadTotalTimeoutConstant) ?? 0,
                ReadTotalTimeoutMultiplier = (uint?)setupElement.AttributeValue(A_ReadTotalTimeoutMultiplier) ?? 0,
                WriteTotalTimeoutConstant = (uint?)setupElement.AttributeValue(A_WriteTotalTimeoutMultiplier) ?? 0
            };

            if (!NativeMethods.SetCommTimeouts(commStream.SafeFileHandle.DangerousGetHandle(), commTimeouts)) {
                error = Marshal.GetLastWin32Error();
                throw new IOException("Could not set " + port + " timeout (error " + String.Format("{0:x}", error) + ")");
            }

            e.Info = commStream;
        }

        [LayoutEvent("create-named-pipe-request")]
        private void createNamedPipeRequest(LayoutEvent e) {
            var pipeName = Ensure.NotNull<string>(e.Sender, "pipeName");
            var overlappedIO = (bool)e.Info;
            PipeOpenModes pipeOpenMode = PipeOpenModes.AccessDuplex;

            if (overlappedIO)
                pipeOpenMode |= PipeOpenModes.Overlapped;

            System.IntPtr handle = NativeMethods.CreateNamedPipe(pipeName,
                pipeOpenMode, 0, 2, 0, 0, 1000, 0);

            if ((int)handle == -1)
                throw new IOException("Unable to create named pipe: " + pipeName);

            e.Info = new SafeFileHandle(handle, true);
        }

        [LayoutEvent("wait-named-pipe-client-to-connect-request")]
        private void waitNamedPipeClientToConnectRequest(LayoutEvent e) {
            var safeHandle = Ensure.NotNull<SafeFileHandle>(e.Sender, "safeHandle");
            bool overlappedIO = (bool)e.Info;

            // Wait for the client to connect
            NativeMethods.ConnectNamedPipe(safeHandle.DangerousGetHandle(), 0);

            e.Info = new FileStream(safeHandle, FileAccess.ReadWrite, 4, overlappedIO);
        }

        [LayoutEvent("disconnect-named-pipe-request")]
        private void disconnectNamedPipeRequest(LayoutEvent e) {
            IntPtr handle;

            if (e.Sender is SafeFileHandle)
                handle = ((SafeFileHandle)e.Sender).DangerousGetHandle();
            else if (e.Sender is IntPtr)
                handle = (IntPtr)e.Sender;
            else if (e.Sender is FileStream)
                handle = ((FileStream)e.Sender).SafeFileHandle.DangerousGetHandle();
            else
                throw new ArgumentException("Invalid pipe handle");

            NativeMethods.DisconnectNamedPipe(handle);
        }

        [LayoutEvent("wait-named-pipe-request")]
        private void waitNamedPipeRequest(LayoutEvent e) {
            string pipeName = (string)e.Sender;
            bool overlappedIO = (bool)e.Info;
            Win32createFileFlags flags = 0;

            NativeMethods.WaitNamedPipe(pipeName, WaitForever);

            if (overlappedIO)
                flags |= Win32createFileFlags.Overlapped;

            System.IntPtr handle = NativeMethods.CreateFile(pipeName, Win32accessModes.GenericRead | Win32accessModes.GenericWrite,
                0, 0, Win32createDisposition.OpenExisting, flags, (IntPtr)null);
            int error = Marshal.GetLastWin32Error();

            if (handle.ToInt32() == -1)
                throw new IOException("Unable to open named pipe " + pipeName + " (error " + error + ")");

            e.Info = new FileStream(new SafeFileHandle(handle, true), FileAccess.ReadWrite, 4, overlappedIO);
        }

        #region System structures and flags

        [Flags]
        internal enum Win32accessModes : uint {
            GenericRead = 0x80000000,
            GenericWrite = 0x40000000,
            GenericExecute = 0x20000000,
            GenericAll = 0x10000000
        };

        internal enum Win32createDisposition {
            None = 0,
            CreateNew = 1,
            CreateAlways = 2,
            OpenExisting = 3,
            OpenAlways = 4,
            TruncateExisting = 5,
        }

        [Flags]
        internal enum Win32createFileFlags : uint {
            WriteThrough = 0x80000000,
            Overlapped = 0x40000000,
            NoBuffering = 0x20000000,
            RandomAccess = 0x10000000,
            SequentialScan = 0x08000000,
            DeleteOnClose = 0x04000000,
            BackupSemantics = 0x02000000,
            PosixSemantics = 0x01000000,
            OpenReparsePoint = 0x00200000,
            OpenNoRecall = 0x00100000,
            FirstPipeInstance = 0x00080000,
        };

        [Flags]
        internal enum Win32commEvents {
            RxChar = 0x0001,        // Any Character received,
            RxFlag = 0x0002,        // Received certain character,
            TxEmpty = 0x0004,   // Transmitt Queue Empty,
            CTS = 0x0008,       // CTS changed state,
            DSR = 0x0010,       // DSR changed state,
            RLSD = 0x0020,      // RLSD changed state,
            Break = 0x0040,     // BREAK received,
            Error = 0x0080,     // Line status error occurred,
            Ring = 0x0100,      // Ring signal detected,
            PrinterError = 0x0200,      // Printer error occured,
            Rx80Full = 0x0400,  // Receive buffer is 80 percent full,
            Event1 = 0x0800,        // Provider specific event 1,
            Event2 = 0x1000,        // Provider specific event 2,
        };

        [Flags]
        internal enum PipeOpenModes : uint {
            AccessInbound = 0x00000001,
            AccessOutbound = 0x00000002,
            AccessDuplex = 0x00000003,
            Overlapped = 0x40000000,
            FirstPipeInstance = 0x00080000,
            WriteThrough = 0x80000000,
        };

        [Flags]
        internal enum PipeModes : uint {
            Wait = 0x00000000,
            NoWait = 0x00000001,
            ReadModeByte = 0x00000000,
            ReadModeMessage = 0x00000002,
            PipeTypeByte = 0x00000000,
            PipeTypeMessage = 0x00000004,
        };

        [Flags]
        internal enum DCBflags {
            Binary = 0x00000001,
            Parity = 0x00000002,
            OutxCtsFlow = 0x00000004,
            OutxDsrFlow = 0x00000008,
            DtrControlMask = 0x00000030,
            DtrControlDisable = 0x00000000,
            DtrControlEnable = 0x00000010,
            DtrControlHabdshake = 0x00000020,
            DsrSensitivity = 0x00000040,
            TxContinueOnXOff = 0x00000080,
            OutX = 0x00000100,
            InX = 0x00000200,
            ErrorChar = 0x00000400,
            Null = 0x00000800,
            RtsControlMask = 0x00003000,
            RtsControlDisable = 0x00000000,
            RtsControlEnable = 0x00001000,
            RtsControlHandshake = 0x00002000,
            RtsControlToggle = 0x00003000,
            AbortOnError = 0x00004000,
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class DCB {
            internal DCB() {
                DCBlength = (uint)Marshal.SizeOf(typeof(DCB));
            }

            public System.UInt32 DCBlength;
            public System.UInt32 baudRate = 0;
            public System.UInt32 flags = 0;
            public System.UInt16 reserved = 0;
            public System.UInt16 XonLim = 0;
            public System.UInt16 XoffLim = 0;
            public System.Byte byteSize = 0;
            public System.Byte parity = 0;
            public System.Byte stopBits = 0;
            public System.SByte XonChar = 0;
            public System.SByte XoffChar = 0;
            public System.SByte errorChar = 0;
            public System.SByte eofChar = 0;
            public System.SByte evtChar = 0;
            public System.UInt16 reserved1 = 0;
        };

        [StructLayout(LayoutKind.Sequential)]
        internal class CommunicationTimeouts {
            public System.UInt32 ReadIntervalTimeout;
            public System.UInt32 ReadTotalTimeoutMultiplier;
            public System.UInt32 ReadTotalTimeoutConstant;
            public System.UInt32 WriteTotalTimeoutMultiplier = 0;
            public System.UInt32 WriteTotalTimeoutConstant = 0;
        }

        internal const System.UInt32 WaitForever = 0xffffffff;

        #endregion

        #region External method definitions

        internal class NativeMethods {
            private NativeMethods() { }

            [DllImport("KERNEL32.DLL", EntryPoint = "BuildCommDCBW", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern bool BuildCommDCB([MarshalAs(UnmanagedType.LPWStr)] string src, DCB dcb);

            [DllImport("KERNEL32.DLL", EntryPoint = "SetCommState", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern bool SetCommState(IntPtr handle, DCB dcb);

            [DllImport("KERNEL32.DLL", EntryPoint = "SetCommTimeouts", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern bool SetCommTimeouts(IntPtr handle, CommunicationTimeouts commTimeouts);

            [DllImport("KERNEL32.DLL", EntryPoint = "ReadFile", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern bool ReadFile(
                IntPtr handle,
                [MarshalAs(UnmanagedType.LPArray)] byte[] buffer,
                uint bytesToRead,
                out uint bytesRead,
                uint mbzOverlapped);

            [DllImport("KERNEL32.DLL", EntryPoint = "SetCommMask", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern bool SetCommMask(IntPtr handle, Win32commEvents mask);

            [DllImport("KERNEL32.DLL", EntryPoint = "WaitCommEvent", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern bool WaitCommEvent(IntPtr handle, out Win32commEvents mask, uint mbzOverlapped);

            [DllImport("KERNEL32.DLL", EntryPoint = "CreateFileW", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern System.IntPtr CreateFile(
                [MarshalAs(UnmanagedType.LPWStr)] string filename,
                Win32accessModes desiredAccess,
                System.UInt32 shareMode,
                System.UInt32 mbzSecurity,
                Win32createDisposition creationDisposition,
                Win32createFileFlags flags,
                System.IntPtr hTemplateFile);

            [DllImport("KERNEL32.DLL", EntryPoint = "CloseHandle", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern bool CloseHandle(IntPtr handle);

            [DllImport("KERNEL32.DLL", EntryPoint = "CreateNamedPipeW", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern System.IntPtr CreateNamedPipe(
                [MarshalAs(UnmanagedType.LPWStr)] string pipeName,
                PipeOpenModes openMode,
                PipeModes pipeMode,
                System.UInt32 nMaxInstances,
                System.UInt32 outBufferSize,
                System.UInt32 inBufferSize,
                System.UInt32 defaultTimeout,
                System.UInt32 mbzSecurity);

            [DllImport("KERNEL32.DLL", EntryPoint = "DisconnectNamedPipe", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern bool DisconnectNamedPipe(System.IntPtr handle);

            [DllImport("KERNEL32.DLL", EntryPoint = "ConnectNamedPipe", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern bool ConnectNamedPipe(System.IntPtr handle, System.UInt32 mbz);

            [DllImport("KERNEL32.DLL", EntryPoint = "WaitNamedPipeW", SetLastError = true,
                 PreserveSig = true, CallingConvention = CallingConvention.Winapi)]
            static internal extern bool WaitNamedPipe(
                [MarshalAs(UnmanagedType.LPWStr)] string pipeName,
                System.UInt32 timeout);
        }

        #endregion

    }
}