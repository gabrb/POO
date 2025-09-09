using System.ComponentModel.DataAnnotations;

namespace FFCE.Models
{
    public class Carrinho
    {
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;

        public ICollection<ItemCarrinho> Itens { get; set; } = new List<ItemCarrinho>();
    }
}