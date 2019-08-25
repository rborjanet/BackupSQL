using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackupSQL.Utilities
{
    public static class Utilities
    {


        public static bool DeleteDirectory(string url)
        {
            try
            {

                if (Directory.Exists(url))
                {
                    Directory.Delete(url,true);
                    return true;
                }
                else
                    return true;
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message.ToString(), "Error Message");
                return false;
            }
        }

        public static bool CreateDirectory(string url)
        {

            try
            {

                //Create TMP (tempora folder) Folder
                if (Directory.Exists(url))
                    return true;
                else
                {
                    Directory.CreateDirectory(url);
                    return true;
                }
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.Message.ToString(), "Error Message");
                return false;
            }

        }

       

        public static string DateFormat(DateTime datetime)
        {
            string year = datetime.Year.ToString();
            string month = datetime.Month.ToString();
            string day = datetime.Day.ToString();

            string hour = datetime.Hour.ToString();
            string minute = datetime.Minute.ToString();

            string FullDateTime;

            if (month.Length < 2)
                month = "0" + month;

            if (day.Length < 2)
                day = "0" + day;

            if (hour.Length < 2)
                hour = "0" + hour;

            if (minute.Length < 2)
                minute = "0" + minute;

            FullDateTime = "["+year+"-"+month+"-"+day+"]["+hour+"h"+minute+"m]";

            return FullDateTime;
        }
    }
}
