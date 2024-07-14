using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class PaymentType
	{
        public int PaymentTypeId { get; set; }
        public string PaymentTypeName { get; set; }
        public string Slug { get; set; }
        public int? DiscountRateId { get; set; }
        public List<Payment> Payments { get; set; } = new();
        public DiscountRate DiscountRate { get; set; }
    }
}
