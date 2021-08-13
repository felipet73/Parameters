using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S10Cuantificacion.Data
{
    public class ElementSet : Data
    {
        protected Autodesk.Revit.DB.ElementSet m_val;

        public
        ElementSet(string label, Autodesk.Revit.DB.ElementSet val)
        : base(label)
        {
            m_val = val;
        }

        public
        ElementSet(string label, ICollection<Autodesk.Revit.DB.ElementId> val, Autodesk.Revit.DB.Document doc)
        : base(label)
        {
            m_val = new Autodesk.Revit.DB.ElementSet();
            foreach (Autodesk.Revit.DB.ElementId elemId in val)
            {
                if (Autodesk.Revit.DB.ElementId.InvalidElementId == elemId)
                    continue;
                Autodesk.Revit.DB.Element elem = doc.GetElement(elemId);
                if (null != elem)
                    m_val.Insert(elem);
            }
        }

        public override string
        StrValue()
        {
            return Utils.ObjToLabelStr(m_val);
        }

        public override bool
        HasDrillDown
        {
            get
            {
                if ((m_val == null) || (m_val.IsEmpty))
                    return false;
                else
                    return true;
            }
        }

        public override void
        DrillDown()
        {
            if ((m_val != null) && (m_val.IsEmpty == false))
            {
               //Snoop.Forms.Objects form = new Snoop.Forms.Objects(m_val);
                //form.ShowDialog();
            }
        }
    }
}
