using EntityLayer;

namespace AKUWebUI.Models.Staff
{
	public class ResultStaffModel
	{
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool Gender { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public string Phone { get; set; }
        public string TC { get; set; }
        public byte  Age{ get; set; }
        public string BranchName { get; set; }
        public Rol Role { get; set; }
        public string ImagePath { get; set; }
    }
}
