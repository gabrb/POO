using Microsoft.EntityFrameworkCore;
using FFCE.Models;

namespace FFCE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Produtor> Produtores { get; set; }
        public DbSet<Flor> Flores { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Carrinho> Carrinhos { get; set; }
        public DbSet<ItemCarrinho> ItensCarrinho { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cliente ↔︎ Carrinho (1:1)
            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.Carrinho)
                .WithOne(carr => carr.Cliente)
                .HasForeignKey<Carrinho>(carr => carr.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);

            // Carrinho ↔︎ ItemCarrinho (1:N)
            modelBuilder.Entity<Carrinho>()
                .HasMany(c => c.Itens)
                .WithOne(i => i.Carrinho)
                .HasForeignKey(i => i.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Produto ↔︎ Flor (N:1) — corrigido para 1:N em Flor
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Flor)
                .WithMany(f => f.Produtos)           // agora 1 flor → N produtos
                .HasForeignKey(p => p.FlorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Produto ↔︎ Produtor (N:1)
            modelBuilder.Entity<Produto>()
                .HasOne(p => p.Produtor)
                .WithMany(prod => prod.Produtos)
                .HasForeignKey(p => p.ProdutorId)
                .OnDelete(DeleteBehavior.Cascade);

            // ItemCarrinho ↔︎ Produto (N:1)
            modelBuilder.Entity<ItemCarrinho>()
                .HasOne(i => i.Produto)
                .WithMany()
                .HasForeignKey(i => i.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração adicional para Flor
            modelBuilder.Entity<Flor>(entity =>
            {
                entity.HasKey(f => f.Id);
                entity.Property(f => f.ImageName)
                      .IsRequired()
                      .HasMaxLength(200);

                // Fluent API para a coleção de produtos
                entity.HasMany(f => f.Produtos)
                      .WithOne(p => p.Flor)
                      .HasForeignKey(p => p.FlorId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Dados iniciais de Flores
            modelBuilder.Entity<Flor>().HasData(
                new Flor
                {
                    Id = 1,
                    Nome = "Cacto em Pote",
                    Descricao = "Cacto decorativo em vaso de cerâmica",
                    ImageName = "cactoempote.jpg"
                },
                new Flor
                {
                    Id = 2,
                    Nome = "Flor Branca",
                    Descricao = "Flor de pétalas brancas, ideal para arranjos clean",
                    ImageName = "florbranca.jpg"
                },
                new Flor
                {
                    Id = 3,
                    Nome = "Rosa Vermelha",
                    Descricao = "Clássica rosa vermelha, símbolo de paixão",
                    ImageName = "rosavermelha.jpg"
                },
                new Flor
                {
                    Id = 4,
                    Nome = "Cacto Simples",
                    Descricao = "Cacto pequeno, resistente e de fácil manutenção",
                    ImageName = "cacto.jpg"
                },
                new Flor
                {
                    Id = 5,
                    Nome = "Girassol",
                    Descricao = "Girassol vibrante, traz alegria aos ambientes",
                    ImageName = "girassol.jpg"
                }
            );
        }
    }
}
