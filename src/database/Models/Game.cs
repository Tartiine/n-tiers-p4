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
                throw new InvalidOperationException("Cannot play turn when game is not in progress.");

            if (player != Host && player != Guest)
                throw new InvalidOperationException("Player is not part of this game.");

            var token = new Token { Color = player == Host ? "Red" : "Yellow" };

            return Grid.DropToken(column, token);
        }


    }

    public enum GameStatus
    {
        AwaitingGuest,
        InProgress,
        Finished
    }
}
