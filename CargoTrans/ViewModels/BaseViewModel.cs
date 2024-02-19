using CargoTrans.Services;
using CargoTrans.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ZXing.QrCode.Internal;

namespace CargoTrans.ViewModels
{

    public partial class BaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private int weight;
        [ObservableProperty]
        private int height;
        [ObservableProperty]
        private int width;
        [ObservableProperty]
        private int length;
        [ObservableProperty]
        private double coubheight;


        public SerialPort _USserialPort, _MassserialPort;

        [ObservableProperty]
        private string[] portsnames;

        [ObservableProperty]
        private int scalesPortName = -5, scannerPortName = -5, printPortName;

        [ObservableProperty]
        protected bool isBusy;

        public MockDataStore mockDataStore = new MockDataStore();

        [ObservableProperty]
        private string title, wifiPrintIpAddres;

        protected Action currentDismissAction;




        // Навигация по основным вкладкам
        [RelayCommand]
        public async void OpenMainPage() => await AppShell.Current.GoToAsync($"//{nameof(MainPage)}");
        [RelayCommand]
        public async void OpenNewDispatchView() => await AppShell.Current.GoToAsync($"//{nameof(NewDispatchView)}");
        //[RelayCommand]
        //public async void DespatchsView() => await AppShell.Current.GoToAsync($"{nameof(MainPage)}");


        public bool IsNotNull(params string[] values) => !values.Any(string.IsNullOrWhiteSpace);
        public bool IsNotNull(params object[] values) => !values.Any(x => x == null);

        [RelayCommand]
        public void ConetDivice()
        {
            if (IsNotNull(_USserialPort))
                if (_USserialPort.IsOpen)
                    _USserialPort.Close();

            if (ScannerPortName >=0) // Если кто то это увидет простите сейчас важна скорость дедлаин сгорел вчера
            {

                _USserialPort = new SerialPort(Portsnames[ScannerPortName], 9600); // Укажите нужный COM порт и скорость передачи данных
                _USserialPort.DataReceived += DataReceivedHandler;
                try { _USserialPort.Open(); }
                catch (Exception ex) { AppShell.Current.DisplayAlert("", ex.Message, "ok"); }
            }
            else
            {
                AppShell.Current.DisplayAlert("Внимание", "Не был выбран порт сканера", "ok");
            }

            if (IsNotNull(_MassserialPort))
                if (_MassserialPort.IsOpen)
                    _USserialPort.Close();


            if (ScalesPortName>=0)
            {
                // Подключаемся к COM порту
                _MassserialPort = new SerialPort(Portsnames[ScalesPortName], 4800, Parity.Even, 8, StopBits.One);

                // Обработчик события при появлении новых данных в порте
                _MassserialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);

                // Открываем порт
                try 
                {
                    _MassserialPort.Open();
                    SendCommand(0x45);
                }
                catch (Exception ex) 
                {
                    AppShell.Current.DisplayAlert("Внимание", "Не получилось подключится к весам", "ok");
                    return; 
                }
            }
            else
            {
                AppShell.Current.DisplayAlert("Внимание", "Не был выбран порт весов", "ok");
            }



        }

        private void SendCommand(byte command)
        {
            try
            {
                _MassserialPort.Write(new byte[] { command }, 0, 1);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();

            // Разделение данных на строки
            string[] lines = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length >= 4)
            {
                // Взятие последних 4 значений
                string[] lastFourLines = lines.Skip(lines.Length - 4).ToArray();

                try
                {
                    Width = Convert.ToInt32(lastFourLines[0].Substring(0, lastFourLines[0].Length - 3));
                    Length = Convert.ToInt32(lastFourLines[1].Substring(0, lastFourLines[1].Length - 3));
                    Height = Convert.ToInt32(lastFourLines[2].Substring(0, lastFourLines[2].Length - 3));
                    if (Width > 2 || Length >2 || Height>2)
                    {
                        SendCommand(0x45);

                    }
                    Coubheight = (Width*Length*Height);
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); }

            }
            else
            {
                Console.WriteLine("Not enough lines received.");
            }
        }
        public void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {

            SerialPort sp = (SerialPort)sender;
            int bytesToRead = sp.BytesToRead;
            byte[] data = new byte[bytesToRead];
            sp.Read(data, 0, bytesToRead);

            // Обработка полученных данных для запроса массы
            if (data.Length >= 2)
            {
                // Извлечение знака массы
                bool isNegative = (data[data.Length- 1] & 0x80) != 0;

                // Извлечение значения массы

                int weightValue = ((data[data.Length - 1] & 0x7F) << 8) | (data[data.Length - 2] & 0x7F); // 
                Weight = weightValue;
                // Вывод результата
                //if (isNegative)
                //{
                //    Console.WriteLine("Масса: -{0} грамм", weightValue);
                //}
                //else
                //{
                //    Console.WriteLine("Масса: {0} грамм", weightValue);
                //}
            }
        }

        [RelayCommand]
        public void GetPorts()
        {
            Portsnames = SerialPort.GetPortNames();
        }

        public async Task<string> GetStockName(string stockId)
        {
            string result = "";
            string apiUrl = $"https://ktzh.shit-systems.dev/api/stock/stock/{stockId}";
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
                        result = data.title;
                        
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
            return result;
        }

        public void Print_Barcode(string plain_text)
        {
            try
            {
                if (!IsNotNull(WifiPrintIpAddres))
                {
                    AppShell.Current.DisplayAlert("","Не введён ip адрес принтера печать невозможна","ok") ;
                }
                //var plain_text = "text";
                var ip = WifiPrintIpAddres;
                //var ip = "192.168.31.98";
                if (string.IsNullOrWhiteSpace(ip))
                {
                    throw new Exception("No Ip address");
                }

                // Connect to the printer using TCP socket
                using (var client = new TcpClient(ip, 9100))  // Assuming the printer uses port 9100
                using (var stream = client.GetStream())
                {


                    var s = Encoding.Default;
                    byte[] postData = Encoding.Default.GetBytes(plain_text);

                    stream.Write(postData);
                }
            }
              catch (Exception ex)
            {
                //DialogService.ShowToast(ex.Message);

                // Handle connection or printing error
                  Console.WriteLine($"Error printing directly to printer: {ex.Message}");
            }

        }

        public static void PrintText(string plain_text)
        {
        }
        public string Generate_barcode(string barcode, string msg)
        {
            char ESC = (char)27; char GS = (char)29;
            string getBarcodeStr;
            getBarcodeStr = ESC + "a" + (char)1; //align center
            getBarcodeStr = getBarcodeStr + GS + "h" + (char)80; //Bardcode Hieght           
            getBarcodeStr = getBarcodeStr + GS + "w" + (char)3; //Barcode Width 1 to 4
            getBarcodeStr = getBarcodeStr + GS + "f" + (char)0; //Font for HRI characters
            getBarcodeStr = getBarcodeStr + GS + "H" + (char)2; //Position of HRI characters
            getBarcodeStr = getBarcodeStr + GS + "k" + (char)69 + (char)barcode.Length; //'Print Barcode Smb 39            
            getBarcodeStr = getBarcodeStr + barcode + (char)0; //'Print Text Under            
            getBarcodeStr = getBarcodeStr + GS + "d" + (char)3;
            getBarcodeStr = getBarcodeStr + msg + "\r\n";
            getBarcodeStr = getBarcodeStr + GS + "@";
            getBarcodeStr = getBarcodeStr + ESC + "a" + (char)0; //align left
            return getBarcodeStr;
        }

    }
}
