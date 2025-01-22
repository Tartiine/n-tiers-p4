using NTiersP4.Domain.Contracts;

namespace NTiersP4.Domain.Model;

public class Cell : Entity
{
    public int Id { get; set; }
    public int Row { get; set; }
    public int Column { get; set; }
    public int? TokenId { get; set; } 
    public Token Token { get; set; }  
    public int GridId { get; set; }
    public Grid Grid { get; set; }
}
