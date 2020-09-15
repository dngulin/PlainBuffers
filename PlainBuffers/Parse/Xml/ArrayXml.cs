using System.Xml.Serialization;

namespace PlainBuffers.Parse.Xml {
  public class ArrayXml : BaseTypeXml {
    [XmlAttribute("itemType")] public string ItemTypeName;
    [XmlAttribute("default")] public string ItemDefaultValue;
    [XmlAttribute("length")] public int Length;
  }
}