using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
	public class Xyz : Data
	{
		protected Autodesk.Revit.DB.XYZ m_val;

		public
		Xyz(string label, Autodesk.Revit.DB.XYZ val)
		: base(label)
		{
			m_val = val;
		}

		public override string
		StrValue()
		{
			if (m_val != null)
				return string.Format("({0}, {1}, {2})", m_val.X, m_val.Y, m_val.Z);
			else
				return "< null >";
		}
	}
}
