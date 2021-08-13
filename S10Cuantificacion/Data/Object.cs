using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows.Forms;

namespace S10Cuantificacion.Data
{
    public class Object : Data
    {
        protected System.Object m_val;

        public
        Object(string label, System.Object val)
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
                if (m_val == null)
                    return false;
                else
                    return true;
            }
        }

        public override void
        DrillDown()
        {
            if (m_val != null)
            {
                ArrayList objs = new ArrayList();
                objs.Add(m_val);

                //Snoop.Forms.Objects form = new Snoop.Forms.Objects(objs);
                //form.ShowDialog();
            }
        }
    }
}
