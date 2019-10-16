<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

public class Handler : IHttpHandler {

    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "application/json";
        context.Response.Write("{ \"hello\": \"Bob\" }");
    }

    public bool IsReusable {
        get {
            return false;
        }
    }
}
