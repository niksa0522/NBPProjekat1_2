using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace NBP_1_2.Models
{
    public class FootballTeam
    {
        [DisplayName("ID")]
        public String id { get; set; }

        [DisplayName("Name")]
        public String name { get; set; }

        [DisplayName("Home Town")]
        public String homeTown { get; set; }

        [DisplayName("Stadium Name")]
        public String stadiumName { get; set; }

        public List<FootballPlayer> players { get; set; }


    }
}