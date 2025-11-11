using System.Xml.Linq;
using NfeProcessor.Data;
using NfeProcessor.Models;
using System.Globalization;

namespace NfeProcessor.Services
{
    public class NfeService
    {
        private readonly NfeDbContext _context;
        
        public NfeService(NfeDbContext context)
        {
            _context = context;
        }

        public async Task<Nfe> ProcessNfe(Stream xmlStream)
        {
            XDocument doc = await XDocument.LoadAsync(xmlStream, LoadOptions.None, CancellationToken.None);
            XNamespace ns = "http://www.portalfiscal.inf.br/nfe";

            var culture = CultureInfo.InvariantCulture;

            var nfeData = doc.Descendants(ns + "infNFe").Select(nfe => new Nfe
            {
                AccessKey = nfe.Attribute("Id")?.Value.Replace("NFe", "") ?? string.Empty,
                Number = int.Parse(nfe.Element(ns + "ide")?.Element(ns + "nNF")?.Value ?? "0"),
                IssueDate = DateTime.Parse(nfe.Element(ns + "ide")?.Element(ns + "dhEmi")?.Value ?? DateTime.Now.ToString()),

                IssuerName = nfe.Element(ns + "emit")?.Element(ns + "xNome")?.Value ?? string.Empty,
                IssuerCNPJ = nfe.Element(ns + "emit")?.Element(ns + "CNPJ")?.Value ?? string.Empty,

                RecipientName = nfe.Element(ns + "dest")?.Element(ns + "xNome")?.Value ?? string.Empty,
                RecipientCNPJ = nfe.Element(ns + "dest")?.Element(ns + "CNPJ")?.Value ?? string.Empty,

                TotalValue = decimal.Parse(nfe.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vNF")?.Value ?? "0", culture),
                IcmsValue = decimal.Parse(nfe.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vICMS")?.Value ?? "0", culture),
                IpiValue = decimal.Parse(nfe.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vIPI")?.Value ?? "0", culture),

                Products = nfe.Elements(ns + "det").Select(det => new NfeProduct
                {
                    ProductCode = det.Element(ns + "prod")?.Element(ns + "cProd")?.Value ?? string.Empty,
                    Name = det.Element(ns + "prod")?.Element(ns + "xProd")?.Value ?? string.Empty,
                    Quantity = (int)decimal.Parse(det.Element(ns + "prod")?.Element(ns + "qCom")?.Value ?? "0", culture),
                    UnitValue = decimal.Parse(det.Element(ns + "prod")?.Element(ns + "vUnCom")?.Value ?? "0", culture),
                    TotalValue = decimal.Parse(det.Element(ns + "prod")?.Element(ns + "vProd")?.Value ?? "0", culture)
                }).ToList()

            }).FirstOrDefault();

            if (nfeData == null)
            {
                throw new Exception("Não foi possível processar o XML da NF-e.");
            }

            // Verificação de duplicata
            // Verifica se uma NFe já existe no banco
            var existingNfe = await _context.Nfes.FindAsync(nfeData.AccessKey);
            if (existingNfe != null)
            {
                // Se a nota já existe, lança um erro amigável que será enviado ao frontend.
                throw new Exception($"A NF-e número {nfeData.Number} (Chave: {nfeData.AccessKey}) já foi processada.");
            }

            // Este código só será executado se a NFe for nova
            await _context.Nfes.AddAsync(nfeData);
            await _context.SaveChangesAsync();

            return nfeData;
        }
    }
}