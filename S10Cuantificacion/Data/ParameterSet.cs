using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;

using Autodesk.Revit.DB;
namespace S10Cuantificacion.Data
{
    public class ParameterSet : Data
    {
        protected Autodesk.Revit.DB.ParameterSet m_val;
        protected Element m_elem;

        public
        ParameterSet(string label, Element elem, Autodesk.Revit.DB.ParameterSet val)
        : base(label)
        {
            m_val = val;
            m_elem = elem;
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
                //Snoop.Forms.Parameters form = new Snoop.Forms.Parameters(m_elem, m_val);
                //form.ShowDialog();
            }
        }
    }
}
