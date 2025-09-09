namespace FFCE.DTOs;

public class ClienteDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public string Endereco { get; set; } = null!;
    public string Gostos { get; set; } = null!;
}