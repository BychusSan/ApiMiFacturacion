using ApiMiFacturacion.DTOs;
using ApiMiFacturacion.Services;
using ApiMiFacturacion.Models;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMiFacturacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly MiFacturacionContext _context;
        private readonly HashService _hashService;
        private readonly TokenService _tokenService;


        public AuthController(MiFacturacionContext miFacturacionContext, HashService hashService,
             TokenService tokenService)
        {
            _context = miFacturacionContext;
            _hashService = hashService;
            _tokenService = tokenService;

        }


        #region LOGIN

        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] DTOUsuario usuario)
        {
            var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return Unauthorized("Usuario no existe o el email es inválido");
            }

            var resultadoHash = _hashService.Hash(usuario.Password, usuarioDB.Salt);
            if (usuarioDB.Password == resultadoHash.Hash)
            {
                // Si el login es exitoso devolvemos el token y el email (DTOLoginResponse) 
                var response = _tokenService.GenerarToken(usuario);
                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }



        #endregion

    }
}
