using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oscae_compiler
{
    static class Lexer
    {
        public static List<Token>? Tokenize(string code)
        {
            List<Token> tokens = [];
            int i = 0;
            while (i < code.Length)
            {
                char c = code[i];

                if (c == '#')
                {
                    while (i < code.Length)
                    {
                        c = code[i];
                        if (c == '\n')
                            break;
                        i++;
                    }
                }
                else if (IsAlphabetical(c))
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
                    if (!AddToken(tokens, word, i)) return null;
                }
                else if (IsOperator(c))
                {
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
                    if (!AddToken(tokens, word, i)) return null;
                }
                else if (IsNumeric(c))
                {
                    string word = c.ToString();
                    i++;
                    while (i < code.Length)
                    {
                        c = code[i];
                        if (!IsNumeric(c))
                            break;
                        word += c;
                        i++;
                    }
                    if (!AddToken(tokens, word, i)) return null;
                }
                else if (c == ';' || c == '(' || c == ')' || c == '{' || c == '}')
                {
                    if (!AddToken(tokens, c.ToString(), i)) return null;
                    i++;
                }
                else if (IsWhitespace(c))
                {
                    i++;
                }
                else
                {
                    Console.WriteLine("Error at pos: " + i);
                    Console.WriteLine("word: [" + c + "]");
                    return null;
                }
            }

            return tokens;
        }

        static bool AddToken(List<Token> tokens, string word, int pos)
        {
            Token? token = GetToken(word);
            if (token != null)
            {
                tokens.Add(token);
                return true;
            }
            else
            {
                Console.WriteLine("Error at pos: " + pos);
                Console.WriteLine("word: [" + word + "]");
                return false;
            }
        }

        static Token? GetToken(string word)
        {
            if (string.IsNullOrEmpty(word))
                return null;

            switch (word)
            {
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

        // return true if c is 0-9
        static bool IsNumeric(char c)
        {
            return c >= '0' && c <= '9';
        }

        // returns true if c is alphabetical or numeric
        static bool IsAlphanumeric(char c)
        {
            return IsAlphabetical(c) || IsNumeric(c);
        }

        static bool IsOperator(char c)
        {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '=' || c == '!' || c == '<' || c == '>';
        }

        static bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }
    }
}
