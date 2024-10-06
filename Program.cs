namespace oscae_compiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
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

<segment> ::= <segment> <statement> | <statement>
<statement> ::= <if_statement> | <return_statement> | <assign_statement> | <statement> <whitespace> | <whitespace> <statement>
<if_statement> ::= "if " <expr> "{" <segment> "}"
<return_statement> ::= "return " <expr> ";"
<assign_statement> ::= <identifier> "=" <expr> ";"
<expr> ::= <expr> "+" <term> | <expr> "-" <term> | <term>
<term> ::= <term> "*" <factor> | <term> "/" <factor> | <factor>
<factor> ::= "(" <expr> ")" | "-" <factor> | <integer> | <identifier> | <factor> <whitespace> | <whitespace> <factor>

<identifier> ::= <identifier> <char> | <identifier> <digit> | <identifier> <whitespace> | <char>
<integer> ::= <integer> <digit> | <digit>
<digit> ::= [0-9]
<char> ::= [A-Z] | [a-z] | "_"
<whitespace> ::= " " | "\n"


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

