namespace FunctionalValidator

open System.IO
open System.Xml
open System.Xml.Linq
open System.Xml.Schema

module XML =
    type XmlSchemaPath = SchemaPath of string
    type XmlFilePath = XmlFilePath of string
    type ValidationSucceeded = ValidationSucceded
    type ValidationFailed =
        | XmlFileNotFound of string
        | ValidationFailed of string

    type SchemaValidator(schemapath) =
        let schemaPath =
            if File.Exists(schemapath) then SchemaPath schemapath
            else invalidArg (nameof schemapath) $"Could not find XML schema file here: '{schemapath}'"
        let loadXmlSchema (SchemaPath path) =
            let tns = XElement.Load(path).Attribute("targetNamespace").Value
            let schemaSet = XmlSchemaSet()
            schemaSet.XmlResolver <- XmlUrlResolver()
            schemaSet.Add(tns, path) |> ignore
            schemaSet

        let validateXmlAgainstSchema (XmlFilePath filePath) =
            let doc = XDocument.Load(filePath)
            let mutable result = Ok ValidationSucceded
            doc.Validate(loadXmlSchema schemaPath, (fun _ e -> result <- Error(ValidationFailed e.Message)))
            result

        member this.Check xmlPath =
            if File.Exists(xmlPath) then validateXmlAgainstSchema (XmlFilePath xmlPath)
            else Error(XmlFileNotFound $"Did not find an XML file here: '{xmlPath}'")