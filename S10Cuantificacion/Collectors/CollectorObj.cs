using System;
using Autodesk.Revit.DB;

namespace S10Cuantificacion.Collectors
{
    public class CollectorObj : Collector
    {
        // TBD: this isn't the way I wanted to do this because it isn't very extensible (as in MgdDbg),
        // but there is no static global initialization for the whole module.  So, I'm faking it here
        // by having a static on this class. (jma - 04/14/05)
        public static CollectorExts.CollectorExtElement m_colExtElement;

        public static bool IsInitialized = false;

        public
        CollectorObj()
        {
        }

        /// <summary>
        /// This method is used to initialized static variables in this class,
        /// in .Net 4, the static variables will not be initialized until use them,
        /// so we need to call this method explicitly in App.cs when Revit Starts up
        /// </summary>
        public static void InitializeCollectors()
        {
            if (!IsInitialized)
            {
                m_colExtElement = new CollectorExts.CollectorExtElement();

                IsInitialized = true;
                System.Diagnostics.Trace.WriteLine("Initialized");
            }
        }

        /// <summary>
        /// This is the point where the ball starts rolling.  We'll walk down the object's class hierarchy,
        /// continually trying to cast it to objects we know about.  NOTE: this is intentionally not Reflection.
        /// We can do that elsewhere, but here we want to explictly control how data is formatted and navigated,
        /// so we will manually walk the entire hierarchy.
        /// </summary>
        /// <param name="obj">Object to collect data for</param>

        public void
        Collect(System.Object obj)
        {
            m_dataObjs.Clear();

            if (obj == null)
                return;

            FireEvent_CollectExt(obj);
        }

    }
}
