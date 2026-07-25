using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLine.Shared.Domain.Interfaces;

/// <summary>
/// Abstraction pour la date et l'heure.
/// 
/// Pourquoi ne pas utiliser DateTime.UtcNow directement ?
/// 
/// DateTime.UtcNow en test → retourne l'heure RÉELLE
/// → impossible de tester "que se passe-t-il si le token expire ?"
/// 
/// Avec IDateTimeProvider :
/// → En prod : retourne DateTime.UtcNow
/// → En test : retourne la date qu'on veut (mock)
/// → On peut simuler n'importe quel scénario temporel
/// </summary>
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
