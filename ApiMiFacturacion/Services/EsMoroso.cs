using ApiMiFacturacion.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiMiFacturacion.Services
{
    public class EsMoroso
    {
        private readonly MiFacturacionContext _context;

        public EsMoroso(MiFacturacionContext miFacturacionContext)
        {
            _context = miFacturacionContext;
        }

        public bool Moroso(int idCliente)
        {
            bool EsUnRatonMoroso = _context.Facturas
                .Any(factura => factura.ClienteId == idCliente && !factura.Pagada);

            return EsUnRatonMoroso;


        }
    }
}

