using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib.Exceptions
{
    public class GraphException : Exception
    {
        public GraphException(string message) : base(message)
        {
        }
    }
}
