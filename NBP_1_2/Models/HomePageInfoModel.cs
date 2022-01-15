using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NBP_1_2.Models
{
    public class HomePageInfoModel
    {
        [DisplayName("Username")]
        public string username { get; set; }

        [DisplayName("UserType")]
        public string UserType { get; set; }

        [DisplayName("OnlinePlayers")]
        public long onlinePlayers { get; set; }
    }
}
