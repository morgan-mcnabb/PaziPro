using PaziPro.ViewModels;

namespace PaziPro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        private readonly MQTTService _mqttService;
        private readonly WifiService _wifiService;
        private SettingsViewModel _vm;

        public SettingPage(MQTTService mqttService, WifiService wifiService)
        {
            InitializeComponent();
            _mqttService = mqttService;
            _wifiService = wifiService;
            _vm = (SettingsViewModel)BindingContext;

            BindingContext = _vm;

            MainThread.BeginInvokeOnMainThread(() =>
            {
                saveButton.IsEnabled = false;
            });
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadUserSettingsAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                saveButton.IsEnabled = true;
            });
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            bool wifiConnected = await _wifiService.ConnectToWiFi(_vm.Ssid, _vm.Password);

            wifiConnected = true;
            if (wifiConnected)
            {
                await SaveUserSettingsAsync();
                //UpdateWiFiConnectionStatus(true);

                if (!_mqttService.IsConnected)
                    await _mqttService.Connect_Client(_vm.MqttServer, _vm.MqttUser, _vm.MqttPassword);


                string topic = _vm.MqttTopic;  
                await _mqttService.SubscribeToTopic(topic);

                await Navigation.PopAsync();
            }
            else
            {
                Console.WriteLine("Failed to connect to WiFi. Cannot proceed with MQTT.");
            }
        }

        private async Task SaveUserSettingsAsync()
        {
            Preferences.Set("wifiSSID", _vm.Ssid);
            Preferences.Set("mqttServer", _vm.MqttServer);
            Preferences.Set("mqttUser", _vm.MqttUser);
            Preferences.Set("mqttTopic", _vm.MqttTopic);

            if(!string.IsNullOrEmpty(_vm.Password))
                await SecureStorage.SetAsync("wifiPassword", _vm.Password);
            else
                SecureStorage.Remove("wifiPassword");
            
            if(!string.IsNullOrEmpty(_vm.MqttPassword))
                await SecureStorage.SetAsync("mqttPassword", _vm.MqttPassword);
            else
                SecureStorage.Remove("mqttPassword");
        }

        private async Task LoadUserSettingsAsync()
        {
            _vm.Ssid = Preferences.Get("wifiSSID", string.Empty);
            _vm.MqttServer = Preferences.Get("mqttServer", string.Empty);
            _vm.MqttUser = Preferences.Get("mqttUser", string.Empty);
            _vm.MqttTopic = Preferences.Get("mqttTopic", string.Empty);

            _vm.Password = await SecureStorage.GetAsync("wifiPassword") ?? string.Empty;
            _vm.MqttPassword = await SecureStorage.GetAsync("mqttPassword") ?? string.Empty;
        }
    }
}