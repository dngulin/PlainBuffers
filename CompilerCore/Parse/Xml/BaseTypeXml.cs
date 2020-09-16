using System.Xml.Serialization;

namespace PlainBuffers.CompilerCore.Parse.Xml {
  public class BaseTypeXml {
    [XmlAttribute("typeName")]
    public string Name;
  }
}