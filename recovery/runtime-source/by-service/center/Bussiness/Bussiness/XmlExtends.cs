using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Bussiness;

public static class XmlExtends
{
	public static string ToString(this XElement node, bool check)
	{
		StringBuilder stringBuilder = new StringBuilder();
		XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
		xmlWriterSettings.CheckCharacters = check;
		xmlWriterSettings.OmitXmlDeclaration = true;
		xmlWriterSettings.Indent = true;
		using (XmlWriter writer = XmlWriter.Create(stringBuilder, xmlWriterSettings))
		{
			node.WriteTo(writer);
		}
		return stringBuilder.ToString();
	}
}
