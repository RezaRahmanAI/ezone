using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IExcelReaderService
    {
        DataTable GetDataTableFromSpreadsheet(Stream MyExcelStream, bool ReadOnly);
    }
}
