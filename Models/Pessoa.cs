using System;
using System.ComponentModel.DataAnnotations;

namespace APIEMAIL.Models
{
    public class Pessoa
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "A função é obrigatória.")]
        [StringLength(50, ErrorMessage = "A função deve ter no máximo 50 caracteres.")]
        public string Funcao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O salário é obrigatório.")]
        [Range(0, double.MaxValue, ErrorMessage = "Informe um salário válido.")]
        public decimal Salario { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }

    }
}
