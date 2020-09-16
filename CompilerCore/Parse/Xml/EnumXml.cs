using System.Xml.Serialization;

namespace PlainBuffers.CompilerCore.Parse.Xml {
  public class EnumXml : BaseTypeXml {
    [XmlAttribute("itemType")] public string UnderlyingType;
    [XmlAttribute("flags")] public bool IsFlags;
    [XmlElement("item")] public ItemXml[] Items;
  }

  public class ItemXml {
    [XmlAttribute("name")] public string Name;
    [XmlAttribute("value")] public string Value;
  }
}