using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static oscae_compiler.AbstractSyntaxTree;

namespace oscae_compiler
{
    static class Transpiler
    {
        public static void Transpile(AbstractSyntaxTree ast, string outFilePath)
        {
            HashSet<string> vars = [];
            string code = "";

            Transpile(ast.nodes, vars, ref code);

            // program
            string program = "#include <stdio.h>\n\nint main() {\n";
            foreach (string var in vars)
            {
                program += "    int " + var + " = 0;\n";
            }
            program += code + "    return 0;\n}";

            File.WriteAllText(outFilePath, program);
        }

        // returns false if exited by a break statement, otherwise true
        static void Transpile(List<StatementNode> nodes, HashSet<string> vars, ref string code, string indent = "    ")
        {
            foreach (StatementNode _node in nodes)
            {
                code += indent;
                switch (_node)
                {
                    case AssignmentNode node:
                        vars.Add(node.Variable);

                        code += node.Variable + " = ";
                        Transpile(node.Value, vars, ref code);
                        code += ";\n";

                        break;

                    case IfNode node:
                        code += "if (";
                        Transpile(node.Condition.Left, vars, ref code);
                        code += " " + GetSymbol(node.Condition) + " ";
                        Transpile(node.Condition.Right, vars, ref code);
                        code += ") {\n";
                        Transpile(node.ThenBranch.Nodes, vars, ref code, indent + "    ");
                        code += indent + "}\n";
                        break;

                    case LoopNode node:
                        code += "while (1) {\n";
                        Transpile(node.Body.Nodes, vars, ref code, indent + "    ");
                        code += indent + "}";
                        break;

                    case BreakNode node:
                        code += "break;\n";
                        break;

                    case PrintNode node:
                        code += "printf(\"%d\\n\", ";
                        Transpile(node.Node, vars, ref code);
                        code += ");\n";
                        break;
                }

            }
        }

        // transpile expression
        static void Transpile(ExpressionNode _node, HashSet<string> vars, ref string code, ExpressionNode? parentNode = null)
        {
            switch (_node)
            {
                case IdentifierNode node:
                    vars.Add(node.Value);
                    code += node.Value;
                    break;
                case NumberNode node:
                    code += node.Value;
                    break;
                case AdditionNode node:
                    bool addParens = parentNode is MultiplicationNode or DivisionNode or UnaryNode;
                    if (addParens)
                        code += "(";
                    Transpile(node.Left, vars, ref code, node);
                    code += " + ";
                    Transpile(node.Right, vars, ref code, node);
                    if (addParens)
                        code += ")";
                    break;
                case SubtractionNode node:
                    addParens = parentNode is MultiplicationNode or DivisionNode or UnaryNode;
                    if (addParens)
                        code += "(";
                    Transpile(node.Left, vars, ref code, node);
                    code += " - ";
                    Transpile(node.Right, vars, ref code, node);
                    if (addParens)
                        code += ")";
                    break;
                case MultiplicationNode node:
                    addParens = parentNode is UnaryNode;
                    if (!addParens && parentNode is DivisionNode parent)
                        addParens = parent.Right == node;

                    if (addParens)
                        code += "(";
                    Transpile(node.Left, vars, ref code, node);
                    code += " * ";
                    Transpile(node.Right, vars, ref code, node);
                    if (addParens)
                        code += ")";
                    break;
                case DivisionNode node:
                    addParens = parentNode is UnaryNode;
                    if (!addParens && parentNode is DivisionNode parent2)
                        addParens = parent2.Right == node;

                    if (addParens)
                        code += "(";
                    Transpile(node.Left, vars, ref code, node);
                    code += " / ";
                    Transpile(node.Right, vars, ref code, node);
                    if (addParens)
                        code += ")";
                    break;
                case UnaryNode node:
                    addParens = parentNode is UnaryNode;
                    if (addParens)
                        code += "(";
                    code += "-";
                    Transpile(node.Node, vars, ref code, node);
                    if (addParens)
                        code += ")";
                    break;
            }
        }

        static string GetSymbol(BoolOpNode node)
        {
            return node.Op switch
            {
                BoolOpNode.Operator.EQ => "==",
                BoolOpNode.Operator.NEQ => "!=",
                BoolOpNode.Operator.GT => ">",
                BoolOpNode.Operator.GTE => ">=",
                BoolOpNode.Operator.LT => "<",
                BoolOpNode.Operator.LTE => "<=",
                _ => throw new Exception("BoolOpNode.Op not defined!"),
            };
        }
    }
}
