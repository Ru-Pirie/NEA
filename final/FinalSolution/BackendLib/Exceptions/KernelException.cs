using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackendLib.Exceptions
{
    internal class KernelException : Exception
    {
        public KernelException(string? message) : base(message)
        {
        }
    }
}