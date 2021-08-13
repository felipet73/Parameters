using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using Autodesk.Revit.DB;

namespace S10Cuantificacion.Data
{
    public class Enumerable : Data
    {
        protected IEnumerable m_val;
        protected ArrayList m_objs = new ArrayList();

        public
        Enumerable(string label, IEnumerable val)
        : base(label)
        {
            m_val = val;

            // iterate over the collection and put them in an ArrayList so we can pass on
            // to our Form
            if (m_val != null)
            {
                IEnumerator iter = m_val.GetEnumerator();
                while (iter.MoveNext())
                    m_objs.Add(iter.Current);
            }
        }

        public
        Enumerable(string label, IEnumerable val, Document doc)
            : base(label)
        {
            m_val = val;

            // iterate over the collection and put them in an ArrayList so we can pass on
            // to our Form
            if (m_val != null)
            {
                IEnumerator iter = m_val.GetEnumerator();
                while (iter.MoveNext())
                {
                    var elementId = iter.Current as Autodesk.Revit.DB.ElementId;

                    if (elementId != null && doc != null)
                    {
                        var elem = doc.GetElement(elementId);
                        if (elem == null) // Likely a category
                            m_objs.Add(Category.GetCategory(doc, elementId));
                        else
                            m_objs.Add(elem); // it's more useful for user to view element rather than element id.
                    }
                    else
                        m_objs.Add(iter.Current);
                }
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
                if ((m_val == null) || (m_objs.Count == 0))
                    return false;
                else
                    return true;
            }
        }

        public override void
        DrillDown()
        {
            if ((m_val != null) && (m_objs.Count != 0))
            {
                //Snoop.Forms.Objects form = new Snoop.Forms.Objects(m_objs);
                //form.ShowDialog();
            }
        }
    }
}
