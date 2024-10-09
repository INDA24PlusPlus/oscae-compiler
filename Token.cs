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
        public class LParen : Token { }
        public class RParen : Token { }
        public class LBrace : Token { }
        public class RBrace : Token { }
        public class Equal : Token { } // a single equal (assign operation)
        public class Plus : Token { }
        public class Minus : Token { }
        public class Star : Token { }
        public class Slash : Token { }
        public class Semicolon : Token { }

        // boolean operations
        public class EQ : Token { }
        public class NEQ : Token { }
        public class GT : Token { }
        public class GTE : Token { }
        public class LT : Token { }
        public class LTE : Token { }
    }
}
