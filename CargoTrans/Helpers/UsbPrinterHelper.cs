using System.IO.Ports;

public class UsbPrinterHelper
{
    private SerialPort usbPort;

    public UsbPrinterHelper()
    {
        usbPort = new SerialPort();
        usbPort.BaudRate = 9600; 
        usbPort.DataBits = 8;
        usbPort.Parity = Parity.None;
        usbPort.StopBits = StopBits.One;
    }

    public string FindPrinterPort()
    {
        string[] portNames = SerialPort.GetPortNames();

        foreach (string portName in portNames)
        {
            try
            {
                usbPort.PortName = portName;
                usbPort.Open();
                usbPort.Close();
                return portName;
            }
            catch (Exception)
            {
                // Продолжаем перебор портов
            }
        }

        return null; // Принтер не обнаружен
    }

    public void ConnectToUsbPrinter()
    {
        string portName = FindPrinterPort();
        if (portName != null)
        {
            try
            {
                usbPort.PortName = portName;
                usbPort.Open();
                Console.WriteLine("Принтер успешно подключен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка подключения к принтеру: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Принтер не найден.");
        }
    }

    public void PrintText(string text)
    {
        try
        {
            if (usbPort.IsOpen)
            {
                usbPort.Write(text);
                Console.WriteLine("Текст успешно отправлен на печать.");
            }
            else
            {
                Console.WriteLine("Порт принтера не открыт.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке текста на принтер: {ex.Message}");
        }
    }




}
