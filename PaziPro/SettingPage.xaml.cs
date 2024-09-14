namespace PaziPro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        private readonly MQTTService _mqttService;
        private readonly WifiService _wifiService;

        public SettingPage(MQTTService mqttService, WifiService wifiService)
        {
            InitializeComponent();
            _mqttService = mqttService;
            _wifiService = wifiService;
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
            bool wifiConnected = await _wifiService.ConnectToWiFi(ssid.Text, password.Text);

            wifiConnected = true;
            if (wifiConnected)
            {
                await SaveUserSettingsAsync();
                //UpdateWiFiConnectionStatus(true);

                if (!_mqttService.IsConnected)
                    await _mqttService.Connect_Client(mqttServer.Text, mqttUser.Text, mqttPassword.Text);


                string topic = mqttTopic.Text;  // Get the topic from the user input
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
            Preferences.Set("wifiSSID", ssid.Text);
            Preferences.Set("mqttServer", mqttServer.Text);
            Preferences.Set("mqttUser", mqttUser.Text);
            Preferences.Set("mqttTopic", mqttTopic.Text);

            if(!string.IsNullOrEmpty(password.Text))
                await SecureStorage.SetAsync("wifiPassword", password.Text);
            
            if(!string.IsNullOrEmpty(mqttPassword.Text))
                await SecureStorage.SetAsync("mqttPassword", mqttPassword.Text);
        }

        private async Task LoadUserSettingsAsync()
        {
            ssid.Text = Preferences.Get("wifiSSID", string.Empty);
            mqttServer.Text = Preferences.Get("mqttServer", string.Empty);
            mqttUser.Text = Preferences.Get("mqttUser", string.Empty);
            mqttTopic.Text = Preferences.Get("mqttTopic", string.Empty);

            password.Text = await SecureStorage.GetAsync("wifiPassword") ?? string.Empty;
            mqttPassword.Text = await SecureStorage.GetAsync("mqttPassword") ?? string.Empty;
        }
    }
}