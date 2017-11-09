using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sundays
{
	public interface ISundayProvider
	{
		Task<IEnumerable<Sunday>> Get(DateTime from, DateTime to);
	}
}
