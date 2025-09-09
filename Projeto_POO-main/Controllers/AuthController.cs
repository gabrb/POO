using System.Security.Claims;
using FFCE.Data;
using FFCE.DTOs;
using FFCE.Models;
using FFCE.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FFCE.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public AuthController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        [HttpPost("registro-cliente")]
        public async Task<IActionResult> RegistroCliente([FromBody] ClienteCreateDTO dto)
        {
            if (await _context.Clientes.AnyAsync(c => c.Email == dto.Email) ||
                await _context.Produtores.AnyAsync(p => p.Email == dto.Email))
            {
                return BadRequest( new { message = "Este email já está cadastrado como cliente ou produtor." });
            }

            var cliente = new Cliente
            {
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                Nome = dto.Nome,
                Telefone = dto.Telefone,
                Endereco = dto.Endereco,
                Gostos = dto.Gostos,
                Carrinho = new Carrinho()
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cliente registrado com sucesso!", id = cliente.Id });
        }

        [HttpPost("registro-produtor")]
        public async Task<IActionResult> RegistroProdutor([FromBody] ProdutorCreateDTO dto)
        {
            if (await _context.Produtores.AnyAsync(p => p.Email == dto.Email) ||
                await _context.Clientes.AnyAsync(c => c.Email == dto.Email))
            {
                return BadRequest( new { message = "Este email já está cadastrado como produtor ou cliente." });
            }

            var produtor = new Produtor
            {
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                Nome = dto.Nome,
                Telefone = dto.Telefone,
                Endereco = dto.Endereco,
                NomeLoja = dto.NomeLoja,
                Descricao = dto.Descricao
            };

            _context.Produtores.Add(produtor);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Produtor registrado com sucesso!", id = produtor.Id });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {

            var clienteTask = _context.Clientes
                .FirstOrDefaultAsync(c => c.Email == dto.Email);

            var produtorTask = _context.Produtores
                .FirstOrDefaultAsync(p => p.Email == dto.Email);

            await Task.WhenAll(clienteTask, produtorTask);

            var cliente = clienteTask.Result;
            var produtor = produtorTask.Result;

            if (cliente != null && BCrypt.Net.BCrypt.Verify(dto.Senha, cliente.SenhaHash))
            {
                var token = _tokenService.GenerateToken(
                    cliente.Id, cliente.Email, "Cliente");
                return Ok(new { token, role = "Cliente", id = cliente.Id });
            }

            if (produtor != null && BCrypt.Net.BCrypt.Verify(dto.Senha, produtor.SenhaHash))
            {
                var token = _tokenService.GenerateToken(
                    produtor.Id, produtor.Email, "Produtor");
                return Ok(new { token, role = "Produtor", id = produtor.Id });
            }

            return Unauthorized("Credenciais inválidas!");
        }

        [Authorize]
        [HttpGet("verificar")]
        public async Task<IActionResult> VerificarCadastro()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            bool cadastroCompleto = role switch
            {
                "Cliente" => await _context.Clientes.AnyAsync(c => c.Id == userId),
                "Produtor" => await _context.Produtores.AnyAsync(p => p.Id == userId),
                _ => throw new InvalidOperationException("Role desconhecida.")
            };

            return Ok(new { cadastroCompleto });
        }
    }
}
