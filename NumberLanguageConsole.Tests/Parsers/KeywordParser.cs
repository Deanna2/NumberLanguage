using System;
using Xunit;
using NumberLanguageConsole;
using System.Linq.Expressions;
using Sprache;
using FluentAssertions;

namespace NumberLanguageConsole.Tests
{
    public class KeywordParserTest
    {
        private KeywordParser KeywordParser;
        public KeywordParserTest()
        {
            KeywordParser = new KeywordParser();
        }

        [Fact]
        public void NotKeywordParserThrowsExceptionsForAllKeywords()
        {
            foreach (var key in KeywordParser.Keywords) {
                KeywordParser.NotKeywordParser.Invoking(o => o.Parse(key)).Should().Throw<ParseException>();
            }
        }

        [Fact]
        public void NotKeywordParserReturnsNullForNonKeyword()
        {
            var input = "a";
            KeywordParser.NotKeywordParser.Parse(input).Should().Be(null);
        }
    }
}