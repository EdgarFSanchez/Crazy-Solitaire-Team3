using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrazySolitaire
{
    public static class StoreSession
    {
        private static readonly HashSet<string> _ownedBackgroundIds = new();

        public static bool IsBackgroundOwned(string id) => _ownedBackgroundIds.Contains(id);

        public static bool TryPurchaseBackground(string id, int price, out string error)
        {
            error = "";

            // Already owned? Treat as success; do NOT charge again
            if (IsBackgroundOwned(id)) return true;

            if (ScoreManager.Score < price)
            {
                error = $"You need {price} Social Credit to buy this background.";
                return false;
            }

            ScoreManager.SubtractPoints(price);
            _ownedBackgroundIds.Add(id);
            return true;
        }
    }
}
