﻿using System.Collections.Generic;
using System.Linq;

namespace MedLaunch._Debug.ScrapeDB
{
    public class MasterView
    {
        public int gid { get; set; }
        public int? mid { get; set; }
        public string GDBTitle { get; set; }
        public int pid { get; set; }
        public string PlatformName { get; set; }
        public string PlatformAlias { get; set; }        
        public string GDBYear { get; set; }
        public string MOBYTitle { get; set; }
        public string MOBYAlias { get; set; }        
        public string MOBYYear { get; set; }
        public string MOBYPlatformName { get; set; }
        public string MOBYPlatformAlias { get; set; }
        public string MOBYurl { get; set; }

        public static List<MasterView> GetMasterView()
        {
            using (var context = new AsniScrapeAdminDbContext())
            {
                var cData = (from g in context.MasterView
                             select g);
                return cData.ToList();
            }
        }
    }
}
