using System.Collections.Generic;

namespace MedLaunch.Classes.Scraper.DAT.NOINTRO.Models
{
    public class NoIntroCollection
    {
        // Properties
        public List<NoIntroObject> Data { get; set; }

        // Constructors
        public NoIntroCollection()
        {
            Data = new List<NoIntroObject>();

            Data = ImportNoIntroData.Go();
        }
    }
}
