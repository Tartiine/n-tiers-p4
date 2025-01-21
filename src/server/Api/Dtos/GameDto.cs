namespace Api.Dtos
{
    public class GameDto
    {
        public int Id { get; set; }
        public int HostId { get; set; }
        public int? GuestId { get; set; }
        public string HostName { get; set; }
        public string GuestName { get; set; }
        public string Status { get; set; }
        public GridDto Grid { get; set; }
        public int CurrentTurnId { get; set; }     
    }
}
