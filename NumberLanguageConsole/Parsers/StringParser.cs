using Sprache;

namespace NumberLanguageConsole {
    public class StringParser {
        public StringParser()
        {
            VariableNameParser = Parse.Letter.AtLeastOnce().Text().Token();
            FileNameParser = Parse.Regex(@"[\\a-z A-Z:.0-9]*").Text().Token();  
        }

        public readonly Parser<string> VariableNameParser;
        public readonly Parser<string> FileNameParser;        
    }
}
