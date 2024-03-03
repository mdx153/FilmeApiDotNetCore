using AutoMapper;
using FilmesApi.Data;
using FilmesApi.Data.Dtos;
using FilmesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FilmesApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CinemaController : ControllerBase
    {
        private FilmeContext _context;
        private IMapper _mapper;

        public CinemaController(FilmeContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        public IActionResult AdicionarCinema([FromBody] CreateCinemaDto cinemaDto)
        {
            Cinema cinema = _mapper.Map<Cinema>(cinemaDto);
            _context.Add(cinema);
            _context.SaveChanges();

            return CreatedAtAction(nameof(RecuperarCinemaPorId),
                new { Id = cinema.Id }, cinema);
        }

        [HttpGet("{id}")]
        public IActionResult RecuperarCinemaPorId(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(x => x.Id == id);
            if (cinema == null) { return  NotFound(); }

            ReadCinemaDto cinemaDto = _mapper.Map<ReadCinemaDto>(cinema);
            return Ok(cinemaDto);
        }

        [HttpGet]
        public IEnumerable<ReadCinemaDto> RecuperarCinemas(
            [FromQuery] int? enderecoId = null)
        {
            if(enderecoId == null)
             return _mapper.Map<List<ReadCinemaDto>>(_context.Cinemas.ToList());

            return _mapper.Map<List<ReadCinemaDto>>(_context.Cinemas.FromSqlRaw(
                $"SELECT Id, Nome, EnderecoId " +
                $"FROM cinemas where cinemas.EnderecoId = {enderecoId}").ToList());
        }

        [HttpPut("{id}")]
        public IActionResult AtualizarCinemaPorId(int id, 
            [FromBody] UpdateCinemaDto updateCinemaDto)
        {
            var cinema = _context.Cinemas.FirstOrDefault(x => x.Id == id);
            if (cinema == null) { return NotFound(); }

            _mapper.Map(updateCinemaDto, cinema); //Mapeia updateCinemaDto em cinema
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletarCinemaPorId(int id)
        {
            var cinema = _context.Cinemas.FirstOrDefault(x => x.Id == id);
            if (cinema == null) { return NotFound(); }

            _context.Remove(cinema);
            _context.SaveChanges(); 

            return NoContent();
        }
    }
}
