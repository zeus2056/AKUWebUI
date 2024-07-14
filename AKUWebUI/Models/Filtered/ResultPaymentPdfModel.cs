namespace AKUWebUI.Models.Filtered
{
    public class ResultPaymentPdfModel
    {
        public string FullName { get; set; }
        public string BranchName { get; set; }
        public string RateName { get; set; }
        public string AgeGroupName { get; set; }
        public double TotalPrice { get; set; }
        public double RemaningPrice { get; set; }
        public double LastPrice { get; set; }
    }
}
