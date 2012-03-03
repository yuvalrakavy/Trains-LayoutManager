using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using LayoutManager;

namespace Test_LayoutBaseServices {
	[TestClass]
	public class PackedArrayTest {
		enum Person { Man, Woman, Child };

		[TestMethod]
		public void TestEnumPackedArray() {
			EnumPackedArray<Person> a = new EnumPackedArray<Person>(1000);

			for(int i = 0; i < a.Length; i++) {
				switch(i % 3) {
					case 0: a[i] = Person.Man; break;
					case 1: a[i] = Person.Woman; break;
					case 2: a[i] = Person.Child; break;
				}
			}

			for(int i = 0; i < a.Length; i++) {
				Person p = Person.Child;

				switch(i % 3) {
					case 0: p = Person.Man; break;
					case 1: p = Person.Woman; break;
					case 2: p = Person.Child; break;
				}

				Assert.AreEqual(a[i], p);
			}

 		}
	}
}
