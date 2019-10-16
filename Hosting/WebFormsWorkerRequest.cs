using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Hosting;
using WebFormsHost.Hosting;

namespace WebFormsHost
{
    public class WebFormsWorkerRequest : SimpleWorkerRequest
    {
        private WebFormsApplicationHost _host;
        private WebFormsWorker _worker;

        private EndOfSendNotification _endSend;
        private object _endSendData;

        private bool _inUnhandledException;

        private string _hostPath;
        private string _hostVPath;
        private string _hostPhysicalRoot;
        private readonly int _localPort;
        private readonly string _localAddress;

        private Encoding _encoding;
        private Encoding _headerEncoding;

        private int _inputLength;
        private bool _refilled;
        private int _position;
        private byte[] _inputBuffer;

        private int _statusCode;
        private string _statusDescription;
        private bool _headersSent;

        private Hashtable _headers;
        private NameValueCollection _responseHeaders;
        private string[][] _unknownHeaders;


        private NameValueCollection _serverVariables;
        private byte[] _queryStringBits;

        static string[] indexFiles = { "index.aspx",
                        "Default.aspx",
                        "default.aspx",
                        "index.html",
                        "index.htm" };

        public WebFormsWorkerRequest(WebFormsWorker worker, WebFormsApplicationHost host) 
            : base(String.Empty, String.Empty, null)
        {
            _host = host;
            _worker = worker;

            try {
                GetRequestHeaders();
            }
            catch{
                CloseConnection();
                throw;
            }

            using(var ms = new MemoryStream()) {
                _worker.GetContext().Request.Body.CopyTo(ms);
                _inputBuffer = ms.ToArray();
                _inputLength = _inputBuffer.Length;
                _position = 0;
            }

            _responseHeaders = new NameValueCollection();

            _statusCode = 200;
            _statusDescription = "OK";

            _localPort = _worker.GetContext().Connection.LocalPort;
            _localAddress = _worker.GetContext().Connection.LocalIpAddress.ToString();
        }

        public event EndOfRequestHandler EndRequest;

        public void ProcessRequest() {
            string error = null;
            _inUnhandledException = false;

            try
            {
                //System.Diagnostics.StackTrace t = new System.Diagnostics.StackTrace();
                //byte[] b = Encoding.GetBytes("Here's the trace: <br /><br />" + t.ToString());
                //_worker.Write(b, 0, b.Length);

                //AssertFileAccessible();
                HttpRuntime.ProcessRequest(this);
            }
            catch (HttpException ex)
            {
                _inUnhandledException = true;
                error = ex.GetHtmlErrorMessage();
            }
            catch (Exception ex)
            {
                _inUnhandledException = true;
                var hex = new HttpException(400, "Bad request", ex);
                error = hex.GetHtmlErrorMessage();
            }

            if (!_inUnhandledException)
                return;
                
            //error = "HELLLO FROM WebFormsWorkerRequest";

            if (error.Length == 0)
                error = String.Format("<html><head><title>Runtime Error</title></head><body>An exception ocurred:<pre>{0}</pre></body></html>", "Unknown error");

            try
            {
                SendStatus(400, "Bad request");
                SendUnknownResponseHeader("Connection", "close");
                SendUnknownResponseHeader("Date", DateTime.Now.ToUniversalTime().ToString("r"));

                Encoding enc = Encoding.UTF8;

                byte[] bytes = enc.GetBytes(error);

                SendUnknownResponseHeader("Content-Type", "text/html; charset=" + enc.WebName);
                SendUnknownResponseHeader("Content-Length", bytes.Length.ToString());
                SendResponseFromMemory(bytes, bytes.Length);
                FlushResponse(true);
            }
            catch (Exception)
            { // should "never" happen
                throw;
            }
        }

        public override void SetEndOfSendNotification(EndOfSendNotification callback, object extraData)
        {
            _endSend = callback;
            _endSendData = extraData;
        }

        public override void EndOfRequest()
        {
            if (EndRequest != null)
                EndRequest(this);

            if (_endSend != null)
                _endSend(this, _endSendData);
        }

        public override string GetAppPath()
        {
            return _hostVPath;
        }

        public override string GetAppPathTranslated()
        {
            return _hostPath;
        }

        public override string GetFilePath()
        {
            return _worker.GetContext().Request.Path;
        }

        public override string GetFilePathTranslated()
        {
            return MapPath(GetFilePath());
        }

        public override string GetHttpVerbName()
        {
            return _worker.GetContext().Request.Method;
        }

        public override string GetHttpVersion()
        {
            return _worker.GetContext().Request.Protocol;
        }

        public override string GetLocalAddress()
        {
            return "localhost";
        }

        public override string GetKnownRequestHeader(int index)
        {
            if (_headers == null)
                return null;

            var headerName = GetKnownRequestHeaderName(index);
            return _headers[headerName] as string;
        }

        public override string GetUnknownRequestHeader(string name)
        {
            if (_headers == null)
                return null;

            return _headers[name] as string;
        }

        public override string[][] GetUnknownRequestHeaders()
        {
            if (_unknownHeaders == null)
            {
                if (_headers == null)
                    return (_unknownHeaders = new string[0][]);

                ICollection keysColl = _headers.Keys;
                ICollection valuesColl = _headers.Values;
                var keys = new string[keysColl.Count];
                var values = new string[valuesColl.Count];
                keysColl.CopyTo(keys, 0);
                valuesColl.CopyTo(values, 0);

                int count = keys.Length;
                var pairs = new List<string[]>();
                for (int i = 0; i < count; i++)
                {
                    int index = GetKnownRequestHeaderIndex(keys[i]);
                    if (index != -1)
                        continue;
                    pairs.Add(new[] { keys[i], values[i] });
                }

                if (pairs.Count != 0)
                    _unknownHeaders = pairs.ToArray();
            }

            return _unknownHeaders;
        }

        public override string GetServerName()
        {
            string hostHeader = GetKnownRequestHeader(HeaderHost);
            if (String.IsNullOrEmpty(hostHeader))
            {
                hostHeader = GetLocalAddress();
            }
            else
            {
                int colonIndex = hostHeader.IndexOf(':');
                if (colonIndex > 0)
                {
                    hostHeader = hostHeader.Substring(0, colonIndex);
                }
                else if (colonIndex == 0)
                {
                    hostHeader = GetLocalAddress();
                }
            }
            return hostHeader;
        }

        public override int GetLocalPort()
        {
            return 0;
        }

        public override byte[] GetPreloadedEntityBody()
        {
            if (GetHttpVerbName().ToLower() != "post" || _refilled || _position >= _inputLength)
                return null;

            if (_inputLength == 0)
                return null;

            var contentLength = (string)_headers["Content-Length"];
            long length = 0;

            // If not empty parse, if correctly parsed validate
            if (!String.IsNullOrEmpty(contentLength)
                && Int64.TryParse(contentLength, out length)
                && length > Int32.MaxValue)
                throw new InvalidOperationException("Content-Length exceeds the maximum accepted size.");

            int inputDataLength = _inputLength - _position;
            if (length < 0 || length > inputDataLength)
                length = inputDataLength;

            var result = new byte[length];
            Buffer.BlockCopy(_inputBuffer, _position, result, 0, (int)length);
            _position = 0;
            _inputLength = 0;

            //maybe do nothing with this and guard reading with other variables
            _inputBuffer = null;

            return result;
        }

        public override byte[] GetQueryStringRawBytes()
        {
            if (_queryStringBits == null) {
                var queryString = GetQueryString();

                if (queryString != null)
                    _queryStringBits = Encoding.GetBytes(queryString);
            }

            return _queryStringBits;
        }

        public override string GetQueryString()
        {
            return _worker.GetContext().Request.QueryString.ToString();
        }

        public override string GetRawUrl()
        {
            return _worker.GetContext().Request.Path;
        }

        public override string GetRemoteAddress()
        {
            return _worker.GetContext().Connection.RemoteIpAddress.ToString();
        }

        public override string GetRemoteName()
        {
            var ip = GetRemoteAddress();
            string name;

            try {
                System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(ip);
                name = entry.HostName;
            }
            catch{
                name = ip;
            }

            return name;
        }

        public override int GetRemotePort()
        {
            return _worker.GetContext().Connection.RemotePort;
        }

        public override string GetPathInfo()
        {
            return GetRawUrl();
        }

        public override string GetUriPath()
        {
            return GetRawUrl();
        }

        public override bool HeadersSent()
        {
            return _headersSent;
        }

        public override bool IsClientConnected()
        {
            return _worker.IsConnected();
        }

        public override string MapPath(string path)
        {
            return "";
        }


        public override bool IsEntireEntityBodyIsPreloaded()
        {
            if (GetHttpVerbName().ToLower() != "post" || _refilled || _position >= _inputLength)
                return false;

            var contentLength = (string)_headers["Content-Length"];
            long length;
            if (!Int64.TryParse(contentLength, out length))
                return false;

            return (length <= _inputLength);
        }

        public override int ReadEntityBody(byte[] buffer, int size)
        {
            var verb = GetHttpVerbName();

            if (verb == "GET" || verb == "HEAD" || size == 0 || buffer == null)
                return 0;

            return ReadInput(buffer, 0, size);
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
            if (length <= 0)
                return;

            if (data.Length < length)
                length = data.Length;
                
            if (!_headersSent)
            {
                SendHeaders();
            }

            int sent = Send(data, 0, length);
            if (sent != length)
                throw new IOException("Blocking send did not send entire buffer");                
        }

        public override void SendResponseFromMemory(IntPtr data, int length)
        {
            byte[] bits = new byte[length];
            Marshal.Copy(data, bits, 0, length);

            SendResponseFromMemory(bits, length);
        }

        public override void SendStatus(int statusCode, string statusDescription)
        {
            _statusCode = statusCode;
            _statusDescription = statusDescription;
        }

        public override void SendCalculatedContentLength(int contentLength)
        {
            SendUnknownResponseHeader("Content-Length", contentLength.ToString());
        }

        public override void SendKnownResponseHeader(int index, string value)
        {
            if (_headersSent)
                return;

            var headerName = GetKnownResponseHeaderName(index);
            SendUnknownResponseHeader(headerName, value);
        }

        public override void SendUnknownResponseHeader(string name, string value)
        {
            if (_headersSent)
                return;

            _responseHeaders.Add(name, value);
        }

        public override int ReadEntityBody(byte[] buffer, int offset, int size)
        {
            var verb = GetHttpVerbName().ToLower();

            if (verb == "get" || verb == "head" || size == 0 || buffer == null)
                return 0;

            return ReadInput(buffer, 0, size);
        }

        public override void SendResponseFromFile(string filename, long offset, long length)
        {
            using (FileStream fs = File.OpenRead(filename))
            {
                SendFromStream(fs, offset, length);
            }
        }

        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
            Stream file = null;
            try
            {
                file = new FileStream(handle, FileAccess.Read);
                SendFromStream(file, offset, length);
            }
            finally
            {
                if (file != null)
                    file.Close();
            }
        }

        public override bool IsSecure()
        {
            return _worker.GetContext().Request.IsHttps;
        }

        public override void CloseConnection()
        {
            _worker.Close();
        }

        public override void FlushResponse(bool finalFlush)
        {
            try {
                if (!_headersSent)
                    SendHeaders();

                if (finalFlush)
                    CloseConnection();
            }
            catch {
                CloseConnection();
            }
        }

        public override string GetServerVariable(string name)
        {
            if (_serverVariables == null)
                return String.Empty;

            var s = _serverVariables[name];
            return s ?? String.Empty;
        }

        #region Client Certs

        public override byte[] GetClientCertificate()
        {
            var cert = GetContextClientCertificate();
            return (cert != null) ? cert.RawData : new byte[0];
        }

        public override byte[] GetClientCertificateBinaryIssuer()
        {
            return new byte[0];
        }

        public override int GetClientCertificateEncoding()
        {
            return 0;
        }

        public override byte[] GetClientCertificatePublicKey()
        {
            var cert = GetContextClientCertificate();
            return (cert != null) ? cert.GetPublicKey() : new byte[0];
        }

        public override DateTime GetClientCertificateValidFrom()
        {
            var cert = GetContextClientCertificate();
            return (cert != null) ? cert.NotBefore : DateTime.Now;
        }

        public override DateTime GetClientCertificateValidUntil()
        {
            var cert = GetContextClientCertificate();
            return (cert != null) ? cert.NotAfter : DateTime.Now;
        }

        private X509Certificate2 GetContextClientCertificate() {
            return _worker.GetContext().Connection.ClientCertificate;
        }

        protected void SendFromStream(Stream stream, long offset, long length)
        {
            if (offset < 0 || length <= 0)
                return;

            long stLength = stream.Length;
            if (offset + length > stLength)
                length = stLength - offset;

            if (offset > 0)
                stream.Seek(offset, SeekOrigin.Begin);

            var fileContent = new byte[8192];
            int count = fileContent.Length;
            while (length > 0 && (count = stream.Read(fileContent, 0, count)) != 0)
            {
                SendResponseFromMemory(fileContent, count);
                length -= count;
                // Keep the System. prefix
                count = (int)System.Math.Min(length, fileContent.Length);
            }
        }

        private void AddConnectionHeader()
        {
            //see how keep-alive can be handled or if it needs to.
        }

        private void GetRequestHeaders() {
            var contextHeaders = _worker.GetContext().Request.Headers;

            _headers = new Hashtable();

            for (var i = 0; i < contextHeaders.Keys.Count; i++) {
                var key = contextHeaders.Keys.ElementAt<string>(i);
                var value = contextHeaders[key];

                _headers.Add(key, value.ToString());
            }
        }

        private void AssembleResponseHeaders() {
            _responseHeaders.Add("Date", DateTime.UtcNow.ToString("r", CultureInfo.InvariantCulture));
            _responseHeaders.Add("Sever", "Mono Kestrel With WebForms");
        }

        private void SendHeaders()
        {
            if (_headersSent)
                return;

            //get response headers...
            AssembleResponseHeaders();
            _headersSent = true;

            _worker.GetContext().Response.StatusCode = _statusCode;
            //description?

            for (var i = 0; i < _responseHeaders.Keys.Count; i++) {
                var key = _responseHeaders.Keys[i];
                var value = _responseHeaders[key];

                _worker.WriteHeader(key, value);
            }
        }

        private int Send(byte[] buffer, int offset, int len) {
            _worker.Write(buffer, offset, len);
            return len;
        }

        private int ReadInput(byte[] buffer, int offset, int size) {
            int length = _inputLength - _position;

            if (length > 0)
            {
                if (length > size)
                    length = size;

                Buffer.BlockCopy(_inputBuffer, _position, buffer, offset, length);
                _position += length;
                offset += length;
                size -= length;

                if (size == 0)
                    return length;
            }

            int localsize = 0;

            while (size > 0)
            {
                byte[] readBuffer = new byte[size];
                int read = _worker.Read(readBuffer, 0, size);

                if (read == 0)
                    break;

                if (read < 0)
                    throw new HttpException(500, "Error reading request.");

                Buffer.BlockCopy(readBuffer, 0, buffer, offset, read);
                offset += read;
                size -= read;
                localsize += read;
            }

            return (length + localsize);
        }

        #endregion

        // Gets the physical path of the application host of the
        // current instance.
        private string HostPath
        {
            get
            {
                if (_hostPath == null)
                    _hostPath = _host.Path;

                return _hostPath;
            }
        }

        // Gets the virtual path of the application host of the
        // current instance.
        private string HostVPath
        {
            get
            {
                if (_hostVPath == null)
                    _hostVPath = _host.VPath;

                return _hostVPath;
            }
        }

        private string HostPhysicalRoot
        {
            get
            {
                if (_hostPhysicalRoot == null)
                    _hostPhysicalRoot = _host.Server.PhysicalRoot;

                return _hostPhysicalRoot;
            }
        }

        protected virtual Encoding Encoding
        {
            get
            {
                if (_encoding == null)
                    _encoding = Encoding.GetEncoding(28591);

                return _encoding;
            }
            set
            {
                _encoding = value;
            }
        }

        protected virtual Encoding HeaderEncoding
        {
            get
            {
                if (_headerEncoding == null)
                {
                    HttpContext ctx = HttpContext.Current;
                    HttpResponse response = ctx != null ? ctx.Response : null;
                    Encoding enc = _inUnhandledException ? null :
                        response != null ? response.HeaderEncoding : null;
                    _headerEncoding = enc ?? Encoding;
                }

                return _headerEncoding;
            }
        }
    }
}
