using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sprache;

namespace NumberLanguageConsole {
    public class Parsers {
        private static ConstantExpressionParser ConstantExpressionParser = new ConstantExpressionParser();

        private static Dictionary<string, ParameterExpression> dictionary= new Dictionary<string, ParameterExpression>();
        private static Stack<string> stack = new Stack<string>();
        private static ArrayParser ArrayParser = new ArrayParser(dictionary, stack);
        private static KeywordParser KeywordParser = new KeywordParser();
        private static StringParser StringParser = new StringParser();
        private static VariableParser VariableParser = new VariableParser(dictionary, stack);

        public static Parser<Expression> ArrayLiteralParser = (
            from openingBracket in Parse.Char('[')
            from value in Parse.Ref(() => ValueExpressionParser).Many()
            from closingBracket in Parse.Char(']')
            select makeNewArrayFromExpressions(value)
        ).Token();

        public static NewArrayExpression makeNewArrayOfSize(Expression arraySize)
        {
            return Expression.NewArrayBounds(typeof(int), arraySize);
        }

        public static NewArrayExpression makeNewArrayFromExpressions(IEnumerable<Expression> expressions)
        {
            return Expression.NewArrayInit(typeof(int), expressions);
        }

        // Expression Parsers
        // Variable parsers

        public static Parser<Expression> NewIntArrayParser = (
            from newKeyword in KeywordParser.NewParser
            from intKeyword in KeywordParser.IntParser
            from openingBracket in Parse.Char('[')
            from value in Parse.Ref(() => ValueExpressionParser)
            from closingBracket in Parse.Char(']')
            select makeNewArrayOfSize(value)
        ).Token();

        public static readonly Parser<Expression> MultiVariableParser = ArrayParser.ArrayAccessParser.Or(VariableParser.NewVariableParser).Or(VariableParser.ExistingVariableParser).Token();



        // Binary expression parsers
        public static readonly Parser<BinaryExpression> AddExpressionParser = (
            from left in ConstantExpressionParser.ConstantParser.Or(ArrayParser.ArrayAccessParser).Or(VariableParser.ExistingVariableParser)
            from plus in Parse.Char('+').Token()
            from right in Parse.Ref(() => AddExpressionParser).Or(ConstantExpressionParser.ConstantParser).Or(ArrayParser.ArrayAccessParser).Or(VariableParser.ExistingVariableParser)
            select Expression.Add(left, right)
        );

        // All binary expressions + existing variables + constants. To be used by boolean, and assignment parsers.
        public static readonly Parser<Expression> ValueExpressionParser = Parse.Ref(() => LengthExpressionParser)
        .Or(Parse.Ref(() => ReadExpressionParser))
        .Or(AddExpressionParser)
        .Or(ArrayParser.ArrayAccessParser)
        .Or(VariableParser.ExistingVariableParser)
        .Or(ConstantExpressionParser.ConstantParser);

        public static readonly Parser<Expression> ValueExpressionParserWithArrayInit = ValueExpressionParser
        .Or(NewIntArrayParser)
        .Or(ArrayLiteralParser);

        // Boolean expressions
        public static readonly Parser<BinaryExpression> EqualExpressionParser = (
            from left in ValueExpressionParser
            from doubleEquals in Parse.Char('=').Repeat(2).Token()
            from right in ValueExpressionParser
            select Expression.Equal(left, right)
        );

        public static readonly Parser<BinaryExpression> LessThanExpressionParser = (
            from left in ValueExpressionParser
            from lessThan in Parse.Char('<').Token()
            from right in ValueExpressionParser
            select Expression.LessThan(left, right)
        );

        public static readonly Parser<BinaryExpression> LessThanOrEqualExpressionParser = (
            from left in ValueExpressionParser
            from lessThan in Parse.String("<=").Text().Token()
            from right in ValueExpressionParser
            select Expression.LessThanOrEqual(left, right)
        );

        public static readonly Parser<BinaryExpression> GreaterThanExpressionParser = (
            from left in ValueExpressionParser
            from greaterThan in Parse.Char('>').Token()
            from right in ValueExpressionParser
            select Expression.GreaterThan(left, right)
        );

        public static readonly Parser<BinaryExpression> GreaterThanOrEqualExpressionParser = (
            from left in ValueExpressionParser
            from greaterThan in Parse.String(">=").Text().Token()
            from right in ValueExpressionParser
            select Expression.GreaterThanOrEqual(left, right)
        );

        public static readonly Parser<BinaryExpression> BooleanExpressionsParser = EqualExpressionParser
            .Or(LessThanOrEqualExpressionParser)
            .Or(LessThanExpressionParser)
            .Or(GreaterThanOrEqualExpressionParser)
            .Or(GreaterThanExpressionParser);

        // Assignment parser
        public static readonly Parser<BinaryExpression> AssignmentExpressionParser = (
            from left in MultiVariableParser
            from equal in Parse.Char('=').Token()
            from right in ValueExpressionParserWithArrayInit
            select Expression.Assign(left, right)
        );

        // Method parsers
        public static readonly Parser<Expression> PrintExpressionParser = (
            from print in KeywordParser.PrintParser
            from value in ValueExpressionParser
            select buildPrintCallExpression(value)
        );

        public static readonly Parser<Expression> LengthExpressionParser = (
            from length in KeywordParser.LengthParser
            from value in VariableParser.ExistingVariableParser
            select Expression.ArrayLength(value)
        );

        public static readonly Parser<Expression> ReadExpressionParser = (
            from read in KeywordParser.ReadParser
            from openingQuote in Parse.Char('"')
            from value in StringParser.FileNameParser
            from closingQuote in Parse.Char('"')
            select buildReadCallExpression(Expression.Constant(value))
        );

        public static readonly Parser<Expression> WriteExpressionParser = (
            from write in KeywordParser.WriteParser
            from openingQuote in Parse.Char('"')
            from filePath in StringParser.FileNameParser
            from closingQuote in Parse.Char('"').Token()
            from value in VariableParser.ExistingVariableParser.Or(ArrayLiteralParser).Or(ReadExpressionParser)
            select buildWriteCallExpression(Expression.Constant(filePath), value)
        );

        public static MethodCallExpression buildPrintCallExpression(Expression value)
        {        
            if (value.Type == typeof(int[]))
            {
                var printIntArray = typeof(Utilities).GetMethod("PrintIntArray");
                return Expression.Call(printIntArray, value);
            } else {
            
                Type[] consoleWriteLineArgTypes = { typeof(int) };
                var consoleWriteLine = typeof(Console).GetMethod("WriteLine", consoleWriteLineArgTypes);
                return Expression.Call(consoleWriteLine, value);
            }
        }

        
        public static MethodCallExpression buildReadCallExpression(Expression value)
        {
            var readFromFile = typeof(Utilities).GetMethod("ReadFromFile");
            return Expression.Call(readFromFile, value);
        }

        public static MethodCallExpression buildWriteCallExpression(Expression filePath, Expression value)
        {
            var writeToFile = typeof(Utilities).GetMethod("WriteToFile");
            return Expression.Call(writeToFile, filePath, value);
        }

        // Control flow parsers
        public static Parser<ConditionalExpression> IfExpressionParser = (
            from ifKeyword in KeywordParser.IfParser
            from condition in BooleanExpressionsParser
            from then in Parse.Ref(() => BlockParserHelper())
            from endIfKeyword in KeywordParser.EndIfParser
            select Expression.IfThen(condition, then)
        );

        public static Parser<Expression> WhileExpressionParser = (
            from whileKeyword in KeywordParser.WhileParser
            from condition in BooleanExpressionsParser
            from loop in Parse.Ref(() => BlockParserHelper())
            from endWhileKeyword in KeywordParser.EndWhileParser
            select buildWhileExpression(condition, loop)
        );

        public static LoopExpression buildWhileExpression(BinaryExpression whileCondition, Expression loop)
        {
            LabelTarget label = Expression.Label();
            return Expression.Loop(Expression.IfThenElse(whileCondition, loop, Expression.Break(label)), label);
        }

        // Block parsers
        public static readonly Parser<IEnumerable<Expression>> StatementsParser = WhileExpressionParser.Or(IfExpressionParser).Or(PrintExpressionParser).Or(WriteExpressionParser).Or(AssignmentExpressionParser).Many();

        public static readonly Parser<BlockExpression> BlockParser = (
            from statements in StatementsParser
            select buildBlockExpression(statements)
        );

        public static Parser<BlockExpression> BlockParserHelper()
        {
            VariableParser.VariableScopeStack.Push("{");
            return BlockParser;
        }

        public static BlockExpression buildBlockExpression(IEnumerable<Expression> expressisons)
        {
            var parametersEnumerable = new List<ParameterExpression>();
            while (VariableParser.VariableScopeStack.Count != 0)
            {
                var value = VariableParser.VariableScopeStack.Pop();
                if (value == "{")
                {
                    break;
                }
                parametersEnumerable.Add(VariableParser.VariableDictionary[value]);
                VariableParser.VariableDictionary.Remove(value);
            }
            return Expression.Block(parametersEnumerable, expressisons.ToArray());
        }
    }
}