namespace NTiersP4.Domain.Model.Dtos;

public class GridDto
{
    public int Rows { get; set; }
    public int Columns { get; set; }
    public List<CellDto> Cells { get; set; }
}