using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace S10Cuantificacion.Data
{
    public class ViewFiltersOverrideGraphicSettings : Data
    {
        private readonly View view;

        public ViewFiltersOverrideGraphicSettings(string label, View view) : base(label)
        {
            this.view = view;
        }

        public override string StrValue()
        {
            return "< view filters ovverride graphic settings >";
        }

        public override bool HasDrillDown => !view.Document.IsFamilyDocument && view.AreGraphicsOverridesAllowed() && view.GetFilters().Any();

        public override void DrillDown()
        {
            if (!HasDrillDown)
                return;

            var filterOverrides = view
                .GetFilters()
                .Select(x => new SnoopableObjectWrapper(view.Document.GetElement(x).Name, view.GetFilterOverrides(x)))
                .ToList();

            if (filterOverrides.Any())
            {
                //var form = new Forms.Objects(filterOverrides);

               // form.ShowDialog();
            }
        }
    }
}
