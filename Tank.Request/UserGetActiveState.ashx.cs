using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using SqlDataProvider.Data;

namespace Tank.Request
{
    public class UserGetActiveState : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            int userID = 0;
            int activeID = 0;
            string key = context.Request["key"] ?? string.Empty;
            if (!int.TryParse(context.Request["selfid"], out userID) ||
                !int.TryParse(context.Request["activeID"], out activeID) ||
                userID <= 0 || activeID <= 0)
            {
                WriteResult(context, false, activeID, false, "Invalid request");
                return;
            }
            using (PlayerBussiness players = new PlayerBussiness())
            {
                PlayerInfo player = players.GetUserSingleByUserID(userID);
                if (player == null || !string.Equals(Md5Hex(player.Password ?? string.Empty), key,
                    StringComparison.OrdinalIgnoreCase))
                {
                    WriteResult(context, false, activeID, false, "Auth failed");
                    return;
                }
            }

            bool isAttend = false;
            string connectionString = ConfigurationManager.ConnectionStrings["Db_TankConnectionString"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            using (SqlCommand command = connection.CreateCommand())
            {
                command.CommandText = "SELECT TOP 1 1 FROM Active_Number " +
                    "WHERE ActiveID = @ActiveID AND UserID = @UserID AND PullDown = 1";
                command.Parameters.AddWithValue("@ActiveID", activeID);
                command.Parameters.AddWithValue("@UserID", userID);
                connection.Open();
                isAttend = command.ExecuteScalar() != null;
            }

            WriteResult(context, true, activeID, isAttend, "Success!");
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

        private static void WriteResult(HttpContext context, bool value, int activeID,
            bool isAttend, string message)
        {
            XElement result = new XElement("Result",
                new XAttribute("value", value),
                new XAttribute("activeID", activeID),
                new XAttribute("isAttend", isAttend),
                new XAttribute("message", message));
            context.Response.Write(result.ToString(SaveOptions.DisableFormatting));
        }

        public bool IsReusable { get { return false; } }
    }
}
