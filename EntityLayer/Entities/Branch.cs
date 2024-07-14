using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Branch 
	{
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public List<Student> Students { get; set; } = new();
        public List<AppUser> AppUsers { get; set; } = new();
        public List<Rate> Rates { get; set; }
    }
}
