using static oscae_compiler.Token;
using System.Collections.Generic;

namespace oscae_compiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--------------CODE--------------");

            string code = "";
            code += "n = 5;\n";
            code += "lastlast = 0;\n";
            code += "last = 1;\n";
            code += "loop\n";
            code += "{\n";
            code += "    n = n - 1;\n";
            code += "    if n < 0\n";
            code += "    {\n";
            code += "        break;\n";
            code += "    }\n";
            code += "    sum = last + lastlast;\n";
            code += "    lastlast = last;\n";
            code += "    \n";
            code += "    last = sum;\n";
            code += "}\n";
            code += "print lastlast;\n";
            Console.WriteLine(code);

            string code2 = "";
            code2 += "n=5;";
            code2 += "lastlast=0;";
            code2 += "last=1;";
            code2 += "loop";
            code2 += "{";
            code2 += "n=n-1;";
            code2 += "if n<0";
            code2 += "{";
            code2 += "break;";
            code2 += "}";
            code2 += "sum=last+lastlast;";
            code2 += "lastlast=last;";
            code2 += "";
            code2 += "last=sum;";
            code2 += "}";
            code2 += "print lastlast;";

            Console.WriteLine("--------------------------------");

            List<Token> tokens = Lexer(code);
            foreach (Token token in tokens)
            {
                Console.Write(token.AsString() + " ");
            }
            Console.WriteLine();

            AbstractSyntaxTree abstractSyntaxTree = new(tokens);
            abstractSyntaxTree.Print();
        }

        static List<Token> Lexer(string code)
        {
            List<Token> tokens = new List<Token>();
            int i = 0;
            while (i < code.Length)
            {
                char c = code[i];
                if (IsAlphabetical(c))
                {
                    string word = c.ToString();
                    i++;
                    while (i < code.Length)
                    {
                        c = code[i];
                        if (!IsAlphanumeric(c))
                            break;
                        word += c;
                        i++;
                    }
                    Token? token = GetToken(word);
                    if (token != null)
                        tokens.Add(token);
                }
                else if (IsOperator(c)) {
                    string word = c.ToString();
                    i++;
                    while (i < code.Length)
                    {
                        c = code[i];

                        if (!IsOperator(c))
                            break;

                        word += c;
                        i++;
                    }
                    Token? token = GetToken(word);
                    if (token != null)
                        tokens.Add(token);
                }
                else
                {
                    Token? token = GetToken(c.ToString());
                    if (token != null)
                        tokens.Add(token);
                    i++;
                }
            }

            return tokens;
        }

        static Token? GetToken(string word)
        {
            if (string.IsNullOrEmpty(word))
                return null;

            switch (word) {
                case "if":
                    return new Token.IfKeyword();
                case "loop":
                    return new Token.LoopKeyword();
                case "break":
                    return new Token.BreakKeyword();
                case "print":
                    return new Token.PrintKeyword();
                case "(":
                    return new Token.LParen();
                case ")":
                    return new Token.RParen();
                case "{":
                    return new Token.LBrace();
                case "}":
                    return new Token.RBrace();
                case "=":
                    return new Token.Equal();
                case "+":
                    return new Token.Plus();
                case "-":
                    return new Token.Minus();
                case "*":
                    return new Token.Star();
                case "/":
                    return new Token.Slash();
                case ";":
                    return new Token.Semicolon();
                case "==":
                    return new Token.EQ();
                case "!=":
                    return new Token.NEQ();
                case ">":
                    return new Token.GT();
                case ">=":
                    return new Token.GTE();
                case "<":
                    return new Token.LT();
                case "<=":
                    return new Token.LTE();
                default:
                    if (int.TryParse(word, out int number))
                        return new Token.Number(number);

                    if (ValidIdentifier(word))
                        return new Token.Identifier(word);

                    return null;
            }
        }

        static bool ValidIdentifier(string identifier)
        {
            if (!IsAlphabetical(identifier[0]))
                return false;
            for (int i = 1; i < identifier.Length; i++)
            {
                if (!IsAlphanumeric(identifier[i]))
                    return false;
            }
            return true;
        }

        // returns true if c is A-Z or a-z or _
        static bool IsAlphabetical(char c)
        {
            return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_';
        }

        // returns true if c is alphabetical or 0-9 or _
        static bool IsAlphanumeric(char c)
        {
            return IsAlphabetical(c) || (c >= '0' && c <= '9');
        }

        static bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '=' || c == '!' || c == '<' || c == '>';
        }

        static bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }

        static int Fib(int n)
        {
            int lastlast = 0;
            int last = 1;
            for (int i = 0; i < n; i++)
            {
                int sum = last + lastlast;

                lastlast = last;
                last = sum;
            }
            return lastlast;
        }
    }
}

// BNF
/*

<program> ::= <segment>
<segment> ::= <segment> <optional_whitespace> <statement> | <statement>

<statement> ::= <if_statement> | <break_statement> | <assign_statement> | <loop_statement> | <print_statement>

<if_statement> ::= "if" <whitespace> <optional_whitespace> <bool_expr> <optional_whitespace> <block>
<loop_statement> ::= "loop" <optional_whitespace> <block>
<break_statement> ::= "break" <optional_whitespace> ";"
<assign_statement> ::= <identifier> <optional_whitespace> "=" <optional_whitespace> <expr> <optional_whitespace> ";"
<print_statement> ::= "print" <whitespace> <optional_whitespace> <expr> <optional_whitespace> ";"

<block> ::= "{" <optional_whitespace> <segment> <optional_whitespace> "}"

<bool_expr> ::= <expr> <optional_whitespace> <bool_op> <optional_whitespace> <expr>
<expr> ::= <expr> <optional_whitespace> "+" <optional_whitespace> <term> | <expr> <optional_whitespace> "-" <optional_whitespace> <term> | <term>
<term> ::= <term> <optional_whitespace> "*" <optional_whitespace> <factor> | <term> <optional_whitespace> "/" <optional_whitespace> <factor> | <factor>
<factor> ::= "(" <optional_whitespace> <expr> <optional_whitespace> ")" | "-" <optional_whitespace> <factor> | <integer> | <identifier>

<identifier> ::= <identifier> <char> | <identifier> <digit> | <char>
<integer> ::= <integer> <digit> | <digit>
<digit> ::= [0-9]
<char> ::= [A-Z] | [a-z] | "_"
<optional_whitespace> ::= (<whitespace>)*
<whitespace> ::= "\t" | "\n" | " "
<bool_op> ::= "==" | "<=" | ">=" | "<" | ">" | "!="


 */

// Goal
/*

n = 5;
lastlast = 0;
last = 1;
loop
{
    n = n - 1;
    if n < 0
    {
        break;    
    }
    sum = last + lastlast;

    lastlast = last;
    last = sum;
}
print lastlast;


// AST

Assign
    n
    5
Assign
    lastlast
    0
Assign
    last
    1
Loop
    Assign
        n
        Subtraction
            n
            1
    If
        Condition
            Opertion
                LT
                n
                0
        Then
            Break
    Assign
        sum
        Addition
            last
            lastlast
    Assign
        lastlast
        last
    Assign
        last
        sum
Print
    lastlast

*/

