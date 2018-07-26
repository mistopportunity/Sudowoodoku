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

		public void PopulateBoard(int seed,int givenTiles) {

			int[,] board = new int[9,9];

			List<int> getPossibilities(int x,int y) {

				bool[] removed = {false,false,false,false,false,false,false,false,false,false };

				//Inner quadrant scan
				for(int yIndex = 0;yIndex<9;yIndex++) {
					removed[board[x,yIndex]] = true;
				}

				//Horizontal left to right scan
				//todo

				//Vertical top to bottom scan
				//todo

				List<int> possibilities = new List<int>();

				for(int i = 1;i<10;i++) {
					if(!removed[i]) {
						possibilities.Add(i);
					}
				}

				return possibilities;
			}

			Random random = new Random(seed);

			for(int x = 0;x<9;x++) {
				for(int y = 0;y<9;y++) {


					var possibilities = getPossibilities(x,y);

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
