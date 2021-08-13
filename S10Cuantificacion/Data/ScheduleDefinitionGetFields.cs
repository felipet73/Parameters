using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace S10Cuantificacion.Data
{
    public class ScheduleDefinitionGetFields : Data
    {
        private readonly ScheduleDefinition _scheduleDefinition;

        public ScheduleDefinitionGetFields(string label, ScheduleDefinition scheduleDefinition) : base(label)
        {
            _scheduleDefinition = scheduleDefinition;
        }

        public override string StrValue()
        {
            return "< Get Fields >";
        }

        public override bool HasDrillDown => _scheduleDefinition != null && _scheduleDefinition.GetFieldCount() > 0;

        public override void DrillDown()
        {
            if (!HasDrillDown) return;

            List<SnoopableObjectWrapper> scheduleFieldObjects = new List<SnoopableObjectWrapper>();

            for (int i = 0; i < _scheduleDefinition.GetFieldCount(); i++)
            {
                ScheduleField field = _scheduleDefinition.GetField(i);
                scheduleFieldObjects.Add(new SnoopableObjectWrapper("[" + i + "] " + field.GetName(), field));
            }

            if (!scheduleFieldObjects.Any()) return;

            //var form = new Forms.Objects(scheduleFieldObjects);
            //form.ShowDialog();
        }
    }
}
