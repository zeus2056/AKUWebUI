using EntityLayer.Entities;
using Microsoft.AspNetCore.Identity;

namespace EntityLayer
{
	public class AppUser : IdentityUser<int>
	{
        public int BranchId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool Gender { get; set; }
        public string Slug { get; set; }
        public string TC { get; set; }
        public string? ImagePath { get; set; }
        public DateTime EntryJobDate { get; set; }
        public DateTime Date { get; set; }
        public Branch Branch { get; set; }
        public List<Permission>  Permissions  { get; set; }
        public Rol Role { get; set; } = Rol.Teacher;
    }
}