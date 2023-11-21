namespace ApiMiFacturacion.DTOs
{
   
        public class DTOFactura
        {
            public int NumeroFactura { get; set; }

            public DateTime Fecha { get; set; }
            public decimal Importe { get; set; }

            public bool Pagada { get; set; }

            public int ClienteId { get; set; }
        }
    
}
