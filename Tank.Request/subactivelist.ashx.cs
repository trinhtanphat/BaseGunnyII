using System;
using System.Web;
using System.Xml.Linq;

namespace Tank.Request
{
    public class subactivelist : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var result = new XElement("Result",
                new XAttribute("value", true),
                new XAttribute("nowTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")),
                new XAttribute("message", "Success!"));

            context.Response.ContentType = "text/plain";
            context.Response.Write(result.ToString(SaveOptions.DisableFormatting));
        }

        public bool IsReusable { get { return false; } }
    }
}
