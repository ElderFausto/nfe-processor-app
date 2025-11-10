using Microsoft.AspNetCore.Mvc;
using NfeProcessor.Services;

namespace NfeProcessor.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Define a rota base como /api/Nfe
    public class NfeController : ControllerBase
    {
        private readonly NfeService _nfeService;

        // Injeta o serviço
        public NfeController(NfeService nfeService)
        {
            _nfeService = nfeService;
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
                // Abre o stream do arquivo e o passa para o serviço
                await using var stream = file.OpenReadStream();
                var nfe = await _nfeService.ProcessNfe(stream);

                // Retorna os dados processados
                return Ok(nfe); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao processar o arquivo: {ex.Message}");
            }
        }
    }
}