using System.Collections.Generic;

namespace NgKillerApiCore.Models
{
    public class Game : IEntity<long>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }

        /// <summary>
        /// Liste des joueurs (Agents) pour la partie
        /// </summary>
        public List<Agent> Agents { get; set; }

        /// <summary>
        /// Toutes les missions disponibles pour la partie
        /// </summary>
        public List<Mission> Missions { get; set; }

        /// <summary>
        /// Toutes les actions passées dans la partie
        /// </summary>
        public List<Action> Actions { get; set; }
    }
}