using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Sudowoodoku {
	public sealed partial class NumberSelector:UserControl {
		public NumberSelector() {
			this.InitializeComponent();
		}
		private int number = 0;
		private void updateNumber() {
			if(number == 0) {
				textBlock.Text = string.Empty;
			} else {
				textBlock.Text = number.ToString();
			}
		}
		public int Number {
			get {
				return number;
			}
			set {
				number = value;
				updateNumber();
			}
		}
		private void updateBorder(bool showBorder) {
			if(showBorder) {
				backgroundGrid.Background = new SolidColorBrush(Color.FromArgb(255,0,0,0));
			} else {
				backgroundGrid.Background = null;
			}
		}

		private void pushSelectionUpdate() {
			var frame = Window.Current.Content as Frame;
			var page = frame.Content as GamePage;

			page.SwapNumberSelection(this);

			updateBorder(true);
		}

		private void UserControl_PointerEntered(object sender,PointerRoutedEventArgs e) {

			pushSelectionUpdate();

		}

		private void UserControl_PointerMoved(object sender,PointerRoutedEventArgs e) {
			if(backgroundGrid.Background == null) {
				pushSelectionUpdate();
			}
		}

		public void Select() {
			updateBorder(showBorder: true);
		}

		public void Deselect() {
			updateBorder(showBorder: false);
		}

		private void UserControl_PointerReleased(object sender,PointerRoutedEventArgs e) {
			var frame = Window.Current.Content as Frame;
			var page = frame.Content as GamePage;
			page.NumberTapped(number);
		}
	}
}
