using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OMathParser.Utils;

namespace OMathParser.Tokens.Abstract
{
    public interface IToken : ISimplifiable
    {
        IToken Parent { get ; set; }
    }
}
