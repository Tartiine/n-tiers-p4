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

            for (int row = Rows - 1; row >= 0; row--)
            {
                var cell = Cells.FirstOrDefault(c => c.Row == row && c.Column == column);
                if (cell != null && cell.Token == null)
                {
                    cell.Token = token;
                    return true;
                }
            }

            return false;
        }

        public bool IsFull()
        {
            return Cells.All(cell => cell.Token != null);
        }

        public bool CheckWinCondition(Token token)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int col = 0; col < Columns; col++)
                {
                    if (CheckDirection(row, col, 1, 0, token) || // Horizontal
                        CheckDirection(row, col, 0, 1, token) || // Vertical
                        CheckDirection(row, col, 1, 1, token) || // Diagonal \
                        CheckDirection(row, col, 1, -1, token))  // Diagonal /
                    {
                        return true; // A winning condition is met
                    }
                }
            }

            return false; // No winning condition found
        }

        private bool CheckDirection(int startRow, int startCol, int rowStep, int colStep, Token token)
        {
            int count = 0;

            for (int i = 0; i < 4; i++)
            {
                int row = startRow + i * rowStep;
                int col = startCol + i * colStep;

                if (row >= 0 && row < Rows && col >= 0 && col < Columns)
                {
                    var cell = Cells.FirstOrDefault(c => c.Row == row && c.Column == col);
                    if (cell?.Token?.Color == token.Color)
                    {
                        count++;
                        if (count == 4)
                        {
                            return true; // Found four in a row
                        }
                    }
                    else
                    {
                        break; // Sequence broken
                    }
                }
                else
                {
                    break; // Out of bounds
                }
            }

            return false; // Less than four in a row
        }

    }
}
