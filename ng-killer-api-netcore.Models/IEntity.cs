using System;
using System.Collections.Generic;
using System.Text;

namespace NgKillerApiCore.Models
{
    /// <summary>
    /// Détermine une instance identifiable
    /// </summary>
    /// <typeparam name="K">type de la clef</typeparam>
    public interface IEntity<out K>
    {
        K Id { get; }
    }
}
