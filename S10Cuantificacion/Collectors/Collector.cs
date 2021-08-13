using System;
using System.Collections;
using System.Windows.Forms;

namespace S10Cuantificacion.Collectors
{
	public class Collector
	{
		public delegate void CollectorExt(object sender, CollectorEventArgs e);
		public static event CollectorExt OnCollectorExt;

		protected ArrayList m_dataObjs = new ArrayList();

		public Collector()
		{

		}

		public ArrayList Data()
		{
			return m_dataObjs;
		}

		// Apparently, you can't call the Event from outside the actual class that defines it.
		// So, we'll simply wrap it.  Now all derived classes can broadcast the event.
		protected void FireEvent_CollectExt(object objToSnoop)
		{
			//MessageBox.Show(objToSnoop.ToString());
			if (OnCollectorExt != null)
				OnCollectorExt(this, new CollectorEventArgs(objToSnoop));
		}
	}
}
