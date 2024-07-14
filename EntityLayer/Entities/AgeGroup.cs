using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class AgeGroup
	{
        public int AgeGroupId { get; set; }
        public string Name { get; set; }
        public int StartAge { get; set; }
        public int EndAge { get; set; }
        public List<Rate> Rates { get; set; } = new();
    }
}
