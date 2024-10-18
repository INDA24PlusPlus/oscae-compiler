using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace oscae_compiler
{
    public class AbstractSyntaxTree
    {
        public List<StatementNode> nodes = [];

        public AbstractSyntaxTree(List<Token> tokens)
        {
            int pos = 0;

            // Segment
            while (pos < tokens.Count)
            {
                nodes.Add(ParseStatement(tokens, ref pos));
            }
        }

        public void Print()
        {
            foreach (Node node in nodes)
            {
                node.Print();
            }
        }

        static BlockNode ParseBlock(List<Token> tokens, ref int pos)
        {
            List<StatementNode> nodes = [];

            if (pos >= tokens.Count || tokens[pos] is not Token.LBrace)
                throw new ParserException("Expected '{' at token no: " + pos);

            pos++;

            if (pos >= tokens.Count)
                throw new ParserException("Expected statement or '}' at token no: " + pos);

            while (tokens[pos] is not Token.RBrace)
            {
                nodes.Add(ParseStatement(tokens, ref pos));
                if (pos >= tokens.Count)
                    throw new ParserException("Expected statement or '}' at token no: " + pos);
            }

            if (pos >= tokens.Count || tokens[pos] is not Token.RBrace)
                throw new ParserException("Expected '}' at token no: " + pos);

            pos++;
            return new BlockNode(nodes);

            //int depth = 1;
            //while (depth > 0)
            //{
            //    if (tokens[pos] is Token.LBrace)
            //    {
            //        depth++;
            //        pos++;
            //    }
            //    else if (tokens[pos] is Token.RBrace)
            //    {
            //        depth--;
            //        pos++;
            //    }
            //    else
            //    {

            //    }
            //}

        }

        static StatementNode ParseStatement(List<Token> tokens, ref int pos)
        {
            // <statement> ::= <if_statement> | <break_statement> | <assign_statement> | <loop_statement> | <print_statement>

            if (tokens.Count <= pos)
                throw new ParserException("Excpected statement at token no: " + pos);

            StatementNode? node = null;
            if (tokens[pos] is Token.Identifier identifier) // Assign statement
            {
                pos++;
                if (pos >= tokens.Count || tokens[pos] is not Token.Equal)
                    throw new ParserException("Expected '=' at token no: " + pos);

                pos++;
                node = new AssignmentNode(identifier.name, ParseExpression(tokens, ref pos));

                if (pos >= tokens.Count || tokens[pos] is not Token.Semicolon) // expecting ;
                    throw new ParserException("Expected ';' at token no: " + pos);
                pos += 1;
            }
            else if (tokens[pos] is Token.IfKeyword)        // If statement
            {
                pos += 1;
                node = new IfNode(ParseCondition(tokens, ref pos), ParseBlock(tokens, ref pos));
            }
            else if (tokens[pos] is Token.LoopKeyword)      // Loop statement
            {
                pos += 1;
                node = new LoopNode(ParseBlock(tokens, ref pos));
            }
            else if (tokens[pos] is Token.BreakKeyword)     // Break statement
            {
                pos += 1;
                node = new BreakNode();

                if (pos >= tokens.Count || tokens[pos] is not Token.Semicolon) // expecting ;
                    throw new ParserException("Expected ';' at token no: " + pos);
                pos += 1;
            }
            else if (tokens[pos] is Token.PrintKeyword)     // Print statement
            {
                pos += 1;
                node = new PrintNode(ParseExpression(tokens, ref pos));

                if (pos >= tokens.Count || tokens[pos] is not Token.Semicolon) // expecting ;
                    throw new ParserException("Expected ';' at token no: " + pos);
                pos += 1;
            }

            if (node == null)
                throw new ParserException("Expected statement at token no " + pos);

            return node;
        }

        static ExpressionNode ParseExpression(List<Token> tokens, ref int pos)
        {
            List<Token> expression = [];

            while (pos < tokens.Count && (tokens[pos] is Token.Identifier || tokens[pos] is Token.Number ||
                tokens[pos] is Token.Plus || tokens[pos] is Token.Minus || tokens[pos] is Token.Star ||
                tokens[pos] is Token.Slash || tokens[pos] is Token.LParen || tokens[pos] is Token.RParen))
            {
                if (expression.Count == 0 || expression[^1] is Token.Operator or Unary or Token.LParen) // Unary is not Token.Operator
                {
                    // only allow identifiers, numbers, lparen and unary
                    if (tokens[pos] is Token.Minus)
                    {
                        if (expression[^1] is Unary) // A unary is not allowed after a unary
                            throw new ParserException("Expected Identifier, Number or '(' at token no: " + pos);
                        expression.Add(new Unary());
                    }
                    else if (tokens[pos] is Token.Identifier or Token.Number or Token.LParen)
                        expression.Add(tokens[pos]);
                    else
                        throw new ParserException("Expected Identifier, Number, '(' or Unary operator at token no: " + pos);
                }
                else
                {
                    // only allow operators (not unary) and rparen
                    if (tokens[pos] is Token.Operator or Token.RParen)
                        expression.Add(tokens[pos]);
                    else
                        throw new ParserException("Expected Operator or ')' at token no: " + pos);
                }
                pos++;
            }

            List<ExpressionNode> stack = [];
            foreach (Token token in ShuntingYard(expression))
            {
                switch (token)
                {
                    case Token.Number number:
                        stack.Add(new NumberNode(number.number));
                        break;
                    case Token.Identifier identifier:
                        stack.Add(new IdentifierNode(identifier.name));
                        break;
                    case Token.Plus:
                        ExpressionNode right = Pop(stack);
                        ExpressionNode left = Pop(stack);
                        stack.Add(new AdditionNode(left, right));
                        break;
                    case Token.Minus:
                        right = Pop(stack);
                        left = Pop(stack);
                        stack.Add(new SubtractionNode(left, right));
                        break;
                    case Token.Star:
                        right = Pop(stack);
                        left = Pop(stack);
                        stack.Add(new MultiplicationNode(left, right));
                        break;
                    case Token.Slash:
                        right = Pop(stack);
                        left = Pop(stack);
                        stack.Add(new DivisionNode(left, right));
                        break;
                    case Unary:
                        stack.Add(new UnaryNode(Pop(stack)));
                        break;
                }
            }
            if (stack.Count != 1)
                throw new ParserException("Expected expression at token pos no " + pos);
            return stack[0];
        }

        static List<Token> ShuntingYard(List<Token> tokens)
        {
            List<Token> stack = [];
            List<Token> result = [];

            foreach (Token token in tokens)
            {
                switch (token)
                {
                    case Token.Identifier:
                    case Token.Number:
                        result.Add(token);
                        break;

                    case Token.LParen:
                        stack.Add(token);
                        break;

                    case Token.RParen:
                        while (true)
                        {
                            Token popped = Pop(stack);
                            if (popped is Token.LParen)
                                break;
                            else
                                result.Add(popped);
                        }
                        break;
                    // left-associative
                    case Token.Star:
                    case Token.Slash:
                    case Token.Plus:
                    case Token.Minus:
                        if (stack.Count == 0 || stack[^1] is Token.LParen)
                            stack.Add(token);
                        else
                        {
                            int incomingPrecedance = Precedance(token);
                            while (stack.Count > 0 && incomingPrecedance <= Precedance(stack[^1]))
                            {
                                result.Add(Pop(stack));
                            }
                            stack.Add(token);
                        }
                        break;
                    // right-associative
                    case Unary:
                        if (stack.Count == 0 || stack[^1] is Token.LParen)
                            stack.Add(token);
                        else
                        {
                            int incomingPrecedance = Precedance(token);
                            while (stack.Count > 0 && incomingPrecedance < Precedance(stack[^1]))
                            {
                                result.Add(Pop(stack));
                            }
                            stack.Add(token);
                        }
                        break;
                }
            }

            // pop stack to result
            while (stack.Count > 0)
            {
                result.Add(Pop(stack));
            }

            return result;
        }

        static int Precedance(Token token)
        {
            switch (token)
            {
                case Token.Plus:
                case Token.Minus:
                    return 1;
                case Token.Star:
                case Token.Slash:
                    return 2;
                case Unary:
                    return 3;
                default:
                    return 0;
            }
        }

        static T Pop<T>(List<T> list)
        {
            T popped = list[^1]; // last item in stack
            list.Remove(popped);
            return popped;
        }

        static BoolOpNode ParseCondition(List<Token> tokens, ref int pos)
        {
            ExpressionNode left = ParseExpression(tokens, ref pos);
            if (tokens.Count <= pos)
                throw new ParserException("Expected comparison at token no " + pos);

            var op = tokens[pos] switch
            {
                Token.EQ => BoolOpNode.Operator.EQ,
                Token.NEQ => BoolOpNode.Operator.NEQ,
                Token.GT => BoolOpNode.Operator.GT,
                Token.GTE => BoolOpNode.Operator.GTE,
                Token.LT => BoolOpNode.Operator.LT,
                Token.LTE => BoolOpNode.Operator.LTE,
                _ => throw new ParserException("Expected a comparative token at token pos no: " + pos),
            };
            pos++;
            ExpressionNode right = ParseExpression(tokens, ref pos);

            return new BoolOpNode(left, op, right);
        }

        public abstract class Node
        {
            public abstract void Print(string indent = "");
        }

        // statement nodes
        public abstract class StatementNode : Node;
        public class AssignmentNode : StatementNode
        {
            public string Variable { get; }
            public ExpressionNode Value { get; }

            public AssignmentNode(string variable, ExpressionNode value)
            {
                Variable = variable;
                Value = value;
            }

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "Assign: " + Variable + " =");
                Value.Print(indent + "  ");
            }
        }

        public class IfNode : StatementNode
        {
            public BoolOpNode Condition { get; }
            public BlockNode ThenBranch { get; }

            public IfNode(BoolOpNode condition, BlockNode thenBranch)
            {
                Condition = condition;
                ThenBranch = thenBranch;
            }

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "If Statement:");
                Console.WriteLine(indent + "  Condition:");
                Condition.Print(indent + "    ");
                Console.WriteLine(indent + "  Then:");
                ThenBranch.Print(indent + "    ");
            }
        }

        public class LoopNode : StatementNode
        {
            public BlockNode Body { get; }
            public LoopNode(BlockNode body) => Body = body;

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "Loop:");
                Body.Print(indent + "  ");
            }
        }

        public class BlockNode : Node
        {
            public List<StatementNode> Nodes;

            public BlockNode(List<StatementNode> nodes)
            {
                Nodes = nodes;
            }

            public override void Print(string indent = "")
            {
                foreach (Node node in Nodes)
                {
                    node.Print(indent);
                }
            }
        }

        public class BreakNode : StatementNode
        {
            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "Break");
            }
        }

        public class PrintNode : StatementNode
        {
            public ExpressionNode Node { get; }
            public PrintNode(ExpressionNode node)
            {
                Node = node;
            }

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "Print:");
                Node.Print(indent + "  ");
            }
        }

        // expression nodes
        public abstract class ExpressionNode : Node;
        public class AdditionNode : ExpressionNode
        {
            public ExpressionNode Left { get; }
            public ExpressionNode Right { get; }
            public AdditionNode(ExpressionNode left, ExpressionNode right)
            {
                Left = left;
                Right = right;
            }

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "Add:");
                Left.Print(indent + "  ");
                Right.Print(indent + "  ");
            }
        }

        public class SubtractionNode : ExpressionNode
        {
            public ExpressionNode Left { get; }
            public ExpressionNode Right { get; }
            public SubtractionNode(ExpressionNode left, ExpressionNode right)
            {
                Left = left;
                Right = right;
            }

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "Subtract:");
                Left.Print(indent + "  ");
                Right.Print(indent + "  ");
            }
        }

        public class MultiplicationNode : ExpressionNode
        {
            public ExpressionNode Left { get; }
            public ExpressionNode Right { get; }
            public MultiplicationNode(ExpressionNode left, ExpressionNode right)
            {
                Left = left;
                Right = right;
            }

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "Multiply:");
                Left.Print(indent + "  ");
                Right.Print(indent + "  ");
            }
        }

        public class DivisionNode : ExpressionNode
        {
            public ExpressionNode Left { get; }
            public ExpressionNode Right { get; }
            public DivisionNode(ExpressionNode left, ExpressionNode right)
            {
                Left = left;
                Right = right;
            }

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "Divide:");
                Left.Print(indent + "  ");
                Right.Print(indent + "  ");
            }
        }

        public class UnaryNode : ExpressionNode
        {
            public ExpressionNode Node { get; }
            public UnaryNode(ExpressionNode node)
            {
                Node = node;
            }

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "Unary:");
                Node.Print(indent + "  ");
            }
        }

        // primitive nodes

        public class NumberNode : ExpressionNode
        {
            public int Value { get; }
            public NumberNode(int value) => Value = value;

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + Value);
            }
        }

        public class IdentifierNode : ExpressionNode
        {
            public string Value { get; }
            public IdentifierNode(string value) => Value = value;

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + Value);
            }
        }

        public class BoolOpNode : Node
        {
            public Operator Op { get; }
            public ExpressionNode Left { get; }
            public ExpressionNode Right { get; }
            public BoolOpNode(ExpressionNode left, Operator op, ExpressionNode right)
            {
                Left = left;
                Op = op;
                Right = right;
            }

            public override void Print(string indent = "")
            {
                Console.WriteLine(indent + "Bool operation: " + Op.ToString());
                Left.Print(indent + "  ");
                Right.Print(indent + "  ");
            }

            public enum Operator
            {
                EQ,
                NEQ,
                GT,
                GTE,
                LT,
                LTE,
            }
        }

        public class ParserException : Exception
        {
            string ex = "";
            public ParserException(string ex = "Parser exception")
            {
                this.ex = ex;
            }

            public void Print()
            {
                Console.WriteLine(ex);
            }
        }

        class Unary : Token { }
    }
}
