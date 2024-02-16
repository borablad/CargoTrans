using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Newtonsoft.Json;
using System.Drawing;
using System.IO.Ports;
using System.Windows.Input;
using ZXing;


namespace CargoTrans.ViewModels
{
    public partial class NewDispatchViewModel : BaseViewModel
    {
        private readonly WifiPrinterHelper wifiPrinterHelper = new ();
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

        public NewDispatchViewModel()
        {
            LoadData ();
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

        
        public async void LoadData()
        {
            PointsList = await FetchPointDataFromApi();
            Portsnames = SerialPort.GetPortNames();

        }

        [RelayCommand]
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
