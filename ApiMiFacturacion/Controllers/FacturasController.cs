using ApiMiFacturacion.DTOs;
using ApiMiFacturacion.Models;
using ApiMiFacturacion.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMiFacturacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class FacturasController : ControllerBase
    {

        private readonly MiFacturacionContext _context;


        public FacturasController(MiFacturacionContext context)
        {
            _context = context;
        
        }

        #region GET
        //[AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturas()
        {
            var facturas = await _context.Facturas.ToListAsync();
            if (facturas == null)
            {
                return NotFound();
            }

            return Ok(facturas);
        }

        [HttpGet("ObtenerFactura/{numeroFactura}")]
        public async Task<ActionResult<Factura>> ObtenerFacturaPorNumero(int numeroFactura)
        {
            var factura = await _context.Facturas.FirstOrDefaultAsync(f => f.Nfactura == numeroFactura);

            if (factura == null)
            {
                return NotFound("La factura no existe");
            }

            return factura;
        }


        [HttpGet("PRECIO")]
        public async Task<ActionResult<IEnumerable<Factura>>> GetFacturasPreciosQueryString([FromQuery] decimal PrecioMinimo)
        {
            var facturitas = await _context.Facturas.Where(x => x.Importe >= PrecioMinimo ).ToListAsync();
            return Ok(facturitas);
        }

        //[HttpGet("importe/desde")]
        //public async Task<ActionResult<IEnumerable<Factura>>> GetFacturaDesdeImporte2([FromQuery] decimal importeMinimo)
        //{
        //    var facturasQuery = _context.Facturas.AsQueryable();

        //    var facturasFiltradas = facturasQuery.Where(x => x.Importe > importeMinimo);

        //    var facturas = await facturasFiltradas.ToListAsync();

        //    //await _operacionesService.AddOperacion("Get", "Libros");
        //    return Ok(facturas);
        //}

        [HttpGet("PAGADAS")]

        public async Task<ActionResult<IEnumerable<Factura>>> GetPagadas()
        {
            var pagadas = await _context.Facturas.Where(x => x.Pagada).ToListAsync();

            return Ok(pagadas);
        }


        [HttpGet("{CLIENTE}")]
        public async Task<ActionResult<List<Factura>>> GetClientesCiudad(int CLIENTE)
        {


            var clientes = await _context.Facturas.Where(x => x.ClienteId == CLIENTE).ToListAsync();

            if (clientes == null )
            {

                return NotFound("No se encontraron clientes con facturas.");
            }

            return Ok(clientes);
        }

        #endregion

        #region POST

        [HttpPost("agregar/NuevaFactura")]
        public async Task<ActionResult> PostNuevoCliente([FromBody] DTOFactura factura)
        {
            var newFactura = new Factura
            {
                Fecha = factura.Fecha,
                Importe = factura.Importe,
                Pagada = factura.Pagada,
                ClienteId = factura.ClienteId,
            };
            await _context.Facturas.AddAsync(newFactura);
            await _context.SaveChangesAsync();

            return Ok(newFactura);
        }

        #endregion

        #region PUT

        [HttpPut("/modificar/[controller]")]
        public async Task<ActionResult<Cliente>> PutFactura(DTOFactura modificarFactura)
        {

            var FacturaExiste = await _context.Facturas.AsTracking().FirstOrDefaultAsync(x => x.Nfactura == modificarFactura.NumeroFactura);

            if (FacturaExiste == null)
            {
                return NotFound("El ID no coincide.");
            }

            FacturaExiste.Fecha = modificarFactura.Fecha;
            FacturaExiste.Importe = modificarFactura.Importe;
            FacturaExiste.Pagada = modificarFactura.Pagada;
            FacturaExiste.ClienteId = modificarFactura.ClienteId;

          

            _context.Update(FacturaExiste);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        #endregion

        #region DELETE
        [HttpDelete("Cliente/{NumeroFactura:int}")]
        public async Task<ActionResult> BorrarFactura(int NumeroFactura)
        {


            var factura = await _context.Facturas.FirstOrDefaultAsync(x => x.Nfactura == NumeroFactura);

            if (factura is null)
            {
                return NotFound("la factura proporcionada no existe");
            }

            _context.Remove(factura);
            await _context.SaveChangesAsync();
            return Ok();
        }
        #endregion

    }
}
