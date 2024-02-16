using System;
using System.IO;
using System.Net;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            string prefix = "http://localhost:8000/";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(prefix);
            listener.Start();
            Console.WriteLine("Listening...");
            StreamWriter file = new StreamWriter("LogFile.txt");
            file.Write("");
            file.Close();
            while (true)
            {
                Listener(listener);
            }
        }

        static void Listener(HttpListener listener)
        {
            HttpListenerContext context = listener.GetContext();
            HttpListenerRequest request = context.Request;
            string str = request.Url.LocalPath;
            if (str == "/index")
                str = "CSS Zen Garden_ The Beauty of CSS Design.html";
            else
                str = str.Remove(0, 1).Replace('/', '\\');
            Console.WriteLine(str);
            if (File.Exists(str))
            {
                if (str.Split('.')[1] == "png")
                {
                    byte[] buffer = File.ReadAllBytes(str);
                    HttpListenerResponse response = context.Response;
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                }
                else
                {
                    StreamReader sr = new StreamReader(str);
                    string responseString = "";
                    string line = sr.ReadLine();
                    while (line != null)
                    {
                        line = sr.ReadLine();
                        responseString += line;
                    }
                    sr.Close();
                    byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);

                    HttpListenerResponse response = context.Response;
                    response.ContentLength64 = buffer.Length;
                    System.IO.Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                    File.AppendAllText("LogFile.txt", DateTime.Now.ToString() + "; " + request.RemoteEndPoint.ToString() + "; " + request.Url.LocalPath + "; " + response.StatusCode + "\n");
                }
            }
            else
            {
                HttpListenerResponse response = context.Response;
                response.StatusCode = 404;
                System.IO.Stream output = response.OutputStream;
                output.Close();
                File.AppendAllText("LogFile.txt", DateTime.Now.ToString() + "; " + request.RemoteEndPoint.ToString() + "; " + request.Url.LocalPath + "; " + response.StatusCode + "\n");
            }
        }
    }
}
