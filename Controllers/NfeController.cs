using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NfeProcessor.Data;
using NfeProcessor.Services;

namespace NfeProcessor.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class NfeController : ControllerBase
  {
    private readonly NfeService _nfeService;
    private readonly NfeDbContext _context;

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
        // Retorna uma resposta de erro mais limpa
        return BadRequest(ex.Message);
      }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllNfes()
    {
      try
      {
        // Busca todas as notas no banco
        // 'Include(n => n.Products)' é como um JOIN que garante
        // que os produtos de cada nota sejam incluídos na resposta.
        var nfes = await _context.Nfes
                            .Include(n => n.Products)
                            .AsNoTracking() // "somente leitura"
                            .ToListAsync();

        return Ok(nfes);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Erro interno ao buscar as notas: {ex.Message}");
      }
    }
  }
}