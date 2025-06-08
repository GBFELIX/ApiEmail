using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APIEMAIL.Data;
using APIEMAIL.Models;


namespace APIEMAIL.Controllers
{
    public class PessoasController : Controller
    {
        private readonly AppDbContext _context;

        public PessoasController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Pessoas.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }
        [HttpPost]
        public IActionResult EnviarSelecionados(int[] idsSelecionados)
        {
            var pessoasSelecionadas = _context.Pessoas
            .Where(p => idsSelecionados.Contains(p.Id))
            .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Nome;Função;Salário;Data de Nascimento");

            foreach (var pessoa in pessoasSelecionadas)
            {
                sb.AppendLine($"{pessoa.Nome};{pessoa.Funcao};{pessoa.Salario};{pessoa.DataNascimento: dd/MM/yyyy}");
            }

            var fileName = "PessoasSelecionadas.csv";
            var fileContent = Encoding.UTF8.GetBytes(sb.ToString());

            return File(fileContent, "text/csv", fileName);
        }
        [HttpPost]
        public IActionResult EnviarEmail(int[] idsSelecionados)
        {
        
            var pessoasSelecionadas = _context.Pessoas
                .Where(p => idsSelecionados.Contains(p.Id))
                .ToList();
                                          
            var sb = new StringBuilder();
            sb.AppendLine("Nome;Função;Salário;Data de Nascimento");

            foreach (var pessoa in pessoasSelecionadas)
            {
                sb.AppendLine($"{pessoa.Nome};{pessoa.Funcao};{pessoa.Salario};{pessoa.DataNascimento:dd/MM/yyyy}");
            }

            var conteudoSimulado = sb.ToString();

            TempData["Erro"] = "Erro ao conectar à API de e-mail. Envio não realizado.";

            return RedirectToAction("Index");
        }
        public IActionResult Create()
        {                         
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nome,Funcao,Salario,DataNascimento")] Pessoa pessoa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pessoa);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pessoa);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null)
            {
                return NotFound();
            }
            return View(pessoa);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Funcao,Salario,DataNascimento")] Pessoa pessoa)
        {
            if (id != pessoa.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pessoa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PessoaExists(pessoa.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(pessoa);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pessoa = await _context.Pessoas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pessoa == null)
            {
                return NotFound();
            }

            return View(pessoa);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa != null)
            {
                _context.Pessoas.Remove(pessoa);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PessoaExists(int id)
        {
            return _context.Pessoas.Any(e => e.Id == id);
        }
    }
}
