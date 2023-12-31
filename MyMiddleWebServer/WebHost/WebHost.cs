﻿using ActionResultExample.Middlewares;
using System.Net;

namespace ActionResultExample.WebHost
{
    public class WebHost
    {
        private readonly int _port;
        private HttpListener _listener;
        private MiddlewareBuilder _middlewareBuilder = new();
        private HttpHandler _handler;

        public WebHost(int port)
        {
            _port = port;
            _listener = new();
            _listener.Prefixes.Add($"http://localhost:{_port}/");
        }

        public void UseStartup<T>() where T : IStartup, new()
        {
            IStartup startup = new T();
            startup.Configure(_middlewareBuilder);
            _handler = _middlewareBuilder.Build();
        }

        public void Run()
        {
            _listener.Start();
            Console.WriteLine($"Server started on {_port}");

            while (true)
            {
                HttpListenerContext context = _listener.GetContext();
                Task.Run(() => HandleRequest(context));
            }
        }

        private void HandleRequest(HttpListenerContext context)
        {
            _handler.Invoke(context);
        }
    }
}
