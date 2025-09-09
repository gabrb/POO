namespace FFCE.Models
{
    public class Flor
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string ImageName { get; set; }

        // NOVA coleção para o relacionamento 1:N
        public ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    }
}