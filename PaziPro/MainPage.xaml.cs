using PaziPro.ViewModels;

namespace PaziPro
{
    public partial class MainPage : ContentPage
    {
        private readonly MainPageViewModel _vm;

        public MainPage()
        {
            InitializeComponent();

            _vm = new MainPageViewModel(new MQTTService(), new WifiService());

            BindingContext = _vm;
        }
    }
}
