using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract.EFCore
{
	public interface IEFCoreParentRepository : IEFCoreGenericRepository<Parent>
	{
		Task<Parent> FindBySlugAsync(string slug);
		Task<Parent> FindByIdWithStudentAsync(int id);
	}
}
