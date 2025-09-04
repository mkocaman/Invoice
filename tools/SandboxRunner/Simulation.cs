// Türkçe Açıklama:
// Bu dosya simülasyon altyapısının merkezi etiketi/yardımcılarıdır.
// - SimulationCodeAttribute: Sadece simülasyon akışına ait kodları işaretler.
// - SimulationMarkers: Log ve içeriklerde "SIMULATION" izini standardize eder.
// - SimulationHelpers: JSON içeriklerine "simulation": true/false alanını ekler.
//
// English Summary:
// Central helpers for simulation labeling, log prefixing and JSON flag injection.

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Invoice.SandboxRunner;

/// <summary>
/// [SIMULATION] Etiketi: Sadece simülasyon amaçlı kullanılan kod noktalarını işaretler.
/// Bu attribute, kod aramalarında ve code review'da görünür kılsın diye eklidir.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Field | AttributeTargets.Property)]
public sealed class SimulationCodeAttribute : Attribute
{
    // Türkçe: İşlevsel davranış içermez; yalnızca dokümantif/analitik amaçlıdır.
}

/// <summary>
/// [SIMULATION] İmza ve metin sabitleri.
/// </summary>
public static class SimulationMarkers
{
    /// <summary>Log/rapor vurgusu için sabit metin.</summary>
    public const string Tag = "SIMULATION";

    /// <summary>JSON içeriklerine eklenecek alan adı.</summary>
    public const string JsonFieldName = "simulation";

    /// <summary>Güncel durumda simülasyonun açık/kapalı olduğunu dışarıya yansıt.</summary>
    public static string AsHuman(bool on) => on ? "Açık" : "Kapalı";
}

/// <summary>
/// [SIMULATION] JSON içeriklerine "simulation" alanını güvenle eklemek için yardımcı fonksiyonlar.
/// </summary>
public static class SimulationHelpers
{
    // Türkçe: İçeriğin JSON olduğu varsayılır; değilse olduğu gibi döndürür.
    public static string EnsureSimulationFlag(string json, bool simulation)
    {
        try
        {
            var node = JsonNode.Parse(json);
            if (node is null) return json;

            // Alan zaten varsa güncelle, yoksa ekle
            node[SimulationMarkers.JsonFieldName] = simulation;

            return node.ToJsonString(new JsonSerializerOptions
            {
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });
        }
        catch
        {
            // Türkçe: JSON parse edilemedi; içeriği bozmamak adına aynen geri ver.
            return json;
        }
    }
}
