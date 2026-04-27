namespace MauiAppAMASBE.Models
{
    public class LocalizacaoItem
    {
        public string Nome      { get; set; } = string.Empty;
        public string Endereco  { get; set; } = string.Empty;
        public double Latitude  { get; set; }
        public double Longitude { get; set; }
        public string Tipo      { get; set; } = string.Empty; // "UBS" ou "Parque"
        public string Telefone  { get; set; } = string.Empty;
        public string Horario   { get; set; } = string.Empty;

        public string Icone => Tipo == "UBS" ? "🏥" : "🌳";

        // Cor do marcador no mapa (hex sem #)
        public string CorMarcador => Tipo == "UBS" ? "2E86AB" : "4CAF50";
    }
}
