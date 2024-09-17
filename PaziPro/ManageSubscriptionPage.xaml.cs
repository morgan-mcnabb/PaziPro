
namespace PaziPro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageSubscriptionsPage : ContentPage
    {
        public ManageSubscriptionsPage(MQTTService mqttService)
        {
            InitializeComponent();
            BindingContext = new ViewModels.ManageSubscriptionsViewModel(mqttService);
        }
    }
}