using System.Windows.Input;

namespace CargoTrans.ViewModels
{
    public partial class NewDispatchViewModel : BaseViewModel
    {
        private readonly WifiPrinterHelper wifiPrinterHelper = new ();
        //TextFields
        private string fioTextField;
        private string addressTextField;
        private string phoneNumberTextField;
        private string emailTextField;
        private string ticketNumberTextField;


        public string FioTextField
        {
            get => fioTextField;
            set => SetProperty(ref fioTextField, value);
        }

        public string AddressTextField
        {
            get { return addressTextField; }
            set => SetProperty(ref addressTextField, value);
        }

        public string PhoneNumberTextField
        {
            get => phoneNumberTextField;
            set => SetProperty(ref phoneNumberTextField, value);
        }

        public string EmailTextField
        {
            get => emailTextField;
            set { SetProperty(ref emailTextField, value);}
        }

        public string TicketNumberTextField
        {
            get => ticketNumberTextField;
            set => SetProperty (ref ticketNumberTextField, value);
        }

        public ICommand GenerateBarcodeCommand { get; }

        public NewDispatchViewModel()
        {
            GenerateBarcodeCommand = new Command(GenerateBarCode);
            
            
           
        }

        private void GenerateBarCode()
        {
            try
            {
                wifiPrinterHelper.ConnectToWifiPrinter("192.168.31.98");


                Bitmap bitmap = GenerateBarcodeImage("Aitanatov Rauan");

                bool success = wifiPrinterHelper.PrintImageToWifiPrinter("192.168.31.98", bitmap);


                if (success)
                {
                    Console.WriteLine("Штрих-код успешно отправлен на печать");
                }
                else
                {
                    Console.WriteLine("Ошибка при отправке штрих-кода на печать");
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Что-то пошло не так!{ex.ToString}");
            }
            
            
        }

        private Bitmap GenerateBarcodeImage(string barcodeData)
        {
            BarcodeWriter<Bitmap> writer = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.CODE_128
            };
            return writer.Write(barcodeData);
        }









    }
}
