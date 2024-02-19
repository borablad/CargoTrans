using CargoTrans.ViewModels;

namespace CargoTrans.Views;

public partial class NewDispatchView : ContentPage
{
    NewDispatchViewModel vm;
    public NewDispatchView()
	{
		InitializeComponent();
        vm = (NewDispatchViewModel)BindingContext;
    }

    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        if (vm.IsNotNull(vm._USserialPort))
            if (vm._USserialPort.IsOpen)
                vm._USserialPort.Close();
        if (vm.IsNotNull(vm._MassserialPort))
            if (vm._MassserialPort.IsOpen)
                vm._MassserialPort.Close();
    }
}