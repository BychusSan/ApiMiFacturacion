using ApiMiFacturacion.DTOs;
using ApiMiFacturacion.Models;
using ApiMiFacturacion.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ApiMiFacturacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly MiFacturacionContext _context;
        private readonly HashService _hashService;

        public UsuariosController(MiFacturacionContext miFacturacionContext, HashService hashService)
        {
            _context = miFacturacionContext;
            _hashService = hashService;
        }


        #region NUEVO USUARIO

        
        [HttpPost("hash/nuevousuario")]
        public async Task<ActionResult> PostNuevoUsuarioHash([FromBody] DTOUsuario usuario)
        {
            var resultadoHash = _hashService.Hash(usuario.Password);
            var newUsuario = new Usuario
            {
                Email = usuario.Email,
                Password = resultadoHash.Hash,
                Salt = resultadoHash.Salt
            };

            await _context.Usuarios.AddAsync(newUsuario);
            await _context.SaveChangesAsync();

            return Ok(newUsuario);
        }
        #endregion

        #region GET USUARIOS

    
        [HttpGet("hash/GetUsuarios")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios.ToListAsync();
            if (usuarios == null)
            {
                return NotFound();
            }

            return Ok(usuarios);
        }

        #endregion

        #region CHECK USUARIO 

        [HttpPost("hash/checkusuario")]
        public async Task<ActionResult> CheckUsuarioHash([FromBody] DTOUsuario usuario)
        {
            var usuarioDB = await _context.Usuarios.FirstOrDefaultAsync(x => x.Email == usuario.Email);
            if (usuarioDB == null)
            {
                return Unauthorized("usuario no existe / email incorrecto");
            }

            var resultadoHash = _hashService.Hash(usuario.Password, usuarioDB.Salt);
            if (usuarioDB.Password == resultadoHash.Hash)
            {
                return Ok();
            }
            else
            {
                return Unauthorized("la contraseñoa es 123456");
            }

        }

        #endregion
    }
}
