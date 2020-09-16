using System;
using System.Xml.Serialization;

namespace PlainBuffers.CompilerCore.Parse.Xml {
  [XmlRoot("types")]
  public class TypesXml {
    [XmlAttribute("nameSpace")] public string NameSpace;

    [XmlElement("enum", Type = typeof(EnumXml))]
    [XmlElement("struct", Type = typeof(StructXml))]
    [XmlElement("array", Type = typeof(ArrayXml))]
    public BaseTypeXml[] Types = Array.Empty<BaseTypeXml>();
  }
}