using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;

namespace S10Cuantificacion.CollectorExts
{
    public abstract class CollectorExt
    {
        // TBD: For the Snoop.Data.ElementId object I need access to the current
        // document so I can retrieve the Element.  However, from the context of something
        // like a Parameter.AsElementId(), the Document is nowhere to be found.  So, hack
        // around it for now by letting original TestCmd set this value.  Its not local-enough
        // when browsing though, so it could be wrong if browsing doesn't stay within the 
        // original document! (jma - 05/03/05)
        static public Autodesk.Revit.UI.UIApplication m_app = null;
        static public Autodesk.Revit.DB.Document m_activeDoc = null;

        public
        CollectorExt()
        {
            // add ourselves to the event list of all SnoopCollectors
            Collectors.Collector.OnCollectorExt += new Collectors.Collector.CollectorExt(CollectEvent);
            if (m_app != null && m_app.ActiveUIDocument != null && m_app.ActiveUIDocument.Document != null)
            {
                m_activeDoc = m_app.ActiveUIDocument.Document;
            }
        }

        protected abstract void
        CollectEvent(object sender, Collectors.CollectorEventArgs e);

        public Element GetElementById(ElementId id)
        {
            if (m_activeDoc != null)
                return m_activeDoc.GetElement(id);
            return null;
        }
    }
}
