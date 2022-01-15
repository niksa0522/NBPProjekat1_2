using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NBP_1_2.Models
{
    public class FootballPlayer
    {
        [DisplayName("ID")]
        public String id { get; set; }

        [DisplayName("Name")]
        public String name { get; set; }

        [DisplayName("Birthday")]
        public String birthday { get; set; }

        [DisplayName("Pozition")]
        public String pozition { get; set; }

        [DisplayName("Number")]
        public int number { get; set; }

        [DisplayName("Value")]
        public int value { get; set; }

        [DisplayName("PlayesIn")]
        public FootballTeam playesIn { get; set; }

        public DateTime getBirthday()
        {
            if (this.birthday == null) return new DateTime();

            long timestamp = Int64.Parse(this.birthday);
            DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return startDateTime.AddMilliseconds(timestamp).ToLocalTime();
        }

        public override string ToString()
        {
            return name;
        }
        [DisplayName("Number of players that have this person in their Team")]
        public long numOfPlayers { get; set; }
    }
}