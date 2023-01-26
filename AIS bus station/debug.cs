using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AIS_bus_station
{
    class debug
    {
        public bool enable = false;
        public void WriteLine(string message = "")
        {
            if (enable)
                Console.WriteLine(message);
        }
        public void Write(string message = "")
        {
            if (enable)
                Console.Write(message);
        }
    }
}
