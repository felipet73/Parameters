using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using DevComponents.DotNetBar.SuperGrid;
using DevComponents.AdvTree;

namespace S10Cuantificacion
{
    public partial class FrmVerAsociados : Form
    {
        
        public GridRow filaCargar = null;
        public string Token="";
        

        public FrmVerAsociados()
        {
            InitializeComponent();
        }




        private void SetupTileView()
        {
            advTree1.Nodes.Clear();
            advTree1.View = eView.Tile;
            // Define group node style
            var groupStyle = new ElementStyle();
            groupStyle.TextColor = Color.Navy;
            groupStyle.Font = new Font(advTree6.Font.FontFamily, 9.5f);
            groupStyle.Name = "groupstyle";
            advTree1.Styles.Add(groupStyle);
            // Define sub-item style, simply to make text gray
            var subItemStyle = new ElementStyle();
            subItemStyle.TextColor = Color.Gray;
            subItemStyle.Name = "subitemstyle";
            advTree1.Styles.Add(subItemStyle);

            ConexionBD bdatos = new ConexionBD();

            
            Node groupNodeSubP = default;
            Node groupNodeSemana = default;
            Node groupNodeAnteriores = default;
            int contarHoy = 0;
            int contarSemana = 0;
            int contarAnteriores = 0;

            bdatos.Conectar();
            foreach (GridRow ff in filaCargar.Rows) {
                
                groupNodeSubP = new Node("SubPresupuesto: " + ff.Cells[1].Value.ToString() + "", groupStyle);
                groupNodeSubP.Expanded = true;
                groupNodeSubP.Tag = ff.Cells[0].Value.ToString();
                advTree1.Nodes.Add(groupNodeSubP);

                System.Data.DataTable datosPl = new System.Data.DataTable();
                
                
                datosPl = bdatos.LmedicionesSubPresupuesto1(filaCargar.Cells[0].Value.ToString(), ff.Cells[0].Value.ToString());

                if (datosPl.Rows.Count != 0)
                {
                    foreach (DataRow item1 in datosPl.Rows)
                    {

                        System.Data.DataTable dt1 = new System.Data.DataTable();


                        dt1 = bdatos.LPlanosCodigo(item1["Vinculo"].ToString().Trim());

                        if (dt1.Rows.Count != 0) {

                            System.Data.DataTable dt2 = new System.Data.DataTable();
                            dt2 = bdatos.LmedicionesSubPresupuesto2(filaCargar.Cells[0].Value.ToString(), ff.Cells[0].Value.ToString(),item1["Vinculo"].ToString().Trim());

                            groupNodeSubP.Nodes.Add(CreateChildNode(dt1.Rows[0]["NombreArchivoRvt"].ToString(), "En (" + dt2.Rows.Count.ToString() + ") Partidas", dt1.Rows[0]["UrnAddIn"].ToString().Trim(),ImageList1.Images[5], subItemStyle, item1["Vinculo"].ToString().Trim()));
                        }


                            

                    }
                }
                else {
                    groupNodeSubP.Visible = false;
                //ocultamos

                }

                





            }
            bdatos.desConectar();
            /*groupNodeSemana = new Node("Ultima semana (" + contarSemana + ")", groupStyle);
            groupNodeSemana.Expanded = true;
            advTree6.Nodes.Add(groupNodeSemana);
            groupNodeAnteriores = new Node("Anteriores a una semana (" + contarAnteriores + ")", groupStyle);
            groupNodeAnteriores.Expanded = true;
            advTree6.Nodes.Add(groupNodeAnteriores);*/



            // sql2 = "Select * from PRESUPUESTOS WHERE CATALOGO =" & advTree1.SelectedNode.Tag & " AND ELABORACION = #" & Convert.ToDateTime(Date.Today()).ToString("MM/dd/yyyy") & "#"

            /*if (dt2.Rows.Count != 0)
            {
                groupNodeHoy = new Node("Hoy (" + contarHoy + ")", groupStyle);
                groupNodeHoy.Expanded = true;
                advTree6.Nodes.Add(groupNodeHoy);
                groupNodeSemana = new Node("Ultima semana (" + contarSemana + ")", groupStyle);
                groupNodeSemana.Expanded = true;
                advTree6.Nodes.Add(groupNodeSemana);
                groupNodeAnteriores = new Node("Anteriores a una semana (" + contarAnteriores + ")", groupStyle);
                groupNodeAnteriores.Expanded = true;
                advTree6.Nodes.Add(groupNodeAnteriores);

                foreach (DataRow item1 in dt2.Rows)
                {
                    DateTime date1 = new DateTime(Convert.ToDateTime(item1["ELABORACION"]).Year, Convert.ToDateTime(item1["ELABORACION"]).Month, Convert.ToDateTime(item1["ELABORACION"]).Day);
                    DateTime date2 = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

                    int result = DateTime.Compare(date1, date2);
                    int result2 = DateTime.Compare(date1, date2.AddDays(-7));

                    if (result == 0)
                    {
                        contarHoy = contarHoy + 1;
                        groupNodeHoy.Text = "Hoy (" + contarHoy + ")";
                        groupNodeHoy.Nodes.Add(CreateChildNode(item1["NOMBRE"].ToString(), item1["USUARIO"].ToString() + " " + Convert.ToDateTime(item1["ELABORACION"]).ToString("dddd dd/MMM/yyyy"), ImageList1.Images[3], subItemStyle));
                    }
                    else if (result < 0 && result2 >= 0)
                    {
                        contarSemana = contarSemana + 1;
                        groupNodeSemana.Text = "Ultima semana (" + contarSemana + ")";
                        groupNodeSemana.Nodes.Add(CreateChildNode(item1["NOMBRE"].ToString(), item1["USUARIO"].ToString() + " " + Convert.ToDateTime(item1["ELABORACION"]).ToString("dddd dd/MMM/yyyy"), ImageList1.Images[3], subItemStyle));
                    }
                    else
                    {
                        contarAnteriores = contarAnteriores + 1;
                        groupNodeAnteriores.Text = "Anteriores a una semana (" + contarAnteriores + ")";
                        groupNodeAnteriores.Nodes.Add(CreateChildNode(item1["NOMBRE"].ToString(), item1["USUARIO"].ToString() + " " + Convert.ToDateTime(item1["ELABORACION"]).ToString("dddd dd/MMM/yyyy"), ImageList1.Images[3], subItemStyle));
                    }

                    // groupNode.Nodes.Add(CreateChildNode(Trim(item1.Item("NOMBRE").ToString), Trim(item1.Item("USUARIO").ToString) & " " & Convert.ToDateTime(item1.Item("ELABORACION")).ToString("dddd dd/MMM/yyyy"), ImageList1.Images(3), subItemStyle))
                }
            }*/

            /*if (contarSubs == 0 & groupNodeSubP is object)
                groupNodeSubP.Visible = false;*/


            /*if (contarSemana == 0 & groupNodeSemana is object)
                groupNodeSemana.Visible = false;
            if (contarAnteriores == 0 & groupNodeAnteriores is object)
                groupNodeAnteriores.Visible = false;*/
        }

        private Node CreateChildNode(string nodeText, string subText, string TagD, Image image, ElementStyle subItemStyle, string codigo)
        {
            var childNode = new Node(nodeText);
            childNode.Image = image;
            childNode.Tag = TagD;
            childNode.Cells.Add(new Cell(subText, subItemStyle));
            childNode.Cells.Add(new Cell(codigo));
            return childNode;
        }

        private void groupPanel5_Click(object sender, EventArgs e)
        {

        }

        private void FrmVerAsociados_Load(object sender, EventArgs e)
        {
            SetupTileView();
        }
        public string codiVinculo = "";
        public string codisub = "";
        private void advTree1_NodeClick(object sender, TreeNodeMouseEventArgs e)
        {

            //var ItemsNive1 = FrmPresupuestos.ListaItems.Where(X => X.Nivel == 1).ToList();
            if (advTree1.SelectedNode.Parent is null) return;
            FrmPresupuestos.SubPresupuesto_actual = advTree1.SelectedNode.Parent.Tag.ToString();
            FrmPresupuestos.solicitar_datosItems1();
            codiVinculo = advTree1.SelectedNode.Cells[2].Text;
            codisub = advTree1.SelectedNode.Parent.Tag.ToString();
            //MessageBox.Show(advTree1.SelectedNode.Cells[2].Text, advTree1.SelectedNode.Parent.Tag.ToString());

            timer1.Enabled = true;

            string urn = ViewerURN(advTree1.SelectedNode.Tag.ToString(), "");
            //browser.Load(urn);

            //string urn = ViewerURN("urn:adsk.wipprod:fs.file:vf.XxTLQ7HyQJysdVLpg96WmA?version=1", "");
            //bRowser.Load(urn);
            //webControl2.WebView = new EO.WinForm.WebControl().WebView;

            //webControl2.WebView.LoadUrl("google.com");
            webControl1.WebView.LoadUrl(urn);
            EO.WebBrowser.Runtime.AddLicense(
            "3a5rp7PD27FrmaQHEPGs4PP/6KFrqKax2r1GgaSxy5916u34GeCt7Pb26bSG" +
            "prT6AO5p3Nfh5LRw4rrqHut659XO6Lto6u34GeCt7Pb26YxDs7P9FOKe5ff2" +
            "6YxDdePt9BDtrNzCnrWfWZekzRfonNzyBBDInbW1yQKzbam2xvGvcau0weKv" +
            "fLOz/RTinuX39vTjd4SOscufWbPw+g7kp+rp9unMmuX59hefi9jx+h3ks7Oz" +
            "/RTinuX39hC9RoGkscufddjw/Rr2d4SOscufWZekscu7mtvosR/4qdzBs/DO" +
            "Z7rsAxrsnpmkBxDxrODz/+iha6iywc2faLWRm8ufWZfAwAzrpeb7z7iJWZek" +
            "sefuq9vpA/Ttn+ak9QzznrSmyNqxaaa2wd2wW5f3Bg3EseftAxDyeuvBs+I=");
        }


        string ViewerURN(string urn, string viewableId)
        {
            string respuesta = string.Empty;
            var curiosidad = Base64Encode(urn);
            if (String.IsNullOrEmpty(viewableId))//vista 3D               
                                                 //respuesta = string.Format("file:///HTML/Viewer.html?URN={0}&Token={1}", Base64Encode(urn), txtAccessToken.Text);
                respuesta = string.Format(@"C:\HTML\Viewer.html?URN={0}&Token={1}", Base64Encode(urn), Token);

            else
                // respuesta = string.Format("file:///HTML/Viewer.html?URN={0}&Token={1}&ViewableId={2}", Base64Encode(urn), txtAccessToken.Text, viewableId);
                respuesta = string.Format(@"C:\HTML\Viewer.html?URN={0}&Token={1}&ViewableId={2}", Base64Encode(urn), Token, viewableId);
            return respuesta;
        }


        string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            var ddd = Convert.ToBase64String(plainTextBytes);
            return Convert.ToBase64String(plainTextBytes);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            //var ItemsNive1 = FrmPresupuestos.ListaItems.Where(X => X.Item == 1).ToList();
            System.Data.DataTable dt2 = new System.Data.DataTable();
            ConexionBD bdatos = new ConexionBD();


            advTree6.ClearAndDisposeAllNodes();

            bdatos.Conectar();
            dt2 = bdatos.LmedicionesSubPresupuesto2(FrmPresupuestos.Presupuesto_actual, codisub, codiVinculo);
            foreach (DataRow item1 in dt2.Rows)
            {
                var ItemsNive1 = FrmPresupuestos.ListaItems.Where(X => X.Item == item1["Item"].ToString().Trim()).ToList();

                if (ItemsNive1.Count != 0) {
                    advTree6.BeginUpdate();

                    DevComponents.AdvTree.Node hubNode = new DevComponents.AdvTree.Node();
                    hubNode.Tag = "";
                    hubNode.Text = (ItemsNive1.First().Descripcion.Trim());
                    hubNode.Image = imageList8.Images[16];
                    //hubNode.Cells.Add(new DevComponents.AdvTree.Cell());
                    //hubNode.Cells.Add(new DevComponents.AdvTree.Cell());
                    //nodes.Add(hubNode);
                    advTree6.Nodes.Add(hubNode);
                    advTree6.EndUpdate();

                }


            }


                //groupNodeSubP.Nodes.Add(CreateChildNode(dt1.Rows[0]["NombreArchivoRvt"].ToString(), "En (" + dt2.Rows.Count.ToString() + ") Partidas", dt1.Rows[0]["UrnAddIn"].ToString().Trim(), ImageList1.Images[5], subItemStyle, item1["Vinculo"].ToString().Trim()));


                bdatos.desConectar();
            //MessageBox.Show(FrmPresupuestos.ListaItems.Count.ToString(), "");
        }
    }
}
