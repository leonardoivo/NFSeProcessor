using System.Xml.Linq;
using NFSeProcessor.DTOs;
using NFSeProcessor.Models;

namespace NFSeProcessor.Services
{
    public class XmlProcessorService
    {
        public NotaFiscalDto ExtractFromXml(string xmlContent)
        {
            try
            {
                XDocument doc = XDocument.Parse(xmlContent);
                XElement root = doc.Root;

                if (root == null || root.Name.LocalName != "NotaFiscal")
                {
                    throw new Exception("XML inválido: elemento raiz 'NotaFiscal' não encontrado");
                }

                var nota = new NotaFiscalDto
                {
                    Numero = root.Element("Numero")?.Value ?? throw new Exception("Número da nota não encontrado"),
                    CNPJPrestador = root.Element("Prestador")?.Element("CNPJ")?.Value ?? throw new Exception("CNPJ do prestador não encontrado"),
                    CNPJTomador = root.Element("Tomador")?.Element("CNPJ")?.Value ?? throw new Exception("CNPJ do tomador não encontrado"),
                    DataEmissao = ParseData(root.Element("DataEmissao")?.Value),
                    DescricaoServico = root.Element("Servico")?.Element("Descricao")?.Value ?? throw new Exception("Descrição do serviço não encontrada"),
                    ValorTotal = ParseValor(root.Element("Servico")?.Element("Valor")?.Value)
                };

                return nota;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao processar XML: {ex.Message}", ex);
            }
        }

        private DateTime ParseData(string dataStr)
        {
            if (string.IsNullOrWhiteSpace(dataStr))
                throw new Exception("Data de emissão não encontrada");

            if (DateTime.TryParse(dataStr, out DateTime data))
                return data;

            throw new Exception($"Data inválida: {dataStr}");
        }

        private decimal ParseValor(string valorStr)
        {
            if (string.IsNullOrWhiteSpace(valorStr))
                throw new Exception("Valor não encontrado");

            if (decimal.TryParse(valorStr, out decimal valor))
                return valor;

            throw new Exception($"Valor inválido: {valorStr}");
        }
    }
}