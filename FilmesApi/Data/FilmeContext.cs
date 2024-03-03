using FilmesApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FilmesApi.Data
{
    public class FilmeContext : DbContext
    {
        public FilmeContext(DbContextOptions<FilmeContext> opts)
            :base(opts)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Estamos dizendo que a entidade Sessao vai ter como chave:
            //FilmeId e CinemaId (chaves composta)
            builder.Entity<Sessao>()
                .HasKey(sessao => new { sessao.FilmeId, sessao.CinemaId });

            //Relacionamento

            //1 sessão vai ter um Cinema
            builder.Entity<Sessao>()
                .HasOne(sessao => sessao.Cinema)
                .WithMany(cinema => cinema.Sessoes) //esse cinema vai ter 1 ou mais sessoes
                .HasForeignKey(sessao => sessao.CinemaId); //essa sessão vai ter como chave estrangeira o CinemaId

            //1 sessão vai ter um Filme
            builder.Entity<Sessao>()
                .HasOne(sessao => sessao.Filme)
                .WithMany(filme => filme.Sessoes) //esse filme vai ter 1 ou mais sessoes
                .HasForeignKey(sessao => sessao.FilmeId);//essa sessão vai ter como chave estrangeira o FilmeId

            //Config de endereço
            builder.Entity<Endereco>()
                .HasOne(endereco => endereco.Cinema)//1 endereço tem 1 cinema
                .WithOne(cinema => cinema.Endereco) //1 cinema tem 1 endereço
                .OnDelete(DeleteBehavior.Restrict); //modo de deleção vai ser restrito
        }

        public DbSet<Filme> Filmes { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Endereco> Enderecos { get; set; }
        public DbSet<Sessao> Sessoes { get; set; }
    }
}
