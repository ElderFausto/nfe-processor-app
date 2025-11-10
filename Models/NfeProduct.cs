namespace NfeProcessor.Models
{
    public class NfeProduct
    {
        public int Id { get; set; }
        public string ProductCode { get; set; } = string.Empty; // Codigo
        public string Name { get; set; } = string.Empty; // Nome
        public int Quantity { get; set; } // Quantidade
        public decimal UnitValue { get; set; } // ValorUnitario
        public decimal TotalValue { get; set; } // ValorTotal

        // Chave Estrangeira
        public string NfeAccessKey { get; set; } = string.Empty; // NfeChaveAcesso
        public Nfe Nfe { get; set; } = null!;
    }
}