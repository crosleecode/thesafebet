using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

using System.ComponentModel.DataAnnotations.Schema;

namespace SafeBet.Models
{
    [Table("UserMetrics")]
    public class Metrics
    {
        [Key]
        [Column("CookieId")]
        public string CookieId { get; set; } = string.Empty;

        [Column("GamesPlayed")]
        public int GamesPlayed { get; set; }

        [Column("AdviceRequests")]
        public int AdviceRequests { get; set; }

        [Column("BlackjackWins")]
        public int BlackjackWins { get; set; }

        [Column("BlackjackLosses")]
        public int BlackjackLosses { get; set; }

        [Column("AdvisedWins")]
        public int AdvisedWins { get; set; }

        [Column("AdvisedLosses")]
        public int AdvisedLosses { get; set; }

        [Column("NetEarnings", TypeName = "decimal(12,2)")]
        public decimal NetEarnings { get; set; }

        [Column("UpdatedTime")]
        public DateTime UpdatedTime { get; set; }

    }
}
