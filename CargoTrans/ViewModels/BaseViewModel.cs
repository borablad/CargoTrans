using CargoTrans.Services;
using CargoTrans.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
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
        private int scalesPortName, scannerPortName, printPortName, wifiPrintIpAddres;

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
            if(IsNotNull(_USserialPort) && _USserialPort.IsOpen)
            {
                _USserialPort.Close();
            }

            Portsnames = SerialPort.GetPortNames();

            _USserialPort = new SerialPort(Portsnames[scannerPortName+1], 9600); // Укажите нужный COM порт и скорость передачи данных
            //_USserialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            _USserialPort.DataReceived += DataReceivedHandler;

            try
            {
                _USserialPort.Open();
                
            }
            catch (Exception ex)
            {
                 AppShell.Current.DisplayAlert("", ex.Message,"ok");
            }

            //// Укажите COM-порт, который вы хотите прослушивать
            //string portName = ports[0];

            //// Подключаемся к COM порту
            //_MassserialPort = new SerialPort(portName, 4800);

            //// Устанавливаем параметры порта
            //_MassserialPort.DataBits = 8;
            //_MassserialPort.Parity = Parity.Space;
            //_MassserialPort.StopBits = StopBits.One;

            //// Обработчик события при появлении новых данных в порте
            //_MassserialPort.DataReceived += SerialPort_DataReceived;

            //// Открываем порт
            //try
            //{
            //    _MassserialPort.Open();
            //    Console.WriteLine("Порт открыт. Ожидание данных...");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Ошибка открытия порта: " + ex.Message);
            //    return;
            //}

            //// Ожидание завершения работы программы
            //Console.ReadLine();

            //// Закрываем порт
            ////serialPort.Close();
        }

        private  void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string data = sp.ReadExisting();

            // Разделение данных на строки
            string[] lines = data.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            if (lines.Length >= 4)
            {
                // Взятие последних 4 значений
                string[] lastFourLines = lines.Skip(lines.Length - 4).ToArray();


                    Width = Convert.ToInt32(lastFourLines[0].Substring(0, lastFourLines[0].Length - 3));
                    Length = Convert.ToInt32(lastFourLines[1].Substring(0, lastFourLines[1].Length - 3));
                    Height = Convert.ToInt32(lastFourLines[2].Substring(0, lastFourLines[2].Length - 3));

                    // Здесь можно выполнить дополнительные действия с полученными значениями
                    //Console.WriteLine($"First value: {firstValue}, Second value: {secondValue}, Third value: {thirdValue}");
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
            else
            {
                AppShell.Current.DisplayAlert("Error", "Ошибка: Неверный формат данных.", "ok");
                Console.WriteLine("Ошибка: Неверный формат данных.");
            }
        }
    }
}
