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

		private void UpdateSignInButtonContent(App app) {
			if(xboxSignInButton.IsEnabled) {
				xboxSignInButton.Content = "sign into Xbox Live";
			} else {
				xboxSignInButton.Content = $"signed in as {app.primaryUser.Gamertag}";
			}
		}


		protected override void OnNavigatedTo(NavigationEventArgs e) {
			var app = Application.Current as App;
			Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);
			xboxSignInButton.IsEnabled = !app.StillSignedIn;
			UpdateSignInButtonContent(app);
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

		internal void WasSignedOutExternally() {
			xboxSignInButton.IsEnabled = true;
			UpdateSignInButtonContent(null);
		}

		private async void xboxSignInButton_Click(object sender,RoutedEventArgs e) {
			var app = Application.Current as App;
			xboxSignInButton.IsEnabled = false;
			StartButton.IsEnabled = false;
			xboxSignInButton.Content = "signing into Xbox Live...";
			await app.SignIn();
			StartButton.IsEnabled = true;
			xboxSignInButton.IsEnabled = !app.StillSignedIn;
			UpdateSignInButtonContent(app);
		}

		private void seedInput_KeyUp(object sender,KeyRoutedEventArgs e) {
			if(e.Key == VirtualKey.Enter) {
				FocusManager.TryMoveFocus(FocusNavigationDirection.Next);
			}
		}
	}
}
