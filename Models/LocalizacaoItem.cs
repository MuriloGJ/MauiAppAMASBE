namespace MauiAppAMASBE.Models
{
    /// <summary>
    /// Representa um local no mapa — UBS ou Parque de lazer.
    /// </summary>
    public class LocalizacaoItem
    {
        public string Nome { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Tipo { get; set; } = string.Empty; // "UBS" ou "Parque"
        public string Telefone { get; set; } = string.Empty;
        public string Horario { get; set; } = string.Empty;

        // Emoji exibido no card
        public string Icone => Tipo == "UBS" ? "🏥" : "🌳";
    }
}
