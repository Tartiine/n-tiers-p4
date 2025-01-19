using System.Collections.Generic;

namespace Database.Models
{   public class Grid
    {
        public int Id { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<Cell> Cells { get; set; } = new List<Cell>();

        public Grid() { }

        public Grid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;

            // Initialize the grid with empty cells
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    Cells.Add(new Cell { Row = row, Column = column, Grid = this });
                }
            }
        }

        public bool DropToken(int column, Token token)
        {
            if (column < 0 || column >= Columns)
                return false; // Invalid column

            for (int row = Rows - 1; row >= 0; row--)
            {
                var cell = Cells.FirstOrDefault(c => c.Row == row && c.Column == column);
                if (cell != null && cell.Token == null)
                {
                    cell.Token = token;
                    return true;
                }
            }

            return false; // Column is full
        }

        public bool IsFull()
        {
            return Cells.All(cell => cell.Token != null);
        }
    }
}
