﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.IO;
using S10Cuantificacion.Collectors;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using S10Cuantificacion.Data;

namespace S10Cuantificacion.CollectorExts
{
    public class CollectorExtElement : CollectorExt
    {
        readonly Type[] types;

        public CollectorExtElement()
        {
            var baseDirectory = Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory);

            types = AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
                .Where(x => Path.GetDirectoryName(x.Location) == baseDirectory)
                .Where(x => x.GetName().Name.ToLower().Contains("revit"))
                .SelectMany(x => x.GetTypes())
                .Union(new[] { typeof(KeyValuePair<,>) })
                .ToArray();
        }

        protected override void CollectEvent(object sender, CollectorEventArgs e)
        {
            Collector snoopCollector = sender as Collector;
            if (snoopCollector == null)
            {
                Debug.Assert(false); // why did someone else send us the message?
                return;
            }

            if (e.ObjToSnoop is IEnumerable)
                snoopCollector.Data().Add(new Data.Enumerable(e.ObjToSnoop.GetType().Name, e.ObjToSnoop as IEnumerable));
            else
                Stream(snoopCollector.Data(), e.ObjToSnoop);
        }

        private void Stream(ArrayList data, object elem)
        {
            var thisElementTypes = types.Where(x => IsSnoopableType(x, elem)).ToList();

            var streams = new IElementStream[]
                {
                    new ElementPropertiesStream(m_app, data, elem),
                    new ElementMethodsStream(m_app, data, elem),
                    new SpatialElementStream(data, elem),
                    new FamilyTypeParameterValuesStream(data, elem),
                    new ExtensibleStorageEntityContentStream(m_app.ActiveUIDocument.Document, data, elem)
                };

            foreach (var type in thisElementTypes)
            {
                data.Add(new ClassSeparator(type));

                foreach (var elementStream in streams)
                    elementStream.Stream(type);
            }

            StreamElementExtensibleStorages(data, elem as Element);

            StreamSimpleType(data, elem);
        }

        private static bool IsSnoopableType(Type type, object element)
        {
            var elementType = element.GetType();

            if (type == elementType || elementType.IsSubclassOf(type) || type.IsAssignableFrom(elementType))
                return true;

            return type.IsGenericType && elementType.IsGenericType && IsSubclassOfRawGeneric(type, elementType);
        }

        private static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        private static void StreamElementExtensibleStorages(ArrayList data, Element elem)
        {
            var schemas = Schema.ListSchemas();

            if (elem == null || !schemas.Any())
                return;

            data.Add(new ExtensibleStorageSeparator());

            foreach (var schema in schemas)
            {
                var objectName = "Entity with Schema [" + schema.SchemaName + "]";
                try
                {
                    var entity = elem.GetEntity(schema);

                    if (!entity.IsValid())
                        continue;

                    data.Add(new Data.Object(objectName, entity));
                }
                catch (System.Exception ex)
                {
                    data.Add(new Data.Exception(objectName, ex));
                }
            }
        }

        private void StreamSimpleType(ArrayList data, object elem)
        {
            var elemType = elem.GetType();

            if (elemType.IsEnum || elemType.IsPrimitive || elemType.IsValueType)
                data.Add(new Data.String($"{elemType.Name} value", elem.ToString()));
        }
    }
}
