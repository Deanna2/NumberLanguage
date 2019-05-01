using System.Linq.Expressions;
using System.Collections.Generic;
using Sprache;

namespace NumberLanguageConsole {
    public class KeywordParser {
        public KeywordParser()
        {
            Keywords = new HashSet<string>(){"PRINT", "LENGTH", "READ", "WRITE", "int", "int[]", "if", "endif", "new", "while", "endwhile"};
            PrintParser = Parse.String("PRINT").Text().Token();
            LengthParser = Parse.String("LENGTH").Text().Token();
            ReadParser = Parse.String("READ").Text().Token();
            WriteParser = Parse.String("WRITE").Text().Token();
            IntParser = Parse.String("int").Text().Token();
            IntArrayParser = Parse.String("int[]").Text().Token();
            NewParser = Parse.String("new").Text().Token();
            IfParser = Parse.String("if").Text().Token();
            EndIfParser = Parse.String("endif").Text().Token();
            WhileParser = Parse.String("while").Text().Token();
            EndWhileParser = Parse.String("endwhile").Text().Token();
            NotKeywordParser = Parse.Not(
                IntParser
                .Or(IfParser)
                .Or(EndIfParser)
                .Or(WhileParser)
                .Or(EndWhileParser)
                .Or(NewParser)
                .Or(PrintParser)
                .Or(LengthParser)
                .Or(ReadParser)
                .Or(WriteParser)
            );
        }

        public readonly HashSet<string> Keywords;
        public readonly Parser<string> PrintParser;
        public readonly Parser<string> LengthParser;
        public readonly Parser<string> ReadParser;
        public readonly Parser<string> WriteParser;
        public readonly Parser<string> IntParser;
        public readonly Parser<string> IntArrayParser;
        public readonly Parser<string> NewParser;
        public readonly Parser<string> IfParser;
        public readonly Parser<string> EndIfParser;
        public readonly Parser<string> WhileParser;
        public readonly Parser<string> EndWhileParser;
        public readonly Parser<object> NotKeywordParser;
    }
}