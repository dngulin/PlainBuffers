using System;
using System.Xml.Serialization;

namespace PlainBuffers.CompilerCore.Parse.Xml {
  public class StructXml : BaseTypeXml {
    [XmlElement("field")]
    public FieldXml[] Fields = Array.Empty<FieldXml>();
  }

  public class FieldXml {
    [XmlAttribute("type")] public string Type;
    [XmlAttribute("name")] public string Name;
    [XmlAttribute("default")] public string Default;
  }
}