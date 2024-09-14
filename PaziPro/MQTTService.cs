using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;

namespace PaziPro
{
    public class MQTTService
    {
        private IMqttClient _mqttClient;
        private HashSet<string> _subscribedTopics = [];
        private string _server;
        private Action<bool> _updateConnectionStatus;
        private Action<string, string> _displayReceivedMessage;

        public MQTTService() { }

        public void Configure(Action<bool> updateConnectionStatus, Action<string, string> displayReceivedMessage)
        {
            _updateConnectionStatus = updateConnectionStatus;
            _displayReceivedMessage = displayReceivedMessage;
        }
        public bool IsConnected => _mqttClient != null && _mqttClient.IsConnected;

        public async Task Connect_Client(string mqttServer, string mqttUser, string mqttPassword)
        {
            _server = mqttServer;
            var mqttFactory = new MqttFactory();

            _mqttClient = mqttFactory.CreateMqttClient();

            try
            {
                var mqttClientOptions = new MqttClientOptionsBuilder()
                    .WithTcpServer(_server)
                    .WithCredentials(mqttUser, mqttPassword)
                    .WithCleanSession()
                    .Build();

                _mqttClient.ConnectedAsync += async e =>
                {
                    _updateConnectionStatus?.Invoke(true);
                };

                _mqttClient.DisconnectedAsync += async e =>
                {
                    _updateConnectionStatus?.Invoke(false);
                };

                _mqttClient.ApplicationMessageReceivedAsync += HandleReceivedApplicationMessage;

                await _mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);
                Console.WriteLine("Connected");
                _updateConnectionStatus?.Invoke(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"There was an error connecting to MQTT: {ex.Message}");
                _displayReceivedMessage?.Invoke("ERROR", "Error connecting to MQTT. Check Settings.");
                _updateConnectionStatus?.Invoke(false);
            }

            Console.WriteLine("The MQTT client is connected.");

        }

        public async Task SubscribeToTopic(string topic)
        {
            if(_mqttClient == null || !_mqttClient.IsConnected)
                return;

            if(_subscribedTopics.Contains(topic))
                return;

            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                .WithTopic(topic)
                .Build());

            _subscribedTopics.Add(topic);

            /*


            if (!_subscribedTopics.Contains(topic))
            {
                if (_mqttClient == null || !_mqttClient.IsConnected)
                {
                    Console.WriteLine("MQTT client is not connected. Cannot subscribe to topic.");
                    return;
                }

                //_mqttClient.ApplicationMessageReceivedAsync -= HandleReceivedApplicationMessage;

                await _mqttClient.SubscribeAsync(new MqttClientSubscribeOptionsBuilder()
                    .WithTopicFilter(f => { f.WithTopic(topic); })
                    .Build());


                //_mqttClient.ApplicationMessageReceivedAsync += HandleReceivedApplicationMessage;

                Console.WriteLine($"Subscribed to topic {topic}");
                _subscribedTopics.Add(topic);
            }
            */
        }

        public async Task UnsubscribeFromTopic(string topic)
        {
            if(_mqttClient == null || !_mqttClient.IsConnected)
                return;

            if(!_subscribedTopics.Contains(topic))
                return;

            await _mqttClient.UnsubscribeAsync(topic);
            _subscribedTopics.Remove(topic);
        }

        public IEnumerable<string> GetSubscribedTopics() { return _subscribedTopics; }


        private async Task HandleReceivedApplicationMessage(MqttApplicationMessageReceivedEventArgs e)
        {
            string payload = System.Text.Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
            string topic = e.ApplicationMessage.Topic;
            Console.WriteLine($"Message received on topic {e.ApplicationMessage.Topic}: {payload}");
            _displayReceivedMessage?.Invoke(topic, payload);
            await Task.CompletedTask;
        }
    }
}
