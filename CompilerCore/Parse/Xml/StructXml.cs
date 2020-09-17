using System;
using System.Xml.Serialization;

namespace PlainBuffers.CompilerCore.Parse.Xml {
  public class StructXml : BaseTypeXml {
    [XmlElement("field")]
    public FieldXml[] Fields {
      get => _fields ?? Array.Empty<FieldXml>();
      set => _fields = value;
    }
    private FieldXml[] _fields;
  }

  public class FieldXml {
    [XmlAttribute("type")] public string Type;
    [XmlAttribute("name")] public string Name;
    [XmlAttribute("default")] public string Default;
  }
}