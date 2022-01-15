using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NBP_1_2.Models
{
    public class PlayedIn
    {
        public Round match { get; set; }

        public FootballPlayer player {get;set;}

        public float score { get; set; }
    }
}