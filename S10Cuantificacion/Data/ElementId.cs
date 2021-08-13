using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace S10Cuantificacion.Data
{
    public class ElementId : Data
    {
        protected Autodesk.Revit.DB.ElementId m_val;
        protected Element m_elem;

        public ElementId(string label, Autodesk.Revit.DB.ElementId val, Document doc) : base(label)
        {
            m_val = val;

            m_elem = doc.GetElement(val);
        }

        public override string StrValue()
        {
            if (m_elem != null)
                return Utils.ObjToLabelStr(m_elem);

            return m_val != Autodesk.Revit.DB.ElementId.InvalidElementId ? m_val.ToString() : Utils.ObjToLabelStr(null);
        }

        public override bool HasDrillDown => m_elem != null;

        public override void DrillDown()
        {
            if (m_elem == null)
                return;

            //var form = new Forms.Objects(m_elem);

            //form.ShowDialog();
        }
    }
}
