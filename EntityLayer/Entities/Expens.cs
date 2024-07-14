using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Expens
	{
        public int ExpensId { get; set; }
        public double  ExpensPrice { get; set; }
        public string Description { get; set; }
        public DateTime ExpensDate { get; set; }
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
    }
}
