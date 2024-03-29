using System.Diagnostics;

namespace LayoutManager {
    public interface ILayoutSwitch {
    };

    /// <summary>
    /// Specify TraceSwitch that is unique to the State Broker system
    /// </summary>
    public class LayoutTraceSwitch : TraceSwitch, ILayoutSwitch {
        public LayoutTraceSwitch(string displayName, string description, string defaultValue)
            : base(displayName, description, defaultValue) {
        }

        public LayoutTraceSwitch(string displayName, string description)
            : base(displayName, description) {
        }
    }

    public class LayoutBooleanSwitch : BooleanSwitch, ILayoutSwitch {
        public LayoutBooleanSwitch(string displayName, string description, string defaultValue)
            : base(displayName, description, defaultValue) {
        }

        public LayoutBooleanSwitch(string displayName, string description)
            : base(displayName, description) {
        }
    }

    /// <summary>
    /// Trace switch that can be either set by itself, or follow the setting of a subsystem wide trace switch
    /// </summary>
    public class LayoutTraceSubsystem : LayoutTraceSwitch {
        private readonly LayoutTraceSwitch master;

        public LayoutTraceSubsystem(LayoutTraceSwitch master, string displayName, string description)
            : base(displayName, description) {
            this.master = master;
        }

        public new bool TraceError => master.TraceError || base.TraceError;

        public new bool TraceWarning => master.TraceWarning || base.TraceWarning;

        public new bool TraceInfo => master.TraceInfo || base.TraceInfo;

        public new bool TraceVerbose => master.TraceVerbose || base.TraceVerbose;
    }
}
