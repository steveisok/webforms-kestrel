using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebFormsHost.Hosting
{
    public abstract class Worker : MarshalByRefObject
    {
        public virtual bool IsAsync
        {
            get { return true; }
        }

        public virtual void SetReuseCount(int reuses)
        {
        }

        public virtual int GetRemainingReuses()
        {
            return 0;
        }

        public abstract Task Run(object state);

        public abstract HttpContext GetContext();

        public abstract void WriteHeader(string name, string value);

        public abstract int Read(byte[] buffer, int position, int size);

        public abstract void Write(byte[] buffer, int position, int size);

        public abstract void Close();

        public abstract void Flush();

        public abstract bool IsConnected();
    }
}
