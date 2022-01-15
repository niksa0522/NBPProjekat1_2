using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NBP_1_2.Models
{
    public class Round
    {
        [DisplayName("ID")]
        public string id { get; set; }

        [DisplayName("Home Team")]
        public FootballTeam homeTeam { get; set; }

        [DisplayName("Away Team")]
        public FootballTeam awayTeam { get; set; }

        [DisplayName("Match Date")]
        public string matchDate { get; set; }

        [DisplayName("Round Number")]
        public int roundNum { get; set; }

        [DisplayName("Attendance")]
        public int attendance { get; set; }

        public List<PlayedIn> players { get; set; }

        public DateTime getDate()
        {
            if (this.matchDate == null) { return new DateTime(); }
            long timestamp = Int64.Parse(this.matchDate);
            DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return startDateTime.AddMilliseconds(timestamp).ToLocalTime();
        }

        [DisplayName("Home Team ID")]
        public string htID { get; set; }

        [DisplayName("Away Team ID")]
        public string atID { get; set; }
    }
}