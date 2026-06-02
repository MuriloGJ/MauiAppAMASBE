using MauiAppAMASBE.Models;
using System.Globalization;
using System.Text.Json;

namespace MauiAppAMASBE.Services
{
    public class MapaService
    {
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(15) };

        // Cache key inclui coordenadas e raio para evitar reuso indevido
        private string CacheKey(string tipo, double lat, double lon, double raio) =>
            $"mapa_{tipo}_{lat:F2}_{lon:F2}_{raio}";
        private string CacheTsKey(string tipo, double lat, double lon, double raio) =>
            $"mapa_{tipo}_{lat:F2}_{lon:F2}_{raio}_ts";

        private static readonly TimeSpan ValidadeCache = TimeSpan.FromHours(12);

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

        // ── UBS via Overpass API (OpenStreetMap) ─────────────────────
        // A API apidadosnet.saude.gov.br não é pública. Usamos Overpass
        // que retorna UBS/postos de saúde com dados reais.
        public async Task<List<LocalizacaoItem>> BuscarUBSsAsync(double lat, double lon, double raioKm)
        {
            var cacheKey = CacheKey("ubs", lat, lon, raioKm);
            var cacheTsKey = CacheTsKey("ubs", lat, lon, raioKm);
            var cache = LerCache<List<LocalizacaoItem>>(cacheKey, cacheTsKey);
            if (cache != null) return cache;

            try
            {
                int raioM = (int)(raioKm * 1000);
                var latStr = lat.ToString(CultureInfo.InvariantCulture);
                var lonStr = lon.ToString(CultureInfo.InvariantCulture);

                // Consulta Overpass: UBS, postos de saúde, clínicas
                string query = $@"[out:json][timeout:25];
(
  node[""amenity""=""clinic""](around:{raioM},{latStr},{lonStr});
  node[""amenity""=""health_post""](around:{raioM},{latStr},{lonStr});
  node[""amenity""=""doctors""](around:{raioM},{latStr},{lonStr});
  node[""healthcare""=""centre""](around:{raioM},{latStr},{lonStr});
  node[""healthcare""=""clinic""](around:{raioM},{latStr},{lonStr});
  way[""amenity""=""clinic""](around:{raioM},{latStr},{lonStr});
  way[""amenity""=""health_post""](around:{raioM},{latStr},{lonStr});
  way[""healthcare""=""centre""](around:{raioM},{latStr},{lonStr});
);
out center 40;";

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

                            var nome = el.Tags?.GetValueOrDefault("name")
                                    ?? el.Tags?.GetValueOrDefault("operator")
                                    ?? "UBS / Posto de Saúde";

                            var rua     = el.Tags?.GetValueOrDefault("addr:street") ?? "";
                            var numero  = el.Tags?.GetValueOrDefault("addr:housenumber") ?? "";
                            var bairro  = el.Tags?.GetValueOrDefault("addr:suburb")
                                       ?? el.Tags?.GetValueOrDefault("addr:neighbourhood") ?? "";
                            var endereco = string.Join(", ", new[]{ rua, numero, bairro }
                                            .Where(s => !string.IsNullOrWhiteSpace(s)));
                            if (string.IsNullOrWhiteSpace(endereco)) endereco = "Ver no mapa";

                            var telefone = el.Tags?.GetValueOrDefault("phone")
                                        ?? el.Tags?.GetValueOrDefault("contact:phone") ?? "";
                            var horario  = el.Tags?.GetValueOrDefault("opening_hours") ?? "";

                            return new LocalizacaoItem
                            {
                                Nome      = nome,
                                Endereco  = endereco,
                                Latitude  = eLat,
                                Longitude = eLon,
                                Telefone  = telefone,
                                Horario   = horario,
                                Tipo      = "UBS"
                            };
                        })
                        .Where(p => p != null).Cast<LocalizacaoItem>()
                        .ToList();

                    SalvarCache(cacheKey, cacheTsKey, lista);
                    return lista;
                }
            }
            catch { }

            return new List<LocalizacaoItem>();
        }

        // ── Parques via Overpass API ──────────────────────────────────
        public async Task<List<LocalizacaoItem>> BuscarParquesAsync(double lat, double lon, double raioKm)
        {
            var cacheKey = CacheKey("parques", lat, lon, raioKm);
            var cacheTsKey = CacheTsKey("parques", lat, lon, raioKm);
            var cache = LerCache<List<LocalizacaoItem>>(cacheKey, cacheTsKey);
            if (cache != null) return cache;

            try
            {
                int raioM = (int)(raioKm * 1000);
                var latStr = lat.ToString(CultureInfo.InvariantCulture);
                var lonStr = lon.ToString(CultureInfo.InvariantCulture);

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
                                Endereco  = el.Tags?.GetValueOrDefault("addr:full")
                                          ?? el.Tags?.GetValueOrDefault("addr:street")
                                          ?? "Ver no mapa",
                                Latitude  = eLat,
                                Longitude = eLon,
                                Tipo      = "Parque"
                            };
                        })
                        .Where(p => p != null).Cast<LocalizacaoItem>()
                        .ToList();

                    SalvarCache(cacheKey, cacheTsKey, lista);
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

    // ── DTOs Overpass ─────────────────────────────────────────────────
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
