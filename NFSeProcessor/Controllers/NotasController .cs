using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NFSeProcessor.Data;
using NFSeProcessor.DTOs;
using NFSeProcessor.Models;
using NFSeProcessor.Services;
using System;

namespace NFSeProcessor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly XmlProcessorService _xmlProcessor;
        private readonly ValidationService _validator;

        public NotasController(AppDbContext context)
        {
            _context = context;
            _xmlProcessor = new XmlProcessorService();
            _validator = new ValidationService();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<NotaFiscal>>> GetNotas()
        {
            var notas = await _context.NotaFiscal
                .OrderByDescending(n => n.DataEmissao)
                .ToListAsync();

            return Ok(notas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NotaFiscal>> GetNota(int id)
        {
            var nota = await _context.NotaFiscal.FindAsync(id);

            if (nota == null)
                return NotFound(new { message = "Nota fiscal não encontrada" });

            return Ok(nota);
        }

        [HttpPost("processar-xml")]
        public async Task<ActionResult> ProcessarXml([FromBody] string xmlContent)
        {
            try
            {
                var notaDto = _xmlProcessor.ExtractFromXml(xmlContent);

                var validationResult = _validator.ValidateNota(notaDto);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Validação falhou",
                        errors = validationResult.Errors
                    });
                }

                var existe = await _context.NotaFiscal
                    .AnyAsync(n => n.Numero == notaDto.Numero);

                if (existe)
                {
                    return Conflict(new
                    {
                        message = $"Nota fiscal {notaDto.Numero} já processada"
                    });
                }

                var notaFiscal = new NotaFiscal
                {
                    Numero = notaDto.Numero,
                    CNPJPrestador = notaDto.CNPJPrestador,
                    CNPJTomador = notaDto.CNPJTomador,
                    DataEmissao = notaDto.DataEmissao,
                    DescricaoServico = notaDto.DescricaoServico,
                    ValorTotal = notaDto.ValorTotal,
                    DataProcessamento = DateTime.Now
                };

                _context.NotaFiscal.Add(notaFiscal);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetNota),
                    new { id = notaFiscal.Id },
                    notaFiscal);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = "Erro ao processar XML",
                    error = ex.Message
                });
            }
        }

        [HttpPost("processar-multiplos")]
        public async Task<ActionResult> ProcessarMultiplosXmls([FromBody] List<string> xmlContents)
        {
            var resultados = new List<object>();

            foreach (var xml in xmlContents)
            {
                try
                {
                    var notaDto = _xmlProcessor.ExtractFromXml(xml);
                    var validationResult = _validator.ValidateNota(notaDto);

                    if (!validationResult.IsValid)
                    {
                        resultados.Add(new
                        {
                            numero = notaDto.Numero,
                            sucesso = false,
                            erros = validationResult.Errors
                        });
                        continue;
                    }

                    var existe = await _context.NotaFiscal
                        .AnyAsync(n => n.Numero == notaDto.Numero);

                    if (existe)
                    {
                        resultados.Add(new
                        {
                            numero = notaDto.Numero,
                            sucesso = false,
                            erros = new[] { "Nota já processada" }
                        });
                        continue;
                    }

                    var notaFiscal = new NotaFiscal
                    {
                        Numero = notaDto.Numero,
                        CNPJPrestador = notaDto.CNPJPrestador,
                        CNPJTomador = notaDto.CNPJTomador,
                        DataEmissao = notaDto.DataEmissao,
                        DescricaoServico = notaDto.DescricaoServico,
                        ValorTotal = notaDto.ValorTotal
                    };

                    _context.NotaFiscal.Add(notaFiscal);
                    await _context.SaveChangesAsync();

                    resultados.Add(new
                    {
                        numero = notaDto.Numero,
                        sucesso = true,
                        id = notaFiscal.Id
                    });
                }
                catch (Exception ex)
                {
                    resultados.Add(new
                    {
                        numero = "desconhecido",
                        sucesso = false,
                        erros = new[] { ex.Message }
                    });
                }
            }

            return Ok(new
            {
                totalProcessado = xmlContents.Count,
                resultados
            });
        }
    }
}