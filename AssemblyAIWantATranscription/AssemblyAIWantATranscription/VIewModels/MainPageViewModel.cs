using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using MvvmHelpers;
using Xamarin.Forms;

namespace AssemblyAIWantATranscription.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        


        public MainPageViewModel()
        {
        }

        private string _transcribedTextLabel = "";
        public string TranscribedTextLabel
        {
            get => _transcribedTextLabel;
            set => SetProperty(ref _transcribedTextLabel, value);
        }

        public ICommand RecordButtonCommand
        {
            get
            {
                return new Command(() =>
                {
                    TranscribedTextLabel = "Button was pressed!";
                });
            }
        }

        private void RecordAudio()
        {
            Console.WriteLine("I recorded Audio!");
        }
    }
}
