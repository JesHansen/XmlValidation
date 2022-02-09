#pragma warning disable CA1812

using FunctionalValidator;
[assembly:CLSCompliant(false)]

var validator = new XML.SchemaValidator(@"C:\src\Skat\rente_schemas\xml\skat2021\view\RenteIndberetningPensiondiverseStrukturType.xsd");
var result = validator.Check(@"C:\src\Skat\TestXML.xml");


var handleOk = (XML.ValidationSucceeded _) =>
{
    Console.WriteLine("OK!");
    return true;
};
var handleBadness = (XML.ValidationFailed failure) =>
{
    switch (failure)
    {
        case XML.ValidationFailed.XmlFileNotFound missing :
            Console.WriteLine($"{missing.Item}");
            break;
        case XML.ValidationFailed.ValidationFailed validationError :
            Console.WriteLine($"XML file is invalid according to the schema: {validationError.Item}");
            break;
    }
    
    return false;
};

result.Match(handleOk, handleBadness);

