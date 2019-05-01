using System;
using Xunit;
using NumberLanguageConsole;
using System.Linq.Expressions;
using System.Collections.Generic;
using Sprache;
using FluentAssertions;

namespace NumberLanguageConsole.Tests
{
    public class VariableParserTest : IDisposable
    {
        private readonly KeywordParser KeywordParser;
        private readonly VariableParser VariableParser;

        public VariableParserTest()
        {
            KeywordParser = new KeywordParser();
            VariableParser = new VariableParser(new Dictionary<string, ParameterExpression>(), new Stack<string>());
        }

        // New variable tests
        [Fact]
        public void NewIntVariableParsed()
        {
            var input = "int abc";
            ParameterExpression result = (VariableParser.NewVariableParser.Parse(input) as ParameterExpression);
            result.Type.Should().Be(typeof(int));
            result.Name.Should().Be("abc");
        }

        [Fact]
        public void NewIntArrayVariableParsed()
        {
            var input = "int[] abc";
            ParameterExpression result = (VariableParser.NewVariableParser.Parse(input) as ParameterExpression);
            result.Type.Should().Be(typeof(int[]));
            result.Name.Should().Be("abc");
        }

        [Fact]
        public void NewIntVariableWithWhiteSpaceBeforeAndAfterParsed()
        {
            var input = "   int abc  ";
            ParameterExpression result = (VariableParser.NewVariableParser.Parse(input) as ParameterExpression);
            result.Type.Should().Be(typeof(int));
            result.Name.Should().Be("abc");
        }

        [Fact]
        public void NewIntVariableIsAddedToDictionary()
        {
            var input = "int abc";
            ParameterExpression result = (VariableParser.NewVariableParser.Parse(input) as ParameterExpression);
            VariableParser.VariableDictionary.Should().NotBeEmpty().And.HaveCount(1).And.ContainValue(result);
        }

        [Fact]
        public void NewIntVariableAndArrayIsAddedToStack()
        {
            var input1 = "int abc";
            var input2 = "int def";
            var input3 = "int[] ghi";
            ParameterExpression result1 = (VariableParser.NewVariableParser.Parse(input1) as ParameterExpression);
            ParameterExpression result2 = (VariableParser.NewVariableParser.Parse(input2) as ParameterExpression);
            ParameterExpression result3 = (VariableParser.NewVariableParser.Parse(input3) as ParameterExpression);
            VariableParser.VariableScopeStack.Should().BeEquivalentTo(new [] {result3.Name, result2.Name, result1.Name, }, options => options.WithStrictOrdering());
            VariableParser.VariableScopeStack.Should().HaveCount(3);
        }

        [Fact]
        public void ExceptionIsThrownWhenDeclaringIntTwice()
        {
            var input1 = "int abc";
            var input2 = "int abc";
            VariableParser.NewVariableParser.Parse(input1);
            VariableParser.NewVariableParser.Invoking(o => o.Parse(input2)).Should().Throw<Exception>().WithMessage("Variable abc has already been defined");
        }

        [Fact]
        public void ExceptionIsThrownWhenDeclaringIntArrayTwice()
        {
            var input1 = "int[] abc";
            var input2 = "int[] abc";
            VariableParser.NewVariableParser.Parse(input1);
            VariableParser.NewVariableParser.Invoking(o => o.Parse(input2)).Should().Throw<Exception>().WithMessage("Variable abc has already been defined");
        }

        [Fact]
        public void ExceptionIsThrownWhenDeclaringIntArrayWithSameNameAsInt()
        {
            var input1 = "int abc";
            var input2 = "int[] abc";
            VariableParser.NewVariableParser.Parse(input1);
            VariableParser.NewVariableParser.Invoking(o => o.Parse(input2)).Should().Throw<Exception>().WithMessage("Variable abc has already been defined");
        }

        [Fact]
        public void ExceptionIsThrownWhenDeclaringVariableWithSameNameAsKeyword()
        {
            foreach (var keyword in KeywordParser.Keywords)
            {
                var input = String.Format("int {0}", keyword);
                if (keyword != "int[]") {
                    VariableParser.NewVariableParser.Invoking(o => o.Parse(input)).Should().Throw<Exception>().WithMessage(String.Format("Cannot use keyword {0} as variable name", keyword));
                } else {
                    // Variable name parser does not parse []
                    VariableParser.NewVariableParser.Invoking(o => o.Parse(input)).Should().Throw<Exception>().WithMessage("Cannot use keyword int as variable name");
                }
                
            }
        }

        [Fact]
        public void ExistingIntVariableIsParsedAndMatchedToDefinition()
        {
            var initial = (VariableParser.NewVariableParser.Parse("int abc") as ParameterExpression);
            var result = (VariableParser.ExistingVariableParser.Parse("abc") as ParameterExpression);
            result.Name.Should().BeEquivalentTo(initial.Name);
            result.Type.Should().Be(initial.Type);
        }

        [Fact]
        public void ExistingIntArrayVariableIsParsedAndMatchedToDefinition()
        {
            var initial = (VariableParser.NewVariableParser.Parse("int[] abc") as ParameterExpression);
            var result = (VariableParser.ExistingVariableParser.Parse("abc") as ParameterExpression);
            result.Name.Should().BeEquivalentTo(initial.Name);
            result.Type.Should().Be(initial.Type);
        }

        [Fact]
        public void VariableNotFoundThrowsException()
        {
            VariableParser.ExistingVariableParser.Invoking(o => o.Parse("abc")).Should().Throw<Exception>().WithMessage("Variable abc has not been defined");
        }

        [Fact]
        public void ParsingKeywordAsExistingVariableThrowsException()
        {
            foreach (var keyword in KeywordParser.Keywords)
            {
                VariableParser.ExistingVariableParser.Invoking(o => o.Parse(keyword)).Should().Throw<ParseException>();
            }
        }
        [Fact]
        public void ParseNewOrExistingVariable()
        {
            var input1 = "int abc";
            var input2 = "abc";

            ParameterExpression result1 = (VariableParser.NewOrExistingVariableParser.Parse(input1) as ParameterExpression);
            ParameterExpression result2 = (VariableParser.NewOrExistingVariableParser.Parse(input2) as ParameterExpression);
            result1.Type.Should().Be(typeof(int));
            result1.Name.Should().Be("abc");
            result2.Should().Be(result1);
        }

        public void Dispose()
        {
            VariableParser.VariableDictionary.Clear();
            VariableParser.VariableScopeStack.Clear();
        }

    }
}