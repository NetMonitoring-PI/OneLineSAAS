using OneLine.AI.Application.Interfaces;

namespace OneLine.AI.Infrastructure.Services;

/// <summary>
/// Service LLM mock pour le developpement et les tests.
/// Retourne des reponses simulees sans appel API reel.
///
/// Utilisation :
///   - Quand AI:ApiKey est vide dans appsettings.json
///   - En environnement de test
///   - Pour tester le flow sans consommer des tokens
/// </summary>
public sealed class MockLLMService : ILLMService
{
    public string ProviderName => "Mock";
    public string DefaultModel => "mock-model";

    private static readonly string[] MockResponses =
    [
        "Bonjour ! Je suis l assistant IA OneLine. Comment puis-je vous aider ?",
        "C est une excellente question. Voici ma reponse simulee pour les tests.",
        "En mode developpement, je simule les reponses pour eviter les couts API.",
        "Pour utiliser un vrai LLM, configurez AI:ApiKey dans appsettings.json.",
        "Je peux vous aider avec de nombreuses taches : analyse, redaction, code..."
    ];

    private static int _responseIndex = 0;

    public Task<LLMResponse> ChatAsync(
        IReadOnlyList<LLMMessage> messages,
        string? model = null,
        CancellationToken ct = default)
    {
        var response = MockResponses[_responseIndex % MockResponses.Length];
        _responseIndex++;

        // Simuler les tokens (approximation : 1 token ~ 4 chars)
        var promptTokens = messages.Sum(m => m.Content.Length / 4);
        var completionTokens = response.Length / 4;

        return Task.FromResult(new LLMResponse(
            Content: response,
            PromptTokens: promptTokens,
            CompletionTokens: completionTokens,
            TotalTokens: promptTokens + completionTokens,
            Model: model ?? DefaultModel,
            Provider: ProviderName));
    }
}
