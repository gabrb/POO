namespace FFCE.DTOs
{
    public class ProdutoDTO
    {
        public int? FlorId { get; set; }            
        public string? FlorNome { get; set; }    
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
    }
}