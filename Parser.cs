using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oscae_compiler
{
    static class Parser
    {
        public static AbstractSyntaxTree Parse(List<Token> tokens)
        {
            return new AbstractSyntaxTree(tokens);
        }
    }
}
