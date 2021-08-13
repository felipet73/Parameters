using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10Cuantificacion.Data
{
    class SnoopableObjectWrapper
    {
        public SnoopableObjectWrapper(string title, object obj)
        {
            Title = title;

            Object = obj;
        }

        public string Title { get; }

        public object Object { get; }

        public static SnoopableObjectWrapper Create(object obj) => new SnoopableObjectWrapper(Utils.ObjToLabelStr(obj), obj);

        public Type GetUnderlyingType() => Object.GetType();
    }
}
