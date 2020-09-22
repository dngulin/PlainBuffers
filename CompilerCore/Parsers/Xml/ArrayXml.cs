using System.Xml.Serialization;

namespace PlainBuffers.CompilerCore.Parsers.Xml {
  public class ArrayXml : BaseTypeXml {
    [XmlAttribute("itemType")] public string ItemType;
    [XmlAttribute("default")] public string ItemDefault;
    [XmlAttribute("length")] public int Length;
  }
}