using System.Collections.Generic;

namespace MedLaunch._Debug.DATDB.Platforms.PSXDATACENTER.Models
{
    public class PsxDataCenterCollection
    {
        // Properties
        public List<DAT_Rom> Data { get; set; }

        public PsxDataCenterCollection()
        {
            Data = new List<DAT_Rom>();

            Data = ImportPsxDataCenterdata.Go();
        }
    }
}
