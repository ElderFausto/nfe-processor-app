using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NfeProcessor.Data;
using NfeProcessor.Models;
using NfeProcessor.Services;

namespace NfeProcessor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NfeController : ControllerBase
    {
        private readonly NfeService _nfeService;
        private readonly NfeDbContext _context; // Já deve estar aqui

        public NfeController(NfeService nfeService, NfeDbContext context)
        {
            _nfeService = nfeService;
            _context = context;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadNfe(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Nenhum arquivo enviado.");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".xml")
            {
                return BadRequest("Formato de arquivo inválido. Por favor, envie um .xml");
            }

            try
            {
                await using var stream = file.OpenReadStream();
                var nfe = await _nfeService.ProcessNfe(stream);
                return Ok(nfe);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllNfes()
        {
            try
            {
                var nfes = await _context.Nfes
                                    .Include(n => n.Products)
                                    .AsNoTracking()
                                    .ToListAsync();
                return Ok(nfes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar as notas: {ex.Message}");
            }
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetStats()
        {
            try
            {
                var totalNotes = await _context.Nfes.CountAsync();
                var totalValue = await _context.Nfes.SumAsync(n => n.TotalValue);
                var totalIcms = await _context.Nfes.SumAsync(n => n.IcmsValue);
                var totalIpi = await _context.Nfes.SumAsync(n => n.IpiValue);

                return Ok(new
                {
                    totalNotes,
                    totalValue,
                    totalIcms,
                    totalIpi
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar as estatísticas: {ex.Message}");
            }
        }
        [HttpDelete("{accessKey}")] // Rota para deletar uma NF-e pelo AccessKey
        public async Task<IActionResult> DeleteNfe(string accessKey)
        {
            try
            {
                // Busca a NF-e no banco de dados
                var nfe = await _context.Nfes.FindAsync(accessKey);

                if (nfe == null)
                {
                    return NotFound("NF-e não encontrada.");
                }

                // Remove a NF-e. O EF Core removerá os produtos em cascata.
                _context.Nfes.Remove(nfe);
                await _context.SaveChangesAsync();

                // Retorna 'No Content', que é o padrão HTTP 204 para um DELETE bem-sucedido
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao excluir a nota: {ex.Message}");
            }
        }

        [HttpGet("export")]
        public IActionResult Export()
        {
            try
            {
                var fileBytes = _nfeService.ExportToExcel();

                string fileName = $"NFe_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                // Define o tipo de conteúdo (MIME type) para um arquivo Excel
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

                // Retorna o arquivo para download
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao exportar o arquivo: {ex.Message}");
            }
        }
    }
}