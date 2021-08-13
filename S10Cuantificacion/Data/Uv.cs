using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
	public class Uv : Data
	{
		protected Autodesk.Revit.DB.UV m_val;

		public
		Uv(string label, Autodesk.Revit.DB.UV val)
		: base(label)
		{
			m_val = val;
		}

		public override string
		StrValue()
		{
			if (m_val != null)
				return string.Format("({0}, {1})", m_val.U, m_val.V);
			else
				return "< null >";
		}
	}
}
