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
using System.Net.Mail;
using System.Net;


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
        public async Task<IActionResult> EnviarEmailAsync(int[] idsSelecionados)
        {
            var pessoas = _context.Pessoas.Where(p => idsSelecionados.Contains(p.Id)).ToList();
            
            var corpo = "Lista de pessoas selecionadas:\n\n";
            foreach (var pessoa in pessoas)
            {
                corpo += $"Nome: {pessoa.Nome}, Função: {pessoa.Funcao}, Salário: {pessoa.Salario}\n";
            }

            var mensagem = new MailMessage();

            //Try para simular erro devido a falta de configuração de email
            try
            {
                mensagem.From = new MailAddress("quem vai enviar");
                mensagem.To.Add("email destino");
                mensagem.Subject = "Pessoas selecionadas";
                mensagem.Body = corpo;

                using (var smtp = new SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new NetworkCredential("email", "senha");
                    smtp.EnableSsl = true;
                    try
                    {
                        await smtp.SendMailAsync(mensagem);
                        TempData["Mensagem"] = "Email enviado com sucesso!";
                    }
                    catch (Exception ex)
                    {
                        TempData["Erro"] = "Erro ao enviar email";
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Erro"] = "Erro ao enviar email";
            }
            
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Pesquisar(string pesquisa)
        {
            var pessoas = from p in _context.Pessoas
                          select p;
            if (!string.IsNullOrEmpty(pesquisa))
            {
                pessoas = pessoas.Where(p => p.Nome.Contains(pesquisa) || p.Funcao.Contains(pesquisa));
            }
            return View("Index", pessoas.ToList());











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
