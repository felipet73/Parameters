using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
    public class ClassSeparator : Data
    {
        protected System.Type m_val;

        public ClassSeparator(System.Type val)
        : base("------- CLASS -------")
        {
            m_val = val;
        }

        override public string
        StrValue()
        {
            return string.Format("--- {0} ---", m_val.Name);
        }

        public override bool
        IsSeparator
        {
            get { return true; }
        }

        public override bool
        HasDrillDown
        {
            get { return true; }
        }

        public override void
        DrillDown()
        {
            // DrillDown on a ClassType will just browse it using Reflection
            //Snoop.Forms.GenericPropGrid pgForm = new Snoop.Forms.GenericPropGrid(m_val);
            //pgForm.Text = string.Format("System.Type = {0}", m_val.FullName);
           // pgForm.ShowDialog();
        }
    }
}
