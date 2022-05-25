﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Uplift.Models
{
    public class LineItem
    {

        [Key]
        public Guid ID { get; set; }
        public DateTime CreateDate { get; set; }
        public String UserId { get; set; }

        [ForeignKey("UserId")]
        public IdentityUser IdentityUser { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }

    }
}