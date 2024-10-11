using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static oscae_compiler.AbstractSyntaxTree;

namespace oscae_compiler
{
    static class Interpreter
    {
        public static void Interpret(AbstractSyntaxTree ast)
        {
            Variables variables = new();
            Eval(ast.nodes, variables);
        }

        class Variables
        {
            Dictionary<string, int> variables = [];

            public int this[string variable]
            {
                get
                {
                    if (variables.TryGetValue(variable, out int value)) return value;
                    return 0;
                }
                set
                {
                    if (!variables.TryAdd(variable, value))
                        variables[variable] = value;
                }
            }
        }

        // returns false if exited by a break statement, otherwise true
        static bool Eval(List<AbstractSyntaxTree.StatementNode> nodes, Variables variables)
        {
            foreach (AbstractSyntaxTree.StatementNode _node in nodes)
            {
                switch (_node)
                {
                    case AbstractSyntaxTree.AssignmentNode node:
                        variables[node.Variable] = Eval(node.Value, variables);
                        break;

                    case AbstractSyntaxTree.IfNode node:
                        if (Eval(node.Condition, variables))
                            if (!Eval(node.ThenBranch.Nodes, variables))
                                return false;
                        break;

                    case AbstractSyntaxTree.LoopNode node:
                        while (Eval(node.Body.Nodes, variables));
                        break;

                    case AbstractSyntaxTree.BreakNode node:
                        return false;

                    case AbstractSyntaxTree.PrintNode node:
                        Console.WriteLine(Eval(node.Node, variables));
                        break;
                }
            }

            return true;
        }
        

        static int Eval(AbstractSyntaxTree.ExpressionNode _node, Variables variables)
        {
            switch (_node)
            {
                case AbstractSyntaxTree.IdentifierNode node:
                    return variables[node.Value];
                case AbstractSyntaxTree.NumberNode node:
                    return node.Value;
                case AbstractSyntaxTree.AdditionNode node:
                    return Eval(node.Left, variables) + Eval(node.Right, variables);
                case AbstractSyntaxTree.SubtractionNode node:
                    return Eval(node.Left, variables) - Eval(node.Right, variables);
                case AbstractSyntaxTree.MultiplicationNode node:
                    return Eval(node.Left, variables) * Eval(node.Right, variables);
                case AbstractSyntaxTree.DivisionNode node:
                    return Eval(node.Left, variables) / Eval(node.Right, variables);
                case AbstractSyntaxTree.UnaryNode node:
                    return -Eval(node.Node, variables);
            }
            return 0;
        }

        static bool Eval(AbstractSyntaxTree.BoolOpNode node, Variables variables)
        {
            return node.Op switch
            {
                AbstractSyntaxTree.BoolOpNode.Operator.EQ => Eval(node.Left, variables) == Eval(node.Right, variables),
                AbstractSyntaxTree.BoolOpNode.Operator.NEQ => Eval(node.Left, variables) != Eval(node.Right, variables),
                AbstractSyntaxTree.BoolOpNode.Operator.GT => Eval(node.Left, variables) > Eval(node.Right, variables),
                AbstractSyntaxTree.BoolOpNode.Operator.GTE => Eval(node.Left, variables) >= Eval(node.Right, variables),
                AbstractSyntaxTree.BoolOpNode.Operator.LT => Eval(node.Left, variables) < Eval(node.Right, variables),
                AbstractSyntaxTree.BoolOpNode.Operator.LTE => Eval(node.Left, variables) <= Eval(node.Right, variables),
                _ => false,
            };
        }
    }
}
