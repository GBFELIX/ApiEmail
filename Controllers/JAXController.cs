using System.IO;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using APIEMAIL.Data;
using APIEMAIL.Models;
using System.Net.Mail;
using System.Net;

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
        public IActionResult Excluir(int id)
        {
            var pessoa = _context.Pessoas.Find(id);
            if (pessoa == null)
            {
                return NotFound();
            }
            return PartialView("Excluir", pessoa);
        }
        [HttpPost]
        public IActionResult ExcluirPessoa(int id)
        {
            var pessoa = _context.Pessoas.Find(id);
            if (pessoa == null) return NotFound();

            _context.Pessoas.Remove(pessoa);
            _context.SaveChanges();

            return Json(new { success = true, message = "Excluído com sucesso!" });
        }
        [HttpPost]
        [HttpGet]
        public IActionResult ExcluirVariosConfirmacao(int quantidade)
        {
            ViewBag.Quantidade = quantidade;
            return PartialView("ExcluirVarios");
        }

        [HttpPost]
        public IActionResult ExcluirVarios([FromBody] List<int> ids)
        {
            var pessoas = _context.Pessoas.Where(p => ids.Contains(p.Id)).ToList();
            if (!pessoas.Any())
                return NotFound();

            _context.Pessoas.RemoveRange(pessoas);
            _context.SaveChanges();

            return Json(new { success = true });
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
        [HttpGet]
        public IActionResult Editar(int id)
        {
            var pessoa = _context.Pessoas.Find(id);
            if (pessoa == null)
            {
                return NotFound();
            }
            return PartialView("Editar", pessoa);
        }
        [HttpPost]
        public IActionResult Editar(Pessoa pessoa)
        {
            if (ModelState.IsValid)
            {
                _context.Pessoas.Update(pessoa);
                _context.SaveChanges();
                return Json(new { success = true, message = "Pessoa editada com sucesso!" });
            }
            return PartialView("Editar", pessoa);
        }
        
        public IActionResult Index()
        {
            var pessoas = _context.Pessoas.ToList();
            return View(pessoas);
        }
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

            var fileName = "Selecionados.csv";
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
            ViewData["filtro"] = Pesquisar;
            return View("Index", pessoas.ToList());

        }
    }
}
