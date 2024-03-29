﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
namespace S10Cuantificacion.Data
{
    public class ViewFiltersVisibilitySettings : Data
    {
        private readonly View view;

        public ViewFiltersVisibilitySettings(string label, View view) : base(label)
        {
            this.view = view;
        }

        public override string StrValue()
        {
            return "< view filters visibility >";
        }

        public override bool HasDrillDown => !view.Document.IsFamilyDocument && view.AreGraphicsOverridesAllowed() && view.GetFilters().Any();

        public override void DrillDown()
        {
            if (!HasDrillDown)
                return;

            var filtersVisibility = view
                .GetFilters()
                .Select(x => new SnoopableObjectWrapper(view.Document.GetElement(x).Name, view.GetFilterVisibility(x)))
                .ToList();

            if (filtersVisibility.Any())
            {
                //var form = new Forms.Objects(filtersVisibility);

                //form.ShowDialog();
            }
        }
    }
}
