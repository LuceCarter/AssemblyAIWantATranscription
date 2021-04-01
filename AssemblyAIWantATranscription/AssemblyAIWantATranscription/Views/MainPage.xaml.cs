using AssemblyAIWantATranscription.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AssemblyAIWantATranscription.Views
{
    public partial class MainPage : ContentPage
    {
        public MainPageViewModel viewModel;
        public MainPage()
        {
            InitializeComponent();
            BindingContext = viewModel = new MainPageViewModel();
        }

        private void RecordButton_Clicked(object sender, EventArgs e)
        {
            RecordButton.PlayAnimation();
            RecordButton.RepeatCount = 5;
            viewModel.RecordButtonCommand.ExecuteAsync();
        }
    }
}
