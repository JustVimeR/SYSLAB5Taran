//using System;
//using System.Collections.Generic;
//using System.Text.RegularExpressions;
//using System.IO;

//class LexicalAnalyzer
//{
//    static void Main(string[] args)
//    {
//        string filePath = "C:\\Users\\dog79\\source\\repos\\SYSLAB5Taran\\SYSLAB5Taran\\index.txt";
//        string code;

//        try
//        {
//            code = File.ReadAllText(filePath);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error reading file: {ex.Message}");
//            return;
//        }

//        List<Token> tokens = Analyze(code);

//        foreach (Token token in tokens)
//        {
//            Console.WriteLine($"< {token.Value}, {token.Type} >");
//        }


//    }



//    public enum TokenType
//    {
//        Number,
//        String,
//        Operator,
//        Identifier,
//        Directive,
//        Comment,
//        Reserved,
//        Delimiter,
//        Error
//    }

//    public class Token
//    {
//        public string Value { get; }
//        public TokenType Type { get; }

//        public Token(string value, TokenType type)
//        {
//            Value = value;
//            Type = type;
//        }
//    }

//    public static List<Token> Analyze(string code)
//    {
//        List<Token> tokens = new List<Token>();
//        HashSet<string> declaredVariables = new HashSet<string>();
//        bool expectingIdentifierOrString = false;
//        bool isInString = false;
//        bool isInDirective = false;
//        string currentString = "";

//        string directivePattern = @"^#(\w+)";
//        string commentPattern = @"//.*";
//        string reservedWordsPattern = @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|using\sstatic|virtual|void|volatile|while)\b";
//        string delimiterPattern = @"[,.;()\[\]{}]";

//        string[] lines = code.Split('\n');

//        foreach (string line in lines)
//        {
//            bool isComment = false;
//            string[] words = Regex.Split(line, @"(\s+|" + delimiterPattern + @")");

//            for (int i = 0; i < words.Length; i++)
//            {
//                string word = words[i];
//                if (string.IsNullOrWhiteSpace(word))
//                {
//                    continue;
//                }

//                if (isInDirective)
//                {
//                    tokens.Add(new Token(word, TokenType.Directive));
//                    isInDirective = false;
//                    continue;
//                }

//                if (word.StartsWith("#"))
//                {
//                    isInDirective = true;
//                    tokens.Add(new Token(word, TokenType.Directive));
//                    continue;
//                }

//                if (isInString)
//                {
//                    currentString += word;
//                    if (word.EndsWith("\""))
//                    {
//                        tokens.Add(new Token(currentString, TokenType.String));
//                        isInString = false;
//                        currentString = "";
//                    }
//                    continue;
//                }

//                if (word.StartsWith("\""))
//                {
//                    isInString = true;
//                    currentString = word;
//                    if (word.EndsWith("\""))
//                    {
//                        tokens.Add(new Token(currentString, TokenType.String));
//                        isInString = false;
//                        currentString = "";
//                    }
//                    continue;
//                }

//                if (isComment)
//                {
//                    tokens.Add(new Token(word, TokenType.Comment));
//                    continue;
//                }

//                TokenType type = TokenType.Error;

//                if (Regex.IsMatch(word, directivePattern))
//                {
//                    type = TokenType.Directive;
//                }
//                else if (Regex.IsMatch(word, commentPattern))
//                {
//                    type = TokenType.Comment;
//                    isComment = true;
//                }
//                else if (Regex.IsMatch(word, delimiterPattern))
//                {
//                    type = TokenType.Delimiter;
//                }
//                else if (IsNumber(word))
//                {
//                    type = TokenType.Number;
//                }
//                else if (IsOperator(word))
//                {
//                    type = TokenType.Operator;
//                }
//                else if (expectingIdentifierOrString)
//                {
//                    if (IsIdentifier(word))
//                    {
//                        declaredVariables.Add(word);
//                        type = TokenType.Identifier;
//                    }
//                    else
//                    {
//                        Console.WriteLine($"Error: Expected identifier or string after type, found {word}");
//                        type = TokenType.Error;
//                    }
//                    expectingIdentifierOrString = false;
//                }
//                else if (Regex.IsMatch(word, reservedWordsPattern))
//                {
//                    type = TokenType.Reserved;
//                    expectingIdentifierOrString = true;
//                }
//                else if (IsIdentifier(word))
//                {
//                    if (declaredVariables.Contains(word))
//                    {
//                        type = TokenType.Identifier;
//                    }
//                    else
//                    {
//                        type = TokenType.Error;
//                        Console.WriteLine($"Error: Undeclared variable {word}");
//                    }
//                }

//                if (!isInString)
//                {
//                    tokens.Add(new Token(word, type));
//                }
//            }
//        }

//        return tokens;
//    }

//    private static bool IsNumber(string word)
//    {
//        return Regex.IsMatch(word, @"^\d+(\.\d+)?(f|F)?|0[xX][0-9a-fA-F]+|0[oO][0-7]+|0[bB][01]+$");
//    }

//    private static bool IsOperator(string word)
//    {
//        return Regex.IsMatch(word, @"^(\+|-|\*|/|%|=|<|>|!|&|\||\(|\)|{|}|,|\[|\]|\.|\;)$");
//    }

//    private static bool IsIdentifier(string word)
//    {
//        return Regex.IsMatch(word, @"^[a-zA-Z_]\w*$");
//    }
//}


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Linq;

class LexicalAnalyzer
{
    static void Main(string[] args)
    {
        string filePath = "C:\\Users\\dog79\\source\\repos\\SYSLAB5Taran\\SYSLAB5Taran\\index.txt"; 
        string code;

        try
        {
            code = File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return;
        }

        List<Token> tokens = Analyze(code);

        foreach (Token token in tokens)
        {
            Console.WriteLine($"< {token.Value}, {token.Type} >");
        }

        // Обчислення виразів
        string expression = ExtractExpression(tokens);
        if (!string.IsNullOrWhiteSpace(expression))
        {
            try
            {
                double result = EvaluateExpression(expression);
                Console.WriteLine($"Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in evaluating expression: {ex.Message}");
            }
        }


        try
        {
            AstNode ast = BuildAst(expression);
            // Візуалізація AST
            ast.Print();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in building AST: {ex.Message}");
        }
    }



    public enum TokenType
    {
        Number,
        Operator,
        Delimiter,
        Unknown
    }

    public class Token
    {
        public string Value { get; }
        public TokenType Type { get; }

        public Token(string value, TokenType type)
        {
            Value = value;
            Type = type;
        }
    }

    public static List<Token> Analyze(string code)
    {
        List<Token> tokens = new List<Token>();
        string[] elements = Regex.Split(code, @"(\s+|[^0-9a-zA-Z\.])").Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();

        foreach (string element in elements)
        {
            TokenType type = GetTokenType(element);
            tokens.Add(new Token(element, type));
        }

        return tokens;
    }

    

    private static TokenType GetTokenType(string element)
    {
        if (Regex.IsMatch(element, @"^\d+(\.\d+)?$"))
            return TokenType.Number;
        if (Regex.IsMatch(element, @"[\+\-\*/%\(\)]"))
            return TokenType.Operator;
        if (Regex.IsMatch(element, @"[;]"))
            return TokenType.Delimiter;

        return TokenType.Unknown;
    }

    

    private static string ExtractExpression(List<Token> tokens)
    {
        string expression = "";
        foreach (var token in tokens)
        {
            if (token.Type != TokenType.Delimiter)
            {
                expression += token.Value;
            }
        }
        return expression;
    }

    private static double EvaluateExpression(string expression)
    {
        DataTable table = new DataTable();
        string processedExpression = Regex.Replace(expression, @"(?<!\.\d*)\b\d+\b", match => match.Value + ".0");
        return Convert.ToDouble(table.Compute(processedExpression, String.Empty));
    }

    abstract class AstNode
    {
        public abstract void Print(string indent = "", bool last = true);
    }

    class OperatorNode : AstNode
    {
        public string Operator { get; set; }
        public AstNode Left { get; set; }
        public AstNode Right { get; set; }

        public OperatorNode(string op, AstNode left, AstNode right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public override void Print(string indent = "", bool last = true)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("└─");
                indent += "  ";
            }
            else
            {
                Console.Write("├─");
                indent += "| ";
            }
            Console.WriteLine(Operator);

            Left?.Print(indent, false);
            Right?.Print(indent, true);
        }
    }

    class NumberNode : AstNode
    {
        public double Value { get; set; }

        public NumberNode(double value)
        {
            Value = value;
        }

        public override void Print(string indent = "", bool last = true)
        {
            Console.Write(indent);
            if (last)
            {
                Console.Write("└─");
                indent += "  ";
            }
            else
            {
                Console.Write("├─");
                indent += "| ";
            }
            Console.WriteLine(Value);
        }
    }

    private static AstNode BuildAst(string expression)
    {
        var tokens = Regex.Split(expression, @"([*()\^\/%+\-])")
                          .Where(token => !string.IsNullOrWhiteSpace(token))
                          .Select(token => token.Trim())
                          .ToList();

        var outputQueue = new Queue<string>();
        var operatorStack = new Stack<string>();

        foreach (var token in tokens)
        {
            if (double.TryParse(token, out _))
            {
                outputQueue.Enqueue(token);
            }
            else if ("+-*/%".Contains(token))
            {
                while (operatorStack.Count > 0 && GetPrecedence(operatorStack.Peek()) >= GetPrecedence(token))
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                operatorStack.Push(token);
            }
            else if (token == "(")
            {
                operatorStack.Push(token);
            }
            else if (token == ")")
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                {
                    outputQueue.Enqueue(operatorStack.Pop());
                }
                if (operatorStack.Count > 0 && operatorStack.Peek() == "(")
                {
                    operatorStack.Pop(); 
                }
                else
                {
                    throw new ArgumentException("Unbalanced parentheses");
                }
            }
        }

        while (operatorStack.Count > 0)
        {
            if (operatorStack.Peek() == "(" || operatorStack.Peek() == ")")
            {
                throw new ArgumentException("Unbalanced parentheses");
            }
            outputQueue.Enqueue(operatorStack.Pop());
        }

        // Побудова AST з постфіксної форми
        var astStack = new Stack<AstNode>();
        while (outputQueue.Count > 0)
        {
            var token = outputQueue.Dequeue();
            if (double.TryParse(token, out double value))
            {
                astStack.Push(new NumberNode(value));
            }
            else
            {
                var rightNode = astStack.Pop();
                var leftNode = astStack.Pop();
                astStack.Push(new OperatorNode(token, leftNode, rightNode));
            }
        }

        return astStack.Pop();
    }

    private static int GetPrecedence(string op)
    {
        switch (op)
        {
            case "+":
            case "-":
                return 1;
            case "*":
            case "/":
            case "%":
                return 2;
            case "(":
                return 0;
            default:
                throw new ArgumentException($"Unknown operator: {op}");
        }
    }


}
