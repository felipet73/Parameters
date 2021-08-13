using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
	public class Bool : Data
	{
		protected bool m_val;

		public
		Bool(string label, bool val)
		: base(label)
		{
			m_val = val;
		}

		public override string
		StrValue()
		{
			return m_val.ToString();
		}
	}
}
