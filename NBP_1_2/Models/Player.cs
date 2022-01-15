using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NBP_1_2.Models
{
    public class Player
    {
        [DisplayName("Username")]
        [Required(ErrorMessage ="This field is required")]
        public String username { get; set; }

        [DisplayName("Password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "This field is required")]
        public String password { get; set; }

        public String userType { get; set; }

        public double money { get; set; }

        [DisplayName("Score")]
        public float score { get; set; }

        public List<Player> isFriends { get; set; }

        public List<FootballPlayer> inTeam { get; set; }

        public List<FootballTeam> following { get; set; }

        public override string ToString()
        {
            return username;
        }

        public String LoginErrorMessage { get; set; }
    }
}