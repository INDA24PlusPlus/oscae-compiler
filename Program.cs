using static oscae_compiler.Token;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace oscae_compiler
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("--------------CODE--------------");


            //string code = File.ReadAllText("..\\..\\..\\Code.txt");
            //string code = File.ReadAllText("..\\..\\..\\Fibonacci.txt");
            string code = File.ReadAllText("..\\..\\..\\MathTest.txt");
            Console.WriteLine(code);


            Console.WriteLine("--------------TOKENS------------");

            List<Token>? tokens = Lexer.Tokenize(code);
            if (tokens == null) return;

            foreach (Token token in tokens)
            {
                Console.Write(token.AsString() + " ");
            }
            Console.WriteLine();

            Console.WriteLine("--------------AST---------------");
            AbstractSyntaxTree? ast = null;

            try
            {
                ast = new(tokens);
            }
            catch (AbstractSyntaxTree.ParserException ex)
            {
                ex.Print();
            }
            ast?.Print();

            Console.WriteLine("--------------EXEC--------------");

            if (ast != null)
            {
                Console.WriteLine("Executing with Interpreter:");
                Interpreter.Interpret(ast);
            }
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
<term> ::= <term> <optional_whitespace> "*" <optional_whitespace> <unary> | <term> <optional_whitespace> "/" <optional_whitespace> <unary> | <unary>
<unary> ::= "-" <optional_whitespace> <factor> | <factor>
<factor> ::= "(" <optional_whitespace> <expr> <optional_whitespace> ")" | <integer> | <identifier>

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

