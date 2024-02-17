using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Newtonsoft.Json;
using System.Drawing;
using System.IO.Ports;
using System.Net;
using System.Text;
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
        [ObservableProperty]
        private string addressTextField;
        [ObservableProperty]
        private string phoneNumberTextField;
        [ObservableProperty]
        private string emailTextField;
        [ObservableProperty]
        private string ticketNumberTextField;
        [ObservableProperty]
        private string description;
        [ObservableProperty]
        private string cargoCode;

        [ObservableProperty]
        private List<string> pointsList = new List<string>();

        [ObservableProperty]
        private int departurePointId, destinationPointId;

        public string AddressTextField
        {
            LoadData();
            //PointsList.Add("Алматы");
            //PointsList.Add("Астана");
            //PointsList.Add("Шымкент");
            //PointsList.Add("Караганда");
            //PointsList.Add("Актобе");
            //PointsList.Add("Тараз");
            //PointsList.Add("Павлодар");
            //PointsList.Add("Усть-Каменогорск");
            //PointsList.Add("Семей");
            //PointsList.Add("Атырау");
            //PointsList.Add("Костанай");
            //PointsList.Add("Кызылорда");
        }

        [RelayCommand]
        public void Send()
        {
            if(!IsNotNull(FioTextField))
            {
                AppShell.Current.DisplayAlert("","Введите все обязательные поля","ok");
                return;
            }
            SendCargoInfo();
        }

        public async void LoadData()
        {
            PointsList = await FetchPointDataFromApi();
            Portsnames = SerialPort.GetPortNames();
            GetCargoCode();
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
                        await AppShell.Current.DisplayAlert("", "Failed to fetch data from the API. Status code: " + response.StatusCode, "ok");
                    }
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("", "An error occurred: " + ex.Message, "ok");
            }
            return resultList;
        }


        public async void GetCargoCode()
        {
            try
            {
                string accessToken = await SecureStorage.GetAsync("AccessToken");
                string refreshToken = await SecureStorage.GetAsync("RefreshToken");

                string apiUrl = "https://ktzh.shit-systems.dev/api/keeper/application/setup";

                using (HttpClient client = new HttpClient())
                {
                    Uri uri = new Uri(apiUrl);

                    // Устанавливаем токены в заголовки запроса
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    client.DefaultRequestHeaders.Add("RefreshToken", refreshToken);

                    var response = await client.GetAsync(uri);

                    if (response.IsSuccessStatusCode)
                    {
                       string json = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(json);
                        CargoCode = data.result;
                    }
                    else
                    {
                        await AppShell.Current.DisplayAlert("", "Failed to fetch data from the API. Status code: " + response.StatusCode, "ok");
                    }
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("", "An error occurred: " + ex.Message, "ok");
            }

        }

        public async void SendCargoInfo()
        {
            try
            {
                var Request = new
                {
                    user = new 
                    { 
                        login = EmailTextField,
                        phone = PhoneNumberTextField,
                        password = "AzSxDc123!!",
 
                    },
                    info = new
                    {

                        first_name = FioTextField,
                        last_name = FioTextField,

                    },
                    code = CargoCode,
                    point_to_id = "c92df023-8450-43cc-8215-030cbb439dbc",
                    point_from_id = "2c200c48-0096-497a-9eeb-dc76b6793bd9"
                };
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(Request);


                string accessToken = await SecureStorage.GetAsync("AccessToken");
                string refreshToken = await SecureStorage.GetAsync("RefreshToken");

                string apiUrl = "https://ktzh.shit-systems.dev/api/keeper/application/";

                using (HttpClient _httpClient = new HttpClient())
                {
                    Uri uri = new Uri(apiUrl);

                    // Устанавливаем токены в заголовки запроса
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                    _httpClient.DefaultRequestHeaders.Add("RefreshToken", refreshToken);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");

                    var response = await _httpClient.PostAsync(uri, content);
                    var r = response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        string responseJson = await response.Content.ReadAsStringAsync();
                        return;//  responseJson;
                    }
                    else
                    {
                        return;//  $"Failed to send application. Status code: {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                return;// $"An error occurred: {ex.Message}";
            }
        }




    }
}
