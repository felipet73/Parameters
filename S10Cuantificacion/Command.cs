#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;


#endregion

namespace S10Cuantificacion
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {

        //FrmBim360 fr1 = new FrmBim360();
        public Result Execute(
          ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            Result result;

            using (var transaction = new Transaction(cmdData.Application.ActiveUIDocument.Document, this.GetType().Name))
            {
                transaction.Start(); // necessary to snoop Document.PlanTopologies
                try
                {
                    //Snoop.CollectorExts.CollectorExt.m_app = cmdData.Application;   // TBD: see note in CollectorExt.cs
                    //Snoop.CollectorExts.CollectorExt.m_activeDoc = cmdData.Application.ActiveUIDocument.Document;

                    // iterate over the collection and put them in an ArrayList so we can pass on
                    // to our Form
                    Autodesk.Revit.DB.Document doc = cmdData.Application.ActiveUIDocument.Document;
                    FilteredElementCollector elemTypeCtor = (new FilteredElementCollector(doc)).WhereElementIsElementType();
                    FilteredElementCollector notElemTypeCtor = (new FilteredElementCollector(doc)).WhereElementIsNotElementType();
                    FilteredElementCollector allElementCtor = elemTypeCtor.UnionWith(notElemTypeCtor);
                    ICollection<Element> founds = allElementCtor.ToElements();

                    //ArrayList objs = new ArrayList();
                    Datos.objs = new ArrayList();
                    foreach (Element element in founds)
                    {
                        Datos.objs.Add(element);
                    }

                    System.Diagnostics.Trace.WriteLine(founds.Count.ToString());
                    //Snoop.Forms.Objects form = new Snoop.Forms.Objects(objs);
                    ActiveDoc.UIApp = cmdData.Application;
                    //form.ShowDialog();

                    result = Result.Succeeded;
                }
                catch (System.Exception e)
                {
                    msg = e.Message;
                    result = Result.Failed;
                }
                transaction.RollBack();
            }



            Datos.cmdData1 = cmdData;

            FrmLogin FORMlog = new FrmLogin();
            FORMlog.ShowDialog();

            //MessageBox.Show("dato", FORMlog.Empresa);

            if (FORMlog.Empresa != "") {
                FrmPresupuestos FORM = new FrmPresupuestos();
                FORM.Token = FORMlog.Token;
                FORM.EmailUsuario = FORMlog.Email;
                FORM.Show();

                /*App.frPrin.Token = FORMlog.Token;
                App.frPrin.EmailUsuario = FORMlog.Email;
                App.frPrin.Show();*/
            }

            return result;
            
        }
    }



    [Transaction(TransactionMode.Manual)]
    public class Gestionar : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData cmdData, ref string msg, ElementSet elems)
        {
            Result result;

            using (var transaction = new Transaction(cmdData.Application.ActiveUIDocument.Document, this.GetType().Name))
            {
                transaction.Start(); // necessary to snoop Document.PlanTopologies
                try
                {
                    //Snoop.CollectorExts.CollectorExt.m_app = cmdData.Application;   // TBD: see note in CollectorExt.cs
                    //Snoop.CollectorExts.CollectorExt.m_activeDoc = cmdData.Application.ActiveUIDocument.Document;

                    // iterate over the collection and put them in an ArrayList so we can pass on
                    // to our Form
                    Autodesk.Revit.DB.Document doc = cmdData.Application.ActiveUIDocument.Document;
                    FilteredElementCollector elemTypeCtor = (new FilteredElementCollector(doc)).WhereElementIsElementType();
                    FilteredElementCollector notElemTypeCtor = (new FilteredElementCollector(doc)).WhereElementIsNotElementType();
                    FilteredElementCollector allElementCtor = elemTypeCtor.UnionWith(notElemTypeCtor);
                    ICollection<Element> founds = allElementCtor.ToElements();

                    //ArrayList objs = new ArrayList();
                    Datos.objs = new ArrayList();
                    foreach (Element element in founds)
                    {
                        Datos.objs.Add(element);
                    }

                    System.Diagnostics.Trace.WriteLine(founds.Count.ToString());
                    //Snoop.Forms.Objects form = new Snoop.Forms.Objects(objs);
                    ActiveDoc.UIApp = cmdData.Application;
                    //form.ShowDialog();

                    result = Result.Succeeded;
                }
                catch (System.Exception e)
                {
                    msg = e.Message;
                    result = Result.Failed;
                }
                transaction.RollBack();
            }



            Datos.cmdData1 = cmdData;

            FrmLogin FORMlog = new FrmLogin();
            FORMlog.ShowDialog();

            //MessageBox.Show("dato", FORMlog.Empresa);

            if (FORMlog.Empresa != "")
            {
                //Datos obj = new Datos();
                FrmBim360 fr1 = new FrmBim360();
                fr1.TokenAct = "";
                fr1.labelItem2.Text = FORMlog.Email;
                fr1.labelItem1.Text = Datos.cmdData1.Application.ActiveUIDocument.Document.Title;
                fr1.TxtModelo.Text = Datos.cmdData1.Application.ActiveUIDocument.Document.PathName;

                fr1.Token = FORMlog.Token;

                fr1.Show();

            }

            return result;

        }
    }



}
