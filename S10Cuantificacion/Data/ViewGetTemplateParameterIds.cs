﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace S10Cuantificacion.Data
{
    public class ViewGetTemplateParameterIds : Data
    {
        private readonly View _view;

        public ViewGetTemplateParameterIds(string label, View view) : base(label)
        {
            _view = view;
        }

        public override string StrValue()
        {
            return "< view template parameter ids >";
        }

        public override bool HasDrillDown => !_view.Document.IsFamilyDocument && _view.IsTemplate && _view.GetTemplateParameterIds().Any();

        public override void DrillDown()
        {
            if (!HasDrillDown) return;

            var viewParams = _view.Parameters.Cast<Parameter>().ToList();

            var templateParameterIds =
                (from id in _view.GetTemplateParameterIds()
                 select viewParams.Find(q => q.Id.IntegerValue == id.IntegerValue)
                into p
                 where p != null
                 select new SnoopableObjectWrapper(p.Definition.Name, p)).ToList();

            if (!templateParameterIds.Any()) return;

            //var form = new Forms.Objects(templateParameterIds);
            //form.ShowDialog();
        }
    }
}
