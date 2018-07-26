using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudowoodoku {
	internal interface ISudokuBoard {

		void UpdatePiece(int value,int gridIndex,int subgridIndex);

		bool BoardComplete();
	
		void PopulateBoard(int seed,int givenTiles);

		int[,] GetTemplateBoard();

	}
}

