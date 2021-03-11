using AssemblyAIWantATranscription.Helpers;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AssemblyAIWantATranscription.Services
{
    public class TranscribeService
	{
		private readonly HttpClient _httpClient;
		private readonly string errorMessage = "Failed to fetch transcription. Please try again.";
		public TranscribeService()
		{
			_httpClient = new HttpClient();
			_httpClient.BaseAddress = new Uri("https://api.assemblyai.com/v2/");
			_httpClient.DefaultRequestHeaders.Add("authorization", Secrets.AssemblyAIApiToken);
			//_httpClient.DefaultRequestHeaders.Add("Transer-Encoding", "chunked");
		}

		public async Task<string> TranscribeAudio(Stream fileStream)
		{		
			var url = await UploadFIle(fileStream);

			if (!String.IsNullOrEmpty(url))
			{
				var id = await FetchId(url);
				if(id is not null)
                {
					Thread.Sleep(5000);
					return await FetchTranscription(id);
				}
                else
                {
					return errorMessage;
                }
			}
			else
			{
				return errorMessage;
			}			
		}

        private async Task<string> UploadFIle(Stream fileStream)
        {
			HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "upload");
			request.Headers.Add("Transfer-Encoding", "chunked");

			var streamContent = new StreamContent(fileStream);
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
				return string.Empty;
            }	
		}

		private async Task<string> FetchId(string url)
		{
			var json = new
			{
				audio_url = url
			};

			try
            {
				StringContent payload = new StringContent(JsonConvert.SerializeObject(json), Encoding.UTF8, "application/json");
				HttpResponseMessage response = await _httpClient.PostAsync("https://api.assemblyai.com/v2/transcript", payload);
				response.EnsureSuccessStatusCode();

				var responseJson = await response.Content.ReadAsStringAsync();

				var jsonResult = JsonConvert.DeserializeObject<UploadResponse>(responseJson);
				return jsonResult.id;
			} catch (Exception ex)
            {
				Console.WriteLine($"Error: {ex.Message}");
				return null;
            }
		
		}

		private async Task<string> FetchTranscription(string id)
        {
			var uploadResponse = await WaitForSuccess($"https://api.assemblyai.com/v2/transcript/{id}", 30000);

			if(uploadResponse is not null)
            {
				return uploadResponse.text;
			}
			return errorMessage;
		}

		public async Task<UploadResponse> WaitForSuccess(string url, int timeout)
		{
			bool shouldContinue = true;
			HttpResponseMessage response;
			UploadResponse uploadResponse = null;
			var successTask = Task.Run(async () => {
				var isSuccess = false;
				while (!isSuccess)
				{
					if (!shouldContinue)
						break;
					response = await _httpClient.GetAsync(url);
					response.EnsureSuccessStatusCode();					

					try
                    {
						var responseJson = await response.Content.ReadAsStringAsync();
						uploadResponse = JsonConvert.DeserializeObject<UploadResponse>(responseJson);
					}
					catch (Exception ex)
					{
						Console.WriteLine($"Error: {ex.Message}");
					}

					if (!uploadResponse.status.Equals("completed")) { isSuccess = false; }
					else
					{
						isSuccess = true;
					}

					if (!isSuccess)
						await Task.Delay(1000);
					else
						return true;
				}
				return isSuccess;
			});
			var result = await Task.WhenAny(successTask, Task.Delay(timeout));
			shouldContinue = false;
			if (result == successTask)
			{
				return uploadResponse;
			}
			else
            {
				return uploadResponse;
			}
		}

	}

	
}
