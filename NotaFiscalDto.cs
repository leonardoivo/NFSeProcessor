namespace NFSeProcessor.DTOs
{
    public class NotaFiscalDto
    {
        public string Numero { get; set; }
        public string CNPJPrestador { get; set; }
        public string CNPJTomador { get; set; }
        public DateTime DataEmissao { get; set; }
        public string DescricaoServico { get; set; }
        public decimal ValorTotal { get; set; }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}