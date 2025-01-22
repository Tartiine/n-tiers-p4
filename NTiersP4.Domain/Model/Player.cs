using NTiersP4.Domain.Contracts;

namespace NTiersP4.Domain.Model;

public class Player : Entity 
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public ICollection<Game> Games { get; set; }
}