using System;

namespace _1131605.Models
{
    public class DecisionHistory
    {
        public int DecisionHistoryId { get; set; }
        public int UserId { get; set; }
        public int ItemId { get; set; }
        public string Mode { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}