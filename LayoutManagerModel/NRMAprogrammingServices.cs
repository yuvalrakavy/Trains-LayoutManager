using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Xml;
using System.Diagnostics;
using System.IO;

using LayoutManager;

namespace LayoutManager.Model {
	public class ReadCVresult {
		public LayoutActionResult Result { get; private set; }
		public int CV { get; private set; }
		public byte Value { get; private set; }

		public ReadCVresult(LayoutActionResult programmingResult, int cv, byte value) {
			this.Result = programmingResult;
			this.CV = cv;
			this.Value = value;
		}
	}
}
