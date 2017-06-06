using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Syntax;
using OMathParser.Syntax.Nodes;
using OMathParser.Syntax.Nodes.Abstract;

namespace OMathPlotter.Util
{
    class ExpressionUtils
    {
        public static bool IsFunctionDefinition(SyntaxTree expression)
        {
            if (expression.RootNode is FunctionApplyNode)
            {
                FunctionApplyNode funcDefinition = expression.RootNode as FunctionApplyNode;
                ArgumentListNode arguments = funcDefinition.Arguments;
                foreach (SyntaxNode argument in arguments)
                {
                    if (!(argument is VariableIdentifierNode))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool CanPlot2D(SyntaxTree expression)
        {
            return IsFunctionDefinition(expression) &&
                (expression.RootNode as FunctionApplyNode).ArgumentsCount == 1;
        }
    }
}
