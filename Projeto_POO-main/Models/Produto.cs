namespace FFCE.Models
{
    public class Produto
    {
        public int Id { get; set; }

        public int FlorId { get; set; }
        public Flor Flor { get; set; }

        public int ProdutorId { get; set; }
        public Produtor Produtor { get; set; }

        public decimal Preco { get; set; }
        public int Estoque { get; set; }

        public string ImageName { get; set; } = null!;
    }
}