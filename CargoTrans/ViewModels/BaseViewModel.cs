using CargoTrans.Services;
using CargoTrans.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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

        public SerialPort _USserialPort, _MassserialPort;

        [ObservableProperty]
        private string[] portsnames;

        [ObservableProperty]
        private int scalesPortName = -5, scannerPortName = -5, printPortName, wifiPrintIpAddres = -5;

        [ObservableProperty]
        protected bool isBusy;

        public MockDataStore mockDataStore = new MockDataStore();

        [ObservableProperty]
        private string title;

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
            if (IsNotNull(_USserialPort) && _USserialPort.IsOpen)
                _USserialPort.Close();


            if (ScannerPortName != -5) // Если кто то это увидет простите сейчас важна скорость дедлаин сгорел вчера
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
            if (IsNotNull(_MassserialPort) && _MassserialPort.IsOpen)
                _USserialPort.Close();

            if (IsNotNull(ScalesPortName))
            {
                // Подключаемся к COM порту
                _MassserialPort = new SerialPort(Portsnames[ScalesPortName], 4800);

                // Устанавливаем параметры порта
                _MassserialPort.DataBits = 8;
                _MassserialPort.Parity = Parity.Space;
                _MassserialPort.StopBits = StopBits.One;

                // Обработчик события при появлении новых данных в порте
                _MassserialPort.DataReceived += SerialPort_DataReceived;

                // Открываем порт
                try { _MassserialPort.Open(); }
                catch (Exception ex) { return; }
            }
            else
            {
                AppShell.Current.DisplayAlert("Внимание", "Не был выбран порт весов", "ok");
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
            SerialPort serialPort = (SerialPort)sender;

            // Читаем 5 байт из порта
            byte[] buffer = new byte[5];
            serialPort.Read(buffer, 0, buffer.Length);

            // Декодируем данные и конвертируем их в массу
            if (buffer[0] == 0x55 && buffer[1] == 0xAA)
            {
                int wght = buffer[3] * 256 + buffer[2]; // 4-й байт - старший
                bool isPositive = (buffer[4] & 0x80) == 0;
                if (!isPositive)
                {
                    wght = -wght;
                }

                //Console.WriteLine($"Получен вес: {wght} г");
                Weight = wght;
            }
            else { AppShell.Current.DisplayAlert("Error", "Ошибка: Неверный формат данных.", "ok"); }
        }


        public void Print_Barcode(string plain_text)
        {
            try
            {
                //var plain_text = "text";
                var ip = "192.168.31.98";
                if (string.IsNullOrWhiteSpace(ip))
                {
                    throw new Exception("No Ip address");
                }

                // Connect to the printer using TCP socket
                using (var client = new TcpClient(ip, 9100))
                {   // Assuming the printer uses port 9100
                    using (var stream = client.GetStream())
                    {
                        byte[] data = Encoding.UTF8.GetBytes(plain_text);
                        stream.Write(data, 0, data.Length);

                        // stream.Write(postData);
                    }
                }
            }
            catch (Exception ex)
            {
                //DialogService.ShowToast(ex.Message);

                // Handle connection or printing error
                //  Console.WriteLine($"Error printing directly to printer: {ex.Message}");
            }

        }

        public static void PrintText(string plain_text)
        {
        }
        public void Generate_barcode(string _code)
        {

        }
    }
}
