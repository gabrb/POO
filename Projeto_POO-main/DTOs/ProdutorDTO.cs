namespace FFCE.DTOs;

public class ProdutorDTO
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public string Endereco { get; set; } = null!;
    public string NomeLoja { get; set; } = null!;
    public string Descricao { get; set; } = null!;
}