using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Auth.Domain.Enums;

/// <summary>
/// Rôles disponibles dans le système.
/// 
/// Pourquoi un enum et pas des strings ?
/// → Typage fort — impossible d'écrire "Admni" par erreur
/// → IntelliSense — Visual Studio propose les valeurs
/// → Refactoring facile — changer un nom = changer partout
/// </summary>
public enum UserRole
{
    /// <summary>Super administrateur — accès total</summary>
    SuperAdmin = 0,

    /// <summary>Administrateur du tenant</summary>
    TenantAdmin = 1,

    /// <summary>Utilisateur standard</summary>
    User = 2,

    /// <summary>Accès lecture seule</summary>
    ReadOnly = 3
}