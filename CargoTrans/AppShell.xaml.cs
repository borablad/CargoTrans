using CargoTrans.Views;

namespace CargoTrans
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(NewDispatchView), typeof(NewDispatchView));
        }
    }
}
