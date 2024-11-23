namespace Api.Models
{
public class Game
{
    public int Id { get; set; }
    public int HostId { get; set; }
    public int? GuestId { get; set; } 
    public string Status { get; set; }

    public Player Host { get; set; }
    public Player Guest { get; set; }
}

}
