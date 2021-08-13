using System;
using System.Collections.Generic;


namespace S10Cuantificacion.Collectors
{
	public class CollectorEventArgs : System.EventArgs
	{
		private object m_objToSnoop;
		private List<Type> seenTypes;

		public CollectorEventArgs(object objToSnoop)
		{
			m_objToSnoop = objToSnoop;
			seenTypes = new List<Type>();
		}

		public object ObjToSnoop
		{
			get { return m_objToSnoop; }
		}
	}
}
