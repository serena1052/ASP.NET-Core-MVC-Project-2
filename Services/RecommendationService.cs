using System.Collections.Generic;
using System.Linq;
using _1131605.Data;
using _1131605.Models;

namespace _1131605.Services
{
    public class RecommendationService
    {
        private readonly AppDbContext _context;

        public RecommendationService(AppDbContext context)
        {
            _context = context;
        }

        public List<Item> GetTop(string category)
        {
            return _context.Items
                .Where(x => x.Category == category)
                .Select(i => new
                {
                    Item = i,
                    Score = (i.Rating * 2) +
                            (_context.UserActions.Count(a => a.ItemId == i.ItemId && a.ActionType == "Like") * 3) +
                            (_context.Favorites.Count(f => f.ItemId == i.ItemId) * 4)
                })
                .OrderByDescending(x => x.Score)
                .Select(x => x.Item)
                .ToList();
        }
    }
}