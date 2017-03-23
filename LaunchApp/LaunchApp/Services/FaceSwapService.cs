using LaunchApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LaunchApp.Services
{
    public class FaceSwapService
    {
        private readonly HttpClient httpClient = new HttpClient();

        public async Task<string> PostAsync(FaceSwapDetails details)
        {
            var response = new HttpResponseMessage();
            var responseBody = string.Empty;
            var bodyContent = new StringContent(JsonConvert.SerializeObject(details));

            try
            {
                response = await httpClient.PostAsync("https://launchatl.azurewebsites.net/api/face", bodyContent);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();

            }
            catch (Exception up)
            {
                responseBody = "Error: " + up.HResult.ToString("X") + " Message: " + up.Message;
            }

            return responseBody;
        }
    }
}
