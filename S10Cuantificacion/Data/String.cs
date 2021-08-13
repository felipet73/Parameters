using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
	public class String : Data
	{
		protected string m_val;

		public
		String(string label, string val)
		: base(label)
		{
			m_val = val;
		}

		public override string
		StrValue()
		{
			return m_val;
		}
	}
}
