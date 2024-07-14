using EntityLayer.Entities;

namespace AKUWebUI.Models
{
	public class ResultPaymentModel
	{
        public string RateName { get; set; }
        public double RatePrice { get; set; }
        public double RemaningPrice { get; set; }
        public int StudentId { get; set; }
        public string? RateSlug { get; set; }
        public int PaymentId { get; set; } = 0;
        public double Bakiye { get; set; }
        public int RateStudentId { get; set; }
        public bool State { get; set; }
    }
}
