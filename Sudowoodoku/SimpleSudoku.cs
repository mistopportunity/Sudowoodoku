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

		public int[,] GetTemplateBoard() {
			return null;
		}

		public void PopulateBoard(int seed,int emptyTiles) {
			return;
		}

		public void UpdatePiece(int value,int gridIndex,int subgridIndex) {
			return;
		}
	}
}
