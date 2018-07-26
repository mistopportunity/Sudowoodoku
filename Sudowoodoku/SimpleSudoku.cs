using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudowoodoku {
	internal sealed class SimpleSudoku:ISudokuBoard {


		public bool BoardComplete() {
			return false;
		}

		private int[,] template = new int[9,9];

		public int[,] GetTemplateBoard() {
			return null;
		}

		private List<int> getPossibilities(int x,int y) {

			bool[] removed = { false,false,false,false,false,false,false,false,false,false };

			//Inner quadrant scan
			for(int yIndex = 0;yIndex<9;yIndex++) {
				removed[board[x,yIndex]] = true;
			}

			//Horizontal left to right scan
			//todo

			var row = x / 3;
			var subRow = y / 3;

			for(int horizontalIndex = 0;horizontalIndex<9;horizontalIndex++) {

				var xValue = (horizontalIndex / 3) + row;
				var yValue = (horizontalIndex % 3) + (subRow * 3);
				removed[board[xValue,yValue]] = true;


			}

			//Vertical top to bottom scan
			//todo


			var column = x % 3;
			var subColumn = y % 3;

			for(int verticalIndex = 0;verticalIndex<9;verticalIndex++) {

				var xValue = ((verticalIndex / 3)*3) + column;
				var yValue = ((verticalIndex % 3)*3) + subColumn;
				removed[board[xValue,yValue]] = true;


			}

			List<int> possibilities = new List<int>();

			for(int i = 1;i<10;i++) {
				if(!removed[i]) {
					possibilities.Add(i);
				}
			}

			return possibilities;
		}

		public void PopulateBoard(int seed,int givenTiles) {

			int[,] board = new int[9,9];


			Random random = new Random(seed);

			for(int x = 0;x<9;x++) {
				for(int y = 0;y<9;y++) {


					var possibilities = getPossibilities(x,y);


					//This doesn't work :(
					board[x,y] = possibilities[
						random.Next(0,possibilities.Count-1)
					];


					
				}
			}

		}

		public void UpdatePiece(int value,int gridIndex,int subgridIndex) {
			return;
		}
	}
}
