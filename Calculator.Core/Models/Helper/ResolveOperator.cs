using Calculator.Core.Enums;

namespace Calculator.Core.Models.Helper
{
    public class ResolveOperatorHelper
    {
        public Operator ResolveOperator(string id)
        {
            return id.ToLowerInvariant() switch
            {
                "plus" or "addition" => Operator.Plus,
                "subtraction" or "minus" => Operator.Subtraction,
                "multiplication" or "multiply" => Operator.Multiplication,
                "division" or "divide" => Operator.Division,
                _ => throw new InvalidOperationException($"Unsupported operation: {id}")
            };
        }
    }
}
