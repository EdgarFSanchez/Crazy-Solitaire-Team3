using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazySolitaire
{
    /// <summary>
    /// Tracks store ownership for the current game session.
    /// Prevents double-charging and answers "is this owned?"
    /// </summary>
    public static class StoreSession
    {
        // IDs of backgrounds purchased during this run
        private static readonly HashSet<string> _ownedBackgroundIds = new();

        /// <summary>
        /// Returns true if the player already owns this background
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsBackgroundOwned(string id) => _ownedBackgroundIds.Contains(id);

        /// <summary>
        /// Attempts to buy a background once per session.
        /// -> if already owned, then succeed without charging.
        /// -> if not enough credits, then fail with an error message.
        /// -> otherwise, deduct and mark as owned.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="price"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool TryPurchaseBackground(string id, int price, out string error)
        {
            error = "";

            // Already owned then no charge
            if (IsBackgroundOwned(id)) return true;

            // Not enough credit
            if (ScoreManager.Score < price)
            {
                error = $"You need {price} Social Credit to buy this background.";
                return false;
            }

            // Charge and give ownership
            ScoreManager.SubtractPoints(price);
            _ownedBackgroundIds.Add(id);
            return true;
        }
    }
}
