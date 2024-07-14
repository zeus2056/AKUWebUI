namespace AKUWebUI.Models.Filtered
{
	public class FilteredBankModel
	{
        public int BranchId { get; set; }
        public int BankId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
