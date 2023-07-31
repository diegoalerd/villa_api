using MagicVilla_API.Datos;
using MagicVilla_API.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using MagicVilla_API.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using MagicVilla_API.Repository.IRepository;

namespace MagicVilla_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class villaController : ControllerBase 
    {

        private readonly ILogger<villaController> _logger;
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly IVillaRepository _villaRepository;


        public villaController(ILogger<villaController> logger, IVillaRepository villaRepository, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
            _villaRepository = villaRepository;
        } 


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VillaDto>>> GetVillas()
        {
            _logger.LogInformation("Obtener las villas"); 

            IEnumerable<Villa> VIllaList = await _villaRepository.ObtainAll();
            return Ok(_mapper.Map<IEnumerable<VillaDto>>(VIllaList));    
        }

        [HttpGet("id:int")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VillaDto>> GetVillas(int id)
        {
            if(id==0)
            {
                _logger.LogError("Error al traer Villa con Id " + id); 
                return BadRequest();
            }

            //var villa = VillaStore.villaList.FirstOrDefault(v => v.Id==id);  
            var villa = await _villaRepository.Obtain(v => v.Id == id);

            if(villa == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<VillaDto>(villa));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<VillaDto>> CrearVilla([FromBody] VillaCreateDto CreateDto)
        {    
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(await _villaRepository.Obtain(v => v.Nombre.ToLower() == CreateDto.Nombre.ToLower()) != null)
            {
                ModelState.AddModelError("Nombre Existe", "La villa con ese nombre ya existe");
                return BadRequest(ModelState);
            }

            if(CreateDto == null)
            {
                return BadRequest(CreateDto);
            }
            
            Villa modelo = _mapper.Map<Villa>(CreateDto);

            await _villaRepository.Create(modelo);
            return CreatedAtRoute("GetVillas", new { id = modelo.Id }, modelo);
        }

        [HttpDelete("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteVilla(int id)
        {
            if(id == 0)
            {
                return BadRequest();
            }
            var villa = await _villaRepository.Obtain(v => v.Id == id);

            if(villa == null) 
            {
                return NotFound();
            }

            await _villaRepository.Remove(villa);

            return NoContent();
        }

        [HttpPut("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {
            if(updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }

            Villa modelo = _mapper.Map<Villa>(updateDto);

           await _villaRepository.Update(modelo);

            return NoContent();
        }

        [HttpPatch("id:int")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if(patchDto == null || id == 0)
            {
                return BadRequest();
            }

            var villa = await _villaRepository.Obtain(v => v.Id == id, tracked:false);

            VillaUpdateDto villaDto = _mapper.Map<VillaUpdateDto>(villa);

            if(villa == null) return BadRequest();

            patchDto.ApplyTo(villaDto, ModelState);
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Villa modelo = _mapper.Map<Villa>(villaDto);

            _villaRepository.Update(modelo);
            await _db.SaveChangesAsync();

            return NoContent();
        }
        
    }

}