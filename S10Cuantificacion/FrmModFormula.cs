using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.SuperGrid;
namespace S10Cuantificacion
{
    public partial class FrmModFormula : System.Windows.Forms.Form
    {
        public string codigosel="";
        public FrmModFormula()
        {
            InitializeComponent();
        }

        void cargar_datos() {

            GridRow fila = (GridRow)SGFormulas.PrimaryGrid.Rows[0];
            
            System.Data.DataTable datos = new System.Data.DataTable();
            ConexionBD bdatos = new ConexionBD();
            datos = bdatos.TablaConfCalculo(codigosel);

            if (datos.Rows.Count != 0)
            {
                fila.Cells[0].Value = datos.Rows[0]["CodConfCalculo"].ToString().Trim();
                fila.Cells[1].Value = datos.Rows[0]["Descripcion"].ToString().Trim();
                fila.Cells[2].Value = datos.Rows[0]["Cantidad"].ToString().Trim();
                fila.Cells[3].Value = datos.Rows[0]["Longitud"].ToString().Trim();
                fila.Cells[4].Value = datos.Rows[0]["Ancho"].ToString().Trim();
                fila.Cells[5].Value = datos.Rows[0]["Alto"].ToString().Trim();

            }
            CargaPropiedades();

        }

        private void FrmModFormula_Load(object sender, EventArgs e)
        {
            cargar_datos();
        }

        private void ButtonItem3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        internal partial class FragrantComboBox : GridComboBoxExEditControl
        {
            public FragrantComboBox(IEnumerable orderArray)
            {
                DataSource = orderArray;
            }
        }


        private void CargaPropiedades()
        {

            System.Data.DataTable elementos = new System.Data.DataTable();
            ConexionBD bdatos = new ConexionBD();
            elementos = bdatos.LElementos(FrmPresupuestos.NombrePl);
            var ArPropiedades = new string[501];
            var ArSignos = new string[501];
            var I = default(int);
            ArPropiedades[0] = "";

            ArSignos[0] = "";
            ArSignos[1] = "+";
            ArSignos[2] = "-";
            ArSignos[3] = "*";
            ArSignos[4] = "/";

            if (elementos.Rows.Count != 0) {
                string UNIQID = elementos.Rows[0]["UniqueId"].ToString().Trim();
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
                }

            }

            GridPanel panel1 = SGFormulas.PrimaryGrid;
            GridPanel panel2 = SGDetFormulas.PrimaryGrid;
            panel1.Columns[1].EditorType = typeof(FragrantComboBox);
            panel1.Columns[1].EditorParams = new object[] { ArPropiedades };
            panel1.Columns[2].EditorType = typeof(FragrantComboBox);
            panel1.Columns[2].EditorParams = new object[] { ArPropiedades };
            panel1.Columns[3].EditorType = typeof(FragrantComboBox);
            panel1.Columns[3].EditorParams = new object[] { ArPropiedades };
            panel1.Columns[4].EditorType = typeof(FragrantComboBox);
            panel1.Columns[4].EditorParams = new object[] { ArPropiedades };
            panel1.Columns[5].EditorType = typeof(FragrantComboBox);
            panel1.Columns[5].EditorParams = new object[] { ArPropiedades };

            panel2.Columns[2].EditorType = typeof(FragrantComboBox);
            panel2.Columns[2].EditorParams = new object[] { ArPropiedades };
            panel2.Columns[3].EditorType = typeof(FragrantComboBox);
            panel2.Columns[3].EditorParams = new object[] { ArSignos };


        }



        private void SGFormulas_SelectionChanged(object sender, GridEventArgs e)
        {
            GridRow fila = (GridRow)SGFormulas.PrimaryGrid.Rows[0];
            GridCell celda = (GridCell)SGFormulas.PrimaryGrid.ActiveCell;
            System.Data.DataTable datos = new System.Data.DataTable();
            ConexionBD bdatos = new ConexionBD();
            datos = bdatos.TablaConfCalculoDetalle(codigosel);
            SGDetFormulas.PrimaryGrid.DeleteAll();
            SGDetFormulas.PrimaryGrid.PurgeDeletedRows();
            GridRow nuevafila = new GridRow("", "", "", "", "");
            SGDetFormulas.PrimaryGrid.Rows.Add(nuevafila);
            GridPanel panel = (GridPanel)SGDetFormulas.PrimaryGrid;
            panel.Title.Text = "";

            string AuxTipoCampo = "";
            if (celda.ColumnIndex == 3)  AuxTipoCampo = "Longitud";  
            if (celda.ColumnIndex == 4) AuxTipoCampo = "Ancho";
            if (celda.ColumnIndex == 5) AuxTipoCampo = "Alto";
            panel.Title.Text = AuxTipoCampo;

            if (celda.ColumnIndex == 3 || celda.ColumnIndex == 4 || celda.ColumnIndex == 5) {
                SGDetFormulas.Enabled = true;
            }else
                SGDetFormulas.Enabled = false;

            if (datos.Rows.Count != 0)
            {
                foreach (DataRow Item in datos.Rows) {

                    if (Item["TipoCampo"].ToString().Trim() == AuxTipoCampo)
                    {
                        //if (dato.Posicion is null) dato.Posicion = "";
                        nuevafila = new GridRow(Item["CodConfCalculoDetalle"].ToString().Trim(), Item["CodConfCalculo"].ToString().Trim(), Item["Campo"].ToString().Trim(), Item["Operacion"].ToString().Trim(), Item["Posicion"].ToString().Trim());
                        SGDetFormulas.PrimaryGrid.Rows.Add(nuevafila);
                    }
                    /*fila.Cells[0].Value = datos.Rows[0]["CodConfCalculo"].ToString().Trim();
                    fila.Cells[1].Value = datos.Rows[0]["Descripcion"].ToString().Trim();
                    fila.Cells[2].Value = datos.Rows[0]["Cantidad"].ToString().Trim();
                    fila.Cells[3].Value = datos.Rows[0]["Longitud"].ToString().Trim();
                    fila.Cells[4].Value = datos.Rows[0]["Ancho"].ToString().Trim();
                    fila.Cells[5].Value = datos.Rows[0]["Alto"].ToString().Trim();*/
                }



            }
        }


        void actualiza_det_formula()
        {

            GridPanel panel = (GridPanel)SGDetFormulas.PrimaryGrid;
            GridPanel panel1 = (GridPanel)SGFormulas.PrimaryGrid;
            GridRow filaP = (GridRow)panel1.Rows[0];
            GridCell celdaP = (GridCell)panel1.ActiveCell;
            panel.Title.Text = "";
            if (filaP is null) return;
            if (celdaP is null) return;
            ConexionBD bdatos = new ConexionBD();

            bdatos.Conexion();

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

                //if (ListaCalculoDetalle is null) return;

                bdatos.EliminarConfCalculoDetalle1(filaP.Cells[0].Value.ToString(), "Longitud");
                //ListaCalculoDetalle.RemoveAll(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Longitud");
                
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
                    //ListaCalculoDetalle.Add(new LCalculoDetalle { CodCalculoDetalle = filaAux.Cells[0].Value.ToString(), CodCalculo = filaP.Cells[0].Value.ToString(), CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, TipoCampo = "Longitud", Campo = filaAux.Cells[2].Value.ToString(), Operacion = filaAux.Cells[3].Value.ToString(), Posicion = filaAux.Cells[4].Value.ToString() });
                    if (filaAux.Index == 0) celdaP.Value = "" + celdaP.Value.ToString() + filaAux.Cells[2].Value + filaAux.Cells[3].Value;
                    else celdaP.Value = "(" + celdaP.Value.ToString() + filaAux.Cells[2].Value + ")" + filaAux.Cells[3].Value;
                    bdatos.GuardarConfCalculoDetalle(filaAux.Cells[0].Value.ToString(), filaP.Cells[0].Value.ToString(), "Longitud", filaAux.Cells[2].Value.ToString(), filaAux.Cells[3].Value.ToString(), filaAux.Cells[4].Value.ToString());
                    //GuardarCalculoDetalle(filaAux.Cells[0].Value.ToString(), filaP.Cells[0].Value.ToString(), "Longitud", filaAux.Cells[2].Value.ToString(), filaAux.Cells[3].Value.ToString(), filaAux.Cells[4].Value.ToString());

                }
                if (panel.Rows.Count() > 1) celdaP.Value = "=" + celdaP.Value.ToString();
                if (filaP.Cells[3].Value is null) return;
                bdatos.GuardarConfCalculo(filaP.Cells[0].Value.ToString(),"","", filaP.Cells[1].Value.ToString(), filaP.Cells[2].Value.ToString(), filaP.Cells[3].Value.ToString(), filaP.Cells[4].Value.ToString(), filaP.Cells[5].Value.ToString());
                //GuardarCalculo(filaP.Cells[0].Value.ToString(), filaP.Cells[1].Value.ToString(), filaP.Cells[2].Value.ToString(), filaP.Cells[3].Value.ToString(), filaP.Cells[4].Value.ToString(), filaP.Cells[5].Value.ToString());
                //(string CodConfCalculo, string Nombre, string Unidad, string Descripcion, string Cantidad, string Longitud, string Ancho, string Alto)



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
                //if (ListaCalculoDetalle is null) return;
                //ListaCalculoDetalle.RemoveAll(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Ancho");
                bdatos.EliminarConfCalculoDetalle1(filaP.Cells[0].Value.ToString(), "Ancho");
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
                    //ListaCalculoDetalle.Add(new LCalculoDetalle { CodCalculoDetalle = filaAux.Cells[0].Value.ToString(), CodCalculo = filaP.Cells[0].Value.ToString(), CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, TipoCampo = "Ancho", Campo = filaAux.Cells[2].Value.ToString(), Operacion = filaAux.Cells[3].Value.ToString(), Posicion = filaAux.Cells[4].Value.ToString() });
                    if (filaAux.Index == 0) celdaP.Value = "" + celdaP.Value.ToString() + filaAux.Cells[2].Value + filaAux.Cells[3].Value;
                    else celdaP.Value = "(" + celdaP.Value.ToString() + filaAux.Cells[2].Value + ")" + filaAux.Cells[3].Value;
                    bdatos.GuardarConfCalculoDetalle(filaAux.Cells[0].Value.ToString(), filaP.Cells[0].Value.ToString(), "Ancho", filaAux.Cells[2].Value.ToString(), filaAux.Cells[3].Value.ToString(), filaAux.Cells[4].Value.ToString());
                    //GuardarCalculoDetalle(filaAux.Cells[0].Value.ToString(), filaP.Cells[0].Value.ToString(), "Ancho", filaAux.Cells[2].Value.ToString(), filaAux.Cells[3].Value.ToString(), filaAux.Cells[4].Value.ToString());
                }
                if (panel.Rows.Count() > 1) celdaP.Value = "=" + celdaP.Value.ToString();
                if (filaP.Cells[3].Value is null) return;
                //GuardarCalculo(filaP.Cells[0].Value.ToString(), filaP.Cells[1].Value.ToString(), filaP.Cells[2].Value.ToString(), filaP.Cells[3].Value.ToString(), filaP.Cells[4].Value.ToString(), filaP.Cells[5].Value.ToString());
                bdatos.GuardarConfCalculo(filaP.Cells[0].Value.ToString(), "", "", filaP.Cells[1].Value.ToString(), filaP.Cells[2].Value.ToString(), filaP.Cells[3].Value.ToString(), filaP.Cells[4].Value.ToString(), filaP.Cells[5].Value.ToString());

            }
            if (celdaP.ColumnIndex == 5 && celdaP.RowIndex == 0)
            {
                panel.Title.Text = "Altura";
                celdaP.ReadOnly = true;
                if (panel.Rows.Count() > 1) celdaP.Value = "";
                else
                {
                    celdaP.ReadOnly = false;
                    celdaP.Value = "";
                }
                //if (ListaCalculoDetalle is null) return;
                //ListaCalculoDetalle.RemoveAll(X => X.CodCalculo == filaP.Cells[0].Value.ToString() && X.TipoCampo == "Alto");
                bdatos.EliminarConfCalculoDetalle1(filaP.Cells[0].Value.ToString(), "Alto");
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
                    //ListaCalculoDetalle.Add(new LCalculoDetalle { CodCalculoDetalle = filaAux.Cells[0].Value.ToString(), CodCalculo = filaP.Cells[0].Value.ToString(), CodPresupuesto = Presupuesto_actual, CodSubpresupuesto = SubPresupuesto_actual, Item = Item_actual, TipoCampo = "Alto", Campo = filaAux.Cells[2].Value.ToString(), Operacion = filaAux.Cells[3].Value.ToString(), Posicion = filaAux.Cells[4].Value.ToString() });
                    if (filaAux.Index == 0) celdaP.Value = celdaP.Value.ToString() + filaAux.Cells[2].Value + filaAux.Cells[3].Value;
                    else celdaP.Value = "(" + celdaP.Value.ToString() + filaAux.Cells[2].Value + ")" + filaAux.Cells[3].Value;
                    //GuardarCalculoDetalle(filaAux.Cells[0].Value.ToString(), filaP.Cells[0].Value.ToString(), "Alto", filaAux.Cells[2].Value.ToString(), filaAux.Cells[3].Value.ToString(), filaAux.Cells[4].Value.ToString());
                    bdatos.GuardarConfCalculoDetalle(filaAux.Cells[0].Value.ToString(), filaP.Cells[0].Value.ToString(), "Alto", filaAux.Cells[2].Value.ToString(), filaAux.Cells[3].Value.ToString(), filaAux.Cells[4].Value.ToString());
                }
                if (panel.Rows.Count() > 1) celdaP.Value = "=" + celdaP.Value.ToString();
                if (filaP.Cells[3].Value is null) return;
                //GuardarCalculo(filaP.Cells[0].Value.ToString(), filaP.Cells[1].Value.ToString(), filaP.Cells[2].Value.ToString(), filaP.Cells[3].Value.ToString(), filaP.Cells[4].Value.ToString(), filaP.Cells[5].Value.ToString());
                bdatos.GuardarConfCalculo(filaP.Cells[0].Value.ToString(), "", "", filaP.Cells[1].Value.ToString(), filaP.Cells[2].Value.ToString(), filaP.Cells[3].Value.ToString(), filaP.Cells[4].Value.ToString(), filaP.Cells[5].Value.ToString());
            }

            bdatos.Conexion();
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
                else
                {
                    buttonItem19.Enabled = true;
                    BtNelimDetForm.Enabled = true;
                }

                System.Drawing.Point pt = System.Windows.Forms.Control.MousePosition;
                BtMDetCalculo.Popup(pt);
            }

        }

        private void SGDetFormulas_EndEdit(object sender, GridEditEventArgs e)
        {
            actualiza_det_formula();
        }

        private void BtNelimDetForm_Click(object sender, EventArgs e)
        {
            GridPanel panel = (GridPanel)SGDetFormulas.PrimaryGrid;
            GridRow fila = (GridRow)panel.ActiveRow;
            if (fila is null) return;

            ConexionBD bdatos = new ConexionBD();
            bdatos.Conexion();
            bdatos.EliminarConfCalculoDetalle(fila.Cells[0].Value.ToString());
            bdatos.Conexion();
            //EliminarxCodigo("CalculoDetalle", fila.Cells[0].Value.ToString());

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
            ConexionBD bdatos = new ConexionBD();
            bdatos.Conexion();

            for (int x = 0; x < panel.Rows.Count - 1; x++)
            {
                GridRow filaaux = (GridRow)panel.Rows[x];
                bdatos.Conexion();
                bdatos.EliminarConfCalculoDetalle(filaaux.Cells[0].Value.ToString());
                bdatos.Conexion();


                //EliminarxCodigo("CalculoDetalle", filaaux.Cells[0].Value.ToString());
            }
            bdatos.Conexion();
            panel.DeleteAll();
            panel.PurgeDeletedRows();
            GridRow nuevafila = new GridRow("", "", "", "", "");
            panel.Rows.Add(nuevafila);

            actualiza_det_formula();
        }

        private void SGFormulas_EndEdit(object sender, GridEditEventArgs e)
        {
            GridPanel panel = (GridPanel)SGFormulas.PrimaryGrid;
            GridRow fila = (GridRow)panel.Rows[0];
            if (fila is null) return;
            //if (fila.Index == panel.Rows.Count() - 1) return;

            try
            {
                e.GridCell.Value = Convert.ToDouble(e.GridCell.Value.ToString()).ToString("N2");
                if (e.GridCell.Value.ToString().Length > 10)
                    e.GridCell.Value = "";
            }
            catch
            {
            }

            //if (Item_actual == "") return;
            //if (TotalElementos is null) return;
            ConexionBD bdatos = new ConexionBD();
            bdatos.Conexion();
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
                bdatos.GuardarConfCalculo(fila.Cells[0].Value.ToString(), "", "", fila.Cells[1].Value.ToString(), fila.Cells[2].Value.ToString(), fila.Cells[3].Value.ToString(), fila.Cells[4].Value.ToString(), fila.Cells[5].Value.ToString());
                //GuardarCalculo(fila.Cells[0].Value.ToString(), fila.Cells[1].Value.ToString(), fila.Cells[2].Value.ToString(), fila.Cells[3].Value.ToString(), fila.Cells[4].Value.ToString(), fila.Cells[5].Value.ToString());
            }
            bdatos.Conexion();
        }
    }
}
