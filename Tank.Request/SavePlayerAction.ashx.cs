using System;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Tank.Request
{
    public class SavePlayerAction : IHttpHandler
    {
        private const string SignKey = "*@&Lsd#1k3df";

        public void ProcessRequest(HttpContext context)
        {
            string username = HttpUtility.UrlDecode(context.Request["username"] ?? string.Empty);
            string actionText = context.Request["actiontype"] ?? string.Empty;
            string sign = context.Request["sign"] ?? string.Empty;

            int actionType;
            bool validAction = int.TryParse(actionText, out actionType) && actionType >= 1 && actionType <= 40;
            bool valid = validAction && IsValidSign(username, actionType, sign);

            XElement result = new XElement("Result",
                new XAttribute("value", valid),
                new XAttribute("message", valid ? "Success!" : "Invalid request"));

            context.Response.ContentType = "text/plain";
            context.Response.Write(result.ToString(SaveOptions.DisableFormatting));
        }

        internal static bool IsValidSign(string username, int actionType, string sign)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(sign))
                return false;

            string expected = Md5Hex(SignKey + username + actionType.ToString());
            return string.Equals(expected, sign, StringComparison.OrdinalIgnoreCase);
        }

        private static string Md5Hex(string value)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
                StringBuilder builder = new StringBuilder(hash.Length * 2);
                foreach (byte b in hash)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        public bool IsReusable { get { return false; } }
    }
}
