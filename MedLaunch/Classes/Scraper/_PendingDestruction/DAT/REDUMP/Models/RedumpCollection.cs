using System.Collections.Generic;

namespace MedLaunch.Classes.Scraper.DAT.REDUMP.Models
{
    public class RedumpCollection
    {
        // Properties
        public List<RedumpObject> Data { get; set; }

        // Constructors
        public RedumpCollection()
        {
            Data = new List<RedumpObject>();

            Data = ImportRedumpData.Go();
        }
    }
}
