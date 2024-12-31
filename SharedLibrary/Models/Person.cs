using System.Xml.Serialization;

namespace SharedLibrary.Models;

[XmlRoot("Person")]
public class Person
{
    public int ID { get; set; }
    public string Name { get; set; }

    public int Age { get; set; }
    public string Email { get; set; }
}
