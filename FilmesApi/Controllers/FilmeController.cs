using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmeController: ControllerBase
{
    private FilmeContext _context;
    private IMapper _mappper;

    /// <summary>
    /// Construtor com as injeções de dependências.
    /// </summary>
    /// <param name="context">DataBase</param>
    /// <param name="mappper">AutoMapper</param>
    public FilmeController(FilmeContext context, IMapper mappper)
    {
        _context = context;
        _mappper = mappper;
    }

    private static int id = 0;

    /// <summary>
    /// Adiciona um filme ao banco de dados
    /// </summary>
    /// <param name="filmeDto">Objeto com os campos necessarios
    /// para criação de um filme</param>
    /// <returns>IActionResult</returns>
    /// <response code="201">Caso inserção seja feita com sucesso</response>
    [HttpPost] //indica que o método vai ser do verbo post.
    [ProducesResponseType(StatusCodes.Status201Created)]
    public IActionResult AdicionarFilme([FromBody]CreateFilmeDto filmeDto) 
    {
        //mapeia filmeDto para filme
        Filme filme = _mappper.Map<Filme>(filmeDto);

        _context.Add(filme); //adicione os dados no insert
        _context.SaveChanges(); //commit
        return CreatedAtAction(nameof(RecuperarFilmePorId),
            new { id = filme.Id},
            filme);
    }

    /// <summary>
    /// Retorna uma lista de filmes, com ou sem paginação
    /// </summary>
    /// <param name="skip">Quantidade que vai pular de registro</param>
    /// <param name="take">Quantidade que deseja retornar</param>
    /// <returns>lista de ReadFilmeDto</returns>
    /// <response code="200">Lista de filmes</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IEnumerable<ReadFilmeDto> RecuperarFilmes(
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50,
        [FromQuery] string? nomeCinema = null)
    {
        if(nomeCinema == null)
        {
            return _mappper.Map<List<ReadFilmeDto>>(
                _context.Filmes.Skip(skip).Take(take).ToList());
        }

        return _mappper.Map<List<ReadFilmeDto>>(
                _context.Filmes.Skip(skip).Take(take)
                .Where(filme => filme.Sessoes.Any(sessao => sessao.Cinema.Nome == nomeCinema))
                .ToList());


    }

    /// <summary>
    /// Retorna um filme por id
    /// </summary>
    /// <param name="id">Id do registro que queremos retornar</param>
    /// <returns>IActionResult</returns>
    /// <response code="200">filme por id</response>
    /// <response code="404">Se não encontrou o registro</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RecuperarFilmePorId(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(x => x.Id == id);
        if (filme == null) return NotFound();

        var filmeDto = _mappper.Map<ReadFilmeDto>(filme);

        return Ok(filmeDto);
    }

    /// <summary>
    /// Atualiza um registro existente
    /// </summary>
    /// <param name="id">Id do registro que deseja atualizar parcialmente</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Atualizado parcialmente com sucesso</response>
    /// <response code="404">Se não encontrou o registro</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AtualizarFilme(int id, [FromBody] UpdateFilmeDto filmeDto)
    {
        var filme = _context.Filmes.FirstOrDefault(x => x.Id == id);
        if (filme == null) return NotFound();

        _mappper.Map(filmeDto, filme);
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Atualiza parcialmente um registro existente
    /// </summary>
    /// <param name="id">Id do registro que deseja atualizar parcialmente</param>
    /// <param name="patch">Objeto que contem os dados de atualização</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Atualizado parcialmente com sucesso</response>
    /// <response code="404">Se não encontrou o registro</response>
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult AtualizarFilmeParcial(int id, 
        JsonPatchDocument<UpdateFilmeDto> patch)
    {
        var filme = _context.Filmes.FirstOrDefault(x => x.Id == id);
        if (filme == null) return NotFound();

        var filmeParaAtualizar = _mappper.Map<UpdateFilmeDto>(filme);

        patch.ApplyTo(filmeParaAtualizar, ModelState);

        if (!TryValidateModel(filmeParaAtualizar))
        {
            return ValidationProblem(ModelState);
        }

        _mappper.Map(filmeParaAtualizar, filme); 
        _context.SaveChanges();
        return NoContent();
    }

    /// <summary>
    /// Deleta um registro por id
    /// </summary>
    /// <param name="id">Id do registro que deseja deletar</param>
    /// <returns>IActionResult</returns>
    /// <response code="204">Deletado com sucesso</response>
    /// <response code="404">Se não encontrou o registro</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult RemoverFilme(int id)
    {
        var filme = _context.Filmes.FirstOrDefault(x => x.Id == id);
        if (filme == null) return NotFound();

        _context.Remove(filme);
        _context.SaveChanges();
        return NoContent();
    }
}
