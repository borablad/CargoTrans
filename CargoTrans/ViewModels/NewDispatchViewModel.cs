using CargoTrans.Helpers;
using Microsoft.Maui.ApplicationModel;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Input;
using ZXing;


namespace CargoTrans.ViewModels
{
    public partial class NewDispatchViewModel : BaseViewModel
    {
        private readonly WifiPrinterHelper wifiPrinterHelper = new ();
        private readonly UsbPrinterHelper usbPrinter =new UsbPrinterHelper ();
        //TextFields
        [ObservableProperty]
        private string fioTextField;
        private string addressTextField;
        private string phoneNumberTextField;
        private string emailTextField;
        private string ticketNumberTextField;
        [ObservableProperty]
        private string description;
        [ObservableProperty]
        private string cargoCode;


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



                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(json);

                        foreach (var item in data.result)
                        {
                            string title = item.title;
                            resultList.Add(title);
                        }
                    }
                    else
                    {
                        await AppShell.Current.DisplayAlert("","Failed to fetch data from the API. Status code: " + response.StatusCode ,"ok");
                    }
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("", "An error occurred: " + ex.Message, "ok");
            }
            return resultList;
        }








    }
}
