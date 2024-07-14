using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class DiscountRate
	{
        public int DiscountRateId { get; set; }
        public string DiscontRateName { get; set; }
        public int DiscountRates { get; set; }
        public List<PaymentType> PaymentTypes { get; set; }
    }
}
