using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities
{
	public class Parent
	{
		public int ParentId
		{
			get; set;
		}
		public string TC
		{
			get; set;
		}
		public string Name
		{
			get; set;
		}
		public string Surname
		{
			get; set;
		}
		public ParentTypes ParentType
		{
			get; set;
		}
		public string Job
		{
			get; set;
		}
		public string PhoneNumber
		{
			get; set;
		}
		public string Mail
		{
			get; set;
		}
		public int StudentId
		{
			get; set;
		}

		public Student Student
		{
			get; set;
		}
        public string Slug { get; set; }
    }
}
