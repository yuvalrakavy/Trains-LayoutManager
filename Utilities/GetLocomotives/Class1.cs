using System;
using System.Xml;
using System.Net;
using System.IO;
using System.Drawing;
using System.Text.RegularExpressions;

namespace GetLocomotives
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	class GetLocos
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			String	webSite = "http://212.185.118.6";

			XmlDocument	doc = new XmlDocument();
			doc.LoadXml("<LocomotiveCatalog />");

			String	pageContent;

			pageContent = getPage(webSite + "/produkt.nsf/Suchagent?Openagent&E10-(Theme1=\"Electric%20Locos\")");
			parsePage(pageContent, webSite, "Europe", "Electric", doc);

#if notnow
			pageContent = getPage(webSite + "/produkt.nsf/Suchagent?Openagent&E10-(Theme1=\"Diesel%20&%20Other%20Locos\")");
			parsePage(pageContent, webSite, "Europe", "Diesel", doc);

			pageContent = getPage(webSite + "/produkt.nsf/Suchagent?Openagent&E10-(Theme1=\"Steam%20Locos\")");
			parsePage(pageContent, webSite, "Europe", "Steam", doc);
#endif

			if(args.Length > 0)
				doc.Save(args[0]);
		}

		static void parsePage(String pageContent, String webSite, String origin, String kind, XmlDocument doc) {
			Regex	rTable = new Regex("<table.*>(?s:.*?)</table>", RegexOptions.IgnoreCase);

			Match m = rTable.Match(pageContent);
			m = m.NextMatch();

			parseTable(m.Value, webSite, origin, kind, doc);
		}

		static void parseTable(String table, String webSite, String origin, String kind, XmlDocument doc) {
			Regex	rRow = new Regex("<tr.*?>(?s:.*?)</tr>", RegexOptions.IgnoreCase);
			Regex	rCol = new Regex("<td.*?>(.*?)</td>", RegexOptions.IgnoreCase);
			Regex	rImage = new Regex("<img +src=\"(.*?)\"", RegexOptions.IgnoreCase);
			Regex	rContent = new Regex("(<.*?>)*(.*?)<", RegexOptions.IgnoreCase);

			Match mRow = rRow.Match(table);

			for(mRow = mRow.NextMatch(); mRow.Success; mRow = mRow.NextMatch()) {
				XmlElement	typeElement = doc.CreateElement("LocomotiveType");

				typeElement.SetAttribute("Origin", origin);
				typeElement.SetAttribute("Kind", kind);
				typeElement.SetAttribute("ID", Guid.NewGuid().ToString());

				mRow = mRow.NextMatch();		// Skip space line

				if(!mRow.Success)
					break;

				Match	mCol = rCol.Match(mRow.Value);

				Match	mImage = rImage.Match(mCol.Value);
				String	imageUrl = mImage.Groups[1].Value;

				mCol = mCol.NextMatch();

				String	productNumber = rContent.Match(mCol.Value).Groups[2].Value;

				mCol = mCol.NextMatch();			// Skip empty column

				mCol = mCol.NextMatch();
				String	productDescription = rContent.Match(mCol.Value).Groups[2].Value;

				XmlElement	nameElement = doc.CreateElement("TypeName");
				nameElement.InnerText = "LGB " + productNumber + " " + productDescription;;

				typeElement.AppendChild(nameElement);

				Console.Write("Getting image for " + nameElement.InnerText + " - ");
				Stream	s = WebRequest.Create(webSite + imageUrl).GetResponse().GetResponseStream();
				Console.WriteLine("Done");

				Image			image = Image.FromStream(s);
				MemoryStream	ms = new MemoryStream();

				image.Save(ms, image.RawFormat);

				XmlElement		imageElement = doc.CreateElement("Image");

				imageElement.InnerText = Convert.ToBase64String(ms.GetBuffer());
				ms.Close();

				typeElement.AppendChild(imageElement);
				doc.DocumentElement.AppendChild(typeElement);
			}
		}

		static String getPage(String url) {
			WebRequest	wreq = WebRequest.Create(url);

			Console.Write("Getting page from: " + url);

			WebResponse	response = wreq.GetResponse();
			Stream		s = response.GetResponseStream();
			TextReader	r = new StreamReader(s);


			String		result = r.ReadToEnd();

			response.Close();
			Console.WriteLine(" Done.");

			return result;
		}
	}
}
