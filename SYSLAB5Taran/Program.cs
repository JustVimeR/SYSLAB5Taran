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
        Console.WriteLine("Please enter the code:");
        string code = Console.ReadLine();

        List<Token> tokens = Analyze(code);

        foreach (Token token in tokens)
        {
            Console.WriteLine($"< {token.Value}, {token.Type} >");
        }

        // Extract and evaluate expressions
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

        // Build and print AST
        try
        {
            AstNode ast = BuildAst(expression);
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
