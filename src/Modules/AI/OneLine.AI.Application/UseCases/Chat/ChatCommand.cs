using MediatR;
using OneLine.AI.Application.DTOs;
using OneLine.Shared.Domain.Result;

namespace OneLine.AI.Application.UseCases.Chat;

/// <summary>
/// Commande pour envoyer un message a l IA.
///
/// Flow :
///   1. Verifier le quota mensuel du tenant
///   2. Recuperer ou creer la conversation
///   3. Ajouter le message utilisateur
///   4. Envoyer au LLM avec l historique
///   5. Sauvegarder la reponse + tracker les tokens
///   6. Retourner la reponse au client
/// </summary>
public sealed record ChatCommand(
    Guid TenantId,
    string Message,
    Guid? ConversationId = null,
    string? SystemPrompt = null,
    Guid? UserId = null
) : IRequest<Result<ChatResponseDto>>;
