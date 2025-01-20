namespace Database.Models
{
    public class Game
    {
        public int Id { get; set; }
        public int HostId { get; set; }
        public int? GuestId { get; set; }
        public GameStatus Status { get; set; }

        public Player Host { get; set; }
        public Player Guest { get; set; }
        public int GridId { get; set; }
        public Grid Grid { get; set; }

        public void StartGame()
        {
            if (Guest == null)
                throw new InvalidOperationException("Cannot start the game without a guest.");

            Status = GameStatus.InProgress;
        }

        public void JoinGame(Player guest)
        {
            if (Guest != null)
                throw new InvalidOperationException("Game already has a guest.");

            Guest = guest;
        }

        public bool PlayTurn(Player player, int column)
        {
            if (Status != GameStatus.InProgress)
                throw new InvalidOperationException("Cannot play turn when the game is not in progress.");

            if (player != Host && player != Guest)
                throw new InvalidOperationException("Player is not part of this game.");

            if (column < 0 || column >= Grid.Columns)
                throw new ArgumentOutOfRangeException(nameof(column), "Column index is out of bounds.");

            var token = new Token
            {
                Color = player == Host ? "Red" : "Yellow"
            };

            bool success = Grid.DropToken(column, token);

            if (!success)
            {
                throw new InvalidOperationException($"Cannot place token in column {column}. The column is full.");
            }

            return true;
        }



    }

    public enum GameStatus
    {
        AwaitingGuest,
        InProgress,
        Finished
    }
}
