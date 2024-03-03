using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace FilmesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnderecoController : ControllerBase
    {
        private FilmeContext _context;
        private IMapper _mapper;

        public EnderecoController(FilmeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult AdicionarEndereco([FromBody] CreateEnderecoDto enderecoDto)
        {
            Endereco endereco = _mapper.Map<Endereco>(enderecoDto);
            _context.Add(endereco);
            _context.SaveChanges();

            return CreatedAtAction(nameof(RecuperarEnderecoPorId),
                new { Id = endereco.Id }, endereco);
        }

        [HttpGet("{id}")]
        public IActionResult RecuperarEnderecoPorId(int id)
        {
            var endereco = _context.Enderecos.FirstOrDefault(x => x.Id == id);
            if (endereco == null) { return NotFound(); }

            ReadEnderecoDto enderecoDto = _mapper.Map<ReadEnderecoDto>(endereco);
            return Ok(enderecoDto);
        }

        [HttpGet]
        public IEnumerable<ReadEnderecoDto> RecuperarEnderecos()
        {
            return _mapper.Map<List<ReadEnderecoDto>>(_context.Enderecos.ToList());
        }

        [HttpPut("{id}")]
        public IActionResult AtualizarEnderecoPorId(int id,
            [FromBody] UpdateEnderecoDto updateEnderecoDto)
        {
            var endereco = _context.Enderecos.FirstOrDefault(x => x.Id == id);
            if (endereco == null) { return NotFound(); }

            _mapper.Map(updateEnderecoDto, endereco); //Mapeia updateCinemaDto em cinema
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarEnderecoPorId(int id)
        {
            var endereco = _context.Enderecos.FirstOrDefault(x => x.Id == id);
            if (endereco == null) { return NotFound(); }

            _context.Remove(endereco);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
