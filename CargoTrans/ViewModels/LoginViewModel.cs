using CargoTrans.Views;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrans.ViewModels
{
    public partial class LoginViewModel :BaseViewModel
    {
        public LoginViewModel() 
        {
            Login();
        }

        [RelayCommand]
        public async void Login()
        {
           await AppShell.Current.GoToAsync($"{nameof(MainPage)}");
        }
    }
}
