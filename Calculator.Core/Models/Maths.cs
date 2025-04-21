using System.Xml.Serialization;

namespace Calculator.Core.Models;

[XmlRoot("Maths")]
public class Maths
{
    [XmlElement("Operation")]
    public Operation? Operation { get; set; }
}

