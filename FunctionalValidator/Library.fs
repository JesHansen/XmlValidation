namespace FunctionalValidator
open System.IO
open System.Xml
open System.Xml.Linq
open System.Xml.Schema

module XML =
    type XmlSchemaPath = SchemaPath of string
    type XmlFilePath = FilePath of string
    type ValidationFailed =
        | XmlFileNotFound of string
        | ValidationFailed of string
    type ValidationSucceeded = ValidationSucceded
    type SchemaValidator (schemapath) =
        let schemaPath =
            if not (File.Exists(schemapath))
            then invalidArg (nameof schemapath) $"Could not find XML schema file here: '{schemapath}'"
            else schemapath
        let xmlSchema =
            let tns = XElement.Load(schemaPath).Attribute("targetNamespace").Value
            let schemaSet = XmlSchemaSet ()
            schemaSet.XmlResolver <- XmlUrlResolver ()
            schemaSet.Add(tns, schemaPath) |> ignore
            schemaSet        
        let validateXmlAgainstSchema (xmlFilePath: string) =
            let doc = XDocument.Load(xmlFilePath)
            let mutable result = Ok ValidationSucceded
            doc.Validate(xmlSchema, fun _ e ->
                result <- Error (ValidationFailed e.Message))
            result
        member this.Check xmlPath =            
            if not (File.Exists(xmlPath)) then Error (XmlFileNotFound $"Did not find an XML file here: '{xmlPath}'")
            else
                validateXmlAgainstSchema xmlPath