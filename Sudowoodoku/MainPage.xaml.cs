﻿using System;
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
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using System.Text;

namespace Sudowoodoku {
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage:Page {

		private readonly List<SudokuNumber> sudokuBlocks;

		private readonly List<NumberSelector> numberBlocks;

		private void allocateSudokuBlocks() {

			var topGrid = SudokuBoard.Children;

			for(int x = 1;x<10;x++) {

				var lowerGrid = ((Grid)topGrid[x]).Children;

				for(int y = 0;y<9;y++) {

					var block = (SudokuNumber)((Grid)lowerGrid[y]).Children[0];

					block.BlockIndex = Premultiply(x-1,y);

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

		private bool gameEnded = false;
		private async void EndGame() {


			foreach(var otherBlock in sudokuBlocks) {
				otherBlock.ExternalFocusChanged(bySelect: true);
			}

			softSelected = null;
			selectedBlock = null;
			gameEnded = true;

			var completionTime = DateTime.Now - startTime;
			var completionTimeString = formatCompletionTime(completionTime);


			var messageDialog = new MessageDialog($"{completionTimeString} is how long it took you to finish{(layer > 1 ? " this layer" : "")}" +

								$"{(layer > 1 ? $"\n{formatCompletionTime(DateTime.Now - originalStartTime)} is your total time for all layers" : "")}" +
												  $"\nWould you like to keep playing?",
				"Congratulations!") {
				DefaultCommandIndex = 1,
				CancelCommandIndex = 0,
			};

			messageDialog.Commands.Add(new UICommand("Yes, duh! Let's go!",(action) => {

				int seed = randomSeedMaker.Next();


				LoadBoard(new SimpleSudoku(),seed,1 * Math.Pow(0.92f,++layer));
			}));

			messageDialog.Commands.Add(new UICommand("No, I need more canned soup!",(action) => {

				Frame.Navigate(typeof(MenuPage));

			}));

			await messageDialog.ShowAsync();

		}

		private ISudokuBoard currentSudokuBoard;

		private DateTime startTime;

		private DateTime originalStartTime;

		private string formatCompletionTime(TimeSpan timeSpan) {

			var stringBuilder = new StringBuilder();


			stringBuilder.Append(timeSpan.Hours.ToString().PadLeft(2,'0'));

			stringBuilder.Append(':');

			stringBuilder.Append(timeSpan.Minutes.ToString().PadLeft(2,'0'));

			stringBuilder.Append(':');

			stringBuilder.Append(timeSpan.Seconds.ToString().PadLeft(2,'0'));


			var extraDays = Math.Floor(timeSpan.TotalDays);
			if(extraDays > 0) {
				stringBuilder.Append($" and {extraDays}{(extraDays != 1 ? "s" : "")}");
			}

			return stringBuilder.ToString();


		}



		private void LoadBoard(ISudokuBoard sudokuBoard,int seed,double difficulty) {



			statusTextBlock.Text = $"seed: {startSeed} layer: {layer}";


			currentSudokuBoard = sudokuBoard;
			currentSudokuBoard.PopulateBoard(seed,(int)Math.Floor(81 * difficulty));


			var template = currentSudokuBoard.GetTemplateBoard();

			//fill with template
			//that is, set readonly only properties and change blanks to zeroes

			sudokuBlocks[32].IsReadOnly = true;
			sudokuBlocks[32].Number = 32;

			foreach(var otherBlock in sudokuBlocks) {
				otherBlock.ExternalFocusChanged(bySelect: false);
			}

			gameEnded = false;

			startTime = DateTime.Now;

		}

		private Random randomSeedMaker;
		private uint startSeed;
		private uint layer = 1;

		protected override void OnNavigatedTo(NavigationEventArgs e) {

			base.OnNavigatedTo(e);

			var seed = (int)e.Parameter;

			startSeed = (uint)seed;

			randomSeedMaker = new Random(seed);

			LoadBoard(new SimpleSudoku(),randomSeedMaker.Next(),1f);

			originalStartTime = startTime;

			Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
		}

		protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {

			base.OnNavigatingFrom(e);

			Window.Current.CoreWindow.KeyDown -= CoreWindow_KeyDown;
		}

		public MainPage() {
			this.InitializeComponent();

			sudokuBlocks = new List<SudokuNumber>();

			numberBlocks = new List<NumberSelector>();

			allocateSudokuBlocks();

		}

		private void CoreWindow_KeyDown(CoreWindow sender,KeyEventArgs args) {
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
						BlockTapped(selectedBlock);
					} else if(softSelected != null && !softSelected.IsReadOnly) {
						softSelected.Number = 0;
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
			if(gameEnded) {
				return;
			}
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
					} else {
						newIndex += 20;
					}


				} else if(xDelta > 0) {

					if(!(blockIndex.Item1 % 3 == 2 && blockIndex.Item2 % 3 == 2)) {
						if((blockIndex.Item2 % 3) + 1 <= 2) {

							newIndex += 1;

						} else if(blockIndex.Item1 % 3 <= 2) {

							newIndex += 7;

						}
					} else {
						newIndex -= 20;
					}


				}

				if(yDelta < 0) {

					if(!(blockIndex.Item1 / 3 == 0 && blockIndex.Item2 / 3 == 0)) {
						if((blockIndex.Item2 / 3 % 3) - 1 >= 0) {
							newIndex -= 3;
						} else if(blockIndex.Item2 / 3 % 3 >= 0) {
							newIndex -= 21;
						}
					} else {
						newIndex += 60;
					}



				} else if(yDelta > 0) {

					if(!(blockIndex.Item1 / 3 == 2 && blockIndex.Item2 / 3 == 2)) {
						if((blockIndex.Item2 / 3 % 3) + 1 <= 2) {
							newIndex += 3;
						} else if(blockIndex.Item2 / 3 % 3 <= 2) {
							newIndex += 21;
						}
					} else {
						newIndex -= 60;
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

		private SudokuNumber selectedBlock = null;
		private SudokuNumber softSelected = null;

		private void showNumberBar() {

			var blockIndex = GetSudokuIndexes(selectedBlock.BlockIndex);

			var row = (blockIndex.Item2 / 3 % 3) + ((blockIndex.Item1 / 3) * 3);

			if(row < 7) {
				NumberBar.Margin = new Thickness(0,((row+1)*11)+6,0,0);
			} else {
				NumberBar.Margin = new Thickness(0,((99 - ((10 - row)) * 11))+6,0,0);
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

			if(selectedBlock.Number == number) {
				selectedBlock.Number = 0;
			} else {
				selectedBlock.Number = number;
			}

			var pieceIndexes = GetSudokuIndexes(selectedBlock.BlockIndex);

			currentSudokuBoard.UpdatePiece(
				selectedBlock.Number,
				pieceIndexes.Item1,
				pieceIndexes.Item2
			);

			BlockTapped(selectedBlock,true);

			if(currentSudokuBoard.BoardComplete()) {
				EndGame();
			}

		}
		internal void BlockTapped(SudokuNumber block,bool fromNumberSelection = false) {

			if(gameEnded) {
				return;
			}

			var isNull = selectedBlock == null;
			if(isNull) {

				if(block.IsReadOnly) {
					return;
				}
				selectedBlock = block;
				selectedBlock.Select();
				showNumberBar();

				if(selectedNumber != null) {
					selectedNumber.Deselect();
				}

				var sudokuIndexes = GetSudokuIndexes(selectedBlock.BlockIndex);


				int column;

				if(selectedBlock.Number == 0) {
					column = ((sudokuIndexes.Item1 % 3) * 3) + (sudokuIndexes.Item2 % 3);
				} else {
					column = selectedBlock.Number - 1;
				}


				selectedNumber = numberBlocks[column];
				selectedNumber.Select();


			} else {

				if(block == selectedBlock && !fromNumberSelection) {
					block.Number = 0;
				}

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
