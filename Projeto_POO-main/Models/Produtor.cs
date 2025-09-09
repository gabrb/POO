namespace FFCE.Models
{
    public class Produtor
    {
        public int    Id           { get; set; }

        public string Email        { get; set; } = null!;
        public string SenhaHash    { get; set; } = null!;

        public string Nome         { get; set; } = null!;
        public string Telefone     { get; set; } = null!;
        public string Endereco     { get; set; } = null!;
        public string NomeLoja     { get; set; } = null!;
        public string Descricao    { get; set; } = null!;

        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}