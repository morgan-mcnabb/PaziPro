using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;

namespace PaziPro.ViewModels
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        // WiFi Settings
        private string _ssid;
        private string _password;
        private bool _isPassword = true;
        private string _eyeIcon = "eye_on.png";

        // MQTT Settings
        private string _mqttServer;
        private string _mqttUser;
        private string _mqttPassword;
        private bool _isMqttPassword = true;
        private string _mqttEyeIcon = "eye_on.png";
        private string _mqttTopic;

        // Commands
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand ToggleMqttPasswordVisibilityCommand { get; }

        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsViewModel()
        {
            TogglePasswordVisibilityCommand = new Command(TogglePasswordVisibility);
            ToggleMqttPasswordVisibilityCommand = new Command(ToggleMqttPasswordVisibility);
        }

        // WiFi Properties
        public string Ssid
        {
            get => _ssid;
            set
            {
                if (_ssid != value)
                {
                    _ssid = value;
                    OnPropertyChanged(nameof(Ssid));
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        public bool IsPassword
        {
            get => _isPassword;
            set
            {
                if (_isPassword != value)
                {
                    _isPassword = value;
                    OnPropertyChanged(nameof(IsPassword));
                }
            }
        }

        public string EyeIcon
        {
            get => _eyeIcon;
            set
            {
                if (_eyeIcon != value)
                {
                    _eyeIcon = value;
                    OnPropertyChanged(nameof(EyeIcon));
                }
            }
        }

        // MQTT Properties
        public string MqttServer
        {
            get => _mqttServer;
            set
            {
                if (_mqttServer != value)
                {
                    _mqttServer = value;
                    OnPropertyChanged(nameof(MqttServer));
                }
            }
        }

        public string MqttUser
        {
            get => _mqttUser;
            set
            {
                if (_mqttUser != value)
                {
                    _mqttUser = value;
                    OnPropertyChanged(nameof(MqttUser));
                }
            }
        }

        public string MqttPassword
        {
            get => _mqttPassword;
            set
            {
                if (_mqttPassword != value)
                {
                    _mqttPassword = value;
                    OnPropertyChanged(nameof(MqttPassword));
                }
            }
        }

        public bool IsMqttPassword
        {
            get => _isMqttPassword;
            set
            {
                if (_isMqttPassword != value)
                {
                    _isMqttPassword = value;
                    OnPropertyChanged(nameof(IsMqttPassword));
                }
            }
        }

        public string MqttEyeIcon
        {
            get => _mqttEyeIcon;
            set
            {
                if (_mqttEyeIcon != value)
                {
                    _mqttEyeIcon = value;
                    OnPropertyChanged(nameof(MqttEyeIcon));
                }
            }
        }

        public string MqttTopic
        {
            get => _mqttTopic;
            set
            {
                if (_mqttTopic != value)
                {
                    _mqttTopic = value;
                    OnPropertyChanged(nameof(MqttTopic));
                }
            }
        }

        // Methods
        private void TogglePasswordVisibility()
        {
            IsPassword = !IsPassword;
            EyeIcon = IsPassword ? "eye_on.png" : "eye_off.png";
        }

        private void ToggleMqttPasswordVisibility()
        {
            IsMqttPassword = !IsMqttPassword;
            MqttEyeIcon = IsMqttPassword ? "eye_on.png" : "eye_off.png";
        }

        // PropertyChanged Implementation
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
