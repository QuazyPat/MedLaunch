using Microsoft.Data.Sqlite;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace MedLaunch.Classes
{
    /// <summary>
    /// Database migration routines
    /// </summary>
    public static class DbMigration
    {
        public static bool CheckVersions()
        {
            // skip this as still testing
           // return false;

            string dbPath = @"Data\Settings\MedLaunch.db";
            // first check whether the database exists - return if it does not
            if (!File.Exists(dbPath))
                return false;

            // create System.Data.SQLite connection
            string connString = "Data Source=" + AppDomain.CurrentDomain.BaseDirectory + dbPath;

            string dbVersion = "";
            string appVersion = VersionChecker.ReturnApplicationVersion();
            // connect to database and retreive the current version
            using (SqliteConnection conn = new SqliteConnection(connString))
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT dbVersion ");
                query.Append("FROM Versions ");
                query.Append("WHERE versionId = 1");
                using (SqliteCommand cmd = new SqliteCommand(query.ToString(), conn))
                {
                    conn.Open();
                    using (SqliteDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            //Console.WriteLine(dr.GetValue(0) + " " + dr.GetValue(1) + " " + dr.GetValue(2));
                            dbVersion = dr.GetValue(0).ToString();
                        }
                    }
                }
            }

            // check the two versions
            string[] dbVersionArr = dbVersion.Split('.');
            string[] appVersionArr = appVersion.Split('.');
            int i = 0;
            bool upgradeNeeded = false;
            while (i < 3)
            {
                // if anything but the 4th number (private build) is greater in the appVersion - database needs to be upgraded
                if (Convert.ToInt32(appVersionArr[i]) > Convert.ToInt32(dbVersionArr[i]))
                {
                    // database upgrade needed
                    upgradeNeeded = true;
                    break;
                }
                i++;
            }

            if (upgradeNeeded == false)
                return false;

            /* start the DB upgrade procedure */
            DoDbUpgrade();
            return false;        
            
        }

        public static void DoDbUpgrade()
        {
            // build command line args
            string processArg = "/P:" + Process.GetCurrentProcess().Id.ToString();
            string dbUpdateArg = "/DBU:MedLaunch.db";
            string args = processArg + " " + dbUpdateArg;

            // call the updater app and close this one
            Process.Start("Updater.exe", args);
            Thread.Sleep(5000);
            Environment.Exit(0);
        }
    }
}
