using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace WebFormsHost.Hosting
{
    public class WebFormsWorker : Worker
    {
        private WebFormsApplicationServer _server;
        private HttpContext _context;

        private TaskCompletionSource<bool> _taskSource;

        public WebFormsWorker(HttpContext context, WebFormsApplicationServer server)
        {
            _context = context;
            _server = server;
        }

        public override bool IsAsync {
            get { return true; }
        }

        public override Task Run(object state)
        {
            _taskSource = new TaskCompletionSource<bool>();
            var task = _taskSource.Task;

            Task.Factory.StartNew(() =>
            {
                try {
                    var host = _server.CreateHost();
                    host.Worker = this;

                    host.ProcessRequest();
                }
                catch(Exception e) {
                    _taskSource.SetException(e);
                }

            });

            return task;
        }

        public override HttpContext GetContext()
        {
            return _context;
        }

        public override void Close()
        {
            if (!_taskSource.Task.IsCanceled && !_taskSource.Task.IsCompleted && !_taskSource.Task.IsFaulted)
                _taskSource.SetResult(true);                
        }

        public override void Flush()
        {
            _context.Response.Body.Flush();
        }

        public override int GetRemainingReuses()
        {
            return 0;
        }

        public override bool IsConnected()
        {
            return (!_context.RequestAborted.IsCancellationRequested);
        }

        public override int Read(byte[] buffer, int position, int size)
        {
            return _context.Request.Body.Read(buffer, position, size);
        }

        public override void SetReuseCount(int reuses)
        {
            base.SetReuseCount(reuses);
        }

        public override void Write(byte[] buffer, int position, int size)
        {
            _context.Response.Body.Write(buffer, position, size);
        }

        public override void WriteHeader(string name, string value)
        {
            _context.Response.Headers.Add(name, new StringValues(value));
        }
    }
}
