using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AssemblyAIWantATranscription.Helpers;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace AssemblyAIWantATranscription.Services
{
	public class TranscribeService
	{
		private readonly HttpClient _httpClient;
		public TranscribeService()
		{
			_httpClient = new HttpClient();
			_httpClient.BaseAddress = new Uri("https://api.assemblyai.com/v2/");
			_httpClient.DefaultRequestHeaders.Add("authorization", Secrets.AssemblyAIApiToken);
			
		}

		public async Task TranscribeAudio(string filePath)
		{		
			var url = await UploadFIle(filePath);
			var id = await Transcribe(url);
			Thread.Sleep(5000);

			var text = await Download(id);
		}

        private async Task<string> UploadFIle(string filePath)
        {
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "upload");
			_httpClient.BaseAddress = new Uri("https://api.assemblyai.com/v2/");
			_httpClient.DefaultRequestHeaders.Add("Transer-Encoding", "chunked");

			var file = await FilePicker.PickAsync();
			var file2 = await file.OpenReadAsync();

			var streamContent = new StreamContent(file2);
			request.Content = streamContent;

			try
            {
				HttpResponseMessage response = await _httpClient.SendAsync(request);
				var contentResponse = await response.Content.ReadAsStringAsync();
				var jsonResult =  JsonConvert.DeserializeObject<UploadResponse>(contentResponse);
				return jsonResult.upload_url;
			} catch (Exception ex)
            {
				Console.WriteLine($"Error: {ex.Message}");
				return "";
            }	
		}

		private async Task<string> Transcribe(string url)
		{
			var json = new
			{
				audio_url = url
			};

			StringContent payload = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
			HttpResponseMessage response = await _httpClient.PostAsync("https://api.assemblyai.com/v2/transcript", payload);
			response.EnsureSuccessStatusCode();

			var responseJson = await response.Content.ReadAsStringAsync();

			var jsonResult = JsonConvert.DeserializeObject<UploadResponse>(responseJson);
			return jsonResult.id;
		}

		private async Task<string> Download(string id)
        {
			HttpResponseMessage response = await _httpClient.GetAsync($"https://api.assemblyai.com/v2/transcript/{id}");
			response.EnsureSuccessStatusCode();

			var responseJson = await response.Content.ReadAsStringAsync();
			var jsonResult = JsonConvert.DeserializeObject<UploadResponse>(responseJson);
			return jsonResult.text;
		}

	}

	public class UploadResponse
    {
		public string upload_url { get; set; }
		public string id { get; set; }

		public string text { get; set; }
    }

	
}
