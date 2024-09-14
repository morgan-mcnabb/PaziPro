using System.Collections.ObjectModel;

namespace PaziPro
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManageSubscriptionsPage : ContentPage
    {
        private readonly MQTTService _MQTTService;
        public ObservableCollection<string> SubscribedTopics { get; set; } = new ObservableCollection<string>();

        public ManageSubscriptionsPage(MQTTService MQTTService)
        {
            InitializeComponent();
            _MQTTService = MQTTService;

            // Load subscribed topics
            LoadSubscribedTopics();

            subscriptionsCollectionView.ItemsSource = SubscribedTopics;
        }

        private void LoadSubscribedTopics()
        {
            SubscribedTopics.Clear();
            foreach (var topic in _MQTTService.GetSubscribedTopics())
            {
                SubscribedTopics.Add(topic);
            }
        }

        private async void OnSubscribeClicked(object sender, EventArgs e)
        {
            string newTopic = newTopicEntry.Text?.Trim();

            if (!string.IsNullOrEmpty(newTopic))
            {
                await _MQTTService.SubscribeToTopic(newTopic);
                SubscribedTopics.Add(newTopic);
                newTopicEntry.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Error", "Please enter a valid topic.", "OK");
            }
        }

        private async void OnUnsubscribeSwiped(object sender, EventArgs e)
        {
            SwipeItem swipeItem = sender as SwipeItem;
            string topic = swipeItem.BindingContext as string;

            bool confirm = await DisplayAlert("Unsubscribe", $"Are you sure you want to unsubscribe from '{topic}'?", "Yes", "No");
            if (confirm)
            {
                await _MQTTService.UnsubscribeFromTopic(topic);
                SubscribedTopics.Remove(topic);
            }
        }
    }
}