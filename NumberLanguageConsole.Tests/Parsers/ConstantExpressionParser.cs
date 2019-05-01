using System;
using Xunit;
using NumberLanguageConsole;
using System.Linq.Expressions;
using Sprache;
using FluentAssertions;

namespace NumberLanguageConsole.Tests
{
    public class ConstantExpressionParserTest
    {
        private ConstantExpressionParser constantExpressionParser;

        public ConstantExpressionParserTest()
        {
            constantExpressionParser = new ConstantExpressionParser();
        }

        [Fact]
        public void NumberIsParsed()
        {
            var input = "3";
            var result = constantExpressionParser.ConstantParser.Parse(input) as ConstantExpression;
            result.Value.Should().Be(3);
        }

        [Fact]
        public void NumberWithWhiteSpaceBeforeIsParsed()
        {
            var input = "  3";
            var result = constantExpressionParser.ConstantParser.Parse(input) as ConstantExpression;
            result.Value.Should().Be(3);
        }

        [Fact]
        public void NumberWithWhiteSpaceBeforeAndAfterIsParsed()
        {
            var input = " 3 ";
            var result = constantExpressionParser.ConstantParser.Parse(input) as ConstantExpression;
            result.Value.Should().Be(3);
        }

        [Fact]
        public void NumberWithWhiteSpaceAndNewLinesBeforeAndAfterIsParsed()
        {
            var input = "\n  3  \n";
            var result = constantExpressionParser.ConstantParser.Parse(input) as ConstantExpression;
            result.Value.Should().Be(3);
        }

        [Fact]
        public void DoesNotParseChar()
        {
            var input = "a";
            constantExpressionParser.ConstantParser.Invoking(o => o.Parse(input)).Should().Throw<ParseException>();
        }
    }
}