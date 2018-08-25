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
	public sealed partial class SudokuNumber:UserControl {
		public SudokuNumber() {
			this.InitializeComponent();
		}
		private bool isReadOnly = false;
		public bool IsReadOnly {
			get {
				return isReadOnly;
			}
			set {
				isReadOnly = value;
				if(isReadOnly) {
					readOnlyBackground = new SolidColorBrush(Color.FromArgb(180,0,0,0));
					updateBorder(false);
				} else {
					readOnlyBackground = null;
					updateBorder(false);
				}
			}
		}
		private int number = 0;
		private void updateNumber() {
			if(number == 0) {
				textBlock.Text = string.Empty;
			} else {
				textBlock.Text = number.ToString();
			}
		}

		private SolidColorBrush readOnlyBackground;

		public int Number {
			get {
				return number;
			}
			set {
				number = value;
				updateNumber();
			}
		}

		private bool otherBlockSelected = false;
		internal int BlockIndex {
			get;set;
		}

		private bool selected = false;
		private bool hasCursor = false;

		public void Select() {
			selected = true;
			updateBorder(true);
		}

		public void Deselect() {
			selected = false;
			if(!hasCursor) {
				updateBorder(false);
			}
		}

		public void ExternalFocusChanged(bool bySelect) {
			if(selected)
				return;
			if(bySelect) {
				otherBlockSelected = true;
				updateBorder(false);
			} else {
				otherBlockSelected = false;
				if(hasCursor) {
					updateBorder(true);
					sendSoftSelect();
				}
			}
		}

		private void updateBorder(bool showBorder) {
			if(showBorder) {
				backgroundGrid.Background = new SolidColorBrush(Color.FromArgb(255,0,0,0));
			} else {
				backgroundGrid.Background = readOnlyBackground;
			}
		}

		private void UserControl_PointerEntered(object sender,PointerRoutedEventArgs e) {
#if mobiletest
			return;
#endif
			SoftSelect();
			if(!otherBlockSelected) {
				sendSoftSelect();
			}
		}

		private void UserControl_PointerExited(object sender,PointerRoutedEventArgs e) {
#if mobiletest
			return;
#endif
			SoftDeselect();
		}


		private void sendSoftSelect() {
			var frame = Window.Current.Content as Frame;
			var page = frame.Content as GamePage;
			page.UpdateSoftSelected(this);
		}


		public void SoftSelect() {
			if(!selected && !otherBlockSelected) {
				updateBorder(true);
			}
			hasCursor = true;
		}

		public void SoftDeselect() {
			if(!selected && !otherBlockSelected) {
				updateBorder(false);
			}
			hasCursor = false;
		}

		private void clickTrigger() {
			var frame = Window.Current.Content as Frame;
			var page = frame.Content as GamePage;
			page.BlockTapped(this);
		}

		private void UserControl_PointerMoved(object sender,PointerRoutedEventArgs e) {
			if(!selected && !otherBlockSelected && !hasCursor) {
				sendSoftSelect();
				updateBorder(true);
			}
			hasCursor = true;
		}

		private void UserControl_PointerPressed(object sender,PointerRoutedEventArgs e) {
			clickTrigger();
		}

	}
}
