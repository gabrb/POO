// ClienteController.cs
using System.Security.Claims;
using FFCE.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FFCE.Models;

namespace FFCE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Cliente")]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ClienteController(AppDbContext context) => _context = context;

        [HttpGet("produtos-disponiveis")]
        public async Task<IActionResult> ListarProdutos()
        {
            var list = await _context.Produtos
                .Include(p => p.Flor)
                .Include(p => p.Produtor)
                .Select(p => new
                {
                    ProdutoId = p.Id,
                    Flor      = p.Flor.Nome,
                    Preco     = p.Preco,
                    Estoque   = p.Estoque,
                    NomeLoja  = p.Produtor.NomeLoja,
                    Telefone  = p.Produtor.Telefone
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpPost("adicionar-carrinho")]
        public async Task<IActionResult> AdicionarAoCarrinho([FromBody] AddCarrinhoDTO dto)
        {
            var clienteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cliente = await _context.Clientes
                .Include(c => c.Carrinho)
                .FirstOrDefaultAsync(c => c.Id == clienteId);
            if (cliente == null) return NotFound("Cliente não encontrado.");

            var produto = await _context.Produtos.FindAsync(dto.ProdutoId);
            if (produto == null) return NotFound("Produto não encontrado.");
            if (dto.Quantidade <= 0) return BadRequest("Quantidade inválida.");

            var item = await _context.ItensCarrinho
                .FirstOrDefaultAsync(i => i.CarrinhoId == cliente.Carrinho.Id && i.ProdutoId == dto.ProdutoId);
            if (item != null)
                item.Quantidade += dto.Quantidade;
            else
                _context.ItensCarrinho.Add(new ItemCarrinho {
                    CarrinhoId = cliente.Carrinho.Id,
                    ProdutoId  = dto.ProdutoId,
                    Quantidade = dto.Quantidade,
                    Preco      = produto.Preco
                });

            await _context.SaveChangesAsync();
            return Ok("Produto adicionado ao carrinho.");
        }

        [HttpGet("ver-carrinho")]
        public async Task<IActionResult> VisualizarCarrinho()
        {
            var clienteId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cliente = await _context.Clientes
                .Include(c => c.Carrinho)
                    .ThenInclude(ca => ca.Itens)
                        .ThenInclude(i => i.Produto)
                            .ThenInclude(p => p.Flor)
                .Include(c => c.Carrinho)
                    .ThenInclude(ca => ca.Itens)
                        .ThenInclude(i => i.Produto)
                            .ThenInclude(p => p.Produtor)
                .FirstOrDefaultAsync(c => c.Id == clienteId);
            if (cliente?.Carrinho == null) return NotFound("Carrinho não encontrado.");

            var baseUrl = $"{Request.Scheme}://{Request.Host}/images/";
            var itens = cliente.Carrinho.Itens.Select(i => new {
                i.Id,
                i.ProdutoId,
                Flor           = i.Produto.Flor.Nome,
                ImageUrl       = $"{baseUrl}{i.Produto.Flor.ImageName}",
                PrecoUnitario  = i.Preco,
                i.Quantidade,
                Subtotal       = i.Preco * i.Quantidade,
                Produtor       = i.Produto.Produtor.Nome,
                NomeLoja       = i.Produto.Produtor.NomeLoja,
                TelefoneProdutor = i.Produto.Produtor.Telefone
            });
            var total = itens.Sum(x => x.Subtotal);

            return Ok(new { Itens = itens, Total = total });
        }
    }
}
