sing NFSeProcessor.DTOs;
using System.Text.RegularExpressions;

namespace NFSeProcessor.Services
{
    public class ValidationService
    {
        public ValidationResult ValidateNota(NotaFiscalDto nota)
        {
            var result = new ValidationResult { IsValid = true };

            // Validar número da nota
            if (string.IsNullOrWhiteSpace(nota.Numero))
            {
                result.Errors.Add("Número da nota é obrigatório");
                result.IsValid = false;
            }

            // Validar CNPJ Prestador
            if (!ValidarCNPJ(nota.CNPJPrestador))
            {
                result.Errors.Add($"CNPJ do prestador inválido: {nota.CNPJPrestador}");
                result.IsValid = false;
            }

            // Validar CNPJ Tomador
            if (!ValidarCNPJ(nota.CNPJTomador))
            {
                result.Errors.Add($"CNPJ do tomador inválido: {nota.CNPJTomador}");
                result.IsValid = false;
            }

            // Validar data
            if (nota.DataEmissao == default(DateTime))
            {
                result.Errors.Add("Data de emissão inválida");
                result.IsValid = false;
            }

            if (nota.DataEmissao > DateTime.Now)
            {
                result.Errors.Add("Data de emissão não pode ser futura");
                result.IsValid = false;
            }

            // Validar descrição
            if (string.IsNullOrWhiteSpace(nota.DescricaoServico))
            {
                result.Errors.Add("Descrição do serviço é obrigatória");
                result.IsValid = false;
            }

            // Validar valor
            if (nota.ValorTotal <= 0)
            {
                result.Errors.Add("Valor total deve ser maior que zero");
                result.IsValid = false;
            }

            return result;
        }

        private bool ValidarCNPJ(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                return false;

            // Remove caracteres não numéricos
            cnpj = Regex.Replace(cnpj, @"[^\d]", "");

            // Deve ter 14 dígitos
            if (cnpj.Length != 14)
                return false;

            // Verifica se todos os dígitos são iguais
            if (cnpj.All(c => c == cnpj[0]))
                return false;

            // Valida dígitos verificadores
            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            string tempCnpj = cnpj.Substring(0, 12);
            int soma = 0;

            for (int i = 0; i < 12; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

            int resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;

            string digito = resto.ToString();
            tempCnpj += digito;
            soma = 0;

            for (int i = 0; i < 13; i++)
                soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            resto = resto < 2 ? 0 : 11 - resto;
            digito += resto.ToString();

            return cnpj.EndsWith(digito);
        }
    }
}