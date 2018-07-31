using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Storage;

namespace Sudowoodoku {

	internal sealed class SimpleSudoku:ISudokuBoard {

		private int[,] template = new int[9,9];


		private int[,] rotate(int[,] board,int times) {
			var newBoard = new int[9,9];
			int[] indexTranslator;
			switch(times % 3) {
				default:
					indexTranslator = new int[]{ 0,1,2,3,4,5,6,7,8 };
					break;
				case 1:
					indexTranslator = new int[]{ 6,3,0,7,4,1,8,5,2 };
					break;
				case 2:
					indexTranslator = new int[]{ 2,1,0,5,4,3,8,7,6 };
					break;
				case 3:
					indexTranslator = new int[]{ 2,5,8,1,4,7,0,3,6 };
					break;
			}
			for(int x = 0;x < 9;x++) {

				for(int y = 0;y<9;y++) {

					var newX = indexTranslator[x];
					var newY = indexTranslator[y];

					newBoard[newX,newY] = board[x,y];

				}

			}
			return newBoard;
		}

		private int[,] translateNumbers(int[,] board, Random random) {

			var newBoard = new int[9,9];

			var translator = new int[]{ 1,2,3,4,5,6,7,8,9 };

			var i = 9;
			while(i-- > 1) {
				int index = random.Next(i + 1);
				int value = translator[index];
				translator[index] = translator[i];
				translator[i] = value;
			}

			for(int x = 0;x < 9;x++) {

				for(int y = 0;y<9;y++) {

					if(board[x,y] != 0) {
						newBoard[x,y] = translator[board[x,y]-1];
					}

				}

			}

			return newBoard;
		}

		private async Task<Tuple<int[,],int>> getCheatingBoard(string fileName) {

			var uri = new Uri($"ms-appx:///Template boards/{fileName}");

			var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

			var lines = await FileIO.ReadLinesAsync(file);

			

			if(lines.Count != 9) {
				throw new Exception("AHHHHHHHHHHH it's totally fucked m8 (the template valid is invalid)");
			}
			var board = new int[9,9];
			var givens = 0;
			for(int x = 0;x < 9;x++) {

				for(int y = 0;y<9;y++) {

					var line = lines[x];

					var character = line[y];

					if(!int.TryParse(character.ToString(),out int result)) {
						throw new Exception("Something in this tempalte file wasn't a number. GOOD JOB IDIOT");
					}

					if(result != 0) {
						givens++;
					}

					board[x,y] = result;

				}

			}
			return new Tuple<int[,], int>(board,givens);

		}

		private int[,] templateBoard;
		public int[,] GetTemplateBoard() {
			return templateBoard;
		}

		public async void PopulateBoard(int seed,double difficulty) {

			Random random = new Random(seed);
			string templatePath;
			switch(Math.Floor(difficulty * 4)) {
				default:
					templatePath = "Easy.txt";
					break;
				case 2:
					templatePath = "Medium.txt";
					break;
				case 1:
					templatePath = "Hard.txt";
					break;
				case 0:
					templatePath = "Harder.text";
					break;
			}

			var cheatingBoard = await getCheatingBoard(templatePath);
			currentBoardPieces = cheatingBoard.Item2;
			var board = cheatingBoard.Item1;

			var rotations = random.Next(0,3);
			if(rotations > 0) {
				board = rotate(board,rotations);
			}
			board = translateNumbers(board,random);

			templateBoard = board;

			workingBoard = board;

		}

		private int[,] workingBoard;
		int currentBoardPieces;
		public bool BoardComplete() {
			if(currentBoardPieces == 81) {
				//grid checking
				for(var i = 0;i<9;i++) {
					var gridCounter = new int[9];
					for(var x = 0;x<9;x++) {
						gridCounter[workingBoard[i,x]-1]++;
					}
					for(int i2 = 0;i2<9;i2++) {
						if(gridCounter[i2] != 1) {
							return false;
						}
					}
				}
				//column checking
				for(var i = 0;i<9;i++) {
					var columnCounter = new int[9];
					var column = i / 3;
					var subColumn = i % 3;
					for(int verticalIndex = 0;verticalIndex<9;verticalIndex++) {
						var xValue = ((verticalIndex / 3)*3) + column;
						var yValue = ((verticalIndex % 3)*3) + subColumn;

						var value = workingBoard[xValue,yValue];

						columnCounter[value-1]++;
					}
					for(int i2 = 0;i2<9;i2++) {
						if(columnCounter[i2] != 1) {
							return false;
						}
					}
				}
				//row checking
				for(var i = 0;i<9;i++) {
					var rowCounter = new int[9];
					var row = i / 3;
					var subRow = i % 3;
					for(int horizontalIndex = 0;horizontalIndex<9;horizontalIndex++) {
						var xValue = (horizontalIndex / 3) + (row * 3);
						var yValue = (horizontalIndex % 3) + (subRow * 3);

						var value = workingBoard[xValue,yValue];

						rowCounter[value-1]++;
					}
					for(int i2 = 0;i2<9;i2++) {
						if(rowCounter[i2] != 1) {
							return false;
						}
					}
				}
				return true;
			}
			return false;
		}


		public void UpdatePiece(int value,int gridIndex,int subgridIndex) {
			if(workingBoard[gridIndex,subgridIndex] == 0) {
				if(value == 0) {
					return;
				} else {
					workingBoard[gridIndex,subgridIndex] = value;
					currentBoardPieces++;
				}
			} else {
				if(value == 0) {
					workingBoard[gridIndex,subgridIndex] = 0;
					currentBoardPieces--;
					return;
				} else {
					workingBoard[gridIndex,subgridIndex] = value;
				}
			}
		}
	}
}
