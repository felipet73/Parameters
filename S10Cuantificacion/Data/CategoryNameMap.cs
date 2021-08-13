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
    public class CategoryNameMap : Data
    {
        protected Autodesk.Revit.DB.CategoryNameMap m_val;

        public
        CategoryNameMap(string label, Autodesk.Revit.DB.CategoryNameMap val)
        : base(label)
        {
            m_val = val;
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
                //Snoop.Forms.Categories form = new Snoop.Forms.Categories(m_val);
                //form.ShowDialog();
            }
        }
    }
}
