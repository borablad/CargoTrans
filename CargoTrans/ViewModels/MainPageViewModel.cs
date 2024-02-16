using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CargoTrans.ViewModels
{
    public partial class  MainPageViewModel : BaseViewModel
    {
        //public ICommand ConetDiviceCommand { get; }

       
       

        public MainPageViewModel()
        {
            Portsnames = SerialPort.GetPortNames();

            // ConetDiviceCommand = new Command(ConetDivice);
        }







    }
}
