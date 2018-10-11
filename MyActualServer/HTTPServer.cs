using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Reflection;
namespace MyActualServer
{
    public class HTTPServer
    {
        private HttpListener listener;

        public HTTPServer()
        {
            listener = new HttpListener();
        }
        public void Start()
        {
            //assigns host and port to listen at and starts the listener
            listener.Prefixes.Add("http://localhost:8000/");
            listener.Start();

            //creates a thread for the listener to run on and runs it
            Thread serverThread = new Thread(new ThreadStart(Run));
              serverThread.Start();            
        }
        public void Run()
        {
            using (listener)
            {
                //keeps the listener open until the program is terminated
                while (true)
                {
                    Console.WriteLine("Waiting for connection");
                    HttpListenerContext context = listener.GetContext();
                    Console.WriteLine("Client connected");

                    //creates a new object response classes
                    Response response = new Response();

                    HttpListenerRequest listenerRequest = context.Request;
                    /* Used for troubleshooting during development
                    Console.WriteLine(listenerRequest.HttpMethod);
                    Console.WriteLine(listenerRequest.Url);
                    Console.WriteLine(listenerRequest.Headers);
                    */

                    //provides the body of any post requests sent by the client to the request class
                    Stream body = listenerRequest.InputStream;
                    Encoding encoding = listenerRequest.ContentEncoding;
                    StreamReader reader = new StreamReader(body, encoding);                   
                    string s = reader.ReadToEnd();

                    //Console.WriteLine(s);

                    body.Close();
                    reader.Close();
                    
                    response.responseGenerator(context, listenerRequest.Url.ToString(), listenerRequest.HttpMethod, s);
                }               
            }
        }     
    }
}
