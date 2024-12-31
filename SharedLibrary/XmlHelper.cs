using System.Xml.Serialization;
using System;
using System.Xml.Schema;
using System.Xml;
using SharedLibrary.Models;

namespace SharedLibrary;

public static class XmlHelper
{

    public static (string XmlData, List<string> ValidationErrors) GenerateAndValidateXml(Person person)
    {
        var xmlSerializer = new XmlSerializer(typeof(Person));
        using var stringWriter = new StringWriter();
        xmlSerializer.Serialize(stringWriter, person);
        var xmlData = stringWriter.ToString();

        var validationErrors = new List<string>();
        var isValid = ValidateXml(xmlData, validationErrors);

        return isValid ? (xmlData, null) : (null, validationErrors);
    }

    public static bool ValidateXml(string xmlData,List<string> validationErrors)
    {
        string xsdPath = @"C:\myprojects\xsdexample-api\SharedLibrary\XML\DataSchema.xsd";
        var schema = new XmlSchemaSet();
        schema.Add("", xsdPath);

        var xmlReaderSettings = new XmlReaderSettings
        {
            ValidationType = ValidationType.Schema,
            Schemas = schema
        };

        xmlReaderSettings.ValidationEventHandler += (sender, e) =>
        {
            validationErrors.Add(e.Message);
        };

        using var stringReader = new StringReader(xmlData);
        using var xmlReader = XmlReader.Create(stringReader, xmlReaderSettings);

        try
        {
            while (xmlReader.Read()) { }
            return !validationErrors.Any();
        }
        catch (XmlSchemaValidationException ex)
        {
            validationErrors.Add($"Validation Exception: {ex.Message}");
            return false;
        }
    }

    public static T ParseXml<T>(string xmlData)
    {
        var xmlSerializer = new XmlSerializer(typeof(T));
        using var stringReader = new StringReader(xmlData);
        return (T)xmlSerializer.Deserialize(stringReader);
    }
}
