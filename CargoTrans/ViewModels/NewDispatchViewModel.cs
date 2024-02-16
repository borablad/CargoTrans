using CargoTrans.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Platform;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Windows.Input;
using ZXing;
using ZXing.QrCode;

namespace CargoTrans.ViewModels
{
    public partial class NewDispatchViewModel : BaseViewModel
    {
        private readonly WifiPrinterHelper wifiPrinterHelper = new ();
        private readonly UsbPrinterHelper usbPrinter =new UsbPrinterHelper ();
        //TextFields
        [ObservableProperty]
        private string fioTextField;

        [ObservableProperty]
        private string addressTextField;

        [ObservableProperty]
        private string phoneNumberTextField;

        [ObservableProperty]
        private string emailTextField;

        [ObservableProperty]
        private string ticketNumberTextField;

        private List<string> data = new List<string> ();


        
        public NewDispatchViewModel()
        {
            GenerateBarcodeCommand = new Command(GenerateBarCode);
            GetTextCommand = new Command(GetText);
            usbPrinter = new UsbPrinterHelper ();

            
        }

        private void addList()
        {
            data.Add(FioTextField);
            data.Add(AddressTextField);
            data.Add(PhoneNumberTextField);
            data.Add(EmailTextField);
            data.Add(TicketNumberTextField);
        }

        public ICommand GenerateBarcodeCommand { get; }
        private void GenerateBarCode()
        {
            try
            {
                wifiPrinterHelper.ConnectToWifiPrinter("192.168.31.98");

                addList();

                if (data != null)
                {
                    wifiPrinterHelper.PrintTextToWoifiPrinter("192.168.31.98", data);
                }

               /* string barcodeData = string.Join("\n", data);
                Bitmap barcodeBitmap = GenerateBarcodeImage(barcodeData);

                wifiPrinterHelper.PrintImageToWifiPrinter("192.168.31.98", barcodeBitmap);*/
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Что-то пошло не так! {ex.ToString()}");
            }
        }

        private Bitmap GenerateBarcodeImage(string barcodeData)
        {
            try


                 
            {
                BarcodeWriter<Bitmap> writer = new BarcodeWriter<Bitmap>
                {
                    Format = BarcodeFormat.CODE_128
                };

                if (writer != null)
                {
                    return writer.Write(barcodeData);
                }
                else
                {
                    Console.WriteLine("Ошибка при генерации штрих-кода: объект writer не был инициализирован");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при генерации штрих-кода: {ex.Message}");
                throw;
            }
        }



        public ICommand GetTextCommand { get; }

        private void GetText()
        {
            try
            {
                usbPrinter.ConnectToUsbPrinter();
                usbPrinter.PrintText("Hello world!\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"что то пошло не так! {ex.ToString}");
            }
        }

        










    }
}
