using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Bank
	{
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string Slug { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
