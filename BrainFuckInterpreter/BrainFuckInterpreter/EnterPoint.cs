using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrainFuckInterpreter
{
    class EnterPoint
    {
        public static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.Priority = System.Threading.ThreadPriority.Lowest;
            var a = new BFInterpreter(args, System.Threading.Thread.CurrentThread);
        }
    }
}
