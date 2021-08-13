using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
    public class Exception : Data
    {
        protected System.Exception m_val;

        public Exception(string label, System.Exception val)
           : base(label)
        {
            m_val = val;
        }

        public override string StrValue()
        {
            return m_val.Message;
        }

        public override bool IsError
        {
            get { return true; }
        }
    }
}
