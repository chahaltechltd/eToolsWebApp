﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SalesSystem.Entities
{
    public partial class Province
    {
        public Province()
        {
            Vendors = new HashSet<Vendor>();
        }

        [Key]
        [StringLength(2)]
        public string ProvinceID { get; set; }
        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [InverseProperty("Province")]
        public virtual ICollection<Vendor> Vendors { get; set; }
    }
}