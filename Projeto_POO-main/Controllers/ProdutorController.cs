// ProdutorController.cs
using System.Security.Claims;
using FFCE.Data;
using FFCE.DTOs;
using FFCE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFCE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Produtor")]
    public class ProdutorController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ProdutorController> _logger;

        public ProdutorController(AppDbContext context, IWebHostEnvironment env, ILogger<ProdutorController> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        [HttpGet("listar-flores")]
        public async Task<IActionResult> ListarFlores()
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}/images/";
            var flores = await _context.Flores
                .Select(f => new
                {
                    f.Id,
                    f.Nome,
                    f.Descricao,
                    ImageUrl = $"{baseUrl}{f.ImageName}"
                })
                .ToListAsync();
            return Ok(flores);
        }

        [HttpPost("cadastrar-produto")]
        public async Task<IActionResult> CadastrarProduto([FromBody] ProdutoCadastroDTO dto)
        {
            var produtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var produtor = await _context.Produtores.FindAsync(produtorId);
            if (produtor == null) return BadRequest("Produtor não encontrado.");

            var flor = await _context.Flores.FindAsync(dto.FlorId);
            if (flor == null) return BadRequest("Flor inválida.");

            var imagens = Path.Combine(_env.WebRootPath, "images", flor.ImageName);
            if (!System.IO.File.Exists(imagens))
                return BadRequest("Imagem selecionada não encontrada.");

            var produto = new Produto
            {
                FlorId = dto.FlorId,
                ProdutorId = produtorId,
                Preco = dto.Preco,
                Estoque = dto.Estoque,
                ImageName = flor.ImageName
            };
            _context.Produtos.Add(produto);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Produto cadastrado com sucesso."
            });
        }

        [HttpGet("meus-produtos")]
        public async Task<IActionResult> MeusProdutos()
        {
            var produtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var baseUrl = $"{Request.Scheme}://{Request.Host}/images/";
            var produtos = await _context.Produtos
                .Include(p => p.Flor)
                .Where(p => p.ProdutorId == produtorId)
                .Select(p => new
                {
                    p.Id,
                    Flor = p.Flor.Nome,
                    p.Preco,
                    p.Estoque,
                    ImageUrl = $"{baseUrl}{p.Flor.ImageName}"
                })
                .ToListAsync();
            return Ok(produtos);
        }

        [HttpPut("editar-produto/{id}")]
        public async Task<IActionResult> EditarProduto(int id, [FromBody] ProdutoAtualizaDTO dto)
        {
            var produtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == id && p.ProdutorId == produtorId);
            if (produto == null) return NotFound("Produto não encontrado ou acesso negado.");
            if (dto.Preco.HasValue) produto.Preco = dto.Preco.Value;
            if (dto.Estoque.HasValue) produto.Estoque = dto.Estoque.Value;
            if (dto.FlorId.HasValue)
            {
                var flor = await _context.Flores.FindAsync(dto.FlorId.Value);
                if (flor == null) return BadRequest("Flor inválida.");

                var imagens = Path.Combine(_env.WebRootPath, "images", flor.ImageName);
                if (!System.IO.File.Exists(imagens))
                    return BadRequest("Imagem da flor não encontrada.");

                produto.FlorId = dto.FlorId.Value;
                produto.ImageName = flor.ImageName;
            }

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Produto atualizado com sucesso."
            });

        }

        [HttpDelete("excluir-produto/{id}")]
        public async Task<IActionResult> ExcluirProduto(int id)
        {
            var produtorId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var produto = await _context.Produtos
                .FirstOrDefaultAsync(p => p.Id == id && p.ProdutorId == produtorId);
            if (produto == null) return NotFound("Produto não encontrado ou acesso negado.");

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
            return Ok(new
            {
                success = true,
                message = "Produto excluído com sucesso."
            });

        }
    }
}
