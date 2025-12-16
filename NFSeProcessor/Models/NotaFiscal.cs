using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NFSeProcessor.Models
{
    [Table("NotaFiscal")]
    public class NotaFiscal
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Numero { get; set; }

        [Required]
        [StringLength(14)]
        public string CNPJPrestador { get; set; }

        [Required]
        [StringLength(14)]
        public string CNPJTomador { get; set; }

        [Required]
        public DateTime DataEmissao { get; set; }

        [Required]
        [StringLength(500)]
        public string DescricaoServico { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal ValorTotal { get; set; }

        public DateTime DataProcessamento { get; set; } = DateTime.Now;
    }
}
