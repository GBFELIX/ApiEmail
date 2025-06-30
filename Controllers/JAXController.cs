using System.IO;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APIEMAIL.Data;
using APIEMAIL.Models;

namespace APIEMAIL.Controllers
{
    public class JAXController : Controller
    {
        private readonly AplicationDbContext _context;

        public JAXController(AplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Adicionar()
        {
            return PartialView("Adicionar");
        }

        [HttpPost]
        public IActionResult Adicionar(Pessoa pessoa)
        {
            if (ModelState.IsValid)
            {
                _context.Pessoas.Add(pessoa);
                _context.SaveChanges();
                return Json(new { success = true, message = "Pessoa adicionada com sucesso!" });
            }
            return PartialView("Adicionar", pessoa);
        }

        // Exemplo de Index
        public IActionResult Index()
        {
            var pessoas = _context.Pessoas.ToList();
            return View(pessoas);
        }
    }
}
