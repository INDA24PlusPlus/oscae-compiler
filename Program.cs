namespace oscae_compiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }


        static List<Token> Lexer(string code)
        {
            List<Token> tokens = new List<Token>();

            string word = "";
            for (int i = 0; i < code.Length; i++)
            {
                char c = code[i];

                if (IsWhitespace(c))
                {
                    Token token = GetToken(word);
                    if (token != Token.None)
                    {
                        tokens.Add(token);

                    }
                }
                else
                {
                    word += c;
                }
            }

            return tokens;
        }

        static Token GetToken(string word)
        {
            switch (word) {
                case "if":
                    return Token.IF_KEYWORD;
                case ""
                default:
                    return Token.None;
            }
        }

        static bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n';
        }

        enum Token
        {
            IDENTIFIER,
            IF_KEYWORD,
            BREAK_KEYWORD,
            LOOP_KEYWORD,
            LPAREN,
            RPAREN,
            LBRACE,
            RBRACE,
            EQUAL,
            GT,
            LT,
            DECIMAL,
            SEMICOLON,
            None
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

<statement> ::= <if_statement> | <break_statement> | <assign_statement> | <loop_statement>

<if_statement> ::= "if" <optional_whitespace> <bool_expr> <optional_whitespace> <block>
<loop_statement> ::= "loop" <optional_whitespace> <block>
<break_statement> ::= "break" <optional_whitespace> ";"
<assign_statement> ::= <identifier> <optional_whitespace> "=" <optional_whitespace> <expr> <optional_whitespace> ";"

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
<bool_op> ::= "==" | "<=" | ">=" | "<" | ">"


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

*/

