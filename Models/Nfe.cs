using System.ComponentModel.DataAnnotations;

namespace NfeProcessor.Models
{
    public class Nfe
    {
        [Key]
        public string AccessKey { get; set; } = string.Empty; // ChaveAcesso

        public int Number { get; set; } // Numero
        public DateTime IssueDate { get; set; } // DataEmissao

        public string IssuerName { get; set; } = string.Empty; // EmitenteNome
        public string IssuerCNPJ { get; set; } = string.Empty; // EmitenteCNPJ

        public string RecipientName { get; set; } = string.Empty; // DestinatarioNome
        public string RecipientCNPJ { get; set; } = string.Empty; // DestinatarioCNPJ

        public decimal TotalValue { get; set; } // ValorTotal
        public decimal IcmsValue { get; set; } // ValorIcms
        public decimal IpiValue { get; set; } // ValorIpi

        // Relação: Uma Nfe tem muitos NfeProducts
        public List<NfeProduct> Products { get; set; } = new List<NfeProduct>(); // Produtos
    }
}