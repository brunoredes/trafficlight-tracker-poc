using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading;
using System.Windows.Input;
using TrafficLight.Services;

namespace TrafficLight.ViewModels
{
    public partial class TrafficLightViewModel : ObservableObject, IDisposable
    {
        private readonly SseEventService _sseEventService;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly string sseEndpoint = "http://localhost:3000/sse";

        [ObservableProperty]
        private string _trafficLightColor;

        public TrafficLightViewModel()
        {
            _sseEventService = new SseEventService() ;
            _sseEventService.MessageReceived += OnMessageReceived;
        }

        public ICommand StartListeningCommand => new Command(async () => await StartListeningAsync());

        private async Task StartListeningAsync()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            try
            {
                await _sseEventService.StartListening(sseEndpoint, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                Dispose();
            }
        }

        private void OnMessageReceived(object sender, string message)
        {
            TrafficLightColor = message;
            // Parse the received message and update the TrafficLightColor property
            //if (message.StartsWith("data:\""))
            //{
            //    var color = message.Substring(6, message.Length - 7); // Extract color from message
            //    TrafficLightColor = color;
            //}
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
