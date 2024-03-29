﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AdvisorManagement.Models.ViewModel
{
    public class ListStudent
    {
        public int id { get; set; }

        public int idAcc { get; set; }
        public string idStudent { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string phone { get; set; }

        public string address { get; set; }
        public string gender { get; set; }
        public string status { get; set; }
    }
}