using System;
using Sprache;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace NumberLanguageConsole {
    public class VariableParser {
        public Dictionary<string, ParameterExpression> VariableDictionary;
        public Stack<string> VariableScopeStack;
        private readonly KeywordParser KeywordParser;
        private readonly StringParser StringParser;
        public readonly Parser<Expression> NewVariableParser;
        public readonly Parser<Expression> ExistingVariableParser;
        public Parser<Expression> NewOrExistingVariableParser;
        public VariableParser(Dictionary<string, ParameterExpression> variableDictionary, Stack<string> variableScopeStack)
        {
            VariableDictionary = variableDictionary;
            VariableScopeStack = variableScopeStack;
            KeywordParser = new KeywordParser();
            StringParser = new StringParser();
            NewVariableParser = (
                from type in KeywordParser.IntArrayParser.Or(KeywordParser.IntParser)
                from variableName in StringParser.VariableNameParser
                select addNewVariable(variableName, type)
            );
            ExistingVariableParser = (
                from notKeyword in KeywordParser.NotKeywordParser
                from variable in StringParser.VariableNameParser
                select GetExistingVariable(variable)
            );
            NewOrExistingVariableParser = NewVariableParser.Or(ExistingVariableParser);
        }
        private ParameterExpression addNewVariable(string variableName, string type)
        {
            if (VariableDictionary.ContainsKey(variableName)) {
                throw new Exception(String.Format("Variable {0} has already been defined", variableName));
            }
            if (KeywordParser.Keywords.Contains(variableName))
            {
                throw new Exception(String.Format("Cannot use keyword {0} as variable name", variableName));
            }
            ParameterExpression expression = null;
            if (type == "int[]")
            {
                expression = Expression.Variable(typeof(int[]), variableName);
            } else {
                expression = Expression.Variable(typeof(int), variableName); 
            }

            VariableDictionary.Add(variableName, expression);
            VariableScopeStack.Push(variableName);
            return expression;
        }

        public Expression GetExistingVariable(string variable)
        {
            if (!VariableDictionary.ContainsKey(variable))
            {
                throw new Exception(String.Format("Variable {0} has not been defined", variable));
            }
            return VariableDictionary[variable];
        }
    }
}
