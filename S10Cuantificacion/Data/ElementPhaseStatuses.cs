﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace S10Cuantificacion.Data
{
    public class ElementPhaseStatuses : Data
    {
        private readonly Element element;

        public ElementPhaseStatuses(string label, Element element) : base(label)
        {
            this.element = element;
        }

        public override string StrValue() => "< phases statuses >";

        public override bool HasDrillDown => !element.Document.Phases.IsEmpty;

        public override void DrillDown()
        {
            if (!HasDrillDown)
                return;

            var elementOnPhaseStatuses = element
                .Document
                .Phases
                .Cast<Phase>()
                .Select(x => new SnoopableObjectWrapper(x.Name, element.GetPhaseStatus(x.Id)))
                .ToList();

            if (elementOnPhaseStatuses.Any())
            {
                //var form = new Forms.Objects(elementOnPhaseStatuses);
                //form.ShowDialog();
            }
        }
    }
}
