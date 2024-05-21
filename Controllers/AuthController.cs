using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProEvoStats_EVO7.Data;
using ProEvoStats_EVO7.Models;
using ProEvoStats_EVO7.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ProEvoStats_EVO7.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Register(Jogador model, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                ViewBag.ConfirmPassword = "True";
                if (model.Password != confirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "As senhas não correspondem.");
                    ViewBag.ConfirmPassword = "False";
                    return View("Login", model);
                }

                var existingUser = await _context.Jogadores
                    .FirstOrDefaultAsync(j => j.Username == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "O nome de usuário já está em uso.");
                    return View("Login", model);
                }
                var jogador = new Jogador
                {
                    Username = model.Username,
                    Password = HashUtils.ComputeSha256Hash(model.Password),
                    Email = model.Email ?? "",
                    Role = model.Role,
                    EquipaPrefId = model.EquipaPrefId,
                    Status = model.Status
                };
                _context.Jogadores.Add(jogador);
                await _context.SaveChangesAsync();

                return RedirectToAction ("UserProfile", "User");
            }

            return View("Login", model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(Jogador model)
        {
            if (ModelState.IsValid)
            {
                var hashedPassword = HashUtils.ComputeSha256Hash(model.Password);
                var jogador = await _context.Jogadores
                    .FirstOrDefaultAsync(j => j.Username == model.Username && j.Password == hashedPassword);

                if (jogador == null)
                {
                    ModelState.AddModelError("", "O nome de usuário ou a senha estão incorretos.");
                    return View(model);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, jogador.Username),
                    new Claim(ClaimTypes.Role, jogador.Role.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, jogador.Id.ToString())
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                if (jogador.Role == Role.Admin)
                {
                    return RedirectToAction("ChooseRole");
                }
                else
                {
                    return RedirectToAction("UserProfile", "User");
                }
            }

            return View("Login", model);
        }


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Register", "Auth");
        }

        public IActionResult ChooseRole()
        {
            return View();
        }

    }
}
