using Calculator.Core.Enums;
using Calculator.Core.Interfaces;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Calculator.Core.Models.Helper;

namespace Calculator.Core.Models;

[XmlRoot("Operation")]
public class Operation : IOperation
{
    [XmlAttribute("ID")]
    [JsonPropertyName("ID")]
    public string? ID { get; set; }

    [XmlElement("Value")]
    public List<string>? Value { get; set; }

    [XmlElement("Operation")]
    [JsonPropertyName("Operation")]
    [JsonConverter(typeof(SingleOrListConverter<Operation>))]
    public List<Operation>? NestedOperation { get; set; } = new();


    public double Calculate()
    {
        var operationType = new ResolveOperatorHelper().ResolveOperator(ID); // Fixed the issue by creating an instance of ResolveOperatorHelper.

        var resultValues = Value?.Select(Parse).ToList() ?? new List<double>();
        // Recursively evaluate nested operation and include its result
        if (NestedOperation != null)
        {
            // Add results from nested operations
            foreach (var nested in NestedOperation)
            {
                resultValues.Add(nested.Calculate());
            }

            //double nestedResult = NestedOperation.Calculate();
           // resultValues.Add(nestedResult);  // Treat nested result as a value!
        }

        return operationType switch
        {
            Operator.Plus => resultValues.Sum(),
            Operator.Subtraction => resultValues.Count > 0
                ? resultValues.Skip(1).Aggregate(resultValues.First(), (acc, val) => acc - val)
                : 0,
            Operator.Multiplication => resultValues.Count > 0
                ? resultValues.Aggregate(1.0, (acc, val) => acc * val)
                : 0,
            Operator.Division => resultValues.Count > 1
                ? resultValues.Skip(1).Aggregate(resultValues.First(), (acc, val) =>
                    val == 0 ? throw new DivideByZeroException() : acc / val)
                : 0,
            _ => throw new InvalidOperationException($"Unsupported operator: {ID}")
        };
    }

    private double Parse(string input)
    {
        return double.TryParse(input, out var result) ? result : 0;
    }
}
