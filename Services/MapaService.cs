using MauiAppAMASBE.Models;
using System.Text.Json;

namespace MauiAppAMASBE.Services
{
    public class MapaService
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(12) };

        private const string CacheUbs      = "mapa_cache_ubs";
        private const string CacheUbsTs    = "mapa_cache_ubs_ts";
        private const string CacheParques  = "mapa_cache_parques";
        private const string CacheParquesTs = "mapa_cache_parques_ts";
        private static readonly TimeSpan ValidadeCache = TimeSpan.FromHours(24);

        public async Task<List<LocalizacaoItem>> BuscarTodosAsync(double lat, double lon, double raioKm)
        {
            var resultados = await Task.WhenAll(
                BuscarUBSsAsync(lat, lon, raioKm),
                BuscarParquesAsync(lat, lon, raioKm)
            );
            return resultados[0].Concat(resultados[1])
                .OrderBy(l => Distancia(lat, lon, l.Latitude, l.Longitude))
                .ToList();
        }

        public async Task<List<LocalizacaoItem>> BuscarUBSsAsync(double lat, double lon, double raioKm)
        {
            var cache = LerCache<List<LocalizacaoItem>>(CacheUbs, CacheUbsTs);
            if (cache != null) return FiltrarRaio(cache, lat, lon, raioKm);

            try
            {
                var latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var url = $"https://apidadosnet.saude.gov.br/api/v1/estabelecimentos" +
                          $"?lat={latStr}&lng={lonStr}&raio={raioKm}&tp_unidade=1&limit=50";

                var json = await _http.GetStringAsync(url);
                var resposta = JsonSerializer.Deserialize<DadosSusResposta>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resposta?.Items?.Count > 0)
                {
                    var lista = resposta.Items
                        .Where(i => i.NuLatitude != 0 && i.NuLongitude != 0)
                        .Select(i => new LocalizacaoItem
                        {
                            Nome      = i.NoFantasia ?? i.NoRazaoSocial ?? "UBS",
                            Endereco  = $"{i.DsLogradouro}, {i.NuEndereco} — {i.NoBairro}",
                            Latitude  = i.NuLatitude,
                            Longitude = i.NuLongitude,
                            Telefone  = i.NuTelefone ?? "",
                            Horario   = "Seg–Sex: 07h às 19h",
                            Tipo      = "UBS"
                        }).ToList();

                    SalvarCache(CacheUbs, CacheUbsTs, lista);
                    return lista;
                }
            }
            catch { }

            return new List<LocalizacaoItem>();
        }

        public async Task<List<LocalizacaoItem>> BuscarParquesAsync(double lat, double lon, double raioKm)
        {
            var cache = LerCache<List<LocalizacaoItem>>(CacheParques, CacheParquesTs);
            if (cache != null) return FiltrarRaio(cache, lat, lon, raioKm);

            try
            {
                int raioM = (int)(raioKm * 1000);
                var latStr = lat.ToString(System.Globalization.CultureInfo.InvariantCulture);
                var lonStr = lon.ToString(System.Globalization.CultureInfo.InvariantCulture);

                string query = $@"[out:json][timeout:20];
(
  node[""leisure""=""park""](around:{raioM},{latStr},{lonStr});
  way[""leisure""=""park""](around:{raioM},{latStr},{lonStr});
  node[""leisure""=""playground""](around:{raioM},{latStr},{lonStr});
  node[""leisure""=""recreation_ground""](around:{raioM},{latStr},{lonStr});
);
out center 30;";

                var content = new FormUrlEncodedContent(
                    new[] { new KeyValuePair<string, string>("data", query) });
                var resp = await _http.PostAsync("https://overpass-api.de/api/interpreter", content);
                var json = await resp.Content.ReadAsStringAsync();

                var resultado = JsonSerializer.Deserialize<OverpassResposta>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultado?.Elements?.Count > 0)
                {
                    var lista = resultado.Elements
                        .Select(el =>
                        {
                            double eLat = el.Lat ?? el.Center?.Lat ?? 0;
                            double eLon = el.Lon ?? el.Center?.Lon ?? 0;
                            if (eLat == 0) return null;
                            return new LocalizacaoItem
                            {
                                Nome      = el.Tags?.GetValueOrDefault("name") ?? "Parque",
                                Endereco  = el.Tags?.GetValueOrDefault("addr:full") ?? "Ver no mapa",
                                Latitude  = eLat,
                                Longitude = eLon,
                                Tipo      = "Parque"
                            };
                        })
                        .Where(p => p != null).Cast<LocalizacaoItem>()
                        .ToList();

                    SalvarCache(CacheParques, CacheParquesTs, lista);
                    return lista;
                }
            }
            catch { }

            return new List<LocalizacaoItem>();
        }

        // ── Cache ─────────────────────────────────────────────────────
        private static void SalvarCache<T>(string chave, string chaveTs, T dados)
        {
            try
            {
                Preferences.Set(chave, JsonSerializer.Serialize(dados));
                Preferences.Set(chaveTs, DateTime.UtcNow.Ticks.ToString());
            }
            catch { }
        }

        private static T? LerCache<T>(string chave, string chaveTs)
        {
            try
            {
                var tsStr = Preferences.Get(chaveTs, "");
                if (string.IsNullOrEmpty(tsStr)) return default;
                if (DateTime.UtcNow - new DateTime(long.Parse(tsStr), DateTimeKind.Utc) > ValidadeCache)
                    return default;
                var json = Preferences.Get(chave, "");
                return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
            }
            catch { return default; }
        }

        // ── Utilitários ───────────────────────────────────────────────
        public static List<LocalizacaoItem> FiltrarRaio(
            List<LocalizacaoItem> lista, double lat, double lon, double raioKm) =>
            lista.Where(l => Distancia(lat, lon, l.Latitude, l.Longitude) <= raioKm).ToList();

        public static double Distancia(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            double dLat = (lat2 - lat1) * Math.PI / 180;
            double dLon = (lon2 - lon1) * Math.PI / 180;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                     + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
                     * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            return R * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }
    }

    // DTOs
    public class DadosSusResposta { public List<DadosSusItem> Items { get; set; } = new(); }
    public class DadosSusItem
    {
        public string? NoFantasia { get; set; }
        public string? NoRazaoSocial { get; set; }
        public string? DsLogradouro { get; set; }
        public string? NuEndereco { get; set; }
        public string? NoBairro { get; set; }
        public double NuLatitude { get; set; }
        public double NuLongitude { get; set; }
        public string? NuTelefone { get; set; }
    }
    public class OverpassResposta { public List<OverpassElement> Elements { get; set; } = new(); }
    public class OverpassElement
    {
        public long Id { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public OverpassCenter? Center { get; set; }
        public Dictionary<string, string>? Tags { get; set; }
    }
    public class OverpassCenter { public double Lat { get; set; } public double Lon { get; set; } }
}
