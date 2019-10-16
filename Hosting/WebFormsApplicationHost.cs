using System;
using System.Threading;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.Hosting;

namespace WebFormsHost.Hosting
{
    public class WebFormsApplicationHost : MarshalByRefObject
    {
        private string _path;
        private string _vPath;

        private readonly EndOfRequestHandler EndRequest;

        public WebFormsApplicationHost()
        {
            EndRequest = OnEndRequest;
            AppDomain.CurrentDomain.DomainUnload += OnUnload;
        }

        public WebFormsApplicationServer Server { get; set; }

        public WebFormsWorker Worker { get; set; }

        public void OnEndRequest(WebFormsWorkerRequest wr) {
            try {
                wr.CloseConnection();
            }
            catch{}
        }

        public void OnUnload(object sender, EventArgs e) {
            //do nothing - for now
        }

        public string Path
        {
            get
            {
                if (_path == null)
                    _path = AppDomain.CurrentDomain.GetData(".appPath").ToString();

                return _path;
            }
        }

        public string VPath
        {
            get
            {
                if (_vPath == null)
                    _vPath = AppDomain.CurrentDomain.GetData(".appVPath").ToString();

                return _vPath;
            }
        }

        public AppDomain Domain
        {
            get { return AppDomain.CurrentDomain; }
        }

        public void ProcessRequest() {
            var wr = new WebFormsWorkerRequest(Worker, this);
            wr.EndRequest += OnEndRequest;

            try {
                wr.ProcessRequest();
            }
            catch(ThreadAbortException) {
                Thread.ResetAbort();
            }
            catch(Exception e) {
                //Log something here...
                OnEndRequest(wr);
            }
        }

        const string CONTENT301 = "<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML 2.0//EN\">\n" +
                "<html><head>\n<title>301 Moved Permanently</title>\n</head><body>\n" +
                "<h1>Moved Permanently</h1>\n" +
                "<p>The document has moved to <a href='http://{0}{1}'>http://{0}{1}</a>.</p>\n" +
                "</body></html>\n";

        static void Redirect(HttpWorkerRequest wr, string location)
        {
            string host = wr.GetKnownRequestHeader(HttpWorkerRequest.HeaderHost);
            wr.SendStatus(301, "Moved Permanently");
            wr.SendUnknownResponseHeader("Connection", "close");
            wr.SendUnknownResponseHeader("Date", DateTime.Now.ToUniversalTime().ToString("r"));
            wr.SendUnknownResponseHeader("Location", String.Format("http://{0}{1}", host, location));
            Encoding enc = Encoding.ASCII;
            wr.SendUnknownResponseHeader("Content-Type", "text/html; charset=" + enc.WebName);
            string content = String.Format(CONTENT301, host, location);
            byte[] contentBytes = enc.GetBytes(content);
            wr.SendUnknownResponseHeader("Content-Length", contentBytes.Length.ToString());
            wr.SendResponseFromMemory(contentBytes, contentBytes.Length);
            wr.FlushResponse(true);
            wr.CloseConnection();
        }
    }
}
