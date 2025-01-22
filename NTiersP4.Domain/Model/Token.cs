using NTiersP4.Domain.Contracts;

namespace NTiersP4.Domain.Model;

public class Token : Entity
{
    public int Id { get; set; }
    public string Color { get; set; }
}
