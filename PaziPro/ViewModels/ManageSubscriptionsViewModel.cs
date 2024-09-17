using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text.Json;

namespace PaziPro.ViewModels
{
    public class ManageSubscriptionsViewModel : INotifyPropertyChanged
    {
        private readonly MQTTService _mqttService;

        public ObservableCollection<string> SubscribedTopics { get; } = new ObservableCollection<string>();

        private string _newTopic;
        public string NewTopic
        {
            get => _newTopic;
            set
            {
                if (_newTopic != value)
                {
                    _newTopic = value;
                    OnPropertyChanged(nameof(NewTopic));
                }
            }
        }

        public ICommand SubscribeCommand { get; }
        public ICommand UnsubscribeCommand { get; }

        public ManageSubscriptionsViewModel(MQTTService mqttService)
        {
            _mqttService = mqttService;

            // Initialize commands
            SubscribeCommand = new Command(async () => await SubscribeToTopic());
            UnsubscribeCommand = new Command<string>(async (topic) => await UnsubscribeFromTopic(topic));

            // Load the initial list of subscribed topics
            LoadSubscribedTopics();
        }

        private void LoadSubscribedTopics()
        {
            SubscribedTopics.Clear();

            string json = Preferences.Get("SubscribedTopics", string.Empty);
            if(!string.IsNullOrEmpty(json))
            {
                var topics = JsonSerializer.Deserialize<List<string>>(json);
                foreach (var topic in topics)
                {
                    SubscribedTopics.Add(topic);

                }
            }
            else
            {
                SubscribedTopics.Add("pazipro1920");
                SaveSubscribedTopics();
            }
        }

        private void SaveSubscribedTopics()
        {
            var topics = SubscribedTopics.ToList();
            string json = JsonSerializer.Serialize(topics);
            Preferences.Set("SubscribedTopics", json);
        }
        private async Task SubscribeToTopic()
        {
            if (!string.IsNullOrWhiteSpace(NewTopic))
            {
                var topic = NewTopic.Trim();
                if (!SubscribedTopics.Contains(topic))
                {
                    await _mqttService.SubscribeToTopic(topic);
                    SubscribedTopics.Add(topic);
                    NewTopic = string.Empty;

                    SaveSubscribedTopics();
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Info", $"Already subscribed to '{topic}'.", "OK");
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Please enter a valid topic.", "OK");
            }
        }

        private async Task UnsubscribeFromTopic(string topic)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert("Unsubscribe", $"Are you sure you want to unsubscribe from '{topic}'?", "Yes", "No");
            if (confirm)
            {
                await _mqttService.UnsubscribeFromTopic(topic);
                SubscribedTopics.Remove(topic);

                SaveSubscribedTopics();
            }
        }


        // Implement INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
