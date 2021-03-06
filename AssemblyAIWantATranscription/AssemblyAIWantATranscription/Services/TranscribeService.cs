using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyAIWantATranscription.Services
{
    public class TranscribeService
    {
        private readonly HttpClient httpClient;

        public TranscribeService()
        {
            httpClient = new HttpClient();
        }

        public string TranscribeAudio(string filePath)
        {
            string jsonResult = SendFile(httpClient, filePath).Result;
            return jsonResult;
        }

        private static async Task<string> SendFile(HttpClient client, string filePath)
        {
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "upload");
                request.Headers.Add("Transer-Encoding", "chunked");

                var fileReader = System.IO.File.OpenRead(filePath);
                var streamContent = new StreamContent(fileReader);
                request.Content = streamContent;

                HttpResponseMessage response = await client.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }
    }
}
