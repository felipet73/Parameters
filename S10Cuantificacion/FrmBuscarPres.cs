using DevComponents.DotNetBar.SuperGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace S10Cuantificacion
{
    public partial class FrmBuscarPres : Form
    {
        public GridRow Fsel = null;
        public int seleccionado=0;
        /*public class LPresupuestos
        {
            public string ERPCode { get; set; }
            public string CodPresupuesto { get; set; }
            public int Nivel { get; set; }
            public string Descripcion { get; set; }
            public string PhantomId { get; set; }
            public string PhantomParentId { get; set; }
            public int Fila { get; set; }
        }*/

        public FrmBuscarPres()
        {
            InitializeComponent();
        }

        void cargar_presupuestos() {

            var ItemsNive3 = FrmPresupuestos.ListaPresupuestos.Where(X => X.Nivel == 3).ToList();

            do
            {
                SGPresupuestos.PrimaryGrid.DeleteAll();
                SGPresupuestos.PrimaryGrid.PurgeDeletedRows();
            }
            while (SGPresupuestos.PrimaryGrid.Rows.Count != 0);

            foreach (var Item in ItemsNive3)
            {
                GridRow fila = new GridRow(Item.CodPresupuesto, Item.Descripcion, Item.Nivel, Item.PhantomId, "0");
                SGPresupuestos.PrimaryGrid.Rows.Add(fila);
                fila.Cells[1].CellStyles.Default.Image = ImageList1.Images[0];
            }



        }


        private void FrmBuscarPres_Load(object sender, EventArgs e)
        {
            cargar_presupuestos();
        }

        private void ButtonItem3_Click(object sender, EventArgs e)
        {
            this.Close();
            seleccionado = 0;
        }

        private void ButtonItem2_Click(object sender, EventArgs e)
        {
            Fsel = (GridRow)SGPresupuestos.PrimaryGrid.ActiveRow;
            if (Fsel is null) return;
            
            seleccionado = 1;
            this.Close();
        }

        private void SGPresupuestos_CellDoubleClick(object sender, GridCellDoubleClickEventArgs e)
        {
            Fsel = (GridRow)SGPresupuestos.PrimaryGrid.ActiveRow;
            if (Fsel is null) return;

            seleccionado = 1;
            this.Close();
        }
    }
}
