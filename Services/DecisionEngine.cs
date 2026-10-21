using System;
using System.Linq;
using _1131605.Data;
using _1131605.Models;

namespace _1131605.Services
{
    public class DecisionEngine
    {
        private readonly AppDbContext _context;

        public DecisionEngine(AppDbContext context)
        {
            _context = context;
        }

        public Item Draw(string category)
        {
            var items = _context.Items
                .Where(x => x.Category == category && !x.IsBanned)
                .ToList();

            if (!items.Any()) return null;

            var random = new Random();

            // 🟢 核心修正：拋棄 SSR，改用「價格」與「評價」手工刻出 CP 值加權
            var weighted = items.Select(i => {
                double baseWeight = 1.0;

                // 評分篩選：4.5 分以上算頂級美味/優質，權重加 5
                if (i.Rating >= 4.5) baseWeight += 5.0;
                else if (i.Rating >= 4.0) baseWeight += 3.0; // 4.0~4.4 算中上，權重加 3

                // 價格篩選：預算友善型（小於 150 元），權重加 4；極度奢華型（大於 1000 元）權重降半
                if (i.Price <= 150) baseWeight += 4.0;
                else if (i.Price > 1000) baseWeight *= 0.5;

                return new { Item = i, Weight = baseWeight };
            }).ToList();

            double total = weighted.Sum(x => x.Weight);
            double roll = random.NextDouble() * total;

            double sum = 0;
            foreach (var w in weighted)
            {
                sum += w.Weight;
                if (roll <= sum)
                    return w.Item;
            }

            return items.First();
        }
    }
}