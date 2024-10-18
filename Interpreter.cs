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
            readonly Dictionary<string, int> variables = [];

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
        static bool Eval(List<StatementNode> nodes, Variables variables)
        {
            foreach (StatementNode _node in nodes)
            {
                switch (_node)
                {
                    case AssignmentNode node:
                        variables[node.Variable] = Eval(node.Value, variables);
                        break;

                    case IfNode node:
                        if (Eval(node.Condition, variables))
                            if (!Eval(node.ThenBranch.Nodes, variables))
                                return false;
                        break;

                    case LoopNode node:
                        while (Eval(node.Body.Nodes, variables));
                        break;

                    case BreakNode node:
                        return false;

                    case PrintNode node:
                        Console.WriteLine(Eval(node.Node, variables));
                        break;
                }
            }

            return true;
        }
        

        static int Eval(ExpressionNode _node, Variables variables)
        {
            switch (_node)
            {
                case IdentifierNode node:
                    return variables[node.Value];
                case NumberNode node:
                    return node.Value;
                case AdditionNode node:
                    return Eval(node.Left, variables) + Eval(node.Right, variables);
                case SubtractionNode node:
                    return Eval(node.Left, variables) - Eval(node.Right, variables);
                case MultiplicationNode node:
                    return Eval(node.Left, variables) * Eval(node.Right, variables);
                case DivisionNode node:
                    return Eval(node.Left, variables) / Eval(node.Right, variables);
                case UnaryNode node:
                    return -Eval(node.Node, variables);
            }
            return 0;
        }

        static bool Eval(BoolOpNode node, Variables variables)
        {
            return node.Op switch
            {
                BoolOpNode.Operator.EQ => Eval(node.Left, variables) == Eval(node.Right, variables),
                BoolOpNode.Operator.NEQ => Eval(node.Left, variables) != Eval(node.Right, variables),
                BoolOpNode.Operator.GT => Eval(node.Left, variables) > Eval(node.Right, variables),
                BoolOpNode.Operator.GTE => Eval(node.Left, variables) >= Eval(node.Right, variables),
                BoolOpNode.Operator.LT => Eval(node.Left, variables) < Eval(node.Right, variables),
                BoolOpNode.Operator.LTE => Eval(node.Left, variables) <= Eval(node.Right, variables),
                _ => false,
            };
        }
    }
}
