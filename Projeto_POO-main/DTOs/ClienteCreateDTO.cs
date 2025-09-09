namespace FFCE.DTOs;

public class ClienteCreateDTO
{
    public string Nome { get; set; } = null!;
    public string Telefone { get; set; } = null!;
    public string Endereco { get; set; } = null!;
    public string Gostos { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Senha { get; set; } = null!;
}