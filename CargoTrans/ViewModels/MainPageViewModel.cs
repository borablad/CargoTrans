using CommunityToolkit.Mvvm.ComponentModel;
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
        public ICommand ConetDiviceCommand { get; }

       
       

        public MainPageViewModel()
        {
            ConetDiviceCommand = new Command(ConetDivice);
        }

        public void SaveData(int _weight, int _height, int _width,int _length)
        {
            Weight = _weight;
            Height = _height;
            Width = _width;
            Length = _length;
        }

        public void ConetDivice()
        {
            // Укажите COM-порт, который вы хотите прослушивать
            var ports = SerialPort.GetPortNames();
             string portName = ports[0];
                
            // Подключаемся к COM порту
            SerialPort serialPort = new SerialPort(portName, 4800);

            // Устанавливаем параметры порта
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.Space;
            serialPort.StopBits = StopBits.One;

            // Обработчик события при появлении новых данных в порте
            serialPort.DataReceived += SerialPort_DataReceived;

            // Открываем порт
            try
            {
                serialPort.Open();
                Console.WriteLine("Порт открыт. Ожидание данных...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка открытия порта: " + ex.Message);
                return;
            }

            // Ожидание завершения работы программы
            Console.ReadLine();

            // Закрываем порт
            //serialPort.Close();
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
                AppShell.Current.DisplayAlert("Error","Ошибка: Неверный формат данных.","ok");
                Console.WriteLine("Ошибка: Неверный формат данных.");
            }
        }

    }
}
