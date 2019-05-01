using System.Linq.Expressions;
using Sprache;

namespace NumberLanguageConsole {
    public class ConstantExpressionParser {
        public readonly Parser<int> NumberParser;
        public readonly Parser<Expression> ConstantParser;
        public ConstantExpressionParser()
        {
            NumberParser = Parse.Number.Select(int.Parse);
            ConstantParser = (NumberParser.Select(num => Expression.Constant(num, typeof(int))))
            .Or(from negativeUnary in Parse.Char('-').Token()
                from num in NumberParser
                select Expression.Constant(num * -1, typeof(int)
            )).Token();
        }        
    }
}