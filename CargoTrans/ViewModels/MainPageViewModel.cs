using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
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


        [ObservableProperty]
        private string cargoCode;

        public MainPageViewModel()
        {
            Portsnames = SerialPort.GetPortNames();

            // ConetDiviceCommand = new Command(ConetDivice);
        }

        [RelayCommand]
        public async void Send()
        {
            //if(Width==0 || Weight==0)
            //{
            //    AppShell.Current.DisplayAlert("","Не получены о весе или размере","ok");
            //    return;
            //}
            if (!IsNotNull(CargoCode))
            {
                AppShell.Current.DisplayAlert("", "Не введён код отправки", "ok");
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
                    //user = new
                    //{
                    //    login = "example_login",
                    //    phone = "123456789",
                    //    password = "example_password",
                    //    point_id = "00000000-0000-0000-0000-000000000000" //только если сотрудник
                    //},
                    //info = new
                    //{
                    //    iin = "123456789012",
                    //    photo = "example_photo_url",
                    //    first_name = "John",
                    //    last_name = "Doe",
                    //    address = "123 Main St",
                    //    notes = "Example notes",
                    //    institution = "Example Institution" //только если юр. лицо
                    //},
                    //code = CargoCode,
                    //point_to_id = "293f91c8-0f3d-487e-9181-638bbc4040b1",
                    //point_from_id = "01b752fd-e628-4d91-b080-6352937b36b8",
                    //recipient = new
                    //{
                    //    first_name = "Nur",
                    //    last_name = "Arman",
                    //    middle_name = "1C",
                    //    address = "Vorovstro karaetsa",
                    //    phone = "7786157277",
                    //    email = "nur@arman.com"
                    //},
                    dimensions = new
                    {
                        height = Height,
                        wigth = Width,
                        depth = Length,
                        mass = Weight
                    }//,
                    //ticket = new
                    //{
                    //    ticket_number = "123124"
                    //}
                };

                var json = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);
                var data = new StringContent(json, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
                _httpClient.DefaultRequestHeaders.Add("RefreshToken", refreshToken);
                // Аутентификация
                var response = await _httpClient.PatchAsync($"keeper/application/{CargoCode}", data);
                //var responseData = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {

                    var responseData = await response.Content.ReadAsStringAsync();
                    //var _authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseData);
                    DataModel item_data = JsonConvert.DeserializeObject<DataModel>(responseData);

                    // Использование значений barcode и mark
                    string barcodeValue = item_data.Barcode;
                    string markValue = item_data.Mark;


                    // Обработка полученных данных, если необходимо
                    return;

                }
                else
                {
                    await AppShell.Current.DisplayAlert("", "Failed to send data with the API. Status code: " + response.StatusCode, "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                await AppShell.Current.DisplayAlert("", "An error occurred: " + ex.Message, "OK");
                return;
            }

        }




    }
    public class DataModel
    {
        public string Barcode { get; set; }
        public string Mark { get; set; }
    }
}
