using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
//using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using System.Xml.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DevComponents.AdvTree;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.SuperGrid;
using DevComponents.DotNetBar.SuperGrid.Style;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic

namespace S10Cuantificacion.Cantidades
{
    public partial class FrmVinculaRvt : System.Windows.Forms.Form
    {
        #region variables

        public ExternalCommandData commandData;
        //public System.Data.OleDb.OleDbConnection conex = new System.Data.OleDb.OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\MRDTC\Presup.mdb");
        //public System.Data.OleDb.OleDbCommand comm = new System.Data.OleDb.OleDbCommand();
        //private OleDbCommand myCommand;
        // Declaro una variable para armar la instrucción SQL
        private string sql;
        private DevComponents.DotNetBar.ElementStyle _RightAlignFileSizeStyle;
        public string ID_PRESUPUESTO;
        public string codigocambiar;
        public int MODIFICADO = 0;
        public string codigo_carpeta = "";
        public int posicion = 1;
        public int que_actualizo = 0;
        public string cod_rubro;
        public string nombre_rubro;
        public ICollection<ElementId> coleccion;
        public GridRow fila;
        public GridRow FILAPLANO;
        public GridRow FILACONTENEDORES;
        public GridRow FILAFAMILIA;
        public GridRow FILATIPO;
        public GridRow FILAMARCA;
        public GridRow FILAUNIDAD;
        public GridRow FILACANTIDAD;
        public GridRow FILALONGITUD;
        public GridRow FILAANCHO;
        public GridRow FILAALTO;
        public System.Data.DataTable dtceros;
        public DataRow Itceros;
        public double P_INDIRECTOS;
        public string nombre_configuracion_seleccionada = "";
        public string id_configuracion_seleccionada = "";



        #endregion

        public FrmVinculaRvt()
        {
            InitializeComponent();
        }

        private void FrmVinculaRvt_Load(object sender, EventArgs e)
        {

        }





    }
}
