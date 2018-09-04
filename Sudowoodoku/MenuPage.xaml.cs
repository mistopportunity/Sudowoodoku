using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.System;
using System.Windows;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Popups;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Sudowoodoku {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MenuPage:Page {
		public MenuPage() {
			this.InitializeComponent();
		}

		protected async override void OnNavigatedTo(NavigationEventArgs e) {
			var app = Application.Current as App;
			if(!app.AttemptedSignIn) {
				await app.SignIn();
			}
			if(!app.StillSignedIn) {
				xboxSignInButton.Visibility = Visibility.Visible;
			}

		}

		private async void Button_Click(object sender,RoutedEventArgs e) {
			if(seedInput.Text != string.Empty) {
				if(uint.TryParse(seedInput.Text,out uint seed)) {

					Frame.Navigate(typeof(GamePage),(int)seed);
				} else {

					var messageDialog = new MessageDialog(
						"A level number should be between 0 and 4,294,967,295! (no symbols)","Whoopsies!"
					) {
						DefaultCommandIndex = 0,
						CancelCommandIndex = 0
					};

					messageDialog.Commands.Add(new UICommand("Okay, I'm sorry! Please forgive me!"));

					await messageDialog.ShowAsync();



				}
			} else {
				Frame.Navigate(typeof(GamePage),(new Random()).Next());

			}
		}

		private async void xboxSignInButton_Click(object sender,RoutedEventArgs e) {
			var app = Application.Current as App;
			await app.SignIn();
			if(app.StillSignedIn) {
				xboxSignInButton.Visibility = Visibility.Collapsed;
				xboxSignInButton.IsEnabled = true;
			}
		}
	}
}
