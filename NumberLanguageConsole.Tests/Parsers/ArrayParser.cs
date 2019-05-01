using System;
using Xunit;
using NumberLanguageConsole;
using System.Linq.Expressions;
using Sprache;
using FluentAssertions;
using System.Collections.Generic;

namespace NumberLanguageConsole.Tests
{
    public class ArrayParserTest : IDisposable
    {        
        private ArrayParser ArrayParser;
        private VariableParser VariableParser;
        public ArrayParserTest()
        {
            var dictionary = new Dictionary<string, ParameterExpression>();
            var stack = new Stack<string>();
            ArrayParser = new ArrayParser(dictionary, stack);
            VariableParser = new VariableParser(dictionary, stack);
        }

        [Fact]
        public void ConstantArrayAccessorIsParsed()
        {
            var input = "int abc";
            ParameterExpression result = (VariableParser.NewVariableParser.Parse(input) as ParameterExpression);
            result.Type.Should().Be(typeof(int));
            result.Name.Should().Be("abc");
            
        }

        public void Dispose()
        {
            VariableParser.VariableDictionary.Clear();
            VariableParser.VariableScopeStack.Clear();
        }
    }
}