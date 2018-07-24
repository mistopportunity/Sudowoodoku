using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.Gaming.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace Sudowoodoku {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage:Page {

		private readonly List<SudokuNumber> sudokuBlocks;

		private void allocateSudokuBlocks() {

			var topGrid = SudokuBoard.Children;

			for(int x = 0;x<9;x++) {

				var lowerGrid = ((Grid)topGrid[x]).Children;

				for(int y = 0;y<9;y++) {

					var block = (SudokuNumber)((Grid)lowerGrid[y]).Children[0];

					block.BlockIndex = (x * 9) + y;

					sudokuBlocks.Add(block);

				}
			}
		}

		public MainPage() {
			this.InitializeComponent();

			sudokuBlocks = new List<SudokuNumber>();

			allocateSudokuBlocks();

			Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;

		}

		private void CoreWindow_KeyUp(CoreWindow sender,KeyEventArgs args) {
			switch(args.VirtualKey) {
				case VirtualKey.GamepadDPadLeft:
				case VirtualKey.Left:
					Navigate(-1,0);
					break;
				case VirtualKey.GamepadDPadRight:
				case VirtualKey.Right:
					Navigate(1,0);
					break;
				case VirtualKey.GamepadDPadUp:
				case VirtualKey.Up:
					Navigate(0,-1);
					break;
				case VirtualKey.GamepadDPadDown:
				case VirtualKey.Down:
					Navigate(0,1);
					break;
				case VirtualKey.Enter:
					if(selectedBlock == null) {
						BlockTapped(softSelected);
					} else {
						BlockTapped(selectedBlock);
					}
					break;
			}
		}

		private void Navigate(int xDelta,int yDelta) {
			if(selectedBlock != null) {
				return;
			}
			if(softSelected == null) {
				softSelected = sudokuBlocks.First();
				softSelected.SoftSelect();
			} else {


				var virtualIndex = GetVirtualIndex(
					softSelected.BlockIndex
				);

				var newX = virtualIndex.Item1;
				var newY = virtualIndex.Item2;

				if(xDelta < 0) {
					if(virtualIndex.Item1 + xDelta >= 0) {
						newX += xDelta;
					}
				} else if(xDelta > 0) {
					if(virtualIndex.Item1 + xDelta <= 8) {
						newX += xDelta;
					}
				}

				if(yDelta < 0) {

					if(virtualIndex.Item2 + yDelta >= 0) {
						newY += yDelta * 9;
					}

				} else if(yDelta > 0) {

					if(virtualIndex.Item2 + yDelta <= 8) {
						newY += yDelta * 9;
					}

				}

				if(newX != virtualIndex.Item1 || newY != virtualIndex.Item2) {
					var premultiplied = GetPremultiplied(newX,newY);

					softSelected.SoftDeselect();

					softSelected = sudokuBlocks[
						premultiplied
					];
					softSelected.SoftSelect();
				}
			}
		}

		//Top 1 by bottom 3 returns a 2 in this example

		//  1 1 2   1 1 1   1 1 1
		//  1 1 1   1 1 1   1 1 1
		//  1 1 1   1 1 1   1 1 1
		//
		//  1 1 1   1 1 1   1 1 1
		//  1 1 1   1 1 1   1 1 1
		//  1 1 1   1 1 1   1 1 1
		//
		//  1 1 1   1 1 1   1 1 1
		//  1 1 1   1 1 1   1 1 1
		//  1 1 1   1 1 1   1 1 1

		internal int Premultiply(int x,int y) {
			return (x * 9) + y;
		}

		internal SudokuNumber GetSudokuBlock(int top,int bottom) {
			return sudokuBlocks[(top * 9) + bottom];
		}

		internal Tuple<int,int> GetSudokuIndexes(int premultiplied) {

			return new Tuple<int,int>(
				premultiplied / 9,premultiplied % 9
			);
		}

		internal Tuple<int,int> GetVirtualIndex(int premultiplied) {

		}

		internal int GetPremultiplied(int virtualIndexX,int virtualIndexY) {


		}

		private SudokuNumber selectedBlock = null;

		private SudokuNumber softSelected = null;

		internal void BlockTapped(SudokuNumber block) {

			var isNull = selectedBlock == null;
			if(isNull) {
				selectedBlock = block;
				selectedBlock.Select();
			} else {
				selectedBlock.Deselect();
				selectedBlock = null;
			}
			foreach(var otherBlock in sudokuBlocks) {
				otherBlock.ExternalFocusChanged(bySelect: isNull);
			}

		}

		internal void UpdateSoftSelected(SudokuNumber block) {

			if(softSelected == null) {
				softSelected = block;
				return;
			}

			softSelected.SoftDeselect();
			softSelected = block;

		}

	}
}
