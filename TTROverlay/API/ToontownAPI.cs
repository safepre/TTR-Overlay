
using System.Net.Http;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;


namespace TTROverlay.API
{
    public class ToontownAPI
    {
        private readonly HttpClient httpClient;
        private const string url = "http://localhost:1547/info.json";
        private readonly string session = "TTR";
        private bool isRunning = false;
        private APIData latestData;
        public event EventHandler<APIData> DataUpdated;

        public ToontownAPI()
        {
            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Host", "localhost:1547");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "TTROverlay/1.0");
            httpClient.DefaultRequestHeaders.Add("Authorization", session);
        }

        public async Task StartConnection(PictureBox btn)
        {
            isRunning = true;
            while (isRunning)
            {
                try
                {
                    var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync();
                    btn.Image = Properties.Resources.ConnectedButtonNormal;
                    var json = JObject.Parse(content);
                    latestData = new APIData
                    {
                        Name = json["toon"]["name"].ToString(),
                        Species = json["toon"]["species"].ToString(),
                        CurrentLaff = json["laff"]["current"].ToString(),
                        Zone = json["location"]["zone"].ToString(),
                        Neighborhood = json["location"]["neighborhood"].ToString(),
                        District = json["location"]["district"].ToString(),
                    };
                    DataUpdated?.Invoke(this, latestData);

                    await Task.Delay(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred: {ex.Message}");
                    MessageBox.Show("Error occurred, please try again later");
                    Application.Exit();
                }
            }
        }

        public void StopConnection()
        {
            isRunning = false;
        }
    }

}
