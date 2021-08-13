#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

#endregion

namespace S10Cuantificacion
{
    class App : IExternalApplication
    {
        
        public Result OnStartup(UIControlledApplication a)
        {
            /*string ChecklistsNumber = “myRibbonButton”; string path = Assembly.GetExecutingAssembly().Location;

            String exeConfigPath = Path.GetDirectoryName(path) + "\\myRibbon.dll";
            a.CreateRibbonTab(ChecklistsNumber);
            RibbonPanel PRLChecklistsPanel = a.CreateRibbonPanel(ChecklistsNumber, ChecklistsNumber);
            PushButtonData myPushButtonData = new PushButtonData(ChecklistsNumber, ChecklistsNumber, exeConfigPath, "myRibbon.Invoke");

            myPushButtonData.LargeImage = new BitmapImage(new Uri(Path.Combine(Path.GetDirectoryName(path) + "\\011 myButtonImage paste into Addin.png"), UriKind.Absolute));

            RibbonItem myRibbonItem = PRLChecklistsPanel.AddItem(myPushButtonData);

            return Result.Succeeded;*/
            //MessageBox.Show("Ingresando desde C#", "Hola Mundo");

            //FrmPresupuestos FORM = new FrmPresupuestos();
            //FORM.ShowDialog();



            try
            {

                a.CreateRibbonTab("Herramientas S10");



                /*RibbonPanel rp;
                PushButton push_button;
                PushButton push_button1;
                PushButton push_button2;
                PushButton push_button3;
                rp = a.CreateRibbonPanel("S10 Cuantificaciones", "Presupuestos y Programación");
                
                string button_name;
                string button_text;
                string button_name1;
                string button_text1;
                string button_name2;
                string button_text2;
                string assembly_path;
                string button_name3;
                string button_text3;
                string class_name;
                string class_name1;
                string class_name2;
                string class_name3;
                assembly_path = GetType().Assembly.Location;
                button_name = "Pres1";
                button_text = "Abrir Presupuesto";
                class_name = "revitapiHW.cmd_hello";
                button_name3 = "Filtros1";
                button_text3 = "Ver elementos vinculados ";
                class_name3 = "revitapiHW.CmdFiltros";
                button_name1 = "Cron1";
                button_text1 = "Abrir Cronograma";
                class_name1 = "revitapiHW.CmdCrono";
                button_name2 = "LineaT";
                button_text2 = "Linea de Tiempo";
                class_name2 = "revitapiHW.CmdLinea";*/

                string AssemblyName = System.Reflection.Assembly.GetExecutingAssembly().Location;
                RibbonPanel panel = a.CreateRibbonPanel("Herramientas S10", "Cuantificador");

                PushButtonData BotonMetrado = new PushButtonData("Botón1", "Metrar Presupuestos", AssemblyName, "S10Cuantificacion.Command");
                PushButtonData BotonGestionar = new PushButtonData("Botón2", "Gestionar Modelos", AssemblyName, "S10Cuantificacion.Gestionar");
                PushButton boton1 = panel.AddItem(BotonMetrado) as PushButton;
                PushButton boton2 = panel.AddItem(BotonGestionar) as PushButton;

                boton1.ToolTip = "Generar metrado de partidas";
                boton1.LongDescription = "Aplicación para Generar metrado en partidas";
                boton1.LargeImage = new BitmapImage(new Uri(@"C:\MrdtC\img\rvt\2.ico"));

                boton2.ToolTip = "Gestionar los modelos BIM";
                boton2.LongDescription = "Aplicación para Gestión de modelos Bim";
                boton2.LargeImage = new BitmapImage(new Uri(@"C:\MrdtC\img\rvt\2.ico"));


                /*push_button = rp.AddItem(new PushButtonData(button_name, button_text, assembly_path, class_name));
                push_button.LargeImage = new BitmapImage(new Uri(@"C:\MrdtC\img\rvt\1.ico", UriKind.Absolute));
                push_button3 = rp.AddItem(new PushButtonData(button_name3, button_text3, assembly_path, class_name3));
                push_button3.LargeImage = new BitmapImage(new Uri(@"C:\MrdtC\img\rvt\1.ico", UriKind.Absolute));
                push_button1 = rp.AddItem(new PushButtonData(button_name1, button_text1, assembly_path, class_name1));
                push_button1.LargeImage = new BitmapImage(new Uri(@"C:\MrdtC\img\rvt\3.ico", UriKind.Absolute));
                push_button2 = rp.AddItem(new PushButtonData(button_name2, button_text2, assembly_path, class_name2));
                push_button2.LargeImage = new BitmapImage(new Uri(@"C:\MrdtC\img\rvt\2.ico", UriKind.Absolute));*/



                //push_button.ToolTip = "Este boton permite ver el presupuesto";
                //push_button.LongDescription = "Accede al presupuesto del proyecto";
                //rp.Visible = true;
                return Result.Succeeded;
            }
            catch (Exception)
            {
                //MsgBox(ex.Message + "  " + ex.StackTrace);
                return Result.Failed;
            }


            //return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
    }
}
