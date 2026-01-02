using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Backend.Controllers
{
    public class CommonAPIController : ApiController
    {

        public static void Log(string message)
        {
            try
            {
                string logPath = System.Web.Hosting.HostingEnvironment.MapPath("~/Logs.txt");
                string logMessage = $"=============================={DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                File.AppendAllText(logPath, logMessage);
            }
            catch(Exception ex) 
            {
                File.AppendAllText("C:\\Temp\\ErrorLog.txt", ex.Message);
            }
        }
    }
}
