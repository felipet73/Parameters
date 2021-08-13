using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Forge.Client;
using Autodesk.Forge;
using Microsoft.Office.Interop.Word;
using DevComponents.DotNetBar.SuperGrid;
using DevComponents.DotNetBar.SuperGrid.Style;
using DevComponents.AdvTree;
//using System.Data.OleDb;
using System.IO;
using Newtonsoft.Json.Linq;
using RestSharp;
using Microsoft.CSharp;
//using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json;
using System.Json;
using Microsoft.AspNet.SignalR.Client;
//using S10Cuantificacion.Cantidades;
using DevComponents.DotNetBar.Charts;
using DevComponents.DotNetBar.Charts.Style;
//using S10Cuantificacion.Data;
using System.Collections;
using S10Cuantificacion.Data;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Microsoft.VisualBasic;
using System.Windows;
using System.Threading;
using System.Data.SqlClient;
using System.Text.Json;

namespace S10Cuantificacion
{
    public partial class FrmPresupuestos : System.Windows.Forms.Form
    {
        HubConnection connection = null;
        IHubProxy _s10ERPHubProxy = null;
        public string Token;
        public string EmailUsuario;

        public static string Presupuesto_actual = "";
        public static string SubPresupuesto_actual = "";
        public static string Item_actual = "";


        public static string tokenstk = "";
        public static string signalstk = "";

        protected object m_curObj;
        protected ArrayList m_treeTypeNodes = new ArrayList();
        protected ArrayList m_types = new ArrayList();
        protected Collectors.CollectorObj m_snoopCollector = new Collectors.CollectorObj();


        #region Control_Chart
        
        
        private System.Windows.Forms.Timer _Timer;
        private System.Windows.Forms.Timer _Timer1;
        private System.Windows.Forms.Timer _Timer2;

        private NwData[] _NwData = new NwData[] {
            new NwData("Armazón estructural", 0),
            new NwData("Armadura estructural", 0),
            new NwData("Bandejas de cables", 0),
            new NwData("Barandillas",0),
            new NwData("Cimentación estructural", 0),
            new NwData("Conductos", 0),
            new NwData("Conductos flexibles", 0),
            new NwData("Conexiones estructurales", 0),
            new NwData("Cubiertas", 0),
            new NwData("Emplazamiento", 0),
            new NwData("Escaleras", 0),
            new NwData("Modelos genéricos", 0),
            new NwData("Aparatos sanitarios", 0),
            new NwData("Muros", 0),
            new NwData("Pilares estructurales", 0),
            new NwData("Rampas", 0),
            new NwData("Suelos", 0),
            new NwData("Techos", 0),
            new NwData("Tuberías", 0),
            new NwData("Tuberías flexibles", 0),
            new NwData("Tubos", 0),
            new NwData("Topografía", 0),
        };


        private NwData[] _NwData1;
        private NwData[] _NwData2;

        public string ModeloCargado = "";
        public int EnEspera = 0;
        public string ModeloXCargar = "";
        public int EntroPrespuesto = 0;

        public static string NombrePl = "";

        #region cargarCatalogos



        void CargarFamilias() {

            TotalElementos = new List<RevitElementoBase>();

            TotalElementos.AddRange(ArmazonesEstructurales);
            TotalElementos.AddRange(BandejasdeCables);
            TotalElementos.AddRange(Barandillas);
            TotalElementos.AddRange(CimentacionesEstructurales);
            TotalElementos.AddRange(Conductos);
            TotalElementos.AddRange(ConductosFlexibles);
            TotalElementos.AddRange(ConexionesEstructurales);
            TotalElementos.AddRange(Cubiertas);
            TotalElementos.AddRange(Emplazamientos);
            TotalElementos.AddRange(Escaleras);
            TotalElementos.AddRange(ModelosGenericos);
            TotalElementos.AddRange(Muros);
            TotalElementos.AddRange(PilaresEstructurales);
            TotalElementos.AddRange(Rampas);
            TotalElementos.AddRange(Suelos);
            TotalElementos.AddRange(Techos);
            TotalElementos.AddRange(Tuberias);
            TotalElementos.AddRange(TuberiasFlexibles);
            TotalElementos.AddRange(Tubos);
            TotalElementos.AddRange(ArmadurasEstructuales);
            TotalElementos.AddRange(AparatosSanitarios);
            TotalElementos.AddRange(Topografias);


            var ArComp = new string[5];
            var ArCategorias = new string[501];
            var ArFamilias = new string[501];
            var ArTipos = new string[501];
            var ArParametros = new string[501];
            var I = default(int);
            ArComp[0] = "";
            ArComp[1] = "Igual";
            ArComp[2] = "Diferente";

            ArFamilias[0] = "";
            ArCategorias[0] = "";
            ArTipos[0] = "";
            ArParametros[0] = "";


            // AGREGAMOS LOS PARAMETROS COMPARTIDOS
            AdvParametrosCompartidos.ClearAndDisposeAllNodes();
            AdvParametrosCompartidos.BeginUpdate();

            I = 1;
            foreach (var dato in ParametrosCompartidos)
            {
                var node = new DevComponents.AdvTree.Node();
                node.Tag = "";
                node.Text = dato.Nombre;
                node.Image = ImageList1.Images[6];
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                AdvParametrosCompartidos.Nodes.Add(node);
                node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;

                ArParametros[I] = dato.Nombre;
                I++;
            }
            AdvParametrosCompartidos.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;



            AdvCategorias.ClearAndDisposeAllNodes();
            CmbCategoria2.Items.Clear();
            CmbCategorias.Items.Clear();
            AdvCategorias.BeginUpdate();
            CmbCategoria2.Items.Add("");
            CmbCategorias.Items.Add("");




            I = 1;
            foreach (var dato in _NwData) {
               var node = new DevComponents.AdvTree.Node(); 
               node.Tag = "";
               node.Text = dato.Country;
                selecciona_categoria(dato.Country);
                ArCategorias[I] = dato.Country;
                node.Image = ImageList1.Images[0];
                CmbCategoria2.Items.Add(dato.Country);
                CmbCategorias.Items.Add(dato.Country);
                node.Cells.Add(new DevComponents.AdvTree.Cell(CategoriaSeleccionada.Count().ToString()));
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                AdvCategorias.Nodes.Add(node);
               node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                I++;
            }

            AdvCategorias.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;


            FamiliaTotal = new List<RevitFamiliaBase>();

            TipoTotal = new List<RevitTipoBase>();

            TipoTotal.AddRange(TiposArmazonesEstructurales);
            TipoTotal.AddRange(TiposBandejasdeCables);
            TipoTotal.AddRange(TiposBarandillas);
            TipoTotal.AddRange(TiposCimentacionesEstructurales);
            TipoTotal.AddRange(TiposConductos);
            TipoTotal.AddRange(TiposConductosFlexibles);
            TipoTotal.AddRange(TiposConexionesEstructurales);
            TipoTotal.AddRange(TiposCubiertas);
            TipoTotal.AddRange(TiposEmplazamientos);
            TipoTotal.AddRange(TiposEscaleras);
            TipoTotal.AddRange(TiposModelosGenericos);
            TipoTotal.AddRange(TiposMuros);
            TipoTotal.AddRange(TiposPilaresEstructurales);
            TipoTotal.AddRange(TiposRampas);
            TipoTotal.AddRange(TiposSuelos);
            TipoTotal.AddRange(TiposTechos);
            TipoTotal.AddRange(TiposTuberias);
            TipoTotal.AddRange(TiposTuberiasFlexibles);
            TipoTotal.AddRange(TiposTubos);
            TipoTotal.AddRange(TiposArmadurasEstructuales);
            TipoTotal.AddRange(TiposAparatosSanitarios);
            TipoTotal.AddRange(TiposTopografias);


            FamiliaTotal.AddRange(FamiliasArmazonesEstructurales);
            FamiliaTotal.AddRange(FamiliasBandejasdeCables);
            FamiliaTotal.AddRange(FamiliasBarandillas);
            FamiliaTotal.AddRange(FamiliasCimentacionesEstructurales);
            FamiliaTotal.AddRange(FamiliasConductos);
            FamiliaTotal.AddRange(FamiliasConductosFlexibles);
            FamiliaTotal.AddRange(FamiliasCubiertas);
            FamiliaTotal.AddRange(FamiliasEmplazamientos);
            FamiliaTotal.AddRange(FamiliasEscaleras);
            FamiliaTotal.AddRange(FamiliasModelosGenericos);
            FamiliaTotal.AddRange(FamiliasMuros);
            FamiliaTotal.AddRange(FamiliasPilaresEstructurales);
            FamiliaTotal.AddRange(FamiliasRampas);
            FamiliaTotal.AddRange(FamiliasSuelos);
            FamiliaTotal.AddRange(FamiliasTechos);
            FamiliaTotal.AddRange(FamiliasTuberias);
            FamiliaTotal.AddRange(FamiliasTuberiasFlexibles);
            FamiliaTotal.AddRange(FamiliasTubos);
            FamiliaTotal.AddRange(FamiliasArmadurasEstructuales);
            FamiliaTotal.AddRange(FamiliasAparatosSanitarios);
            FamiliaTotal.AddRange(FamiliasTopografias);


            I = 1;
            CmbFamilia.Items.Clear();
            CmbFamilia.Items.Add("");
            AdvFamilias.ClearAndDisposeAllNodes();
            AdvFamilias.BeginUpdate();
            foreach (var dato in FamiliaTotal)
            {
                var node = new DevComponents.AdvTree.Node();
                node.Tag = "";
                node.Text = dato.Familia;
                ArFamilias[I] = dato.Familia;
                node.Image = ImageList1.Images[0];
                CmbFamilia.Items.Add(dato.Familia);
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                AdvFamilias.Nodes.Add(node);
                node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                I++;
            }
            AdvFamilias.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;


            I = 1;
            AdvTreeTipos.ClearAndDisposeAllNodes();
            AdvTreeTipos.BeginUpdate();
            foreach (var dato in TipoTotal)
            {
                var node = new DevComponents.AdvTree.Node();
                node.Tag = "";
                node.Text = dato.Tipo;
                ArTipos[I] = dato.Tipo;
                node.Image = ImageList1.Images[0];
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                AdvTreeTipos.Nodes.Add(node);
                node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                I++;
            }
            AdvTreeTipos.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;

            GridPanel panel1 = SGAsociados.PrimaryGrid;
            GridPanel panel2 = SGestructuraC.PrimaryGrid;
            panel1.Columns[2].EditorType = typeof(FragrantComboBox);
            panel1.Columns[2].EditorParams = new object[] { ArCategorias };
            panel1.Columns[3].EditorType = typeof(FragrantComboBox);
            panel1.Columns[3].EditorParams = new object[] { ArFamilias };
            panel1.Columns[4].EditorType = typeof(FragrantComboBox);
            panel1.Columns[4].EditorParams = new object[] { ArTipos };
            panel1.Columns[5].EditorType = typeof(FragrantComboBox);
            panel1.Columns[5].EditorParams = new object[] { ArParametros };

            panel1.Columns[6].EditorType = typeof(FragrantComboBox);
            panel1.Columns[6].EditorParams = new object[] { ArComp };


            panel2.Columns[3].EditorType = typeof(FragrantComboBox);
            panel2.Columns[3].EditorParams = new object[] { ArParametros };

            //panel2.Columns[4].EditorType = typeof();
            //checkbox1.CheckedChanged += new EventHandler(this.Check_Clicked);

        }


        internal partial class FragrantComboBox : GridComboBoxExEditControl
        {
            public FragrantComboBox(IEnumerable orderArray)
            {
                DataSource = orderArray;
            }
        }


        #endregion








        #region chartControl1_PieSelectionChanged

        /// <summary>
        /// Handles PieSelectionChanged events.
        /// 
        /// We will use this event to display the user selections in the
        /// inner Pie center area.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        void chartControl2_PieSelectionChanged(object sender, PieSelectionChangedEventArgs e) {

            PieChart pieChart = e.PieChart;
            _Timer1.Stop();
            List<PieSeriesPoint> list = pieChart.GetSelectedPoints();

            if (list != null && list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (PieSeriesPoint psp in list)
                    sb.AppendLine((string)psp.ValueX);

                pieChart.CenterLabel = sb.ToString();
                sb[sb.Length - 1] = ' ';
     
                chartControl3.ChartPanel.ChartContainers[0].Titles[0].Text = "Elementos de la Familia: " + sb.ToString() + " (Por Tipo)";


                /* foreach (var tipo in _NwData1) {

                     //_NwData2 = new NwData[FamiliasArmazonesEstructurales.Count()];

                 }*/

                var ListaAuxiliar = from l in CategoriaSeleccionada
                               where l.Familia == sb.ToString().Trim()
                                    select new
                               {
                                   Id = l.Id,
                                   Familia = l.Familia,
                                   Tipo = l.Tipo,
                                   UniqueId = l.UniqueId
                               };
                //var arregloAux;
                int pos = 0, contarValidos=0, encontrado=0;
                //string AuxTipo[150] = null;
                string[] AuxTipo = new string[150];
                foreach (var ItemD in ListaAuxiliar)
                {
                    //AuxTipo.Append(ItemD.Tipo);
                    encontrado = 0;
                    for (int x = 0; x < pos; x++) {
                        if (AuxTipo[x] == ItemD.Tipo)
                            encontrado = 1;
                    }

                    if (encontrado == 0) {
                        AuxTipo[pos] = ItemD.Tipo;
                        pos++;
                        contarValidos++;
                    }


                    //_NwData2 = new NwData[FamiliasArmazonesEstructurales.Count()];

                }

                //MessageBox.Show(AuxTipo.Length.ToString(), "");

                _NwData2 = new NwData[contarValidos];
                for (int x = 0; x < contarValidos; x++) {

                    var Auxiliar = from l in CategoriaSeleccionada
                                   where l.Tipo == AuxTipo[x]
                                   select new
                                   {
                                       Id = l.Id,
                                       Familia = l.Familia,
                                       Tipo = l.Tipo,
                                       UniqueId = l.UniqueId
                                   };


                    _NwData2[x] = new NwData(AuxTipo[x], Auxiliar.Count());
                }


                //_NwData2 = new NwData[FamiliasArmazonesEstructurales.Count()];


                /*switch (sb.ToString().Trim())
                {
                    case "Armazón estructural":
                        _NwData2 = new NwData[FamiliasArmazonesEstructurales.Count()];
                        
                        foreach (RevitFamiliaBase dato in FamiliasArmazonesEstructurales)
                        {
                            var Auxiliar = from l in ArmazonesEstructurales
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData2[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                }*/


                //_NwData1[0].Count = ArmazonesEstructurales.Count();


                if (_NwData2 == null) return;
                InitializeChart2();

                PieChart pieChart1 = (PieChart)chartControl3.ChartPanel.ChartContainers[0];

                pieChart1.PaletteGroup = (PaletteGroup)
                    Enum.Parse(typeof(PaletteGroup), PaletteGroup.Color2.ToString());

                _Timer2 = new System.Windows.Forms.Timer();
                _Timer2.Interval = 1500;
                _Timer2.Tick += Timer_Tick2;

                // Hook the PieSelectionChanged event so that we can give the user
                // feedback on what items they have selected.

                //chartControl2.PieSelectionChanged += chartControl3_PieSelectionChanged;


            }
            else
            {
                // Nothing selected. Show that fact, but do so only
                // for a a short period of time (1-1/2 seconds).
                
                pieChart.CenterLabel = "Nada seleccionado";
                chartControl3.ChartPanel.ChartContainers[0].Titles[0].Text = "No ha seleccionado una categoria";
                _Timer1.Start();
            }






        }
        void chartControl1_PieSelectionChanged(object sender, PieSelectionChangedEventArgs e)
        {
            PieChart pieChart = e.PieChart;
            _Timer.Stop();
            List<PieSeriesPoint> list = pieChart.GetSelectedPoints();
            
            if (list != null && list.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (PieSeriesPoint psp in list)
                    sb.AppendLine((string)psp.ValueX);

                pieChart.CenterLabel = sb.ToString();
                sb[sb.Length - 1] = ' ';
                //TextBoxX1.Text = sb.ToString();
                //chartControl2.ChartPanel.ChartControl
                //chartControl2.ChartPanel.ChartContainers[""].
                //chartControl2.ChartPanel.ChartContainers[]
                /*DevComponents.DotNetBar.Charts.ChartTitle title = new DevComponents.DotNetBar.Charts.ChartTitle();
                title.Text = "City Population Ranking for the 50 Most Populous USA Cities";
                title.XyAlignment = XyAlignment.Top;
                ChartTitleVisualStyle tstyle = title.ChartTitleVisualStyle;
                tstyle.Font = new System.Drawing.Font("Georgia", 16);
                tstyle.TextColor = Color.Navy;
                tstyle.Alignment = DevComponents.DotNetBar.Charts.Style.Alignment.MiddleCenter;
                tstyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(10);
                chartControl2.ChartPanel.Titles.Add(title);*/

                

                chartControl2.ChartPanel.ChartContainers[0].Titles[0].Text = "Elementos de la Categoría: " + sb.ToString() +  " (Por Familia)";

                switch (sb.ToString().Trim()) {
                    case "Armazón estructural":
                        CategoriaSeleccionada = ArmazonesEstructurales.ToList();
                        FamiliaSeleccionada = FamiliasArmazonesEstructurales.ToList();
                        TipoSeleccionado = TiposArmazonesEstructurales.ToList();
                        
                        _NwData1 = new NwData[FamiliasArmazonesEstructurales.Count()];
                        int pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasArmazonesEstructurales)
                        {
                            var Auxiliar = from l in ArmazonesEstructurales
                                           where l.Familia == dato.Familia
                                                   select new
                                                   {
                                                       Id = l.Id,
                                                       Familia = l.Familia,
                                                       Tipo = l.Tipo,
                                                       UniqueId= l.UniqueId
                                                   };
                            //dato.Familia;
                            _NwData1[pos] =  new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;

                    case "Armadura estructural":
                        CategoriaSeleccionada = ArmadurasEstructuales.ToList();
                        FamiliaSeleccionada = FamiliasArmazonesEstructurales.ToList();
                        TipoSeleccionado = TiposArmazonesEstructurales.ToList();

                        _NwData1 = new NwData[FamiliasArmadurasEstructuales.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasArmadurasEstructuales)
                        {
                            var Auxiliar = from l in ArmadurasEstructuales
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;

                    case "Bandejas de cables":
                        CategoriaSeleccionada = BandejasdeCables.ToList();
                        FamiliaSeleccionada = FamiliasBandejasdeCables.ToList();
                        TipoSeleccionado = TiposBandejasdeCables.ToList();

                        _NwData1 = new NwData[FamiliasBandejasdeCables.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasBandejasdeCables)
                        {
                            var Auxiliar = from l in BandejasdeCables
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;

                    case "Barandillas":
                        CategoriaSeleccionada = Barandillas.ToList();
                        FamiliaSeleccionada = FamiliasBarandillas.ToList();
                        TipoSeleccionado = TiposBarandillas.ToList();

                        _NwData1 = new NwData[FamiliasBarandillas.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasBarandillas)
                        {
                            var Auxiliar = from l in Barandillas
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Cimentación estructural":
                        CategoriaSeleccionada = CimentacionesEstructurales.ToList();
                        FamiliaSeleccionada = FamiliasCimentacionesEstructurales.ToList();
                        TipoSeleccionado = TiposCimentacionesEstructurales.ToList();

                        _NwData1 = new NwData[FamiliasCimentacionesEstructurales.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasCimentacionesEstructurales)
                        {
                            var Auxiliar = from l in CimentacionesEstructurales
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Conductos":
                        CategoriaSeleccionada = Conductos.ToList();
                        FamiliaSeleccionada = FamiliasConductos.ToList();
                        TipoSeleccionado = TiposConductos.ToList();

                        _NwData1 = new NwData[FamiliasConductos.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasConductos)
                        {
                            var Auxiliar = from l in Conductos
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Conductos flexibles":
                        CategoriaSeleccionada = ConductosFlexibles.ToList();
                        FamiliaSeleccionada = FamiliasConductosFlexibles.ToList();
                        TipoSeleccionado = TiposConductosFlexibles.ToList();

                        _NwData1 = new NwData[FamiliasConductosFlexibles.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasConductosFlexibles)
                        {
                            var Auxiliar = from l in ConductosFlexibles
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Conexiones estructurales":
                        CategoriaSeleccionada = ConexionesEstructurales.ToList();
                        FamiliaSeleccionada = FamiliasConexionesEstructurales.ToList();
                        TipoSeleccionado = TiposConexionesEstructurales.ToList();

                        _NwData1 = new NwData[FamiliasConexionesEstructurales.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasConexionesEstructurales)
                        {
                            var Auxiliar = from l in ConexionesEstructurales
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Cubiertas":
                        CategoriaSeleccionada = Cubiertas.ToList();
                        FamiliaSeleccionada = FamiliasCubiertas.ToList();
                        TipoSeleccionado = TiposCubiertas.ToList();

                        _NwData1 = new NwData[FamiliasCubiertas.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasCubiertas)
                        {
                            var Auxiliar = from l in Cubiertas
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Emplazamiento":
                        CategoriaSeleccionada = Emplazamientos.ToList();
                        FamiliaSeleccionada = FamiliasEmplazamientos.ToList();
                        TipoSeleccionado = TiposEmplazamientos.ToList();

                        _NwData1 = new NwData[FamiliasEmplazamientos.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasEmplazamientos)
                        {
                            var Auxiliar = from l in Emplazamientos
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Escaleras":
                        CategoriaSeleccionada = Escaleras.ToList();
                        FamiliaSeleccionada = FamiliasEscaleras.ToList();
                        TipoSeleccionado = TiposEscaleras.ToList();

                        _NwData1 = new NwData[FamiliasEscaleras.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasEscaleras)
                        {
                            var Auxiliar = from l in Escaleras
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Modelos genéricos":
                        CategoriaSeleccionada = ModelosGenericos.ToList();
                        FamiliaSeleccionada = FamiliasModelosGenericos.ToList();
                        TipoSeleccionado = TiposModelosGenericos.ToList();

                        _NwData1 = new NwData[FamiliasModelosGenericos.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasModelosGenericos)
                        {
                            var Auxiliar = from l in ModelosGenericos
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Aparatos sanitarios":
                        CategoriaSeleccionada = AparatosSanitarios.ToList();
                        FamiliaSeleccionada = FamiliasAparatosSanitarios.ToList();
                        TipoSeleccionado = TiposAparatosSanitarios.ToList();

                        _NwData1 = new NwData[FamiliasAparatosSanitarios.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasAparatosSanitarios)
                        {
                            var Auxiliar = from l in AparatosSanitarios
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Muros":
                        CategoriaSeleccionada = Muros.ToList();
                        FamiliaSeleccionada = FamiliasMuros.ToList();
                        TipoSeleccionado = TiposMuros.ToList();

                        _NwData1 = new NwData[FamiliasMuros.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasMuros)
                        {
                            var Auxiliar = from l in Muros
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Pilares estructurales":
                        CategoriaSeleccionada = PilaresEstructurales.ToList();
                        FamiliaSeleccionada = FamiliasPilaresEstructurales.ToList();
                        TipoSeleccionado = TiposPilaresEstructurales.ToList();

                        _NwData1 = new NwData[FamiliasPilaresEstructurales.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasPilaresEstructurales)
                        {
                            var Auxiliar = from l in PilaresEstructurales
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Rampas":
                        CategoriaSeleccionada = Rampas.ToList();
                        FamiliaSeleccionada = FamiliasRampas.ToList();
                        TipoSeleccionado = TiposRampas.ToList();

                        _NwData1 = new NwData[FamiliasRampas.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasRampas)
                        {
                            var Auxiliar = from l in Rampas
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Suelos":
                        CategoriaSeleccionada = Suelos.ToList();
                        FamiliaSeleccionada = FamiliasSuelos.ToList();
                        TipoSeleccionado = TiposSuelos.ToList();

                        _NwData1 = new NwData[FamiliasSuelos.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasSuelos)
                        {
                            var Auxiliar = from l in Suelos
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Techos":
                        CategoriaSeleccionada = Techos.ToList();
                        FamiliaSeleccionada = FamiliasTechos.ToList();
                        TipoSeleccionado = TiposTechos.ToList();

                        _NwData1 = new NwData[FamiliasTechos.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasTechos)
                        {
                            var Auxiliar = from l in Techos
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Tuberías":
                        CategoriaSeleccionada = Tuberias.ToList();
                        FamiliaSeleccionada = FamiliasTuberias.ToList();
                        TipoSeleccionado = TiposTuberias.ToList();

                        _NwData1 = new NwData[FamiliasTuberias.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasTuberias)
                        {
                            var Auxiliar = from l in Tuberias
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Tuberías flexibles":
                        CategoriaSeleccionada = TuberiasFlexibles.ToList();
                        FamiliaSeleccionada = FamiliasTuberiasFlexibles.ToList();
                        TipoSeleccionado = TiposTuberiasFlexibles.ToList();

                        _NwData1 = new NwData[FamiliasTuberiasFlexibles.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasTuberiasFlexibles)
                        {
                            var Auxiliar = from l in TuberiasFlexibles
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Tubos":
                        CategoriaSeleccionada = Tubos.ToList();
                        FamiliaSeleccionada = FamiliasTubos.ToList();
                        TipoSeleccionado = TiposTubos.ToList();

                        _NwData1 = new NwData[FamiliasTubos.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasTubos)
                        {
                            var Auxiliar = from l in Tubos
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;
                    case "Topografía":
                        CategoriaSeleccionada = Topografias.ToList();
                        FamiliaSeleccionada = FamiliasTopografias.ToList();
                        TipoSeleccionado = TiposTopografias.ToList();

                        _NwData1 = new NwData[FamiliasTopografias.Count()];
                        pos = 0;
                        foreach (RevitFamiliaBase dato in FamiliasTopografias)
                        {
                            var Auxiliar = from l in Topografias
                                           where l.Familia == dato.Familia
                                           select new
                                           {
                                               Id = l.Id,
                                               Familia = l.Familia,
                                               Tipo = l.Tipo,
                                               UniqueId = l.UniqueId
                                           };
                            //dato.Familia;
                            _NwData1[pos] = new NwData(dato.Familia.ToString(), Auxiliar.Count());
                            pos++;
                        }
                        break;

                }


                //_NwData1[0].Count = ArmazonesEstructurales.Count();


                if (_NwData1 == null) return;
                InitializeChart1();
                
                PieChart pieChart1 = (PieChart)chartControl2.ChartPanel.ChartContainers[0];

                pieChart1.PaletteGroup = (PaletteGroup)
                    Enum.Parse(typeof(PaletteGroup), PaletteGroup.Color2.ToString());

                _Timer1 = new System.Windows.Forms.Timer();
                _Timer1.Interval = 1500;
                _Timer1.Tick += Timer_Tick1;

                // Hook the PieSelectionChanged event so that we can give the user
                // feedback on what items they have selected.

                chartControl2.PieSelectionChanged += chartControl2_PieSelectionChanged;


            }
            else
            {
                // Nothing selected. Show that fact, but do so only
                // for a a short period of time (1-1/2 seconds).
                TextBoxX1.Text = "";
                pieChart.CenterLabel = "Nada seleccionado";
                chartControl2.ChartPanel.ChartContainers[0].Titles[0].Text = "No ha seleccionado una categoria";
                _Timer.Start();
            }
        }

        #endregion


        #region InitializeChart

        //PieChart pieChart = null;
        //public PieSeries series = null;


        /// <summary>
        /// Initializes our chart.
        /// </summary>
        private void InitializeChart()
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            AddSeries(pieChart);
        }

        private void InitializeChart1()
        {
            PieChart pieChart =
                (PieChart)chartControl2.ChartPanel.ChartContainers[0];
            AddSeries1(pieChart);
        }
        private void InitializeChart2()
        {
            PieChart pieChart =
                (PieChart)chartControl3.ChartPanel.ChartContainers[0];
            AddSeries2(pieChart);
        }


        #region AddSeries

        /// <summary>
        /// Creates a new series from the given data.
        /// </summary>
        /// <param name="chartXy"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <param name="color"></param>
        private void AddSeries(PieChart pieChart)
        {
            PieSeries series = new PieSeries("NwPieSeries");
            series = new PieSeries("NwPieSeries");
            // Set our format string to display the country name (X), followed
            // by the 'angular' percentage of the pie (VAL or V format code).

            string format = "{x} - {v:0.0#%}";

            // Iterate through each data element, creating and adding a new
            // PieSeriesPoint for each respective entry.

            foreach (NwData data in _NwData)
            {
                PieSeriesPoint psp = new PieSeriesPoint(data.Country, data.Count);

                psp.LegendText = format;

                series.SeriesPoints.Add(psp);
            }

            // Get a reference to the control generated "Other" slice, and
            // establish some defaults for its dislpay and behavior.

            PieSeriesPoint opsp = series.GetOtherPieSeriesPoint();

            // Legend related.

            opsp.LegendText = format;
            opsp.LegendItem.ChartLegendItemVisualStyles.Default.TextColor = System.Drawing.Color.Green;

            // Setup some Outer label styles for the 'Default' state.

            SliceOuterLabelVisualStyle ostyle =
                opsp.SliceVisualStyles.Default.SliceOuterLabelStyle;

            ostyle.Border.LineWidth = 1;
            ostyle.Border.LinePattern = DevComponents.DotNetBar.Charts.Style.LinePattern.Dash;
            ostyle.Border.LineColor = System.Drawing.Color.DarkGreen;
            ostyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(4);

            ostyle.TextColor = System.Drawing.Color.White;
            ostyle.Background = new DevComponents.DotNetBar.Charts.Style.Background(System.Drawing.Color.Green);

            // Setup some Outer label styles for the 'MouseOver' state.

            ostyle = opsp.SliceVisualStyles.MouseOver.SliceOuterLabelStyle;

            ostyle.TextColor = System.Drawing.Color.Black;
            ostyle.Background = new DevComponents.DotNetBar.Charts.Style.Background(System.Drawing.Color.GreenYellow);

            // Add the series to the chart.

            pieChart.ChartSeries.Add(series);
        }


        private void AddSeries1(PieChart pieChart)
        {
            pieChart.ChartSeries.Clear();
            PieSeries series = new PieSeries("NwPieSeries");
            series = new PieSeries("NwPieSeries");
            // Set our format string to display the country name (X), followed
            // by the 'angular' percentage of the pie (VAL or V format code).

            string format = "{x} - {v:0.0#%}";

            // Iterate through each data element, creating and adding a new
            // PieSeriesPoint for each respective entry.

            foreach (NwData data in _NwData1)
            {
                PieSeriesPoint psp = new PieSeriesPoint(data.Country, data.Count);

                psp.LegendText = format;

                series.SeriesPoints.Add(psp);
            }

            // Get a reference to the control generated "Other" slice, and
            // establish some defaults for its dislpay and behavior.

            PieSeriesPoint opsp = series.GetOtherPieSeriesPoint();

            // Legend related.

            opsp.LegendText = format;
            opsp.LegendItem.ChartLegendItemVisualStyles.Default.TextColor = System.Drawing.Color.Green;

            // Setup some Outer label styles for the 'Default' state.

            SliceOuterLabelVisualStyle ostyle =
                opsp.SliceVisualStyles.Default.SliceOuterLabelStyle;

            ostyle.Border.LineWidth = 1;
            ostyle.Border.LinePattern = DevComponents.DotNetBar.Charts.Style.LinePattern.Dash;
            ostyle.Border.LineColor = System.Drawing.Color.DarkGreen;
            ostyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(4);

            ostyle.TextColor = System.Drawing.Color.White;
            ostyle.Background = new DevComponents.DotNetBar.Charts.Style.Background(System.Drawing.Color.Green);

            // Setup some Outer label styles for the 'MouseOver' state.

            ostyle = opsp.SliceVisualStyles.MouseOver.SliceOuterLabelStyle;

            ostyle.TextColor = System.Drawing.Color.Black;
            ostyle.Background = new DevComponents.DotNetBar.Charts.Style.Background(System.Drawing.Color.GreenYellow);

            // Add the series to the chart.

            pieChart.ChartSeries.Add(series);
        }

        private void AddSeries2(PieChart pieChart)
        {
            pieChart.ChartSeries.Clear();
            PieSeries series = new PieSeries("NwPieSeries");
            series = new PieSeries("NwPieSeries");
            // Set our format string to display the country name (X), followed
            // by the 'angular' percentage of the pie (VAL or V format code).

            string format = "{x} - {v:0.0#%}";

            // Iterate through each data element, creating and adding a new
            // PieSeriesPoint for each respective entry.

            foreach (NwData data in _NwData2)
            {
                PieSeriesPoint psp = new PieSeriesPoint(data.Country, data.Count);
                psp.LegendText = format;
                series.SeriesPoints.Add(psp);
            }

            // Get a reference to the control generated "Other" slice, and
            // establish some defaults for its dislpay and behavior.

            PieSeriesPoint opsp = series.GetOtherPieSeriesPoint();

            // Legend related.

            opsp.LegendText = format;
            opsp.LegendItem.ChartLegendItemVisualStyles.Default.TextColor = System.Drawing.Color.Green;

            // Setup some Outer label styles for the 'Default' state.

            SliceOuterLabelVisualStyle ostyle =
                opsp.SliceVisualStyles.Default.SliceOuterLabelStyle;

            ostyle.Border.LineWidth = 1;
            ostyle.Border.LinePattern = DevComponents.DotNetBar.Charts.Style.LinePattern.Dash;
            ostyle.Border.LineColor = System.Drawing.Color.DarkGreen;
            ostyle.Padding = new DevComponents.DotNetBar.Charts.Style.Padding(4);

            ostyle.TextColor = System.Drawing.Color.White;
            ostyle.Background = new DevComponents.DotNetBar.Charts.Style.Background(System.Drawing.Color.Green);

            // Setup some Outer label styles for the 'MouseOver' state.

            ostyle = opsp.SliceVisualStyles.MouseOver.SliceOuterLabelStyle;

            ostyle.TextColor = System.Drawing.Color.Black;
            ostyle.Background = new DevComponents.DotNetBar.Charts.Style.Background(System.Drawing.Color.GreenYellow);

            // Add the series to the chart.

            pieChart.ChartSeries.Add(series);
        }
        #endregion

        #endregion


        #region Timer_Tick


        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Timer_Tick(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.CenterLabel = null;
        }
        void Timer_Tick1(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl2.ChartPanel.ChartContainers[0];

            pieChart.CenterLabel = null;
        }
        void Timer_Tick2(object sender, EventArgs e)
        {
            PieChart pieChart =
                (PieChart)chartControl3.ChartPanel.ChartContainers[0];
            pieChart.CenterLabel = null;
        }


        #endregion

        #region NwData Class def

        private class NwData
        {
            public string Country;
            public int Count;

            public NwData(string country, int count)
            {
                Country = country;
                Count = count;
            }



        }

        #endregion

        #endregion




        private System.Windows.Forms.Screen pantalla = System.Windows.Forms.Screen.PrimaryScreen;
   
        internal static string tokk = string.Empty;
        //private static string FORGE_CLIENT_ID;
        //private static string FORGE_CLIENT_SECRET;
        //private static string FORGE_CALLBACK;//= "http://localhost:3000/api/forge/callback/oauth";
        private static Scope[] _scope = new Scope[] { Scope.DataRead, Scope.DataWrite, Scope.AccountRead, Scope.ViewablesRead };
        private static ThreeLeggedApi _threeLeggedApi = new ThreeLeggedApi();
        internal delegate void NewBearerDelegate(dynamic bearer);
        public string UNIQID = "";
        public string PlanoCargado = "";


        //System.Data.OleDb.OleDbConnection conex = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + "C:\\MrdtC\\bdrubros.mdb");
        //System.Data.OleDb.OleDbConnection conex_1 = new System.Data.OleDb.OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\\MrdtC\\Presup.mdb");

        #region variables

        

       
        
        private DevComponents.DotNetBar.ElementStyle _RightAlignFileSizeStyle;


        #endregion
        //CefSharp.WinForms.ChromiumWebBrowser bRowser;


        public dynamic JObjetoPres;
        public JArray ArregloPres;


        #region ObjetosDatos

        public class LPresupuestos
        {
            public string ERPCode { get; set; }
            public string CodPresupuesto { get; set; }
            public int Nivel { get; set; }
            public string Descripcion { get; set; }
            public string PhantomId { get; set; }
            public string PhantomParentId { get; set; }
            public int Fila { get; set; }
        }

        public class LSubPresupuestos
        {
            public string CodSubpresupuesto { get; set; }
            public string Descripcion { get; set; }

        }

        public class LItems
        {
            public string ERPCode { get; set; }
            public string CodPresupuesto { get; set; }
            public string CodSubpresupuesto { get; set; }
            public string Item { get; set; }
            public string Orden { get; set; }
            public int Nivel { get; set; }
            public string Descripcion { get; set; }
            public string Unidad { get; set; }
            public string Metrado { get; set; }
            public string Precio1 { get; set; }
            public string Precio2 { get; set; }
            public string PhantomParentId { get; set; }
            public int Fila { get; set; }

        }

        public class ResponseExecuteS10ERPData
        {
            public string Data { get; set; }
            public string Key { get; set; }
            public string Name { get; set; }
            public int Part { get; set; }
            public int Count { get; set; }
            public string TargetName { get; set; }
            public string SignalRConnectionID { get; set; }
            public string OutputValue { get; set; }
        }

        public static List<LPresupuestos> ListaPresupuestos = null;
        private List<LSubPresupuestos> ListaSubPresupuestos = null;
        public static List<LItems> ListaItems = null;

        /*  "ERPCode": "01",
        "CodPresupuesto": "01",
        "Nivel": 1,
        "Descripcion": "ADMINISTRATIVO TRIADA",
        "PhantomId": "01",
        "PhantomParentId": null,
        "Fila": 1*/

        public class LItemsAPU
        {
            public string TipoDetalle { get; set; }
            public string Descripcion { get; set; }
            public string Unidad { get; set; }
            public string CuadrillaInsumo { get; set; }
            public string CantidadInsumo { get; set; }
            public string PrecioInsumo1 { get; set; }
            public string PrecioInsumo2 { get; set; }
            public string Parcial1 { get; set; }
            public string Parcial2 { get; set; }

        }
        private List<LItemsAPU> ListaItemsAPU = null;


        public class LAsociado
        {
            public string CodPresupuesto { get; set; }
            public string CodSubpresupuesto { get; set; }
            public string Item { get; set; }
            public string CodAsociado { get; set; }
            public string Categoria { get; set; }
            public string Familia { get; set; }
            public string Tipo { get; set; }
            public string CampoFiltro { get; set; }
            public string Valor { get; set; }
        }
        private List<LAsociado> ListaAsociados = null;

        public class LEstructura
        {
            public string CodPresupuesto { get; set; }
            public string CodSubpresupuesto { get; set; }
            public string Item { get; set; }
            public string CodEstructura { get; set; }
            public string Nivel { get; set; }
            public string Campo { get; set; }
            public string Mostrar { get; set; }
        }
        private List<LEstructura> ListaEstructuras = null;

        public class LCalculo
        {
            public string CodPresupuesto { get; set; }
            public string CodSubpresupuesto { get; set; }
            public string Item { get; set; }
            public string CodCalculo { get; set; }
            public string Descripcion { get; set; }
            public string Cantidad { get; set; }
            public string Longitud { get; set; }
            public string Ancho { get; set; }
            public string Alto { get; set; }
        }
        private List<LCalculo> ListaCalculos = null;

        public class LCalculoDetalle
        {
            public string CodPresupuesto { get; set; }
            public string CodSubpresupuesto { get; set; }
            public string Item { get; set; }
            public string CodCalculoDetalle { get; set; }
            public string CodCalculo { get; set; }
            public string TipoCampo { get; set; }
            public string Campo { get; set; }
            public string Operacion { get; set; }
            public string Posicion { get; set; }
        }

        private List<LCalculoDetalle> ListaCalculoDetalle = null;


        public class LMedicion
        {
            public string CodMedicion { get; set; }
            public string CodPresupuesto { get; set; }
            public string CodSubpresupuesto { get; set; }
            public string Item { get; set; }
            public string Descripcion { get; set; }
            public string Cantidad { get; set; }
            public string Longitud { get; set; }
            public string Ancho { get; set; }
            public string Alto { get; set; }
            public string Total { get; set; }
            public string Detalle { get; set; }
            public string Vinculo { get; set; }
            public string UniqueId { get; set; }
            public string PhantomParentId { get; set; }
            public int Nivel { get; set; }
            public string Tipo { get; set; }
        }

        private List<LMedicion> ListaItemsMedicion = null;



        public class LParameters
        {
            public string P { get; set; }
            public int O { get; set; }
        }


        public class LData
        {
            public int Id { get; set; }
            public LParameters[] Parameters { get; set; }
        }
        private List<LData> Data = null;

        #endregion



        #region SOLICITUDES


        private async void autentificar_S10()
        {
            RestClient client = new RestClient("http://200.48.100.203:5033/api");
            RestRequest request = new RestRequest("/SecurityAuthApi/LogonApp", RestSharp.Method.POST);
            request.AddParameter("ModuleId", "11");
            request.AddParameter("AccessTypeId", "2");
            request.AddParameter("UserName", "ctorres@s10peru.com");
            request.AddParameter("Password", "uT9pLH4V");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //IRestResponse response = await client.ExecuteTaskAsync(request);
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
            TokenS10.Text = responseDynamic.Value.Companies[0].Token.ToString();
        }

        private async void solicitar_datosPresupuestos()
        {

            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "true");
            request.AddParameter("ObjectName", "dbo.S10_00_Identificador_ListarPorDescripcionPaginado '', 12, 1");
            request.AddParameter("RequestId", "BusinessPartner");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "580");

            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //IRestResponse response = await client.ExecuteTaskAsync(request);
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
            //TokenS10.Text = responseDynamic.Value.Companies[0].Token.ToString();
        }

        private async void solicitar_datosArbPresupuestos()
        {           
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_Presupuesto_ListarArbol 'ncortez@s10peru.com'");
            request.AddParameter("RequestId", "Arbol");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //IRestResponse response = await client.ExecuteTaskAsync(request);
            IRestResponse response = await client.ExecuteAsync(request);
            
                dynamic responseDynamic = JObject.Parse(response.Content);
                textBox1.Text = JObject.Parse(response.Content).ToString();

            //TokenS10.Text = responseDynamic.Value.Companies[0].Token.ToString();
        }



        private async void solicitar_datosSubPresupuestos()
        {
            ListaSubPresupuestos = new List<LSubPresupuestos>();
            GridRow Filasel = (GridRow) SGPresupuestos.PrimaryGrid.ActiveRow;
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_Presupuesto_ListarSubpresupuestos '" + Filasel.Cells[0].Value.ToString() + "'");
            request.AddParameter("RequestId", "SubPresupuesto");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
            
        }



        public async void solicitar_datosItems()
        {
            ListaItems = new List<LItems>();
            GridRow Filasel = (GridRow)SGPresupuestos.PrimaryGrid.ActiveRow;
            GridRow Filapadre = (GridRow)Filasel.Parent;
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_Presupuesto_ListarHoja '" + Filapadre.Cells[0].Value.ToString() + "','" + Filasel.Cells[0].Value.ToString() + "'");
            request.AddParameter("RequestId", "Items");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();

        }

        public static async void solicitar_datosItems1()
        {
            ListaItems = new List<LItems>();
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_Presupuesto_ListarHoja '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "'");
            request.AddParameter("RequestId", "Items");
            request.AddParameter("SignalRConnectionID", signalstk);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", tokenstk);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            //textBox1.Text = JObject.Parse(response.Content).ToString();

        }


        private async void solicitar_datosApus()
        {
            ListaItemsAPU = new List<LItemsAPU>();
            GridRow Filasel = (GridRow)SGPresupuestos.PrimaryGrid.ActiveRow;
            if (Filasel is null) return;
            if ((string)Filasel.Parent.GetType().Name.ToString() == "GridPanel") return;
            GridRow Filapadre = (GridRow)Filasel.Parent;

            GridRow FilaselItem = (GridRow)SGPresupC.PrimaryGrid.ActiveRow;

            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            //"dbo.s10_01_SubpresupuestoDetalle_ListarAnalisisPU '0101001', '001', '000000000000009'"
            request.AddParameter("ObjectName", "dbo.s10_01_SubpresupuestoDetalle_ListarAnalisisPU '" + Filapadre.Cells[0].Value.ToString() + "','" + Filasel.Cells[0].Value.ToString() + "','" + FilaselItem.Cells[17].Value.ToString() + "'");
            request.AddParameter("RequestId", "ItemsAPU");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();

        }

        #endregion


        


        public FrmPresupuestos()
        {
            EO.WebBrowser.Runtime.AddLicense(
            "3a5rp7PD27FrmaQHEPGs4PP/6KFrqKax2r1GgaSxy5916u34GeCt7Pb26bSG" +
            "prT6AO5p3Nfh5LRw4rrqHut659XO6Lto6u34GeCt7Pb26YxDs7P9FOKe5ff2" +
            "6YxDdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW1yQKzbam2xvGvcau0weKv" +
            "fLOz/RTinuX39vTjd4SOscufWbPw+g7kp+rp9unMmuX59hefi9jx+h3ks7Oz" +
            "/RTinuX39hC9RoGkscufddjw/Rr2d4SOscufWZekscu7mtvosR/4qdzBs/DO" +
            "Z7rsAxrsnpmkBxDxrODz/+iha6iywc2faLWRm8ufWZfAwAzrpeb7z7iJWZek" +
            "sefuq9vpA/Ttn+ak9QzznrSmyNqxaaa2wd2wW5f3Bg3EseftAxDyeuvBs+I=");

            InitializeComponent();
            //webView21.CoreWebView2.Navigate(@"C:\Users\CRISTIAN\source\repos\S10Cuantificacion\S10Cuantificacion\HTML\Viewer.html");
            //webView21.NavigateToString(@"C:\Users\CRISTIAN\source\repos\S10Cuantificacion\S10Cuantificacion\HTML\Viewer.html");

            //webView21.NavigateToString("file:///HTML/Viewer.html");

            //bRowser = new CefSharp.WinForms.ChromiumWebBrowser("file:///HTML/Viewer.html"); // CefSharp needs a initial page...

            //bRowser = new CefSharp.WinForms.ChromiumWebBrowser("www.google.com"); // CefSharp needs a initial page...
            //webBrowser2.Url="file:///HTML/Viewer.html"; // CefSharp needs a initial page...
            //webBrowser2.Navigate("file:///HTML/Viewer.html");
            /*bRowser.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));*/
            //bRowser.MinimumSize = new System.Drawing.Size(20, 20);
            //bRowser.Name = "webBRowser1";
            //bRowser.TabIndex = 1;
            //bRowser.Dock = DockStyle.Fill;
  

            //panelEx1.Controls.Add(bRowser);

        }





        #region bRowser to c#

        private System.Windows.Forms.Timer _tokenTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer _translationTimer = new System.Windows.Forms.Timer();
        private DateTime _expiresAt;

        private async void autentificar()
        {
            if (string.IsNullOrWhiteSpace(txtClientId.Text) || string.IsNullOrWhiteSpace(txtClientSecret.Text)) return;
            Scope[] scope = new Scope[] { Scope.DataRead, Scope.DataWrite };
            //LLAMADA A LA API. DOCUMENTACION AUTODESK
            //https://forge.autodesk.com/en/docs/oauth/v1/reference/http/authenticate-POST/
            // get the access token
            //https://developer.api.autodesk.com/authentication/v1/authorize
            RestClient client = new RestClient("https://developer.api.autodesk.com");
            RestRequest request = new RestRequest("/authentication/v1/authenticate", RestSharp.Method.POST);
            //RestRequest request = new RestRequest("/authentication/v1/authorize", RestSharp.Method.POST);
            request.AddParameter("client_id", txtClientId.Text);
            request.AddParameter("client_secret", txtClientSecret.Text);
            request.AddParameter("grant_type", "client_credentials");
            //request.AddParameter("refresh_token", "client_credentials");
            //request.AddParameter("scope", "data:read data:write data:create data:search bucket:create bucket:read bucket:update bucket:delete", ParameterType.UrlSegment);
            request.AddParameter("scope", "data:read data:write data:create data:search bucket:create bucket:read bucket:update bucket:delete");
            //request.AddParameter("scope", { Scope.DataRead, Scope.DataWrite }, ParameterType.QueryStringWithoutEncode);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteTaskAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            txtAccessToken.Text = responseDynamic.access_token;
            advTree1.Nodes.Clear();
            try
            {
                _expiresAt = DateTime.Now.AddSeconds((double)(responseDynamic.expires_in));
                // keep track on time
                _tokenTimer.Tick += new EventHandler(tickTokenTimer);
                _tokenTimer.Interval = 1000;
                _tokenTimer.Enabled = true;
            }
            catch
            {
            }
        }

        private async void btnAuthenticate_Click(object sender, EventArgs e)
        {

        }
        void tickTokenTimer(object sender, EventArgs e)
        {
            // update the time left on the access token
            double secondsLeft = (_expiresAt - DateTime.Now).TotalSeconds;
            txtTimeout.Text = secondsLeft.ToString("0");
            txtTimeout.BackColor = (secondsLeft < 60 ? System.Drawing.Color.Red : System.Drawing.SystemColors.Control);
            if (textosaca != "")
            {
                TextBoxX1.Text = textosaca;
                textosaca = "";
            }
        }

        string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            var ddd = Convert.ToBase64String(plainTextBytes);
            return Convert.ToBase64String(plainTextBytes);
        }

        string ViewerURN(string urn, string viewableId)
        {
            string respuesta = string.Empty;
            var curiosidad = Base64Encode(urn);
            if (System.String.IsNullOrEmpty(viewableId))//vista 3D               
                //respuesta = string.Format("file:///HTML/Viewer.html?URN={0}&Token={1}", Base64Encode(urn), txtAccessToken.Text);
                respuesta = string.Format(@"C:\HTML\Viewer.html?URN={0}&Token={1}", Base64Encode(urn), txtAccessToken.Text);
            else
                //respuesta = string.Format("file:///HTML/Viewer.html?URN={0}&Token={1}&ViewableId={2}", Base64Encode(urn), txtAccessToken.Text, viewableId);
                respuesta = string.Format(@"C:\HTML\Viewer.html?URN={0}&Token={1}&ViewableId={2}", Base64Encode(urn), txtAccessToken.Text, viewableId);
            return respuesta;
        }

        public static System.String textosaca = "";
        public static int veces = 1;

        public class CallbackObjectForJs
        {
            public void showMessage(List<System.Object> msg, int n)
            {
                if (veces == n)
                {
                    System.Windows.Forms.MessageBox.Show(System.String.Join(",", msg));
                    veces = 0;
                }
                veces++;
                //MessageBox.Show(String.Join(",", msg, "-" , n.ToString()));
                //MessageBox.Show(msg + " " + n);
                //TextBox nnn = new TextBox();
                //BIM360_Menfis.textosaca = String.Join(",", msg);
                textosaca = System.String.Join(",", msg);
                //FrmPresupuestos Fr1 = new FrmPresupuestos();
                //Fr1.Show();
                //obj.TextBoxX1.Text = msg.ToString();
                //TextBoxX1.Text=msg.ToString();


            }
        }
        private CallbackObjectForJs _callBackObjectForJs;






        #endregion



        void selecciona_categoria(string categ) {
            /*CategoriaSeleccionada = FamiliaTotal.ToList();
            FamiliaSeleccionada = FamiliasArmazonesEstructurales.ToList();
            TipoSeleccionado = TiposArmazonesEstructurales.ToList();*/
            switch (categ)
            {
                case "Armazón estructural":
                    CategoriaSeleccionada = ArmazonesEstructurales.ToList();
                    FamiliaSeleccionada = FamiliasArmazonesEstructurales.ToList();
                    TipoSeleccionado = TiposArmazonesEstructurales.ToList();
                    break;
                case "Armadura estructural":
                    CategoriaSeleccionada = ArmadurasEstructuales.ToList();
                    FamiliaSeleccionada = FamiliasArmadurasEstructuales.ToList();
                    TipoSeleccionado = TiposArmadurasEstructuales.ToList();
                    break;
                case "Bandejas de cables":
                    CategoriaSeleccionada = BandejasdeCables.ToList();
                    FamiliaSeleccionada = FamiliasBandejasdeCables.ToList();
                    TipoSeleccionado = TiposBandejasdeCables.ToList();
                    break;
                case "Barandillas":
                    CategoriaSeleccionada = Barandillas.ToList();
                    FamiliaSeleccionada = FamiliasBarandillas.ToList();
                    TipoSeleccionado = TiposBarandillas.ToList();

                    break;
                case "Cimentación estructural":
                    CategoriaSeleccionada = CimentacionesEstructurales.ToList();
                    FamiliaSeleccionada = FamiliasCimentacionesEstructurales.ToList();
                    TipoSeleccionado = TiposCimentacionesEstructurales.ToList();

                    break;
                case "Conductos":
                    CategoriaSeleccionada = Conductos.ToList();
                    FamiliaSeleccionada = FamiliasConductos.ToList();
                    TipoSeleccionado = TiposConductos.ToList();

                    break;
                case "Conductos flexibles":
                    CategoriaSeleccionada = ConductosFlexibles.ToList();
                    FamiliaSeleccionada = FamiliasConductosFlexibles.ToList();
                    TipoSeleccionado = TiposConductosFlexibles.ToList();

                    break;
                case "Conexiones estructurales":
                    CategoriaSeleccionada = ConexionesEstructurales.ToList();
                    FamiliaSeleccionada = FamiliasConexionesEstructurales.ToList();
                    TipoSeleccionado = TiposConexionesEstructurales.ToList();

                    break;
                case "Cubiertas":
                    CategoriaSeleccionada = Cubiertas.ToList();
                    FamiliaSeleccionada = FamiliasCubiertas.ToList();
                    TipoSeleccionado = TiposCubiertas.ToList();

                    break;
                case "Emplazamiento":
                    CategoriaSeleccionada = Emplazamientos.ToList();
                    FamiliaSeleccionada = FamiliasEmplazamientos.ToList();
                    TipoSeleccionado = TiposEmplazamientos.ToList();

                    break;
                case "Escaleras":
                    CategoriaSeleccionada = Escaleras.ToList();
                    FamiliaSeleccionada = FamiliasEscaleras.ToList();
                    TipoSeleccionado = TiposEscaleras.ToList();

                    break;
                case "Modelos genéricos":
                    CategoriaSeleccionada = ModelosGenericos.ToList();
                    FamiliaSeleccionada = FamiliasModelosGenericos.ToList();
                    TipoSeleccionado = TiposModelosGenericos.ToList();

                    break;
                case "Aparatos sanitarios":
                    CategoriaSeleccionada = AparatosSanitarios.ToList();
                    FamiliaSeleccionada = FamiliasAparatosSanitarios.ToList();
                    TipoSeleccionado = TiposAparatosSanitarios.ToList();

                    break;
                case "Muros":
                    CategoriaSeleccionada = Muros.ToList();
                    FamiliaSeleccionada = FamiliasMuros.ToList();
                    TipoSeleccionado = TiposMuros.ToList();

                    break;
                case "Pilares estructurales":
                    CategoriaSeleccionada = PilaresEstructurales.ToList();
                    FamiliaSeleccionada = FamiliasPilaresEstructurales.ToList();
                    TipoSeleccionado = TiposPilaresEstructurales.ToList();

                    break;
                case "Rampas":
                    CategoriaSeleccionada = Rampas.ToList();
                    FamiliaSeleccionada = FamiliasRampas.ToList();
                    TipoSeleccionado = TiposRampas.ToList();

                    break;
                case "Suelos":
                    CategoriaSeleccionada = Suelos.ToList();
                    FamiliaSeleccionada = FamiliasSuelos.ToList();
                    TipoSeleccionado = TiposSuelos.ToList();

                    break;
                case "Techos":
                    CategoriaSeleccionada = Techos.ToList();
                    FamiliaSeleccionada = FamiliasTechos.ToList();
                    TipoSeleccionado = TiposTechos.ToList();

                    break;
                case "Tuberías":
                    CategoriaSeleccionada = Tuberias.ToList();
                    FamiliaSeleccionada = FamiliasTuberias.ToList();
                    TipoSeleccionado = TiposTuberias.ToList();

                    break;
                case "Tuberías flexibles":
                    CategoriaSeleccionada = TuberiasFlexibles.ToList();
                    FamiliaSeleccionada = FamiliasTuberiasFlexibles.ToList();
                    TipoSeleccionado = TiposTuberiasFlexibles.ToList();

                    break;
                case "Tubos":
                    CategoriaSeleccionada = Tubos.ToList();
                    FamiliaSeleccionada = FamiliasTubos.ToList();
                    TipoSeleccionado = TiposTubos.ToList();

                    break;
                case "Topografía":
                    CategoriaSeleccionada = Topografias.ToList();
                    FamiliaSeleccionada = FamiliasTopografias.ToList();
                    TipoSeleccionado = TiposTopografias.ToList();
                    break;
                case "":
                    CategoriaSeleccionada = TotalElementos.ToList();
                    FamiliaSeleccionada = FamiliaTotal.ToList();
                    TipoSeleccionado = TipoTotal.ToList();
                    break;

            }
  

        }

        void selecciona_familia(string Fami)
        {
            if (Fami == "") return;
            if (CategoriaSeleccionada is null) return;
            TipoSeleccionado = new List<RevitTipoBase>();
            var ListaAuxiliar = from l in CategoriaSeleccionada
                                where l.Familia == Fami
                                select new
                                {
                                    Id = l.Id,
                                    Familia = l.Familia,
                                    Tipo = l.Tipo,
                                    UniqueId = l.UniqueId
                                };
            int pos = 0, contarValidos = 0, encontrado = 0;
            string[] AuxTipo = new string[150];
            foreach (var ItemD in ListaAuxiliar)
            {
                encontrado = 0;
                for (int x = 0; x < pos; x++)
                {
                    if (AuxTipo[x] == ItemD.Tipo)
                        encontrado = 1;
                }

                if (encontrado == 0)
                {
                    AuxTipo[pos] = ItemD.Tipo;
                    pos++;
                    contarValidos++;
                }
            }

            //_NwData2 = new NwData[contarValidos];
            for (int x = 0; x < contarValidos; x++)
            {
                var Auxiliar = from l in CategoriaSeleccionada
                               where l.Tipo == AuxTipo[x]
                               select new
                               {
                                   Id = l.Id,
                                   Familia = l.Familia,
                                   Tipo = l.Tipo,
                                   UniqueId = l.UniqueId
                               };

                TipoSeleccionado.Add(new RevitTipoBase { Tipo = AuxTipo[x] });
                //_NwData2[x] = new NwData(AuxTipo[x], Auxiliar.Count());
            }
        }


         private void expandablePanel2_Click(object sender, EventArgs e)
        {

        }

        private void expandablePanel2_ExpandedChanged(object sender, DevComponents.DotNetBar.ExpandedChangeEventArgs e)
        {

            if (expandablePanel2.Expanded == true)
            {
                groupPanel9.Width = 664;
            }
            else
            {
                groupPanel9.Width = 40;
            }

            
        }

        private void expandablePanel3_ExpandedChanged(object sender, DevComponents.DotNetBar.ExpandedChangeEventArgs e)
        {
            if (expandablePanel3.Expanded == true)
            {
                //GroupPanel55.Height = 366;
                GroupPanel55.Height =(int)(this.Height*0.40);
                expandablePanel3.Height = (int)(this.Height * 0.40) - 10;

            }
            else
            {
                GroupPanel55.Height = 40;
            }

        }

        private void buttonItem86_Click(object sender, EventArgs e)
        {
            //webBrowser1.Navigate("https://www.google.es/");
            //solicitar_datosPresupuestos();
            //solicitar_datosArbPresupuestos();
            //ConnectWithRetry();
            textBox3.Text = cadenaObtenida;
            //solicitar_datosArbPresupuestos();
                timer11.Enabled = true;
       }

        private void expandablePanel4_ExpandedChanged(object sender, DevComponents.DotNetBar.ExpandedChangeEventArgs e)
        {
            if (expandablePanel4.Expanded == true)
            {
                groupPanel14.Width = 300;
            }
            else
            {
                groupPanel14.Width = 40;
            }

        }

        private void buttonItem73_Click(object sender, EventArgs e)
        {
            //autentificar_S10();
            System.Windows.Forms.MessageBox.Show(Token, "");

        }

        void verficarTablas()
        {
            ConexionBD baseC = new ConexionBD();
            if (!existeTabla("LPlano"))
                baseC.crearTablaPlano();
            if (!existeTabla("LMedicion"))
                baseC.crearTablaMedicion();
            if (!existeTabla("LAsociado"))
                baseC.crearTablaAsociado();
            if (!existeTabla("LEstructura"))
                baseC.crearTablaEstructura();
            if (!existeTabla("LCalculo"))
                baseC.crearTablaCalculo();
            if (!existeTabla("LCalculoDetalle"))
                baseC.crearTablaCalculoDetalle();
            if (!existeTabla("LConfCalculo"))
                baseC.crearTablaConfCalculo();
            if (!existeTabla("LConfCalculoDetalle"))
                baseC.crearTablaConfCalculoDetalle();
        }


        //LOAD ***********************************************************************************************************************************************************************
        public System.Data.DataTable tablaPresPl = new System.Data.DataTable();
        private void FrmPresupuestos_Load(object sender, EventArgs e)
        {
            verficarTablas();
            labelItem9.Text = EmailUsuario;
            labelItem8.Text = Datos.cmdData1.Application.ActiveUIDocument.Document.Title;
            NombrePl = labelItem8.Text;
            timer13.Enabled = true;

            Item_actual = "";
            SubPresupuesto_actual = "";
            Presupuesto_actual = "";
            double height = SystemParameters.FullPrimaryScreenHeight;
            double width = SystemParameters.FullPrimaryScreenWidth;
            double resolution = height * width;

            this.Height = (int) (height * 0.7);
            this.Width = (int ) (width * 0.7);

            metroShell1.Width = this.Width - 45;

            GroupPanel55.Height = (int)(this.Height * 0.40);
            expandablePanel3.Height = (int)(this.Height * 0.40) - 10;

            if (width <= 1200)
            {
                expandablePanel2.Expanded = false;
                expandablePanel4.Expanded = false;
            }

            ListaPresupuestos = new List<LPresupuestos>();
            textBox1.Text = "";
            var signalServer = @"http://200.48.100.203:5030/";
            connection = new HubConnection(signalServer);
            _s10ERPHubProxy = connection.CreateHubProxy("S10ERPHub");
            connection.Headers.Add("AuthType", "1"); //CLIENTE
            connection.Headers.Add("Token", Token);
            connection.Headers.Add("ModuleId", "11");
            ConnectWithRetry();
            TokenS10.Text = Token;
            tokenstk = Token;
            timer11.Enabled = true;

            chartControl1.Top = 0;
            chartControl1.Left = 0;
            chartControl1.Width = groupPanel16.Width/2;
            chartControl1.Height = groupPanel16.Height / 2;

            chartControl2.Top = 0;
            chartControl2.Left = groupPanel16.Width / 2;
            chartControl2.Width = groupPanel16.Width / 2;
            chartControl2.Height = groupPanel16.Height / 2;

            chartControl3.Top = groupPanel16.Height / 2;
            chartControl3.Left = 0;
            chartControl3.Width = groupPanel16.Width;
            chartControl3.Height = groupPanel16.Height / 2;

            autentificar();

            this.SGAsociados.DragOver += new System.Windows.Forms.DragEventHandler(this.SuperGridControlDragOver);
            this.SGAsociados.DragDrop += new System.Windows.Forms.DragEventHandler(this.SuperGridControlDragDrop);

            this.SGestructuraC.DragOver += new System.Windows.Forms.DragEventHandler(this.SuperGridControlDragOver);
            this.SGestructuraC.DragDrop += new System.Windows.Forms.DragEventHandler(this.SuperGridControlDragDrop);

            this.SGFormulas.DragOver += new System.Windows.Forms.DragEventHandler(this.SuperGridControlDragOver);
            this.SGFormulas.DragDrop += new System.Windows.Forms.DragEventHandler(this.SuperGridControlDragDrop);

            this.SGDetFormulas.DragOver += new System.Windows.Forms.DragEventHandler(this.SuperGridControlDragOver);
            this.SGDetFormulas.DragDrop += new System.Windows.Forms.DragEventHandler(this.SuperGridControlDragDrop);

        }








        public string cadenaObtenida;


        private  void ConnectWithRetry()
        {
            connection.Start().ContinueWith(task => {
                if (task.IsFaulted)
                {
                    //log.Error(string.Format("There was an error opening the connection:{0}", task.Exception.GetBaseException()));
                }
                else
                {
                    var idSignal = connection.ConnectionId;
                    textBox2.Text =  idSignal.ToString();
                    signalstk= idSignal.ToString();
                    #region S10ERPHub


                    _s10ERPHubProxy.On<string>("receiveS10ERPDataResult", (s1) =>
                    {
                        //Console.Clear();
                        //Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " S10ERP " + s1);
                        //try
                        //{
                        //textBox3.Text = s1.ToString();

                        //cadenaObtenida = s1.ToString();
                        if (!string.IsNullOrEmpty(s1))
                        {
                            ResponseExecuteS10ERPData responseSR = JsonConvert.DeserializeObject<ResponseExecuteS10ERPData>(s1);
                            if (responseSR != null)
                            {
                                switch (responseSR.Name)
                                {
                                    case "Items":
                                        List<LItems> ListaItems1 = JsonConvert.DeserializeObject<List<LItems>>(responseSR.Data);
                                        ListaItems.AddRange(ListaItems1);
                                        ; break;
                                    case "Arbol":
                                        List<LPresupuestos> ListaPresupuestos1 = JsonConvert.DeserializeObject<List<LPresupuestos>>(responseSR.Data);
                                        ListaPresupuestos.AddRange(ListaPresupuestos1);
                                        ; break;
                                    case "SubPresupuesto":
                                        List<LSubPresupuestos> ListaSubPresupusestos1 = JsonConvert.DeserializeObject<List<LSubPresupuestos>>(responseSR.Data);
                                        ListaSubPresupuestos.AddRange(ListaSubPresupusestos1);
                                        cadenaObtenida = s1.ToString();
                                        ; break;
                                    case "ItemsAPU":
                                        List<LItemsAPU> ListaItemsAPU1 = JsonConvert.DeserializeObject<List<LItemsAPU>>(responseSR.Data);
                                        ListaItemsAPU.AddRange(ListaItemsAPU1);
                                        cadenaObtenida = s1.ToString();
                                        ; break;
                                    case "ListarAsociado":
                                        List<LAsociado> ListaAsociados1 = JsonConvert.DeserializeObject<List<LAsociado>>(responseSR.Data);
                                        ListaAsociados.AddRange(ListaAsociados1);
                                        //cadenaObtenida = s1.ToString();
                                        ; break;
                                    case "ListarEstructura":
                                        List<LEstructura> Listaestructuras1 = JsonConvert.DeserializeObject<List<LEstructura>>(responseSR.Data);
                                        ListaEstructuras.AddRange(Listaestructuras1);
                                        cadenaObtenida = s1.ToString();
                                        ; break;
                                    case "ListarCalculo":
                                        List<LCalculo> ListaCalculos1 = JsonConvert.DeserializeObject<List<LCalculo>>(responseSR.Data);
                                        ListaCalculos.AddRange(ListaCalculos1);
                                        cadenaObtenida = s1.ToString();
                                        ; break;
                                    case "ListarCalculoDetalle":
                                        List<LCalculoDetalle> ListaCalculoDetalle1 = JsonConvert.DeserializeObject<List<LCalculoDetalle>>(responseSR.Data);
                                        ListaCalculoDetalle.AddRange(ListaCalculoDetalle1);
                                        cadenaObtenida = s1.ToString();
                                        ; break;
                                    case "ListarMedicion":
                                        List<LMedicion> ListaItemsMedicion1 = JsonConvert.DeserializeObject<List<LMedicion>>(responseSR.Data);
                                        ListaItemsMedicion.AddRange(ListaItemsMedicion1);
                                        cadenaObtenida = s1.ToString();
                                        ; break;
                                    case "Modificar":
                                        /*List<LMedicion> ListaItemsMedicion2 = JsonConvert.DeserializeObject<List<LMedicion>>(responseSR.Data);
                                        ListaItemsMedicion.AddRange(ListaItemsMedicion2);*/
                                        cadenaObtenida = s1.ToString();
                                        ; break;

                                    default: return;
                                }
                            }
                        }



                        //}
                        //catch { 

                        //}


                    });

                    /*_s10ERPHubProxy.On<string>("receiveTransportDataResult", (s1) =>
                    {
                        Console.Clear();
                        Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " S10ERPD " + s1);


                    });*/
                    #endregion

                }

            });


        }

        private void buttonItem45_Click(object sender, EventArgs e)
        {
            //JsonValue json = JsonValue.Parse(textBox3.Text);
            //textBox4.Text = json.ToString();
            //textBox4.Text = JObject.Parse(json).ToString();
            //FrmVerElementos fr1 = new FrmVerElementos();
            FrmBim360 fr1 = new FrmBim360();
            fr1.TokenAct=this.txtAccessToken.Text;
            fr1.SignalToken = this.textBox2.Text;
            fr1.labelItem2.Text = this.labelItem9.Text;
            fr1.labelItem1.Text = this.labelItem8.Text;
            fr1.TxtModelo.Text = Datos.cmdData1.Application.ActiveUIDocument.Document.PathName;
            fr1.Show();
            
            /*textBox3.Text = textBox3.Text.Replace("/", "");
            textBox3.Text = textBox3.Text.Replace("\\", "");
            textBox3.Text = textBox3.Text.Replace("\"[", "[");
            textBox3.Text = textBox3.Text.Replace("]\"", "]");
            textBox4.Text = JObject.Parse(textBox3.Text).ToString();*/
            
            
            
            //textBox4.Text = JsonValue.Parse(textBox3.Text).ToString();
            //MessageBox.Show(json["Data"][0]["Descripcion"].ToString(), "");
            //string jsonString = "{\"Items\": [{\"Name\": \"Apple\",\"Price\": 12.3},{\"Name\": \"Grape\",\"Price\": 3.21}],\"Date\": \"21/11/2010\"}";
            //string jsonString = textBox3.Text;
            //dynamic DynamicData = JsonConvert.DeserializeObject(jsonString);
            //textBox4.Text = JObject.Parse(DynamicData).ToString();
            //textBox4.Text = DynamicData.ToString();
        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            
            if (textBox1.Text != "" && ListaPresupuestos.Count!=0) {
                timer5.Enabled = false;


                System.Data.DataTable datosPl = new System.Data.DataTable();
                ConexionBD bdatos = new ConexionBD();
                datosPl = bdatos.LPlanos(labelItem8.Text);
                string codModelo = "";

                if (datosPl.Rows.Count == 0)
                {
                    ModeloCargado = "";
                    FrmBim360 fr1 = new FrmBim360();
                    if (System.Windows.Forms.MessageBox.Show("Este modelo no está cargado en la Nube u asociado a un modelo existente, Desea Vincularlo ahora?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        fr1.UrnCargado = "";
                        fr1.TokenAct = this.txtAccessToken.Text;
                        fr1.labelItem2.Text = this.labelItem9.Text;
                        fr1.labelItem1.Text = this.labelItem8.Text;
                        fr1.TxtModelo.Text = Datos.cmdData1.Application.ActiveUIDocument.Document.PathName;
                        fr1.ShowDialog();





                        if (fr1.UrnCargado != "") {
                            ModeloCargado = fr1.UrnCargado.Trim();
                            //VERIFICAR SI EL MODELO ESTA EN PROCESO DE CARGA
                            string urn = ViewerURN(ModeloCargado, "");
                            //bRowser.Load(urn);
                            webControl1.WebView.LoadUrl(urn);
                            EO.WebBrowser.Runtime.AddLicense(
                            "3a5rp7PD27FrmaQHEPGs4PP/6KFrqKax2r1GgaSxy5916u34GeCt7Pb26bSG" +
                            "prT6AO5p3Nfh5LRw4rrqHut659XO6Lto6u34GeCt7Pb26YxDs7P9FOKe5ff2" +
                            "6YxDdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW1yQKzbam2xvGvcau0weKv" +
                            "fLOz/RTinuX39vTjd4SOscufWbPw+g7kp+rp9unMmuX59hefi9jx+h3ks7Oz" +
                            "/RTinuX39hC9RoGkscufddjw/Rr2d4SOscufWZekscu7mtvosR/4qdzBs/DO" +
                            "Z7rsAxrsnpmkBxDxrODz/+iha6iywc2faLWRm8ufWZfAwAzrpeb7z7iJWZek" +
                            "sefuq9vpA/Ttn+ak9QzznrSmyNqxaaa2wd2wW5f3Bg3EseftAxDyeuvBs+I=");
                            textBoxX2.Text = urn;
                            timer12.Enabled = true;
                            //string datoError = (string)webControl1.WebView.EvalScript("RetornaErrorCode();");
                            //string datoError = (string)webControl1.WebView.EvalScript(@"CodError");
                            //System.Windows.Forms.MessageBox.Show(datoError.ToString(), "");

                        }

                    }
                    
                }
                else
                {
                    codModelo = datosPl.Rows[0]["CodPlano"].ToString().Trim();
                    ModeloCargado = datosPl.Rows[0]["UrnAddIn"].ToString().Trim();


                    tablaPresPl = new System.Data.DataTable();
                    var conbase = new ConexionBD();
                    conbase.Conexion();
                    tablaPresPl = conbase.LmedicionesPresVinculo(codModelo);
                    conbase.Conexion();



                    string urn = ViewerURN(ModeloCargado, "");
                    //bRowser.Load(urn);
                    webControl1.WebView.LoadUrl(urn);
                    EO.WebBrowser.Runtime.AddLicense(
                    "3a5rp7PD27FrmaQHEPGs4PP/6KFrqKax2r1GgaSxy5916u34GeCt7Pb26bSG" +
                    "prT6AO5p3Nfh5LRw4rrqHut659XO6Lto6u34GeCt7Pb26YxDs7P9FOKe5ff2" +
                    "6YxDdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW1yQKzbam2xvGvcau0weKv" +
                    "fLOz/RTinuX39vTjd4SOscufWbPw+g7kp+rp9unMmuX59hefi9jx+h3ks7Oz" +
                    "/RTinuX39hC9RoGkscufddjw/Rr2d4SOscufWZekscu7mtvosR/4qdzBs/DO" +
                    "Z7rsAxrsnpmkBxDxrODz/+iha6iywc2faLWRm8ufWZfAwAzrpeb7z7iJWZek" +
                    "sefuq9vpA/Ttn+ak9QzznrSmyNqxaaa2wd2wW5f3Bg3EseftAxDyeuvBs+I=");
                    textBoxX2.Text = urn;
                    timer12.Enabled = true;
                    /*TxtUrnAddIn.Text = datosPl.Rows[0]["UrnAddIn"].ToString().Trim();
                    TxtUrnWeb.Text = datosPl.Rows[0]["UrnWeb"].ToString().Trim();
                    String cadena = TxtUrnAddIn.Text;
                    //MessageBox.Show(cadena, "Ejemplo Mensaje Aceptar");
                    string urn = ViewerURN(cadena, "");
                    webControl1.WebView.LoadUrl(urn);*/
                }






                /*string cadenaaux = textBox3.Text;
                cadenaaux = cadenaaux.Replace("/", "");
                cadenaaux = cadenaaux.Replace("\\", "");
                cadenaaux = cadenaaux.Replace("\"[", "[");
                cadenaaux = cadenaaux.Replace("]\"", "]");

                textBox4.Text = JObject.Parse(cadenaaux).ToString();
                
                JObjetoPres = JObject.Parse(cadenaaux);

                //int length = ((JArray)JObjetoPres.Value["Companies"]).Count;
                ArregloPres = (JArray)JObjetoPres["Data"];
                ListaPresupuestos = new List<LPresupuestos>();
                //ListaPresupuestos.Add
                //responseDynamic.Value["Companies"])
                //CmbEmpresa.Items.Add(responseDynamic.Value.Companies[i].Name.ToString());
                //Tokens[i] = responseDynamic.Value.Companies[i].Token.ToString();
                for (int i = 0; i < ArregloPres.Count; i++)
                {
                    ListaPresupuestos.Add(new LPresupuestos { ERPCode = JObjetoPres.Data[i].ERPCode, CodPresupuesto = JObjetoPres.Data[i].CodPresupuesto, Nivel = JObjetoPres.Data[i].Nivel, Descripcion = JObjetoPres.Data[i].Descripcion, PhantomId = JObjetoPres.Data[i].PhantomId, PhantomParentId = JObjetoPres.Data[i].PhantomParentId, Fila = JObjetoPres.Data[i].Fila });
                }*/
                /*ListaPresupuestos = new List<LPresupuestos> {
                    new LPresupuestos { ERPCode = "1", CodPresupuesto = "Chai", Nivel = 1, Descripcion = " ", PhantomId = "", PhantomParentId = "", Fila = 1 }
                
                };*/

                Advtree4.Nodes.Clear();
                var node = new DevComponents.AdvTree.Node();
                node.Tag = ("");
                node.Text = ("TODOS");
                node.Image = ImageList1.Images[0];
                node.Cells.Add(new DevComponents.AdvTree.Cell());
                node.Cells.Add(new DevComponents.AdvTree.Cell());
                Advtree4.Nodes.Add(node);
                node.Expanded = false;

                try
                {
                    var PresupuestoNive1 = from l in ListaPresupuestos
                                           where l.Nivel == 1
                                           select new
                                           {
                                               ERPCode = l.ERPCode,
                                               CodPresupuesto = l.CodPresupuesto,
                                               Nivel = l.Nivel,
                                               Descripcion = l.Descripcion,
                                               PhantomId = l.PhantomId,
                                               PhantomParentId = l.PhantomParentId,
                                               Fila = l.Fila
                                           };
                    foreach (var Item in PresupuestoNive1)
                    {
                        //GridRow fila = new GridRow(Item.CodPresupuesto.ToString(), Item.Descripcion.ToString(), Item.Nivel.ToString(), Item.PhantomId.ToString(), Item.PhantomParentId.ToString());
                        GridRow fila = new GridRow(Item.CodPresupuesto.ToString(), Item.Descripcion.ToString(), Item.Nivel.ToString(), Item.PhantomId.ToString(), "0");


                        foreach (DataRow ItemCodPres in tablaPresPl.Rows)
                        {
                            if (Item.CodPresupuesto.ToString() == ItemCodPres["CodPresupuesto"].ToString().Trim())
                            {
                                node = new DevComponents.AdvTree.Node();
                                node.Tag = (Item.CodPresupuesto.ToString());
                                node.Text = (Item.Descripcion.ToString());
                                node.Image = ImageList1.Images[0];
                                node.Cells.Add(new DevComponents.AdvTree.Cell(Item.Nivel.ToString()));
                                node.Cells.Add(new DevComponents.AdvTree.Cell(Item.PhantomId.ToString()));
                                node.Cells.Add(new DevComponents.AdvTree.Cell(Item.PhantomParentId.ToString()));
                                Advtree4.Nodes.Add(node);
                                node.Expanded = false;
                            }
                            
                        }
                        /*Advtree4.Columns[1].Visible = false;
                        Advtree4.Columns[2].Visible = false;
                        Advtree4.Columns[3].Visible = false;*/
                        SGPresupuestos.PrimaryGrid.Rows.Add(fila);
                        cargar_hijos(Item.CodPresupuesto.ToString(), fila);
                        fila.Expanded = true;
                        fila.Cells[1].CellStyles.Default.Image = ImageList1.Images[1];
                        //ShellServices.GetFileImage(directory, false, false);
                    }

                }
                catch (System.Exception) { 
                
                }

                
                /*foreach (LPresupuestos aPart in ListaPresupuestos)
                {
                    //Console.WriteLine(aPart);
                    //MessageBox.Show(aPart.Descripcion.ToString(), "");
                    GridRow fila = new GridRow(aPart.CodPresupuesto.ToString(), aPart.Descripcion.ToString(), aPart.Nivel.ToString(), aPart.PhantomId.ToString());
                    SGPresupuestos.PrimaryGrid.Rows.Add(fila);
                }*/
                //MessageBox.Show(ArregloPres.Count.ToString(), "");
                }
        }






        private void cargar_hijos(string padre, GridRow filap) {

            var PresupuestoOtro = from l in ListaPresupuestos
                                   where l.PhantomParentId == padre
                                   select new
                                   {
                                       CodPresupuesto = l.CodPresupuesto,
                                       Descripcion = l.Descripcion,
                                       Nivel = l.Nivel,
                                       PhantomId = l.PhantomId,
                                       PhantomParentId = l.PhantomParentId
                                   };

            if (PresupuestoOtro.Count() != 0)
            {
                filap.Cells[1].CellStyles.Default.Image = ImageList1.Images[1];
            }
            else {
                filap.Cells[1].CellStyles.Default.Image = ImageList1.Images[16];
            }

            foreach (var Item in PresupuestoOtro)
            {
                GridRow fila = new GridRow(Item.CodPresupuesto.ToString(), Item.Descripcion.ToString(), Item.Nivel.ToString(), Item.PhantomId.ToString(), Item.PhantomParentId.ToString());
                filap.Rows.Add(fila);

                foreach (DataRow ItemCodPres in tablaPresPl.Rows)
                {
                    if (Item.CodPresupuesto.ToString() == ItemCodPres["CodPresupuesto"].ToString().Trim())
                    {
                        var node = new DevComponents.AdvTree.Node();
                        node.Tag = (Item.CodPresupuesto.ToString());
                        node.Text = (Item.Descripcion.ToString());
                        node.Image = ImageList1.Images[0];
                        node.Cells.Add(new DevComponents.AdvTree.Cell(Item.Nivel.ToString()));
                        node.Cells.Add(new DevComponents.AdvTree.Cell(Item.PhantomId.ToString()));
                        node.Cells.Add(new DevComponents.AdvTree.Cell(Item.PhantomParentId.ToString()));
                        Advtree4.Nodes.Add(node);
                        node.Expanded = false;
                    }

                }

                cargar_hijos(Item.CodPresupuesto.ToString(), fila);
                fila.Expanded = true;
            }
                
        }


        private void Limpiar_Partidas() {
            do
            {
                SGPresupC.PrimaryGrid.DeleteAll();
                SGPresupC.PrimaryGrid.PurgeDeletedRows();
            }
            while (SGPresupC.PrimaryGrid.Rows.Count != 0);
            GridRow fila = new GridRow("", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "");
            SGPresupC.PrimaryGrid.Rows.Add(fila);
            do
            {
                SGMediciones.PrimaryGrid.DeleteAll();
                SGMediciones.PrimaryGrid.PurgeDeletedRows();
            }
            while (SGMediciones.PrimaryGrid.Rows.Count != 0);
            //GridRow filaauxP = new GridRow("", "", "", "", "", "", "", "", "", "", "", "", "");
            //filaauxP.ReadOnly = true;
            //SGMediciones.PrimaryGrid.Rows.Add(filaauxP);

            SGApus.PrimaryGrid.DeleteAll();
            SGApus.PrimaryGrid.PurgeDeletedRows();
            fila = new GridRow("", "", "", "", "", "", "", "");
            SGApus.PrimaryGrid.Rows.Add(fila);

            SGestructuraC.PrimaryGrid.DeleteAll();
            SGestructuraC.PrimaryGrid.PurgeDeletedRows();
            fila = new GridRow("", "", "", "", "");
            SGestructuraC.PrimaryGrid.Rows.Add(fila);


            SGAsociados.PrimaryGrid.DeleteAll();
            SGAsociados.PrimaryGrid.PurgeDeletedRows();
            fila = new GridRow("", "", "", "", "", "", "", "");
            SGAsociados.PrimaryGrid.Rows.Add(fila);


            fila = (GridRow)SGFormulas.PrimaryGrid.Rows[0];
            for (int X = 0; X < 6; X++)
                fila.Cells[X].Value = "";

            SGDetFormulas.PrimaryGrid.DeleteAll();
            SGDetFormulas.PrimaryGrid.PurgeDeletedRows();
            fila = new GridRow("", "", "", "", "");
            SGDetFormulas.PrimaryGrid.Rows.Add(fila);

            SuperTabItem2.Text = "APU PARTIDA ";
            SuperTabItem5.Text = "METRADO ";

            expandablePanel3.TitleText = "SIN SELECCION";

            GridRow filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[0];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[1];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[2];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[3];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[4];
            filaaz.Cells[2].Value = "0.00";



        }

        private void Limpiar_Partidas1()
        {
       
            do
            {
                SGMediciones.PrimaryGrid.DeleteAll();
                SGMediciones.PrimaryGrid.PurgeDeletedRows();
            }
            while (SGMediciones.PrimaryGrid.Rows.Count != 0);
            //GridRow filaauxP = new GridRow("", "", "", "", "", "", "", "", "", "", "", "", "");
            //filaauxP.ReadOnly = true;
            //SGMediciones.PrimaryGrid.Rows.Add(filaauxP);

            SGApus.PrimaryGrid.DeleteAll();
            SGApus.PrimaryGrid.PurgeDeletedRows();
            GridRow fila = new GridRow("", "", "", "", "", "", "", "");
            SGApus.PrimaryGrid.Rows.Add(fila);

            SGestructuraC.PrimaryGrid.DeleteAll();
            SGestructuraC.PrimaryGrid.PurgeDeletedRows();
            fila = new GridRow("", "", "", "", "");
            SGestructuraC.PrimaryGrid.Rows.Add(fila);


            SGAsociados.PrimaryGrid.DeleteAll();
            SGAsociados.PrimaryGrid.PurgeDeletedRows();
            fila = new GridRow("", "", "", "", "", "", "", "");
            SGAsociados.PrimaryGrid.Rows.Add(fila);


            fila = (GridRow)SGFormulas.PrimaryGrid.Rows[0];
            for (int X = 0; X < 6; X++)
                fila.Cells[X].Value = "";

            SGDetFormulas.PrimaryGrid.DeleteAll();
            SGDetFormulas.PrimaryGrid.PurgeDeletedRows();
            fila = new GridRow("", "", "", "", "");
            SGDetFormulas.PrimaryGrid.Rows.Add(fila);

            SuperTabItem2.Text = "APU PARTIDA ";
            SuperTabItem5.Text = "METRADO ";

            expandablePanel3.TitleText = "SIN SELECCION";

            GridRow filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[0];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[1];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[2];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[3];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[4];
            filaaz.Cells[2].Value = "0.00";


            AdvTree7.Nodes.Clear();

        }


        private void SGPresupuestos_SelectionChanged(object sender, GridEventArgs e)
        {
            filaanterior = null;
            expandablePanel2.TitleText = "SIN SELECCION";
            Limpiar_Partidas();
            Item_actual = "";
            SubPresupuesto_actual = "";
            Presupuesto_actual = "";
            GridRow Filasel = (GridRow)SGPresupuestos.PrimaryGrid.ActiveRow;
            if (Filasel is null) return;
            if (Filasel.Rows.Count != 0) {
                expandablePanel2.TitleText = "SELECCIONADO: " + Filasel.Cells[1].Value.ToString().Trim();
                return;
            }
            

            if (Filasel.Cells[2].Value.ToString().Trim() == "Sub")
            {
                textBox1.Text = "";
                solicitar_datosItems();
                SubPresupuesto_actual = Filasel.Cells[0].Value.ToString();
                GridRow Filapadre = (GridRow)Filasel.Parent;
                Presupuesto_actual = Filapadre.Cells[0].Value.ToString();

                expandablePanel2.TitleText=" " + Filapadre.Cells[1].Value.ToString() + " - " + Filasel.Cells[1].Value.ToString();

                //cadenaObtenida = "";
                timer7.Enabled = true;
            }
            else {
                textBox1.Text = "";
                solicitar_datosSubPresupuestos();
                Presupuesto_actual = Filasel.Cells[0].Value.ToString();
                expandablePanel2.TitleText = "PRESUPUESTO: " + Filasel.Cells[1].Value.ToString();
                //cadenaObtenida = "";
                timer6.Enabled = true;
            }

        }

        private void timer6_Tick(object sender, EventArgs e)
        {
            //textBox3.Text = cadenaObtenida;
            if (textBox1.Text != "" && ListaSubPresupuestos.Count !=0)
            {
                timer6.Enabled = false;
                GridRow Filasel = (GridRow)SGPresupuestos.PrimaryGrid.ActiveRow;
                
                /*string cadenaaux = textBox3.Text;
                cadenaaux = cadenaaux.Replace("/", "");
                cadenaaux = cadenaaux.Replace("\\", "");
                cadenaaux = cadenaaux.Replace("\"[", "[");
                cadenaaux = cadenaaux.Replace("]\"", "]");
                textBox4.Text = JObject.Parse(cadenaaux).ToString();
                JObjetoPres = JObject.Parse(cadenaaux);
                ArregloPres = (JArray)JObjetoPres["Data"];
                ListaSubPresupuestos = new List<LSubPresupuestos>();
                for (int i = 0; i < ArregloPres.Count; i++)
                {
                    ListaSubPresupuestos.Add(new LSubPresupuestos { CodSubpresupuesto = JObjetoPres.Data[i].CodSubpresupuesto, Descripcion = JObjetoPres.Data[i].Descripcion });
                }*/
                foreach (var Item in ListaSubPresupuestos)
                {
                    GridRow fila = new GridRow(Item.CodSubpresupuesto.ToString(), Item.Descripcion.ToString(),"Sub","","");
                    Filasel.Rows.Add(fila);
                    Filasel.Expanded = true;
                    fila.Cells[1].CellStyles.Default.Image = ImageList1.Images[2];
                }
            }
        }

        private void timer7_Tick(object sender, EventArgs e)
        {
            //textBox3.Text = cadenaObtenida;
            if (textBox1.Text != "" && ListaItems.Count != 0)
            {
                timer7.Enabled = false;
                do
                {
                    SGPresupC.PrimaryGrid.DeleteAll();
                    SGPresupC.PrimaryGrid.PurgeDeletedRows();
                }
                while (SGPresupC.PrimaryGrid.Rows.Count != 0);


                var ItemsNive1 = ListaItems.Where(X => X.Nivel == 1).ToList();


               

                foreach (var Item in ItemsNive1)
                {
                    //GridRow fila = new GridRow(Item.CodPresupuesto.ToString(), Item.Descripcion.ToString(), Item.Nivel.ToString(), Item.PhantomId.ToString(), Item.PhantomParentId.ToString());
                    string unid = "";
                    if (Item.Unidad is null) unid = ""; else unid = Item.Unidad;
                    //GridRow fila = new GridRow(Item.CodPresupuesto.ToString(), Item.Orden.ToString(), Item.Descripcion.ToString(), unid, "", "", "", "", "", "", "", "", "", "", "Tipo", Item.PhantomParentId.ToString(), Item.Nivel.ToString(),"","","","","");

                    string ErpCodeAux = "";
                    if (Item.ERPCode is null) ErpCodeAux = ""; else ErpCodeAux = Item.ERPCode;
                    string ItemAux = "";
                    if (Item.Item is null) ItemAux = ""; else ItemAux = Item.Item;
                    string CodPresupuestoAux = "";
                    if (Item.CodPresupuesto is null) CodPresupuestoAux = ""; else CodPresupuestoAux = Item.CodPresupuesto;
                    string CodSubPresupuestoAux = "";
                    if (Item.CodSubpresupuesto is null) CodSubPresupuestoAux = ""; else CodSubPresupuestoAux = Item.CodSubpresupuesto;
                    string PhantomParentIdAux = "";
                    if (Item.PhantomParentId is null) PhantomParentIdAux = ""; else PhantomParentIdAux = Item.PhantomParentId;

                    string Metrado = "";
                    if (Item.Metrado is null) Metrado = ""; else Metrado = Convert.ToDouble(Item.Metrado.ToString()).ToString("N2");
                    string Precio = "";
                    if (Item.Precio1 is null) Precio = ""; else Precio = Convert.ToDouble(Item.Precio1.ToString()).ToString("N2");

                    double Metradodouble = 0.0, Preciodouble = 0.0;
                    if (Metrado != "")  Metradodouble = Convert.ToDouble(Metrado); else Metradodouble = 0;
                    if (Precio != "") Preciodouble = Convert.ToDouble(Precio); else Preciodouble = 0;
                    double Parcial = Metradodouble * Preciodouble;


                    GridRow fila = new GridRow(Item.CodPresupuesto.ToString(), Item.Orden.ToString(), Item.Descripcion.ToString(), unid, Metrado, Precio, Parcial.ToString("N2"), "", "", "", "", "", "", "", "Tipo", "0", Item.Nivel.ToString(), ItemAux, CodPresupuestoAux, CodSubPresupuestoAux, PhantomParentIdAux, "");
                    SGPresupC.PrimaryGrid.Rows.Add(fila);
                    cargar_hijos_items(Item.Orden.ToString(), fila);
                    fila.Expanded = true;

                    if (unid == "") {
                        var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(202, 223, 218), 90);
                        fila.CellStyles.Default.Background = Background;
                    }
                    //fila.Cells[1].CellStyles.Default.Image = ImageList1.Images[1];
                    //ShellServices.GetFileImage(directory, false, false);
                }




                double total = 0.00;
                foreach (GridRow Itfila in SGPresupC.PrimaryGrid.Rows)
                {
                    if (Itfila.Rows.Count() != 0) { 
                            total = sumar_hijos(Itfila);
                            Itfila.Cells[6].Value = total.ToString("N2");
                        }
                }






            }

        }



        private double sumar_hijos(GridRow filap)
        {
            double total = 0.00;
            double total1 = 0.00;
            foreach (GridRow Itfila in filap.Rows)
            {
                if (Itfila.Rows.Count() != 0) {
                    total1 = sumar_hijos(Itfila);
                    Itfila.Cells[6].Value = total1.ToString("N2");
                    total = total + total1;
                }
                else
                    total = total + Convert.ToDouble(Itfila.Cells[6].Value.ToString());
            }
            return total;
        }



        private void cargar_hijos_items(string padre, GridRow filap)
        {

            var ItemsNive2 = ListaItems.Where(X => X.PhantomParentId == padre).ToList();

            foreach (var Item in ItemsNive2)
            {
                //GridRow fila = new GridRow(Item.CodPresupuesto.ToString(), Item.Descripcion.ToString(), Item.Nivel.ToString(), Item.PhantomId.ToString(), Item.PhantomParentId.ToString());
                string unid = "";
                if (Item.Unidad is null) unid = ""; else unid = Item.Unidad;

                string ErpCodeAux = "";
                if (Item.ERPCode is null) ErpCodeAux = ""; else ErpCodeAux = Item.ERPCode;
                string ItemAux = "";
                if (Item.Item is null) ItemAux = ""; else ItemAux = Item.Item;
                string CodPresupuestoAux = "";
                if (Item.CodPresupuesto is null) CodPresupuestoAux = ""; else CodPresupuestoAux = Item.CodPresupuesto;
                string CodSubPresupuestoAux = "";
                if (Item.CodSubpresupuesto is null) CodSubPresupuestoAux = ""; else CodSubPresupuestoAux = Item.CodSubpresupuesto;
                string PhantomParentIdAux = "";
                if (Item.PhantomParentId is null) PhantomParentIdAux = ""; else PhantomParentIdAux = Item.PhantomParentId;

                string Metrado = "";
                if (Item.Metrado is null) Metrado = ""; else Metrado = Convert.ToDouble(Item.Metrado.ToString()).ToString("N2");
                string Precio = "";
                if (Item.Precio1 is null) Precio = ""; else Precio = Convert.ToDouble(Item.Precio1.ToString()).ToString("N2");
                double Metradodouble = 0.0, Preciodouble = 0.0;
                if (Metrado != "") Metradodouble = Convert.ToDouble(Metrado); else Metradodouble = 0;
                if (Precio != "") Preciodouble = Convert.ToDouble(Precio); else Preciodouble = 0;
                double Parcial = Metradodouble * Preciodouble;


                //GridRow fila = new GridRow(Item.CodPresupuesto.ToString(), Item.Orden.ToString(), Item.Descripcion.ToString(), unid, "", "", "", "", "", "", "", "", "", "", "Tipo", Item.PhantomParentId.ToString(), Item.Nivel.ToString(),"","","","","");
                GridRow fila = new GridRow(Item.CodPresupuesto.ToString(), Item.Orden.ToString(), Item.Descripcion.ToString(), unid, Metrado, Precio, Parcial.ToString("N2"), "", "", "", "", "", "", "", "Tipo", "0", Item.Nivel.ToString(), ItemAux, CodPresupuestoAux, CodSubPresupuestoAux, PhantomParentIdAux, "");
                filap.Rows.Add(fila);
                if (Item.Orden.ToString()!="")
                cargar_hijos_items(Item.Orden.ToString(), fila);
                fila.Expanded = true;

                if (unid == "")
                {
                    var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(202, 223, 218), 90);
                    fila.CellStyles.Default.Background = Background;
                }

                //fila.Cells[1].CellStyles.Default.Image = ImageList1.Images[1];
                //ShellServices.GetFileImage(directory, false, false);
            }


            

        }

        private void timer8_Tick(object sender, EventArgs e)
        {
            timer8.Enabled = false;
        }

        private void GrupoContenido_Click(object sender, EventArgs e)
        {

        }

        private void SuperTabControlPanel24_Click(object sender, EventArgs e)
        {
            


            //if (SuperTab9.SelectedTabIndex == 1)


        }

        private void SuperTab9_TabMoved(object sender, DevComponents.DotNetBar.SuperTabStripTabMovedEventArgs e)
        {

            
        }

        private void SuperTab9_SelectedTabChanged(object sender, DevComponents.DotNetBar.SuperTabStripSelectedTabChangedEventArgs e)
        {
            //MessageBox.Show(SuperTab9.SelectedTabIndex.ToString(), "");
            if (SuperTab9.SelectedTabIndex==3 && m_tvObjs.Nodes.Count ==0)
            {
                //MessageBox.Show("", "");
                this.Refresh();
                timer9.Enabled = true;
                // Get settings of current document

            }

            if (SuperTab9.SelectedTabIndex == 2)
            {
                chartControl1.Top = 0;
                chartControl1.Left = 0;
                chartControl1.Width = groupPanel16.Width / 2;
                chartControl1.Height = groupPanel16.Height / 2;

                chartControl2.Top = 0;
                chartControl2.Left = groupPanel16.Width / 2;
                chartControl2.Width = groupPanel16.Width / 2;
                chartControl2.Height = groupPanel16.Height / 2;

                chartControl3.Top = groupPanel16.Height / 2;
                chartControl3.Left = 0;
                chartControl3.Width = groupPanel16.Width;
                chartControl3.Height = groupPanel16.Height / 2;


            }

            if (SuperTab9.SelectedTabIndex == 1)
            {


            }



        }


        void CommonInit(IEnumerable<SnoopableObjectWrapper> objs)
        {
            m_tvObjs.BeginUpdate();

            AddObjectsToTree(objs);

            // if the tree isn't well populated, expand it and select the first item
            // so its not a pain for the user when there is only one relevant item in the tree
            if (m_tvObjs.Nodes.Count == 1)
            {
                m_tvObjs.Nodes[0].Expand();
                if (m_tvObjs.Nodes[0].Nodes.Count == 0)
                    m_tvObjs.SelectedNode = m_tvObjs.Nodes[0];
                else
                    m_tvObjs.SelectedNode = m_tvObjs.Nodes[0].Nodes[0];
            }

            m_tvObjs.EndUpdate();
        }







        #region ListasElementos

        public class CategoriasRvt
        {
            public string Nombre { get; set; }
            public string Tabla { get; set; }
        }

        public class RevitParametroBase
        {
            public string Nombre { get; set; }
        }


        private List<CategoriasRvt> ListaCategoriasRvt = null;

        public class RevitTipoBase
        {
            public string Tipo { get; set; }
        }
        public class RevitFamiliaBase
        {
            public string Familia { get; set; }
        }


        public class RevitElementoBase
        {
            public string Id { get; set; }
            public string Categoria { get; set; }
            public string Familia { get; set; }
            public string Tipo { get; set; }           
            public string UniqueId { get; set; }
        }


        public class RevitElementoBaseCampo
        {
            public string Id { get; set; }
            public string Categoria { get; set; }
            public string Familia { get; set; }
            public string Tipo { get; set; }
            public string UniqueId { get; set; }
            public string Campo { get; set; }
            public string Valor { get; set; }

        }

        private List<RevitParametroBase> ParametrosCompartidos = null;

        private List<RevitElementoBase> ArmazonesEstructurales = null;
        private List<RevitElementoBase> BandejasdeCables = null;
        private List<RevitElementoBase> Barandillas = null;
        private List<RevitElementoBase> CimentacionesEstructurales = null;
        private List<RevitElementoBase> Conductos = null;
        private List<RevitElementoBase> ConductosFlexibles = null;
        private List<RevitElementoBase> ConexionesEstructurales = null;
        private List<RevitElementoBase> Cubiertas = null;
        private List<RevitElementoBase> Emplazamientos = null;
        private List<RevitElementoBase> Escaleras = null;
        private List<RevitElementoBase> ModelosGenericos = null;
        private List<RevitElementoBase> Muros = null;
        private List<RevitElementoBase> PilaresEstructurales = null;
        private List<RevitElementoBase> Rampas = null;
        private List<RevitElementoBase> Suelos = null;
        private List<RevitElementoBase> Techos = null;
        private List<RevitElementoBase> Tuberias = null;
        private List<RevitElementoBase> TuberiasFlexibles = null;
        private List<RevitElementoBase> Tubos = null;
        private List<RevitElementoBase> ArmadurasEstructuales = null;
        private List<RevitElementoBase> AparatosSanitarios = null;
        private List<RevitElementoBase> Topografias = null;

        List<RevitElementoBase> TotalElementos = null;
        
        List<RevitElementoBase> ElementosFiltro = null;
        List<RevitElementoBaseCampo> ElementosFiltroCampo = null;


        private List<RevitElementoBase> CategoriaSeleccionada = null;


        private List<RevitTipoBase> TiposArmazonesEstructurales = null;
        private List<RevitTipoBase> TiposBandejasdeCables = null;
        private List<RevitTipoBase> TiposBarandillas = null;
        private List<RevitTipoBase> TiposCimentacionesEstructurales = null;
        private List<RevitTipoBase> TiposConductos = null;
        private List<RevitTipoBase> TiposConductosFlexibles = null;
        private List<RevitTipoBase> TiposConexionesEstructurales = null;
        private List<RevitTipoBase> TiposCubiertas = null;
        private List<RevitTipoBase> TiposEmplazamientos = null;
        private List<RevitTipoBase> TiposEscaleras = null;
        private List<RevitTipoBase> TiposModelosGenericos = null;
        private List<RevitTipoBase> TiposMuros = null;
        private List<RevitTipoBase> TiposPilaresEstructurales = null;
        private List<RevitTipoBase> TiposRampas = null;
        private List<RevitTipoBase> TiposSuelos = null;
        private List<RevitTipoBase> TiposTechos = null;
        private List<RevitTipoBase> TiposTuberias = null;
        private List<RevitTipoBase> TiposTuberiasFlexibles = null;
        private List<RevitTipoBase> TiposTubos = null;
        private List<RevitTipoBase> TiposArmadurasEstructuales = null;
        private List<RevitTipoBase> TiposAparatosSanitarios = null;
        private List<RevitTipoBase> TiposTopografias = null;

        private List<RevitTipoBase> TipoSeleccionado = null;
        private List<RevitTipoBase> TipoTotal = null;

        private List<RevitFamiliaBase> FamiliasArmazonesEstructurales = null;
        private List<RevitFamiliaBase> FamiliasBandejasdeCables = null;
        private List<RevitFamiliaBase> FamiliasBarandillas = null;
        private List<RevitFamiliaBase> FamiliasCimentacionesEstructurales = null;
        private List<RevitFamiliaBase> FamiliasConductos = null;
        private List<RevitFamiliaBase> FamiliasConductosFlexibles = null;
        private List<RevitFamiliaBase> FamiliasConexionesEstructurales = null;
        private List<RevitFamiliaBase> FamiliasCubiertas = null;
        private List<RevitFamiliaBase> FamiliasEmplazamientos = null;
        private List<RevitFamiliaBase> FamiliasEscaleras = null;
        private List<RevitFamiliaBase> FamiliasModelosGenericos = null;
        private List<RevitFamiliaBase> FamiliasMuros = null;
        private List<RevitFamiliaBase> FamiliasPilaresEstructurales = null;
        private List<RevitFamiliaBase> FamiliasRampas = null;
        private List<RevitFamiliaBase> FamiliasSuelos = null;
        private List<RevitFamiliaBase> FamiliasTechos = null;
        private List<RevitFamiliaBase> FamiliasTuberias = null;
        private List<RevitFamiliaBase> FamiliasTuberiasFlexibles = null;
        private List<RevitFamiliaBase> FamiliasTubos = null;
        private List<RevitFamiliaBase> FamiliasArmadurasEstructuales = null;
        private List<RevitFamiliaBase> FamiliasAparatosSanitarios = null;
        private List<RevitFamiliaBase> FamiliasTopografias = null;

        private List<RevitFamiliaBase> FamiliaSeleccionada = null;
        private List<RevitFamiliaBase> FamiliaTotal = null;


        #endregion




        void AddObjectsToTree(IEnumerable<SnoopableObjectWrapper> snoopableObjects)
        {
            
            ArmazonesEstructurales = new List<RevitElementoBase>();
            ArmadurasEstructuales= new List<RevitElementoBase>();
            BandejasdeCables = new List<RevitElementoBase>();
            Barandillas= new List<RevitElementoBase>();
            CimentacionesEstructurales = new List<RevitElementoBase>();
            Conductos= new List<RevitElementoBase>();
            ConductosFlexibles= new List<RevitElementoBase>();
            ConexionesEstructurales = new List<RevitElementoBase>();
            Cubiertas= new List<RevitElementoBase>();
            Emplazamientos = new List<RevitElementoBase>();
            Escaleras= new List<RevitElementoBase>();
            ModelosGenericos = new List<RevitElementoBase>();
            AparatosSanitarios = new List<RevitElementoBase>();
            Muros= new List<RevitElementoBase>();
            PilaresEstructurales = new List<RevitElementoBase>();
            Rampas= new List<RevitElementoBase>();
            Suelos= new List<RevitElementoBase>();
            Techos= new List<RevitElementoBase>();
            Tuberias= new List<RevitElementoBase>();
            TuberiasFlexibles= new List<RevitElementoBase>();
            Tubos= new List<RevitElementoBase>();
            Topografias = new List<RevitElementoBase>();

            FamiliasArmazonesEstructurales = new List<RevitFamiliaBase>();
            FamiliasArmadurasEstructuales = new List<RevitFamiliaBase>();
            FamiliasBandejasdeCables = new List<RevitFamiliaBase>();
            FamiliasBarandillas = new List<RevitFamiliaBase>();
            FamiliasCimentacionesEstructurales = new List<RevitFamiliaBase>();
            FamiliasConductos = new List<RevitFamiliaBase>();
            FamiliasConductosFlexibles = new List<RevitFamiliaBase>();
            FamiliasConexionesEstructurales = new List<RevitFamiliaBase>();
            FamiliasCubiertas = new List<RevitFamiliaBase>();
            FamiliasEmplazamientos = new List<RevitFamiliaBase>();
            FamiliasEscaleras = new List<RevitFamiliaBase>();
            FamiliasModelosGenericos = new List<RevitFamiliaBase>();
            FamiliasAparatosSanitarios = new List<RevitFamiliaBase>();
            FamiliasMuros = new List<RevitFamiliaBase>();
            FamiliasPilaresEstructurales = new List<RevitFamiliaBase>();
            FamiliasRampas = new List<RevitFamiliaBase>();
            FamiliasSuelos = new List<RevitFamiliaBase>();
            FamiliasTechos = new List<RevitFamiliaBase>();
            FamiliasTuberias = new List<RevitFamiliaBase>();
            FamiliasTuberiasFlexibles = new List<RevitFamiliaBase>();
            FamiliasTubos = new List<RevitFamiliaBase>();
            FamiliasTopografias = new List<RevitFamiliaBase>();

            TiposArmazonesEstructurales = new List<RevitTipoBase>();
            TiposArmadurasEstructuales = new List<RevitTipoBase>();
            TiposBandejasdeCables = new List<RevitTipoBase>();
            TiposBarandillas = new List<RevitTipoBase>();
            TiposCimentacionesEstructurales = new List<RevitTipoBase>();
            TiposConductos = new List<RevitTipoBase>();
            TiposConductosFlexibles = new List<RevitTipoBase>();
            TiposConexionesEstructurales = new List<RevitTipoBase>();
            TiposCubiertas = new List<RevitTipoBase>();
            TiposEmplazamientos = new List<RevitTipoBase>();
            TiposEscaleras = new List<RevitTipoBase>();
            TiposModelosGenericos = new List<RevitTipoBase>();
            TiposAparatosSanitarios = new List<RevitTipoBase>();
            TiposMuros = new List<RevitTipoBase>();
            TiposPilaresEstructurales = new List<RevitTipoBase>();
            TiposRampas = new List<RevitTipoBase>();
            TiposSuelos = new List<RevitTipoBase>();
            TiposTechos = new List<RevitTipoBase>();
            TiposTuberias = new List<RevitTipoBase>();
            TiposTuberiasFlexibles = new List<RevitTipoBase>();
            TiposTubos = new List<RevitTipoBase>();
            TiposTopografias = new List<RevitTipoBase>();

            ParametrosCompartidos = new List<RevitParametroBase>();


            //PieChart pieChart = (PieChart)chartControl1.ChartPanel.ChartContainers[0];
            m_tvObjs.Sorted = true;
            treeView2.BeginUpdate();
            treeView2.Sorted = true;
            // initialize the tree control
            foreach (var snoopableObject in snoopableObjects)
            {
                /*if (snoopableObject.GetUnderlyingType().Name == "Family") {
                    MessageBox.Show("Family " + snoopableObject.Title, "");
                }*/
                if (snoopableObject.GetUnderlyingType().Name == "FamilyInstance") {
                    //FamilyInstance dato= (FamilyInstance) snoopableObject;
                    //var obj = (FamilyInstance) snoopableObject.Object;
                    //var elem = snoopableObject.Object as Element;
                    //elem.Category.Name;
                    //MessageBox.Show("Family " + snoopableObject.Title, "");
                    //TreeNode Nodop = new TreeNode(/*snoopableObject.Title + " TAG: " +*/ snoopableObject.Object.ToString());
                    //treeView1.Nodes.Add(Nodop);
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            TreeNode Nodop = new TreeNode(elem.Category.Name + " Id =" + elem.Id);
                            treeView2.Nodes.Add(Nodop);
                            //elem.GetType().GetProperty("FamilyName");
                            FamilySymbol elemType = (FamilySymbol)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Armazón estructural":
                                    ArmazonesEstructurales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria= "Armazón estructural", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    
                                    var Comprueba = from l in FamiliasArmazonesEstructurales
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                   {
                                                       Familia = l.Familia,
                                                   };
                                    if (Comprueba.Count()==0) 
                                        FamiliasArmazonesEstructurales.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposArmazonesEstructurales
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                    {
                                                        Tipo = l.Tipo,
                                                    };
                                    if (Comprueba1.Count() == 0)
                                        TiposArmazonesEstructurales.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });
                                    
                                    //chartControl1.BeginUpdate();
                                    //pieChart.ChartSeries[0].SeriesPoints[0].ValueY.SetValue(ArmazonesEstructurales.Count(), 0);
                                    //pieChart.ChartSeries[0].SeriesPoints.Clear();
                                    //AddSeries(pieChart);
                                    //pieChart.
                                    //pieChart.ChartSeries.
                                    //chartControl1.EndUpdate();
                                    ; break;
                                case "Cimentación estructural":
                                    CimentacionesEstructurales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Cimentación estructural", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    Comprueba = from l in FamiliasCimentacionesEstructurales
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasCimentacionesEstructurales.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });

                                    Comprueba1 = from l in TiposCimentacionesEstructurales
                                                 where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposCimentacionesEstructurales.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;
                                case "Conexiones estructurales":
                                    ConexionesEstructurales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Conexiones estructurales", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });

                                    Comprueba = from l in FamiliasConexionesEstructurales
                                                where l.Familia == elemType.FamilyName.ToString()
                                                select new
                                                {
                                                    Familia = l.Familia,
                                                };
                                    if (Comprueba.Count() == 0)
                                        FamiliasConexionesEstructurales.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    Comprueba1 = from l in TiposConexionesEstructurales
                                                 where l.Tipo == elem.Name.ToString()
                                                 select new
                                                 {
                                                     Tipo = l.Tipo,
                                                 };
                                    if (Comprueba1.Count() == 0)
                                        TiposConexionesEstructurales.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;
                                case "Emplazamiento":
                                    Emplazamientos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Emplazamiento", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    Comprueba = from l in FamiliasEmplazamientos
                                                where l.Familia == elemType.FamilyName.ToString()
                                                select new
                                                {
                                                    Familia = l.Familia,
                                                };
                                    if (Comprueba.Count() == 0)
                                        FamiliasEmplazamientos.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    Comprueba1 = from l in TiposEmplazamientos
                                                 where l.Tipo == elem.Name.ToString()
                                                 select new
                                                 {
                                                     Tipo = l.Tipo,
                                                 };
                                    if (Comprueba1.Count() == 0)
                                        TiposEmplazamientos.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;
                                case "Escaleras":
                                    Escaleras.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Escaleras", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    Comprueba = from l in FamiliasEscaleras
                                                where l.Familia == elemType.FamilyName.ToString()
                                                select new
                                                {
                                                    Familia = l.Familia,
                                                };
                                    if (Comprueba.Count() == 0)
                                        FamiliasEscaleras.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    Comprueba1 = from l in TiposEscaleras
                                                 where l.Tipo == elem.Name.ToString()
                                                 select new
                                                 {
                                                     Tipo = l.Tipo,
                                                 };
                                    if (Comprueba1.Count() == 0)
                                        TiposEscaleras.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;
                                case "Modelos genéricos":
                                    ModelosGenericos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Modelos genéricos", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    Comprueba = from l in FamiliasModelosGenericos
                                                where l.Familia == elemType.FamilyName.ToString()
                                                select new
                                                {
                                                    Familia = l.Familia,
                                                };
                                    if (Comprueba.Count() == 0)
                                        FamiliasModelosGenericos.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    Comprueba1 = from l in TiposModelosGenericos
                                                 where l.Tipo == elem.Name.ToString()
                                                 select new
                                                 {
                                                     Tipo = l.Tipo,
                                                 };
                                    if (Comprueba1.Count() == 0)
                                        TiposModelosGenericos.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;
                                case "Pilares estructurales":
                                    PilaresEstructurales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Pilares estructurales", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    Comprueba = from l in FamiliasPilaresEstructurales
                                                where l.Familia == elemType.FamilyName.ToString()
                                                select new
                                                {
                                                    Familia = l.Familia,
                                                };
                                    if (Comprueba.Count() == 0)
                                        FamiliasPilaresEstructurales.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });

                                    Comprueba1 = from l in TiposPilaresEstructurales
                                                 where l.Tipo == elem.Name.ToString()
                                                 select new
                                                 {
                                                     Tipo = l.Tipo,
                                                 };
                                    if (Comprueba1.Count() == 0)
                                        TiposPilaresEstructurales.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;
                                case "Aparatos sanitarios":
                                    AparatosSanitarios.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Aparatos sanitarios", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    Comprueba = from l in FamiliasAparatosSanitarios
                                                where l.Familia == elemType.FamilyName.ToString()
                                                select new
                                                {
                                                    Familia = l.Familia,
                                                };
                                    if (Comprueba.Count() == 0)
                                        FamiliasAparatosSanitarios.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    Comprueba1 = from l in TiposAparatosSanitarios
                                                 where l.Tipo == elem.Name.ToString()
                                                 select new
                                                 {
                                                     Tipo = l.Tipo,
                                                 };
                                    if (Comprueba1.Count() == 0)
                                        TiposAparatosSanitarios.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;

                                    //default: return;
                            }


                        }
                    //MessageBox.Show(elem.Category.Name + " Id =" + elem.Id, "");
                    /////////////////////////////////////////////////////////////////////
                    /*m_curObj = snoopableObject.Object;
                    // collect the data about this object
                    m_snoopCollector.Collect(m_curObj);
                    // display it
                    Utils.Display(m_lvData, m_snoopCollector);*/
                }

                if (snoopableObject.GetUnderlyingType().Name == "Rebar")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            RebarBarType elemType = (RebarBarType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Armadura estructural":
                                    ArmadurasEstructuales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Armadura estructural", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasArmadurasEstructuales
                                                where l.Familia == elemType.FamilyName.ToString()
                                                select new
                                                {
                                                    Familia = l.Familia,
                                                };
                                    if (Comprueba.Count() == 0)
                                        FamiliasArmadurasEstructuales.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposArmadurasEstructuales
                                                     where l.Tipo == elem.Name.ToString()
                                                 select new
                                                 {
                                                     Tipo = l.Tipo,
                                                 };
                                    if (Comprueba1.Count() == 0)
                                        TiposArmadurasEstructuales.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });
                                    ; break;
                            }
                        }
                }

                if (snoopableObject.GetUnderlyingType().Name == "Floor")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            FloorType elemType = (FloorType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Cimentación estructural":
                                    CimentacionesEstructurales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Cimentación estructural", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasCimentacionesEstructurales
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasCimentacionesEstructurales.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposCimentacionesEstructurales
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposCimentacionesEstructurales.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;
                                case "Suelos":
                                    Suelos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Suelos", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    Comprueba = from l in FamiliasSuelos
                                                where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasSuelos.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    Comprueba1 = from l in TiposSuelos
                                                 where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposSuelos.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "CableTray")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            CableTrayType elemType = (CableTrayType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Bandejas de cables":
                                    BandejasdeCables.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Bandejas de cables", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasBandejasdeCables
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasBandejasdeCables.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposBandejasdeCables
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposBandejasdeCables.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "Railing")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            RailingType elemType = (RailingType) Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Barandillas":
                                    Barandillas.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Barandillas", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasBarandillas
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasBarandillas.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposBarandillas
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposBarandillas.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });
                                    ; break;

                            }
                        }
                }

                if (snoopableObject.GetUnderlyingType().Name == "Duct")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            DuctType elemType = (DuctType) Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Conductos":
                                    Conductos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Conductos", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    //Barandillas.Add(new RevitElementoBase { Id = elem.Id.ToString(), Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasConductos
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasConductos.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposConductos
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposConductos.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "FlexDuct")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            FlexDuctType elemType = (FlexDuctType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Conductos flexibles":
                                    ConductosFlexibles.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Conductos flexibles", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasConductosFlexibles
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasConductosFlexibles.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposConductosFlexibles
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposConductosFlexibles.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "FootPrintRoof")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            RoofType elemType = (RoofType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Cubiertas":
                                    Cubiertas.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Cubiertas", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasCubiertas
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasCubiertas.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposCubiertas
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposCubiertas.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "Wall")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            WallType elemType = (WallType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Muros":
                                    Muros.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Muros", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasMuros
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasMuros.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposMuros
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposMuros.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "Ceiling")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            CeilingType elemType = (CeilingType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Techos":
                                    Techos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Techos", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasTechos
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasTechos.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposTechos
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposTechos.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "Pipe")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            PipeType elemType = (PipeType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Tuberías":
                                    Tuberias.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Tuberías", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasTuberias
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasTuberias.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposTuberias
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposTuberias.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "FlexPipe")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            FlexPipeType elemType = (FlexPipeType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Tuberías flexibles":
                                    TuberiasFlexibles.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Tuberías flexibles", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasTuberiasFlexibles
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasTuberiasFlexibles.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposTuberiasFlexibles
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposTuberiasFlexibles.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;

                            }
                        }
                }

                if (snoopableObject.GetUnderlyingType().Name == "Conduit")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            ConduitType elemType = (ConduitType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Tubos":
                                    Tubos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Tubos", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasTubos
                                                    where l.Familia == elemType.FamilyName.ToString()
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasTubos.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                    var Comprueba1 = from l in TiposTubos
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposTubos.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "TopographySurface")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            //ConduitType elemType = (ConduitType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Topografía":
                                    Topografias.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Topografía", Familia = "Topografía", Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    var Comprueba = from l in FamiliasTopografias
                                                    where l.Familia == "Topografía"
                                                    select new
                                                    {
                                                        Familia = l.Familia,
                                                    };
                                    if (Comprueba.Count() == 0)
                                        FamiliasTopografias.Add(new RevitFamiliaBase { Familia = "Topografía" });
                                    var Comprueba1 = from l in TiposTopografias
                                                     where l.Tipo == elem.Name.ToString()
                                                     select new
                                                     {
                                                         Tipo = l.Tipo,
                                                     };
                                    if (Comprueba1.Count() == 0)
                                        TiposTopografias.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "Element")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            ElementType elemType = (ElementType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            if (elemType is null) { }
                            else
                                switch (elem.Category.Name)
                                {
                                    case "Rampas":
                                        Rampas.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Rampas", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                        var Comprueba = from l in FamiliasRampas
                                                        where l.Familia == elemType.FamilyName.ToString()
                                                        select new
                                                        {
                                                            Familia = l.Familia,
                                                        };
                                        if (Comprueba.Count() == 0)
                                            FamiliasRampas.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                        var Comprueba1 = from l in TiposRampas
                                                         where l.Tipo == elem.Name.ToString()
                                                         select new
                                                         {
                                                             Tipo = l.Tipo,
                                                         };
                                        if (Comprueba1.Count() == 0)
                                            TiposRampas.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                        ; break;

                                }
                        }
                }



                if (snoopableObject.GetUnderlyingType().Name == "Stairs")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            ElementType elemType = (ElementType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            if (elemType is null) { }
                            else
                                switch (elem.Category.Name)
                                {
                                    case "Escaleras":
                                        Escaleras.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Escaleras", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                        var Comprueba = from l in FamiliasEscaleras
                                                        where l.Familia == elemType.FamilyName.ToString()
                                                        select new
                                                        {
                                                            Familia = l.Familia,
                                                        };
                                        if (Comprueba.Count() == 0)
                                            FamiliasEscaleras.Add(new RevitFamiliaBase { Familia = elemType.FamilyName.ToString() });
                                        var Comprueba1 = from l in TiposEscaleras
                                                         where l.Tipo == elem.Name.ToString()
                                                         select new
                                                         {
                                                             Tipo = l.Tipo,
                                                         };
                                        if (Comprueba1.Count() == 0)
                                            TiposEscaleras.Add(new RevitTipoBase { Tipo = elem.Name.ToString() });

                                        ; break;

                                }
                        }
                }



                //PARAMETROS COMPARTIDOS
                if (snoopableObject.GetUnderlyingType().Name == "SharedParameterElement")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        ParametrosCompartidos.Add(new RevitParametroBase { Nombre = elem.Name.ToString() });
                }


                // hook this up to the correct spot in the tree based on the object's type
                TreeNode parentNode = GetExistingNodeForType(snoopableObject.GetUnderlyingType());
                if (parentNode == null)
                {
                    parentNode = new TreeNode(snoopableObject.GetUnderlyingType().Name);
                    m_tvObjs.Nodes.Add(parentNode);
                    
                    //MessageBox.Show();
                    
                    // record that we've seen this one
                    m_treeTypeNodes.Add(parentNode);
                    m_types.Add(snoopableObject.GetUnderlyingType());
                }

                // add the new node for this element
                var tmpNode = new TreeNode(snoopableObject.Title) { Tag = snoopableObject.Object };
                parentNode.Nodes.Add(tmpNode);
            }
            treeView2.EndUpdate();

            //AddSeries(pieChart);
            _NwData[0].Count = ArmazonesEstructurales.Count();
            _NwData[1].Count = ArmadurasEstructuales.Count();
            _NwData[2].Count = BandejasdeCables.Count();
            _NwData[3].Count = Barandillas.Count();
            _NwData[4].Count = CimentacionesEstructurales.Count();
            _NwData[5].Count = Conductos.Count();
            _NwData[6].Count = ConductosFlexibles.Count();
            _NwData[7].Count = ConexionesEstructurales.Count();
            _NwData[8].Count = Cubiertas.Count();
            _NwData[9].Count = Emplazamientos.Count();
            _NwData[10].Count = Escaleras.Count();
            _NwData[11].Count = ModelosGenericos.Count();
            _NwData[12].Count = AparatosSanitarios.Count();
            _NwData[13].Count = Muros.Count();
            _NwData[14].Count = PilaresEstructurales.Count();
            _NwData[15].Count = Rampas.Count();
            _NwData[16].Count = Suelos.Count();
            _NwData[17].Count = Techos.Count();
            _NwData[18].Count = Tuberias.Count();
            _NwData[19].Count = TuberiasFlexibles.Count();
            _NwData[20].Count = Tubos.Count();
            _NwData[21].Count = Topografias.Count();


            /*FamiliasArmazonesEstructurales = FamiliasArmazonesEstructurales.Distinct().ToList(); 
            FamiliasArmadurasEstructuales = FamiliasArmadurasEstructuales.Distinct().ToList();
            FamiliasBandejasdeCables = FamiliasBandejasdeCables.Distinct().ToList();
            FamiliasBarandillas = FamiliasBarandillas.Distinct().ToList();
            FamiliasCimentacionesEstructurales = FamiliasCimentacionesEstructurales.Distinct().ToList();
            FamiliasConductos = FamiliasConductos.Distinct().ToList();
            FamiliasConductosFlexibles = FamiliasConductosFlexibles.Distinct().ToList();
            FamiliasConexionesEstructurales = FamiliasConexionesEstructurales.Distinct().ToList();
            FamiliasCubiertas = FamiliasCubiertas.Distinct().ToList();
            FamiliasEmplazamientos = FamiliasEmplazamientos.Distinct().ToList();
            FamiliasEscaleras = FamiliasEscaleras.Distinct().ToList();
            FamiliasModelosGenericos = FamiliasModelosGenericos.Distinct().ToList();
            FamiliasAparatosSanitarios = FamiliasAparatosSanitarios.Distinct().ToList();
            FamiliasMuros = FamiliasMuros.Distinct().ToList();
            FamiliasPilaresEstructurales = FamiliasPilaresEstructurales.Distinct().ToList();
            FamiliasRampas = FamiliasRampas.Distinct().ToList();
            FamiliasSuelos = FamiliasSuelos.Distinct().ToList();
            FamiliasTechos = FamiliasTechos.Distinct().ToList();
            FamiliasTuberias = FamiliasTuberias.Distinct().ToList();
            FamiliasTuberiasFlexibles = FamiliasTuberiasFlexibles.Distinct().ToList();
            FamiliasTubos = FamiliasTubos.Distinct().ToList();

            TiposArmazonesEstructurales = TiposArmazonesEstructurales.Distinct().ToList();
            TiposArmadurasEstructuales = TiposArmadurasEstructuales.Distinct().ToList();
            TiposBandejasdeCables = TiposBandejasdeCables.Distinct().ToList();
            TiposBarandillas = TiposBarandillas.Distinct().ToList();
            TiposCimentacionesEstructurales = TiposCimentacionesEstructurales.Distinct().ToList();
            TiposConductos = TiposConductos.Distinct().ToList();
            TiposConductosFlexibles = TiposConductosFlexibles.Distinct().ToList();
            TiposConexionesEstructurales = TiposConexionesEstructurales.Distinct().ToList();
            TiposCubiertas = TiposCubiertas.Distinct().ToList();
            TiposEmplazamientos = TiposEmplazamientos.Distinct().ToList();
            TiposEscaleras = TiposEscaleras.Distinct().ToList();
            TiposModelosGenericos = TiposModelosGenericos.Distinct().ToList();
            TiposAparatosSanitarios = TiposAparatosSanitarios.Distinct().ToList();
            TiposMuros = TiposMuros.Distinct().ToList();
            TiposPilaresEstructurales = TiposPilaresEstructurales.Distinct().ToList();
            TiposRampas = TiposRampas.Distinct().ToList();
            TiposSuelos = TiposSuelos.Distinct().ToList();
            TiposTechos = TiposTechos.Distinct().ToList();
            TiposTuberias = TiposTuberias.Distinct().ToList();
            TiposTuberiasFlexibles = TiposTuberiasFlexibles.Distinct().ToList();
            TiposTubos = TiposTubos.Distinct().ToList();*/

            InitializeChart();
            //InitializeComboItems();
            PieChart pieChart = (PieChart)chartControl1.ChartPanel.ChartContainers[0];

            pieChart.PaletteGroup = (PaletteGroup)
                Enum.Parse(typeof(PaletteGroup), PaletteGroup.Color2.ToString());
            // Allocate and initialize a timer to use for displaying
            // temp information in the Pie Center area.

            _Timer = new System.Windows.Forms.Timer();

            _Timer.Interval = 1500;
            _Timer.Tick += Timer_Tick;

            // Hook the PieSelectionChanged event so that we can give the user
            // feedback on what items they have selected.

            chartControl1.PieSelectionChanged += chartControl1_PieSelectionChanged;

        }


        protected TreeNode GetExistingNodeForType(System.Type objType)
        {
            int len = m_types.Count;
            for (int i = 0; i < len; i++)
            {
                if ((System.Type)m_types[i] == objType)
                    return (TreeNode)m_treeTypeNodes[i];
            }

            return null;
        }

        private void m_tvObjs_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            //m_curObj = e.Node.Tag;
            //m_curObj = m_tvObjs.SelectedNode.Tag;
            //MessageBox.Show(e.Node.Tag.ToString(), "");
            // collect the data about this object
            //m_snoopCollector.Collect(e.Node.Tag);
            //MessageBox.Show(m_snoopCollector.Data().Count.ToString(), "");
            // display it
            //Utils.Display(m_lvData, m_snoopCollector);
            
            
            /*var elem = (Element)e.Node.Tag as Element;
            if (elem != null)
                if (elem.Category != null)
                    MessageBox.Show(elem.Category.Name + " Id =" + elem.Id, "");*/
            
            m_pgProps.SelectedObject = e.Node.Tag;

            var elem = (Element)e.Node.Tag as Element;
            List<Autodesk.Revit.DB.Parameter> lista = new List<Autodesk.Revit.DB.Parameter>();
            if (lista != null && elem != null) {
                lista = (List<Autodesk.Revit.DB.Parameter>)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.Id).GetOrderedParameters();
                //lista = commandData.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters("Marca")
                //SGPropiedades.PrimaryGrid.DeleteAll()
                //SGPropiedades.PrimaryGrid.PurgeDeletedRows()
                //propertyGrid2.SelectedObject = (object)lista.AsEnumerable();
                //Element elemType = (FamilySymbol) Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                try
                {
                    FamilySymbol elemType = (FamilySymbol)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                }
                catch { 
                
                }
                
                //MessageBox.Show(elemType.FamilyName, "");
                //elem?.Symbol.FamilyName;
            }
            

            do
            {
                superGridControl2.PrimaryGrid.DeleteAll();
                superGridControl2.PrimaryGrid.PurgeDeletedRows();
            }
            while (superGridControl2.PrimaryGrid.Rows.Count != 0);

            foreach (Autodesk.Revit.DB.Parameter propiedad in lista)
            { 
                GridRow fil = new GridRow(elem.Id.ToString(), elem.Id.ToString(), propiedad.Definition.Name.ToString(), propiedad.AsValueString(), propiedad.AsString(), propiedad.AsDouble().ToString());
                superGridControl2.PrimaryGrid.Rows.Add(fil);
            }

                /*lista = commandData.Application.ActiveUIDocument.Document.GetElement(currentid).GetOrderedParameters
                For Each DATO As Parameter In lista
                    Dim FACTOR As Double = 1
                    Dim FACTOR1 As Double = 1
                    If Not DATO.AsValueString Is Nothing Then
                        If Len(DATO.AsValueString.ToString) > 3 Then
                            'MsgBox(DATO.Definition.Name.ToString & " " & Mid(DATO.AsValueString(), Len(DATO.AsValueString) - 1, 2) & " " & Mid(DATO.AsValueString(), Len(DATO.AsValueString), 1))
                            If Mid(DATO.AsValueString.ToString, Len(DATO.AsValueString.ToString) -1, 2) = " m" Then FACTOR = 3.281
                            If Mid(DATO.AsValueString.ToString, Len(DATO.AsValueString.ToString) -1, 2) = "m²" Then FACTOR = 10.764
                            If Mid(DATO.AsValueString.ToString, Len(DATO.AsValueString.ToString) -1, 2) = "m³" Then FACTOR = 35.315
                            If Mid(DATO.AsValueString.ToString, Len(DATO.AsValueString.ToString) -1, 2) = "mm" Then FACTOR1 = 305
                        End If
                    End If
                    'DATO.Definition.UnitType
                    Dim fil As New GridRow(currentid.ToString, currentid.ToString, DATO.Definition.Name.ToString, DATO.AsValueString(), DATO.AsString, (DATO.AsDouble * FACTOR1 / FACTOR).ToString)
                    SGPropiedades.PrimaryGrid.Rows.Add(fil)
                    'MsgBox(DATO.Definition.Name.ToString & " = " & DATO.AsValueString() & " - " & DATO.AsString & " - " & (DATO.AsDouble / 3.281).ToString)
                Next*/





            }

        private void m_pgProps_Click(object sender, EventArgs e)
        {
           /* object selObj = m_pgProps.SelectedGridItem.Value;
            MessageBox.Show(m_pgProps.SelectedGridItem.Value.ToString(),"");
            propertyGrid1.SelectedObject = selObj;*/
        }

        private void m_pgProps_MouseClick(object sender, MouseEventArgs e)
        {
            

        }

        private void m_pgProps_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            
        }

        private void m_pgProps_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            object selObj = m_pgProps.SelectedGridItem.Value;
            //var elem = (Element) selObj as Element;
            //var elem = (Element) selObj as Element;
            //elem.Category.Name;

            
            //MessageBox.Show(m_pgProps.SelectedGridItem.Value.ToString(), "");
            propertyGrid1.SelectedObject = selObj;
        }

        private void propertyGrid1_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            object selObj = propertyGrid1.SelectedGridItem.Value;
            //MessageBox.Show(m_pgProps.SelectedGridItem.Value.ToString(), "");
            propertyGrid3.SelectedObject = selObj;

        }

        private void timer9_Tick(object sender, EventArgs e)
        {
            timer9.Enabled = false;
            
            Autodesk.Revit.DB.Document doc = Datos.cmdData1.Application.ActiveUIDocument.Document;
            Settings documentSettings = doc.Settings;

            // Get all categories of current document
            Autodesk.Revit.DB.Categories groups = documentSettings.Categories;

            // Show the number of all the categories to the user
            //System.String prompt = "Number of all categories in current Revit document:" + groups.Size;
            treeView1.BeginUpdate();
            treeView1.Sorted = true;

            //propertyGrid2.SelectedObject = groups;
            foreach (var eleme in groups)
            {
                var elem = (Autodesk.Revit.DB.Category)eleme;
                TreeNode Nodop = new TreeNode(elem.Name.ToString());
                treeView1.Nodes.Add(Nodop);
            }
            treeView1.EndUpdate();
            // get Floor category according to OST_Floors and show its name
            //Category floorCategory = groups.get_Item(BuiltInCategory.OST_Floors);
            //prompt += floorCategory.Name;

            // Give the user some information
            //TaskDialog.Show("Revit", prompt);


            CommonInit(Datos.objs.Cast<object>().Select(SnoopableObjectWrapper.Create));
            CargarFamilias();
        }


 



        private void SGPresupuestos_MouseClick(object sender, MouseEventArgs e)
        {
           
            if (e.Button == MouseButtons.Right)
            {
                System.Drawing.Point pt = System.Windows.Forms.Control.MousePosition;
                MenuPresupuestos.Popup(pt);
            }
        }

        private void SGCategoriasC_RowAdded(object sender, GridRowAddedEventArgs e)
        {
            //verificar que ingrese un dato
            GridPanel panel = (GridPanel)SGestructuraC.PrimaryGrid;
            GridRow ultimaFila = (GridRow)panel.Rows[panel.Rows.Count-1];
            GridRow PenultimaFila = (GridRow)panel.Rows[panel.Rows.Count - 2];
            PenultimaFila.Cells[2].Value = "Nivel " + (panel.Rows.Count() - 1).ToString();


            GridRow fila = PenultimaFila;
            if (fila is null) return;

            if (Item_actual == "") return;
            if (TotalElementos is null) return;

            //si existen datos?
            bool vacios = true;
            for (int x = 0; x <= 3; x++)
            {
                if (fila.Cells[x].Value is null) fila.Cells[x].Value = "";
                if (fila.Cells[x].Value.ToString() != "") vacios = false;
            }
            if (fila.Cells[4].Value is null) fila.Cells[4].Value = false;

            if (!vacios)
            {
                if (fila.Cells[0].Value.ToString() == "") fila.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                string campoMostrar = "";
                if ((bool)fila.Cells[4].Value == true) campoMostrar = "true";
                if ((bool)fila.Cells[4].Value == false) campoMostrar = "false";
                GuardarEstructura(fila.Cells[0].Value.ToString(), fila.Cells[2].Value.ToString(), fila.Cells[3].Value.ToString(), campoMostrar);
            }



        }

        private void SGAsociados_EndEdit(object sender, GridEditEventArgs e)
        {
            
            GridPanel panel = (GridPanel)SGAsociados.PrimaryGrid;
            GridRow fila = (GridRow)panel.ActiveRow;
            if (fila is null) return;
            GridCell fami = (GridCell)fila.Cells[3];
            GridCell Tip = (GridCell)fila.Cells[4];

            if (Item_actual == "") return;
            if (TotalElementos is null) return;

            var ArFamilias = new string[501];
            var ArTipos = new string[501];
            var ArPropiedades = new string[501];



            var I = default(int);
            ArFamilias[0] = "";
            ArTipos[0] = "";
            ArPropiedades[0] = "";

            if (e.GridCell.ColumnIndex == 2 ) {
                if ((string)e.GridCell.Value != "")
                {
                    selecciona_categoria((string)e.GridCell.Value);
                    if (TipoSeleccionado is null) return;
                    if (FamiliaSeleccionada is null) return;
                    I = 1;
                    foreach (var dato in TipoSeleccionado)
                    {
                        ArTipos[I] = dato.Tipo;
                        I++;
                    }
                    I = 1;
                    foreach (var dato in FamiliaSeleccionada)
                    {
                        ArFamilias[I] = dato.Familia;
                        I++;
                    }
                }
                else {
                    I = 1;
                    foreach (var dato in FamiliaTotal)
                    {
                        ArFamilias[I] = dato.Familia;
                        I++;
                    }
                    I = 1;
                    foreach (var dato in TipoTotal)
                    {
                        ArTipos[I] = dato.Tipo;
                        I++;
                    }
                }
                fila.Cells[3].Value = "";
                fila.Cells[4].Value = "";
                fami.EditorType = typeof(FragrantComboBox);
                fami.EditorParams = new object[] { ArFamilias };
                Tip.EditorType = typeof(FragrantComboBox);
                Tip.EditorParams = new object[] { ArTipos };
            }

            if (e.GridCell.ColumnIndex == 3)
            {
                if (TotalElementos is null) return;
                var FitroCategoria = from l in TotalElementos
                                     where l.Familia == (string)e.GridCell.Value
                                     select new
                                      {
                                          Categoria = l.Categoria,
                                      };
                if (FitroCategoria.Count() != 0)
                {
                    var Dato = FitroCategoria.First();
                    fila.Cells[2].Value = Dato.Categoria;
                    fila.Cells[4].Value = "";
                }
                selecciona_categoria((string)fila.Cells[2].Value);
                selecciona_familia((string)e.GridCell.Value);
                I = 1;
                if (TipoSeleccionado is null) { }
                else
                foreach (var dato in TipoSeleccionado)
                {
                    ArTipos[I] = dato.Tipo;
                    I++;
                }
                Tip.EditorType = typeof(FragrantComboBox);
                Tip.EditorParams = new object[] { ArTipos };

            }



            if (e.GridCell.ColumnIndex == 4)
            {
                var FitroFamCat = from l in TotalElementos
                                     where l.Tipo == (string)e.GridCell.Value
                                     select new
                                     {
                                         Categoria = l.Categoria,
                                         Familia= l.Familia,
                                     };
                if (FitroFamCat.Count() != 0)
                {
                    var Dato = FitroFamCat.First();
                    fila.Cells[2].Value = Dato.Categoria;
                    fila.Cells[3].Value = Dato.Familia;
                }
            }



            if (SGAsociados.PrimaryGrid.Rows.Count > 1)
            {
                //verificar sise han cargado antes
                //GridPanel panelAS = (GridPanel)SGAsociados.PrimaryGrid;
                //GridRow filaAS = (GridRow)panel.Rows[0];

                var FitroCategoria = from l in TotalElementos
                                     where l.Categoria == (string)fila.Cells[2].Value
                                     select new
                                     {
                                         Categoria = l.Categoria,
                                         UniqueId = l.UniqueId,
                                     };
                if (FitroCategoria.Count() != 0)
                {
                    var Dato = FitroCategoria.First();
                    UNIQID = Dato.UniqueId;
                       var elem = (Element)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQID) as Element;
                    List<Autodesk.Revit.DB.Parameter> lista = new List<Autodesk.Revit.DB.Parameter>();
                    if (lista != null && elem != null)
                    {
                        lista = (List<Autodesk.Revit.DB.Parameter>)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.Id).GetOrderedParameters();
                        I = 1;
                        foreach (Autodesk.Revit.DB.Parameter propiedad in lista)
                        {
                            ArPropiedades[I] = propiedad.Definition.Name.ToString();
                            I++;
                        }
                        fila.Cells[5].EditorType = typeof(FragrantComboBox);
                        fila.Cells[5].EditorParams = new object[] { ArPropiedades };
                    }
                }
            }

            //si existen datos?
            bool vacios = true;
            for (int x = 0; x <= 7; x++)
            {
                if (fila.Cells[x].Value is null) fila.Cells[x].Value = "";
                if (fila.Cells[x].Value.ToString() != "") vacios = false;
            }

            if (!vacios) {
                string valorF = "";
                if (fila.Cells[6].Value.ToString()=="Igual") valorF = "=";
                if (fila.Cells[6].Value.ToString() == "Diferente") valorF = "!";
                if (fila.Cells[6].Value.ToString() == "" && fila.Cells[7].Value.ToString() != "") valorF = "?";
                if (fila.Cells[0].Value.ToString() == "") fila.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                string cadenaTipo = fila.Cells[4].Value.ToString().Replace("'", "''");
                GuardarAsociado(fila.Cells[0].Value.ToString(), fila.Cells[2].Value.ToString(), fila.Cells[3].Value.ToString(), cadenaTipo, fila.Cells[5].Value.ToString(), valorF + fila.Cells[7].Value.ToString());
            }



            CargaPropiedades();


        }

        private void SGCategoriasC_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                GridPanel panel = (GridPanel)SGestructuraC.PrimaryGrid;
                GridRow fila = (GridRow)panel.ActiveRow;
                if (fila is null) return;
                if (fila.Index == panel.Rows.Count - 1) 
                    BtnElimCantD.Enabled = false;
                else
                    BtnElimCantD.Enabled = true;
                System.Drawing.Point pt = System.Windows.Forms.Control.MousePosition;
                BtMNivel.Popup(pt);
            }
        }

        private void SGAsociados_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                GridPanel panel = (GridPanel)SGAsociados.PrimaryGrid;
                GridRow fila = (GridRow)panel.ActiveRow;
                if (fila is null) return;
                if (fila.Index == panel.Rows.Count - 1)
                    BtcElimAPU.Enabled = false;
                else
                    BtcElimAPU.Enabled = true;

                System.Drawing.Point pt = System.Windows.Forms.Control.MousePosition;
                BtnMAsociados.Popup(pt);
            }

        }

        private void BtnElimCantD_Click(object sender, EventArgs e)
        {
            GridPanel panel = (GridPanel)SGestructuraC.PrimaryGrid;
            GridRow fila = (GridRow)panel.ActiveRow;
            if (fila is null) return;

            EliminarxCodigo("Estructura", fila.Cells[0].Value.ToString());
            fila.IsDeleted = true;
            panel.PurgeDeletedRows();
            for (int x = 0; x < panel.Rows.Count() - 1; x++)
            {
                GridRow auxfila = (GridRow)panel.Rows[x];
                auxfila.Cells[2].Value = "Nivel " + (x+1).ToString();

                bool vacios = true;
                for (int x1 = 0; x1 <= 3; x1++)
                {
                    if (auxfila.Cells[x1].Value is null) auxfila.Cells[x1].Value = "";
                    if (auxfila.Cells[x1].Value.ToString() != "") vacios = false;
                }
                if (auxfila.Cells[4].Value is null) auxfila.Cells[4].Value = false;

                if (!vacios)
                {
                    if (auxfila.Cells[0].Value.ToString() == "") auxfila.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string campoMostrar = "";
                    if ((bool)auxfila.Cells[4].Value == true) campoMostrar = "true";
                    if ((bool)auxfila.Cells[4].Value == false) campoMostrar = "false";
                    GuardarEstructura(auxfila.Cells[0].Value.ToString(), auxfila.Cells[2].Value.ToString(), auxfila.Cells[3].Value.ToString(), campoMostrar);
                }

            }
        }

        private void BtcElimAPU_Click(object sender, EventArgs e)
        {
            GridPanel panel = (GridPanel)SGAsociados.PrimaryGrid;
            GridRow fila = (GridRow)panel.ActiveRow;
            if (fila is null) return;
            
            EliminarxCodigo("Asociado", fila.Cells[0].Value.ToString());

            fila.IsDeleted = true;
            panel.PurgeDeletedRows();
            //eliminar el registro actual

        }

        private void BtnAgreRegC_Click(object sender, EventArgs e)
        {
            GridPanel panel = (GridPanel)SGestructuraC.PrimaryGrid;
            GridRow fila = (GridRow)panel.ActiveRow;
            if (fila is null) return;
            GridRow filaNueva = new GridRow("","","","","");
            panel.Rows.Insert(fila.Index, filaNueva);
            for (int x = 0; x < panel.Rows.Count() - 1; x++)
            {
                GridRow auxfila = (GridRow)panel.Rows[x];
                auxfila.Cells[2].Value = "Nivel " + (x+1).ToString();
            }

            //fila.IsDeleted = true;
            //panel.PurgeDeletedRows();

        }

        private void SuperTabControl11_SelectedTabChanged(object sender, DevComponents.DotNetBar.SuperTabStripSelectedTabChangedEventArgs e)
        {
            var ArPropiedades = new string[501];
            var ArSignos = new string[501];
            var I = default(int);
            GridPanel panel1 = (GridPanel)SGFormulas.PrimaryGrid;
            GridPanel panel2 = (GridPanel)SGDetFormulas.PrimaryGrid;
            GridRow fila1 = (GridRow)panel1.Rows[0];
            if (TotalElementos is null) return;
            ArPropiedades[0] = "";
            ArSignos[0] = "";
            ArSignos[1] = "+";
            ArSignos[2] = "-";
            ArSignos[3] = "*";
            ArSignos[4] = "/";
            if (SuperTabControl11.SelectedTabIndex == 4) {
                if (SGAsociados.PrimaryGrid.Rows.Count > 1) {
                    //verificar sise han cargado antes
                    GridPanel panel = (GridPanel)SGAsociados.PrimaryGrid;
                    GridRow fila = (GridRow)panel.Rows[0];
                    var FitroCategoria = from l in TotalElementos
                                         where l.Categoria == (string)fila.Cells[2].Value
                                         select new
                                         {
                                             Categoria = l.Categoria,
                                             UniqueId = l.UniqueId,
                                         };
                    if (FitroCategoria.Count() != 0)
                    {
                        var Dato = FitroCategoria.First();
                        UNIQID = Dato.UniqueId;
                        //ELEMENTO As Element = commandData.Application.ActiveUIDocument.Document.GetElement(UNIQID)

                        //Dim ESTEID As ElementId = ELEMENTO.Id

                        //m_pgProps.SelectedObject = e.Node.Tag;

                        //var elem = (Element)e.Node.Tag as Element;
                        var elem = (Element)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQID) as Element;
                        List<Autodesk.Revit.DB.Parameter> lista = new List<Autodesk.Revit.DB.Parameter>();
                        if (lista != null && elem != null)
                        {
                            lista = (List<Autodesk.Revit.DB.Parameter>)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.Id).GetOrderedParameters();
                            I = 1;
                            foreach (Autodesk.Revit.DB.Parameter propiedad in lista)
                            {
                                ArPropiedades[I] = propiedad.Definition.Name.ToString();
                                I++;
                                //CARGAR TAMBIEN EN EL TREVIEW DEL CATALOGO
                                //GridRow fil = new GridRow(elem.Id.ToString(), elem.Id.ToString(), propiedad.Definition.Name.ToString(), propiedad.AsValueString(), propiedad.AsString(), propiedad.AsDouble().ToString());
                                //superGridControl2.PrimaryGrid.Rows.Add(fil);
                            }
                            fila1.Cells[1].EditorType = typeof(FragrantComboBox);
                            fila1.Cells[1].EditorParams = new object[] { ArPropiedades };
                            fila1.Cells[2].EditorType = typeof(FragrantComboBox);
                            fila1.Cells[2].EditorParams = new object[] { ArPropiedades };
                            fila1.Cells[3].EditorType = typeof(FragrantComboBox);
                            fila1.Cells[3].EditorParams = new object[] { ArPropiedades };
                            fila1.Cells[4].EditorType = typeof(FragrantComboBox);
                            fila1.Cells[4].EditorParams = new object[] { ArPropiedades };
                            fila1.Cells[5].EditorType = typeof(FragrantComboBox);
                            fila1.Cells[5].EditorParams = new object[] { ArPropiedades };

                            panel2.Columns[2].EditorType = typeof(FragrantComboBox);
                            panel2.Columns[2].EditorParams = new object[] { ArPropiedades };
                            panel2.Columns[3].EditorType = typeof(FragrantComboBox);
                            panel2.Columns[3].EditorParams = new object[] { ArSignos };



                        }




                        //fila.Cells[2].Value = Dato.Categoria;
                        //fila.Cells[4].Value = "";
                    }



                }
            }
        }

        private void SGDetFormulas_RowAdded(object sender, GridRowAddedEventArgs e)
        {
            
 
        }


        void actualiza_det_formulaDet() {

            GridPanel panel = (GridPanel)SGDetFormulas.PrimaryGrid;
            GridPanel panel1 = (GridPanel)SGFormulas.PrimaryGrid;
            GridRow filaP = (GridRow)panel1.ActiveRow;
            GridCell celdaP = (GridCell)panel1.ActiveCell;
            SGDetFormulas.Enabled = false;

            panel.Title.Text = "";
            panel.DeleteAll();
            panel.PurgeDeletedRows();

            GridRow nuevafila = new GridRow("", "", "", "");
            panel.Rows.Add(nuevafila);

            if (filaP is null) return;
            if (celdaP is null) return;



            if (celdaP.ColumnIndex == 3 && celdaP.RowIndex == 0)
            {
                panel.Title.Text = "Longitud/Area/Volumen";
                SGDetFormulas.Enabled = true;

                if (ListaCalculoDetalle is null) return;
                if (filaP.Cells.Count() == 0) return;
                if (filaP.Cells[0].Value is null) filaP.Cells[0].Value = "";

                List<LCalculoDetalle> listaDetFormulaFitrada = ListaCalculoDetalle.Where(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Longitud").OrderBy(X => X.Posicion).ToList();
                foreach (var dato in listaDetFormulaFitrada)
                {
                    if (dato.Posicion is null) dato.Posicion = "";
                    nuevafila = new GridRow(dato.CodCalculoDetalle, dato.CodCalculo, dato.Campo, dato.Operacion, dato.Posicion.ToString());
                    panel.Rows.Add(nuevafila);
                }

            }
            if (celdaP.ColumnIndex == 4 && celdaP.RowIndex == 0)
            {
                panel.Title.Text = "Ancho/Peso";
                SGDetFormulas.Enabled = true;

                if (ListaCalculoDetalle is null) return;
                if (filaP.Cells.Count() == 0) return;
                if (filaP.Cells[0].Value is null) filaP.Cells[0].Value = "";
                List<LCalculoDetalle> listaDetFormulaFitrada = ListaCalculoDetalle.Where(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Ancho").OrderBy(X => X.Posicion).ToList();
                foreach (var dato in listaDetFormulaFitrada)
                {
                    if (dato.Posicion is null) dato.Posicion = "";
                    nuevafila = new GridRow(dato.CodCalculoDetalle, dato.CodCalculo, dato.Campo, dato.Operacion, dato.Posicion.ToString());
                    panel.Rows.Add(nuevafila);
                }


            }
            if (celdaP.ColumnIndex == 5 && celdaP.RowIndex == 0)
            {
                panel.Title.Text = "Altura";
                SGDetFormulas.Enabled = true;

                if (ListaCalculoDetalle is null) return;
                if (filaP.Cells.Count() == 0) return;
                if (filaP.Cells[0].Value is null) filaP.Cells[0].Value = "";
                List<LCalculoDetalle> listaDetFormulaFitrada = ListaCalculoDetalle.Where(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Alto").OrderBy(X => X.Posicion).ToList();
                foreach (var dato in listaDetFormulaFitrada)
                {
                    if (dato.Posicion is null) dato.Posicion = "";
                    nuevafila = new GridRow(dato.CodCalculoDetalle, dato.CodCalculo, dato.Campo, dato.Operacion, dato.Posicion.ToString());
                    panel.Rows.Add(nuevafila);
                }

            }


        }
        
        private void SGFormulas_SelectionChanged(object sender, GridEventArgs e)
        {
            actualiza_det_formulaDet();
        }

        private void SGPresupC_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                GridPanel panel = (GridPanel)SGPresupC.PrimaryGrid;
                GridRow fila = (GridRow)panel.ActiveRow;
                if (fila is null) return;
                /*if (fila.Index == panel.Rows.Count - 1)
                    BtnElimCantD.Enabled = false;
                else
                    BtnElimCantD.Enabled = true;*/
                System.Drawing.Point pt = System.Windows.Forms.Control.MousePosition;
                BtnMenuPres.Popup(pt);
            }
        }


        private void CargarMedicion() {

            GridPanel Panel = (GridPanel)SGPresupC.PrimaryGrid;
            GridRow FilaActual = (GridRow)Panel.ActiveRow;
            if (FilaActual is null) return;
            GridRow PrimeraFila = null;
            GridPanel Panel1 = (GridPanel)SGMediciones.PrimaryGrid;
            if (Panel1.Rows.Count > 0) 
                PrimeraFila = (GridRow)Panel1.Rows[0];


            GridRow Filasel = (GridRow)SGPresupC.PrimaryGrid.ActiveRow;
            if (Filasel is null) return;

            ListaItemsMedicion = new List<LMedicion>();
            ConexionBD basedatos = new ConexionBD();
            basedatos.Conexion();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt= basedatos.LmedicionesSubPresupuesto(Presupuesto_actual, SubPresupuesto_actual, Item_actual);

            if (dt.Rows.Count <= 2)
            {
                Filasel.Cells[4].ReadOnly = false;

                if (dt.Rows.Count == 0)
                {
                    double parcial = 0.0;
                    //valido la cantidad ingresada
                    if (Filasel.Cells[4].Value.ToString() == "") parcial = 0.00;
                    try { parcial = Convert.ToDouble(Filasel.Cells[4].Value.ToString()); } catch { parcial = 0.00; }
                    string CodigoUni = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Metrados Personalizados", Cantidad = "", Longitud = "", Ancho = "", Alto = "", Total = parcial.ToString("N2"), Detalle = "Titulo de Nivel 1", Vinculo = "Personalizado", UniqueId = "", PhantomParentId = "", Nivel = 1, Tipo = "Titulo" });
                    var basdat = new ConexionBD();
                    basdat.Conexion();
                    //basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);
                    basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni, "Metrados Personalizados", "", "", "", "", parcial.ToString("N2"), "Titulo de Nivel 1", "Personalizado", "", "", 1, "Titulo", EmailUsuario);
                    //GuardarMedicion(CodigoUni, "Metrados Personalizados", "", "", "", "", "1.00", "Titulo de Nivel 1", "Personalizado", "", "", 1, "Titulo");
                    //GridRow filaInsertar = new GridRow(CodigoUni, "", "Metrados Personalizados", "", "", "", "", parcial.ToString("N2"), "Titulo de Nivel 1", "Personalizado", "Titulo", "", "1");
                    //var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(181, 185, 168), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                    //filaInsertar.CellStyles.Default.Background = Background;
                    //Panel1.Rows.Insert(0, filaInsertar);
                    //PrimeraFila = filaInsertar;
                    string CodigoUni2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    //filaInsertar = new GridRow(CodigoUni2, "", "Nuevo Item", parcial.ToString("N2"), "", "", "", parcial.ToString("N2"), "Metrado de Nivel 1", "Personalizado", "Medicion", "", "2");
                    ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni2, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Nuevo Item", Cantidad = parcial.ToString("N2"), Longitud = "", Ancho = "", Alto = "", Total = parcial.ToString("N2"), Detalle = "Metrado de Nivel 2", Vinculo = "Personalizado", UniqueId = "", PhantomParentId = CodigoUni, Nivel = 2, Tipo = "Medicion" });
                    //PrimeraFila.Rows.Add(filaInsertar);
                    //PrimeraFila.Expanded = true;
                    basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni2, "Nuevo Item", parcial.ToString("N2"), "", "", "", parcial.ToString("N2"), "Metrado de Nivel 2", "Personalizado", "", CodigoUni, 2, "Medicion", EmailUsuario);
                    basdat.Conexion();
                    //Filasel.Cells[4].Value = parcial.ToString("N2");
                }
                else {
                    if (dt.Rows.Count > 1)
                        if (dt.Rows[1]["Longitud"].ToString().Trim() != "" || dt.Rows[1]["Ancho"].ToString().Trim() != "" || dt.Rows[1]["Alto"].ToString().Trim() != "") {
                        Filasel.Cells[4].ReadOnly = true;
                    }

                }
                
                


            }
            else {
                Filasel.Cells[4].ReadOnly = true;
            }

            foreach (DataRow dat in dt.Rows) {
                ListaItemsMedicion.Add(new LMedicion { CodMedicion = dat["CodMedicion"].ToString().Trim(), CodPresupuesto = dat["CodPresupuesto"].ToString().Trim(), CodSubpresupuesto = dat["CodSubpresupuesto"].ToString().Trim(), Item = dat["Item"].ToString().Trim(), Descripcion = dat["Descripcion"].ToString().Trim(), Cantidad = dat["Cantidad"].ToString().Trim(), Longitud = dat["Longitud"].ToString().Trim(), Ancho = dat["Ancho"].ToString().Trim(), Alto = dat["Alto"].ToString().Trim(), Total = dat["Total"].ToString().Trim(), Detalle = dat["Detalle"].ToString().Trim(), Vinculo = dat["Vinculo"].ToString().Trim(), UniqueId = dat["UniqueId"].ToString().Trim(), PhantomParentId = dat["PhantomParentId"].ToString().Trim(), Nivel = (int)dat["Nivel"], Tipo = dat["Tipo"].ToString().Trim() });
            }
            basedatos.Conexion();
            CargarMediciones();
        }





        //public int filaanterior = -10;
        GridRow filaanterior = null;

        void actualizar_datosCalculoDetalle() {
            var basdat = new ConexionBD();
            ListaCalculoDetalle = new List<LCalculoDetalle>();
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = basdat.TablaDatos("LCalculoDetalle", Presupuesto_actual, SubPresupuesto_actual, Item_actual);
            foreach (DataRow dat in dt.Rows)
            {
                ListaCalculoDetalle.Add(new LCalculoDetalle { CodCalculoDetalle = dat["CodCalculoDetalle"].ToString().Trim(), CodCalculo = dat["CodCalculo"].ToString().Trim(), CodPresupuesto = dat["CodPresupuesto"].ToString().Trim(), CodSubpresupuesto = dat["CodSubpresupuesto"].ToString().Trim(), Item = dat["Item"].ToString().Trim(), TipoCampo = dat["TipoCampo"].ToString().Trim(), Campo = dat["Campo"].ToString().Trim(), Operacion = dat["Operacion"].ToString().Trim(), Posicion = dat["Posicion"].ToString().Trim() });
            }

        }


        private void SGPresupC_SelectionChanged(object sender, GridEventArgs e)
        {
            
            GridRow Filasel = (GridRow)SGPresupC.PrimaryGrid.ActiveRow;
            if (Filasel is null) return;
            if (filaanterior == Filasel) return; else filaanterior = Filasel;
            Limpiar_Partidas1();
            ListaCalculoDetalle = new List<LCalculoDetalle>();
            Item_actual = "";

            /*SuperTabItem5.Text = "METRADO ";
            SuperTabItem2.Text = "APU PARTIDA ";
            
            expandablePanel3.TitleText = "SIN SELECCION";
            SGApus.PrimaryGrid.DeleteAll();
            SGApus.PrimaryGrid.PurgeDeletedRows();
            GridRow fila = new GridRow("", "", "", "", "", "", "", "");
            SGApus.PrimaryGrid.Rows.Add(fila);



            GridRow filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[0];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[1];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[2];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[3];
            filaaz.Cells[2].Value = "0.00";
            filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[4];
            filaaz.Cells[2].Value = "0.00";*/



           /* do
            {
                SGMediciones.PrimaryGrid.DeleteAll();
                SGMediciones.PrimaryGrid.PurgeDeletedRows();
            }
            while (SGMediciones.PrimaryGrid.Rows.Count() != 0);
            //GridRow filaauxP = new GridRow("", "", "", "", "", "", "", "", "", "", "", "", "");
            //filaauxP.ReadOnly = true;
            //SGMediciones.PrimaryGrid.Rows.Add(filaauxP);

            SGDetFormulas.PrimaryGrid.DeleteAll();
            SGDetFormulas.PrimaryGrid.PurgeDeletedRows();
            fila = new GridRow("", "", "", "", "");
            SGDetFormulas.PrimaryGrid.Rows.Add(fila);*/

            //if (Filasel.Rows.Count != 0) return;

            if (Filasel.Cells[3].Value.ToString().Trim() != "")
            {
                expandablePanel3.TitleText = "PARTIDA SELECCIONADA:" + Filasel.Cells[2].Value.ToString();
                textBox1.Text = "";
                cadenaObtenida = "";
                Item_actual = Filasel.Cells[17].Value.ToString();
                solicitar_datosApus();


        //PARA LLAMAR A CARGA POR SIGNAL
        /*SolicitarAsociado();
        SolicitarEstructura();
        SolicitarCalculo();
        SolicitarCalculoDetalle();*/
                ListaAsociados = new List<LAsociado>();
                ListaEstructuras = new List<LEstructura>();
                ListaCalculos = new List<LCalculo>();
                ListaCalculoDetalle= new List<LCalculoDetalle>();

                //CARGA DE BASE LOCAL

                var basdat = new ConexionBD();

                System.Data.DataTable dt = new System.Data.DataTable();
                dt = basdat.TablaDatos("LAsociado",Presupuesto_actual, SubPresupuesto_actual, Item_actual);
                foreach (DataRow dat in dt.Rows)
                {
                    ListaAsociados.Add(new LAsociado { CodAsociado = dat["CodAsociado"].ToString().Trim(), CodPresupuesto = dat["CodPresupuesto"].ToString().Trim(), CodSubpresupuesto = dat["CodSubpresupuesto"].ToString().Trim(), Item = dat["Item"].ToString().Trim(), Categoria = dat["Categoria"].ToString().Trim(), Familia = dat["Familia"].ToString().Trim(), Tipo = dat["Tipo"].ToString().Trim(), CampoFiltro = dat["CampoFiltro"].ToString().Trim(), Valor = dat["Valor"].ToString().Trim() });
                }

                dt = new System.Data.DataTable();
                dt = basdat.TablaDatos("LEstructura", Presupuesto_actual, SubPresupuesto_actual, Item_actual);
                foreach (DataRow dat in dt.Rows)
                {
                    ListaEstructuras.Add(new LEstructura { CodEstructura = dat["CodEstructura"].ToString().Trim(), CodPresupuesto = dat["CodPresupuesto"].ToString().Trim(), CodSubpresupuesto = dat["CodSubpresupuesto"].ToString().Trim(), Item = dat["Item"].ToString().Trim(), Nivel = dat["Nivel"].ToString().Trim(), Campo = dat["Campo"].ToString().Trim(), Mostrar = dat["Mostrar"].ToString().Trim() });
                }

                dt = new System.Data.DataTable();
                dt = basdat.TablaDatos("LCalculo", Presupuesto_actual, SubPresupuesto_actual, Item_actual);
                foreach (DataRow dat in dt.Rows)
                {
                    ListaCalculos.Add(new LCalculo { CodCalculo = dat["CodCalculo"].ToString().Trim(), CodPresupuesto = dat["CodPresupuesto"].ToString().Trim(), CodSubpresupuesto = dat["CodSubpresupuesto"].ToString().Trim(), Item = dat["Item"].ToString().Trim(), Descripcion = dat["Descripcion"].ToString().Trim(), Cantidad = dat["Cantidad"].ToString().Trim(), Longitud = dat["Longitud"].ToString().Trim(), Ancho = dat["Ancho"].ToString().Trim(), Alto = dat["Alto"].ToString().Trim() });
                }

                dt = new System.Data.DataTable();
                dt = basdat.TablaDatos("LCalculoDetalle", Presupuesto_actual, SubPresupuesto_actual, Item_actual);
                foreach (DataRow dat in dt.Rows)
                {
                    ListaCalculoDetalle.Add(new LCalculoDetalle { CodCalculoDetalle = dat["CodCalculoDetalle"].ToString().Trim(), CodCalculo = dat["CodCalculo"].ToString().Trim(), CodPresupuesto = dat["CodPresupuesto"].ToString().Trim(), CodSubpresupuesto = dat["CodSubpresupuesto"].ToString().Trim(), Item = dat["Item"].ToString().Trim(), TipoCampo = dat["TipoCampo"].ToString().Trim(), Campo = dat["Campo"].ToString().Trim(), Operacion = dat["Operacion"].ToString().Trim(),  Posicion = dat["Posicion"].ToString().Trim() });
                }

                Cargar_ConfigsUnd();
                //SolicitarMedicion();
                CargarMedicion();

                CargaDetalleItem();

                actualiza_det_formulaDet();
                timer10.Enabled = true;
            }
            else
            {
                expandablePanel3.TitleText = "TITULO " + Filasel.Cells[2].Value.ToString();
                //textBox1.Text = "";
                //solicitar_datosSubPresupuestos();
                //cadenaObtenida = "";
                //timer6.Enabled = true;
            }
            

        }


        void Cargar_ConfigsUnd() {
            var basdat = new ConexionBD();
            GridRow Filasel = (GridRow)SGPresupC.PrimaryGrid.ActiveRow;
            if (Filasel is null) return;
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = basdat.TablaConfCalculoUnid(Filasel.Cells[3].Value.ToString());


            AdvTree7.ClearAndDisposeAllNodes();
            AdvTree7.BeginUpdate();
            foreach (DataRow dat in dt.Rows)
            {

                var node = new DevComponents.AdvTree.Node();
                node.Tag = dat["CodConfCalculo"].ToString().Trim();
                node.Text = dat["Nombre"].ToString().Trim();
                node.Image = ImageList1.Images[12];
                node.Cells.Add(new DevComponents.AdvTree.Cell(dat["Unidad"].ToString().Trim()));
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                AdvTree7.Nodes.Add(node);
                node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;

                //ListaCalculoDetalle.Add(new LCalculoDetalle { CodCalculoDetalle = dat["CodCalculoDetalle"].ToString().Trim(), CodCalculo = dat["CodCalculo"].ToString().Trim(), CodPresupuesto = dat["CodPresupuesto"].ToString().Trim(), CodSubpresupuesto = dat["CodSubpresupuesto"].ToString().Trim(), Item = dat["Item"].ToString().Trim(), TipoCampo = dat["TipoCampo"].ToString().Trim(), Campo = dat["Campo"].ToString().Trim(), Operacion = dat["Operacion"].ToString().Trim(), Posicion = dat["Posicion"].ToString().Trim() });
            }


            AdvTree7.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;


        }


        void CargaDetalleItem() {

                //cargamos apus
                SGApus.PrimaryGrid.DeleteAll();
                SGApus.PrimaryGrid.PurgeDeletedRows();
                GridRow fila = new GridRow("", "", "", "", "", "", "", "");
                SGApus.PrimaryGrid.Rows.Add(fila);
                SuperTabItem2.Text = "APU PARTIDA ";

                double equipo = 0.0, material = 0.0, manoObra = 0.0, subcontrato = 0.0, total = 0.0;

                foreach (var Item in ListaItemsAPU)
                {
                    string unid = "";
                    if (Item.Unidad is null) unid = ""; else unid = Item.Unidad;
                    string CuadrillaInsumo = "";
                    if (Item.CuadrillaInsumo is null) CuadrillaInsumo = ""; else CuadrillaInsumo = Item.CuadrillaInsumo.ToString();


                    string Parcial1 = "0.00";
                    if (Item.Parcial1 is null) Parcial1 = "0.00"; else Parcial1 = Item.Parcial1.ToString();
                    Parcial1 = Convert.ToDouble(Parcial1).ToString("N2");

                    string CantidadInsumo = "0.00";
                    if (Item.CantidadInsumo is null) CantidadInsumo = "0.00"; else CantidadInsumo = Item.CantidadInsumo.ToString();
                    CantidadInsumo = Convert.ToDouble(CantidadInsumo).ToString("N2");

                    string PrecioInsumo1 = "0.00";
                    if (Item.PrecioInsumo1 is null) PrecioInsumo1 = "0.00"; else PrecioInsumo1 = Item.PrecioInsumo1.ToString();
                    PrecioInsumo1 = Convert.ToDouble(PrecioInsumo1).ToString("N2");

                    fila = new GridRow("", Item.TipoDetalle.ToString(), Item.Descripcion.ToString(), unid, CuadrillaInsumo, CantidadInsumo, PrecioInsumo1, Parcial1);

                    if (Item.TipoDetalle.ToString() == "Material")
                    {
                        fila.Cells[1].CellStyles.Default.Image = ImageList4.Images[2];
                        material = material + Convert.ToDouble(Parcial1);
                    }

                    if (Item.TipoDetalle.ToString() == "Mano de Obra")
                    {
                        fila.Cells[1].CellStyles.Default.Image = ImageList4.Images[0];
                        manoObra = manoObra + Convert.ToDouble(Parcial1);
                    }

                    if (Item.TipoDetalle.ToString() == "Equipo")
                    {
                        fila.Cells[1].CellStyles.Default.Image = ImageList4.Images[1];
                        equipo = equipo + Convert.ToDouble(Parcial1);
                    }

                    if (Item.TipoDetalle.ToString() == "Subcontrato")
                    {
                        fila.Cells[1].CellStyles.Default.Image = ImageList4.Images[3];
                        subcontrato = subcontrato + Convert.ToDouble(Parcial1);
                    }
                    SGApus.PrimaryGrid.Rows.Add(fila);
                }

                GridRow filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[0];
                filaaz.Cells[2].Value = manoObra.ToString("N2");
                filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[1];
                filaaz.Cells[2].Value = equipo.ToString("N2");
                filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[2];
                filaaz.Cells[2].Value = material.ToString("N2");
                filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[3];
                filaaz.Cells[2].Value = subcontrato.ToString("N2");
                filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[4];
                total = manoObra + equipo + material + subcontrato;
                filaaz.Cells[2].Value = total.ToString("N2");
                //CARGAMOS ASOCIADOS

                SuperTabItem2.Text = "APU PARTIDA " + total.ToString("N2");

                SGAsociados.PrimaryGrid.DeleteAll();
                SGAsociados.PrimaryGrid.PurgeDeletedRows();
                fila = new GridRow("", "", "", "", "", "", "", "");
                SGAsociados.PrimaryGrid.Rows.Add(fila);

                foreach (var Item in ListaAsociados)
                {
                    string opera = "";
                    string opera1 = "";
                    if (Item.Valor != "")
                    {
                        opera = Strings.Mid(Item.Valor, 1, 1);
                        opera1 = Strings.Mid(Item.Valor, 2, Strings.Len(Item.Valor) - 1);
                    }
                    if (opera == "=") opera = "Igual";
                    if (opera == "!") opera = "Diferente";
                    if (opera == "?") opera = "";

                    string cadenaTipo = Item.Tipo.ToString().Replace("''", "'");

                    fila = new GridRow(Item.CodAsociado, "", Item.Categoria, Item.Familia, cadenaTipo, Item.CampoFiltro, opera, opera1);
                    SGAsociados.PrimaryGrid.Rows.Add(fila);
                }


                //CARGAMOS ESTRUCTURAS
                SGestructuraC.PrimaryGrid.DeleteAll();
                SGestructuraC.PrimaryGrid.PurgeDeletedRows();
                fila = new GridRow("", "", "", "", "");
                SGestructuraC.PrimaryGrid.Rows.Add(fila);
                //List<Order> SortedList = objListOrder.OrderBy(o => o.OrderDate).ToList();
                ListaEstructuras = ListaEstructuras.OrderBy(o => o.Nivel).ToList();
                foreach (var Item in ListaEstructuras)
                {
                    bool vmos = false;
                    if (Item.Mostrar == "false") vmos = false;
                    if (Item.Mostrar == "true") vmos = true;
                    fila = new GridRow(Item.CodEstructura, "", Item.Nivel, Item.Campo, vmos);
                    SGestructuraC.PrimaryGrid.Rows.Add(fila);
                }



                //CARGAMOS formulas
                //SGFormulas.PrimaryGrid.DeleteAll();
                //SGFormulas.PrimaryGrid.PurgeDeletedRows();
                fila = (GridRow)SGFormulas.PrimaryGrid.Rows[0];
                for (int X = 0; X < 6; X++)
                    fila.Cells[X].Value = "";

                //SGestructuraC.PrimaryGrid.Rows.Add(fila);
                if ((!(ListaCalculos is null)) && ListaCalculos.Count != 0)
                {
                    fila.Cells[0].Value = ListaCalculos.First().CodCalculo;
                    fila.Cells[1].Value = ListaCalculos.First().Descripcion;
                    fila.Cells[2].Value = ListaCalculos.First().Cantidad;
                    fila.Cells[3].Value = ListaCalculos.First().Longitud;
                    fila.Cells[4].Value = ListaCalculos.First().Ancho;
                    fila.Cells[5].Value = ListaCalculos.First().Alto;

                    fila.Cells[3].ReadOnly = false;
                    fila.Cells[4].ReadOnly = false;
                    fila.Cells[5].ReadOnly = false;

                    if (fila.Cells[3].Value.ToString().Length > 1)
                        if (Microsoft.VisualBasic.Strings.Mid(fila.Cells[3].Value.ToString(), 1, 1) == "=")
                            fila.Cells[3].ReadOnly = true;
                    if (fila.Cells[4].Value.ToString().Length > 1)
                        if (Microsoft.VisualBasic.Strings.Mid(fila.Cells[4].Value.ToString(), 1, 1) == "=")
                            fila.Cells[4].ReadOnly = true;
                    if (fila.Cells[5].Value.ToString().Length > 1)
                        if (Microsoft.VisualBasic.Strings.Mid(fila.Cells[5].Value.ToString(), 1, 1) == "=")
                            fila.Cells[5].ReadOnly = true;

                }


                CargarMediciones();

                CargaPropiedades();



        }


        private void timer10_Tick(object sender, EventArgs e)
        {
         textBox3.Text = cadenaObtenida;
 
         if (textBox1.Text != "" && ListaItems.Count != 0)
         {
             timer10.Enabled = false;
 //cargamos apus
                SGApus.PrimaryGrid.DeleteAll();
                SGApus.PrimaryGrid.PurgeDeletedRows();
                GridRow fila = new GridRow("", "", "", "", "", "", "", "");
                SGApus.PrimaryGrid.Rows.Add(fila);
                SuperTabItem2.Text = "APU PARTIDA ";

                double equipo = 0.0, material = 0.0, manoObra = 0.0, subcontrato = 0.0, total = 0.0;
                
                foreach (var Item in ListaItemsAPU)
                {
                 string unid = "";
                 if (Item.Unidad is null) unid = ""; else unid = Item.Unidad;
                    string CuadrillaInsumo = "";
                    if (Item.CuadrillaInsumo is null) CuadrillaInsumo = ""; else CuadrillaInsumo = Item.CuadrillaInsumo.ToString();


                    string Parcial1 = "0.00";
                    if (Item.Parcial1 is null) Parcial1 = "0.00"; else Parcial1 = Item.Parcial1.ToString();
                    Parcial1 = Convert.ToDouble(Parcial1).ToString("N2");

                    string CantidadInsumo = "0.00";
                    if (Item.CantidadInsumo is null) CantidadInsumo = "0.00"; else CantidadInsumo = Item.CantidadInsumo.ToString();
                    CantidadInsumo = Convert.ToDouble(CantidadInsumo).ToString("N2");

                    string PrecioInsumo1 = "0.00";
                    if (Item.PrecioInsumo1 is null) PrecioInsumo1 = "0.00"; else PrecioInsumo1 = Item.PrecioInsumo1.ToString();
                    PrecioInsumo1 = Convert.ToDouble(PrecioInsumo1).ToString("N2");

                    fila = new GridRow("", Item.TipoDetalle.ToString(), Item.Descripcion.ToString(), unid, CuadrillaInsumo, CantidadInsumo, PrecioInsumo1, Parcial1);

                    if (Item.TipoDetalle.ToString() == "Material") {
                        fila.Cells[1].CellStyles.Default.Image = ImageList4.Images[2];
                        material = material + Convert.ToDouble(Parcial1);
                    }

                    if (Item.TipoDetalle.ToString() == "Mano de Obra") {
                        fila.Cells[1].CellStyles.Default.Image = ImageList4.Images[0];
                        manoObra = manoObra + Convert.ToDouble(Parcial1);
                    }

                    if (Item.TipoDetalle.ToString() == "Equipo") {
                        fila.Cells[1].CellStyles.Default.Image = ImageList4.Images[1];
                        equipo = equipo + Convert.ToDouble(Parcial1);
                    }

                    if (Item.TipoDetalle.ToString() == "Subcontrato") {
                        fila.Cells[1].CellStyles.Default.Image = ImageList4.Images[3];
                        subcontrato = subcontrato + Convert.ToDouble(Parcial1);
                    }
                    SGApus.PrimaryGrid.Rows.Add(fila);
             }

                GridRow filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[0];
                filaaz.Cells[2].Value = manoObra.ToString("N2");
                filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[1];
                filaaz.Cells[2].Value = equipo.ToString("N2");
                filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[2];
                filaaz.Cells[2].Value = material.ToString("N2");
                filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[3];
                filaaz.Cells[2].Value = subcontrato.ToString("N2");
                filaaz = (GridRow)SGTotRubro.PrimaryGrid.Rows[4];
                total = manoObra + equipo + material + subcontrato;
                filaaz.Cells[2].Value = total.ToString("N2");
                //CARGAMOS ASOCIADOS

                SuperTabItem2.Text= "APU PARTIDA " + total.ToString("N2");

                /*SGAsociados.PrimaryGrid.DeleteAll();
                SGAsociados.PrimaryGrid.PurgeDeletedRows();
                fila = new GridRow("", "", "", "", "", "", "", "");
                SGAsociados.PrimaryGrid.Rows.Add(fila);

                foreach (var Item in ListaAsociados)
                {
                    string opera = "";
                    string opera1 = "";
                    if (Item.Valor != "") {
                        opera = Strings.Mid(Item.Valor, 1, 1);
                        opera1 = Strings.Mid(Item.Valor, 2, Strings.Len(Item.Valor)-1);
                    }
                    if (opera == "=") opera = "Igual";
                    if (opera == "!") opera = "Diferente";
                    if (opera == "?") opera = "";

                    string cadenaTipo = Item.Tipo.ToString().Replace("''", "'");

                    fila = new GridRow(Item.CodAsociado, "", Item.Categoria, Item.Familia, cadenaTipo, Item.CampoFiltro, opera, opera1);
                    SGAsociados.PrimaryGrid.Rows.Add(fila);
                }




                //CARGAMOS ESTRUCTURAS
                SGestructuraC.PrimaryGrid.DeleteAll();
                SGestructuraC.PrimaryGrid.PurgeDeletedRows();
                fila = new GridRow("", "", "", "", "");
                SGestructuraC.PrimaryGrid.Rows.Add(fila);
                //List<Order> SortedList = objListOrder.OrderBy(o => o.OrderDate).ToList();
                ListaEstructuras = ListaEstructuras.OrderBy(o => o.Nivel).ToList();
                foreach (var Item in ListaEstructuras)
                {
                    bool vmos = false;
                    if (Item.Mostrar == "false") vmos = false;
                    if (Item.Mostrar == "true") vmos = true;
                    fila = new GridRow(Item.CodEstructura, "", Item.Nivel, Item.Campo, vmos);
                    SGestructuraC.PrimaryGrid.Rows.Add(fila);
                }



                //CARGAMOS formulas
                //SGFormulas.PrimaryGrid.DeleteAll();
                //SGFormulas.PrimaryGrid.PurgeDeletedRows();
                fila = (GridRow) SGFormulas.PrimaryGrid.Rows[0];
                for (int X = 0; X < 6; X++)
                    fila.Cells[X].Value = "";

                //SGestructuraC.PrimaryGrid.Rows.Add(fila);
                if ((!(ListaCalculos is null)) && ListaCalculos.Count!=0) {
                    fila.Cells[0].Value = ListaCalculos.First().CodCalculo;
                    fila.Cells[1].Value = ListaCalculos.First().Descripcion;
                    fila.Cells[2].Value = ListaCalculos.First().Cantidad;
                    fila.Cells[3].Value = ListaCalculos.First().Longitud;
                    fila.Cells[4].Value = ListaCalculos.First().Ancho;
                    fila.Cells[5].Value = ListaCalculos.First().Alto;

                    fila.Cells[3].ReadOnly = false;
                    fila.Cells[4].ReadOnly = false;
                    fila.Cells[5].ReadOnly = false;

                    if (fila.Cells[3].Value.ToString().Length > 1)
                        if (Microsoft.VisualBasic.Strings.Mid(fila.Cells[3].Value.ToString(),1,1)=="=")
                            fila.Cells[3].ReadOnly = true;
                    if (fila.Cells[4].Value.ToString().Length > 1)
                        if (Microsoft.VisualBasic.Strings.Mid(fila.Cells[4].Value.ToString(), 1, 1) == "=")
                            fila.Cells[4].ReadOnly = true;
                    if (fila.Cells[5].Value.ToString().Length > 1)
                        if (Microsoft.VisualBasic.Strings.Mid(fila.Cells[5].Value.ToString(), 1, 1) == "=")
                            fila.Cells[5].ReadOnly = true;

                }


                CargarMediciones();

                CargaPropiedades();*/

            }





        }

        private void SGPresupC_Click(object sender, EventArgs e)
        {

        }




        private void CargarMediciones()
        {
            GridRow filaauxPadre = (GridRow)SGPresupC.PrimaryGrid.ActiveRow;
            if (filaauxPadre is null) return;


            do
            {
                 SGMediciones.PrimaryGrid.DeleteAll();
                 SGMediciones.PrimaryGrid.PurgeDeletedRows();
            }while (SGMediciones.PrimaryGrid.Rows.Count() != 0);
            //GridRow filaauxP = new GridRow("", "", "", "", "", "","", "", "", "", "", "", "");
            //filaauxP.ReadOnly = true;
            //SGMediciones.PrimaryGrid.Rows.Add(filaauxP);


            double CalculoTotal = 0;
            var ItemsNive1Pers = new List<LMedicion>();
            try { ItemsNive1Pers = ListaItemsMedicion.Where(X => (X.Nivel == 1 && X.Vinculo == "Personalizado")).ToList();
            }catch{}

            foreach (var Item in ItemsNive1Pers)
            {
                string AuxDesc = "";
                string Uniques = Item.UniqueId.ToString();
                if (Item.Descripcion is null) AuxDesc = ""; else AuxDesc = Item.Descripcion;
                GridRow fila = new GridRow(Item.CodMedicion.ToString(), Item.UniqueId.ToString(), AuxDesc, Item.Cantidad.ToString(), Item.Longitud.ToString(), Item.Ancho.ToString(), Item.Alto.ToString(), Item.Total.ToString(), Item.Detalle.ToString(), Item.Vinculo.ToString(), Item.Tipo.ToString(), Item.PhantomParentId.ToString(), Item.Nivel.ToString());
                fila.ReadOnly = false;
                SGMediciones.PrimaryGrid.Rows.Add(fila);
                Uniques = cargar_hijos_itemsMediciones(Item.CodMedicion.ToString(), fila);
                fila.Expanded = true;
                if (Item.Tipo.ToString() == "Titulo")
                {
                    var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(202, 223, 218), 90);
                    fila.CellStyles.Default.Background = Background;
                    fila.ReadOnly = true;
                }
                else {
                    var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                    fila.CellStyles.Default.Background = Background;
                    fila.ReadOnly = false;
                }
                CalculoTotal = CalculoTotal + Convert.ToDouble(Item.Total);
            }



            var ItemsNive1 = new List<LMedicion>();
            try{ ItemsNive1 = ListaItemsMedicion.Where(X => (X.Nivel == 1 && X.Vinculo != "Personalizado")).ToList();
            }catch {}
            
            foreach (var Item in ItemsNive1)
                {
                string AuxDesc = "";
                string Uniques = Item.UniqueId.ToString();
                if (Item.Descripcion is null) AuxDesc = ""; else AuxDesc = Item.Descripcion;
                    GridRow fila = new GridRow(Item.CodMedicion.ToString(), Item.UniqueId.ToString(), AuxDesc, Item.Cantidad.ToString(), Item.Longitud.ToString(), Item.Ancho.ToString(), Item.Alto.ToString(), Item.Total.ToString(), Item.Detalle.ToString(), Item.Vinculo.ToString(), Item.Tipo.ToString(), Item.PhantomParentId.ToString(), Item.Nivel.ToString());
                fila.ReadOnly = true;
                SGMediciones.PrimaryGrid.Rows.Add(fila);
                Uniques=cargar_hijos_itemsMediciones(Item.CodMedicion.ToString(), fila);
                fila.Expanded = true;
                if (Item.Tipo.ToString() == "Titulo")
                {
                    fila.ReadOnly = true;
                    var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(202, 223, 218), 90);
                    fila.CellStyles.Default.Background = Background;
                }
                CalculoTotal = CalculoTotal + Convert.ToDouble(Item.Total);
             }


            string dato = "";
            foreach (GridRow Itfila in SGMediciones.PrimaryGrid.Rows) {
                if (!(Itfila.Cells[1].Value is null))
                if (Itfila.Cells[1].Value.ToString() == "") {
                    dato=buscar_en_hijos(Itfila);
                    Itfila.Cells[1].Value=dato;
                }
            }
            
            filaauxPadre.Cells[4].Value = CalculoTotal.ToString("N2");
            SuperTabItem5.Text="METRADO " + CalculoTotal.ToString("N2");
            recalcular_pres_desdeFila(filaauxPadre);
            if (CalculoTotal > 0)
                GuardarMetrado(CalculoTotal.ToString());

        }

        private string buscar_en_hijos(GridRow filap) {
            string dato = "";
            string dato1 = "";
            foreach (GridRow Itfila in filap.Rows)
            {
                if (Itfila.Cells[1].Value.ToString() == "")
                {
                    dato1 = buscar_en_hijos(Itfila);
                    Itfila.Cells[1].Value = dato1;
                    //return dato1;
                    if (dato == "") dato = dato1;
                    else dato = dato + "," + dato1;

                }
                else
                    if (dato=="") dato = Itfila.Cells[1].Value.ToString();
                    else dato = dato + "," +Itfila.Cells[1].Value.ToString();
            }
            return dato;
        }


        private string cargar_hijos_itemsMediciones(string padre, GridRow filap)
        {
            var ItemsNive2 = new List<LMedicion>();

            string CadenaUniques = "";
            string Uniques = "";
            try
            {
                ItemsNive2 = ListaItemsMedicion.Where(X => X.PhantomParentId == padre).ToList();
            }
            catch { 
            }
            
             foreach (var Item in ItemsNive2)
            {
                string AuxDesc = "";
                if (Item.Descripcion is null) AuxDesc = ""; else AuxDesc = Item.Descripcion;
                GridRow fila = new GridRow(Item.CodMedicion.ToString(), Item.UniqueId.ToString(), AuxDesc, Item.Cantidad.ToString(), Item.Longitud.ToString(), Item.Ancho.ToString(), Item.Alto.ToString(), Item.Total.ToString(), Item.Detalle.ToString(), Item.Vinculo.ToString(), Item.Tipo.ToString(), Item.PhantomParentId.ToString(), Item.Nivel.ToString());
                if (Item.Vinculo.ToString() != "Personalizado")
                {
                    fila.ReadOnly = true;
                }
                else
                {
                    fila.ReadOnly = false;
                    fila.Cells[2].ReadOnly = false;
                    if (Item.Tipo.ToString() != "Titulo")
                    {
                        var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                        fila.CellStyles.Default.Background = Background;
                        //fila.Cells[5].ReadOnly = true;
                        fila.ReadOnly = false;
                    }
                    else
                    {
                        fila.Cells[2].ReadOnly = false;
                    }
                    
                }

                filap.Rows.Add(fila);

                if (Item.CodMedicion.ToString() != "")
                    Uniques = cargar_hijos_itemsMediciones(Item.CodMedicion.ToString(), fila);
                fila.Expanded = true;
                /*if (Item.UniqueId.ToString() != "")
                {
                 if (CadenaUniques=="")
                    CadenaUniques = Item.UniqueId.ToString();
                 else
                    CadenaUniques = CadenaUniques + "," + Item.UniqueId.ToString();
                }
                if (Item.UniqueId.ToString() == "")
                {
                    fila.Cells[1].Value = Uniques;
                }*/

                if (Item.Tipo.ToString() == "Titulo")
                {
                    var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(202, 223, 218), 90);
                    fila.CellStyles.Default.Background = Background;
                    fila.ReadOnly = true;
                    if (Item.Vinculo.ToString() == "Personalizado") {
                        fila.ReadOnly = false;
                        for (int x=3;x<8;x++)
                            fila.Cells[x].ReadOnly = true;
                        fila.Cells[2].ReadOnly = false;
                    }
                }

             
            }
            return CadenaUniques;


        }



        


        private void ponerCampos(GridRow FilaAux)
        {
                foreach (var dato in ElementosFiltro)
                {
                if (FilaAux.Cells[5].Value.ToString() != "" && FilaAux.Cells[6].Value.ToString() != "" && FilaAux.Cells[7].Value.ToString() != "")
                {
                    string UNIQUEID = dato.UniqueId;
                    Element ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                    Autodesk.Revit.DB.ElementId currentid = ELEMENTO.Id;
                    string VCAMPO = "";
                    IList<Autodesk.Revit.DB.Parameter> LiParametros;
                    LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(FilaAux.Cells[5].Value.ToString());
                    foreach (var dato1 in LiParametros)
                    {
                        VCAMPO = dato1.AsString();
                    }
                    ElementosFiltroCampo.Add(new RevitElementoBaseCampo { Id = dato.Id, Categoria = dato.Categoria, Familia = dato.Familia, Tipo = dato.Tipo, UniqueId = dato.UniqueId, Campo = FilaAux.Cells[5].Value.ToString(), Valor = VCAMPO });
                }else
                    ElementosFiltroCampo.Add(new RevitElementoBaseCampo { Id = dato.Id, Categoria = dato.Categoria, Familia = dato.Familia, Tipo = dato.Tipo, UniqueId = dato.UniqueId, Campo = "", Valor = "" });
            }

        }

        private void FiltarXCategoria(string Cat, GridRow FilaAux)
        {
            if (TotalElementos is null) return;
            //var AuxFiltro = new List<RevitElementoBase>();
            //AuxFiltro = TotalElementos.Where(X => X.Categoria == Cat).ToList();
            ElementosFiltro = TotalElementos.Where(X => X.Categoria == Cat).ToList();
            //ElementosFiltro.AddRange(AuxFiltro);
            ponerCampos(FilaAux);
            if (FilaAux.Cells[5].Value.ToString() != "" && FilaAux.Cells[6].Value.ToString() != "" && FilaAux.Cells[7].Value.ToString() != "")
            {
                //ElementosFiltroCampo = new List<RevitElementoBaseCampo>();
                if (FilaAux.Cells[6].Value.ToString() == "Igual")
                {
                    ElementosFiltroCampo = ElementosFiltroCampo.Where(X => (X.Campo == FilaAux.Cells[5].Value.ToString() && X.Valor == FilaAux.Cells[7].Value.ToString())).ToList();
                }else {
                    ElementosFiltroCampo = ElementosFiltroCampo.Where(X => (X.Campo == FilaAux.Cells[5].Value.ToString() && X.Valor != FilaAux.Cells[7].Value.ToString())).ToList();
                }
            }

        }
        private void FiltarXCategoriaFamilia(string Cat, string Fam, GridRow FilaAux)
        {
            ElementosFiltro = TotalElementos.Where(X => X.Categoria == Cat && X.Familia == Fam).ToList();
            ponerCampos(FilaAux);
            if (FilaAux.Cells[5].Value.ToString() != "" && FilaAux.Cells[6].Value.ToString() != "" && FilaAux.Cells[7].Value.ToString() != "")
            {
                if (FilaAux.Cells[6].Value.ToString() == "Igual")
                    ElementosFiltroCampo = ElementosFiltroCampo.Where(X => (X.Campo == FilaAux.Cells[5].Value.ToString() && X.Valor == FilaAux.Cells[7].Value.ToString())).ToList();
                else
                    ElementosFiltroCampo = ElementosFiltroCampo.Where(X => (X.Campo == FilaAux.Cells[5].Value.ToString() && X.Valor != FilaAux.Cells[7].Value.ToString())).ToList();
            }

        }
        private void FiltarXCategoriaFamiliaTipo(string Cat, string Fam, string Tip, GridRow FilaAux)
        {
            ElementosFiltro = TotalElementos.Where(X => X.Categoria == Cat && X.Familia == Fam && X.Tipo == Tip).ToList();
            ponerCampos(FilaAux);
            if (FilaAux.Cells[5].Value.ToString() != "" && FilaAux.Cells[6].Value.ToString() != "" && FilaAux.Cells[7].Value.ToString() != "")
            {
                if (FilaAux.Cells[6].Value.ToString() == "Igual")
                    ElementosFiltroCampo = ElementosFiltroCampo.Where(X => (X.Campo == FilaAux.Cells[5].Value.ToString() && X.Valor == FilaAux.Cells[7].Value.ToString())).ToList();
                else
                    ElementosFiltroCampo = ElementosFiltroCampo.Where(X => (X.Campo == FilaAux.Cells[5].Value.ToString() && X.Valor != FilaAux.Cells[7].Value.ToString())).ToList();
            }
        }


        private double CALCULA_FACTOR(string DATO) {
            double FACTOR=1.0;
            if (Strings.Mid(DATO, Strings.Len(DATO) - 1, 2) == " m") FACTOR = 3.281;
            if (Strings.Mid(DATO, Strings.Len(DATO) - 1, 2) == "m²") FACTOR = 10.764;
            if (Strings.Mid(DATO, Strings.Len(DATO) - 1, 2) == "m³") FACTOR = 35.315;
            return FACTOR;
        }
        private double CALCULA_FACTOR1(string DATO)
        {
            double FACTOR = 1.0;
            if (Strings.Mid(DATO, Strings.Len(DATO) - 1, 2) == "mm") FACTOR = 305;
            return FACTOR;
        }


        private void GenerarMedicion()
        {

            if (Item_actual == "") {
                System.Windows.Forms.MessageBox.Show("Debe seleccionar la partida (posicionese en la partida que desea metrar) ", "Error");
                return;
            }




            //validaciones
            //QUE TENGA ASOCIADO
            var ErrorenFormulas = new string[20];
            int Cuenta_ErrorenFormulas = 0;
            
            ListaItemsMedicion = new List<LMedicion>();




            ElementosFiltro = new List<RevitElementoBase>();
            ElementosFiltroCampo = new List<RevitElementoBaseCampo>();

            
            GridPanel panelAsociados = (GridPanel)SGAsociados.PrimaryGrid;
            GridPanel panelNiveles = (GridPanel)SGestructuraC.PrimaryGrid;
            GridPanel panelFormulas = (GridPanel)SGFormulas.PrimaryGrid;
            //GridPanel panelMediciones = (GridPanel)SGMediciones.PrimaryGrid;

            GridRow FilaAsoc = (GridRow)SGAsociados.PrimaryGrid.Rows[0];
            GridRow FilaFormula = (GridRow)SGFormulas.PrimaryGrid.Rows[0];

            if (SGAsociados.PrimaryGrid.Rows.Count() == 1) {
                System.Windows.Forms.MessageBox.Show("Debe seleccionar un elmento asociado", "Error");
                return;
            }
            if (FilaAsoc is null) {
                System.Windows.Forms.MessageBox.Show("Debe seleccionar un elmento asociado", "Error");
                return;
            }
            if (FilaAsoc.Cells[2].Value.ToString() == "") {
                System.Windows.Forms.MessageBox.Show("Debe seleccionar un elmento asociado", "Error");
                return;
            }

            if (FilaFormula.Cells[1].Value is null) FilaFormula.Cells[1].Value = "";
            
            if (FilaFormula.Cells[2].Value is null) FilaFormula.Cells[2].Value = "";
            if (FilaFormula.Cells[3].Value is null) FilaFormula.Cells[3].Value = "";
            if (FilaFormula.Cells[4].Value is null) FilaFormula.Cells[4].Value = "";
            if (FilaFormula.Cells[5].Value is null) FilaFormula.Cells[5].Value = "";

            //QUE TENGA FORMULA
            if (FilaFormula.Cells[2].Value.ToString() == "" && FilaFormula.Cells[3].Value.ToString() == "" && FilaFormula.Cells[4].Value.ToString() == "" && FilaFormula.Cells[5].Value.ToString() == "")
            {
                System.Windows.Forms.MessageBox.Show("No esta definido el campo para cálculo", "Error");
                return;
            }

            System.Data.DataTable datosPl = new System.Data.DataTable();
            ConexionBD bdatos = new ConexionBD();
            datosPl = bdatos.LPlanos(labelItem8.Text);
            string codModelo = "";
            if (datosPl.Rows.Count == 0) return;
            else
                codModelo = datosPl.Rows[0]["CodPlano"].ToString().Trim();



            



            //recorro cada clasificacion
            for (int x = 0; x < panelAsociados.Rows.Count() - 1; x++) {
                
                

                GridRow FilaAux = (GridRow)panelAsociados.Rows[x];
                for (int i = 2; i <= 7; i++) {
                    if (FilaAux.Cells[i].Value is null) FilaAux.Cells[i].Value = "";
                }
                if (FilaAux.Cells[4].Value.ToString() != "")
                {
                    FiltarXCategoriaFamiliaTipo(FilaAux.Cells[2].Value.ToString(), FilaAux.Cells[3].Value.ToString(), FilaAux.Cells[4].Value.ToString(), FilaAux);
                }
                else if (FilaAux.Cells[3].Value.ToString() != "")
                {
                    FiltarXCategoriaFamilia(FilaAux.Cells[2].Value.ToString(), FilaAux.Cells[3].Value.ToString(), FilaAux);
                }
                else {
                    FiltarXCategoria(FilaAux.Cells[2].Value.ToString(), FilaAux);
                }
                //MessageBox.Show("" + x.ToString(),"");     
            }


            //generar niveles
            for (int x = 0; x < panelNiveles.Rows.Count() - 1; x++)
            {
                GridRow FilaAux = (GridRow)panelNiveles.Rows[x];
                int NivelActual = x+1;
                foreach (var dato in ElementosFiltroCampo)
                {
                    string UNIQUEID = dato.UniqueId;
                    Element ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                    Autodesk.Revit.DB.ElementId currentid = ELEMENTO.Id;
                    string VCAMPO = "SN";
                    IList<Autodesk.Revit.DB.Parameter> LiParametros;
                    LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(FilaAux.Cells[3].Value.ToString());
                    foreach (var dato1 in LiParametros) {
                        VCAMPO = dato1.AsValueString();
                        break;
                    }
                    if (VCAMPO == "") VCAMPO = "SN";
                    if (VCAMPO is null) VCAMPO = "SN";                    
                    if (FilaAux.Cells[4].Value is null) FilaAux.Cells[4].Value = false;
                    if (FilaAux.Cells[4].Value == "") FilaAux.Cells[4].Value = false;
                    if ((bool)FilaAux.Cells[4].Value == true) VCAMPO = FilaAux.Cells[3].Value.ToString() + " " + VCAMPO;
                    
                    if (NivelActual == 1)
                    {
                        var AuxFiltro = new List<LMedicion>();
                        AuxFiltro = ListaItemsMedicion.Where(X => X.Descripcion == VCAMPO).ToList();

                        if (AuxFiltro.Count() == 0) {
                            string CodigoUni = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                            ListaItemsMedicion.Add(new LMedicion { CodMedicion = CodigoUni, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = VCAMPO, Cantidad = "", Longitud = "", Ancho = "", Alto = "", Total = "", Detalle = "Titulo de Nivel " + NivelActual.ToString(), Vinculo = codModelo, UniqueId = "", PhantomParentId = "", Nivel = NivelActual, Tipo = "Titulo" });
                            //GuardarMedicion(CodigoUni, VCAMPO, "", "", "", "", "", "Titulo de Nivel " + NivelActual.ToString(), "", "", "", NivelActual, "Titulo");
                        }
                            
                    }
                    else {
                        var AuxFiltroT = new List<LMedicion>();
                        AuxFiltroT = ListaItemsMedicion.Where(X => X.Nivel == NivelActual-1).ToList();
                        foreach (var datoAux in AuxFiltroT) {
                            var AuxFiltro = new List<LMedicion>();
                            try {
                                AuxFiltro = ListaItemsMedicion.Where(X => X.Descripcion == VCAMPO && X.PhantomParentId == datoAux.CodMedicion).ToList();
                            } catch { }                            
                            if (AuxFiltro.Count() == 0) {
                                string CodigoUni = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                                ListaItemsMedicion.Add(new LMedicion { CodMedicion = CodigoUni, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = VCAMPO, Cantidad = "", Longitud = "", Ancho = "", Alto = "", Total = "", Detalle = "Titulo de Nivel " + NivelActual.ToString(), Vinculo = codModelo, UniqueId = "", PhantomParentId = datoAux.CodMedicion, Nivel = NivelActual, Tipo = "Titulo" });
                                //GuardarMedicion(CodigoUni, VCAMPO, "", "", "", "", "", "Titulo de Nivel " + NivelActual.ToString(), "", "", datoAux.CodMedicion, NivelActual, "Titulo");
                            }
                                
                        }
                    }
                }

            }


            progressBar1.Maximum = ElementosFiltroCampo.Count() - 1;
            progressBar1.Value = 0;
            progressBar1.Refresh();
            int posi = 0;

            



            //int auxcod = 0;
            foreach (var dato in ElementosFiltroCampo) {


                progressBar1.Value = posi;
                progressBar1.Refresh();
                posi++;

                string UNIQUEID = dato.UniqueId;
                Element ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                Autodesk.Revit.DB.ElementId currentid = ELEMENTO.Id;
                //BUSCAMOS LA UBICACION DEL ELEMENTO EN LA CLASIFICACION ESCOGIDA
                string PADREANTERIOR = "";
                int NivelActual = 0;
                for (int x = 0; x < panelNiveles.Rows.Count() - 1; x++)
                {
                    GridRow FilaAux = (GridRow)panelNiveles.Rows[x];
                    NivelActual = x + 1;
                    string VCAMPO = "SN";
                    IList<Autodesk.Revit.DB.Parameter> LiParametros;
                    LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(FilaAux.Cells[3].Value.ToString());
                    foreach (var dato1 in LiParametros) {
                        VCAMPO = dato1.AsValueString();
                        break;
                    }
                    if (VCAMPO == "") VCAMPO = "SN";
                    if (VCAMPO is null) VCAMPO = "SN";
                    if (FilaAux.Cells[4].Value is null) FilaAux.Cells[4].Value = false;
                    if ((bool)FilaAux.Cells[4].Value == true) VCAMPO = FilaAux.Cells[3].Value.ToString() + " " + VCAMPO;

                    if (PADREANTERIOR == "")
                    {
                        var AuxFiltro = new List<LMedicion>();
                        AuxFiltro = ListaItemsMedicion.Where(X => X.Descripcion == VCAMPO).ToList();
                        if (AuxFiltro.Count()!=0)
                        PADREANTERIOR = AuxFiltro.First().CodMedicion;
                    }
                    else {
                        var AuxFiltro = new List<LMedicion>();
                        AuxFiltro = ListaItemsMedicion.Where(X => X.Descripcion == VCAMPO && X.PhantomParentId == PADREANTERIOR).ToList();
                        PADREANTERIOR = AuxFiltro.First().CodMedicion;
                    }

                }

                //VERIFICAMOS SI TIENE CAMPO DE DESCRIPCION
                string DESCRI = "";
                if (FilaFormula.Cells[1].Value is null) FilaFormula.Cells[1].Value = "";
                if (FilaFormula.Cells[1].Value.ToString() != "") {
                    UNIQUEID = dato.UniqueId;
                    ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                    currentid = ELEMENTO.Id;
                    IList<Autodesk.Revit.DB.Parameter> LiParametros;
                    LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(FilaFormula.Cells[1].Value.ToString());
                    if (LiParametros is null) DESCRI = "";
                    else DESCRI = LiParametros.First().AsString();
                }

                //VERIFICAMOS VALOR DE CANTIDAD
                string CANTI = "1.00";
                if (FilaFormula.Cells[2].Value is null) FilaFormula.Cells[2].Value = "";
                if (FilaFormula.Cells[2].Value.ToString() != "")
                {
                    double numero = 1;
                    if (!double.TryParse(FilaFormula.Cells[2].Value.ToString(), out numero))
                    {
                        //MessageBox.Show("no es un numero valido");
                        UNIQUEID = dato.UniqueId;
                        ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                        currentid = ELEMENTO.Id;
                        IList<Autodesk.Revit.DB.Parameter> LiParametros;
                        LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(FilaFormula.Cells[2].Value.ToString());
                        if (LiParametros is null || LiParametros.Count() == 0) CANTI = "1.00";
                        else {
                            if (LiParametros.First().AsValueString() == null || LiParametros.First().AsValueString() == "" || LiParametros.First().AsValueString().Length <= 2) {

                                //aqui hay que validar cuando el campo no es numerico
                                CANTI = "0.00";
                            }
                            else
                            CANTI = (LiParametros.First().AsDouble() * CALCULA_FACTOR1(LiParametros.First().AsValueString()) / CALCULA_FACTOR(LiParametros.First().AsValueString())).ToString("N3");
                        }
                        
                        if (CANTI is null) CANTI = "1.00";
                    }
                    else
                        CANTI = numero.ToString("N2");
                }



                /*If Mid(DATO.AsValueString.ToString, Len(DATO.AsValueString.ToString) -1, 2) = " m" Then FACTOR = 3.281
                  If Mid(DATO.AsValueString.ToString, Len(DATO.AsValueString.ToString) -1, 2) = "m²" Then FACTOR = 10.764
                  If Mid(DATO.AsValueString.ToString, Len(DATO.AsValueString.ToString) -1, 2) = "m³" Then FACTOR = 35.315
                  If Mid(DATO.AsValueString.ToString, Len(DATO.AsValueString.ToString) -1, 2) = "mm" Then FACTOR1 = 305*/

                //VERIFICAMOS VALOR DE longitud
                string LONGITUD = "";
                if (FilaFormula.Cells[3].Value is null) FilaFormula.Cells[3].Value = "";

                if (FilaFormula.Cells[3].Value.ToString() != "" && FilaFormula.Cells[3].Value.ToString()[0].ToString() != "=")
                {
                    double numero = 1;
                    if (!double.TryParse(FilaFormula.Cells[3].Value.ToString(), out numero))
                    {
                        //MessageBox.Show("no es un numero valido");
                        UNIQUEID = dato.UniqueId;
                        ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                        currentid = ELEMENTO.Id;
                        IList<Autodesk.Revit.DB.Parameter> LiParametros;
                        LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(FilaFormula.Cells[3].Value.ToString());
                        if (LiParametros is null || LiParametros.Count() == 0) LONGITUD = "";
                        else {

                            if (LiParametros.First().AsValueString() == null || LiParametros.First().AsValueString() == "" || LiParametros.First().AsValueString().Length <= 2)
                            {

                                //aqui hay que validar cuando el campo no es numerico
                                CANTI = "0.00";
                            }
                            else
                                LONGITUD = (LiParametros.First().AsDouble() * CALCULA_FACTOR1(LiParametros.First().AsValueString()) / CALCULA_FACTOR(LiParametros.First().AsValueString())).ToString("N3");

                        } 
                        if (LONGITUD is null) LONGITUD = "";
                    }
                    else
                        LONGITUD = numero.ToString("N3");
                }
                else if (FilaFormula.Cells[3].Value.ToString() != "" && FilaFormula.Cells[3].Value.ToString()[0].ToString() == "=")
                {
                    //CALCULAR CON FORMULA EN LONGITUD
                    //MessageBox.Show("Es igual el volumen", "");
                    //OBTENGO LOS DATOS DE MI FORMULA
                    //ListaCalculoDetalle.RemoveAll(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Longitud");

                    GridPanel panel1 = (GridPanel)SGFormulas.PrimaryGrid;
                    GridRow filaP = (GridRow)panel1.Rows[0];
                    List<LCalculoDetalle> listaDetFormulaFitrada = ListaCalculoDetalle.Where(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Longitud").ToList();
                    double CalculoTotal = 0;
                    string OperacionCadena = "";
                    string OperacionActual = "";
                    foreach (var datoF in listaDetFormulaFitrada)
                    {
                        
                        string CalculoCadena = "";
                        if (datoF.Operacion is null) datoF.Operacion = "";
                        OperacionCadena = datoF.Operacion;
                        double numeroAux = 1;
                        if (!double.TryParse(datoF.Campo, out numeroAux)) {
                            UNIQUEID = dato.UniqueId;
                            ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                            currentid = ELEMENTO.Id;
                            IList<Autodesk.Revit.DB.Parameter> LiParametros;
                            LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(datoF.Campo);
                            if (LiParametros is null || LiParametros.Count() == 0) CalculoCadena = "";
                            else {

                                if (LiParametros.First().AsValueString() == null || LiParametros.First().AsValueString() == "" || LiParametros.First().AsValueString().Length <= 2)
                                {

                                    //aqui hay que validar cuando el campo no es numerico
                                    CalculoCadena = "0.00";
                                }
                                else
                                    CalculoCadena = (LiParametros.First().AsDouble() * CALCULA_FACTOR1(LiParametros.First().AsValueString()) / CALCULA_FACTOR(LiParametros.First().AsValueString())).ToString("N3");

                            } 
                            if (CalculoCadena is null) CalculoCadena = "";
                        }
                        else
                            CalculoCadena = numeroAux.ToString("N3");

                        if (OperacionActual == "" && OperacionCadena != "")
                        {
                            if (CalculoCadena == "") CalculoCadena = "0.00";
                            CalculoTotal = Convert.ToDouble(CalculoCadena);
                            OperacionActual = OperacionCadena;
                        }
                        else {
                            if (CalculoCadena == "") CalculoCadena = "0.00";
                            switch (OperacionActual) {
                                case "+":
                                    CalculoTotal = CalculoTotal + Convert.ToDouble(CalculoCadena);
                                    break;
                                case "-":
                                    CalculoTotal = CalculoTotal - Convert.ToDouble(CalculoCadena);
                                    break;
                                case "*":
                                    CalculoTotal = CalculoTotal * Convert.ToDouble(CalculoCadena);
                                    break;
                                case "/":
                                    CalculoTotal = CalculoTotal / Convert.ToDouble(CalculoCadena);
                                    break;
                            }
                            OperacionActual = OperacionCadena;
                        }

                        //nuevafila = new GridRow(dato.CodDetalleCalculo, datoF.CodCalculo, datoF.Campo, datoF.Operacion);
                        //panel.Rows.Add(nuevafila);
                    }
                    LONGITUD = CalculoTotal.ToString("N3");

                }


                //ANCHO
                string ANCHO = "";
                if (FilaFormula.Cells[4].Value is null) FilaFormula.Cells[4].Value = "";

                if (FilaFormula.Cells[4].Value.ToString() != "" && FilaFormula.Cells[4].Value.ToString()[0].ToString() != "=")
                {
                    double numero = 1;
                    if (!double.TryParse(FilaFormula.Cells[4].Value.ToString(), out numero))
                    {
                        //MessageBox.Show("no es un numero valido");
                        UNIQUEID = dato.UniqueId;
                        ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                        currentid = ELEMENTO.Id;
                        IList<Autodesk.Revit.DB.Parameter> LiParametros;
                        LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(FilaFormula.Cells[4].Value.ToString());
                        if (LiParametros is null || LiParametros.Count() == 0) ANCHO = "";
                        else {
                            if (LiParametros.First().AsValueString() == null || LiParametros.First().AsValueString() == "" || LiParametros.First().AsValueString().Length <= 2) 
                            {

                                //aqui hay que validar cuando el campo no es numerico
                                ANCHO = "0.00";
                            }
                            else
                                ANCHO = (LiParametros.First().AsDouble() * CALCULA_FACTOR1(LiParametros.First().AsValueString()) / CALCULA_FACTOR(LiParametros.First().AsValueString())).ToString("N3");


                        } 
                        if (ANCHO is null) ANCHO= "";
                    }
                    else
                        ANCHO = numero.ToString("N3");
                }
                else if (FilaFormula.Cells[4].Value.ToString() != "" && FilaFormula.Cells[4].Value.ToString()[0].ToString() == "=")
                {
                    //MessageBox.Show("Es igual el volumen", "");

                    GridPanel panel1 = (GridPanel)SGFormulas.PrimaryGrid;
                    GridRow filaP = (GridRow)panel1.Rows[0];
                    List<LCalculoDetalle> listaDetFormulaFitrada = ListaCalculoDetalle.Where(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Ancho").ToList();
                    double CalculoTotal = 0;
                    string OperacionCadena = "";
                    string OperacionActual = "";
                    foreach (var datoF in listaDetFormulaFitrada)
                    {

                        string CalculoCadena = "";
                        if (datoF.Operacion is null) datoF.Operacion = "";
                        OperacionCadena = datoF.Operacion;
                        double numeroAux = 1;
                        if (!double.TryParse(datoF.Campo, out numeroAux))
                        {
                            UNIQUEID = dato.UniqueId;
                            ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                            currentid = ELEMENTO.Id;
                            IList<Autodesk.Revit.DB.Parameter> LiParametros;
                            LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(datoF.Campo);
                            if (LiParametros is null || LiParametros.Count() == 0) CalculoCadena = "";
                            else {
                                if (LiParametros.First().AsValueString() == null || LiParametros.First().AsValueString() == "" || LiParametros.First().AsValueString().Length <= 2)
                                {

                                    //aqui hay que validar cuando el campo no es numerico
                                    CalculoCadena = "0.00";
                                }
                                else
                                    CalculoCadena = (LiParametros.First().AsDouble() * CALCULA_FACTOR1(LiParametros.First().AsValueString()) / CALCULA_FACTOR(LiParametros.First().AsValueString())).ToString("N3");

                            } 
                            if (CalculoCadena is null) CalculoCadena = "";
                        }
                        else
                            CalculoCadena = numeroAux.ToString("N3");

                        if (OperacionActual == "" && OperacionCadena != "")
                        {
                            if (CalculoCadena == "") CalculoCadena = "0.00";
                            CalculoTotal = Convert.ToDouble(CalculoCadena);
                            OperacionActual = OperacionCadena;
                        }
                        else
                        {
                            if (CalculoCadena == "") CalculoCadena = "0.00";
                            switch (OperacionActual)
                            {
                                case "+":
                                    CalculoTotal = CalculoTotal + Convert.ToDouble(CalculoCadena);
                                    break;
                                case "-":
                                    CalculoTotal = CalculoTotal - Convert.ToDouble(CalculoCadena);
                                    break;
                                case "*":
                                    CalculoTotal = CalculoTotal * Convert.ToDouble(CalculoCadena);
                                    break;
                                case "/":
                                    CalculoTotal = CalculoTotal / Convert.ToDouble(CalculoCadena);
                                    break;
                            }
                            OperacionActual = OperacionCadena;
                        }
                    }
                    ANCHO = CalculoTotal.ToString("N3");


                }


                //ALTO
                string ALTO = "";
                if (FilaFormula.Cells[5].Value is null) FilaFormula.Cells[5].Value = "";

                if (FilaFormula.Cells[5].Value.ToString() != "" && FilaFormula.Cells[5].Value.ToString()[0].ToString() != "=")
                {
                    double numero = 1;
                    if (!double.TryParse(FilaFormula.Cells[5].Value.ToString(), out numero))
                    {
                        //MessageBox.Show("no es un numero valido");
                        UNIQUEID = dato.UniqueId;
                        ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                        currentid = ELEMENTO.Id;
                        IList<Autodesk.Revit.DB.Parameter> LiParametros;
                        LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(FilaFormula.Cells[5].Value.ToString());
                        if (LiParametros is null || LiParametros.Count() == 0) ALTO = "";
                        else {

                            if (LiParametros.First().AsValueString() == null || LiParametros.First().AsValueString() == "" || LiParametros.First().AsValueString().Length <= 2)
                            {

                                //aqui hay que validar cuando el campo no es numerico
                                ALTO = "0.00";
                            }
                            else
                                ALTO = (LiParametros.First().AsDouble() * CALCULA_FACTOR1(LiParametros.First().AsValueString()) / CALCULA_FACTOR(LiParametros.First().AsValueString())).ToString("N3");

                        } 
                        if (ALTO is null) ALTO = "";
                    }
                    else
                        ALTO = numero.ToString("N3");
                }
                else if (FilaFormula.Cells[5].Value.ToString() != "" && FilaFormula.Cells[5].Value.ToString()[0].ToString() == "=")
                {
                    //calculo con formulas en altura
                    GridPanel panel1 = (GridPanel)SGFormulas.PrimaryGrid;
                    GridRow filaP = (GridRow)panel1.Rows[0];
                    List<LCalculoDetalle> listaDetFormulaFitrada = ListaCalculoDetalle.Where(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Alto").ToList();
                    double CalculoTotal = 0;
                    string OperacionCadena = "";
                    string OperacionActual = "";
                    foreach (var datoF in listaDetFormulaFitrada)
                    {

                        string CalculoCadena = "";
                        if (datoF.Operacion is null) datoF.Operacion = "";
                        OperacionCadena = datoF.Operacion;
                        double numeroAux = 1;
                        if (!double.TryParse(datoF.Campo, out numeroAux))
                        {
                            UNIQUEID = dato.UniqueId;
                            ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
                            currentid = ELEMENTO.Id;
                            IList<Autodesk.Revit.DB.Parameter> LiParametros;
                            LiParametros = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(currentid).GetParameters(datoF.Campo);
                            if (LiParametros is null || LiParametros.Count() == 0) CalculoCadena = "";
                            else {

                                if (LiParametros.First().AsValueString() == null || LiParametros.First().AsValueString() == "" || LiParametros.First().AsValueString().Length <= 2)
                                {

                                    //aqui hay que validar cuando el campo no es numerico
                                    CalculoCadena = "0.00";
                                }
                                else
                                    CalculoCadena = (LiParametros.First().AsDouble() * CALCULA_FACTOR1(LiParametros.First().AsValueString()) / CALCULA_FACTOR(LiParametros.First().AsValueString())).ToString("N3");

                            } 
                            if (CalculoCadena is null) CalculoCadena = "";
                        }
                        else
                            CalculoCadena = numeroAux.ToString("N3");

                        if (OperacionActual == "" && OperacionCadena != "")
                        {
                            if (CalculoCadena == "") CalculoCadena = "0.00";
                             CalculoTotal = Convert.ToDouble(CalculoCadena);
                            OperacionActual = OperacionCadena;
                        }
                        else
                        {
                            if (CalculoCadena == "") CalculoCadena = "0.00";
                            switch (OperacionActual)
                            {
                                case "+":
                                    CalculoTotal = CalculoTotal + Convert.ToDouble(CalculoCadena);
                                    break;
                                case "-":
                                    CalculoTotal = CalculoTotal - Convert.ToDouble(CalculoCadena);
                                    break;
                                case "*":
                                    CalculoTotal = CalculoTotal * Convert.ToDouble(CalculoCadena);
                                    break;
                                case "/":
                                    CalculoTotal = CalculoTotal / Convert.ToDouble(CalculoCadena);
                                    break;
                            }
                            OperacionActual = OperacionCadena;
                        }
                    }
                    ALTO = CalculoTotal.ToString("N3");


                }

                

                double S1=1, S2=1, S3=1, S4 = 1;

                if (CANTI != "") S1 = Convert.ToDouble(CANTI); else S1 = 1;
                if (LONGITUD != "") S2 = Convert.ToDouble(LONGITUD); else S2 = 1;
                if (ALTO != "") S3 = Convert.ToDouble(ALTO); else S3 = 1;
                if (ANCHO != "") S4 = Convert.ToDouble(ANCHO); else S4 = 1;
                double TOTAL = S1 * S2 * S3 * S4;



                // SUMAR DENTRO DEL TITULO QUE CORRESPONDE DENTRO DE VARIABLE PADREANTERIOR


                foreach (var campoModifica in ListaItemsMedicion.Where(r => (r.CodMedicion == PADREANTERIOR && r.Tipo == "Titulo")))
                {
                    if (campoModifica.Total == "") campoModifica.Total = "0.00";
                    campoModifica.Total = (Convert.ToDouble(campoModifica.Total) + TOTAL).ToString("N3");
                    //string padreact = campoModifica.PhantomParentId;
                    //string totalactual = campoModifica.Total;
                    /*for (int p = campoModifica.Nivel-1; p >= 1; p--)
                    {
                        foreach (var campoModifica1 in ListaItemsMedicion.Where(r => (r.CodMedicion == padreact && r.Tipo == "Titulo"))) {
                            
                            if (campoModifica1.Total == "") campoModifica1.Total = "0.00";
                            campoModifica1.Total = (Convert.ToDouble(campoModifica1.Total) + Convert.ToDouble(totalactual)).ToString("N3");

                            padreact = campoModifica1.PhantomParentId;
                            totalactual = campoModifica1.Total;
                        }
                        
                    }*/

                }



                string CodigoUni = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                ListaItemsMedicion.Add(new LMedicion { CodMedicion = CodigoUni, CodPresupuesto = "", CodSubpresupuesto = "", Item = "", Descripcion = dato.Familia.ToString() + " - " + dato.Tipo.ToString() + " - " + DESCRI, Cantidad = CANTI, Longitud = LONGITUD, Ancho = ANCHO, Alto = ALTO, Total = TOTAL.ToString("N3"), Detalle = dato.Categoria.ToString(), Vinculo = codModelo, UniqueId = dato.UniqueId.ToString(), PhantomParentId = PADREANTERIOR, Nivel = NivelActual+1, Tipo = "Medicion" });
                //GuardarMedicion(CodigoUni, dato.Familia.ToString() + " - " + dato.Tipo.ToString() + " - " + DESCRI, CANTI, LONGITUD, ANCHO, ALTO, TOTAL.ToString("N3"), dato.Categoria.ToString(), "", dato.UniqueId.ToString(), PADREANTERIOR, NivelActual + 1, "Medicion");



            }


            //BORRAR TITULOS QUE NO TENGAN HIJOS
            for (int p = 0; p < panelNiveles.Rows.Count() - 1; p++) {
                var OtroFiltro = new List<LMedicion>();
                OtroFiltro = ListaItemsMedicion.Where(X => X.Tipo == "Titulo").ToList();
                foreach (var datoa in OtroFiltro)
                {
                    var AuxFiltro = new List<LMedicion>();
                    AuxFiltro = ListaItemsMedicion.Where(X => X.PhantomParentId == datoa.CodMedicion).ToList();

                    if (AuxFiltro.Count() == 0)
                    {
                        var item = ListaItemsMedicion.Single(x => x.CodMedicion == datoa.CodMedicion);
                        ListaItemsMedicion.Remove(item);
                    }
                }
            }

            
            foreach (var campoModifica in ListaItemsMedicion.Where(r => (r.Nivel == panelNiveles.Rows.Count() - 1 && r.Tipo == "Titulo")))
            {
                //if (campoModifica.Total == "") campoModifica.Total = "0.00";
                //campoModifica.Total = (Convert.ToDouble(campoModifica.Total) + TOTAL).ToString("N3");
                string padreact = campoModifica.PhantomParentId;
                string totalactual = campoModifica.Total;
                for (int p = campoModifica.Nivel-1; p >= 1; p--)
                {
                    foreach (var campoModifica1 in ListaItemsMedicion.Where(r => (r.CodMedicion == padreact && r.Tipo == "Titulo"))) {

                        if (campoModifica1.Total == "") campoModifica1.Total = "0.00";
                        campoModifica1.Total = (Convert.ToDouble(campoModifica1.Total) + Convert.ToDouble(totalactual)).ToString("N3");

                        padreact = campoModifica1.PhantomParentId;
                        //totalactual = campoModifica1.Total;
                    }

                }

            }


        
        




            cadenaMedicionG = "";
            var basdat = new ConexionBD();
            basdat.Conexion();
            //basdat.GuardarPlano(Presupuesto_actual, SubPresupuesto_actual, Item_actual, "001", "rvt", "urn1", "Urn2", "email");
            basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);

            /*"RequestId":"Modificar",
   "ObjectName":"dbo.S10_01_SubpresupuestoDetalleCalculoDetalle_Actualizar '0501001','001','000000000000034','dvRmC9IjYE6FMR1SH24ewg==','888','Longitud','Volumen','*','1','jleon@s10peru.com'",
   "SignalRConnectionID":"da1f6cea-96c3-44e8-9761-6d0b310acd46",
   "SecurityUserId":580,
   "IsBulkTransaction": true,
   "Data":"[{\"Id\":1,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2}…….."*/


            //request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleMedicion_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodMedicion + "','" + Descripcion + "','" + Cantidad + "','" + Longitud + "','" + Ancho + "','" + Alto + "','" + Total + "','" + Detalle + "','" + Vinculo + "','" + UniqueId + "','" + PhantomParentId + "','" + Nivel + "','" + Tipo + "','" + EmailUsuario + "'");

            /*{\"Id\":1,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2},{\"P\":\"MQ==\",\"O\":3},{\"P\":\"888\",\"O\":4},{\"P\":\"Titulo\",\"O\":5},{\"P\":\"Volumen\",\"O\":6},{\"P\":\"*\",\"O\":7},{\"P\":\"1\",\"O\":8},{\"P\":\jleon@s10peru.com\,\"O\":9}]},
              {\"Id\":2,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2},{\"P\":\"Mg==\",\"O\":3},{\"P\":\"888\",\"O\":4},{\"P\":\"Titulo\",\"O\":5},{\"P\":\"Volumen\",\"O\":6},{\"P\":\"*\",\"O\":7},{\"P\":\"1\",\"O\":8},{\"P\":\jleon@s10peru.com\,\"O\":9}]},
              {\"Id\":3,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2},{\"P\":\"Mw==\",\"O\":3},{\"P\":\"888\",\"O\":4},{\"P\":\"Titulo\",\"O\":5},{\"P\":\"Volumen\",\"O\":6},{\"P\":\"*\",\"O\":7},{\"P\":\"1\",\"O\":8},{\"P\":\jleon@s10peru.com\,\"O\":9}]},
              {\"Id\":4,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2},{\"P\":\"NA==\",\"O\":3},{\"P\":\"888\",\"O\":4},{\"P\":\"Titulo\",\"O\":5},{\"P\":\"Volumen\",\"O\":6},{\"P\":\"*\",\"O\":7},{\"P\":\"1\",\"O\":8},{\"P\":\jleon@s10peru.com\,\"O\":9}]}*/

         Data = new List<LData>();


        int pos = 1;
            System.Data.DataTable dt = new System.Data.DataTable();
            dt = basdat.LmedicionesSubPresupuesto(Presupuesto_actual, SubPresupuesto_actual, Item_actual);

            foreach (var campo1 in ListaItemsMedicion) {
                //GuardarMedicion(campo1.CodMedicion, campo1.Descripcion, campo1.Cantidad, campo1.Longitud, campo1.Ancho, campo1.Alto, campo1.Total, campo1.Detalle, campo1.Vinculo, campo1.UniqueId, campo1.PhantomParentId, campo1.Nivel, campo1.Tipo);
                //Thread.Sleep(30);
                //basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, campo1.CodMedicion, campo1.Descripcion, campo1.Cantidad, campo1.Longitud, campo1.Ancho, campo1.Alto, campo1.Total, campo1.Detalle, campo1.Vinculo, campo1.UniqueId, campo1.PhantomParentId, campo1.Nivel, campo1.Tipo, EmailUsuario);
                basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, campo1.CodMedicion, campo1.Descripcion, campo1.Cantidad, campo1.Longitud, campo1.Ancho, campo1.Alto, campo1.Total, campo1.Detalle, codModelo, campo1.UniqueId, campo1.PhantomParentId, campo1.Nivel, campo1.Tipo, EmailUsuario);
                //new[] { "Cool", "Windy", "Humid" }
                LParameters[] Parametros = new[] { new LParameters { P = Presupuesto_actual, O = 0 } , new LParameters { P = SubPresupuesto_actual, O = 1 }, new LParameters { P = Item_actual, O = 2 } , new LParameters { P = campo1.CodMedicion, O = 3 } , new LParameters { P = campo1.Descripcion, O = 4 } , new LParameters { P = campo1.Cantidad, O = 5 } , new LParameters { P = campo1.Longitud, O = 6 } , new LParameters { P = campo1.Ancho, O = 7 } , new LParameters { P = campo1.Alto, O = 8 }, new LParameters { P = campo1.Total, O = 9 }, new LParameters { P = campo1.Detalle, O = 10 }, new LParameters { P = codModelo, O = 11 }, new LParameters { P = campo1.UniqueId, O = 12 }, new LParameters { P = campo1.PhantomParentId, O = 13 }, new LParameters { P = campo1.Nivel.ToString(), O = 14 }, new LParameters { P = campo1.Tipo, O = 15 }, new LParameters { P = EmailUsuario, O = 16 } };
                Data.Add(new LData { Id = pos, Parameters = Parametros });
                
                //cadenaMedicionG = cadenaMedicionG + @"{\""Id\"":" + pos.ToString() + @",\""Parameters\"":[{\""P\"":\""" + Presupuesto_actual + @"\"",\""O\"":0},{\""P\"":\""" + SubPresupuesto_actual +  @"\"",\""O\"":1},{\""P\"":\""" + Item_actual + @"\"",\""O\"":2},{\""P\"":\""" + campo1.CodMedicion + @"\"",\""O\"":3},{\""P\"":\""" + campo1.Descripcion + @"\"",\""O\"":4},{\""P\"":\""" + campo1.Cantidad + @"\"",\""O\"":5},{\""P\"":\""" + campo1.Longitud + @"\"",\""O\"":6},{\""P\"":\""" + campo1.Ancho + @"\"",\""O\"":7},{\""P\"":\""" + campo1.Alto + @"\"",\""O\"":8},{\""P\"":\""" + campo1.Total + @"\"",\""O\"":9},{\""P\"":\""" + campo1.Detalle + @"\"",\""O\"":10},{\""P\"":\""" + codModelo + @"\"",\""O\"":11},{\""P\"":\""" + campo1.UniqueId + @"\"",\""O\"":12},{\""P\"":\""" + campo1.PhantomParentId + @"\"",\""O\"":13},{\""P\"":\""" + campo1.Nivel + @"\"",\""O\"":14},{\""P\"":\""" + campo1.Tipo + @"\"",\""O\"":15},{\""P\"":\""" + EmailUsuario + @"\"",\""O\"":16}]},";
                pos++;

            }


            foreach (DataRow dat in dt.Rows)
            {
                ListaItemsMedicion.Add(new LMedicion { CodMedicion = dat["CodMedicion"].ToString().Trim(), CodPresupuesto = dat["CodPresupuesto"].ToString().Trim(), CodSubpresupuesto = dat["CodSubpresupuesto"].ToString().Trim(), Item = dat["Item"].ToString().Trim(), Descripcion = dat["Descripcion"].ToString().Trim(), Cantidad = dat["Cantidad"].ToString().Trim(), Longitud = dat["Longitud"].ToString().Trim(), Ancho = dat["Ancho"].ToString().Trim(), Alto = dat["Alto"].ToString().Trim(), Total = dat["Total"].ToString().Trim(), Detalle = dat["Detalle"].ToString().Trim(), Vinculo = dat["Vinculo"].ToString().Trim(), UniqueId = dat["UniqueId"].ToString().Trim(), PhantomParentId = dat["PhantomParentId"].ToString().Trim(), Nivel = (int)dat["Nivel"], Tipo = dat["Tipo"].ToString().Trim() });

                //cadenaMedicionG = cadenaMedicionG + @"{\""Id\"":" + pos.ToString() + @",\""Parameters\"":[{\""P\"":\""" + Presupuesto_actual + @"\"",\""O\"":0},{\""P\"":\""" + SubPresupuesto_actual + @"\"",\""O\"":1},{\""P\"":\""" + Item_actual + @"\"",\""O\"":2},{\""P\"":\""" + dat["CodMedicion"].ToString().Trim() + @"\"",\""O\"":3},{\""P\"":\""" + dat["Descripcion"].ToString().Trim() + @"\"",\""O\"":4},{\""P\"":\""" + dat["Cantidad"].ToString().Trim() + @"\"",\""O\"":5},{\""P\"":\""" + dat["Longitud"].ToString().Trim() + @"\"",\""O\"":6},{\""P\"":\""" + dat["Ancho"].ToString().Trim() + @"\"",\""O\"":7},{\""P\"":\""" + dat["Alto"].ToString().Trim() + @"\"",\""O\"":8},{\""P\"":\""" + dat["Total"].ToString().Trim() + @"\"",\""O\"":9},{\""P\"":\""" + dat["Detalle"].ToString().Trim() + @"\"",\""O\"":10},{\""P\"":\""" + dat["Vinculo"].ToString().Trim() + @"\"",\""O\"":11},{\""P\"":\""" + dat["UniqueId"].ToString().Trim() + @"\"",\""O\"":12},{\""P\"":\""" + dat["PhantomParentId"].ToString().Trim() + @"\"",\""O\"":13},{\""P\"":\""" + dat["Nivel"].ToString().Trim() + @"\"",\""O\"":14},{\""P\"":\""" + dat["Tipo"].ToString().Trim() + @"\"",\""O\"":15},{\""P\"":\""" + EmailUsuario + @"\"",\""O\"":16}]},";


                LParameters[] Parametros = new[] { new LParameters { P = Presupuesto_actual, O = 0 }, new LParameters { P = SubPresupuesto_actual, O = 1 }, new LParameters { P = Item_actual, O = 2 }, new LParameters { P = dat["CodMedicion"].ToString().Trim(), O = 3 }, new LParameters { P = dat["Descripcion"].ToString().Trim(), O = 4 }, new LParameters { P = dat["Cantidad"].ToString().Trim(), O = 5 }, new LParameters { P = dat["Longitud"].ToString().Trim(), O = 6 }, new LParameters { P = dat["Ancho"].ToString().Trim(), O = 7 }, new LParameters { P = dat["Alto"].ToString().Trim(), O = 8 }, new LParameters { P = dat["Total"].ToString().Trim(), O = 9 }, new LParameters { P = dat["Detalle"].ToString().Trim(), O = 10 }, new LParameters { P = dat["Vinculo"].ToString().Trim(), O = 11 }, new LParameters { P = dat["UniqueId"].ToString().Trim(), O = 12 }, new LParameters { P = dat["PhantomParentId"].ToString().Trim(), O = 13 }, new LParameters { P = dat["Nivel"].ToString().Trim(), O = 14 }, new LParameters { P = dat["Tipo"].ToString().Trim(), O = 15 }, new LParameters { P = EmailUsuario, O = 16 } };
                Data.Add(new LData { Id = pos, Parameters = Parametros });
                pos++;

            }
            //cadenaMedicionG = cadenaMedicionG.Substring(0, cadenaMedicionG.Length - 1);
            EliminarMedionesItemActual();
            GuardarMedicionMasiva();

            basdat.Conexion();
            CargarMediciones();


        }

        private void ButtonItem16_Click(object sender, EventArgs e)
        {

            System.Data.DataTable datosPl = new System.Data.DataTable();
            ConexionBD bdatos = new ConexionBD();
            datosPl = bdatos.LPlanos(labelItem8.Text);

            if (datosPl.Rows.Count == 0) {
                if (System.Windows.Forms.MessageBox.Show("Este modelo no está cargado en la Nube u asociado a un modelo existente, Desea Vincularlo ahora?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                    FrmBim360 fr1 = new FrmBim360();
                    fr1.TokenAct = this.txtAccessToken.Text;
                    fr1.labelItem2.Text = this.labelItem9.Text;
                    fr1.labelItem1.Text = this.labelItem8.Text;
                    fr1.TxtModelo.Text = this.labelItem8.Text + ".Rvt";
                    fr1.Show();
                }
                return;
            }
            GrpCargar.Top = (int)((this.Height / 2) - 80);
            GrpCargar.Left = 50;
            GrpCargar.Width = (int)((this.Width) - 100);
            GrpCargar.Visible = true;
            GrpCargar.Text = "Obteniendo datos de metrado ";
            GrpCargar.Refresh();

            GenerarMedicion();

            GrpCargar.Visible = false;
            GrpCargar.Refresh();

        }

        private void SGMediciones_SelectionChanged(object sender, GridEventArgs e)
        {
            GridPanel panelMediciones = (GridPanel)SGMediciones.PrimaryGrid;
            GridRow FilaSel = (GridRow)panelMediciones.ActiveRow;
            if (FilaSel is null) return;
            if (FilaSel.Cells[1].Value is null) return;
            if (FilaSel.Cells[1].Value.ToString() == "") return;

            //System.Windows.Forms.MessageBox.Show(FilaSel.Cells[1].Value.ToString(), "");

            if ((string)FilaSel.Cells[1].Value.ToString() != "")
            {


                string AuxModelo = "";
                //System.Data.DataTable datosPl = new System.Data.DataTable();
                ConexionBD bdatos = new ConexionBD();
                AuxModelo = bdatos.LPlanosXcdodigo(FilaSel.Cells[9].Value.ToString().Trim()).Trim();

                if (AuxModelo != "")
                {
                    if (AuxModelo != ModeloCargado) {
                        EntroPrespuesto = 1;
                        ModeloCargado = AuxModelo;
                        string urn = ViewerURN(ModeloCargado, "");
                        //bRowser.Load(urn);
                        webControl1.WebView.LoadUrl(urn);
                        EO.WebBrowser.Runtime.AddLicense(
                        "3a5rp7PD27FrmaQHEPGs4PP/6KFrqKax2r1GgaSxy5916u34GeCt7Pb26bSG" +
                        "prT6AO5p3Nfh5LRw4rrqHut659XO6Lto6u34GeCt7Pb26YxDs7P9FOKe5ff2" +
                        "6YxDdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW1yQKzbam2xvGvcau0weKv" +
                        "fLOz/RTinuX39vTjd4SOscufWbPw+g7kp+rp9unMmuX59hefi9jx+h3ks7Oz" +
                        "/RTinuX39hC9RoGkscufddjw/Rr2d4SOscufWZekscu7mtvosR/4qdzBs/DO" +
                        "Z7rsAxrsnpmkBxDxrODz/+iha6iywc2faLWRm8ufWZfAwAzrpeb7z7iJWZek" +
                        "sefuq9vpA/Ttn+ak9QzznrSmyNqxaaa2wd2wW5f3Bg3EseftAxDyeuvBs+I=");
                        textBoxX2.Text = urn;
                        timer12.Enabled = true;
                    }
                }
                else { 
                //algo anda mal porque no esta asociado
                
                }




                string[] cadena = new string[] { "" + (string)FilaSel.Cells[1].Value.ToString() + "" };
                //webControl1.eva("highlightRevit", cadena);
                try
                {
                    webControl1.WebView.EvalScript("highlightRevit('" + cadena[0] + "');");
                    EO.WebBrowser.Runtime.AddLicense(
                    "3a5rp7PD27FrmaQHEPGs4PP/6KFrqKax2r1GgaSxy5916u34GeCt7Pb26bSG" +
                    "prT6AO5p3Nfh5LRw4rrqHut659XO6Lto6u34GeCt7Pb26YxDs7P9FOKe5ff2" +
                    "6YxDdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW1yQKzbam2xvGvcau0weKv" +
                    "fLOz/RTinuX39vTjd4SOscufWbPw+g7kp+rp9unMmuX59hefi9jx+h3ks7Oz" +
                    "/RTinuX39hC9RoGkscufddjw/Rr2d4SOscufWZekscu7mtvosR/4qdzBs/DO" +
                    "Z7rsAxrsnpmkBxDxrODz/+iha6iywc2faLWRm8ufWZfAwAzrpeb7z7iJWZek" +
                    "sefuq9vpA/Ttn+ak9QzznrSmyNqxaaa2wd2wW5f3Bg3EseftAxDyeuvBs+I=");

                }
                catch { }
                
            }
            

           /* string UNIQUEID = FilaSel.Cells[1].Value.ToString();
            Element ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);
            Autodesk.Revit.DB.ElementId ESTEID = ELEMENTO.Id;

            ICollection<Autodesk.Revit.DB.ElementId> coleccionnueva;
            coleccionnueva = Datos.cmdData1.Application.ActiveUIDocument.Selection.GetElementIds();
            coleccionnueva.Clear();
            coleccionnueva.Add(ESTEID);
            Autodesk.Revit.DB.ElementId currentid = coleccionnueva.ElementAt(0);
            Datos.cmdData1.Application.ActiveUIDocument.Selection.SetElementIds(coleccionnueva);
            Datos.cmdData1.Application.ActiveUIDocument.ShowElements(currentid);*/


        }

        private void SGDetFormulas_SelectionChanged(object sender, GridEventArgs e)
        {

        }



        void actualiza_det_formula() {

            GridPanel panel = (GridPanel)SGDetFormulas.PrimaryGrid;
            GridPanel panel1 = (GridPanel)SGFormulas.PrimaryGrid;
            GridRow filaP = (GridRow)panel1.Rows[0];
            GridCell celdaP = (GridCell)panel1.ActiveCell;
            panel.Title.Text = "";
            if (filaP is null) return;
            if (celdaP is null) return;


            if (filaP.Cells[0].Value is null) filaP.Cells[0].Value = "";
            if (filaP.Cells[0].Value.ToString() == "") filaP.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            //filaP.Cells[0].Value = "888"; //llenamos un codigo falso provisional

            if (celdaP.ColumnIndex == 3 && celdaP.RowIndex == 0)
            {
                panel.Title.Text = "Longitud/Area/Volumen";
                celdaP.ReadOnly = true;
                if (panel.Rows.Count() > 1) celdaP.Value = "";
                else
                {
                    celdaP.ReadOnly = false;
                    celdaP.Value = "";
                }
                if (ListaCalculoDetalle is null) return;
                ListaCalculoDetalle.RemoveAll(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Longitud");
                for (int X = 0; X < panel.Rows.Count() - 1; X++)
                {
                    GridRow filaAux = (GridRow)panel.Rows[X];
                    
                    if (filaAux.Cells[0].Value is null) filaAux.Cells[0].Value = "";
                    if (filaAux.Cells[1].Value is null) filaAux.Cells[1].Value = "";
                    if (filaAux.Cells[2].Value is null) filaAux.Cells[2].Value = "";
                    if (filaAux.Cells[3].Value is null) filaAux.Cells[3].Value = "";
                    if (filaAux.Cells[4].Value is null) filaAux.Cells[4].Value = "";

                    filaAux.Cells[4].Value=(X+1).ToString();

                    if (filaAux.Cells[0].Value.ToString() == "") filaAux.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    ListaCalculoDetalle.Add(new LCalculoDetalle { CodCalculoDetalle = filaAux.Cells[0].Value.ToString(), CodCalculo = filaP.Cells[0].Value.ToString(), CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, TipoCampo = "Longitud", Campo = filaAux.Cells[2].Value.ToString(), Operacion = filaAux.Cells[3].Value.ToString(), Posicion = filaAux.Cells[4].Value.ToString() });
                    if (filaAux.Index == 0) celdaP.Value = "" + celdaP.Value.ToString() + filaAux.Cells[2].Value + filaAux.Cells[3].Value;
                    else celdaP.Value = "(" + celdaP.Value.ToString() + filaAux.Cells[2].Value + ")" + filaAux.Cells[3].Value;

                    GuardarCalculoDetalle(filaAux.Cells[0].Value.ToString(), filaP.Cells[0].Value.ToString(), "Longitud", filaAux.Cells[2].Value.ToString(), filaAux.Cells[3].Value.ToString(), filaAux.Cells[4].Value.ToString());

                }
                if (panel.Rows.Count() > 1) celdaP.Value = "=" + celdaP.Value.ToString();
                if (filaP.Cells[3].Value is null) return;
                GuardarCalculo(filaP.Cells[0].Value.ToString(), filaP.Cells[1].Value.ToString(), filaP.Cells[2].Value.ToString(), filaP.Cells[3].Value.ToString(), filaP.Cells[4].Value.ToString(), filaP.Cells[5].Value.ToString());




            }
            if (celdaP.ColumnIndex == 4 && celdaP.RowIndex == 0)
            {
                panel.Title.Text = "Ancho/Peso";
                celdaP.ReadOnly = true;
                if (panel.Rows.Count() > 1) celdaP.Value = "";
                else
                {
                    celdaP.ReadOnly = false;
                    celdaP.Value = "";
                }
                if (ListaCalculoDetalle is null) return;
                ListaCalculoDetalle.RemoveAll(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Ancho");
                for (int X = 0; X < panel.Rows.Count() - 1; X++)
                {
                    GridRow filaAux = (GridRow)panel.Rows[X];
                    if (filaAux.Cells[0].Value is null) filaAux.Cells[0].Value = "";
                    if (filaAux.Cells[1].Value is null) filaAux.Cells[1].Value = "";
                    if (filaAux.Cells[2].Value is null) filaAux.Cells[2].Value = "";
                    if (filaAux.Cells[3].Value is null) filaAux.Cells[3].Value = "";
                    if (filaAux.Cells[4].Value is null) filaAux.Cells[4].Value = "";
                    filaAux.Cells[4].Value = (X + 1).ToString();

                    if (filaAux.Cells[0].Value.ToString() == "") filaAux.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    ListaCalculoDetalle.Add(new LCalculoDetalle { CodCalculoDetalle = filaAux.Cells[0].Value.ToString(), CodCalculo = filaP.Cells[0].Value.ToString(), CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, TipoCampo = "Ancho", Campo = filaAux.Cells[2].Value.ToString(), Operacion = filaAux.Cells[3].Value.ToString(), Posicion = filaAux.Cells[4].Value.ToString() });
                    if (filaAux.Index == 0) celdaP.Value = "" + celdaP.Value.ToString() + filaAux.Cells[2].Value + filaAux.Cells[3].Value;
                    else celdaP.Value = "(" + celdaP.Value.ToString() + filaAux.Cells[2].Value + ")" + filaAux.Cells[3].Value;
                    GuardarCalculoDetalle(filaAux.Cells[0].Value.ToString(), filaP.Cells[0].Value.ToString(), "Ancho", filaAux.Cells[2].Value.ToString(), filaAux.Cells[3].Value.ToString(), filaAux.Cells[4].Value.ToString());
                }
                if (panel.Rows.Count() > 1) celdaP.Value = "=" + celdaP.Value.ToString();
                if (filaP.Cells[3].Value is null) return;
                GuardarCalculo(filaP.Cells[0].Value.ToString(), filaP.Cells[1].Value.ToString(), filaP.Cells[2].Value.ToString(), filaP.Cells[3].Value.ToString(), filaP.Cells[4].Value.ToString(), filaP.Cells[5].Value.ToString());


            }
            if (celdaP.ColumnIndex == 5 && celdaP.RowIndex == 0)
            {
                panel.Title.Text = "Altura";
                celdaP.ReadOnly = true;
                if (panel.Rows.Count() > 1) celdaP.Value = ""; else {
                    celdaP.ReadOnly = false;
                    celdaP.Value = "";
                }
                if (ListaCalculoDetalle is null) return;
                ListaCalculoDetalle.RemoveAll(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Alto");
                for (int X = 0; X < panel.Rows.Count() - 1; X++)
                {
                    GridRow filaAux = (GridRow)panel.Rows[X];
                    if (filaAux.Cells[0].Value is null) filaAux.Cells[0].Value = "";
                    if (filaAux.Cells[1].Value is null) filaAux.Cells[1].Value = "";
                    if (filaAux.Cells[2].Value is null) filaAux.Cells[2].Value = "";
                    if (filaAux.Cells[3].Value is null) filaAux.Cells[3].Value = "";
                    if (filaAux.Cells[4].Value is null) filaAux.Cells[4].Value = "";
                    filaAux.Cells[4].Value = (X + 1).ToString();

                    if (filaAux.Cells[0].Value.ToString() == "") filaAux.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    ListaCalculoDetalle.Add(new LCalculoDetalle { CodCalculoDetalle = filaAux.Cells[0].Value.ToString(), CodCalculo = filaP.Cells[0].Value.ToString(), CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, TipoCampo = "Alto", Campo = filaAux.Cells[2].Value.ToString(), Operacion = filaAux.Cells[3].Value.ToString(), Posicion = filaAux.Cells[4].Value.ToString() });
                    if (filaAux.Index==0) celdaP.Value = celdaP.Value.ToString() + filaAux.Cells[2].Value + filaAux.Cells[3].Value;
                    else celdaP.Value = "(" + celdaP.Value.ToString() + filaAux.Cells[2].Value + ")" + filaAux.Cells[3].Value;
                    GuardarCalculoDetalle(filaAux.Cells[0].Value.ToString(), filaP.Cells[0].Value.ToString(), "Alto", filaAux.Cells[2].Value.ToString(), filaAux.Cells[3].Value.ToString(), filaAux.Cells[4].Value.ToString());
                }
                if (panel.Rows.Count() > 1) celdaP.Value = "=" + celdaP.Value.ToString();
                if (filaP.Cells[3].Value is null) return;
                GuardarCalculo(filaP.Cells[0].Value.ToString(), filaP.Cells[1].Value.ToString(), filaP.Cells[2].Value.ToString(), filaP.Cells[3].Value.ToString(), filaP.Cells[4].Value.ToString(), filaP.Cells[5].Value.ToString());

            }


        }

        private void SGDetFormulas_EndEdit(object sender, GridEditEventArgs e)
        {


            /* public string CodDetalleCalculo { get; set; }
    public string CodCalculo { get; set; }
    public string CodPresupuesto { get; set; }
    public string CodSubpresupuesto { get; set; }
    public string Item { get; set; }
    public string TipoCampo { get; set; }
    public string Campo { get; set; }
    public string Operacion { get; set; }*/

            //var item = ListaItemsMedicion.Single(x => x.CodMedicion == datoa.CodMedicion);

            //List<LCalculoDetalle> listaDetFormulaFitrada = ListaCalculoDetalle.Where(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Ancho").ToList();
            actualiza_det_formula();
            

        }

        private void SGDetFormulas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                GridPanel panel = (GridPanel)SGDetFormulas.PrimaryGrid;
                GridRow fila = (GridRow)panel.ActiveRow;
                if (fila is null) return;
                if (fila.Index == panel.Rows.Count - 1)
                {
                    buttonItem19.Enabled = false;
                    BtNelimDetForm.Enabled = false;
                }
                else {
                    buttonItem19.Enabled = true;
                    BtNelimDetForm.Enabled = true;
                }
                    
                System.Drawing.Point pt = System.Windows.Forms.Control.MousePosition;
                BtMDetCalculo.Popup(pt);
            }
        }

        private void SGMediciones_MouseClick(object sender, MouseEventArgs e)
        {
            
            if (e.Button == MouseButtons.Right)
            {
                GridPanel panel = (GridPanel)SGMediciones.PrimaryGrid;
                GridPanel panel1 = (GridPanel)SGestructuraC.PrimaryGrid;
                GridRow fila = (GridRow)panel.ActiveRow;
                GridRow Primerafila = (GridRow)panel.Rows[0];
                if (fila is null) return;

                ButtonItem67.Text = "Eliminar ";
                ButtonItem46.Enabled = true;
                ButtonItem47.Enabled = true;
                ButtonItem47.Text = "Agregar SubItem";
                ButtonItem46.Text = "Agregar Item";
                ButtonItem12.Enabled = true;
                /*if (fila.Index == panel.Rows.Count - 1)
                    BtNelimDetForm.Enabled = false;
                else
                    BtNelimDetForm.Enabled = true;*/
                if (fila.Cells[9].Value.ToString() == "Personalizado")
                {
                    ButtonItem12.Enabled = false;
                    ButtonItem67.Enabled = true;
                    ButtonItem67.Text = "Eliminar " + fila.Cells[2].Value.ToString();
                    if (Primerafila == fila)
                    {
                        ButtonItem46.Enabled = false;
                        ButtonItem47.Enabled = true;
                        ButtonItem47.Text = "Agregar SubItem en " + fila.Cells[2].Value.ToString();
                    }
                    else {
                        ButtonItem46.Enabled = true;
                        ButtonItem47.Enabled = true;
                        if (fila.Parent.GetType().Name != "GridPanel") {
                            GridRow fpapa = (GridRow)fila.Parent;
                            ButtonItem46.Text = "Agregar Item en " + fpapa.Cells[2].Value.ToString();
                        }
                        ButtonItem47.Text = "Agregar SubItem en " + fila.Cells[2].Value.ToString();
                    }
                }
                else {
                    ButtonItem67.Enabled = false;
                    ButtonItem67.Text = "Eliminar";
                    ButtonItem47.Enabled = false;
                    ButtonItem46.Enabled = true;
                    ButtonItem46.Text = "Agregar Item en Metrados Personalizados";

                    if (fila.Cells[10].Value.ToString() == "Titulo") {
                        ButtonItem12.Enabled = false;
                    }

                    //verificar que este plano es el que corresponde

                    ConexionBD bdatos = new ConexionBD();
                    string AuxModelo = bdatos.LPlanosXcdodigoNombre(fila.Cells[9].Value.ToString().Trim()).Trim();
                    if (AuxModelo != labelItem8.Text) {
                        ButtonItem12.Enabled = false;
                    }

                }
                    

                MenuComboContraer.Items.Clear();
                foreach (GridRow filaact in panel1.Rows)
                {
                    if (!(filaact is null)) {
                        if (!(filaact.Cells[3].Value is null))
                        MenuComboContraer.Items.Add(filaact.Cells[3].Value.ToString());
                    }
                        
                }
                System.Drawing.Point pt = System.Windows.Forms.Control.MousePosition;
                BtMediciones.Popup(pt);
            }
        }

        private void buttonItem15_Click(object sender, EventArgs e)
        {
            //expandir todo metrados

            int NivelSel = SGestructuraC.PrimaryGrid.Rows.Count;


            if (NivelSel >= 1)
            {
                foreach (GridRow ff in SGMediciones.PrimaryGrid.Rows)
                    if (!(ff is null)) contraer_todo(ff);
            }


            if (NivelSel >= 1)
            {
                foreach (GridRow ff in SGMediciones.PrimaryGrid.Rows)
                    if (!(ff is null)) expandir(ff, 1, NivelSel);
            }




            /*GridPanel panel = (GridPanel)SGMediciones.PrimaryGrid;
            foreach (GridRow filaact in panel.Rows)
            {
                if (!(filaact is null))
                    filaact.Expanded = true;
            }*/
        }

        private void buttonItem14_Click(object sender, EventArgs e)
        {
            //contraer todo metrados
            /*GridPanel panel = (GridPanel)SGMediciones.PrimaryGrid;
            foreach (GridRow filaact in panel.Rows)
            {
                if (!(filaact is null))
                    filaact.Expanded = false;
            }*/
            foreach (GridRow ff in SGMediciones.PrimaryGrid.Rows)
                if (!(ff is null)) contraer_todo(ff);

        }

        private void BtNelimDetForm_Click(object sender, EventArgs e)
        {
            GridPanel panel = (GridPanel)SGDetFormulas.PrimaryGrid;
            GridRow fila = (GridRow)panel.ActiveRow;
            if (fila is null) return;

            EliminarxCodigo("CalculoDetalle", fila.Cells[0].Value.ToString());

            //recorro y pongo las posiciones
            fila.IsDeleted = true;
            panel.PurgeDeletedRows();
            for (int x = 0; x < panel.Rows.Count - 1; x++)
            {
                GridRow filaaux = (GridRow)panel.Rows[x];
                filaaux.Cells[4].Value = (x + 1).ToString();
            }

            actualiza_det_formula();


        }

        private void buttonItem19_Click(object sender, EventArgs e)
        {
            GridPanel panel = (GridPanel)SGDetFormulas.PrimaryGrid;
            //GridRow fila = (GridRow)panel.ActiveRow;
            //if (fila is null) return;
            //fila.IsDeleted = true;

            for (int x = 0; x < panel.Rows.Count - 1; x++) {
                GridRow filaaux = (GridRow)panel.Rows[x];
                EliminarxCodigo("CalculoDetalle", filaaux.Cells[0].Value.ToString());
            }
                
            panel.DeleteAll();
            panel.PurgeDeletedRows();
            GridRow nuevafila = new GridRow("", "", "", "","");
            panel.Rows.Add(nuevafila);

            actualiza_det_formula();

        }

        private void expandablePanel1_ExpandedChanged(object sender, DevComponents.DotNetBar.ExpandedChangeEventArgs e)
        {
            if (expandablePanel1.Expanded == true)
            {
                expandablePanel1.Width = (int) (this.Width * 0.18);
                metroShell1.Width = this.Width - expandablePanel1.Width - 25;
            }
            else {
                metroShell1.Width = this.Width - 45;
            }
            
        }

        private void FrmPresupuestos_Resize(object sender, EventArgs e)
        {
            metroShell1.Width = this.Width - 45;

            GroupPanel55.Height = (int)(this.Height * 0.40);
            expandablePanel3.Height = (int)(this.Height * 0.40) - 10;
            SGFormulas.Width = (int)(GroupPanel55.Width * 0.65);
        }

        private void ButtonItem12_Click(object sender, EventArgs e)
        {


            /*'RESTABLEZCO LA VISTA  ******************************************************************************** EN CASO DE AISLARLO
        'If commandData.Application.ActiveUIDocument.ActiveView.IsTemporaryHideIsolateActive() Then
        'Dim tempView As TemporaryViewMode = TemporaryViewMode.TemporaryHideIsolate
        'commandData.Application.ActiveUIDocument.ActiveView.DisableTemporaryViewMode(tempView)
        'End If
        'commandData.Application.ActiveUIDocument.RefreshActiveView()*/

            if (Datos.cmdData1.Application.ActiveUIDocument.ActiveView.IsTemporaryHideIsolateActive()) {
                TemporaryViewMode tempView = TemporaryViewMode.TemporaryHideIsolate;
                Datos.cmdData1.Application.ActiveUIDocument.ActiveView.DisableTemporaryViewMode(tempView);
                Datos.cmdData1.Application.ActiveUIDocument.RefreshActiveView();
            }


            GridPanel panelMediciones = (GridPanel)SGMediciones.PrimaryGrid;
            GridRow FilaSel = (GridRow)panelMediciones.ActiveRow;
            if (FilaSel is null) return;
            if (FilaSel.Cells[1].Value is null) return;
            if (FilaSel.Cells[1].Value.ToString() == "") return;


            /*if ((string)FilaSel.Cells[1].Value.ToString() != "")
            {
                string[] cadena = new string[] { "" + (string)FilaSel.Cells[1].Value.ToString() + "" };
                //webControl1.eva("highlightRevit", cadena);
                webView1.EvalScript("highlightRevit('" + cadena[0] + "');");
            }*/


             string UNIQUEID = FilaSel.Cells[1].Value.ToString();
             Element ELEMENTO = Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQUEID);

            if (ELEMENTO is null) return;
             Autodesk.Revit.DB.ElementId ESTEID = ELEMENTO.Id;

             ICollection<Autodesk.Revit.DB.ElementId> coleccionnueva;
             coleccionnueva = Datos.cmdData1.Application.ActiveUIDocument.Selection.GetElementIds();
             coleccionnueva.Clear();
             coleccionnueva.Add(ESTEID);
             Autodesk.Revit.DB.ElementId currentid = coleccionnueva.ElementAt(0);
             Datos.cmdData1.Application.ActiveUIDocument.Selection.SetElementIds(coleccionnueva);
             Datos.cmdData1.Application.ActiveUIDocument.ShowElements(currentid);
            Datos.cmdData1.Application.ActiveUIDocument.ActiveView.IsolateElementsTemporary(coleccionnueva);



        }



        #region MODIFICAR_DATOS



        private async void GuardarPlano(string CodPlano, string NombreArchivoRvt, string RutaArchivoRvt, string UrnAddIn, string UrnWeb)
        {

            /*var basdat = new ConexionBD();
            basdat.Conexion();
            basdat.GuardarAsociado(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodAsociado, Categoria, Familia, Tipo, campoFiltro, valorFiltro);
            basdat.Conexion();*/



            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            //request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetallePlano_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodPlano + "','" + NombreArchivoRvt + "','" + UrnAddIn + "','" + UrnWeb + "','" + EmailUsuario + "'");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetallePlano_Actualizar '" + CodPlano + "','" + NombreArchivoRvt + "','" + RutaArchivoRvt +  "','" + UrnAddIn + "','" + UrnWeb + "','" + EmailUsuario + "'");
            request.AddParameter("RequestId", "ActPlano");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }



        private async void GuardarAsociado(string CodAsociado, string Categoria, string Familia, string Tipo, string campoFiltro, string valorFiltro)
        {

            var basdat = new ConexionBD();
            basdat.Conexion();
            basdat.GuardarAsociado(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodAsociado, Categoria, Familia, Tipo, campoFiltro, valorFiltro);
            basdat.Conexion();



            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleAsociado_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodAsociado + "','" + Categoria + "','" + Familia + "','" + Tipo + "','" + campoFiltro + "','" + valorFiltro + "','" + EmailUsuario + "'");
            request.AddParameter("RequestId", "ActAsociado");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }






        private async void SolicitarAsociado()
        {
            ListaAsociados = new List<LAsociado>();
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalle_ListarAsociado '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "'");
            request.AddParameter("RequestId", "ListarAsociado");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }


        private async void GuardarEstructura(string CodEstructura, string Nivel, string Campo, string Mostrar)
        {
  

            var basdat = new ConexionBD();
            basdat.Conexion();
            basdat.GuardarEstructura(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodEstructura, Nivel, Campo, Mostrar);
            basdat.Conexion();


            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleEstructura_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodEstructura + "','" + Nivel + "','" + Campo + "','" + Mostrar + "','" + EmailUsuario + "'");
            request.AddParameter("RequestId", "ActEstructura");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }


        /*private async void GuardarEstructura1(string ItemAc,string CodEstructura, string Nivel, string Campo, string Mostrar)
        {


            var basdat = new ConexionBD();
            //basdat.Conexion();
            basdat.GuardarEstructura(Presupuesto_actual, SubPresupuesto_actual, ItemAc, CodEstructura, Nivel, Campo, Mostrar);
            //basdat.Conexion();

            //modificar para utilizar la carga masiva
            
            
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleEstructura_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodEstructura + "','" + Nivel + "','" + Campo + "','" + Mostrar + "','" + EmailUsuario + "'");
            request.AddParameter("RequestId", "ActEstructura");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }*/


        private async void SolicitarEstructura()
        {
            ListaEstructuras = new List<LEstructura>();
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalle_ListarEstructura '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "'");
            request.AddParameter("RequestId", "ListarEstructura");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }



        private async void GuardarCalculo(string CodCalculo, string Descripcion, string Cantidad, string Longitud, string Ancho, string Alto)
        {
            /*@CodPresupuesto VARCHAR(7),
            @CodSubpresupuesto VARCHAR(3),
            @Item VARCHAR(20),
            @CodCalculo VARCHAR(25),
            @Descripcion VARCHAR(35),
            @Cantidad VARCHAR(35),
            @Longitud VARCHAR(100),
            @Ancho VARCHAR(100),
            @Alto VARCHAR(100)*/

            var basdat = new ConexionBD();
            basdat.Conexion();
            basdat.GuardarCalculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodCalculo, Descripcion, Cantidad, Longitud, Ancho, Alto);
            basdat.Conexion();
            
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleCalculo_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodCalculo + "','" + Descripcion + "','" + Cantidad + "','" + Longitud + "','" + Ancho + "','" + Alto + "','" + EmailUsuario + "'");
            request.AddParameter("RequestId", "ActCalculo");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }


        private async void SolicitarCalculo()
        {
            ListaCalculos = new List<LCalculo>();
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalle_ListarCalculo '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "'");
            request.AddParameter("RequestId", "ListarCalculo");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }




        private async void GuardarCalculoDetalle(string CodCalculoDetalle, string CodCalculo, string TipoCampo, string Campo, string Operacion, string Posicion)
        {
            /*@CodPresupuesto VARCHAR(7),
            @CodSubpresupuesto VARCHAR(3),
            @Item VARCHAR(20),
            @CodCalculoDetalle VARCHAR(25),
            @CodCalculo VARCHAR(25),
            @TipoCampo VARCHAR(10),
            @Campo VARCHAR(35),
            @Operacion VARCHAR(2) */
            var basdat = new ConexionBD();
            basdat.Conexion();
            basdat.GuardarCalculoDetalle(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodCalculoDetalle, CodCalculo, TipoCampo, Campo, Operacion, Posicion);
            basdat.Conexion();
            

            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleCalculoDetalle_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodCalculoDetalle + "','" + CodCalculo + "','" + TipoCampo + "','" + Campo + "','" + Operacion + "','" + Posicion + "','" + EmailUsuario + "'");
            request.AddParameter("RequestId", "ActCalculoDetalle");
            //string cadenaaux = "dbo.S10_01_SubpresupuestoDetalleCalculoDetalle_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodCalculoDetalle + "','" + CodCalculo + "','" + TipoCampo + "','" + Campo + "','" + Operacion + "','" + EmailUsuario + "'";
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
            //System.Windows.Forms.MessageBox.Show("dbo.S10_01_SubpresupuestoDetalleCalculoDetalle_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodCalculoDetalle + "','" + CodCalculo + "','" + TipoCampo + "','" + Campo + "','" + Operacion + "','" + EmailUsuario + "'", "");
        }


        private async void SolicitarCalculoDetalle()
        {
            ListaCalculoDetalle = new List<LCalculoDetalle>();
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalle_ListarCalculoDetalle '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "'");
            request.AddParameter("RequestId", "ListarCalculoDetalle");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }



        private async void GuardarMedicion(string CodMedicion, string Descripcion, string Cantidad, string Longitud, string Ancho, string Alto, string Total, string Detalle, string Vinculo, string UniqueId, string PhantomParentId, int Nivel, string Tipo)
        {
            /*@CodPresupuesto VARCHAR(7),
            @CodSubpresupuesto VARCHAR(3),
            @Item VARCHAR(20),
            @CodMedicion VARCHAR(25),
            @Descripcion VARCHAR(100),
            @Cantidad VARCHAR(12),
            @Longitud VARCHAR(12),
            @Ancho VARCHAR(12),
            @Alto VARCHAR(12),
            @Total VARCHAR(15),
            @Detalle VARCHAR(80),
            @Vinculo VARCHAR(20),
            @UniqueId VARCHAR(20),
            @PhantomParentId VARCHAR(25),
            @Nivel INT,
            @Tipo VARCHAR(10)*/

            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleMedicion_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodMedicion + "','" + Descripcion + "','" + Cantidad + "','" + Longitud + "','" + Ancho + "','" + Alto + "','" + Total + "','" + Detalle + "','" + Vinculo + "','" + UniqueId + "','" + PhantomParentId + "','" + Nivel + "','" + Tipo + "','" + EmailUsuario + "'");
            request.AddParameter("RequestId", "ActMedicion");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }



        private async void GuardarMedicionMasiva(/*string CodMedicion, string Descripcion, string Cantidad, string Longitud, string Ancho, string Alto, string Total, string Detalle, string Vinculo, string UniqueId, string PhantomParentId, int Nivel, string Tipo*/)
        {


            /*@CodPresupuesto VARCHAR(7),
            @CodSubpresupuesto VARCHAR(3),
            @Item VARCHAR(20),
            @CodMedicion VARCHAR(25),
            @Descripcion VARCHAR(100),
            @Cantidad VARCHAR(12),
            @Longitud VARCHAR(12),
            @Ancho VARCHAR(12),
            @Alto VARCHAR(12),
            @Total VARCHAR(15),
            @Detalle VARCHAR(80),
            @Vinculo VARCHAR(20),
            @UniqueId VARCHAR(20),
            @PhantomParentId VARCHAR(25),
            @Nivel INT,
            @Tipo VARCHAR(10)*/

            //Request


            /*"RequestId":"Modificar",
    "ObjectName":"dbo.S10_01_SubpresupuestoDetalleCalculoDetalle_Actualizar '0501001','001','000000000000034','dvRmC9IjYE6FMR1SH24ewg==','888','Longitud','Volumen','*','1','jleon@s10peru.com'",
    "SignalRConnectionID":"da1f6cea-96c3-44e8-9761-6d0b310acd46",
    "SecurityUserId":580,
    "IsBulkTransaction": true,
    "Data":"[{\"Id\":1,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2}…….."*/
            var options = new JsonSerializerOptions { WriteIndented = false };
            string jsonString = System.Text.Json.JsonSerializer.Serialize(Data, options);

            //http://200.48.100.203:5030/api/S10ERP/RequestS10ERPTransaction
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPTransaction", RestSharp.Method.POST);
            //RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            //request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleMedicion_Actualizar '" + "0501001" + "','" + "001" + "','" + "000000000000034" + "','" + "dvRmC9IjYE6FMR1SH24ewg==" + "','" + "Descripcion" + "','" + "0.00" + "','" + "0.00" + "','" + "0.00" + "','" + "0.00" + "','" + "0.00" + "','" + "Detalle" + "','" + "dvRmC9IjYE6FMR1SH24ewg==" + "','" + "da1f6cea-96c3-44e8-9761-6d0b310acd46" + "','" + "dvRmC9IjYE6FMR1SH24ewg==" + "','" + "1" + "','" + "Titulo" + "','" + "email@email.com" + "'");
            request.AddParameter("RequestId", "Modificar");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "580");
            request.AddParameter("IsBulkTransaction", "true");
            request.AddParameter("Data", jsonString );
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox5.Text = JObject.Parse(response.Content).ToString();



        }



        private async void SolicitarMedicion()
        {
            ListaItemsMedicion = new List<LMedicion>();
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalle_ListarMedicion '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "'");
            request.AddParameter("RequestId", "ListarMedicion");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }



        private async void GuardarMetrado(string Metrado)
        {
            /*@CodPresupuesto VARCHAR(7),
            @CodSubpresupuesto VARCHAR(3),
            @Item VARCHAR(20),
            @CodCalculoDetalle VARCHAR(25),
            @CodCalculo VARCHAR(25),
            @TipoCampo VARCHAR(10),
            @Campo VARCHAR(35),
            @Operacion VARCHAR(2) */

            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalle_ActualizarMetrado '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + Metrado + "','" + EmailUsuario + "'");
            request.AddParameter("RequestId", "ActMetrado");
            //string cadenaaux = "dbo.S10_01_SubpresupuestoDetalleCalculoDetalle_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodCalculoDetalle + "','" + CodCalculo + "','" + TipoCampo + "','" + Campo + "','" + Operacion + "','" + EmailUsuario + "'";
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
            //System.Windows.Forms.MessageBox.Show("dbo.S10_01_SubpresupuestoDetalleCalculoDetalle_Actualizar '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + CodCalculoDetalle + "','" + CodCalculo + "','" + TipoCampo + "','" + Campo + "','" + Operacion + "','" + EmailUsuario + "'", "");
        }


        private async void EliminarxCodigo(string Tabla,string CodEliminar)
        {
            var basdat = new ConexionBD();
            basdat.Conexion();
            
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            switch (Tabla)
            {
                case "Asociado":
                    basdat.EliminarAsociado(CodEliminar);
                    request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleAsociado_Eliminar '" + CodEliminar + "'");
                    break;
                case "Estructura":
                    basdat.EliminarEstructura(CodEliminar);
                    request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleEstructura_Eliminar '" + CodEliminar + "'");
                    break;
                case "Medicion":
                    request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleMedicion_Eliminar '" + CodEliminar + "'");
                    break;
                case "Calculo":
                    basdat.EliminarCalculo(CodEliminar);
                    request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleCalculo_Eliminar '" + CodEliminar + "'");
                    break;
                case "CalculoDetalle":
                    basdat.EliminarCalculoDetalle(CodEliminar);
                    request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleCalculoDetalle_Eliminar '" + CodEliminar + "'");
                    break;
            }
            request.AddParameter("RequestId", "ElimAsociado");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
            
            basdat.Conexion();
        }


        private async void EliminarMedionesItemActual()
        {
            RestClient client = new RestClient("http://200.48.100.203:5030/api");
            RestRequest request = new RestRequest("/S10ERP/RequestS10ERPData", RestSharp.Method.POST);
            request.AddParameter("HasOutputParam", "false");
            request.AddParameter("ObjectName", "dbo.S10_01_SubpresupuestoDetalleMedicion_EliminarPorItem '" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "'");
            request.AddParameter("RequestId", "ElimAsociado");
            request.AddParameter("SignalRConnectionID", textBox2.Text);
            request.AddParameter("SecurityUserId", "1148");
            request.AddHeader("Token", Token);
            request.AddHeader("ModuleID", "11");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            IRestResponse response = await client.ExecuteAsync(request);
            dynamic responseDynamic = JObject.Parse(response.Content);
            textBox1.Text = JObject.Parse(response.Content).ToString();
        }


        #endregion

        private void SGestructuraC_EndEdit(object sender, GridEditEventArgs e)
        {
            GridPanel panel = (GridPanel)SGestructuraC.PrimaryGrid;
            GridRow fila = (GridRow)panel.ActiveRow;
            if (fila is null) return;
            
            if (Item_actual == "") return;
            if (TotalElementos is null) return;

            //si existen datos?
            bool vacios = true;
            for (int x = 0; x <= 3; x++)
            {
                if (fila.Cells[x].Value is null) fila.Cells[x].Value = "";
                if (fila.Cells[x].Value.ToString() != "") vacios = false;
            }
            if (fila.Cells[4].Value is null) fila.Cells[4].Value = false;

            if (!vacios)
            {
                if (fila.Cells[0].Value.ToString() == "") fila.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                string campoMostrar = "";
                if ((bool)fila.Cells[4].Value == true) campoMostrar = "true";
                if ((bool)fila.Cells[4].Value == false) campoMostrar = "false";
                GuardarEstructura(fila.Cells[0].Value.ToString(), fila.Cells[2].Value.ToString(), fila.Cells[3].Value.ToString(), campoMostrar); 
            }
        }

        private void SGestructuraC_CellValueChanged(object sender, GridCellValueChangedEventArgs e)
        {

            GridPanel panel = (GridPanel)e.GridPanel;
            GridRow fila = (GridRow)e.GridPanel.ActiveRow;
            if (fila is null) return;
            if (fila.Index == panel.Rows.Count() - 1) return;

            if (Item_actual == "") return;
            if (TotalElementos is null) return;

            //si existen datos?
            bool vacios = true;
            for (int x = 0; x <= 3; x++)
            {
                if (fila.Cells[x].Value is null) fila.Cells[x].Value = "";
                if (fila.Cells[x].Value.ToString() != "") vacios = false;
            }
            if (fila.Cells[4].Value is null) fila.Cells[4].Value = false;

            if (!vacios)
            {
                if (fila.Cells[0].Value.ToString() == "") fila.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                string campoMostrar = "";
                if (fila.Cells[4].Value == "") fila.Cells[4].Value = false;
                if ((bool)fila.Cells[4].Value == true) campoMostrar = "true";
                if ((bool)fila.Cells[4].Value == false) campoMostrar = "false";
                GuardarEstructura(fila.Cells[0].Value.ToString(), fila.Cells[2].Value.ToString(), fila.Cells[3].Value.ToString(), campoMostrar);
            }

            //System.Windows.Forms.MessageBox.Show(e.GridCell.Value.ToString(), "");
        }

        private void timer11_Tick(object sender, EventArgs e)
        {
            ListaPresupuestos = new List<LPresupuestos>();
            timer11.Enabled = false;
            solicitar_datosArbPresupuestos();
            do
            {
                SGPresupuestos.PrimaryGrid.DeleteAll();
                SGPresupuestos.PrimaryGrid.PurgeDeletedRows();
            }
            while (SGPresupuestos.PrimaryGrid.Rows.Count != 0);

            timer5.Enabled = true;


        }

        private void SGFormulas_EndEdit(object sender, GridEditEventArgs e)
        {
            GridPanel panel = (GridPanel)SGFormulas.PrimaryGrid;
            GridRow fila = (GridRow) panel.Rows[0];
            if (fila is null) return;
            //if (fila.Index == panel.Rows.Count() - 1) return;

            try
            {
                e.GridCell.Value = Convert.ToDouble(e.GridCell.Value.ToString()).ToString("N2");
                if (e.GridCell.Value.ToString().Length > 10)
                    e.GridCell.Value = "";
            }
            catch {
            }

            if (Item_actual == "") return;
            if (TotalElementos is null) return;

            //si existen datos?
            bool vacios = true;
            for (int x = 0; x <= 5; x++)
            {
                if (fila.Cells[x].Value is null) fila.Cells[x].Value = "";
                if (fila.Cells[x].Value.ToString() != "") vacios = false;
            }
            
            if (!vacios)
            {
                if (fila.Cells[0].Value.ToString() == "") fila.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());                
                GuardarCalculo(fila.Cells[0].Value.ToString(), fila.Cells[1].Value.ToString(), fila.Cells[2].Value.ToString(), fila.Cells[3].Value.ToString(), fila.Cells[4].Value.ToString(), fila.Cells[5].Value.ToString());
            }
        }

        private void ButtonItem18_Click(object sender, EventArgs e)
        {
            //var lista = linq.personas.Where(a => a.nombre.contains(paramNombre)).ToList();
            //var OtroFiltro = ListaItemsMedicion.Where(X => X.Tipo == "Titulo").ToList();
            AdvCategorias.ClearAndDisposeAllNodes();
            AdvCategorias.BeginUpdate();
            foreach (var dato in _NwData)
            {
                if (dato.Country.ToUpper().Contains(TxtBuscaCategoria.Text.ToUpper())) {
                    var node = new DevComponents.AdvTree.Node();
                    node.Tag = "";
                    node.Text = dato.Country;
                    selecciona_categoria(dato.Country);
                    node.Image = ImageList1.Images[0];
                    node.Cells.Add(new DevComponents.AdvTree.Cell(CategoriaSeleccionada.Count().ToString()));
                    node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                    AdvCategorias.Nodes.Add(node);
                    node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                }
            }
            AdvCategorias.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;
        }

        private void ButtonItem102_Click(object sender, EventArgs e)
        {
            TxtBuscaCategoria.Text = "";
            AdvCategorias.ClearAndDisposeAllNodes();
            AdvCategorias.BeginUpdate();
            foreach (var dato in _NwData)
            {
                    var node = new DevComponents.AdvTree.Node();
                    node.Tag = "";
                    node.Text = dato.Country;
                    selecciona_categoria(dato.Country);
                    node.Image = ImageList1.Images[0];
                    node.Cells.Add(new DevComponents.AdvTree.Cell(CategoriaSeleccionada.Count().ToString()));
                    node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                    AdvCategorias.Nodes.Add(node);
                    node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
            }
            AdvCategorias.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;

        }

        private void SGAsociados_Click(object sender, EventArgs e)
        {

        }

        private void CmbCategorias_Click(object sender, EventArgs e)
        {

        }

        private void CmbCategorias_SelectedIndexChanged(object sender, EventArgs e)
        {

            /*List<RevitElementoBase> OtroFiltro = new List<RevitElementoBase>();
            if (CmbCategorias.Text =="")
                OtroFiltro = TotalElementos.Where(X => X.Familia.ToUpper().Contains(TxtBuscaFamilia.Text.ToUpper())).Distinct().ToList();
            else
                OtroFiltro = TotalElementos.Where(X => X.Categoria == CmbCategorias.Text && X.Familia.ToUpper().Contains(TxtBuscaFamilia.Text.ToUpper())).Distinct().ToList();*/
            selecciona_categoria(CmbCategorias.Text);
            //var OtroFiltro = TotalElementos.Where(X => X.Categoria == CmbCategorias.Text && X.Tipo.ToUpper().Contains(TxtBuscaFamilia.Text.ToUpper())).ToList();
            AdvFamilias.ClearAndDisposeAllNodes();
            AdvFamilias.BeginUpdate();
            foreach (var dato in FamiliaSeleccionada)
            {
                if (dato.Familia.ToUpper().Contains(TxtBuscaFamilia.Text.ToUpper()))
                {
                    var node = new DevComponents.AdvTree.Node();
                    node.Tag = "";
                    node.Text = dato.Familia;
                    node.Image = ImageList1.Images[0];
                    var AuxFiltro = TotalElementos.Where(X => X.Familia == dato.Familia).ToList();
                    node.Cells.Add(new DevComponents.AdvTree.Cell(AuxFiltro.Count().ToString()));
                    node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                    AdvFamilias.Nodes.Add(node);
                    node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                }
            }
            AdvFamilias.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;

            //var lista = personas.Where(a => a.nombre.contains(paramNombre)).ToList();
            //var OtroFiltro = ListaItemsMedicion.Where(X => X.Tipo == "Titulo").ToList();

        }

        private void ButtonItem20_Click(object sender, EventArgs e)
        {
            selecciona_categoria(CmbCategorias.Text);
            //var OtroFiltro = TotalElementos.Where(X => X.Categoria == CmbCategorias.Text && X.Tipo.ToUpper().Contains(TxtBuscaFamilia.Text.ToUpper())).ToList();
            AdvFamilias.ClearAndDisposeAllNodes();
            AdvFamilias.BeginUpdate();
            foreach (var dato in FamiliaSeleccionada)
            {
                if (dato.Familia.ToUpper().Contains(TxtBuscaFamilia.Text.ToUpper()))
                {
                    var node = new DevComponents.AdvTree.Node();
                    node.Tag = "";
                    node.Text = dato.Familia;
                    node.Image = ImageList1.Images[0];
                    var AuxFiltro = TotalElementos.Where(X => X.Familia == dato.Familia).ToList();
                    node.Cells.Add(new DevComponents.AdvTree.Cell(AuxFiltro.Count().ToString()));
                    node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                    AdvFamilias.Nodes.Add(node);
                    node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                }
            }
            AdvFamilias.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;

        }

        private void CmbCategoria2_Click(object sender, EventArgs e)
        {

        }

        private void CmbCategoria2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selecciona_categoria(CmbCategoria2.Text);
            //var OtroFiltro = TotalElementos.Where(X => X.Categoria == CmbCategorias.Text && X.Tipo.ToUpper().Contains(TxtBuscaFamilia.Text.ToUpper())).ToList();

            CmbFamilia.Items.Clear();
            CmbFamilia.Items.Add("");
            foreach (var dato in FamiliaSeleccionada)
            {
                CmbFamilia.Items.Add(dato.Familia);
            }
            AdvFamilias.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;



            AdvTreeTipos.ClearAndDisposeAllNodes();
            AdvTreeTipos.BeginUpdate();
            foreach (var dato in TipoSeleccionado)
            {
                //if (dato.Familia.ToUpper().Contains(TxtBuscaFamilia.Text.ToUpper()))
                //{
                    var node = new DevComponents.AdvTree.Node();
                    node.Tag = "";
                    node.Text = dato.Tipo;
                    node.Image = ImageList1.Images[0];
                    var AuxFiltro = TotalElementos.Where(X => X.Tipo == dato.Tipo).ToList();
                    node.Cells.Add(new DevComponents.AdvTree.Cell(AuxFiltro.Count().ToString()));
                    node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                    AdvTreeTipos.Nodes.Add(node);
                    node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                //}
            }
            AdvTreeTipos.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;


        }

        private void CmbFamilia_SelectedIndexChanged(object sender, EventArgs e)
        {
            selecciona_categoria(CmbCategoria2.Text);
            //var OtroFiltro = TotalElementos.Where(X => X.Categoria == CmbCategorias.Text && X.Tipo.ToUpper().Contains(TxtBuscaFamilia.Text.ToUpper())).ToList();

            AdvTreeTipos.ClearAndDisposeAllNodes();
            AdvTreeTipos.BeginUpdate();
            foreach (var dato in TipoSeleccionado)
            {
                //if (dato.Familia.ToUpper().Contains(TxtBuscaFamilia.Text.ToUpper()))
                //{
                var AuxFiltro1 = CategoriaSeleccionada.Where(X => X.Tipo == dato.Tipo && X.Familia == CmbFamilia.Text).ToList();
                if (AuxFiltro1.Count != 0) {
                    var node = new DevComponents.AdvTree.Node();
                    node.Tag = "";
                    node.Text = dato.Tipo;
                    node.Image = ImageList1.Images[0];
                    var AuxFiltro = TotalElementos.Where(X => X.Tipo == dato.Tipo).ToList();
                    node.Cells.Add(new DevComponents.AdvTree.Cell(AuxFiltro.Count().ToString()));
                    node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                    AdvTreeTipos.Nodes.Add(node);
                    node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                }


                //}
            }
            AdvTreeTipos.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;

        }

        private void ButtonItem114_Click(object sender, EventArgs e)
        {
            AdvParametrosCompartidos.ClearAndDisposeAllNodes();
            AdvParametrosCompartidos.BeginUpdate();

            foreach (var dato in ParametrosCompartidos)
            {
                if (dato.Nombre.ToUpper().Contains(TxtBuscarParametroC.Text.ToUpper())){
                    var node = new DevComponents.AdvTree.Node();
                    node.Tag = "";
                    node.Text = dato.Nombre;
                    node.Image = ImageList1.Images[6];
                    node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                    node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                    AdvParametrosCompartidos.Nodes.Add(node);
                    node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                }
            }
            AdvParametrosCompartidos.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;
        }


        private void CargaPropiedades() {
            
            if (SGAsociados.PrimaryGrid.Rows.Count > 1)
            {
                GridRow fila = (GridRow) SGAsociados.PrimaryGrid.Rows[0];
                if (TotalElementos is null) return;
                var FitroCategoria = from l in TotalElementos
                                     where l.Categoria == (string)fila.Cells[2].Value
                                     select new
                                     {
                                         Categoria = l.Categoria,
                                         UniqueId = l.UniqueId,
                                     };
                if (FitroCategoria.Count() != 0)
                {
                    var Dato = FitroCategoria.First();
                    UNIQID = Dato.UniqueId;
                    var elem = (Element)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(UNIQID) as Element;
                    List<Autodesk.Revit.DB.Parameter> lista = new List<Autodesk.Revit.DB.Parameter>();
                    if (lista != null && elem != null)
                    {
                        lista = (List<Autodesk.Revit.DB.Parameter>)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.Id).GetOrderedParameters();

                        AdvPropiedades.ClearAndDisposeAllNodes();
                        AdvPropiedades.BeginUpdate();
                        foreach (Autodesk.Revit.DB.Parameter propiedad in lista)
                        {
                            //ArPropiedades[I] = propiedad.Definition.Name.ToString();
                            if (propiedad.Definition.Name.ToString().ToUpper().Contains(TxtBuscaPropiedades.Text.ToUpper()))
                            {
                                var node = new DevComponents.AdvTree.Node();
                                node.Tag = "";
                                node.Text = propiedad.Definition.Name.ToString();
                                node.Image = ImageList1.Images[6];
                                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                                AdvPropiedades.Nodes.Add(node);
                                node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                            }
                        }
                        //fila.Cells[5].EditorType = typeof(FragrantComboBox);
                        //fila.Cells[5].EditorParams = new object[] { ArPropiedades };
                        AdvPropiedades.EndUpdate();
                    }
                }

            }

        }

        private void ButtonItem118_Click(object sender, EventArgs e)
        {
            CargaPropiedades();
        }







        #region GRAGSYDROPS

        private SuperGridControl _SrcGrid;
        private GridElement _SrcElement;
        private GridRow _DragOverRow;
        private GridCell _DragOverCell;
        private GridColumn _DragOverColumn;
        private System.Drawing.Point _MouseDownPoint;


        private void ClearDragHighlight()
        {
            if (_DragOverRow != null)
                _DragOverRow.CellStyles.Default.Background = null;

            if (_DragOverCell != null)
                _DragOverCell.CellStyles.Default.Background = null;

            if (_DragOverColumn != null)
                _DragOverColumn.CellStyles.Default.Background = null;
        }
        private void SuperGridControlDragOver(object sender, System.Windows.Forms.DragEventArgs e)  //SGPresupC.DragOver//, SGEquipoC.DragOver, SGMOC.DragOver, SGMatC.DragOver, SGTransC.DragOver, SGEquipoC1.DragOver, SGMOC1.DragOver, SGMatC1.DragOver, SGTransC1.DragOver 
        {
            SuperGridControl sg = sender as SuperGridControl;

            if (sg != null)
            {
                ClearDragHighlight();

                e.Effect = System.Windows.Forms.DragDropEffects.None;

                System.Drawing.Point clientPoint = sg.PointToClient(new System.Drawing.Point(e.X, e.Y));
                GridElement item = sg.GetElementAt(clientPoint.X, clientPoint.Y);

                if (item is GridCell)
                {
                    if (_SrcElement is GridRow)
                    {
                        item = ((GridCell)item).GridRow;
                    }
                    else if (_SrcElement == null || _SrcElement is GridCell)
                    {
                        _DragOverCell = (GridCell)item;
                        _DragOverCell.CellStyles.Default.Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.AliceBlue);

                        e.Effect = System.Windows.Forms.DragDropEffects.Copy | System.Windows.Forms.DragDropEffects.Move;
                    }
                }

                if (item is GridRow)
                {
                    if (_SrcElement == null || _SrcElement is GridRow)
                    {
                        _DragOverRow = (GridRow)item;
                        _DragOverRow.CellStyles.Default.Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.AliceBlue);

                        e.Effect = System.Windows.Forms.DragDropEffects.Copy | System.Windows.Forms.DragDropEffects.Move;
                    }
                }
                else if (item is GridColumnHeader)
                {
                    if (_SrcElement is GridColumn)
                    {
                        GridColumn doColumn = ((GridColumnHeader)item).GetHitColumn(clientPoint);

                        if (((GridColumn)_SrcElement).EditorType == doColumn.EditorType)
                        {
                            _DragOverColumn = doColumn;
                            _DragOverColumn.CellStyles.Default.Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.AliceBlue);

                            e.Effect = System.Windows.Forms.DragDropEffects.Copy | System.Windows.Forms.DragDropEffects.Move;
                        }
                    }
                    else if (_SrcElement == null || _SrcElement is GridRow)
                    {
                        e.Effect = System.Windows.Forms.DragDropEffects.Copy | System.Windows.Forms.DragDropEffects.Move;
                    }
                }

                if (sg != _SrcGrid)
                {
                    e.Effect &= ~(System.Windows.Forms.DragDropEffects.Move);
                }
                else
                {
                    if ((e.KeyState & 8) != 8)
                        e.Effect &= ~(System.Windows.Forms.DragDropEffects.Copy);
                }
            }
        }


        private void SuperGridControlDragDrop(object sender, System.Windows.Forms.DragEventArgs e) // Handles SGPresupC.DragDrop, SGEquipoC.DragDrop, SGMOC.DragDrop, SGMatC.DragDrop, SGTransC.DragDrop, SGEquipoC1.DragDrop, SGMOC1.DragDrop, SGMatC1.DragDrop, SGTransC1.DragDrop
        {
            SuperGridControl sg = sender as SuperGridControl;
            if (sg is object)
            {
                System.Drawing.Point pt = sg.PointToClient(new System.Drawing.Point(e.X, e.Y));
                GridElement item = sg.GetElementAt(pt.X, pt.Y);

                // If the data we are dropping is from a SuperGrid, then
                // be a little bit more discerning about how we drop it

                if (_SrcGrid is object)
                {
                    SelectedElementCollection items = (SelectedElementCollection)e.Data.GetData(typeof(SelectedElementCollection));
                    if (item is GridCell)
                    {
                        if (_SrcElement is GridRow)
                        {
                            DropSgRow(e, ((GridCell)item).GridRow, items);
                        }
                        else
                        {
                            DropSgCell((GridCell)item, items);
                        }
                    }
                    else if (item is GridRow)
                    {
                        DropSgRow(e, (GridRow)item, items);

                
                    }
                }
                // MsgBox(item.ToString & " ------ " & e.ToString)

                // EN ITEM TENGO EL NOMBRE DE LA CELDA DE DESTINO

                else if (item is GridCell)
                {
                    if (_SrcElement is GridRow)
                    {
                        DropRow(e, ((GridCell)item).GridRow);
                    }
                    else
                    {
                       DropCell(e, (GridCell)item);
                    }
                }
                else if (item is GridRow)
                {
                    DropRow(e, (GridRow)item);

                    // ElseIf TypeOf item Is GridColumnHeader Then
                    // DropColumnHeader(e, CType(item, GridColumnHeader), pt)
                }
            }
        }



        // copiar en la misma falta implementar
        private void CopySgRows(GridRow row, IEnumerable<GridElement> rows)
        {
            //MsgBox(row.Cells[1].Value);
            GridPanel panel = row.GridPanel;
            int J;
            int I;
            // If superTabControl1.SelectedTabIndex = 5 Then
            foreach (GridRow item in rows)
            {
                if (row is null)
                {
                    row = new GridRow();
                    panel.Rows.Add(row);
                }
          
                row = row.NextVisibleRow as GridRow;
            }


        }

        private void MoveSgRows(GridRow row, IEnumerable<GridElement> rows)
        {
            GridPanel panel = row.GridPanel;

            foreach (GridRow item in rows)
                panel.Rows.Remove(item);
            int n = row.RowIndex;
            foreach (GridRow item in rows)
            {
                panel.Rows.Insert(n, item);
                foreach (GridCell cell in item.Cells)
                {
                    // cell.CellStyles.Default.TextColor = Color.Red
                }
            }
        }

        private void DropSgRow(System.Windows.Forms.DragEventArgs e, GridRow row, IEnumerable<GridElement> rows)
        {

            if (e.Effect == System.Windows.Forms.DragDropEffects.Move)
            {
                if (row.IsSelected == false)
                {
                    MoveSgRows(row, rows);
                }
            }
            else
            {
                CopySgRows(row, rows);
            }
        }

        // MOVIENDO EN LA MISMA TABLA SI FUNCIONA
        private void DropSgCell(GridCell cell, IEnumerable<GridElement> cells)
        {
            var sb = new StringBuilder();
            foreach (GridCell droppedCell in cells)
            {
                if (droppedCell.Value is object)
                {
                    sb.Append(droppedCell.Value.ToString());
                    sb.Append(", ");
                }
            }

            if (sb.Length > 0)
            {
                sb.Length -= 2;
            }

            if ((System.String)cell.Value != "")
            {
                //int acep;
                //acep = MessageBoxEx.Show("Desea reemplazar " + cell.Value + " por " + sb.ToString(), "Advertencia", MessageBoxButtons.YesNo);
                if (System.Windows.Forms.MessageBox.Show("Desea reemplazar " + cell.Value + " por " + sb.ToString(), "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    cell.Value = sb.ToString();
                }
            }
            else
            {
                cell.Value = sb.ToString();
            }
        }

        // NO SIRVE
        private void DropTextRow(GridRow row, string[] s)
        {
            GridPanel panel = row.GridPanel;
            int n = Math.Min(s.Length, panel.Columns.Count);
            for (int i = 0, loopTo = n - 1; i <= loopTo; i++)
            {
                if (i >= row.Cells.Count)
                {
                    row.Cells.Add(new GridCell());
                }

                row.Cells[i].Value = s[i];
                row.Cells[i].CellStyles.Default.TextColor = System.Drawing.Color.Red;
            }

            row.EnsureVisible(false);
        }

        // NO SIRVE
        private void DropNodeRow(GridRow row, CellCollection cells)
        {
            var text = new string[cells.Count];
            for (int i = 0, loopTo = cells.Count - 1; i <= loopTo; i++)
                text[i] = cells[i].Text;
            DropTextRow(row, text);
        }


        // NO SIRVE
        private void DropRow(System.Windows.Forms.DragEventArgs e, GridRow row)
        {
            GridPanel panel = row.GridPanel;
            if (e.Data.GetDataPresent(typeof(string)) == true)
            {
                string s = (string)e.Data.GetData(typeof(string));
                DropTextRow(row, s.Split(','));
            }
            else if (e.Data.GetDataPresent(typeof(TreeNode)) == true)
            {
                TreeNode tnode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                DropTextRow(row, tnode.Text.Split(','));
            }
            else if (e.Data.GetDataPresent(typeof(Node)) == true)
            {
                Node node = (Node)e.Data.GetData(typeof(Node));
                DropNodeRow(row, node.Cells);
            }
            else if (e.Data.GetDataPresent(typeof(Node[])) == true)
            {
                Node[] nodes = (Node[])e.Data.GetData(typeof(Node[]));
                foreach (Node node in nodes)
                {
                    if (row is null)
                    {
                        row = new GridRow();
                        panel.Rows.Add(row);
                    }

                    DropNodeRow(row, node.Cells);
                    row = row.NextVisibleRow as GridRow;
                }
            }
        }


        private void DropCell(System.Windows.Forms.DragEventArgs e, GridCell cell)
        {
            var node = default(Node);
            var sb = new StringBuilder();
            var CODI = default(string);
            if (e.Data.GetDataPresent(typeof(string)) == true)
            {
                string s = (string)e.Data.GetData(typeof(string));
                sb.Append(s);
            }
            else if (e.Data.GetDataPresent(typeof(TreeNode)) == true)
            {
                TreeNode tnode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                sb.Append(tnode.Text);
            }
            else if (e.Data.GetDataPresent(typeof(Node)) == true)
            {
                node = (Node)e.Data.GetData(typeof(Node));
                // EN NODE TENGO EL RUBRO QUE ESTOY IMPORTANDO
                if ((node.Tag.ToString().Length) > 2)
                    CODI = Strings.Mid(node.Tag.ToString(), 1, (node.Tag.ToString().Length) - 2);
                else
                    CODI = node.Tag.ToString();

                foreach (DevComponents.AdvTree.Cell droppedCell in node.Cells)
                {
                    sb.Append(droppedCell.Text);
                    sb.Append(", ");
                }

                if (sb.Length > 0)
                    sb.Length -= 2;
            }
            else if (e.Data.GetDataPresent(typeof(Node[])) == true)
            {
                Node[] nodes = (Node[])e.Data.GetData(typeof(Node[]));
                foreach (Node node1 in nodes)
                {
                    foreach (DevComponents.AdvTree.Cell droppedCell in node1.Cells)
                    {
                        sb.Append(droppedCell.Text);
                        sb.Append(", ");
                    }

                    if (sb.Length > 0)
                        sb.Length -= 2;
                }
            }
            // cell.GridRow.GridPanel
            GridPanel panel = cell.GridPanel;

            GridRow FILAACT = cell.GridRow;
            if (FILAACT is null) return;


            var ArFamilias = new string[501];
            var ArTipos = new string[501];
            var ArPropiedades = new string[501];
            var I = default(int);
            ArFamilias[0] = "";
            ArTipos[0] = "";
            ArPropiedades[0] = "";




            if (panel.GridPanel.SuperGrid.Name == "SGAsociados" & node.TreeControl.Name == "AdvCategorias")
            {
                //System.Windows.Forms.MessageBox.Show(panel.GridPanel.SuperGrid.Name + " " + node.TreeControl.Name, "");
                if (FILAACT.Cells[2].Value is null) FILAACT.Cells[2].Value = "";
                if (cell.ColumnIndex == 2) {
                    if ((string)FILAACT.Cells[2].Value == "")
                    {
                        //System.Windows.Forms.MessageBox.Show("esta en categorias, correcto", "");
                        if (cell.RowIndex == panel.Rows.Count - 1)
                        {
                            GridRow filaNueva = new GridRow("", "", node.Text, "", "", "", "","");
                            panel.Rows.Insert(FILAACT.Index, filaNueva);

                            selecciona_categoria((string)node.Text);
                            if (TipoSeleccionado is null) return;
                            if (FamiliaSeleccionada is null) return;
                            I = 1;
                            foreach (var dato in TipoSeleccionado)
                            {
                                ArTipos[I] = dato.Tipo;
                                I++;
                            }
                            I = 1;
                            foreach (var dato in FamiliaSeleccionada)
                            {
                                ArFamilias[I] = dato.Familia;
                                I++;
                            }

                            filaNueva.Cells[3].EditorType = typeof(FragrantComboBox);
                            filaNueva.Cells[3].EditorParams = new object[] { ArFamilias };
                            filaNueva.Cells[4].EditorType = typeof(FragrantComboBox);
                            filaNueva.Cells[4].EditorParams = new object[] { ArTipos };
                            FILAACT = filaNueva;
                        }
                        else {

                            FILAACT.Cells[2].Value = node.Text;
                            FILAACT.Cells[3].Value = "";
                            FILAACT.Cells[4].Value = "";
                    }
                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + FILAACT.Cells[2].Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FILAACT.Cells[2].Value = node.Text;
                        FILAACT.Cells[3].Value = "";
                        FILAACT.Cells[4].Value = "";
                    }

                    selecciona_categoria((string)node.Text);
                    if (TipoSeleccionado is null) return;
                    if (FamiliaSeleccionada is null) return;
                    I = 1;
                    foreach (var dato in TipoSeleccionado)
                    {
                        ArTipos[I] = dato.Tipo;
                        I++;
                    }
                    I = 1;
                    foreach (var dato in FamiliaSeleccionada)
                    {
                        ArFamilias[I] = dato.Familia;
                        I++;
                    }

                    FILAACT.Cells[3].EditorType = typeof(FragrantComboBox);
                    FILAACT.Cells[3].EditorParams = new object[] { ArFamilias };
                    FILAACT.Cells[4].EditorType = typeof(FragrantComboBox);
                    FILAACT.Cells[4].EditorParams = new object[] { ArTipos };


                }

                //si existen datos?
                bool vacios = true;
                for (int x = 0; x <= 7; x++)
                {
                    if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                    if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                }

                if (!vacios)
                {
                    string valorF = "";
                    if (FILAACT.Cells[6].Value.ToString() == "Igual") valorF = "=";
                    if (FILAACT.Cells[6].Value.ToString() == "Diferente") valorF = "!";
                    if (FILAACT.Cells[6].Value.ToString() == "" && FILAACT.Cells[7].Value.ToString() != "") valorF = "?";
                    if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    GuardarAsociado(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), FILAACT.Cells[4].Value.ToString(), FILAACT.Cells[5].Value.ToString(), valorF + FILAACT.Cells[7].Value.ToString());
                }



            }


            if (panel.GridPanel.SuperGrid.Name == "SGAsociados" & node.TreeControl.Name == "AdvFamilias")
            {
                if (FILAACT.Cells[3].Value is null) FILAACT.Cells[3].Value = "";
                if (cell.ColumnIndex == 3)
                {
                    if ((string)FILAACT.Cells[3].Value == "")
                    {
                        if (cell.RowIndex == panel.Rows.Count - 1)
                        {
                            GridRow filaNueva = new GridRow("", "", "", node.Text, "", "", "","");
                            panel.Rows.Insert(FILAACT.Index, filaNueva);
                            
                            if (TotalElementos is null) return;
                            var FitroCategoria1 = from l in TotalElementos
                                                 where l.Familia == (string)node.Text
                                                 select new
                                                 {
                                                     Categoria = l.Categoria,
                                                 };
                            if (FitroCategoria1.Count() != 0)
                            {
                                var Dato = FitroCategoria1.First();
                                filaNueva.Cells[2].Value = Dato.Categoria;
                                filaNueva.Cells[4].Value = "";
                            }
                            selecciona_categoria((string)filaNueva.Cells[2].Value);
                            selecciona_familia((string)node.Text);
                            I = 1;
                            if (TipoSeleccionado is null) { }
                            else
                                foreach (var dato in TipoSeleccionado)
                                {
                                    ArTipos[I] = dato.Tipo;
                                    I++;
                                }
                            filaNueva.Cells[4].EditorType = typeof(FragrantComboBox);
                            filaNueva.Cells[4].EditorParams = new object[] { ArTipos };
                            FILAACT = filaNueva;
                        }
                        else {
                            FILAACT.Cells[3].Value = node.Text;
                        }

                        

                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + FILAACT.Cells[3].Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                       FILAACT.Cells[3].Value = node.Text;
                    }


                    if (TotalElementos is null) return;
                    var FitroCategoria = from l in TotalElementos
                                         where l.Familia == (string)node.Text
                                         select new
                                         {
                                             Categoria = l.Categoria,
                                         };
                    if (FitroCategoria.Count() != 0)
                    {
                        var Dato = FitroCategoria.First();
                        FILAACT.Cells[2].Value = Dato.Categoria;
                        FILAACT.Cells[4].Value = "";
                    }
                    selecciona_categoria((string)FILAACT.Cells[2].Value);
                    selecciona_familia((string)node.Text);
                    I = 1;
                    if (TipoSeleccionado is null) { }
                    else
                        foreach (var dato in TipoSeleccionado)
                        {
                            ArTipos[I] = dato.Tipo;
                            I++;
                        }
                    FILAACT.Cells[4].EditorType = typeof(FragrantComboBox);
                    FILAACT.Cells[4].EditorParams = new object[] { ArTipos };



                }

                //si existen datos?
                bool vacios = true;
                for (int x = 0; x <= 7; x++)
                {
                    if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                    if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                }

                if (!vacios)
                {
                    string valorF = "";
                    if (FILAACT.Cells[6].Value.ToString() == "Igual") valorF = "=";
                    if (FILAACT.Cells[6].Value.ToString() == "Diferente") valorF = "!";
                    if (FILAACT.Cells[6].Value.ToString() == "" && FILAACT.Cells[7].Value.ToString() != "") valorF = "?";
                    if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

                    GuardarAsociado(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), FILAACT.Cells[4].Value.ToString(), FILAACT.Cells[5].Value.ToString(), valorF + FILAACT.Cells[7].Value.ToString());
                }


            }

            if (panel.GridPanel.SuperGrid.Name == "SGAsociados" & node.TreeControl.Name == "AdvTreeTipos")
            {
                if (FILAACT.Cells[4].Value is null) FILAACT.Cells[4].Value = "";
                if (cell.ColumnIndex == 4)
                {
                    if ((string)FILAACT.Cells[4].Value == "")
                    {
                        if (cell.RowIndex == panel.Rows.Count - 1)
                        {
                            GridRow filaNueva = new GridRow("", "", "", "", node.Text, "", "","");
                            panel.Rows.Insert(FILAACT.Index, filaNueva);
                            var FitroFamCat1 = from l in TotalElementos
                                              where l.Tipo == (string)node.Text
                                              select new
                                              {
                                                  Categoria = l.Categoria,
                                                  Familia = l.Familia,
                                              };
                            if (FitroFamCat1.Count() != 0)
                            {
                                var Dato = FitroFamCat1.First();
                                filaNueva.Cells[2].Value = Dato.Categoria;
                                filaNueva.Cells[3].Value = Dato.Familia;
                            }
                            FILAACT = filaNueva;
                        }

                        FILAACT.Cells[4].Value = node.Text;

                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + FILAACT.Cells[4].Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FILAACT.Cells[4].Value = node.Text;
                    }


                    var FitroFamCat = from l in TotalElementos
                                      where l.Tipo == (string)node.Text
                                      select new
                                      {
                                          Categoria = l.Categoria,
                                          Familia = l.Familia,
                                      };
                    if (FitroFamCat.Count() != 0)
                    {
                        var Dato = FitroFamCat.First();
                        FILAACT.Cells[2].Value = Dato.Categoria;
                        FILAACT.Cells[3].Value = Dato.Familia;
                    }

                }


                //si existen datos?
                bool vacios = true;
                for (int x = 0; x <= 7; x++)
                {
                    if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                    if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                }

                if (!vacios)
                {
                    string valorF = "";
                    if (FILAACT.Cells[6].Value.ToString() == "Igual") valorF = "=";
                    if (FILAACT.Cells[6].Value.ToString() == "Diferente") valorF = "!";
                    if (FILAACT.Cells[6].Value.ToString() == "" && FILAACT.Cells[7].Value.ToString() != "") valorF = "?";
                    if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string cadenaTipoGuardar = FILAACT.Cells[4].Value.ToString().Replace("'", "''");
                    GuardarAsociado(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), cadenaTipoGuardar, FILAACT.Cells[5].Value.ToString(),  valorF + FILAACT.Cells[7].Value.ToString());
                }



            }
            
            if (panel.GridPanel.SuperGrid.Name == "SGAsociados" & node.TreeControl.Name == "AdvParametrosCompartidos")
            {
                if (cell.ColumnIndex == 5)
                {
                    if (FILAACT.Cells[5].Value is null) FILAACT.Cells[5].Value = "";

                    if ((string)FILAACT.Cells[5].Value == "")
                    {
                        //System.Windows.Forms.MessageBox.Show("esta en tipos, correcto", "");
                        /*if (cell.RowIndex == panel.Rows.Count - 1)
                        {
                            GridRow filaNueva = new GridRow("", "", "", "", "", node.Text, "", "");
                            panel.Rows.Insert(FILAACT.Index, filaNueva);
                        }*/
                        if (cell.RowIndex != panel.Rows.Count - 1)
                            FILAACT.Cells[5].Value = node.Text;
                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + FILAACT.Cells[5].Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FILAACT.Cells[5].Value = node.Text;
                    }
                }

                //si existen datos?
                bool vacios = true;
                for (int x = 0; x <= 7; x++)
                {
                    if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                    if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                }

                if (!vacios)
                {
                    string valorF = "";
                    if (FILAACT.Cells[6].Value.ToString() == "Igual") valorF = "=";
                    if (FILAACT.Cells[6].Value.ToString() == "Diferente") valorF = "!";
                    if (FILAACT.Cells[6].Value.ToString() == "" && FILAACT.Cells[7].Value.ToString() != "") valorF = "?";
                    if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string cadenaTipoGuardar = FILAACT.Cells[4].Value.ToString().Replace("'", "''");
                    GuardarAsociado(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), cadenaTipoGuardar, FILAACT.Cells[5].Value.ToString(), valorF + FILAACT.Cells[7].Value.ToString());
                }


            }
            
            if (panel.GridPanel.SuperGrid.Name == "SGAsociados" & node.TreeControl.Name == "AdvPropiedades")
            {
                if (cell.ColumnIndex == 5)
                {
                    if (FILAACT.Cells[5].Value is null) FILAACT.Cells[5].Value = "";

                    if ((string)FILAACT.Cells[5].Value == "")
                    {
                        //System.Windows.Forms.MessageBox.Show("esta en tipos, correcto", "");
                        if (cell.RowIndex == panel.Rows.Count - 1)
                        {
                            GridRow filaNueva = new GridRow("", "", "", "", "", node.Text, "", "");
                            panel.Rows.Insert(FILAACT.Index, filaNueva);
                        }
                        FILAACT.Cells[5].Value = node.Text;
                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + FILAACT.Cells[5].Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FILAACT.Cells[5].Value = node.Text;
                    }
                }
                //si existen datos?
                bool vacios = true;
                for (int x = 0; x <= 7; x++)
                {
                    if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                    if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                }
                if (!vacios)
                {
                    string valorF = "";
                    if (FILAACT.Cells[6].Value.ToString() == "Igual") valorF = "=";
                    if (FILAACT.Cells[6].Value.ToString() == "Diferente") valorF = "!";
                    if (FILAACT.Cells[6].Value.ToString() == "" && FILAACT.Cells[7].Value.ToString() != "") valorF = "?";
                    if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string cadenaTipoGuardar = FILAACT.Cells[4].Value.ToString().Replace("'", "''");
                    GuardarAsociado(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), cadenaTipoGuardar, FILAACT.Cells[5].Value.ToString(), valorF + FILAACT.Cells[7].Value.ToString());
                }
            }


            if (panel.GridPanel.SuperGrid.Name == "SGestructuraC" & node.TreeControl.Name == "AdvPropiedades")
            {
                if (cell.ColumnIndex == 3)
                {
                    if (FILAACT.Cells[3].Value is null) FILAACT.Cells[3].Value = "";

                    if ((string)FILAACT.Cells[3].Value == "")
                    {
                        //System.Windows.Forms.MessageBox.Show("esta en tipos, correcto", "");
                        if (cell.RowIndex == panel.Rows.Count - 1)
                        {
                            GridRow filaNueva = new GridRow("", "", "Nivel " + panel.Rows.Count.ToString() , node.Text, false);
                            panel.Rows.Insert(FILAACT.Index, filaNueva);
                            FILAACT = filaNueva;
                        }
                        FILAACT.Cells[3].Value = node.Text;
                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + FILAACT.Cells[3].Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FILAACT.Cells[3].Value = node.Text;
                    }
                }

                bool vacios = true;
                for (int x = 0; x <= 3; x++)
                {
                    if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                    if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                }
                if (FILAACT.Cells[4].Value is null) FILAACT.Cells[4].Value = false;

                if (!vacios)
                {
                    if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string campoMostrar = "";
                    if ((bool)FILAACT.Cells[4].Value == true) campoMostrar = "true";
                    if ((bool)FILAACT.Cells[4].Value == false) campoMostrar = "false";
                    GuardarEstructura(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), campoMostrar);
                }
            }
            if (panel.GridPanel.SuperGrid.Name == "SGestructuraC" & node.TreeControl.Name == "AdvParametrosCompartidos")
            {
                if (cell.ColumnIndex == 3)
                {
                    if (FILAACT.Cells[3].Value is null) FILAACT.Cells[3].Value = "";
                    if ((string)FILAACT.Cells[3].Value == "")
                    {
                        //System.Windows.Forms.MessageBox.Show("esta en tipos, correcto", "");
                        if (cell.RowIndex == panel.Rows.Count - 1)
                        {
                            GridRow filaNueva = new GridRow("", "", "Nivel " + panel.Rows.Count.ToString(), node.Text, false);
                            panel.Rows.Insert(FILAACT.Index, filaNueva);
                            FILAACT = filaNueva;
                        }
                        FILAACT.Cells[3].Value = node.Text;
                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + FILAACT.Cells[3].Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FILAACT.Cells[3].Value = node.Text;
                    }
                }
                bool vacios = true;
                for (int x = 0; x <= 3; x++)
                {
                    if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                    if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                }
                if (FILAACT.Cells[4].Value is null) FILAACT.Cells[4].Value = false;
                if (!vacios)
                {
                    if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    string campoMostrar = "";
                    if ((bool)FILAACT.Cells[4].Value == true) campoMostrar = "true";
                    if ((bool)FILAACT.Cells[4].Value == false) campoMostrar = "false";
                    GuardarEstructura(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), campoMostrar);
                }
            }



            if (panel.GridPanel.SuperGrid.Name == "SGFormulas" & node.TreeControl.Name == "AdvPropiedades")
            {
                if (cell.ColumnIndex >= 1 && cell.ColumnIndex <= 5)
                {
                    if (cell.Value is null) cell.Value = "";

                    if (cell.Value.ToString().Length > 1)
                        if (Microsoft.VisualBasic.Strings.Mid(cell.Value.ToString(), 1, 1) == "=")
                        {
                            System.Windows.Forms.MessageBox.Show("Debe eliminar primero la formula");
                            return;
                        }


                    if ((string)cell.Value == "")
                    {
                        cell.Value = node.Text;
                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + cell.Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        cell.Value = node.Text;
                    }
                }
                
                bool vacios = true;
                for (int x = 0; x <= 5; x++)
                {
                    if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                    if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                }

                if (!vacios)
                {
                    if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    GuardarCalculo(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[1].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), FILAACT.Cells[4].Value.ToString(), FILAACT.Cells[5].Value.ToString());
                }


            }
            if (panel.GridPanel.SuperGrid.Name == "SGFormulas" & node.TreeControl.Name == "AdvParametrosCompartidos")
            {

                if (cell.ColumnIndex >= 1 && cell.ColumnIndex <= 5)
                {
                    
                    if (cell.Value is null) cell.Value = "";

                    if (cell.Value.ToString().Length > 1)
                        if (Microsoft.VisualBasic.Strings.Mid(cell.Value.ToString(), 1, 1) == "=") {
                            System.Windows.Forms.MessageBox.Show("Debe eliminar primero la formula");
                            return;
                        }

                    if ((string)cell.Value == "")
                    {
                        cell.Value = node.Text;
                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + cell.Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        cell.Value = node.Text;
                    }
                }

                bool vacios = true;
                for (int x = 0; x <= 5; x++)
                {
                    if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                    if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                }

                if (!vacios)
                {
                    if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    GuardarCalculo(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[1].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), FILAACT.Cells[4].Value.ToString(), FILAACT.Cells[5].Value.ToString());
                }

            }








            if (panel.GridPanel.SuperGrid.Name == "SGDetFormulas" & node.TreeControl.Name == "AdvPropiedades")
            {
                if (cell.ColumnIndex == 2)
                {
                    if (cell.Value is null) cell.Value = "";
                    if ((string)cell.Value == "")
                    {
                        if (cell.RowIndex == panel.Rows.Count - 1)
                        {
                            GridRow filaNueva = new GridRow("", "", node.Text, "","");
                            panel.Rows.Insert(FILAACT.Index, filaNueva);
                            FILAACT = filaNueva;
                        }
                        cell.Value = node.Text;
                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + cell.Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        cell.Value = node.Text;
                    }
                }
                actualiza_det_formula();
            }
            if (panel.GridPanel.SuperGrid.Name == "SGDetFormulas" & node.TreeControl.Name == "AdvParametrosCompartidos")
            {
                if (cell.ColumnIndex == 2 )
                {
                    if (cell.Value is null) cell.Value = "";

                    if ((string)cell.Value == "")
                    {
                        //System.Windows.Forms.MessageBox.Show("esta en tipos, correcto", "");
                        if (cell.RowIndex == panel.Rows.Count - 1)
                        {
                            GridRow filaNueva = new GridRow("", "", node.Text, "","");
                            panel.Rows.Insert(FILAACT.Index, filaNueva);
                            FILAACT = filaNueva;
                        }
                        cell.Value = node.Text;
                    }
                    else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar " + cell.Value + " con " + node.Text + " ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        cell.Value = node.Text;
                    }
                }
                actualiza_det_formula();
            }
            

            if (panel.GridPanel.SuperGrid.Name == "SGFormulas" & node.TreeControl.Name == "AdvTree7")
            {
                if (FILAACT.Index == 1) return;
                if (FILAACT.Cells[1].Value.ToString() == "" && FILAACT.Cells[2].Value.ToString() == "" && FILAACT.Cells[3].Value.ToString() == "" && FILAACT.Cells[4].Value.ToString() == "" && FILAACT.Cells[5].Value.ToString() == "")
                {

                    System.Data.DataTable dt = new System.Data.DataTable();
                    System.Data.DataTable dtdet = new System.Data.DataTable();
                    var basdat = new ConexionBD();

                    dt = basdat.TablaConfCalculo(AdvTree7.SelectedNode.Tag.ToString());
                    dtdet = basdat.TablaConfCalculoDetalle(AdvTree7.SelectedNode.Tag.ToString());

                    if (dt.Rows.Count > 0) {

                        FILAACT.Cells[1].Value = dt.Rows[0]["Descripcion"].ToString().Trim();
                        FILAACT.Cells[2].Value = dt.Rows[0]["Cantidad"].ToString().Trim();
                        FILAACT.Cells[3].Value = dt.Rows[0]["Longitud"].ToString().Trim();
                        FILAACT.Cells[4].Value = dt.Rows[0]["Ancho"].ToString().Trim();
                        FILAACT.Cells[5].Value = dt.Rows[0]["alto"].ToString().Trim();


                        bool vacios = true;
                        for (int x = 0; x <= 5; x++)
                        {
                            if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                            if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                        }

                        if (!vacios)
                        {
                            if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                            GuardarCalculo(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[1].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), FILAACT.Cells[4].Value.ToString(), FILAACT.Cells[5].Value.ToString());
                        }


                        /*if (filaAux.Cells[0].Value.ToString() == "") filaAux.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                        ListaCalculoDetalle.Add(new LCalculoDetalle { CodCalculoDetalle = filaAux.Cells[0].Value.ToString(), CodCalculo = filaP.Cells[0].Value.ToString(), CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, TipoCampo = "Longitud", Campo = filaAux.Cells[2].Value.ToString(), Operacion = filaAux.Cells[3].Value.ToString(), Posicion = filaAux.Cells[4].Value.ToString() });
                        if (filaAux.Index == 0) celdaP.Value = "" + celdaP.Value.ToString() + filaAux.Cells[2].Value + filaAux.Cells[3].Value;
                        else celdaP.Value = "(" + celdaP.Value.ToString() + filaAux.Cells[2].Value + ")" + filaAux.Cells[3].Value;*/
                        foreach (DataRow item in dtdet.Rows) {
                            string AuxCodigo = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                            GuardarCalculoDetalle(AuxCodigo, FILAACT.Cells[0].Value.ToString(), item["TipoCampo"].ToString().Trim(), item["Campo"].ToString().Trim(), item["Operacion"].ToString().Trim(), item["Posicion"].ToString().Trim());
                        }

                        actualizar_datosCalculoDetalle();
                        actualiza_det_formulaDet();
                    }


                }
                else if (System.Windows.Forms.MessageBox.Show("Desea Reemplazar el registro actual ?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {

                    System.Data.DataTable dt = new System.Data.DataTable();
                    System.Data.DataTable dtdet = new System.Data.DataTable();
                    var basdat = new ConexionBD();

                    dt = basdat.TablaConfCalculo(AdvTree7.SelectedNode.Tag.ToString());
                    dtdet = basdat.TablaConfCalculoDetalle(AdvTree7.SelectedNode.Tag.ToString());

                    if (dt.Rows.Count > 0)
                    {
                        FILAACT.Cells[0].Value = "";
                        FILAACT.Cells[1].Value = dt.Rows[0]["Descripcion"].ToString().Trim();
                        FILAACT.Cells[2].Value = dt.Rows[0]["Cantidad"].ToString().Trim();
                        FILAACT.Cells[3].Value = dt.Rows[0]["Longitud"].ToString().Trim();
                        FILAACT.Cells[4].Value = dt.Rows[0]["Ancho"].ToString().Trim();
                        FILAACT.Cells[5].Value = dt.Rows[0]["alto"].ToString().Trim();

                        bool vacios = true;
                        for (int x = 0; x <= 5; x++)
                        {
                            if (FILAACT.Cells[x].Value is null) FILAACT.Cells[x].Value = "";
                            if (FILAACT.Cells[x].Value.ToString() != "") vacios = false;
                        }

                        if (!vacios)
                        {
                            if (FILAACT.Cells[0].Value.ToString() == "") FILAACT.Cells[0].Value = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                            GuardarCalculo(FILAACT.Cells[0].Value.ToString(), FILAACT.Cells[1].Value.ToString(), FILAACT.Cells[2].Value.ToString(), FILAACT.Cells[3].Value.ToString(), FILAACT.Cells[4].Value.ToString(), FILAACT.Cells[5].Value.ToString());
                        }

                        foreach (DataRow item in dtdet.Rows)
                        {
                            string AuxCodigo = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                            GuardarCalculoDetalle(AuxCodigo, FILAACT.Cells[0].Value.ToString(), item["TipoCampo"].ToString().Trim(), item["Campo"].ToString().Trim(), item["Operacion"].ToString().Trim(), item["Posicion"].ToString().Trim());
                        }

                        actualizar_datosCalculoDetalle();
                        actualiza_det_formulaDet();
                    }
                    
                    //cell.Value = node.Text;
                }

            }


            if (panel.GridPanel.SuperGrid.Name == "SGMatC" & node.TreeControl.Name == "AdvTree4")
            {

            }


        }















        #endregion

        private void ButtonItem1_Click(object sender, EventArgs e)
        {
            




        }

        public string cadenaMedicionG = "";

        private void ButtonItem46_Click(object sender, EventArgs e)
        {
            GridPanel Panel = (GridPanel)SGMediciones.PrimaryGrid;
            GridRow FilaActual = (GridRow)Panel.ActiveRow;
            GridRow PrimeraFila = (GridRow)Panel.Rows[0];
            var basdat = new ConexionBD();
            GridPanel Panel1 = (GridPanel)SGPresupC.PrimaryGrid;
            GridRow filaauxPadre = (GridRow)Panel1.ActiveRow;

            if (Item_actual == "") return;
            if (PrimeraFila is null) return;
            
            if (PrimeraFila.Cells[2].Value is null) return;
            //Si no existen Items personalizados y estoy en cualquier parte
            if (PrimeraFila.Cells[2].Value.ToString() != "Metrados Personalizados")
            {
                //creo la fila para metrados personalizados
                string CodigoUni = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Metrados Personalizados", Cantidad = "", Longitud = "", Ancho = "", Alto = "", Total = "1.00", Detalle = "Titulo de Nivel 1", Vinculo = "Personalizado", UniqueId = "", PhantomParentId = "", Nivel = 1, Tipo = "Titulo" });
                //GridRow FilaInserttar = new GridRow();
                
                basdat.Conexion();
                //basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);
                basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni, "Metrados Personalizados", "", "", "", "", "1.00", "Titulo de Nivel 1", "Personalizado", "", "", 1, "Titulo", EmailUsuario);



                basdat.Conexion();
                //GuardarMedicion(CodigoUni, "Metrados Personalizados", "", "", "", "", "1.00", "Titulo de Nivel 1", "Personalizado", "", "", 1, "Titulo");
                GridRow filaInsertar = new GridRow(CodigoUni, "", "Metrados Personalizados", "", "", "", "", "1.00", "Titulo de Nivel 1", "Personalizado", "Titulo", "", "1");
                //var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(181, 185, 168), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(202, 223, 218), 90);
                //System.Drawing.Color.FromArgb(202, 223, 208), 90);
                filaInsertar.CellStyles.Default.Background = Background;
                Panel.Rows.Insert(0, filaInsertar);
                PrimeraFila = filaInsertar;
                string CodigoUni2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                filaInsertar = new GridRow(CodigoUni2, "", "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel 2", "Personalizado", "Medicion", "", "2");
                ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni2, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Nuevo Item", Cantidad = "1.00", Longitud = "", Ancho = "", Alto = "", Total = "1.00", Detalle = "Metrado de Nivel 2", Vinculo = "Personalizado", UniqueId = "", PhantomParentId = CodigoUni, Nivel = 2, Tipo = "Medicion" });
                PrimeraFila.Rows.Add(filaInsertar);
                Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                //System.Drawing.Color.FromArgb(202, 223, 208), 90);
                filaInsertar.CellStyles.Default.Background = Background;
                PrimeraFila.Expanded = true;

                basdat.Conexion();
                //basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);
                basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni2, "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel 2", "Personalizado", "", CodigoUni, 2, "Medicion", EmailUsuario);
                basdat.Conexion();





                GridRow fpadre = (GridRow)FilaActual;
                /*double sumatoria = 0;
                if (fpadre.Parent.GetType().Name != "GridPanel")
                    do
                    {
                        fpadre = (GridRow)fpadre.Parent;
                        double sumatoria1 = 0;
                        if (fpadre.Rows.Count != 0)
                            foreach (GridRow fac in fpadre.Rows)
                                sumatoria1 += Convert.ToDouble(fac.Cells[7].Value.ToString());
                        fpadre.Cells[7].Value = sumatoria1.ToString("N2");
                        basdat.ActualizarMedicion1(fpadre.Cells[0].Value.ToString(), fpadre.Cells[2].Value.ToString(), fpadre.Cells[3].Value.ToString(), fpadre.Cells[4].Value.ToString(), fpadre.Cells[5].Value.ToString(), fpadre.Cells[6].Value.ToString(), fpadre.Cells[7].Value.ToString(), fpadre.Cells[8].Value.ToString());
                        sumatoria += sumatoria1;
                    } while (fpadre.Parent.GetType().Name != "GridPanel");*/

                double CalculoTotal = 0.0;
                foreach (GridRow Itfila in SGMediciones.PrimaryGrid.Rows)
                {
                    if (!(Itfila.Cells[7].Value is null))
                        CalculoTotal += Convert.ToDouble(Itfila.Cells[7].Value.ToString());
                }

                filaauxPadre.Cells[4].Value = CalculoTotal.ToString("N2");
                recalcular_pres_desdeFila(filaauxPadre);
                SuperTabItem5.Text = "METRADO " + CalculoTotal.ToString("N2");
                GuardarMetrado(CalculoTotal.ToString());






                //GuardarMedicion(CodigoUni2, "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel 1", "Personalizado", "", CodigoUni, 2, "Medicion");

            }
            else {
                if (FilaActual.Cells[9].Value.ToString() == "Personalizado") //verificar si se encuentra en un metrado personalizado
                {
                    
                    
                    GridRow filaPapa = (GridRow)FilaActual.Parent;
                    int nivelToca = Convert.ToInt32(filaPapa.Cells[12].Value.ToString()) + 1;
                    string CodigoUni2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    GridRow filaInsertar = new GridRow(CodigoUni2, "", "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel " + nivelToca.ToString(), "Personalizado", "Medicion", filaPapa.Cells[0].Value.ToString(), nivelToca.ToString());
                    ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni2, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Nuevo Item", Cantidad = "1.00", Longitud = "", Ancho = "", Alto = "", Total = "1.00", Detalle = "Metrado de Nivel "+ nivelToca.ToString(), Vinculo = "Personalizado", UniqueId = "", PhantomParentId = filaPapa.Cells[0].Value.ToString(), Nivel = nivelToca, Tipo = "Medicion" });
                    filaPapa.Rows.Add(filaInsertar);
                    filaPapa.Expanded = true;
                    Panel.SetActiveRow(filaInsertar);
                    filaInsertar.EnsureVisible();
                    filaInsertar.IsSelected = true;

                    var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                    filaInsertar.CellStyles.Default.Background = Background;
                    basdat.Conexion();
                    //basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);
                    basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni2, "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel " + nivelToca.ToString(), "Personalizado", "", filaPapa.Cells[0].Value.ToString(), nivelToca, "Medicion", EmailUsuario);
                    basdat.Conexion();


                    GridRow fpadre = (GridRow)filaInsertar;
                    double sumatoria = 0;
                    if (fpadre.Parent.GetType().Name != "GridPanel")
                        do
                        {
                            fpadre = (GridRow)fpadre.Parent;
                            double sumatoria1 = 0;
                            if (fpadre.Rows.Count != 0)
                                foreach (GridRow fac in fpadre.Rows)
                                    sumatoria1 += Convert.ToDouble(fac.Cells[7].Value.ToString());
                            fpadre.Cells[7].Value = sumatoria1.ToString("N2");
                            basdat.ActualizarMedicion1(fpadre.Cells[0].Value.ToString(), fpadre.Cells[2].Value.ToString(), fpadre.Cells[3].Value.ToString(), fpadre.Cells[4].Value.ToString(), fpadre.Cells[5].Value.ToString(), fpadre.Cells[6].Value.ToString(), fpadre.Cells[7].Value.ToString(), fpadre.Cells[8].Value.ToString());
                            sumatoria += sumatoria1;
                        } while (fpadre.Parent.GetType().Name != "GridPanel");

                    double CalculoTotal = 0.0;
                    foreach (GridRow Itfila in SGMediciones.PrimaryGrid.Rows)
                    {
                        if (!(Itfila.Cells[7].Value is null))
                            CalculoTotal += Convert.ToDouble(Itfila.Cells[7].Value.ToString());
                    }

                    filaauxPadre.Cells[4].Value = CalculoTotal.ToString("N2");
                    recalcular_pres_desdeFila(filaauxPadre);
                    SuperTabItem5.Text = "METRADO " + CalculoTotal.ToString("N2");
                    GuardarMetrado(CalculoTotal.ToString());



                }
                else
                { //si se encuentra en cualquier parte y ya existen metrados personalizados


                    string CodigoUni2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    GridRow filaInsertar = new GridRow(CodigoUni2, "", "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel 2", "Personalizado", "Medicion", PrimeraFila.Cells[0].Value.ToString(), "2");
                    ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni2, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Nuevo Item", Cantidad = "1.00", Longitud = "", Ancho = "", Alto = "", Total = "1.00", Detalle = "Metrado de Nivel 2", Vinculo = "Personalizado", UniqueId = "", PhantomParentId = PrimeraFila.Cells[0].Value.ToString(), Nivel = 2, Tipo = "Medicion" });
                    PrimeraFila.Rows.Add(filaInsertar);
                    PrimeraFila.Expanded = true;
                    Panel.SetActiveRow(filaInsertar);
                    filaInsertar.EnsureVisible();
                    filaInsertar.IsSelected = true;

                    var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                    filaInsertar.CellStyles.Default.Background = Background;
                    basdat.Conexion();
                    //basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);
                    basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni2, "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel 2", "Personalizado", "", PrimeraFila.Cells[0].Value.ToString(), 2, "Medicion", EmailUsuario);
                    basdat.Conexion();



                    GridRow fpadre = (GridRow)filaInsertar;
                    double sumatoria = 0;
                    if (fpadre.Parent.GetType().Name != "GridPanel")
                        do
                        {
                            fpadre = (GridRow)fpadre.Parent;
                            double sumatoria1 = 0;
                            if (fpadre.Rows.Count != 0)
                                foreach (GridRow fac in fpadre.Rows)
                                    sumatoria1 += Convert.ToDouble(fac.Cells[7].Value.ToString());
                            fpadre.Cells[7].Value = sumatoria1.ToString("N2");
                            basdat.ActualizarMedicion1(fpadre.Cells[0].Value.ToString(), fpadre.Cells[2].Value.ToString(), fpadre.Cells[3].Value.ToString(), fpadre.Cells[4].Value.ToString(), fpadre.Cells[5].Value.ToString(), fpadre.Cells[6].Value.ToString(), fpadre.Cells[7].Value.ToString(), fpadre.Cells[8].Value.ToString());
                            sumatoria += sumatoria1;
                        } while (fpadre.Parent.GetType().Name != "GridPanel");

                    double CalculoTotal = 0.0;
                    foreach (GridRow Itfila in SGMediciones.PrimaryGrid.Rows)
                    {
                        if (!(Itfila.Cells[7].Value is null))
                            CalculoTotal += Convert.ToDouble(Itfila.Cells[7].Value.ToString());
                    }

                    filaauxPadre.Cells[4].Value = CalculoTotal.ToString("N2");
                    recalcular_pres_desdeFila(filaauxPadre);
                    SuperTabItem5.Text = "METRADO " + CalculoTotal.ToString("N2");
                    GuardarMetrado(CalculoTotal.ToString());


                }
                
                
            }


            //CargarMediciones();

            //si la fila actual es un metrado personalizado?
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            //bRowser.Load(textBoxX2.Text);

            cadenaMedicionG = "";
            var basdat = new ConexionBD();
            basdat.Conexion();
            //basdat.GuardarPlano(Presupuesto_actual, SubPresupuesto_actual, Item_actual, "001", "rvt", "urn1", "Urn2", "email");
            //basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);

            /*"RequestId":"Modificar",
   "ObjectName":"dbo.S10_01_SubpresupuestoDetalleCalculoDetalle_Actualizar '0501001','001','000000000000034','dvRmC9IjYE6FMR1SH24ewg==','888','Longitud','Volumen','*','1','jleon@s10peru.com'",
   "SignalRConnectionID":"da1f6cea-96c3-44e8-9761-6d0b310acd46",
   "SecurityUserId":580,
   "IsBulkTransaction": true,
   "Data":"[{\"Id\":1,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2}…….."*/


            /*{\"Id\":1,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2},{\"P\":\"MQ==\",\"O\":3},{\"P\":\"888\",\"O\":4},{\"P\":\"Titulo\",\"O\":5},{\"P\":\"Volumen\",\"O\":6},{\"P\":\"*\",\"O\":7},{\"P\":\"1\",\"O\":8},{\"P\":\jleon@s10peru.com\,\"O\":9}]},
              {\"Id\":2,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2},{\"P\":\"Mg==\",\"O\":3},{\"P\":\"888\",\"O\":4},{\"P\":\"Titulo\",\"O\":5},{\"P\":\"Volumen\",\"O\":6},{\"P\":\"*\",\"O\":7},{\"P\":\"1\",\"O\":8},{\"P\":\jleon@s10peru.com\,\"O\":9}]},
              {\"Id\":3,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2},{\"P\":\"Mw==\",\"O\":3},{\"P\":\"888\",\"O\":4},{\"P\":\"Titulo\",\"O\":5},{\"P\":\"Volumen\",\"O\":6},{\"P\":\"*\",\"O\":7},{\"P\":\"1\",\"O\":8},{\"P\":\jleon@s10peru.com\,\"O\":9}]},
              {\"Id\":4,\"Parameters\":[{\"P\":\"0501001\",\"O\":0},{\"P\":\"001\",\"O\":1},{\"P\":\"000000000000034\",\"O\":2},{\"P\":\"NA==\",\"O\":3},{\"P\":\"888\",\"O\":4},{\"P\":\"Titulo\",\"O\":5},{\"P\":\"Volumen\",\"O\":6},{\"P\":\"*\",\"O\":7},{\"P\":\"1\",\"O\":8},{\"P\":\jleon@s10peru.com\,\"O\":9}]}*/

            System.Data.DataTable dt = new System.Data.DataTable();
            dt = basdat.LmedicionesSubPresupuesto(Presupuesto_actual, SubPresupuesto_actual, Item_actual);
            if (ListaItemsMedicion is null) return;
            int pos=1;
            foreach (var campo1 in ListaItemsMedicion)
            {
                //GuardarMedicion(campo1.CodMedicion, campo1.Descripcion, campo1.Cantidad, campo1.Longitud, campo1.Ancho, campo1.Alto, campo1.Total, campo1.Detalle, campo1.Vinculo, campo1.UniqueId, campo1.PhantomParentId, campo1.Nivel, campo1.Tipo);
                //Thread.Sleep(30);
                //basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, campo1.CodMedicion, campo1.Descripcion, campo1.Cantidad, campo1.Longitud, campo1.Ancho, campo1.Alto, campo1.Total, campo1.Detalle, campo1.Vinculo, campo1.UniqueId, campo1.PhantomParentId, campo1.Nivel, campo1.Tipo, EmailUsuario);
                //basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, campo1.CodMedicion, campo1.Descripcion, campo1.Cantidad, campo1.Longitud, campo1.Ancho, campo1.Alto, campo1.Total, campo1.Detalle, codModelo, campo1.UniqueId, campo1.PhantomParentId, campo1.Nivel, campo1.Tipo, EmailUsuario);
                //if (cadenaMedicionG == "")
                    cadenaMedicionG = cadenaMedicionG + @"{\""Id\"":" + pos.ToString() + @",\""Parameters\"":[{\""P\"":\""0501001\"",\""O\"":0},{\""P\"":\""001\"",\""O\"":1},{\""P\"":\""000000000000034\"",\""O\"":2},{\""P\"":\""MQ==\"",\""O\"":3},{\""P\"":\""888\"",\""O\"":4},{\""P\"":\""Titulo\"",\""O\"":5},{\""P\"":\""Volumen\"",\""O\"":6},{\""P\"":\""*\"",\""O\"":7},{\""P\"":\""1\"",\""O\"":8},{\""P\"":\jleon@s10peru.com\,\""O\"":9}]},";
                pos++;
            }
            cadenaMedicionG = cadenaMedicionG.Substring(0, cadenaMedicionG.Length-1);
            cadenaMedicionG = "";
            foreach (DataRow dat in dt.Rows)
            {
                //ListaItemsMedicion.Add(new LMedicion { CodMedicion = dat["CodMedicion"].ToString().Trim(), CodPresupuesto = dat["CodPresupuesto"].ToString().Trim(), CodSubpresupuesto = dat["CodSubpresupuesto"].ToString().Trim(), Item = dat["Item"].ToString().Trim(), Descripcion = dat["Descripcion"].ToString().Trim(), Cantidad = dat["Cantidad"].ToString().Trim(), Longitud = dat["Longitud"].ToString().Trim(), Ancho = dat["Ancho"].ToString().Trim(), Alto = dat["Alto"].ToString().Trim(), Total = dat["Total"].ToString().Trim(), Detalle = dat["Detalle"].ToString().Trim(), Vinculo = dat["Vinculo"].ToString().Trim(), UniqueId = dat["UniqueId"].ToString().Trim(), PhantomParentId = dat["PhantomParentId"].ToString().Trim(), Nivel = (int)dat["Nivel"], Tipo = dat["Tipo"].ToString().Trim() });
                //if (cadenaMedicionG == "")
                    cadenaMedicionG = cadenaMedicionG + @"{\""Id\"":1,\""Parameters\"":[{\""P\"":\""0501001\"",\""O\"":0},{\""P\"":\""001\"",\""O\"":1},{\""P\"":\""000000000000034\"",\""O\"":2},{\""P\"":\""MQ==\"",\""O\"":3},{\""P\"":\""888\"",\""O\"":4},{\""P\"":\""Titulo\"",\""O\"":5},{\""P\"":\""Volumen\"",\""O\"":6},{\""P\"":\""*\"",\""O\"":7},{\""P\"":\""1\"",\""O\"":8},{\""P\"":\jleon@s10peru.com\,\""O\"":9}]},";

            }

            basdat.Conexion();

            System.Windows.Forms.MessageBox.Show(cadenaMedicionG, "");

        }

        private void MenuComboContraer_Click(object sender, EventArgs e)
        {
            
        }

        private void MenuComboContraer_SelectedIndexChanged(object sender, EventArgs e)
        {
            //System.Windows.Forms.MessageBox.Show(MenuComboContraer.SelectedIndex.ToString());

            int NivelSel = MenuComboContraer.SelectedIndex+1;


            if (NivelSel >= 1)
            {
                foreach (GridRow ff in SGMediciones.PrimaryGrid.Rows)
                    if (!(ff is null)) contraer_todo(ff);
            }


            if (NivelSel >= 1) {
                foreach (GridRow ff in SGMediciones.PrimaryGrid.Rows)
                    if (!(ff is null)) expandir(ff, 1, NivelSel);
            }





        }


        private void contraer_todo(GridRow filaex)
        {
            if (filaex.Rows.Count != 0) {
                filaex.Expanded = false;
                foreach (GridRow ff in filaex.Rows)
                    if (!(ff is null)) contraer_todo(ff);
            } 
        }


        private void expandir(GridRow filaex, int actual, int tope) {
            if (tope >= actual) { 
                if (filaex.Rows.Count != 0) filaex.Expanded = true;
                int act = actual + 1;
                foreach (GridRow ff in filaex.Rows) {
                    if (!(ff is null)) expandir(ff, act, tope);
                }

            }


        }



        //VERIFICAR SI ESXISTE TABLA  ///////////////////////// LLEVA CADENA DE CONEXION  ////////////////////*****************************************************************
        private bool existeTabla(string nombreTabla)
        {
           /* SqlConnectionStringBuilder csb =
                new SqlConnectionStringBuilder();
            csb.DataSource = "(local)\\SQLEXPRESS";
            csb.InitialCatalog = nombreBase;
            csb.IntegratedSecurity = true;*/

            string sCmd =
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES " +
                "WHERE TABLE_TYPE = 'BASE TABLE' " +
                "AND TABLE_NAME = @nombreTabla";

            try
            {
                using (SqlConnection con =
                    new SqlConnection("Data Source=CTORRES;Initial Catalog=Metrados;User ID=sa"))
                {
                    con.Open();
                    SqlCommand cmd =
                        new SqlCommand(sCmd, con);
                    cmd.Parameters.AddWithValue("@nombreTabla",
                                                nombreTabla);
                    int n = (int)cmd.ExecuteScalar();
                    con.Close();
                    return n > 0;
                }
            }
            catch
            {
                return false;
            }
        }


        private void buttonX2_Click(object sender, EventArgs e)
        {
            ConexionBD baseC = new ConexionBD();

            if (!existeTabla("LPlano")) {
                baseC.crearTablaPlano();
            }
            if (!existeTabla("LMedicion"))
            {
                baseC.crearTablaMedicion();
            }


        }

        private void timer12_Tick(object sender, EventArgs e)
        {
            timer12.Enabled = false;
            string datoError = "0";
            //string datoError = (string)webControl1.WebView.EvalScript("RetornaErrorCode();");
            try
            {
                datoError = (string)webControl1.WebView.EvalScript(@"CodError");
            }
            catch { }
            
            //System.Windows.Forms.MessageBox.Show(datoError.ToString(), "");
            if (datoError.Trim() != "0" && EntroPrespuesto==0) {
                //activar proceso de espera de carga
                ModeloXCargar = ModeloCargado;
                ModeloCargado = "urn:adsk.wipprod:fs.file:vf.kJrrf5s8T1KntOTN3n4zrw?version=1";
                //VERIFICAR SI EL MODELO ESTA EN PROCESO DE CARGA
                string urn = ViewerURN(ModeloCargado, "");
                //bRowser.Load(urn);
                webControl1.WebView.LoadUrl(urn);
                EO.WebBrowser.Runtime.AddLicense(
                "3a5rp7PD27FrmaQHEPGs4PP/6KFrqKax2r1GgaSxy5916u34GeCt7Pb26bSG" +
                "prT6AO5p3Nfh5LRw4rrqHut659XO6Lto6u34GeCt7Pb26YxDs7P9FOKe5ff2" +
                "6YxDdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW1yQKzbam2xvGvcau0weKv" +
                "fLOz/RTinuX39vTjd4SOscufWbPw+g7kp+rp9unMmuX59hefi9jx+h3ks7Oz" +
                "/RTinuX39hC9RoGkscufddjw/Rr2d4SOscufWZekscu7mtvosR/4qdzBs/DO" +
                "Z7rsAxrsnpmkBxDxrODz/+iha6iywc2faLWRm8ufWZfAwAzrpeb7z7iJWZek" +
                "sefuq9vpA/Ttn+ak9QzznrSmyNqxaaa2wd2wW5f3Bg3EseftAxDyeuvBs+I=");

                textBoxX2.Text = urn;
                //timer12.Enabled = true;
                EnEspera = 1;
                timerEspera.Enabled = true;
            }

        }

        private void timerEspera_Tick(object sender, EventArgs e)
        {
            timerEspera.Enabled = false;
            //string datoError = (string)webControl1.WebView.EvalScript(@"CodError");
                //activar proceso de espera de carga
            ModeloCargado = ModeloXCargar;
            //ModeloCargado = "urn:adsk.wipprod:fs.file:vf.kJrrf5s8T1KntOTN3n4zrw?version=1";
                string urn = ViewerURN(ModeloCargado, "");
                //bRowser.Load(urn);
                webControl1.WebView.LoadUrl(urn);
            EO.WebBrowser.Runtime.AddLicense(
            "3a5rp7PD27FrmaQHEPGs4PP/6KFrqKax2r1GgaSxy5916u34GeCt7Pb26bSG" +
            "prT6AO5p3Nfh5LRw4rrqHut659XO6Lto6u34GeCt7Pb26YxDs7P9FOKe5ff2" +
            "6YxDdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW1yQKzbam2xvGvcau0weKv" +
            "fLOz/RTinuX39vTjd4SOscufWbPw+g7kp+rp9unMmuX59hefi9jx+h3ks7Oz" +
            "/RTinuX39hC9RoGkscufddjw/Rr2d4SOscufWZekscu7mtvosR/4qdzBs/DO" +
            "Z7rsAxrsnpmkBxDxrODz/+iha6iywc2faLWRm8ufWZfAwAzrpeb7z7iJWZek" +
            "sefuq9vpA/Ttn+ak9QzznrSmyNqxaaa2wd2wW5f3Bg3EseftAxDyeuvBs+I=");

            textBoxX2.Text = urn;
                //timer12.Enabled = true;
                EnEspera = 0;
            ModeloXCargar = "";
            timer12.Enabled = true;
        }

        private void SGPresupC_EndEdit(object sender, GridEditEventArgs e)
        {
            GridPanel Panel = (GridPanel)SGPresupC.PrimaryGrid;
            GridRow FilaActual = (GridRow)Panel.ActiveRow;
            if (FilaActual is null) return;
            GridPanel Panel1 = (GridPanel)SGMediciones.PrimaryGrid;
            if (Panel1.Rows[0] is null) return;
            GridRow PrimeraFila = (GridRow)Panel1.Rows[0];
            if (Panel1.Rows.Count > 3) return;

            if (e.GridCell.ColumnIndex == 4) {
                double parcial=0.0;
                //valido la cantidad ingresada
                if (e.GridCell.Value.ToString() == "") parcial = 0.00; 
                try { parcial= Convert.ToDouble(e.GridCell.Value.ToString()); }catch{ parcial = 0.00; }
                
                
                

                if (PrimeraFila.Cells[2].Value is null)                //que no tenga
                {
                    string CodigoUni = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Metrados Personalizados", Cantidad = "", Longitud = "", Ancho = "", Alto = "", Total = parcial.ToString("N2"), Detalle = "Titulo de Nivel 1", Vinculo = "Personalizado", UniqueId = "", PhantomParentId = "", Nivel = 1, Tipo = "Titulo" });
                    var basdat = new ConexionBD();
                    basdat.Conexion();
                    //basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);
                    basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni, "Metrados Personalizados", "", "", "", "", parcial.ToString("N2"), "Titulo de Nivel 1", "Personalizado", "", "", 1, "Titulo", EmailUsuario);
                    //GuardarMedicion(CodigoUni, "Metrados Personalizados", "", "", "", "", "1.00", "Titulo de Nivel 1", "Personalizado", "", "", 1, "Titulo");
                    GridRow filaInsertar = new GridRow(CodigoUni, "", "Metrados Personalizados", "", "", "", "", parcial.ToString("N2"), "Titulo de Nivel 1", "Personalizado", "Titulo", "", "1");
                    var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                    filaInsertar.CellStyles.Default.Background = Background;
                    Panel1.Rows.Insert(0, filaInsertar);
                    PrimeraFila = filaInsertar;
                    string CodigoUni2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    filaInsertar = new GridRow(CodigoUni2, "", "Nuevo Item", parcial.ToString("N2"), "", "", "", parcial.ToString("N2"), "Metrado de Nivel 2", "Personalizado", "Medicion", "", "2");
                    ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni2, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Nuevo Item", Cantidad = parcial.ToString("N2"), Longitud = "", Ancho = "", Alto = "", Total = parcial.ToString("N2"), Detalle = "Metrado de Nivel 2", Vinculo = "Personalizado", UniqueId = "", PhantomParentId = CodigoUni, Nivel = 2, Tipo = "Medicion" });
                    PrimeraFila.Rows.Add(filaInsertar);
                    PrimeraFila.Expanded = true;
                    basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni2, "Nuevo Item", parcial.ToString("N2"), "", "", "", parcial.ToString("N2"), "Metrado de Nivel 2", "Personalizado", "", CodigoUni, 2, "Medicion", EmailUsuario);
                    basdat.Conexion();
                    e.GridCell.Value = parcial.ToString("N2");
                    recalcular_pres_desdeFila(FilaActual);
                }
                else {
                    if (PrimeraFila.Cells[2].Value.ToString() == "Metrados Personalizados")
                    {
                        if (PrimeraFila.Rows.Count == 0) return;
                        GridRow SegundaFila = (GridRow)PrimeraFila.Rows[0]; //fila en que voy a editar

                        var basdat = new ConexionBD();
                        basdat.Conexion();
                        PrimeraFila.Cells[7].Value = parcial.ToString("N2");
                        SegundaFila.Cells[3].Value = parcial.ToString("N2");
                        SegundaFila.Cells[7].Value = parcial.ToString("N2");

                        basdat.ActualizarMedicion1(SegundaFila.Cells[0].Value.ToString(), SegundaFila.Cells[2].Value.ToString(), SegundaFila.Cells[3].Value.ToString(), SegundaFila.Cells[4].Value.ToString(), SegundaFila.Cells[5].Value.ToString(), SegundaFila.Cells[6].Value.ToString(), SegundaFila.Cells[7].Value.ToString(), SegundaFila.Cells[8].Value.ToString());
                        basdat.ActualizarMedicion1(PrimeraFila.Cells[0].Value.ToString(), PrimeraFila.Cells[2].Value.ToString(), PrimeraFila.Cells[3].Value.ToString(), PrimeraFila.Cells[4].Value.ToString(), PrimeraFila.Cells[5].Value.ToString(), PrimeraFila.Cells[6].Value.ToString(), PrimeraFila.Cells[7].Value.ToString(), PrimeraFila.Cells[8].Value.ToString());

                        basdat.Conexion();
                        e.GridCell.Value = parcial.ToString("N2");
                        recalcular_pres_desdeFila(FilaActual);
                        GuardarMetrado(parcial.ToString());
                        //pendiente actualizar sumas
                    }
                }
                //que ya tenga metrados personalizasos



            }



            //if ()
        }

        private void SGMediciones_EndEdit(object sender, GridEditEventArgs e)
        {
            

            GridPanel Panel1 = (GridPanel)SGPresupC.PrimaryGrid;
            GridRow filaauxPadre = (GridRow)Panel1.ActiveRow;
            GridPanel Panel = (GridPanel)SGMediciones.PrimaryGrid;
            GridRow PrimeraFila = (GridRow)Panel.Rows[0];
            GridRow FilaActual = (GridRow)Panel.ActiveRow;

            GridRow UltimaFila = (GridRow)Panel.Rows[Panel.Rows.Count-1];
            if (FilaActual == UltimaFila) {
                e.GridCell.Value = "";
                return;
            }

            var basdat = new ConexionBD();
            basdat.Conexion();

            if (FilaActual is null) return;

            if (e.GridCell.ColumnIndex >= 3 && e.GridCell.ColumnIndex <= 7) {

                double cantidad = 1.0, longitud = 1.0, Ancho =1.0, Alto = 1.0, Total = 0.0;

                if (FilaActual.Cells[3].Value.ToString() == "") cantidad = 1.0;
                else
                {
                    try { cantidad = Convert.ToDouble(FilaActual.Cells[3].Value.ToString());
                        FilaActual.Cells[3].Value = cantidad.ToString("N2");
                    } catch { cantidad = 1.0; FilaActual.Cells[3].Value = ""; }
                }
                if (FilaActual.Cells[4].Value.ToString() == "") longitud = 1.0;
                else
                {
                    try { longitud = Convert.ToDouble(FilaActual.Cells[4].Value.ToString());
                        FilaActual.Cells[4].Value = longitud.ToString("N2");
                    } catch { longitud = 1.0; FilaActual.Cells[4].Value = ""; }
                }
                if (FilaActual.Cells[5].Value.ToString() == "") Ancho = 1.0;
                else
                {
                    try { Ancho = Convert.ToDouble(FilaActual.Cells[5].Value.ToString());
                        FilaActual.Cells[5].Value = Ancho.ToString("N2");
                    } catch { Ancho = 1.0; FilaActual.Cells[5].Value = ""; }
                }
                if (FilaActual.Cells[6].Value.ToString() == "") Alto = 1.0;
                else
                {
                    try { Alto = Convert.ToDouble(FilaActual.Cells[6].Value.ToString());
                        FilaActual.Cells[6].Value = Alto.ToString("N2");
                    } catch { Alto = 1.0; FilaActual.Cells[6].Value = ""; }
                }
                Total = cantidad * longitud * Ancho * Alto;
                FilaActual.Cells[7].Value = Total.ToString("N2");

                GridRow fpadre= (GridRow)FilaActual;
                double sumatoria = 0;

                //while (fpadre.GetType().Name != "GridPanel")
                do
                {
                    fpadre = (GridRow)fpadre.Parent;
                    double sumatoria1 = 0;
                    if (fpadre.Rows.Count != 0)
                    {
                        foreach (GridRow fac in fpadre.Rows)
                        {
                            sumatoria1 += Convert.ToDouble(fac.Cells[7].Value.ToString());
                        }
                    }
                    fpadre.Cells[7].Value = sumatoria1.ToString("N2");
                    basdat.ActualizarMedicion1(fpadre.Cells[0].Value.ToString(), fpadre.Cells[2].Value.ToString(), fpadre.Cells[3].Value.ToString(), fpadre.Cells[4].Value.ToString(), fpadre.Cells[5].Value.ToString(), fpadre.Cells[6].Value.ToString(), fpadre.Cells[7].Value.ToString(), fpadre.Cells[8].Value.ToString());
                    sumatoria += sumatoria1;
                } while (fpadre.Parent.GetType().Name != "GridPanel");

                double CalculoTotal = 0.0;
                foreach (GridRow Itfila in SGMediciones.PrimaryGrid.Rows)
                {
                    if (!(Itfila.Cells[7].Value is null))
                    CalculoTotal+=Convert.ToDouble(Itfila.Cells[7].Value.ToString());
                }

                filaauxPadre.Cells[4].Value = CalculoTotal.ToString("N2");
                recalcular_pres_desdeFila(filaauxPadre);
                SuperTabItem5.Text = "METRADO " + CalculoTotal.ToString("N2");
                GuardarMetrado(CalculoTotal.ToString());

            }

            if (!(FilaActual.Cells[0].Value is null))
                basdat.ActualizarMedicion1(FilaActual.Cells[0].Value.ToString(), FilaActual.Cells[2].Value.ToString(), FilaActual.Cells[3].Value.ToString(), FilaActual.Cells[4].Value.ToString(), FilaActual.Cells[5].Value.ToString(), FilaActual.Cells[6].Value.ToString(), FilaActual.Cells[7].Value.ToString(), FilaActual.Cells[8].Value.ToString());

            basdat.Conexion();



        }

        private void Btt2_Click(object sender, EventArgs e)
        {

            
        }

        private void ButtonItem67_Click(object sender, EventArgs e)
        {
            GridPanel Panel = (GridPanel)SGMediciones.PrimaryGrid;
            GridRow FilaActual = (GridRow)Panel.ActiveRow;
            GridPanel Panel1 = (GridPanel)SGPresupC.PrimaryGrid;
            GridRow filaauxPadre = (GridRow)Panel1.ActiveRow;
            GridRow PrimeraFila = (GridRow)Panel.Rows[0];


            var basdat = new ConexionBD();
            basdat.Conexion();
            if (FilaActual.Rows.Count == 0)
            {
                if (PrimeraFila==(GridRow)FilaActual.Parent && PrimeraFila.Rows.Count == 1)
                {
                    System.Windows.Forms.MessageBox.Show("No es posible eliminar el registro, para hacerlo modifique manualmente la cantidad a 0", "Advertencia");
                    return;
                }

                basdat.EliminarItemMedicion(FilaActual.Cells[0].Value.ToString());
                FilaActual.Cells[7].Value = "0.00";
                FilaActual.Visible = false;

                GridRow fpadre = (GridRow)FilaActual;
                double sumatoria = 0;
                if (fpadre.Parent.GetType().Name != "GridPanel")
                    do
                    {
                        fpadre = (GridRow)fpadre.Parent;
                        double sumatoria1 = 0;
                        if (fpadre.Rows.Count != 0)
                            foreach (GridRow fac in fpadre.Rows)
                                sumatoria1 += Convert.ToDouble(fac.Cells[7].Value.ToString());
                        fpadre.Cells[7].Value = sumatoria1.ToString("N2");
                        basdat.ActualizarMedicion1(fpadre.Cells[0].Value.ToString(), fpadre.Cells[2].Value.ToString(), fpadre.Cells[3].Value.ToString(), fpadre.Cells[4].Value.ToString(), fpadre.Cells[5].Value.ToString(), fpadre.Cells[6].Value.ToString(), fpadre.Cells[7].Value.ToString(), fpadre.Cells[8].Value.ToString());
                        sumatoria += sumatoria1;
                    } while (fpadre.Parent.GetType().Name != "GridPanel");

                double CalculoTotal = 0.0;
                foreach (GridRow Itfila in SGMediciones.PrimaryGrid.Rows)
                {
                    if (!(Itfila.Cells[7].Value is null))
                        CalculoTotal += Convert.ToDouble(Itfila.Cells[7].Value.ToString());
                }

                filaauxPadre.Cells[4].Value = CalculoTotal.ToString("N2");
                recalcular_pres_desdeFila(filaauxPadre);
                SuperTabItem5.Text = "METRADO " + CalculoTotal.ToString("N2");
                GuardarMetrado(CalculoTotal.ToString());
            }
            else {//si tiene hijos
                //si es la primera fila y solo existen dos items no puede eliminarse
                /*if (FilaActual == PrimeraFila && Panel.Rows.Count == 2) {
                    System.Windows.Forms.MessageBox.Show("No es posible eliminar el registro, para hacerlo modifique manualmente la cantidad a 0", "Advertencia");
                    return;
                }*/

                if (Panel.Rows.Count == 1)
                {
                    System.Windows.Forms.MessageBox.Show("No es posible eliminar el registro, para hacerlo modifique manualmente la cantidad a 0", "Advertencia");
                    return;
                }


                if (System.Windows.Forms.MessageBox.Show("Desea eliminar, " + FilaActual.Cells[2].Value.ToString() + " y todo su contenido?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes) {

                    foreach (GridRow filaux in FilaActual.Rows) {
                        if (filaux.Rows.Count != 0) {
                            EliminarHijos(filaux, basdat);
                            basdat.EliminarHijosMedicion(filaux.Cells[0].Value.ToString());
                            //ListaItemsMedicion.RemoveAll((List<LMedicion>) ListaItemsMedicion.FindAll(x => x.PhantomParentId == filaux.Cells[0].Value.ToString()));
                            ListaItemsMedicion.RemoveAll(ListaItemsMedicion => ListaItemsMedicion.PhantomParentId == filaux.Cells[0].Value.ToString());
                            filaux.Cells[7].Value = "0.00";
                            filaux.Visible = false;
                        }
                    }

                    //EliminarHijos(FilaActual, basdat);
                    basdat.EliminarHijosMedicion(FilaActual.Cells[0].Value.ToString());
                    ListaItemsMedicion.RemoveAll(ListaItemsMedicion => ListaItemsMedicion.PhantomParentId == FilaActual.Cells[0].Value.ToString());
                    FilaActual.Cells[7].Value = "0.00";
                    FilaActual.Visible = false;

                    basdat.EliminarItemMedicion(FilaActual.Cells[0].Value.ToString());
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


                    GridRow fpadre = (GridRow)FilaActual;
                    double sumatoria = 0;
                    if (fpadre.Parent.GetType().Name != "GridPanel")
                        do
                        {
                            fpadre = (GridRow)fpadre.Parent;
                            double sumatoria1 = 0;
                            if (fpadre.Rows.Count != 0)
                                foreach (GridRow fac in fpadre.Rows)
                                    sumatoria1 += Convert.ToDouble(fac.Cells[7].Value.ToString());
                            fpadre.Cells[7].Value = sumatoria1.ToString("N2");
                            basdat.ActualizarMedicion1(fpadre.Cells[0].Value.ToString(), fpadre.Cells[2].Value.ToString(), fpadre.Cells[3].Value.ToString(), fpadre.Cells[4].Value.ToString(), fpadre.Cells[5].Value.ToString(), fpadre.Cells[6].Value.ToString(), fpadre.Cells[7].Value.ToString(), fpadre.Cells[8].Value.ToString());
                            sumatoria += sumatoria1;
                        } while (fpadre.Parent.GetType().Name != "GridPanel");
                    
                    double CalculoTotal = 0.0;
                    foreach (GridRow Itfila in SGMediciones.PrimaryGrid.Rows)
                    {
                        if (!(Itfila.Cells[7].Value is null))
                            CalculoTotal += Convert.ToDouble(Itfila.Cells[7].Value.ToString());
                    }

                    filaauxPadre.Cells[4].Value = CalculoTotal.ToString("N2");
                    recalcular_pres_desdeFila(filaauxPadre);
                    SuperTabItem5.Text = "METRADO " + CalculoTotal.ToString("N2");
                    GuardarMetrado(CalculoTotal.ToString());




                }
            }



            basdat.Conexion();
        }


        private void EliminarHijos(GridRow filaborrar, ConexionBD basdat) {
            foreach (GridRow filaux in filaborrar.Rows)
            {
                if (filaux.Rows.Count != 0)
                {
                    EliminarHijos(filaux, basdat);
                    basdat.EliminarHijosMedicion(filaux.Cells[0].Value.ToString());
                    ListaItemsMedicion.RemoveAll(ListaItemsMedicion => ListaItemsMedicion.PhantomParentId == filaux.Cells[0].Value.ToString());
                    filaux.Cells[7].Value = "0.00";
                    filaux.Visible = false;
                }
            }

        }



        private void ButtonItem47_Click(object sender, EventArgs e)
        {
            GridPanel Panel = (GridPanel)SGMediciones.PrimaryGrid;
            GridRow FilaActual = (GridRow)Panel.ActiveRow;
            GridRow PrimeraFila = (GridRow)Panel.Rows[0];
            var basdat = new ConexionBD();
            GridPanel Panel1 = (GridPanel)SGPresupC.PrimaryGrid;
            GridRow filaauxPadre = (GridRow)Panel1.ActiveRow;

            if (Item_actual == "") return;
            if (PrimeraFila is null) return;

            if (PrimeraFila.Cells[2].Value is null) return;
            //Si no existen Items personalizados y estoy en cualquier parte
            if (PrimeraFila.Cells[2].Value.ToString() != "Metrados Personalizados") {}

            if (FilaActual.Cells[2].Value.ToString() == "Metrados Personalizados")
            {


                string CodigoUni2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                GridRow filaInsertar = new GridRow(CodigoUni2, "", "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel 2", "Personalizado", "Medicion", PrimeraFila.Cells[0].Value.ToString(), "2");
                ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni2, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Nuevo Item", Cantidad = "1.00", Longitud = "", Ancho = "", Alto = "", Total = "1.00", Detalle = "Metrado de Nivel 2", Vinculo = "Personalizado", UniqueId = "", PhantomParentId = PrimeraFila.Cells[0].Value.ToString(), Nivel = 2, Tipo = "Medicion" });
                PrimeraFila.Rows.Add(filaInsertar);
                PrimeraFila.Expanded = true;
                Panel.SetActiveRow(filaInsertar);
                filaInsertar.EnsureVisible();
                filaInsertar.IsSelected = true;

                var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                filaInsertar.CellStyles.Default.Background = Background;
                basdat.Conexion();
                //basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);
                basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni2, "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel 2", "Personalizado", "", PrimeraFila.Cells[0].Value.ToString(), 2, "Medicion", EmailUsuario);
                basdat.Conexion();



                GridRow fpadre = (GridRow)filaInsertar;
                double sumatoria = 0;
                if (fpadre.Parent.GetType().Name != "GridPanel")
                    do
                    {
                        fpadre = (GridRow)fpadre.Parent;
                        double sumatoria1 = 0;
                        if (fpadre.Rows.Count != 0)
                            foreach (GridRow fac in fpadre.Rows)
                                sumatoria1 += Convert.ToDouble(fac.Cells[7].Value.ToString());
                        fpadre.Cells[7].Value = sumatoria1.ToString("N2");
                        basdat.ActualizarMedicion1(fpadre.Cells[0].Value.ToString(), fpadre.Cells[2].Value.ToString(), fpadre.Cells[3].Value.ToString(), fpadre.Cells[4].Value.ToString(), fpadre.Cells[5].Value.ToString(), fpadre.Cells[6].Value.ToString(), fpadre.Cells[7].Value.ToString(), fpadre.Cells[8].Value.ToString());
                        sumatoria += sumatoria1;
                    } while (fpadre.Parent.GetType().Name != "GridPanel");

                double CalculoTotal = 0.0;
                foreach (GridRow Itfila in SGMediciones.PrimaryGrid.Rows)
                {
                    if (!(Itfila.Cells[7].Value is null))
                        CalculoTotal += Convert.ToDouble(Itfila.Cells[7].Value.ToString());
                }

                filaauxPadre.Cells[4].Value = CalculoTotal.ToString("N2");
                recalcular_pres_desdeFila(filaauxPadre);
                SuperTabItem5.Text = "METRADO " + CalculoTotal.ToString("N2");
                GuardarMetrado(CalculoTotal.ToString());


            }
            else {
                //     QUE YA TENGA HIJOS
                if (FilaActual.Rows.Count > 0)
                {

                    //GridRow filaPapa = (GridRow)FilaActual.Parent;
                    int nivelToca = Convert.ToInt32(FilaActual.Cells[12].Value.ToString()) + 1;

                    string CodigoUni2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    GridRow filaInsertar = new GridRow(CodigoUni2, "", "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel " + nivelToca.ToString(), "Personalizado", "Medicion", FilaActual.Cells[0].Value.ToString(), nivelToca.ToString());
                    ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni2, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Nuevo Item", Cantidad = "1.00", Longitud = "", Ancho = "", Alto = "", Total = "1.00", Detalle = "Metrado de Nivel " + nivelToca.ToString() , Vinculo = "Personalizado", UniqueId = "", PhantomParentId = FilaActual.Cells[0].Value.ToString(), Nivel = nivelToca, Tipo = "Medicion" });
                    FilaActual.Rows.Add(filaInsertar);
                    FilaActual.Expanded = true;
                    Panel.SetActiveRow(filaInsertar);
                    FilaActual.EnsureVisible();
                    FilaActual.IsSelected = true;

                    var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                    filaInsertar.CellStyles.Default.Background = Background;
                    basdat.Conexion();
                    //basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);
                    basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni2, "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel " + nivelToca.ToString(), "Personalizado", "", FilaActual.Cells[0].Value.ToString(), nivelToca, "Medicion", EmailUsuario);
                    basdat.Conexion();



                    GridRow fpadre = (GridRow)filaInsertar;
                    double sumatoria = 0;
                    if (fpadre.Parent.GetType().Name != "GridPanel")
                        do
                        {
                            fpadre = (GridRow)fpadre.Parent;
                            double sumatoria1 = 0;
                            if (fpadre.Rows.Count != 0)
                                foreach (GridRow fac in fpadre.Rows)
                                    sumatoria1 += Convert.ToDouble(fac.Cells[7].Value.ToString());
                            fpadre.Cells[7].Value = sumatoria1.ToString("N2");
                            basdat.ActualizarMedicion1(fpadre.Cells[0].Value.ToString(), fpadre.Cells[2].Value.ToString(), fpadre.Cells[3].Value.ToString(), fpadre.Cells[4].Value.ToString(), fpadre.Cells[5].Value.ToString(), fpadre.Cells[6].Value.ToString(), fpadre.Cells[7].Value.ToString(), fpadre.Cells[8].Value.ToString());
                            sumatoria += sumatoria1;
                        } while (fpadre.Parent.GetType().Name != "GridPanel");

                    double CalculoTotal = 0.0;
                    foreach (GridRow Itfila in SGMediciones.PrimaryGrid.Rows)
                    {
                        if (!(Itfila.Cells[7].Value is null))
                            CalculoTotal += Convert.ToDouble(Itfila.Cells[7].Value.ToString());
                    }

                    filaauxPadre.Cells[4].Value = CalculoTotal.ToString("N2");
                    recalcular_pres_desdeFila(filaauxPadre);
                    SuperTabItem5.Text = "METRADO " + CalculoTotal.ToString("N2");
                    GuardarMetrado(CalculoTotal.ToString());




                }
                else
                { // QUE NO TENGA HIJOS

                    //1 LE HACEMOS PAPÁ

                    //MODIFICAMOS COLOR - MODIFICAMOS CANTIDAD-LONG-ANCHO-ALTO A VACIO - TOTAL QUEDA CON 1.00 TIPO Titulo Detalle modificamos a Titulo de Nivel X

                    FilaActual.Cells[3].Value = "";
                    FilaActual.Cells[4].Value = "";
                    FilaActual.Cells[5].Value = "";
                    FilaActual.Cells[6].Value = "";
                    FilaActual.Cells[7].Value = "1.00";
                    FilaActual.Cells[8].Value = "Titulo de Nivel " + FilaActual.Cells[12].Value.ToString();
                    FilaActual.Cells[10].Value = "Titulo";

                    //modificamos en la lista
                    ListaItemsMedicion.Remove(ListaItemsMedicion.Find(x => x.CodMedicion == FilaActual.Cells[0].Value.ToString()));
                    ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = FilaActual.Cells[0].Value.ToString(), CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = FilaActual.Cells[2].Value.ToString(), Cantidad = "", Longitud = "", Ancho = "", Alto = "", Total = "1.00", Detalle = "Titulo de Nivel " + FilaActual.Cells[12].Value.ToString(), Vinculo = "Personalizado", UniqueId = "", PhantomParentId = FilaActual.Cells[10].Value.ToString(), Nivel = Convert.ToInt32(FilaActual.Cells[12].Value.ToString()), Tipo = "Titulo" });


                    //modificamos en base de datos
                    basdat.Conexion();
                    basdat.ActualizarMedicion2(FilaActual.Cells[0].Value.ToString(), FilaActual.Cells[2].Value.ToString(), "", "", "", "", "1.00", "Titulo de Nivel " + FilaActual.Cells[12].Value.ToString(), "Titulo");
                    basdat.Conexion();

                    

                    FilaActual.ReadOnly = false;
                        for (int x = 3; x < 8; x++)
                          FilaActual.Cells[x].ReadOnly = true;
                    FilaActual.Cells[2].ReadOnly = false;



                    var Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(202, 223, 218), 90);
                    FilaActual.CellStyles.Default.Background = Background;
                    

                    //GridRow filaPapa = (GridRow)FilaActual.Parent;
                    int nivelToca = Convert.ToInt32(FilaActual.Cells[12].Value.ToString()) + 1;

                    string CodigoUni2 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    GridRow filaInsertar = new GridRow(CodigoUni2, "", "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel " + nivelToca.ToString(), "Personalizado", "Medicion", FilaActual.Cells[0].Value.ToString(), nivelToca.ToString());
                    ListaItemsMedicion.Insert(0, new LMedicion { CodMedicion = CodigoUni2, CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, Descripcion = "Nuevo Item", Cantidad = "1.00", Longitud = "", Ancho = "", Alto = "", Total = "1.00", Detalle = "Metrado de Nivel " + nivelToca.ToString(), Vinculo = "Personalizado", UniqueId = "", PhantomParentId = FilaActual.Cells[0].Value.ToString(), Nivel = nivelToca, Tipo = "Medicion" });
                    FilaActual.Rows.Add(filaInsertar);
                    FilaActual.Expanded = true;
                    Panel.SetActiveRow(filaInsertar);
                    FilaActual.EnsureVisible();
                    FilaActual.IsSelected = true;

                    Background = new DevComponents.DotNetBar.SuperGrid.Style.Background(System.Drawing.Color.FromArgb(224, 224, 224), System.Drawing.Color.FromArgb(255, 255, 255), 90);
                    filaInsertar.CellStyles.Default.Background = Background;
                    basdat.Conexion();
                    //basdat.DeleteMedicionesVinculo(Presupuesto_actual, SubPresupuesto_actual, Item_actual, codModelo);
                    basdat.GuardarMedicion1(Presupuesto_actual, SubPresupuesto_actual, Item_actual, CodigoUni2, "Nuevo Item", "1.00", "", "", "", "1.00", "Metrado de Nivel " + nivelToca.ToString(), "Personalizado", "", FilaActual.Cells[0].Value.ToString(), nivelToca, "Medicion", EmailUsuario);
                    basdat.Conexion();



                    GridRow fpadre = (GridRow)filaInsertar;
                    double sumatoria = 0;
                    if (fpadre.Parent.GetType().Name != "GridPanel")
                        do
                        {
                            fpadre = (GridRow)fpadre.Parent;
                            double sumatoria1 = 0;
                            if (fpadre.Rows.Count != 0)
                                foreach (GridRow fac in fpadre.Rows)
                                    sumatoria1 += Convert.ToDouble(fac.Cells[7].Value.ToString());
                            fpadre.Cells[7].Value = sumatoria1.ToString("N2");
                            basdat.ActualizarMedicion1(fpadre.Cells[0].Value.ToString(), fpadre.Cells[2].Value.ToString(), fpadre.Cells[3].Value.ToString(), fpadre.Cells[4].Value.ToString(), fpadre.Cells[5].Value.ToString(), fpadre.Cells[6].Value.ToString(), fpadre.Cells[7].Value.ToString(), fpadre.Cells[8].Value.ToString());
                            sumatoria += sumatoria1;
                        } while (fpadre.Parent.GetType().Name != "GridPanel");

                    double CalculoTotal = 0.0;
                    foreach (GridRow Itfila in SGMediciones.PrimaryGrid.Rows)
                    {
                        if (!(Itfila.Cells[7].Value is null))
                            CalculoTotal += Convert.ToDouble(Itfila.Cells[7].Value.ToString());
                    }

                    filaauxPadre.Cells[4].Value = CalculoTotal.ToString("N2");
                    recalcular_pres_desdeFila(filaauxPadre);
                    SuperTabItem5.Text = "METRADO " + CalculoTotal.ToString("N2");
                    GuardarMetrado(CalculoTotal.ToString());


                }


            }


        }

        private void buttonItem25_Click(object sender, EventArgs e)
        {
            GridPanel Panel = (GridPanel)SGMediciones.PrimaryGrid;
            GridRow FilaActual = (GridRow)Panel.ActiveRow;
            GridRow PrimeraFila = (GridRow)Panel.Rows[0];
            GridPanel Panel1 = (GridPanel)SGPresupC.PrimaryGrid;
            GridRow filaauxPadre = (GridRow)Panel1.ActiveRow;

            var basdat = new ConexionBD();
            basdat.Conexion();
            if (System.Windows.Forms.MessageBox.Show("Esta seguro de Eliminar todo el metrado?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                basdat.EliminarTodoMedicion(Presupuesto_actual, SubPresupuesto_actual, Item_actual);
                ListaItemsMedicion.RemoveAll(ListaItemsMedicion => (ListaItemsMedicion.CodPresupuesto == Presupuesto_actual && ListaItemsMedicion.CodSubpresupuesto == SubPresupuesto_actual && ListaItemsMedicion.Item == Item_actual));

                filaauxPadre.Cells[4].Value = "0.00";
                recalcular_pres_desdeFila(filaauxPadre);
                SuperTabItem5.Text = "METRADO " + "0.00";
                GuardarMetrado("0");

                do
                {
                    SGMediciones.PrimaryGrid.DeleteAll();
                    SGMediciones.PrimaryGrid.PurgeDeletedRows();
                }
                while (SGMediciones.PrimaryGrid.Rows.Count != 0);




            }
            basdat.Conexion();
            
        }


        private void recalcular_pres_desdeFila(GridRow fila) {

            double tot = 0;
            if (fila.Cells[5].Value.ToString() == "")
                tot = 0;
            else
                tot = Convert.ToDouble(fila.Cells[4].Value.ToString()) * Convert.ToDouble(fila.Cells[5].Value.ToString());
            fila.Cells[6].Value = tot.ToString("N2");

            GridRow fpadre = (GridRow)fila;
            double sumatoria = 0;
            if (fpadre.Parent.GetType().Name != "GridPanel")
                do
                {
                    fpadre = (GridRow)fpadre.Parent;
                    double sumatoria1 = 0;
                    if (fpadre.Rows.Count != 0)
                        foreach (GridRow fac in fpadre.Rows)
                            sumatoria1 += Convert.ToDouble(fac.Cells[6].Value.ToString());
                    fpadre.Cells[6].Value = sumatoria1.ToString("N2");
                    //basdat.ActualizarMedicion1(fpadre.Cells[0].Value.ToString(), fpadre.Cells[2].Value.ToString(), fpadre.Cells[3].Value.ToString(), fpadre.Cells[4].Value.ToString(), fpadre.Cells[5].Value.ToString(), fpadre.Cells[6].Value.ToString(), fpadre.Cells[7].Value.ToString(), fpadre.Cells[8].Value.ToString());
                    sumatoria += sumatoria1;
                } while (fpadre.Parent.GetType().Name != "GridPanel");

            /*double CalculoTotal = 0.0;
            foreach (GridRow Itfila in SGPresupC.PrimaryGrid.Rows)
            {
                if (!(Itfila.Cells[7].Value is null))
                    CalculoTotal += Convert.ToDouble(Itfila.Cells[7].Value.ToString());
            }*/

            //fila.Cells[4].Value = CalculoTotal.ToString("N2");
            //SuperTabItem5.Text = "METRADO " + CalculoTotal.ToString("N2");
            //GuardarMetrado(CalculoTotal.ToString());


        }





        #region Carga_en_Tablas

        void cargar_elementos_en_tablas()
        {
            Autodesk.Revit.DB.Document doc = Datos.cmdData1.Application.ActiveUIDocument.Document;
            Settings documentSettings = doc.Settings;
            Autodesk.Revit.DB.Categories groups = documentSettings.Categories;
            //treeView1.BeginUpdate();
            //treeView1.Sorted = true;
            /*foreach (var eleme in groups)
            {
                var elem = (Autodesk.Revit.DB.Category)eleme;
                TreeNode Nodop = new TreeNode(elem.Name.ToString());
                treeView1.Nodes.Add(Nodop);
            }
            treeView1.EndUpdate();*/

            CommonInit1(Datos.objs.Cast<object>().Select(SnoopableObjectWrapper.Create));
            CargarFamilias1();
        }


        void CommonInit1(IEnumerable<SnoopableObjectWrapper> objs)
        {
            //m_tvObjs.BeginUpdate();
            AddObjectsToTree1(objs);
           /* if (m_tvObjs.Nodes.Count == 1)
            {
                m_tvObjs.Nodes[0].Expand();
                if (m_tvObjs.Nodes[0].Nodes.Count == 0)
                    m_tvObjs.SelectedNode = m_tvObjs.Nodes[0];
                else
                    m_tvObjs.SelectedNode = m_tvObjs.Nodes[0].Nodes[0];
            }
            m_tvObjs.EndUpdate();*/
        }


        void AddObjectsToTree1(IEnumerable<SnoopableObjectWrapper> snoopableObjects)
        {
            var basdat = new ConexionBD();
            basdat.Conexion();
            definir_listas();
            progressBar1.Maximum = snoopableObjects.Count();
            progressBar1.Value=0;
            progressBar1.Refresh();
            int posi = 0;

            foreach (var snoopableObject in snoopableObjects)
            {
                progressBar1.Value = posi;
                progressBar1.Refresh();
                posi++;

                if (snoopableObject.GetUnderlyingType().Name == "FamilyInstance")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            TreeNode Nodop = new TreeNode(elem.Category.Name + " Id =" + elem.Id);
                            treeView2.Nodes.Add(Nodop);
                            FamilySymbol elemType = (FamilySymbol)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Armazón estructural":
                                    ArmazonesEstructurales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Armazón estructural", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Armazón estructural", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;
                                case "Cimentación estructural":
                                    CimentacionesEstructurales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Cimentación estructural", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Cimentación estructural", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;
                                case "Conexiones estructurales":
                                    ConexionesEstructurales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Conexiones estructurales", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Conexiones estructurales", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;
                                case "Emplazamiento":
                                    Emplazamientos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Emplazamiento", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Emplazamiento", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;
                                case "Escaleras":
                                    Escaleras.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Escaleras", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Escaleras", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;
                                case "Modelos genéricos":
                                    ModelosGenericos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Modelos genéricos", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Modelos genéricos", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;
                                case "Pilares estructurales":
                                    PilaresEstructurales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Pilares estructurales", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Pilares estructurales", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;
                                case "Aparatos sanitarios":
                                    AparatosSanitarios.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Aparatos sanitarios", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Aparatos sanitarios", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;
                            }
                        }
                }

                if (snoopableObject.GetUnderlyingType().Name == "Rebar")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            RebarBarType elemType = (RebarBarType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Armadura estructural":
                                    ArmadurasEstructuales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Armadura estructural", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Armadura estructural", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;
                            }
                        }
                }

                if (snoopableObject.GetUnderlyingType().Name == "Floor")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            FloorType elemType = (FloorType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Cimentación estructural":
                                    CimentacionesEstructurales.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Cimentación estructural", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Cimentación estructural", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;
                                case "Suelos":
                                    Suelos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Suelos", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Suelos", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "CableTray")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            CableTrayType elemType = (CableTrayType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Bandejas de cables":
                                    BandejasdeCables.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Bandejas de cables", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Bandejas de cables", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "Railing")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            RailingType elemType = (RailingType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Barandillas":
                                    Barandillas.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Barandillas", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Barandillas", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }

                if (snoopableObject.GetUnderlyingType().Name == "Duct")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            DuctType elemType = (DuctType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Conductos":
                                    Conductos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Conductos", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Conductos", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "FlexDuct")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            FlexDuctType elemType = (FlexDuctType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Conductos flexibles":
                                    ConductosFlexibles.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Conductos flexibles", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Conductos flexibles", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "FootPrintRoof")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            RoofType elemType = (RoofType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Cubiertas":
                                    Cubiertas.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Cubiertas", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Cubiertas", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "Wall")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            WallType elemType = (WallType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Muros":
                                    Muros.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Muros", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Muros", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "Ceiling")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            CeilingType elemType = (CeilingType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Techos":
                                    Techos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Techos", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Techos", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "Pipe")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            PipeType elemType = (PipeType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Tuberías":
                                    Tuberias.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Tuberías", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Tuberías", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "FlexPipe")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            FlexPipeType elemType = (FlexPipeType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Tuberías flexibles":
                                    TuberiasFlexibles.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Tuberías flexibles", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Tuberías flexibles", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }

                if (snoopableObject.GetUnderlyingType().Name == "Conduit")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            ConduitType elemType = (ConduitType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Tubos":
                                    Tubos.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Tubos", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Tubos", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "TopographySurface")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            //ConduitType elemType = (ConduitType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Topografía":
                                    Topografias.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Topografía", Familia = "Topografía", Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Topografía", "Topografía", cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }


                if (snoopableObject.GetUnderlyingType().Name == "Stairs")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            //ConduitType elemType = (ConduitType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            StairsType elemType = (StairsType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            switch (elem.Category.Name)
                            {
                                case "Escaleras":
                                    Escaleras.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Escaleras", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                    string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                    basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Escaleras", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                    ; break;

                            }
                        }
                }




                if (snoopableObject.GetUnderlyingType().Name == "Element")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null)
                        if (elem.Category != null)
                        {
                            ElementType elemType = (ElementType)Datos.cmdData1.Application.ActiveUIDocument.Document.GetElement(elem.GetTypeId());
                            if (elemType is null) { }
                            else
                                switch (elem.Category.Name)
                                {
                                    case "Rampas":
                                        Rampas.Add(new RevitElementoBase { Id = elem.Id.ToString(), Categoria = "Rampas", Familia = elemType.FamilyName.ToString(), Tipo = elem.Name.ToString(), UniqueId = elem.UniqueId.ToString() });
                                        string cadenaTipo = elem.Name.ToString().Replace("'", "''");
                                        basdat.GuardarElemento(labelItem8.Text, elem.Id.ToString(), "Rampas", elemType.FamilyName.ToString(), cadenaTipo, elem.UniqueId.ToString());
                                        ; break;

                                }
                        }
                }

                //PARAMETROS COMPARTIDOS
                if (snoopableObject.GetUnderlyingType().Name == "SharedParameterElement")
                {
                    var elem = (Element)snoopableObject.Object as Element;
                    if (elem != null) {
                        ParametrosCompartidos.Add(new RevitParametroBase { Nombre = elem.Name.ToString() });
                        basdat.GuardarParametros(labelItem8.Text, elem.Name.ToString());
                    }
                }

            }
            _NwData[0].Count = ArmazonesEstructurales.Count();
            _NwData[1].Count = ArmadurasEstructuales.Count();
            _NwData[2].Count = BandejasdeCables.Count();
            _NwData[3].Count = Barandillas.Count();
            _NwData[4].Count = CimentacionesEstructurales.Count();
            _NwData[5].Count = Conductos.Count();
            _NwData[6].Count = ConductosFlexibles.Count();
            _NwData[7].Count = ConexionesEstructurales.Count();
            _NwData[8].Count = Cubiertas.Count();
            _NwData[9].Count = Emplazamientos.Count();
            _NwData[10].Count = Escaleras.Count();
            _NwData[11].Count = ModelosGenericos.Count();
            _NwData[12].Count = AparatosSanitarios.Count();
            _NwData[13].Count = Muros.Count();
            _NwData[14].Count = PilaresEstructurales.Count();
            _NwData[15].Count = Rampas.Count();
            _NwData[16].Count = Suelos.Count();
            _NwData[17].Count = Techos.Count();
            _NwData[18].Count = Tuberias.Count();
            _NwData[19].Count = TuberiasFlexibles.Count();
            _NwData[20].Count = Tubos.Count();
            _NwData[21].Count = Topografias.Count();

            InitializeChart();
            PieChart pieChart = (PieChart)chartControl1.ChartPanel.ChartContainers[0];
            pieChart.PaletteGroup = (PaletteGroup)
            Enum.Parse(typeof(PaletteGroup), PaletteGroup.Color2.ToString());
            _Timer = new System.Windows.Forms.Timer();
            _Timer.Interval = 1500;
            _Timer.Tick += Timer_Tick;
            // Hook the PieSelectionChanged event so that we can give the user
            // feedback on what items they have selected.
            chartControl1.PieSelectionChanged += chartControl1_PieSelectionChanged;
            basdat.Conexion();
            cargar_FamiliasyTipos();
        }

        void definir_listas() {

            ArmazonesEstructurales = new List<RevitElementoBase>();
            ArmadurasEstructuales = new List<RevitElementoBase>();
            BandejasdeCables = new List<RevitElementoBase>();
            Barandillas = new List<RevitElementoBase>();
            CimentacionesEstructurales = new List<RevitElementoBase>();
            Conductos = new List<RevitElementoBase>();
            ConductosFlexibles = new List<RevitElementoBase>();
            ConexionesEstructurales = new List<RevitElementoBase>();
            Cubiertas = new List<RevitElementoBase>();
            Emplazamientos = new List<RevitElementoBase>();
            Escaleras = new List<RevitElementoBase>();
            ModelosGenericos = new List<RevitElementoBase>();
            AparatosSanitarios = new List<RevitElementoBase>();
            Muros = new List<RevitElementoBase>();
            PilaresEstructurales = new List<RevitElementoBase>();
            Rampas = new List<RevitElementoBase>();
            Suelos = new List<RevitElementoBase>();
            Techos = new List<RevitElementoBase>();
            Tuberias = new List<RevitElementoBase>();
            TuberiasFlexibles = new List<RevitElementoBase>();
            Tubos = new List<RevitElementoBase>();
            Topografias = new List<RevitElementoBase>();

            FamiliasArmazonesEstructurales = new List<RevitFamiliaBase>();
            FamiliasArmadurasEstructuales = new List<RevitFamiliaBase>();
            FamiliasBandejasdeCables = new List<RevitFamiliaBase>();
            FamiliasBarandillas = new List<RevitFamiliaBase>();
            FamiliasCimentacionesEstructurales = new List<RevitFamiliaBase>();
            FamiliasConductos = new List<RevitFamiliaBase>();
            FamiliasConductosFlexibles = new List<RevitFamiliaBase>();
            FamiliasConexionesEstructurales = new List<RevitFamiliaBase>();
            FamiliasCubiertas = new List<RevitFamiliaBase>();
            FamiliasEmplazamientos = new List<RevitFamiliaBase>();
            FamiliasEscaleras = new List<RevitFamiliaBase>();
            FamiliasModelosGenericos = new List<RevitFamiliaBase>();
            FamiliasAparatosSanitarios = new List<RevitFamiliaBase>();
            FamiliasMuros = new List<RevitFamiliaBase>();
            FamiliasPilaresEstructurales = new List<RevitFamiliaBase>();
            FamiliasRampas = new List<RevitFamiliaBase>();
            FamiliasSuelos = new List<RevitFamiliaBase>();
            FamiliasTechos = new List<RevitFamiliaBase>();
            FamiliasTuberias = new List<RevitFamiliaBase>();
            FamiliasTuberiasFlexibles = new List<RevitFamiliaBase>();
            FamiliasTubos = new List<RevitFamiliaBase>();
            FamiliasTopografias = new List<RevitFamiliaBase>();

            TiposArmazonesEstructurales = new List<RevitTipoBase>();
            TiposArmadurasEstructuales = new List<RevitTipoBase>();
            TiposBandejasdeCables = new List<RevitTipoBase>();
            TiposBarandillas = new List<RevitTipoBase>();
            TiposCimentacionesEstructurales = new List<RevitTipoBase>();
            TiposConductos = new List<RevitTipoBase>();
            TiposConductosFlexibles = new List<RevitTipoBase>();
            TiposConexionesEstructurales = new List<RevitTipoBase>();
            TiposCubiertas = new List<RevitTipoBase>();
            TiposEmplazamientos = new List<RevitTipoBase>();
            TiposEscaleras = new List<RevitTipoBase>();
            TiposModelosGenericos = new List<RevitTipoBase>();
            TiposAparatosSanitarios = new List<RevitTipoBase>();
            TiposMuros = new List<RevitTipoBase>();
            TiposPilaresEstructurales = new List<RevitTipoBase>();
            TiposRampas = new List<RevitTipoBase>();
            TiposSuelos = new List<RevitTipoBase>();
            TiposTechos = new List<RevitTipoBase>();
            TiposTuberias = new List<RevitTipoBase>();
            TiposTuberiasFlexibles = new List<RevitTipoBase>();
            TiposTubos = new List<RevitTipoBase>();
            TiposTopografias = new List<RevitTipoBase>();

            ParametrosCompartidos = new List<RevitParametroBase>();

        }

        void cargar_ElementosBD()
        {
            definir_listas();

            System.Data.DataTable datosPl = new System.Data.DataTable();
            ConexionBD bdatos = new ConexionBD();

            datosPl = bdatos.LElementos(labelItem8.Text);

            progressBar1.Maximum = datosPl.Rows.Count;
            progressBar1.Value = 0;
            progressBar1.Refresh();
            int posi = 0;

            foreach (DataRow item in datosPl.Rows)
            {

                progressBar1.Value = posi;
                progressBar1.Refresh();
                posi++;

                switch (item["Categoria"].ToString().Trim())
                {
                    case "Armazón estructural":
                        ArmazonesEstructurales.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Armadura estructural":
                        ArmadurasEstructuales.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Bandejas de cables":
                        BandejasdeCables.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Barandillas":
                        Barandillas.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Cimentación estructural":
                        CimentacionesEstructurales.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Conductos":
                        Conductos.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Conductos flexibles":
                        ConductosFlexibles.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Conexiones estructurales":
                        ConexionesEstructurales.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Cubiertas":
                        Cubiertas.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Emplazamiento":
                        Emplazamientos.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Escaleras":
                        Escaleras.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Modelos genéricos":
                        ModelosGenericos.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Aparatos sanitarios":
                        AparatosSanitarios.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Muros":
                        Muros.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Pilares estructurales":
                        PilaresEstructurales.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Rampas":
                        Rampas.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Suelos":
                        Suelos.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Techos":
                        Techos.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Tuberías":
                        Tuberias.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Tuberías flexibles":
                        TuberiasFlexibles.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Tubos":
                        Tubos.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                    case "Topografía":
                        Topografias.Add(new RevitElementoBase { Id = item["Id"].ToString().Trim(), Categoria = item["Categoria"].ToString().Trim(), Familia = item["Familia"].ToString().Trim(), Tipo = item["Tipo"].ToString().Trim(), UniqueId = item["UniqueId"].ToString().Trim() });
                        break;
                }
            }

            //cargar parametros compartidos

            System.Data.DataTable datosPl1 = new System.Data.DataTable();
            datosPl = bdatos.LElementosParComp(labelItem8.Text);
            progressBar1.Maximum = datosPl.Rows.Count;
            progressBar1.Value = 0;
            progressBar1.Refresh();
            posi = 0;
            foreach (DataRow item in datosPl.Rows)
            {
                progressBar1.Value = posi;
                progressBar1.Refresh();
                posi++;
                ParametrosCompartidos.Add(new RevitParametroBase { Nombre = item["Nombre"].ToString().Trim() });
            }



            _NwData[0].Count = ArmazonesEstructurales.Count();
            _NwData[1].Count = ArmadurasEstructuales.Count();
            _NwData[2].Count = BandejasdeCables.Count();
            _NwData[3].Count = Barandillas.Count();
            _NwData[4].Count = CimentacionesEstructurales.Count();
            _NwData[5].Count = Conductos.Count();
            _NwData[6].Count = ConductosFlexibles.Count();
            _NwData[7].Count = ConexionesEstructurales.Count();
            _NwData[8].Count = Cubiertas.Count();
            _NwData[9].Count = Emplazamientos.Count();
            _NwData[10].Count = Escaleras.Count();
            _NwData[11].Count = ModelosGenericos.Count();
            _NwData[12].Count = AparatosSanitarios.Count();
            _NwData[13].Count = Muros.Count();
            _NwData[14].Count = PilaresEstructurales.Count();
            _NwData[15].Count = Rampas.Count();
            _NwData[16].Count = Suelos.Count();
            _NwData[17].Count = Techos.Count();
            _NwData[18].Count = Tuberias.Count();
            _NwData[19].Count = TuberiasFlexibles.Count();
            _NwData[20].Count = Tubos.Count();
            _NwData[21].Count = Topografias.Count();

            InitializeChart();
            PieChart pieChart = (PieChart)chartControl1.ChartPanel.ChartContainers[0];
            pieChart.PaletteGroup = (PaletteGroup)
            Enum.Parse(typeof(PaletteGroup), PaletteGroup.Color2.ToString());
            _Timer = new System.Windows.Forms.Timer();
            _Timer.Interval = 1500;
            _Timer.Tick += Timer_Tick;
            // Hook the PieSelectionChanged event so that we can give the user
            // feedback on what items they have selected.
            chartControl1.PieSelectionChanged += chartControl1_PieSelectionChanged;

            cargar_FamiliasyTipos();
        }

        void cargar_FamiliasyTipos() {

            System.Data.DataTable datosPl = new System.Data.DataTable();
            ConexionBD bdatos = new ConexionBD();

            datosPl = bdatos.LFamilias(labelItem8.Text);
            
            progressBar1.Maximum = datosPl.Rows.Count;
            progressBar1.Value = 0;
            progressBar1.Refresh();
            int posi = 0;

            foreach (DataRow item in datosPl.Rows) {

                progressBar1.Value = posi;
                progressBar1.Refresh();
                posi++;

                switch (item["Categoria"].ToString().Trim())
                {
                    case "Armazón estructural":
                        FamiliasArmazonesEstructurales.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() });
                        break;
                    case "Armadura estructural":
                        FamiliasArmadurasEstructuales.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Bandejas de cables":
                        FamiliasBandejasdeCables.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Barandillas":
                        FamiliasBarandillas.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() });
                        break;
                    case "Cimentación estructural":
                        FamiliasCimentacionesEstructurales.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() });
                        break;
                    case "Conductos":
                        FamiliasConductos.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Conductos flexibles":
                        FamiliasConductosFlexibles.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Conexiones estructurales":
                        FamiliasConexionesEstructurales.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() });
                        break;
                    case "Cubiertas":
                        FamiliasCubiertas.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Emplazamiento":
                        FamiliasEmplazamientos.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() });
                        break;
                    case "Escaleras":
                        FamiliasEscaleras.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Modelos genéricos":
                        FamiliasModelosGenericos.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Aparatos sanitarios":
                        FamiliasAparatosSanitarios.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Muros":
                        FamiliasMuros.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Pilares estructurales":
                        FamiliasPilaresEstructurales.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Rampas":
                        FamiliasRampas.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Suelos":
                        FamiliasSuelos.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Techos":
                        FamiliasTechos.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Tuberías":
                        FamiliasTuberias.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Tuberías flexibles":
                        FamiliasTuberiasFlexibles.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Tubos":
                        FamiliasTubos.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                    case "Topografía":
                        FamiliasTopografias.Add(new RevitFamiliaBase { Familia = item["Familia"].ToString().Trim() }); 
                        break;
                }
            }




            System.Data.DataTable datosPl1 = new System.Data.DataTable();
            datosPl1 = bdatos.LTipos(labelItem8.Text);
            progressBar1.Maximum = datosPl1.Rows.Count;
            progressBar1.Value = 0;
            progressBar1.Refresh();
            posi = 0;
            
            foreach (DataRow item in datosPl1.Rows)
            {
                progressBar1.Value = posi;
                progressBar1.Refresh();
                posi++;

                switch (item["Categoria"].ToString().Trim())
                {
                    case "Armazón estructural":
                        TiposArmazonesEstructurales.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Armadura estructural":
                        TiposArmadurasEstructuales.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Bandejas de cables":
                        TiposBandejasdeCables.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Barandillas":
                        TiposBarandillas.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Cimentación estructural":
                        TiposCimentacionesEstructurales.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Conductos":
                        TiposConductos.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Conductos flexibles":
                        TiposConductosFlexibles.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Conexiones estructurales":
                        TiposConexionesEstructurales.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Cubiertas":
                        TiposCubiertas.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Emplazamiento":
                        TiposEmplazamientos.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Escaleras":
                        TiposEscaleras.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Modelos genéricos":
                        TiposModelosGenericos.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Aparatos sanitarios":
                        TiposAparatosSanitarios.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Muros":
                        TiposMuros.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Pilares estructurales":
                        TiposPilaresEstructurales.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Rampas":
                        TiposRampas.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Suelos":
                        TiposSuelos.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Techos":
                        TiposTechos.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Tuberías":
                        TiposTuberias.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Tuberías flexibles":
                        TiposTuberiasFlexibles.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Tubos":
                        TiposTubos.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                    case "Topografía":
                        TiposTopografias.Add(new RevitTipoBase { Tipo = item["Tipo"].ToString().Trim() });
                        break;
                }
            }


        }



        void CargarFamilias1()
        {

            TotalElementos = new List<RevitElementoBase>();

            TotalElementos.AddRange(ArmazonesEstructurales);
            TotalElementos.AddRange(BandejasdeCables);
            TotalElementos.AddRange(Barandillas);
            TotalElementos.AddRange(CimentacionesEstructurales);
            TotalElementos.AddRange(Conductos);
            TotalElementos.AddRange(ConductosFlexibles);
            TotalElementos.AddRange(ConexionesEstructurales);
            TotalElementos.AddRange(Cubiertas);
            TotalElementos.AddRange(Emplazamientos);
            TotalElementos.AddRange(Escaleras);
            TotalElementos.AddRange(ModelosGenericos);
            TotalElementos.AddRange(Muros);
            TotalElementos.AddRange(PilaresEstructurales);
            TotalElementos.AddRange(Rampas);
            TotalElementos.AddRange(Suelos);
            TotalElementos.AddRange(Techos);
            TotalElementos.AddRange(Tuberias);
            TotalElementos.AddRange(TuberiasFlexibles);
            TotalElementos.AddRange(Tubos);
            TotalElementos.AddRange(ArmadurasEstructuales);
            TotalElementos.AddRange(AparatosSanitarios);
            TotalElementos.AddRange(Topografias);


            var ArComp = new string[5];
            var ArCategorias = new string[501];
            var ArFamilias = new string[501];
            var ArTipos = new string[501];
            var ArParametros = new string[501];
            var I = default(int);
            ArComp[0] = "";
            ArComp[1] = "Igual";
            ArComp[2] = "Diferente";

            ArFamilias[0] = "";
            ArCategorias[0] = "";
            ArTipos[0] = "";
            ArParametros[0] = "";


            // AGREGAMOS LOS PARAMETROS COMPARTIDOS
            AdvParametrosCompartidos.ClearAndDisposeAllNodes();
            AdvParametrosCompartidos.BeginUpdate();

            I = 1;
            foreach (var dato in ParametrosCompartidos)
            {
                var node = new DevComponents.AdvTree.Node();
                node.Tag = "";
                node.Text = dato.Nombre;
                node.Image = ImageList1.Images[6];
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                AdvParametrosCompartidos.Nodes.Add(node);
                node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;

                ArParametros[I] = dato.Nombre;
                I++;
            }
            AdvParametrosCompartidos.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;



            AdvCategorias.ClearAndDisposeAllNodes();
            CmbCategoria2.Items.Clear();
            CmbCategorias.Items.Clear();
            AdvCategorias.BeginUpdate();
            CmbCategoria2.Items.Add("");
            CmbCategorias.Items.Add("");

            I = 1;
            foreach (var dato in _NwData)
            {
                var node = new DevComponents.AdvTree.Node();
                node.Tag = "";
                node.Text = dato.Country;
                selecciona_categoria(dato.Country);
                ArCategorias[I] = dato.Country;
                node.Image = ImageList1.Images[0];
                CmbCategoria2.Items.Add(dato.Country);
                CmbCategorias.Items.Add(dato.Country);
                node.Cells.Add(new DevComponents.AdvTree.Cell(CategoriaSeleccionada.Count().ToString()));
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                AdvCategorias.Nodes.Add(node);
                node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                I++;
            }

            AdvCategorias.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;

            FamiliaTotal = new List<RevitFamiliaBase>();

            TipoTotal = new List<RevitTipoBase>();

            TipoTotal.AddRange(TiposArmazonesEstructurales);
            TipoTotal.AddRange(TiposBandejasdeCables);
            TipoTotal.AddRange(TiposBarandillas);
            TipoTotal.AddRange(TiposCimentacionesEstructurales);
            TipoTotal.AddRange(TiposConductos);
            TipoTotal.AddRange(TiposConductosFlexibles);
            TipoTotal.AddRange(TiposConexionesEstructurales);
            TipoTotal.AddRange(TiposCubiertas);
            TipoTotal.AddRange(TiposEmplazamientos);
            TipoTotal.AddRange(TiposEscaleras);
            TipoTotal.AddRange(TiposModelosGenericos);
            TipoTotal.AddRange(TiposMuros);
            TipoTotal.AddRange(TiposPilaresEstructurales);
            TipoTotal.AddRange(TiposRampas);
            TipoTotal.AddRange(TiposSuelos);
            TipoTotal.AddRange(TiposTechos);
            TipoTotal.AddRange(TiposTuberias);
            TipoTotal.AddRange(TiposTuberiasFlexibles);
            TipoTotal.AddRange(TiposTubos);
            TipoTotal.AddRange(TiposArmadurasEstructuales);
            TipoTotal.AddRange(TiposAparatosSanitarios);
            TipoTotal.AddRange(TiposTopografias);


            FamiliaTotal.AddRange(FamiliasArmazonesEstructurales);
            FamiliaTotal.AddRange(FamiliasBandejasdeCables);
            FamiliaTotal.AddRange(FamiliasBarandillas);
            FamiliaTotal.AddRange(FamiliasCimentacionesEstructurales);
            FamiliaTotal.AddRange(FamiliasConductos);
            FamiliaTotal.AddRange(FamiliasConductosFlexibles);
            FamiliaTotal.AddRange(FamiliasCubiertas);
            FamiliaTotal.AddRange(FamiliasEmplazamientos);
            FamiliaTotal.AddRange(FamiliasEscaleras);
            FamiliaTotal.AddRange(FamiliasModelosGenericos);
            FamiliaTotal.AddRange(FamiliasMuros);
            FamiliaTotal.AddRange(FamiliasPilaresEstructurales);
            FamiliaTotal.AddRange(FamiliasRampas);
            FamiliaTotal.AddRange(FamiliasSuelos);
            FamiliaTotal.AddRange(FamiliasTechos);
            FamiliaTotal.AddRange(FamiliasTuberias);
            FamiliaTotal.AddRange(FamiliasTuberiasFlexibles);
            FamiliaTotal.AddRange(FamiliasTubos);
            FamiliaTotal.AddRange(FamiliasArmadurasEstructuales);
            FamiliaTotal.AddRange(FamiliasAparatosSanitarios);
            FamiliaTotal.AddRange(FamiliasTopografias);


            I = 1;
            CmbFamilia.Items.Clear();
            CmbFamilia.Items.Add("");
            AdvFamilias.ClearAndDisposeAllNodes();
            AdvFamilias.BeginUpdate();
            foreach (var dato in FamiliaTotal)
            {
                var node = new DevComponents.AdvTree.Node();
                node.Tag = "";
                node.Text = dato.Familia;
                ArFamilias[I] = dato.Familia;
                node.Image = ImageList1.Images[0];
                CmbFamilia.Items.Add(dato.Familia);
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                AdvFamilias.Nodes.Add(node);
                node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                I++;
            }
            AdvFamilias.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;


            I = 1;
            AdvTreeTipos.ClearAndDisposeAllNodes();
            AdvTreeTipos.BeginUpdate();
            foreach (var dato in TipoTotal)
            {
                var node = new DevComponents.AdvTree.Node();
                node.Tag = "";
                node.Text = dato.Tipo;
                ArTipos[I] = dato.Tipo;
                node.Image = ImageList1.Images[0];
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                node.Cells.Add(new DevComponents.AdvTree.Cell(""));
                AdvTreeTipos.Nodes.Add(node);
                node.ExpandVisibility = DevComponents.AdvTree.eNodeExpandVisibility.Hidden;
                I++;
            }
            AdvTreeTipos.EndUpdate();
            _RightAlignFileSizeStyle = new DevComponents.DotNetBar.ElementStyle();
            _RightAlignFileSizeStyle.TextAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Far;

            GridPanel panel1 = SGAsociados.PrimaryGrid;
            GridPanel panel2 = SGestructuraC.PrimaryGrid;
            panel1.Columns[2].EditorType = typeof(FragrantComboBox);
            panel1.Columns[2].EditorParams = new object[] { ArCategorias };
            panel1.Columns[3].EditorType = typeof(FragrantComboBox);
            panel1.Columns[3].EditorParams = new object[] { ArFamilias };
            panel1.Columns[4].EditorType = typeof(FragrantComboBox);
            panel1.Columns[4].EditorParams = new object[] { ArTipos };
            panel1.Columns[5].EditorType = typeof(FragrantComboBox);
            panel1.Columns[5].EditorParams = new object[] { ArParametros };

            panel1.Columns[6].EditorType = typeof(FragrantComboBox);
            panel1.Columns[6].EditorParams = new object[] { ArComp };


            panel2.Columns[3].EditorType = typeof(FragrantComboBox);
            panel2.Columns[3].EditorParams = new object[] { ArParametros };

            //panel2.Columns[4].EditorType = typeof();
            //checkbox1.CheckedChanged += new EventHandler(this.Check_Clicked);

        }



        #endregion

        private void timer13_Tick(object sender, EventArgs e)
        {
            timer13.Enabled = false;
            this.Refresh();
            GrpCargar.Top = (int)((this.Height/2) - 80);
            GrpCargar.Left = 50;
            GrpCargar.Width = (int)((this.Width) - 100);
            GrpCargar.Text = "Obteniendo la información del modelo";
            GrpCargar.Visible = true;
            GrpCargar.Refresh();
            var baseC = new ConexionBD();
            if (!existeTabla(labelItem8.Text))
            {
                baseC.crearTablaElementos(labelItem8.Text);
                //llenar los datos en las tablas Una sola vez por modelo
                cargar_elementos_en_tablas();
            }
            else {

                if (TotalElementos is null)
                {
                    cargar_ElementosBD();
                    CargarFamilias1();
                }
                else {
                    System.Windows.Forms.MessageBox.Show("Estan cargados " + TotalElementos.Count().ToString(),"");
                }
                //si los datos estan vacios
            }

            GrpCargar.Visible = false;
            GrpCargar.Refresh();
        }

        private void buttonItem30_Click(object sender, EventArgs e)
        {

            GrpCargar.Top = (int)((this.Height / 2) - 80);
            GrpCargar.Left = 50;
            GrpCargar.Width = (int)((this.Width) - 100);
            GrpCargar.Visible = true;
            GrpCargar.Text = "Obteniendo la información del modelo";
            GrpCargar.Refresh();
            var baseC = new ConexionBD();
            if (!existeTabla(labelItem8.Text))
            {
                baseC.crearTablaElementos(labelItem8.Text);
            }
            baseC.Conexion();
            baseC.EliminarDatosModelo(labelItem8.Text);
            baseC.Conexion();
            //llenar los datos en las tablas Una sola vez por modelo
            cargar_elementos_en_tablas();
            GrpCargar.Visible = false;
            GrpCargar.Refresh();
        }

        private void buttonItem10_Click(object sender, EventArgs e)
        {
            //establecer una estructura de metrado en todos los items de un subpresupuesto
            GridPanel Panel = SGestructuraC.PrimaryGrid;
            GridPanel Panel1 = SGPresupC.PrimaryGrid;
            var baseC = new ConexionBD();
            baseC.Conectar();
            baseC.EliminarEstructuraXSubPresupuesto(Presupuesto_actual, SubPresupuesto_actual); //esta funcion me falta en la tabla central

            GrpCargar.Top = (int)((this.Height / 2) - 80);
            GrpCargar.Left = 50;
            GrpCargar.Width = (int)((this.Width) - 100);
            GrpCargar.Visible = true;
            GrpCargar.Text = "Estableciendo configuración de Estructura en todo el subPresupuesto ";
            GrpCargar.Refresh();

            progressBar1.Maximum = Panel.Rows.Count() - 1;
            progressBar1.Value = 0;
            progressBar1.Refresh();
            int posi = 0;



            foreach (GridRow fila1 in Panel1.Rows) //recorro todas las filas de mi subpresupuesto
            {
                if (!(fila1.Cells[0].Value is null)) {
                    if (fila1.Rows.Count != 0)
                    { //si tiene hijos
                        recorrer_hijos_sub(fila1);
                    }
                    else
                    { //agrego la estructura al item actual
                        foreach (GridRow ff in Panel.Rows)
                        {
                            if (!(ff.Cells[0].Value is null))
                            {
                                progressBar1.Value = posi;
                                progressBar1.Refresh();
                                posi++;
                                //borro los registro de la estructura actual PRESUPUESTO Y SUBPRESUPUESTO
                                string CodigoUn = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                                string campoMostrar = "";
                                if ((bool)ff.Cells[4].Value == true) campoMostrar = "true";
                                if ((bool)ff.Cells[4].Value == false) campoMostrar = "false";
                                //GuardarEstructura(CodigoUn, ff.Cells[2].Value.ToString(), ff.Cells[3].Value.ToString(), campoMostrar);
                                baseC.GuardarEstructura(Presupuesto_actual, SubPresupuesto_actual, fila1.Cells[17].Value.ToString(), CodigoUn, ff.Cells[2].Value.ToString(), ff.Cells[3].Value.ToString(), campoMostrar);
                                //hacer actualizacion masiva para la tabla central **********************************************
                            }
                        }
                    }
                }
            }
            baseC.desConectar();

            GrpCargar.Visible = false;
            GrpCargar.Refresh();


        }

        void recorrer_hijos_sub(GridRow fila) {
            GridPanel Panel = SGestructuraC.PrimaryGrid;
            var baseC = new ConexionBD();
            baseC.Conectar();
            foreach (GridRow fila1 in fila.Rows) //recorro todas las filas de mi subpresupuesto
            {
                if (fila1.Rows.Count != 0)
                { //si tiene hijos
                    recorrer_hijos_sub(fila1);
                }
                else
                { //agrego la estructura al item actual
                    foreach (GridRow ff in Panel.Rows)
                    {
                        if (!(ff.Cells[0].Value is null))
                        {
                            //borro los registro de la estructura actual PRESUPUESTO Y SUBPRESUPUESTO
                            string CodigoUn = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                            string campoMostrar = "";
                            if ((bool)ff.Cells[4].Value == true) campoMostrar = "true";
                            if ((bool)ff.Cells[4].Value == false) campoMostrar = "false";
                            //GuardarEstructura(CodigoUn, ff.Cells[2].Value.ToString(), ff.Cells[3].Value.ToString(), campoMostrar);
                            baseC.GuardarEstructura(Presupuesto_actual, SubPresupuesto_actual, fila1.Cells[17].Value.ToString(), CodigoUn, ff.Cells[2].Value.ToString(), ff.Cells[3].Value.ToString(), campoMostrar);
                            //hacer actualizacion masiva para la tabla central **********************************************
                        }
                    }
                }

            }
            baseC.desConectar();


        }

        private void SGFormulas_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                GridPanel panel = (GridPanel)SGFormulas.PrimaryGrid;
                GridRow fila = (GridRow)panel.ActiveRow;
                if (fila is null) return;
                if (fila.Index == panel.Rows.Count - 1) return;
                System.Drawing.Point pt = System.Windows.Forms.Control.MousePosition;
                BtCalculos.Popup(pt);
            }
        }

        private void buttonItem27_Click(object sender, EventArgs e)
        {
            GridPanel panelP = (GridPanel)SGPresupC.PrimaryGrid;
            GridPanel panel = (GridPanel)SGDetFormulas.PrimaryGrid;
            GridPanel panel1 = (GridPanel)SGFormulas.PrimaryGrid;

            GridRow FilaP = (GridRow) panelP.ActiveRow;
            GridRow filaActual = (GridRow)panel1.Rows[0];

            // verificar que no esten vacios todos
            if (filaActual.Cells[1].Value.ToString() == "" && filaActual.Cells[2].Value.ToString() == "" && filaActual.Cells[3].Value.ToString() == "" && filaActual.Cells[4].Value.ToString() == "" && filaActual.Cells[5].Value.ToString() == "") {
                System.Windows.Forms.MessageBox.Show("El Registro está vacio", "");
                return;
            }


            var baseC = new ConexionBD();
            baseC.Conectar();
            string txt = Microsoft.VisualBasic.Interaction.InputBox("Nueva Configuración ", "Escriba el Nombre de esta Configuración :", "Mi Configuración");
            if (!string.IsNullOrEmpty(txt))
            {
                //Guardo los datos
                string CodigoUn = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                baseC.GuardarConfCalculo(CodigoUn, txt.ToString().ToUpper(), FilaP.Cells[3].Value.ToString(), filaActual.Cells[1].Value.ToString(), filaActual.Cells[2].Value.ToString(), filaActual.Cells[3].Value.ToString(), filaActual.Cells[4].Value.ToString(), filaActual.Cells[5].Value.ToString());
                //GuardarConfCalculoDetalle(string CodConfCalculoDetalle, string CodConfCalculo, string TipoCampo, string Campo, string Operacion, string Posicion)
                //foreach ()
                //ListaCalculoDetalle
                //List<LCalculoDetalle> listaDetFormulaFitrada = ListaCalculoDetalle.Where(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Longitud").OrderBy(X => X.Posicion).ToList();
                foreach (var dato in ListaCalculoDetalle)
                {
                    if (dato.Posicion is null) dato.Posicion = "";
                    //nuevafila = new GridRow(dato.CodCalculoDetalle, dato.CodCalculo, dato.Campo, dato.Operacion, dato.Posicion.ToString());
                    //panel.Rows.Add(nuevafila);
                    string CodigoUn1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    baseC.GuardarConfCalculoDetalle(CodigoUn1, CodigoUn, dato.TipoCampo, dato.Campo, dato.Operacion, dato.Posicion);
                }


                //obtener todos los datos de FormulaDetalle y Guardarlos
            }
            baseC.desConectar();
            Cargar_ConfigsUnd();
        }

        private void BtObtenerCantidades_Click(object sender, EventArgs e)
        {
            GridPanel panel = (GridPanel)SGPresupuestos.PrimaryGrid;
            GridRow FilaP = (GridRow)panel.ActiveRow;
            var fr2 = new FrmVerAsociados();
            if (FilaP.Rows.Count == 0)
            {
                GridRow FilaP1 = (GridRow)FilaP.Parent;
                fr2.labelItem1.Text = FilaP1.Cells[1].Value.ToString();
                fr2.labelItem2.Text = labelItem9.Text;
                fr2.Token = txtAccessToken.Text;
                Presupuesto_actual = FilaP1.Cells[0].Value.ToString();
                //fr2.ListaItems = (List<LItems>) ListaItems.ToList();
                fr2.filaCargar = FilaP1;
            }
            else {
                Presupuesto_actual = FilaP.Cells[0].Value.ToString();
                fr2.labelItem1.Text = FilaP.Cells[1].Value.ToString();
                fr2.labelItem2.Text = labelItem9.Text;
                fr2.Token = txtAccessToken.Text;
                fr2.filaCargar = FilaP;
            }
            
            fr2.Show();

        }

        private void Advtree4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Advtree4.SelectedNode is null) return;
            if (Advtree4.SelectedNode.Text == "TODOS")
            {
                do
                {
                    SGPresupuestos.PrimaryGrid.DeleteAll();
                    SGPresupuestos.PrimaryGrid.PurgeDeletedRows();
                }
                while (SGPresupuestos.PrimaryGrid.Rows.Count != 0);
                timer5.Enabled = true;
            }
            else {
                do
                {
                    SGPresupuestos.PrimaryGrid.DeleteAll();
                    SGPresupuestos.PrimaryGrid.PurgeDeletedRows();
                }
                while (SGPresupuestos.PrimaryGrid.Rows.Count != 0);
               GridRow fila = new GridRow(Advtree4.SelectedNode.Tag.ToString(), Advtree4.SelectedNode.Text, Advtree4.SelectedNode.Cells[2].ToString(), Advtree4.SelectedNode.Cells[3].ToString(), "0");
               SGPresupuestos.PrimaryGrid.Rows.Add(fila);
                fila.Expanded = true;
                fila.Cells[1].CellStyles.Default.Image = ImageList1.Images[0];
            }
        }

        private void AdvTree7_NodeClick(object sender, TreeNodeMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                System.Drawing.Point pt = System.Windows.Forms.Control.MousePosition;
                BtCalculoP.Popup(pt);
            }
        }

        private void buttonItem2_Click(object sender, EventArgs e)
        {
            FrmModFormula fr = new FrmModFormula();
            fr.labelItem1.Text = AdvTree7.SelectedNode.Text;
            fr.codigosel = AdvTree7.SelectedNode.Tag.ToString();
            fr.ShowDialog();
        }

        private void buttonItem3_Click(object sender, EventArgs e)
        {
            
            ConexionBD bdatos = new ConexionBD();
            bdatos.Conexion();
            if (System.Windows.Forms.MessageBox.Show("Desea Eliminar la configuracion " + AdvTree7.SelectedNode.Text + "?", "Advertencia", MessageBoxButtons.YesNo) == DialogResult.Yes) {
                bdatos.EliminarConfCalculo(AdvTree7.SelectedNode.Tag.ToString());
                bdatos.EliminarConfCalculoDetalle2(AdvTree7.SelectedNode.Tag.ToString());
                AdvTree7.SelectedNode.Visible = false;
            }
            bdatos.Conexion();
        }

        private void ButtonItem68_Click(object sender, EventArgs e)
        {
            GridPanel panelP = (GridPanel)SGPresupC.PrimaryGrid;
            GridPanel panel = (GridPanel)SGDetFormulas.PrimaryGrid;
            GridPanel panel1 = (GridPanel)SGFormulas.PrimaryGrid;

            GridRow FilaP = (GridRow)panelP.ActiveRow;
            GridRow filaActual = (GridRow)panel1.Rows[0];

            // verificar que no esten vacios todos
            /*if (filaActual.Cells[1].Value.ToString() == "" && filaActual.Cells[2].Value.ToString() == "" && filaActual.Cells[3].Value.ToString() == "" && filaActual.Cells[4].Value.ToString() == "" && filaActual.Cells[5].Value.ToString() == "")
            {
                System.Windows.Forms.MessageBox.Show("El Registro está vacio", "");
                return;
            }*/


            var baseC = new ConexionBD();
            
            string txt = Microsoft.VisualBasic.Interaction.InputBox("Nueva Configuración ", "Escriba el Nombre de esta Configuración :", "Mi Configuración");
            if (!string.IsNullOrEmpty(txt))
            {
                //Guardo los datos
                baseC.Conectar();
                string CodigoUn = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                baseC.GuardarConfCalculo(CodigoUn, txt.ToString().ToUpper(), FilaP.Cells[3].Value.ToString(), "", "", "", "", "");
                /*foreach (var dato in ListaCalculoDetalle)
                {
                    if (dato.Posicion is null) dato.Posicion = "";
                    //nuevafila = new GridRow(dato.CodCalculoDetalle, dato.CodCalculo, dato.Campo, dato.Operacion, dato.Posicion.ToString());
                    //panel.Rows.Add(nuevafila);
                    string CodigoUn1 = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                    baseC.GuardarConfCalculoDetalle(CodigoUn1, CodigoUn, dato.TipoCampo, dato.Campo, dato.Operacion, dato.Posicion);
                }*/
                baseC.desConectar();
                FrmModFormula fr = new FrmModFormula();
                fr.labelItem1.Text = txt.ToString();
                fr.codigosel = CodigoUn;
                fr.ShowDialog();

                //obtener todos los datos de FormulaDetalle y Guardarlos
            }
            
                        
            Cargar_ConfigsUnd();

        }

        private void buttonItem24_Click(object sender, EventArgs e)
        {
            FrmBuscarPres fr = new FrmBuscarPres();
            fr.seleccionado = 0;
            fr.ShowDialog();
            if (fr.seleccionado == 1) {
                
                    do
                    {
                        SGPresupuestos.PrimaryGrid.DeleteAll();
                        SGPresupuestos.PrimaryGrid.PurgeDeletedRows();
                    }
                    while (SGPresupuestos.PrimaryGrid.Rows.Count != 0);
                    //timer5.Enabled = true;
     
                   

                GridRow fila = new GridRow(fr.Fsel.Cells[0].Value.ToString(), fr.Fsel.Cells[1].Value.ToString(), fr.Fsel.Cells[2].Value.ToString(), fr.Fsel.Cells[3].Value.ToString(), "0");
                //GridRow fila = fr.Fsel;
                SGPresupuestos.PrimaryGrid.Rows.Add(fila);
                fila.Expanded = true;
                fila.Cells[1].CellStyles.Default.Image = ImageList1.Images[0];
                

            }
        }
    }
}
