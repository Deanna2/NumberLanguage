using System;
using Xunit;
using NumberLanguageConsole;
using System.Linq.Expressions;
using Sprache;
using FluentAssertions;

namespace NumberLanguageConsole.Tests
{
    public class StringParserTest
    {
        private StringParser StringParser;
        public StringParserTest()
        {
            StringParser = new StringParser();
        }
        [Fact]
        public void VariableNameIsParsed()
        {
            var input = "abc";
            StringParser.VariableNameParser.Parse(input).Should().Be("abc");
        }

        [Fact]
        public void VariableNameWithWhiteSpaceBeforeAndAfterIsParsed()
        {
            var input = "  abc ";
            StringParser.VariableNameParser.Parse(input).Should().Be("abc");
        }

        [Fact]
        public void FileNameIsParsed()
        {
            var fileName = @"C:\Users\Deanna\Documents\Deanna\Programming\VSCode\NumberLanguage\NumberLanguageConsole.Tests\Parsers\String Parser.cs";
            StringParser.FileNameParser.Parse(fileName).Should().Be(fileName);
        }

    }
}