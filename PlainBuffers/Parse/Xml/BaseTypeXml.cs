using System.Xml.Serialization;

namespace PlainBuffers.Parse.Xml {
  public class BaseTypeXml {
    [XmlAttribute("typeName")]
    public string Name;
  }
}