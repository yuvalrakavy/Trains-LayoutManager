namespace LayoutManager {

    public class ApplicationStateInfo : LayoutXmlWrapper {
		string filename;

		public enum LayoutState {
			Design, Operation, Simulation
		};

		public ApplicationStateInfo() : base("ApplicationState") {
		}

		public ApplicationStateInfo(string filename) {
			this.filename = filename;

            try {
                Load(filename);
            }
            catch (System.IO.FileNotFoundException) {
                InitElement("ApplicationState");
            }
		}

		public void Save() {
			Element.OwnerDocument.Save(filename);
		}

		public string LayoutFilename {
			get {
				return GetAttribute("LayoutFilename");
			}

			set {
				SetAttribute("LayoutFilename", value);
			}
		}
	}
}
