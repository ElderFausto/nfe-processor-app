using System.Xml.Linq;
using NfeProcessor.Data;
using NfeProcessor.Models;
using System.Globalization;
using ClosedXML.Excel;
using System.IO;
using Microsoft.EntityFrameworkCore;

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
            XDocument doc = await XDocument.LoadAsync(xmlStream, System.Xml.Linq.LoadOptions.None, CancellationToken.None);
            
            XNamespace ns = "http://www.portalfiscal.inf.br/nfe";
            var culture = CultureInfo.InvariantCulture;

            var infNFeNode = doc.Descendants(ns + "infNFe").FirstOrDefault();
            if (infNFeNode == null)
            {
                throw new Exception("Tag <infNFe> não encontrada no XML.");
            }

            string accessKey = infNFeNode.Attribute("Id")?.Value.Replace("NFe", "") ?? string.Empty;
            if (string.IsNullOrEmpty(accessKey))
            {
                accessKey = doc.Descendants(ns + "chNFe").FirstOrDefault()?.Value ?? string.Empty;
            }

            if (string.IsNullOrEmpty(accessKey))
            {
                throw new Exception("Não foi possível extrair a Chave de Acesso (AccessKey) do XML.");
            }
            
            var nfeData = new Nfe
            {
                AccessKey = accessKey,
                Number = int.Parse(infNFeNode.Element(ns + "ide")?.Element(ns + "nNF")?.Value ?? "0"),
                IssueDate = DateTime.Parse(infNFeNode.Element(ns + "ide")?.Element(ns + "dhEmi")?.Value ?? DateTime.Now.ToString()),
                IssuerName = infNFeNode.Element(ns + "emit")?.Element(ns + "xNome")?.Value ?? string.Empty,
                IssuerCNPJ = infNFeNode.Element(ns + "emit")?.Element(ns + "CNPJ")?.Value ?? string.Empty,
                RecipientName = infNFeNode.Element(ns + "dest")?.Element(ns + "xNome")?.Value ?? string.Empty,
                RecipientCNPJ = infNFeNode.Element(ns + "dest")?.Element(ns + "CNPJ")?.Value ?? string.Empty,
                TotalValue = decimal.Parse(infNFeNode.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vNF")?.Value ?? "0", culture),
                IcmsValue = decimal.Parse(infNFeNode.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vICMS")?.Value ?? "0", culture),
                IpiValue = decimal.Parse(infNFeNode.Element(ns + "total")?.Element(ns + "ICMSTot")?.Element(ns + "vIPI")?.Value ?? "0", culture),

                Products = infNFeNode.Elements(ns + "det").Select(det => new NfeProduct
                {
                    ProductCode = det.Element(ns + "prod")?.Element(ns + "cProd")?.Value ?? string.Empty,
                    Name = det.Element(ns + "prod")?.Element(ns + "xProd")?.Value ?? string.Empty,
                    Quantity = (int)decimal.Parse(det.Element(ns + "prod")?.Element(ns + "qCom")?.Value ?? "0", culture),
                    UnitValue = decimal.Parse(det.Element(ns + "prod")?.Element(ns + "vUnCom")?.Value ?? "0", culture),
                    // ESTA É A LINHA QUE FOI CORRIGIDA (O 'D' FOI REMOVIDO)
                    TotalValue = decimal.Parse(det.Element(ns + "prod")?.Element(ns + "vProd")?.Value ?? "0", culture)
                }).ToList()
            };
            
            var existingNfe = await _context.Nfes.FindAsync(nfeData.AccessKey);
            if (existingNfe != null)
            {
                throw new Exception($"A NF-e número {nfeData.Number} (Chave: {nfeData.AccessKey}) já foi processada.");
            }

            await _context.Nfes.AddAsync(nfeData);
            await _context.SaveChangesAsync();

            return nfeData;
        }

        public byte[] ExportToExcel()
        {
            var nfes = _context.Nfes.Include(n => n.Products).ToList();
            
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Notas Fiscais");
                
                // Cabeçalho
                worksheet.Cell("A1").Value = "Número";
                worksheet.Cell("B1").Value = "Chave de Acesso";
                worksheet.Cell("C1").Value = "Data Emissão";
                worksheet.Cell("D1").Value = "Emitente";
                worksheet.Cell("E1").Value = "CNPJ Emitente";
                worksheet.Cell("F1").Value = "Destinatário";
                worksheet.Cell("G1").Value = "CNPJ Destinatário";
                worksheet.Cell("H1").Value = "Valor Total (R$)";
                worksheet.Cell("I1").Value = "ICMS (R$)";
                worksheet.Cell("J1").Value = "IPI (R$)";
                
                var headerRange = worksheet.Range("A1:J1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#4A5568");
                headerRange.Style.Font.FontColor = XLColor.White;

                // Corpo
                int currentRow = 2;
                foreach (var nfe in nfes)
                {
                    worksheet.Cell(currentRow, 1).Value = nfe.Number;
                    worksheet.Cell(currentRow, 2).Value = nfe.AccessKey;
                    worksheet.Cell(currentRow, 3).Value = nfe.IssueDate;
                    worksheet.Cell(currentRow, 4).Value = nfe.IssuerName;
                    worksheet.Cell(currentRow, 5).Value = nfe.IssuerCNPJ;
                    worksheet.Cell(currentRow, 6).Value = nfe.RecipientName;
                    worksheet.Cell(currentRow, 7).Value = nfe.RecipientCNPJ;
                    worksheet.Cell(currentRow, 8).Value = nfe.TotalValue;
                    worksheet.Cell(currentRow, 9).Value = nfe.IcmsValue;
                    worksheet.Cell(currentRow, 10).Value = nfe.IpiValue;
                    currentRow++;
                }

                worksheet.Columns().AdjustToContents();
                
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    return stream.ToArray();
                }
            }
        }
    }
}