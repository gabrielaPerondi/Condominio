using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VizinhApp.Context;

namespace VizinhApp.Controllers
{
    public class UsuarioController:Controller
    {
        private readonly CondominioContext _context;

        public UsuarioController(CondominioContext context)
        {
            _context = context;
        }

         public IActionResult Login() => View();

        [HttpPost, AllowAnonymous]
        public async Task<IActionResult> Login(string email, string senha)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Mensagem = "Preencha e-mail e senha.";
                return View();
            }

            var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
            if (usuario == null || usuario.Senha != senha)
            {
                ViewBag.Mensagem = "Credenciais inválidas.";
                return View();
            }

            // Cria claims com a Role vinda do banco (Administrador / Síndico / Morador)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.TipoUsuario)
            };

            var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redireciona conforme o tipo
            return usuario.TipoUsuario switch
            {
                "Administrador" => RedirectToAction(nameof(AdminDashboard)),
                "Síndico"       => RedirectToAction(nameof(SindicoDashboard)),
                _               => RedirectToAction(nameof(MoradorDashboard))
            };
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AcessoNegado() => Content("Acesso negado.");

        // Dashboards protegidos por role (não abre por URL se não tiver a permissão)
        [Authorize(Roles = "Administrador")]
        public IActionResult AdminDashboard() => View();

        [Authorize(Roles = "Síndico")]
        public IActionResult SindicoDashboard() => View();

        [Authorize(Roles = "Morador")]
        public IActionResult MoradorDashboard() => View();

        
    }
}