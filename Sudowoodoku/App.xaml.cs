using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.Xbox.Services.System;
using Microsoft.Xbox.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.System;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.Networking.Connectivity;
using Windows.UI.Popups;

namespace Sudowoodoku {
	sealed partial class App:Application {

		internal XboxLiveUser primaryUser;
		private XboxLiveContext xboxLiveContext;
		public bool AttemptedSignIn = false;
		public bool StillSignedIn = false;
		public EventHandler SignedOut;

		public async Task SignIn() {
			AttemptedSignIn = true;

			ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
			NetworkConnectivityLevel connectionLevel;
			if(InternetConnectionProfile != null) {
				connectionLevel = InternetConnectionProfile.GetNetworkConnectivityLevel();
			} else {
				connectionLevel = NetworkConnectivityLevel.None;
			}

			switch(connectionLevel) {
				case NetworkConnectivityLevel.InternetAccess:
					break;
				default:
					var dialog = new MessageDialog("There is no internet connection. Try again later.","Couldn't sign in") {
						DefaultCommandIndex = 0,
						CancelCommandIndex = 0
					};
					dialog.Commands.Add(new UICommand("Okay"));
					await dialog.ShowAsync();
					return;
			}


			bool signedIn = false;
			IReadOnlyList<User> users = await User.FindAllAsync();
			CoreDispatcher UIDispatcher = Window.Current.CoreWindow.Dispatcher;
			try {
				primaryUser = new XboxLiveUser(users[0]);
				SignInResult signInSilentResult = await primaryUser.SignInSilentlyAsync(UIDispatcher);
				switch(signInSilentResult.Status) {
					case SignInStatus.Success:
						signedIn = true;
						break;
					case SignInStatus.UserInteractionRequired:
						SignInResult signInLoud = await primaryUser.SignInAsync(UIDispatcher);
						if(signInLoud.Status == SignInStatus.Success) {
							signedIn = true;
						}
						break;
					default:
						break;
				}
				if(signedIn) {
					xboxLiveContext = new XboxLiveContext(primaryUser);
					StillSignedIn = true;
					XboxLiveUser.SignOutCompleted += OnSignOut;
				}
			} catch(Exception exception) {
				Debug.WriteLine(exception.Message);
			}
		}

		public void OnSignOut(object sender,SignOutCompletedEventArgs e) {
			primaryUser = null;
			xboxLiveContext = null;
			StillSignedIn = false;
			var rootFrame = Window.Current.Content as Frame;
			if(rootFrame != null) {
				if(rootFrame.Content is GamePage) {
					(rootFrame.Content as GamePage).WasSignedOutExternally();
				} else if(rootFrame.Content is MenuPage) {
					(rootFrame.Content as MenuPage).WasSignedOutExternally();
				}
			}
		}

		public App() {
			InitializeComponent();
			this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e) {
			Frame rootFrame = Window.Current.Content as Frame;
			if(rootFrame == null) {
				rootFrame = new Frame();
				rootFrame.NavigationFailed += OnNavigationFailed;
				ElementSoundPlayer.State = ElementSoundPlayerState.On;
				Window.Current.Content = rootFrame;
			}
			if(e.PrelaunchActivated == false) {
				if(rootFrame.Content == null) {
					rootFrame.Navigate(typeof(MenuPage),e.Arguments);
				}
				Window.Current.Activate();
			}
		}

		void OnNavigationFailed(object sender,NavigationFailedEventArgs e) {
			throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
		}
	}
}
