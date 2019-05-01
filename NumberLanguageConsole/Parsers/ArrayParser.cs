using Sprache;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace NumberLanguageConsole {
    public class ArrayParser {
        private ConstantExpressionParser ConstantExpressionParser;
        private VariableParser VariableParser;
        
        // TODO value with ValueExpressionParser eg. Length or Read or Add or Array Access or existing variable or constant
        public Parser<Expression> ArrayAccessParser;

        public ArrayParser(Dictionary<string, ParameterExpression> variableDictionary, Stack<string> variableScopeStack)
        {
            ConstantExpressionParser = new ConstantExpressionParser();
            VariableParser = new VariableParser(variableDictionary, variableScopeStack);
            ArrayAccessParser = (
                from variableName in VariableParser.ExistingVariableParser
                from openingBracket in Parse.Char('[')
                from value in Parse.Ref(() => ConstantExpressionParser.ConstantParser)
                from closingBracket in Parse.Char(']')
                select Expression.ArrayAccess(variableName, value)
            ).Token();
        }
    }
}

