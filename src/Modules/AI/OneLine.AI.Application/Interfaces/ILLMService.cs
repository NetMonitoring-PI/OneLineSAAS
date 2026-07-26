namespace OneLine.AI.Application.Interfaces;

/// <summary>
/// Abstraction du service LLM.
/// Pattern : Strategy â€” permet de changer de provider sans toucher Application.
///
/// Providers supportes :
///   - OpenAI (GPT-4o, GPT-3.5)
///   - Mistral (Mistral-7B, Mistral-Large)
///   - Groq (Llama, Mixtral â€” inference ultra-rapide)
///   - Ollama (modeles locaux â€” 100% prive)
/// </summary>
public interface ILLMService
{
    /// <summary>
    /// Envoie une liste de messages et retourne la reponse du LLM.
    /// </summary>
    Task<LLMResponse> ChatAsync(
        IReadOnlyList<LLMMessage> messages,
        string? model = null,
        CancellationToken ct = default);

    /// <summary>Nom du provider actif</summary>
    string ProviderName { get; }

    /// <summary>Modele par defaut</summary>
    string DefaultModel { get; }
}

public sealed record LLMMessage(string Role, string Content);

public sealed record LLMResponse(
    string Content,
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens,
    string Model,
    string Provider
);
