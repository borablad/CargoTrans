using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrans.Helpers
{
    internal interface IWifiPrinterHelper
    {
          bool ConnectToWifiPrinter(string ipAddress);
    }
}
