using System.Xml.Serialization;

namespace PlainBuffers.CompilerCore.Parsers.Xml {
  public class BaseTypeXml {
    [XmlAttribute("typeName")]
    public string Name;
  }
}