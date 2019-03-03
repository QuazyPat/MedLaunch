using System.Collections.Generic;

namespace MedLaunch.Classes.Scraper.DAT.TOSEC.Models
{
    public class ToSecCollection
    {
        // Properties
        public List<ToSecObject> Data { get; set; }

        // Constructors
        public ToSecCollection()
        {
            Data = new List<ToSecObject>();

            Data = ImportToSecData.Go();
        }
    }
}
