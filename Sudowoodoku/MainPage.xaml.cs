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

		private readonly List<NumberSelector> numberBlocks;

		private void allocateSudokuBlocks() {

			var topGrid = SudokuBoard.Children;

			for(int x = 0;x<9;x++) {

				var lowerGrid = ((Grid)topGrid[x]).Children;

				for(int y = 0;y<9;y++) {

					var block = (SudokuNumber)((Grid)lowerGrid[y]).Children[0];

					block.BlockIndex = Premultiply(x,y);

					sudokuBlocks.Add(block);


				}
			}

			var numberButtons = NumberBar.Children;

			for(int i = 0;i<9;i++) {

				var numberButton = numberButtons[i] as NumberSelector;
				numberButton.Number = i + 1;

				numberBlocks.Add(numberButton);

			}
		}

		public MainPage() {
			this.InitializeComponent();

			sudokuBlocks = new List<SudokuNumber>();

			numberBlocks = new List<NumberSelector>();

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
				case VirtualKey.Escape:
				case VirtualKey.GamepadB:
					if(selectedBlock != null) {
						selectedBlock.Deselect();
						selectedBlock = null;
						hideNumberBar();
					}
					break;
				case VirtualKey.GamepadA:
				case VirtualKey.Enter:
					if(selectedBlock == null) {
						BlockTapped(softSelected);
					} else {
						NumberTapped(selectedNumber.Number);
					}
					break;
			}
		}

		private void Navigate(int xDelta,int yDelta) {
			if(selectedBlock != null) {
				if(yDelta != 0) {
					xDelta = yDelta;
				}
				if(xDelta < 0) {
					selectedNumber.Deselect();
					if(selectedNumber.Number - 1 < 1) {

						selectedNumber = numberBlocks[8];

					} else {
						selectedNumber = numberBlocks[selectedNumber.Number - 2];
					}
					selectedNumber.Select();
				} else if(xDelta > 0) {
					selectedNumber.Deselect();
					if(selectedNumber.Number > 8) {

						selectedNumber = numberBlocks[0];

					} else {
						selectedNumber = numberBlocks[selectedNumber.Number];
					}
					selectedNumber.Select();
				}
				return;
			}
			if(softSelected == null) {
				softSelected = sudokuBlocks[40];
				softSelected.SoftSelect();
			} else {

				var blockIndex = GetSudokuIndexes(softSelected.BlockIndex);

				var newIndex = softSelected.BlockIndex;


				if(xDelta < 0) {

					if(!(blockIndex.Item1 % 3 == 0 && blockIndex.Item2 % 3 == 0)) {
						if((blockIndex.Item2 % 3) - 1 >= 0) {

							newIndex -= 1;

						} else if(blockIndex.Item1 % 3 >= 0) {

							newIndex -= 7;

						}
					}


				} else if(xDelta > 0) {

					if(!(blockIndex.Item1 % 3 == 2 && blockIndex.Item2 % 3 == 2)) {
						if((blockIndex.Item2 % 3) + 1 <= 2) {

							newIndex += 1;

						} else if(blockIndex.Item1 % 3 <= 2) {

							newIndex += 7;

						}
					}


				}

				if(yDelta < 0) {

					if(!(blockIndex.Item1 / 3 == 0 && blockIndex.Item2 / 3 == 0)) {
						if((blockIndex.Item2 / 3 % 3) - 1 >= 0) {
							newIndex -= 3;
						} else if(blockIndex.Item2 / 3 % 3 >= 0) {
							newIndex -= 21;
						}
					}



				} else if(yDelta > 0) {

					if(!(blockIndex.Item1 / 3 == 2 && blockIndex.Item2 / 3 == 2)) {
						if((blockIndex.Item2 / 3 % 3) + 1 <= 2) {
							newIndex += 3;
						} else if(blockIndex.Item2 / 3 % 3 <= 2) {
							newIndex += 21;
						}
					}

				}

				if(newIndex != softSelected.BlockIndex) {

					softSelected.SoftDeselect();

					softSelected = sudokuBlocks[
						newIndex
					];
					softSelected.SoftSelect();
				}
			}
		}

		internal int Premultiply(int x,int y) {
			return (x * 9) + y;
		}

		internal SudokuNumber GetSudokuBlock(int top,int bottom) {
			return sudokuBlocks[Premultiply(top,bottom)];
		}

		internal Tuple<int,int> GetSudokuIndexes(int premultiplied) {

			return new Tuple<int,int>(
				premultiplied / 9,premultiplied % 9
			);
		}

		internal Tuple<int,int> GetVirtualIndex(int premultiplied) {

			var indexes = GetSudokuIndexes(premultiplied);

			var x = 0;
			var y = 0;


			x += (indexes.Item1 % 3) * 3;
			y += (indexes.Item1 / 3) * 3;

			x += indexes.Item2 % 3;
			y += indexes.Item2 / 3;

			return new Tuple<int,int>(x,y);


		}

		private SudokuNumber selectedBlock = null;
		private SudokuNumber softSelected = null;

		private void showNumberBar() {

			var blockIndex = GetSudokuIndexes(selectedBlock.BlockIndex);

			var row = (blockIndex.Item2 / 3 % 3) + ((blockIndex.Item1 / 3) * 3);

			if(row < 7) {
				NumberBar.Margin = new Thickness(0,(row+1)*11,0,0);
			} else {
				NumberBar.Margin = new Thickness(0,99 - ((10 - row) * 11),0,0);
			}

			NumberBar.Visibility = Visibility.Visible;


		}

		public void SwapNumberSelection(NumberSelector selector) {

			selectedNumber.Deselect();

			selectedNumber = selector;

		}

		private void hideNumberBar() {
			NumberBar.Visibility = Visibility.Collapsed;
		}

		private NumberSelector selectedNumber = null;

		internal void NumberTapped(int number) {

			selectedBlock.Number = number;

			//todo make the actual f-ing game

			BlockTapped(selectedBlock);
			return;
		}

		internal void BlockTapped(SudokuNumber block) {

			var isNull = selectedBlock == null;
			if(isNull) {
				selectedBlock = block;
				selectedBlock.Select();
				showNumberBar();

				if(selectedNumber != null) {
					selectedNumber.Deselect();
				}

				var sudokuIndexes = GetSudokuIndexes(selectedBlock.BlockIndex);

				var column = ((sudokuIndexes.Item1 % 3) * 3) + (sudokuIndexes.Item2 % 3);


				selectedNumber = numberBlocks[column];
				selectedNumber.Select();

			} else {
				selectedBlock.Deselect();
				selectedBlock = null;
				hideNumberBar();
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

			if(softSelected == block) {
				return;
			}

			softSelected.SoftDeselect();
			softSelected = block;

		}

	}
}
