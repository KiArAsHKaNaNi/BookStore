using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookStore.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }

        //CompanyId is nullabe bacause indivusual users wont have company
        [ForeignKey("CompanyId")]
        public int? CompanyId { get; set; }
        public Company Company { get; set; }


        [NotMapped]
        public string Role { get; set; }
    }
}
