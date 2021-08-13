using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
    public class ExtensibleStorageSeparator : Data
    {
        public ExtensibleStorageSeparator() : base(string.Empty)
        {
        }

        public override string StrValue()
        {
            return "--- Extensible storages ---";
        }

        public override bool IsSeparator
        {
            get { return true; }
        }

        public override bool HasDrillDown
        {
            get { return false; }
        }
    }
}
