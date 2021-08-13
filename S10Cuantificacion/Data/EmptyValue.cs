using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
    public class EmptyValue : Data
    {
        public EmptyValue(string label) : base(label)
        {
        }

        public override string StrValue()
        {
            return string.Empty;
        }
    }
}
