using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Auth.Application.DTOs;

/// <summary>
/// Représentation publique d'un utilisateur.
/// Ne contient JAMAIS : PasswordHash, tokens, données sensibles.
///
/// DTO = Data Transfer Object
/// → Objet de transfert entre couches
/// → Seules les données nécessaires au client
/// </summary>
public sealed record UserDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    string FullName,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);
