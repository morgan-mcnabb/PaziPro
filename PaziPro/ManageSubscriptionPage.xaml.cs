using System.Collections.ObjectModel;

namespace PaziPro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageSubscriptionsPage : ContentPage
    {
        //private readonly MQTTService _MQTTService;
        //public ObservableCollection<string> SubscribedTopics { get; set; } = new ObservableCollection<string>();

        public ManageSubscriptionsPage(MQTTService mqttService)
        {
            InitializeComponent();
            BindingContext = new ViewModels.ManageSubscriptionsViewModel(mqttService);
        }
    }
}