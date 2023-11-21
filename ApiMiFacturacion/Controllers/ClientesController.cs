using ApiMiFacturacion.DTOs;
using ApiMiFacturacion.Models;
using ApiMiFacturacion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMiFacturacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ClientesController : ControllerBase
    {

        private readonly MiFacturacionContext _context;
        private readonly EsMoroso _esMoroso;


        public ClientesController(MiFacturacionContext context, EsMoroso esMoroso)
        {
            _context = context;
            _esMoroso = esMoroso;
        }


        #region GET
        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            var clientes = await _context.Clientes.ToListAsync();
            if (clientes == null)
            {
                return NotFound();
            }

            return Ok(clientes);
        }

       
        [HttpGet("{CIUDAD}")]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientesCiudad(string CIUDAD)
        {
         
            
            var clientes = await _context.Clientes.Where(x => x.Ciudad == CIUDAD).ToListAsync();

            if (clientes == null || clientes.Count == 0)
            {
                //return NotFound();

                return NotFound("No se encontraron clientes para la ciudad especificada.");
            }

            return Ok(clientes);
        }
        #endregion

        #region POST

        
        [HttpPost("agregar/NuevoCliente")]
        public async Task<ActionResult> PostNuevoCliente([FromBody] DTOCliente cliente)
        {
            var newCliente = new Cliente
            {
                Nombre = cliente.Nombre,
                Ciudad = cliente.Ciudad,
            };
            await _context.Clientes.AddAsync(newCliente);
            await _context.SaveChangesAsync();

            return Ok(newCliente);
        }
        #endregion

        #region PUT

        [HttpPut("/modificar/[controller]")]
        public async Task<ActionResult<Cliente>> PutLibro(DTOCliente modificarCliente)
        {

            var ClienteExiste = await _context.Clientes.AsTracking().FirstOrDefaultAsync(x => x.IdCliente == modificarCliente.ID);

            if (ClienteExiste == null)
            {
                return NotFound("El ID no coincide.");
            }

            ClienteExiste.Nombre = modificarCliente.Nombre;
            ClienteExiste.Ciudad = modificarCliente.Ciudad;

            _context.Update(ClienteExiste);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        #endregion

        #region DELETE
        [HttpDelete("Cliente/{id:int}")]
        public async Task<ActionResult> BorrarCliente( int id)
        {

            var facturas = await _context.Facturas.AnyAsync(x => x.ClienteId == id);

            if (facturas)
            {
                return BadRequest("El cliente contiene facturas relacionados");
            }


            var cliente = await _context.Clientes.FirstOrDefaultAsync(x => x.IdCliente == id);

            if (cliente is null)
            {
                return NotFound("El cliente con la ID proporcionada no existe");
            }

            _context.Remove(cliente);
            await _context.SaveChangesAsync();
            return Ok();
        }
        #endregion


        #region MOROSO

        [HttpGet("{idCliente}/EsMoroso")]
        public async Task<ActionResult<bool>> Moroso(int idCliente)
        {

            var clientes = await _context.Clientes.FindAsync(idCliente);
            if (clientes == null)
            {
                return NotFound("El ID no existe");
            }

            bool ratonMoroso = _esMoroso.Moroso(idCliente);

            return ratonMoroso;
        }

        #endregion

    }
}
