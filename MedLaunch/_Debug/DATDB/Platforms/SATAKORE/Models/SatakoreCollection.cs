using System.Collections.Generic;

namespace MedLaunch._Debug.DATDB.Platforms.SATAKORE.Models
{
    public class SatakoreCollection
    {
        // Properties
        public List<DAT_Rom> Data { get; set; }

        public SatakoreCollection()
        {
            Data = new List<DAT_Rom>();
            Data = ImportSatakoreData.Go();
        }
    }
}
