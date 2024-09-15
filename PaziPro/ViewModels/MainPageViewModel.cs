using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PaziPro.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly MQTTService _mqttService;
        private readonly WifiService _wifiService;

        public string _connectionStatus;
        public string ConnectionStatus
        {
            get => _connectionStatus;
            set
            {
                if (_connectionStatus != value)
                {
                    _connectionStatus = value;
                    OnPropertyChanged(nameof(ConnectionStatus));
                }
            }
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    OnPropertyChanged(nameof(IsConnected));
                    OnPropertyChanged(nameof(ConnectionStatusText));
                    OnPropertyChanged(nameof(ConnectionStatusColor));
                }
            }
        }

        public string ConnectionStatusText => IsConnected ? "Connected" : "Disconnected";
        public Color ConnectionStatusColor => IsConnected ? Colors.Green : Colors.Red;


        public ObservableCollection<MessageItem> Messages { get; } = [];

        // Commands
        public ICommand NavigateToSettingsCommand { get; }
        public ICommand NavigateToManageSubscriptionsCommand { get; }

        // Constructor
        public MainPageViewModel(MQTTService mqttService, WifiService wifiService)
        {
            _mqttService = mqttService;
            _wifiService = wifiService;

            // Initialize properties
            ConnectionStatus = "Disconnected";
            IsConnected = false;

            // Initialize commands
            NavigateToSettingsCommand = new Command(async () => await NavigateToSettings());
            NavigateToManageSubscriptionsCommand = new Command(async () => await NavigateToManageSubscriptions());

            // Configure MQTT Service with callbacks
            _mqttService.Configure(UpdateConnectionStatus, DisplayReceivedMessage);

            // Attempt automatic connection
            _ = AttemptAutomaticConnectionAsync();
        }

        private async Task AttemptAutomaticConnectionAsync()
        {
            try
            {
                // Delay to allow network initialization
                //await Task.Delay(2000);

                // Check network access
                if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                {
                    Console.WriteLine("No internet connection available.");
                    await DisplayAlert("Connection Error", "No internet connection available.");
                    return;
                }

                // Load stored credentials
                string ssid = Preferences.Get("wifiSSID", string.Empty);
                string wifiPassword = await SecureStorage.GetAsync("wifiPassword") ?? string.Empty;
                string mqttServer = Preferences.Get("mqttServer", string.Empty);
                string mqttUser = Preferences.Get("mqttUser", string.Empty);
                string mqttPassword = await SecureStorage.GetAsync("mqttPassword") ?? string.Empty;

                // Check if credentials are present
                if (CredentialsStored(mqttServer, mqttUser, mqttPassword, ssid, wifiPassword))
                {
                    //UpdateWiFiConnectionStatus(false);

                    // Connect to WiFi
                    //bool wifiConnected = await _wifiService.ConnectToWiFi(ssid, wifiPassword);
                    var wifiConnected = true;
                    if (wifiConnected)
                    {
                        UpdateWiFiConnectionStatus(true);

                        // Connect to MQTT
                        if (!_mqttService.IsConnected)
                        {
                            await _mqttService.Connect_Client(mqttServer, mqttUser, mqttPassword);
                        }

                        // Subscribe to persisted topics
                        await SubscribeToPersistedTopics();
                    }
                    else
                    {
                        Console.WriteLine("Failed to connect to WiFi. Cannot proceed with MQTT.");
                        await DisplayAlert("Connection Error", "Failed to connect to WiFi.");
                    }
                }
                else
                {
                    Console.WriteLine("No stored credentials found. Automatic connection skipped.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during automatic connection: {ex.Message}");
                await DisplayAlert("Connection Error", $"An error occurred: {ex.Message}");
            }
        }

        private async Task SubscribeToPersistedTopics()
        {
            string json = Preferences.Get("SubscribedTopics", string.Empty);
            if (!string.IsNullOrEmpty(json))
            {
                var topics = System.Text.Json.JsonSerializer.Deserialize<List<string>>(json);
                foreach (var topic in topics)
                {
                    await _mqttService.SubscribeToTopic(topic);
                }
            }
        }

        private void UpdateConnectionStatus(bool isConnected)
        {
            IsConnected = isConnected;
        }

        private void UpdateWiFiConnectionStatus(bool isConnected)
        {
            // Update WiFi connection status if needed
        }

        private void DisplayReceivedMessage(string topic, string message)
        {
            // Since we're updating an ObservableCollection, we don't need to dispatch to the main thread.
            Messages.Add(new MessageItem { Topic = topic, Message = message });
        }

        private async Task NavigateToSettings()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SettingPage(_mqttService, _wifiService));
        }

        private async Task NavigateToManageSubscriptions()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ManageSubscriptionsPage(_mqttService));
        }

        private Task DisplayAlert(string title, string message)
        {
            return Application.Current.MainPage.DisplayAlert(title, message, "OK");
        }

        // Implement INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool CredentialsStored(string mqttServer, string mqttUser, string mqttPassword, string wifiSSID, string wifiPassword)
        {
            return !string.IsNullOrEmpty(mqttServer) ;//&& !string.IsNullOrEmpty(mqttTopic);

            // for later :D
            return !string.IsNullOrEmpty(mqttServer) && !string.IsNullOrEmpty(mqttUser) &&
                !string.IsNullOrEmpty(mqttPassword) && //!string.IsNullOrEmpty(mqttTopic) &&
                !string.IsNullOrEmpty(wifiSSID) && !string.IsNullOrEmpty(wifiPassword);
        }
    }

    // Define a MessageItem class
    public class MessageItem
    {
        public string Topic { get; set; }
        public string Message { get; set; }
    }
}
