﻿using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract.EFCore
{
	public interface IEFCoreBankService : IEFCoreGenericService<Bank>
	{
		Task<Bank> FindBySlugAsync(string slug);
	}
}
