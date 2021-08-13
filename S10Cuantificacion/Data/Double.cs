using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
	public class Double : Data
	{
		protected double m_val;

		public
		Double(string label, double val)
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
