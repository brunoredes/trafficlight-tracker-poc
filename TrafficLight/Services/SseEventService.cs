namespace TrafficLight.Services
{
    public sealed class SseEventService
    {
        private readonly HttpClient _httpClient;

        public event EventHandler<string> MessageReceived;

        public SseEventService()
        {
            _httpClient = new HttpClient();
        }

        public async Task StartListening(string url, CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("text/event-stream"));

            using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            var stream = await response?.Content.ReadAsStreamAsync();
            var reader = new StreamReader(stream);

            string eventName = null;
            string eventData = null;

            while (!cancellationToken.IsCancellationRequested)
            {
                var line = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(line))
                {
                    continue;
                }

                if (line.StartsWith("retry:"))
                {
                    // Ignore retry line
                    continue;
                }
                else if (line.StartsWith("event:"))
                {
                    eventName = line.Substring(6).Trim();
                }
                else if (line.StartsWith("data:"))
                {
                    eventData = line.Substring(5).Trim();
                }
                else if (line == ":") // Empty line signifies the end of an event
                {
                    if (eventName == "trafficLightCurrentColor" && !string.IsNullOrEmpty(eventData))
                    {
                        // Invoke event handler with the data
                        MessageReceived?.Invoke(this, eventData);
                    }

                    // Reset event name and data for the next event
                    eventName = null;
                    eventData = null;
                }
            }
        }
    }
}
