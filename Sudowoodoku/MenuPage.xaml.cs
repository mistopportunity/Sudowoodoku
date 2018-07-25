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

		protected override void OnNavigatedTo(NavigationEventArgs e) {
			base.OnNavigatedTo(e);
			Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
			base.OnNavigatingFrom(e);
			Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
		}

		private void CoreWindow_KeyDown(CoreWindow sender,KeyEventArgs args) {

			switch(args.VirtualKey) {

				case VirtualKey.GamepadDPadDown:
				case VirtualKey.GamepadDPadRight:
				case VirtualKey.Right:
				case VirtualKey.Down:


					FocusManager.TryMoveFocus(FocusNavigationDirection.Previous);

					break;
				case VirtualKey.GamepadDPadLeft:
				case VirtualKey.GamepadDPadUp:
				case VirtualKey.Left:
				case VirtualKey.Up:

					FocusManager.TryMoveFocus(FocusNavigationDirection.Next);

					break;

			}
			
		}

		private async void Button_Click(object sender,RoutedEventArgs e) {
			if(seedInput.Text != string.Empty) {
				if(uint.TryParse(seedInput.Text,out uint seed)) {

					Frame.Navigate(typeof(MainPage),(int)seed);
				} else {

					var messageDialog = new MessageDialog(
						"A seed should be a number between 0 and 4,294,967,295! (no symbols)","Whoopsies!"
					) {
						DefaultCommandIndex = 0,
						CancelCommandIndex = 0
					};

					messageDialog.Commands.Add(new UICommand("Okay, I'm sorry! Please forgive me!"));

					await messageDialog.ShowAsync();



				}
			} else {
				Frame.Navigate(typeof(MainPage),(new Random()).Next());

			}
		}
	}
}
