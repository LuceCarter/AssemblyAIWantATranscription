using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using MvvmHelpers;
using Xamarin.Forms;
using Plugin.AudioRecorder;
using AssemblyAIWantATranscription.Helpers;
using Xamarin.Essentials;
using MvvmHelpers.Commands;
using System.Threading.Tasks;
using AssemblyAIWantATranscription.Services;

namespace AssemblyAIWantATranscription.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        private readonly AudioRecorderService audioRecorderService;
        private readonly AudioPlayer audioPlayer;
        private readonly TranscribeService transcribeService;
        
        public MainPageViewModel()
        {
            audioRecorderService = new AudioRecorderService();            
            // audioRecorderService.StopRecordingOnSilence = true;
            transcribeService = new TranscribeService();
        }

        private bool _isBusy = false;
        public bool IsBusy {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
         }

        private string _transcribedTextEditor = "Press Record to begin...";
        public string TranscribedTextEditor
        {
            get => _transcribedTextEditor;
            set => SetProperty(ref _transcribedTextEditor, value);
        }

        public AsyncCommand RecordButtonCommand
        {
            get
            {
                return new AsyncCommand(RecordAudio);
            }
        }

        private async Task RecordAudio()
        {
            // This was not in the video, we need to ask permission
            // for the microphone to make it work for Android, see https://youtu.be/uBdX54sTCP0
            var status = await Permissions.RequestAsync<Permissions.Microphone>();

            if (status != PermissionStatus.Granted)
                return;

            if (audioRecorderService.IsRecording)
            {
                await audioRecorderService.StopRecording();
                IsBusy = true;
                TranscribedTextEditor = "Transcription in progress...";

                try
                {
                    var transcription = await transcribeService.TranscribeAudio(audioRecorderService.GetAudioFileStream());
                    if(transcription.Length > 0)
                    {
                        TranscribedTextEditor = transcription;
                        IsBusy = false;
                    }
                } catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error!", ex.Message, "OK");
                }
            }
            else
            {
                    
                 await audioRecorderService .StartRecording();
            }
        }
    }
}
