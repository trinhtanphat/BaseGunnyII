using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Xml.Linq;
using Bussiness;
using log4net;
using SqlDataProvider.Data;

namespace Tank.Request.Live
{
    public sealed class DailyLogListHandler : IHttpHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public void ProcessRequest(HttpContext context)
        {
            bool success = false;
            string message = "Fail!";
            XElement result = new XElement("Result");
            try
            {
                int userId;
                if (!Int32.TryParse(context.Request["selfid"], out userId) || userId <= 0)
                    throw new ArgumentException("Invalid selfid.");
                DateTime now = DateTime.Now;
                using (ProduceBussiness db = new ProduceBussiness())
                {
                    DailyLogListInfo info = db.GetDailyLogListSingle(userId);
                    if (info == null)
                    {
                        info = new DailyLogListInfo();
                        info.UserID = userId;
                        info.UserAwardLog = 0;
                        info.DayLog = String.Empty;
                        info.LastDate = now.AddDays(-1);
                        if (!db.AddDailyLogList(info))
                            throw new InvalidOperationException("Cannot create DailyLogList row.");
                    }

                    NormalizeDailyLog(info, now);
                    if (!db.UpdateDailyLogList(info))
                        throw new InvalidOperationException("Cannot update DailyLogList row.");

                    result.Add(new XElement("DailyLogList",
                        new XAttribute("UserAwardLog", info.UserAwardLog),
                        new XAttribute("DayLog", info.DayLog ?? String.Empty),
                        new XAttribute("luckyNum", 0),
                        new XAttribute("myLuckyNum", 0)));
                }
                success = true;
                message = "Success!";
            }
            catch (Exception ex)
            {
                Log.Error("dailyloglist", ex);
            }

            result.Add(new XAttribute("value", success));
            result.Add(new XAttribute("message", message));
            result.Add(new XAttribute("nowDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
            context.Response.ContentType = "text/plain";
            context.Response.BinaryWrite(Tank.Request.StaticFunction.Compress(result.ToString(false)));
        }

        private static void NormalizeDailyLog(DailyLogListInfo info, DateTime now)
        {
            bool monthChanged = info.LastDate.Year != now.Year || info.LastDate.Month != now.Month;
            if (monthChanged)
            {
                info.DayLog = String.Empty;
                info.UserAwardLog = 0;
                info.LastDate = now.AddDays(-1);
            }

            List<string> days = ParseDays(info.DayLog);
            if (days.Count > now.Day)
                days.RemoveRange(now.Day, days.Count - now.Day);

            while (days.Count < now.Day - 1)
                days.Add("False");

            if (days.Count >= now.Day && IsChecked(days[now.Day - 1]) && info.LastDate.Date < now.Date)
                info.LastDate = now;

            int checkedCount = 0;
            for (int i = 0; i < days.Count; i++)
            {
                days[i] = IsChecked(days[i]) ? "True" : "False";
                if (days[i] == "True") checkedCount++;
            }
            info.UserAwardLog = checkedCount;
            info.DayLog = String.Join(",", days.ToArray());
        }

        private static List<string> ParseDays(string dayLog)
        {
            List<string> days = new List<string>();
            if (String.IsNullOrWhiteSpace(dayLog)) return days;
            string[] parts = dayLog.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < parts.Length; i++)
                days.Add(parts[i].Trim());
            return days;
        }

        private static bool IsChecked(string value)
        {
            return String.Equals(value, "True", StringComparison.OrdinalIgnoreCase);
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}
