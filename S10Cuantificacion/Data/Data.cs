using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
	public abstract class Data
	{
		private string m_label;

		public
		Data(string label)
		{
			m_label = label;
		}

		/// <summary>
		/// The Label value for the property (e.g. "Radius" for a Circle
		/// </summary>

		public virtual string
		Label
		{
			get { return m_label; }
			set { m_label = value; }
		}

		/// <summary>
		/// The value for the Property, expressed as a string
		/// </summary>
		/// <returns>The value formatted as a string</returns>

		public abstract string StrValue();

		/// <summary>
		/// Format the Label and Value as a single string.  The Snoop Forms will
		///	handle the Label/Value pair individually, but in other contexts, this
		///	could be used to make a flat list of Label/Value pairs.
		/// </summary>
		/// <returns>Label/Value pair as a string</returns>

		public override string ToString()
		{
			return string.Format("{0}: {1}", m_label, StrValue());
		}

		/// <summary>
		/// Is there more information available about this property.  For instance,
		/// a type double would not have anything further to show.  But, a Collection
		/// can bring up a nested dialog showing all those objects.
		/// </summary>

		public virtual bool
		HasDrillDown
		{
			get { return false; }
		}

		/// <summary>
		/// Do the act of drilling down on the data
		/// </summary>

		public virtual void
		DrillDown()
		{
			;   // do nothing by default
		}

		/// <summary>
		/// Is this real data, or just a logical category separator?
		/// </summary>

		public virtual bool
		IsSeparator
		{
			get { return false; }
		}

		/// <summary>
		/// Is this an error condition
		/// </summary>

		public virtual bool
		IsError
		{
			get { return false; }
		}
	}
}
