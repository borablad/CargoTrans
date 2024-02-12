using CargoTrans.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CargoTrans.ViewModels
{

    public partial class BaseViewModel : ObservableObject
    {

        [ObservableProperty]
        protected bool isBusy;


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


    }
}
