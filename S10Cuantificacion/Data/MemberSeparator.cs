using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
    public class MemberSeparator : Data
    {
        protected string name;

        public MemberSeparator(string val)
        : base("------- CLASS -------")
        {
            name = val;
        }

        override public string
        StrValue()
        {
            return string.Format("--- {0} ---", name);
        }

        public override bool
        IsSeparator
        {
            get { return true; }
        }

        public override bool
        HasDrillDown
        {
            get { return false; }
        }

        public override void
        DrillDown()
        {

        }
    }

    public class MemberSeparatorWithOffset : MemberSeparator
    {
        public MemberSeparatorWithOffset(string val)
            : base(val)
        {
        }

        override public string StrValue()
        {
            return string.Format("  --- {0} ---", name);
        }
    }
}
