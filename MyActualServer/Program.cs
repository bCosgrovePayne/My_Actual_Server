using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace MyActualServer
{
    class Program
    {
        static void Main(string[] args)
        {          
            {
                //starts a server
                HTTPServer server = new HTTPServer();
                server.Start();
            }                           
        }        
    }
}
