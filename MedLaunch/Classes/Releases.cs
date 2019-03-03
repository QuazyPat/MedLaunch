using System;
using System.Collections.Generic;

namespace Medlaunch.Classes
{
    /// <summary>
    /// Release class (possibly not needed anymore)
    /// </summary>
    public class Release
    {
        public string Version { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public List<string> Changelog { get; set; }
    }
    
}
