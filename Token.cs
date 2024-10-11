using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oscae_compiler
{
    public abstract class Token
    {
        //  IDENTIFIER,
        //  NUMBER,
        //  IF_KEYWORD,
        //  LOOP_KEYWORD,
        //  BREAK_KEYWORD,
        //  PRINT_KEYWORD,
        //  LPAREN,
        //  RPAREN,
        //  LBRACE,
        //  RBRACE,
        //  EQUAL,
        //  PLUS
        //  MINUS,
        //  STAR,
        //  SLASH,
        //  SEMICOLON
        //  EQ,
        //  NEQ,
        //  GT,
        //  GTE,
        //  LT,
        //  LTE,

        public virtual string AsString()
        {
            return this.GetType().Name;
        }

        public class Identifier : Token
        {
            public string name;
            public Identifier(string name) => this.name = name;
            public override string AsString()
            {
                return base.AsString() + "(" + name + ")";
            }
        }

        public class Number : Token
        {
            public int number = 0;
            public Number(int number) => this.number = number;
            public override string AsString()
            {
                return base.AsString() + "(" + number + ")";
            }
        }

        
        public class IfKeyword : Token { }
        public class LoopKeyword : Token { }
        public class BreakKeyword : Token { }
        public class PrintKeyword : Token { }

        public abstract class Paren : Token;
        public class LParen : Paren { }
        public class RParen : Paren { }
        public class LBrace : Token { }
        public class RBrace : Token { }

        public abstract class Operator : Token;
        public class Equal : Operator { } // a single equal (assign operation)
        public class Plus : Operator { }
        public class Minus : Operator { }
        public class Star : Operator { }
        public class Slash : Operator { }
        public class Semicolon : Token { }

        // boolean operations

        public abstract class Comparator : Token;
        public class EQ : Comparator { }
        public class NEQ : Comparator { }
        public class GT : Comparator { }
        public class GTE : Comparator { }
        public class LT : Comparator { }
        public class LTE : Comparator { }
    }
}
