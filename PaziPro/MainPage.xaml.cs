using System.Collections.ObjectModel;

namespace PaziPro
{
    public partial class MainPage : ContentPage
    {
        private readonly MQTTService _mqttService;
        private readonly WifiService _wifiService;
        public ObservableCollection<MessageItem> Messages { get; set; } = [];
        public MainPage()
        {
            InitializeComponent();
            messagesListView.ItemsSource = Messages;
            _mqttService = new MQTTService();
            _wifiService = new WifiService();

            _mqttService.Configure(UpdateConnectionStatus, DisplayReceivedMessage);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            await AttemptAutomaticConnectionAsync();
        }

        private void UpdateWiFiConnectionStatus(bool isConnected)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                wifiStatusLabel.Text = isConnected ? "WiFi Connected - (Disabled for Testing)" : "WiFi Not Connected - (Disabled for Testing)";
                //wifiStatusLabel.TextColor = isConnected ? Colors.Green : Colors.Red;
                wifiStatusLabel.TextColor = isConnected ? Colors.Gray : Colors.Gray;
            });
        }

        private void UpdateConnectionStatus(bool isConnected)
        {
            connectionStatusLabel.Text = isConnected ? "Connected" : "Disconnected";
            connectionStatusLabel.TextColor = isConnected ? Colors.Green : Colors.Red;
        }

        // Method to display received MQTT messages
        private void DisplayReceivedMessage(string topic, string message)
        {
            // this is really bad but im doing this for testing :D 
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Messages.Add(new MessageItem { Topic = topic, Message = message});
            });
        }

        private void OnClearMessagesClicked(object sender, EventArgs e)
        {
            // Clear the messages in the ListView
            Messages.Clear();
        }

        private void OnSettingsClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingPage(_mqttService, _wifiService));
        }

        private void OnManageSubscriptionsClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ManageSubscriptionsPage(_mqttService));
        }

        private async Task AttemptAutomaticConnectionAsync()
        {
            // Load stored credentials
            string ssid = Preferences.Get("wifiSSID", string.Empty);
            string wifiPassword = await SecureStorage.GetAsync("wifiPassword") ?? string.Empty;
            string mqttServer = Preferences.Get("mqttServer", string.Empty);
            string mqttUser = Preferences.Get("mqttUser", string.Empty);
            string mqttPassword = await SecureStorage.GetAsync("mqttPassword") ?? string.Empty;
            string mqttTopic = Preferences.Get("mqttTopic", string.Empty);

            // Check if credentials are present
            if (CredentialsStored(mqttServer, mqttUser, mqttPassword, mqttTopic, ssid, wifiPassword))
            {
                // Connect to MQTT
                if (!_mqttService.IsConnected)
                {
                    await _mqttService.Connect_Client(mqttServer, mqttUser, mqttPassword);
                    await _mqttService.SubscribeToTopic(mqttTopic);
                }
            }
            else
            {
                Console.WriteLine("No stored credentials found. Automatic connection skipped.");
            }
        }

        private bool CredentialsStored(string mqttServer, string mqttUser, string mqttPassword, string mqttTopic, string wifiSSID, string wifiPassword)
        {
            return !string.IsNullOrEmpty(mqttServer) && !string.IsNullOrEmpty(mqttTopic);

            // for later :D
            return !string.IsNullOrEmpty(mqttServer) && !string.IsNullOrEmpty(mqttUser) &&
                !string.IsNullOrEmpty(mqttPassword) && !string.IsNullOrEmpty(mqttTopic) &&
                !string.IsNullOrEmpty(wifiSSID) && !string.IsNullOrEmpty(wifiPassword);
        }
    }

    public class MessageItem
    {
        public string Topic { get; set;}
        public string Message { get; set; }
    }
}
