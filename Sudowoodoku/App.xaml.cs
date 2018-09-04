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

namespace Sudowoodoku {
	sealed partial class App:Application {

		private XboxLiveUser primaryUser;
		private XboxLiveContext xboxLiveContext;
		public bool AttemptedSignIn = false;
		public bool StillSignedIn = false;

		public EventHandler SignedOut;

		public async Task SignIn() {
			AttemptedSignIn = true;
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
		}

		public App() {
			InitializeComponent();
		}

		protected override void OnLaunched(LaunchActivatedEventArgs e) {
			Frame rootFrame = Window.Current.Content as Frame;
			if(rootFrame == null) {
				rootFrame = new Frame();
				rootFrame.NavigationFailed += OnNavigationFailed;
				Window.Current.Content = rootFrame;
				ElementSoundPlayer.State = ElementSoundPlayerState.On;
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
