using MediatR;
using OneLine.AI.Application.DTOs;
using OneLine.Shared.Domain.Result;

namespace OneLine.AI.Application.UseCases.GetUsage;

public sealed record GetAIUsageQuery(Guid TenantId)
    : IRequest<Result<AIUsageDto>>;
