﻿using CargoTrans.Helpers;
using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public  class WifiPrinterHelper : IWifiPrinterHelper
{
    public  bool ConnectToWifiPrinter(string ipAddress)
    {
        try
        {
            using (var client = new TcpClient(ipAddress, 9100)) 
            {
                //тестовая печать
                using (var stream = client.GetStream())
                {
                    byte[] data = Encoding.UTF8.GetBytes("CargoTrans!\n");
                    stream.Write(data, 0, data.Length);

                    Console.WriteLine("Тестовая печать прошла успешно!");

                    return true; 
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка подключения к принтеру: {ex.Message}");
            return false;
        }
    }


    public bool PrintImageToWifiPrinter(string ipAddress, Bitmap image)
    {
        try
        {
            using (var client = new TcpClient(ipAddress, 9100))
            using (var stream = client.GetStream())
            {
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                Console.WriteLine("Успешная печать штрих-кода");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке изображения на принтер: {ex.Message}");
            return false;
        }
    }



    public WifiPrinterHelper() { }

}
