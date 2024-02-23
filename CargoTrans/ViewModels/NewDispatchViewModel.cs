using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        private string recepfirst_name,
                        receplast_name,
                        recepmiddle_name,
                        recepaddress,
                        recepphone,
                        recepemail;

        [ObservableProperty]
        private List<string> pointsList = new List<string>();
        [ObservableProperty]
        private List<string> pointsIdList = new List<string>();

        [ObservableProperty]
        private int departurePointId, destinationPointId;

        public NewDispatchViewModel()
        {
            LoadData ();

        }

        [RelayCommand]
        public async void Send()
        {
            if(!IsNotNull(FioTextField)|| !IsNotNull(AddressTextField)|| !IsNotNull(EmailTextField)|| DeparturePointId<0||DestinationPointId<0)
            {
                await AppShell.Current.DisplayAlert("", "Введите все обязательные поля", "ok");
                return;
            }
            if (Height < 2 || Weight < 2)
            {
                await AppShell.Current.DisplayAlert("", "Некоректные данные о размере или весе", "ok");
                return;
            }

            bool _pay = await AppShell.Current.DisplayAlert("Оплата", "Ожидание оплаты", "Отправитель оплатил отправку", "Отправитель не оплатил отправку");
            if (!_pay)
                return;
            SendCargoInfo();
        }

        public async void LoadData()
        {
            PointsList = await FetchPointDataFromApi();
            //Portsnames = SerialPort.GetPortNames();
            GetCargoCode();
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

        public async Task<List<string>> FetchPointDataFromApi()
        {
            string apiUrl = "https://ktzh.shit-systems.dev/api/point/?page=0&size=0";
            List<string> resultList = new List<string>();
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    Uri uri = new Uri(apiUrl);
                    //HttpResponseMessage response = await client.GetAsync(uri);
                    var response = await client.GetAsync(uri);
                    //var h = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        string json = await response.Content.ReadAsStringAsync();
                        dynamic data = JsonConvert.DeserializeObject(json);

                        foreach (var item in data.result)
                        {
                            string title = item.title;
                            string ID = item.ID;
                            resultList.Add(title);
                            PointsIdList.Add(ID);
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


        private Bitmap GenerateBarcodeImage(string barcodeData)
        {
            BarcodeWriter<Bitmap> writer = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.CODE_128
            };
            return writer.Write(barcodeData);
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

        [RelayCommand]
        public async void SendCargoInfo()
        {
            //if(Width==0 || Weight==0)
            //{
            //    AppShell.Current.DisplayAlert("","Не получены о весе или размере","ok");
            //    return;
            //}
            if (!IsNotNull(CargoCode))
            {
                AppShell.Current.DisplayAlert("", "Не получен код отправки", "ok");
                return;
            }

            var _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://ktzh.shit-systems.dev/api/");
            try
            {
                string accessToken = await SecureStorage.GetAsync("AccessToken");
                string refreshToken = await SecureStorage.GetAsync("RefreshToken");
                var requestData = new
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
                        address = AddressTextField,
                        //notes = ,

                    },
                    code = CargoCode,
                    point_to_id = PointsIdList[DestinationPointId],
                    //point_from_id = PointsIdList[DeparturePointId],
                    recipient = new
                    {
                        first_name = Recepfirst_name,
                        last_name = Receplast_name,
                        middle_name = Recepmiddle_name,
                        address = Recepaddress,
                        phone = Recepphone,
                        email = Recepemail
                    },
                    dimensions = new
                    {
                        height = Height,
                        weigth = Width,
                        depth = Length,
                        mass = Weight
                    },
                    ticket = new
                    {
                        ticket_number = TicketNumberTextField
                    }
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                _httpClient.DefaultRequestHeaders.Add("RefreshToken", refreshToken);
                // Аутентификация
                var response = await _httpClient.PostAsync($"keeper/application/", data);
                //var responseData = await response.Content.ReadAsStringAsync();

                 if (response.IsSuccessStatusCode)
                {

                    var responseData = await response.Content.ReadAsStringAsync();
                    //var _authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseData);
                    DataModel item_data = JsonConvert.DeserializeObject<DataModel>(responseData);

                    // Использование значений barcode и mark
                    string barcodeValue = item_data.Barcode;
                    string markValue = item_data.Mark;
                    string entity_id = item_data.Entity_id;
                    AppShell.Current.DisplayAlert("Отправка отправлена на кассу", $"Отнесите посылку на склад: { await GetStockName(entity_id)} \r\n Barcode: {barcodeValue}", "готово");

                    string GS = Convert.ToString((char)29);
                    string ESC = Convert.ToString((char)27);

                    string CUTCOMMAND = "";
                    CUTCOMMAND = ESC + "@";
                    CUTCOMMAND += GS + "V" + (char)48;
                    Print_Barcode($" \r\n \r\n \r\n Height : {Height},\r\n Wight : {Width},\r\n Length : {Length},\r\n Weight : {Weight},\r\n CargoCode : {CargoCode}  \r\n" + Generate_barcode(barcodeValue, barcodeValue) + " \r\n  \r\n  \r\n \r\n  \r\n  \r\n " + CUTCOMMAND);


                    // Обработка полученных данных, если необходимо

                }
                else
                {
                    await AppShell.Current.DisplayAlert("", "Failed to send data with the API. Status code: " + response.StatusCode, "OK");
                 
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("", "An error occurred: " + ex.Message, "OK");
               
            }
            GetCargoCode();

        }




    }



}
