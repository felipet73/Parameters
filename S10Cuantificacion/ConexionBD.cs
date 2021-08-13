using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace S10Cuantificacion
{
    public class ConexionBD
    {
        //var stringConexion = global.CProyectos.My.MySettings.Default.DB
        SqlCommand comando = new SqlCommand();
        SqlConnection cn;
        public SqlConnection Conexion() {
            cn = new SqlConnection("Data Source=CTORRES;Initial Catalog=Metrados;User ID=sa");
            if (cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
            else {
                cn.Open();
            }
            return cn;
        }


        public SqlConnection Conectar()
        {
            cn = new SqlConnection("Data Source=CTORRES;Initial Catalog=Metrados;User ID=sa");
            if (cn.State == ConnectionState.Open)
            {
               // cn.Close();
            }
            else
            {
                cn.Open();
            }
            return cn;
        }

        public SqlConnection desConectar()
        {
            cn = new SqlConnection("Data Source=CTORRES;Initial Catalog=Metrados;User ID=sa");
            if (cn.State == ConnectionState.Open)
            {
                cn.Close();
            }
            return cn;
        }


        public void crearTablaPlano()
        {
            Conexion();
            using (SqlCommand command = new SqlCommand("CREATE TABLE LPlano(CodPlano char(25), NombreArchivoRvt char(100), RutaArchivoRvt char(200), UrnAddIn char(200),UrnWeb char(200),EmailUsuario char(40) primary key (CodPlano) );", cn))
                command.ExecuteNonQuery();
            Conexion();
        }

        public void crearTablaMedicion() {
            Conexion();
            using (SqlCommand command = new SqlCommand("CREATE TABLE LMedicion(CodMedicion char(25),CodPresupuesto char(7),CodSubpresupuesto char(3),Item char(20),Descripcion char(100),Cantidad char(12),Longitud char(12),Ancho char(12),Alto char(12),Total char(15),Detalle char(80),Vinculo char(25),UniqueId char(60),PhantomParentId char(25),Nivel int,Tipo char(10),EmailUsuario char(40) primary key (CodMedicion) FOREIGN KEY (Vinculo) REFERENCES LPlano(CodPlano));", cn))
            command.ExecuteNonQuery();
            Conexion();
        }


        public void crearTablaAsociado()
        {
            Conexion();
            using (SqlCommand command = new SqlCommand("CREATE TABLE LAsociado(CodAsociado char(25),CodPresupuesto char(7),CodSubpresupuesto char(3),Item char(20),Categoria char(50),Familia char(80),Tipo char(80),CampoFiltro char(35),Valor char(20) primary key (CodAsociado));", cn))
                command.ExecuteNonQuery();
            Conexion();
        }


        public void crearTablaConfCalculo()
        {
            Conexion();
            using (SqlCommand command = new SqlCommand("CREATE TABLE LConfCalculo(CodConfCalculo char(25),Nombre char(50),Unidad char(8),Descripcion char(35),Cantidad char(35),Longitud char(100),Ancho char(100),Alto char(100) primary key (CodConfCalculo));", cn))
                command.ExecuteNonQuery();
            Conexion();
        }

        public void crearTablaConfCalculoDetalle()
        {
            Conexion();
            using (SqlCommand command = new SqlCommand("CREATE TABLE LConfCalculoDetalle(CodConfCalculoDetalle char(25), CodConfCalculo char(25), TipoCampo char(10),Campo char(35),Operacion char(2),Posicion int primary key (CodConfCalculoDetalle) FOREIGN KEY (CodConfCalculo) REFERENCES LConfCalculo(CodConfCalculo));", cn))
                command.ExecuteNonQuery();
            Conexion();
        }




        public DataTable TablaDatos(string Tabla, string Presupuesto_actual, string SubPresupuesto_actual, string Item_actual)
        {
            Conexion();
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM " + Tabla + " WHERE CodPresupuesto='" + Presupuesto_actual + "' AND CodSubpresupuesto='" + SubPresupuesto_actual + "' AND Item='" + Item_actual + "';";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch { }
            Conexion();
            return dt;
        }





        public void GuardarAsociado(string Presupuesto_actual, string SubPresupuesto_actual, string Item_actual, string CodAsociado, string Categoria, string Familia, string Tipo, string campoFiltro, string valorFiltro)
        {

            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM LAsociado WHERE CodAsociado='" + CodAsociado + "';";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch{}

            if (dt.Rows.Count == 0)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO LAsociado VALUES('" + CodAsociado + "','" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + Categoria + "','" + Familia + "','" + Tipo + "','" + campoFiltro + "','" + valorFiltro + "');", cn))
                    command.ExecuteNonQuery();
            }
            else {
                using (SqlCommand command = new SqlCommand("UPDATE LAsociado SET Categoria ='" + Categoria + "', Familia ='" + Familia + "', Tipo ='" + Tipo + "', CampoFiltro ='" + campoFiltro + "', Valor ='" + valorFiltro + "' WHERE CodAsociado ='" + CodAsociado + "';", cn))
                    command.ExecuteNonQuery();
            }
        }





        public void EliminarAsociado(string CodAsociado) {
            using (SqlCommand command = new SqlCommand("DELETE FROM LAsociado WHERE CodAsociado='" + CodAsociado + "';", cn))
                command.ExecuteNonQuery();
        }


        public void crearTablaEstructura()
        {
            Conexion();
            using (SqlCommand command = new SqlCommand("CREATE TABLE LEstructura(CodEstructura char(25),CodPresupuesto char(7),CodSubpresupuesto char(3),Item char(20),Nivel char(10),Campo char(35),Mostrar char(6) primary key (CodEstructura));", cn))
                command.ExecuteNonQuery();
            Conexion();
        }

        public void GuardarEstructura(string Presupuesto_actual, string SubPresupuesto_actual, string Item_actual, string CodEstructura, string Nivel, string Campo, string Mostrar)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM LEstructura WHERE CodEstructura='" + CodEstructura + "';";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch { }

            if (dt.Rows.Count == 0)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO LEstructura VALUES('" + CodEstructura + "','" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + Nivel + "','" + Campo + "','" + Mostrar + "');", cn))
                    command.ExecuteNonQuery();
            }
            else
            {
                using (SqlCommand command = new SqlCommand("UPDATE LEstructura SET Nivel ='" + Nivel + "', Campo ='" + Campo + "', Mostrar ='" + Mostrar + "' WHERE CodEstructura ='" + CodEstructura + "';", cn))
                    command.ExecuteNonQuery();
            }
        }

        public void EliminarEstructura(string CodEstructura)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LEstructura WHERE CodEstructura='" + CodEstructura + "';", cn))
                command.ExecuteNonQuery();
        }


        public void EliminarEstructuraXSubPresupuesto(string CodPresupuesto, string CodSubpresupuesto)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LEstructura WHERE CodPresupuesto='" + CodPresupuesto + "' AND CodSubpresupuesto='" + CodSubpresupuesto + "';", cn))
                command.ExecuteNonQuery();
        }




        public void crearTablaCalculo()
        {
            Conexion();
            using (SqlCommand command = new SqlCommand("CREATE TABLE LCalculo(CodCalculo char(25),CodPresupuesto char(7),CodSubpresupuesto char(3),Item char(20),Descripcion char(35),Cantidad char(35),Longitud char(100),Ancho char(100),Alto char(100) primary key (CodCalculo));", cn))
                command.ExecuteNonQuery();
            Conexion();
        }

        public void GuardarCalculo(string Presupuesto_actual, string SubPresupuesto_actual, string Item_actual, string CodCalculo, string Descripcion, string Cantidad, string Longitud, string Ancho, string Alto)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM LCalculo WHERE CodCalculo='" + CodCalculo + "';";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch { }

            if (dt.Rows.Count == 0)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO LCalculo VALUES('" + CodCalculo + "','" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + Descripcion + "','" + Cantidad + "','" + Longitud + "','" + Ancho + "','" + Alto + "');", cn))
                    command.ExecuteNonQuery();
            }
            else
            {
                using (SqlCommand command = new SqlCommand("UPDATE LCalculo SET Descripcion ='" + Descripcion + "', Cantidad ='" + Cantidad + "', Longitud ='" + Longitud + "', Ancho ='" + Ancho + "', Alto ='" + Alto + "'  WHERE CodCalculo ='" + CodCalculo + "';", cn))
                    command.ExecuteNonQuery();
            }
        }

        public void EliminarCalculo(string CodCalculo)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LCalculo WHERE Codcalculo='" + CodCalculo + "';", cn))
                command.ExecuteNonQuery();
        }






        public void GuardarConfCalculo(string CodConfCalculo, string Nombre, string Unidad, string Descripcion, string Cantidad, string Longitud, string Ancho, string Alto)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM LConfCalculo WHERE CodConfCalculo='" + CodConfCalculo + "';";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch { }
            //LConfCalculo(CodConfCalculo char(25), Nombre char(50), Unidad char(8), Descripcion char(35), Cantidad char(35), Longitud char(100), Ancho char(100), Alto char(100) primary key(CodConfCalculo)); ", cn))
            if (dt.Rows.Count == 0)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO LConfCalculo VALUES('" + CodConfCalculo + "','" + Nombre + "','" + Unidad + "','" + Descripcion + "','" + Cantidad + "','" + Longitud + "','" + Ancho + "','" + Alto + "');", cn))
                    command.ExecuteNonQuery();
            }
            else
            {
                using (SqlCommand command = new SqlCommand("UPDATE LConfCalculo SET Descripcion ='" + Descripcion + "', Cantidad ='" + Cantidad + "', Longitud ='" + Longitud + "', Ancho ='" + Ancho + "', Alto ='" + Alto + "'  WHERE CodConfCalculo ='" + CodConfCalculo + "';", cn))
                    command.ExecuteNonQuery();
            }
        }

        public void EliminarConfCalculo(string CodConfCalculo)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LConfCalculo WHERE CodConfCalculo='" + CodConfCalculo + "';", cn))
                command.ExecuteNonQuery();
        }


        public DataTable TablaConfCalculoUnid(string Unidad)
        {
            Conexion();
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM LConfCalculo WHERE Unidad='" + Unidad + "';";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch { }
            Conexion();
            return dt;
        }

        public DataTable TablaConfCalculo(string Codigo)
        {
            Conexion();
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM LConfCalculo WHERE CodConfCalculo='" + Codigo + "';";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch { }
            Conexion();
            return dt;
        }


        public void crearTablaCalculoDetalle()
        {
            Conexion();
            using (SqlCommand command = new SqlCommand("CREATE TABLE LCalculoDetalle(CodCalculoDetalle char(25),CodCalculo char(25),CodPresupuesto char(7),CodSubpresupuesto char(3),Item char(20),TipoCampo char(10),Campo char(35),Operacion char(2),Posicion int primary key (CodCalculoDetalle) FOREIGN KEY (CodCalculo) REFERENCES LCalculo(CodCalculo));", cn))
                command.ExecuteNonQuery();
            Conexion();
        }


        public void GuardarCalculoDetalle(string Presupuesto_actual, string SubPresupuesto_actual, string Item_actual, string CodCalculoDetalle,string CodCalculo, string TipoCampo, string Campo, string Operacion, string Posicion)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM LCalculoDetalle WHERE CodCalculoDetalle='" + CodCalculoDetalle + "';";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch { }

            if (dt.Rows.Count == 0)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO LCalculoDetalle VALUES('" + CodCalculoDetalle + "','" + CodCalculo + "','" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + TipoCampo + "','" + Campo + "','" + Operacion + "','" + Posicion + "');", cn))
                    command.ExecuteNonQuery();
            }
            else
            {
                using (SqlCommand command = new SqlCommand("UPDATE LCalculoDetalle SET TipoCampo ='" + TipoCampo + "', Campo ='" + Campo + "', Operacion ='" + Operacion + "', Posicion ='" + Posicion + "'  WHERE CodCalculoDetalle ='" + CodCalculoDetalle + "';", cn))
                    command.ExecuteNonQuery();
            }
        }

        public void EliminarCalculoDetalle(string CodCalculoDetalle)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LCalculoDetalle WHERE Codcalculo='" + CodCalculoDetalle + "';", cn))
                command.ExecuteNonQuery();
        }



        public void GuardarConfCalculoDetalle(string CodConfCalculoDetalle, string CodConfCalculo, string TipoCampo, string Campo, string Operacion, string Posicion)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM LConfCalculoDetalle WHERE CodConfCalculoDetalle='" + CodConfCalculoDetalle + "';";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch { }
            //using (SqlCommand command = new SqlCommand("CREATE TABLE LConfCalculoDetalle(LConfCalculoDetalle char(25), CodConfCalculo char(25), TipoCampo char(10),Campo char(35),Operacion char(2),Posicion int primary key (LConfCalculoDetalle) FOREIGN KEY (CodConfCalculo) REFERENCES LConfCalculo(CodConfCalculo));", cn))
                if (dt.Rows.Count == 0)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO LConfCalculoDetalle VALUES('" + CodConfCalculoDetalle + "','" + CodConfCalculo + "','" + TipoCampo + "','" + Campo + "','" + Operacion + "','" + Posicion + "');", cn))
                    command.ExecuteNonQuery();
            }
            else
            {
                using (SqlCommand command = new SqlCommand("UPDATE LConfCalculoDetalle SET TipoCampo ='" + TipoCampo + "', Campo ='" + Campo + "', Operacion ='" + Operacion + "', Posicion ='" + Posicion + "'  WHERE CodConfCalculoDetalle ='" + CodConfCalculoDetalle + "';", cn))
                    command.ExecuteNonQuery();
            }
        }

        public void EliminarConfCalculoDetalle(string CodConfCalculoDetalle)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LConfCalculoDetalle WHERE CodConfCalculoDetalle='" + CodConfCalculoDetalle + "';", cn))
                command.ExecuteNonQuery();
        }

        public void EliminarConfCalculoDetalle1(string CodConfCalculo, string TipoCampo)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LConfCalculoDetalle WHERE CodConfCalculo='" + CodConfCalculo + "' and TipoCampo ='" + TipoCampo + "';", cn))
                command.ExecuteNonQuery();
        }

        public void EliminarConfCalculoDetalle2(string CodConfCalculo)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LConfCalculoDetalle WHERE CodConfCalculo='" + CodConfCalculo + "';", cn))
                command.ExecuteNonQuery();
        }


        public DataTable TablaConfCalculoDetalle(string Codigo)
        {
            Conexion();
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM LConfCalculoDetalle WHERE CodConfCalculo='" + Codigo + "' ORDER BY(Posicion);";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch { }
            Conexion();
            return dt;
        }




        /*public class RevitElementoBase
        {
            public string Id { get; set; }
            public string Categoria { get; set; }
            public string Familia { get; set; }
            public string Tipo { get; set; }
            public string UniqueId { get; set; }
        }*/
        public void crearTablaElementos(string NombrePlano)
        {
            Conexion();
            using (SqlCommand command = new SqlCommand("CREATE TABLE [" + NombrePlano + "] (Id char(50),Categoria char(50),Familia char(80),Tipo char(80),UniqueId char(60));", cn))
            command.ExecuteNonQuery();

            /*using (SqlCommand command = new SqlCommand("CREATE TABLE [" + NombrePlano + "Cat] (Id char(60), Nombre char(50));", cn))
                command.ExecuteNonQuery();

            using (SqlCommand command = new SqlCommand("CREATE TABLE [" + NombrePlano + "Fam] (Id char(60), Nombre char(80), Categoria char(50));", cn))
                command.ExecuteNonQuery();

            using (SqlCommand command = new SqlCommand("CREATE TABLE [" + NombrePlano + "Tip] (Nombre char(80), Familia char(80));", cn))
                command.ExecuteNonQuery();*/

            using (SqlCommand command = new SqlCommand("CREATE TABLE [" + NombrePlano + "ParComp] (Nombre char(80));", cn))
                command.ExecuteNonQuery();


            Conexion();
        }

        public void GuardarElemento(string NombrePlano, string Id, string Categoria, string Familia, string Tipo, string UniqueId)
        {
            using (SqlCommand command = new SqlCommand("INSERT INTO [" + NombrePlano + "] VALUES('" + Id + "','" + Categoria + "','" + Familia + "','" + @Tipo + "','" + UniqueId + "');", cn))
            command.ExecuteNonQuery();
        }
        
        public void GuardarParametros(string NombrePlano, string Nombre)
        {
            using (SqlCommand command = new SqlCommand("INSERT INTO [" + NombrePlano + "ParComp] VALUES('" + Nombre + "');", cn))
                command.ExecuteNonQuery();
        }


        public DataTable LFamilias(string NombrePlano)
        {
            Conexion();
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT DISTINCT Familia, Categoria FROM [" + NombrePlano + "];";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch
            {
                //error
            }
            Conexion();
            return dt;
        }

        public DataTable LTipos(string NombrePlano)
        {
            Conexion();
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT DISTINCT Tipo, Categoria FROM [" + NombrePlano + "];";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch
            {
                //error
            }
            Conexion();
            return dt;
        }


        public DataTable LElementos(string NombrePlano)
        {
            Conexion();
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM [" + NombrePlano + "];";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch
            {
                //error
            }
            Conexion();
            return dt;
        }

        public DataTable LElementosParComp(string NombrePlano)
        {
            Conexion();
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM [" + NombrePlano + "ParComp];";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch
            {
                //error
            }
            Conexion();
            return dt;
        }

        public void EliminarDatosModelo(string NombrePlano)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM [" + NombrePlano + "];", cn))
                command.ExecuteNonQuery();
            using (SqlCommand command = new SqlCommand("DELETE FROM [" + NombrePlano + "ParComp];", cn))
                command.ExecuteNonQuery();

        }

        public DataTable LPlanos(string NombreMdl) {
            Conexion();
            DataTable dt = new DataTable();
            try {
                comando.Connection = cn;
                comando.CommandText = "select * from Lplano where NombreArchivoRvt='" + NombreMdl + "'";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch
            {
                //error
            }
            Conexion();
            return dt;
        }


        public DataTable LPlanosCodigo(string Codigo)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "select * from Lplano where CodPlano='" + Codigo + "'";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch
            {
                //error
            }
            return dt;
        }

        public string LPlanosXcdodigo(string Cod)
        {
            string NombrePl = "";
            Conexion();
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "select * from Lplano where CodPlano='" + Cod + "'";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                    NombrePl = "";
                else
                    NombrePl = dt.Rows[0]["UrnAddIn"].ToString().Trim();
            }
            catch
            {
                //error
            }
            Conexion();
            return NombrePl;
        }


        public string LPlanosXcdodigoNombre(string Cod)
        {
            string NombrePl = "";
            Conexion();
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "select * from Lplano where CodPlano='" + Cod + "'";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                    NombrePl = "";
                else
                    NombrePl = dt.Rows[0]["NombreArchivoRvt"].ToString().Trim();
            }
            catch
            {
                //error
            }
            Conexion();
            return NombrePl;
        }


        public void GuardarMedicion1(string Presupuesto_actual, string SubPresupuesto_actual, string Item_actual, string CodMedicion, string Descripcion, string Cantidad, string Longitud, string Ancho, string Alto, string Total, string Detalle, string Vinculo, string UniqueId, string PhantomParentId, int Nivel, string Tipo, String EmailUsuario)
        {
            /*Total = Convert.ToDouble(Total).ToString(); 
            if (Longitud != "") Longitud = Convert.ToDouble(Longitud).ToString();
            if (Cantidad != "") Cantidad = Convert.ToDouble(Cantidad).ToString();
            if (Ancho != "") Ancho = Convert.ToDouble(Ancho).ToString();
            if (Alto != "") Alto = Convert.ToDouble(Alto).ToString();*/
            using (SqlCommand command = new SqlCommand("INSERT INTO LMedicion VALUES('" + CodMedicion + "','" + Presupuesto_actual + "','" + SubPresupuesto_actual + "','" + Item_actual + "','" + Descripcion + "','" + Cantidad + "','" + Longitud + "','" + Ancho + "','" + Alto + "','" + Total + "','" + Detalle + "','" + Vinculo + "','" + UniqueId + "','" + PhantomParentId + "','" + Nivel + "','" + Tipo + "','" + EmailUsuario + "');", cn))
                command.ExecuteNonQuery();
        }

        public void ActualizarMedicion1(string CodMedicion, string Descripcion, string Cantidad, string Longitud, string Ancho, string Alto, string Total, string Detalle)
        {
            using (SqlCommand command = new SqlCommand("UPDATE LMedicion SET  Descripcion='" + Descripcion + "', Cantidad ='" + Cantidad + "', Longitud='" + Longitud + "', Ancho='" + Ancho + "', Alto='" + Alto + "', Total='" + Total + "', Detalle='" + Detalle + "' WHERE CodMedicion='" + CodMedicion + "';", cn))
            command.ExecuteNonQuery();
        }

        public void ActualizarMedicion2(string CodMedicion, string Descripcion, string Cantidad, string Longitud, string Ancho, string Alto, string Total, string Detalle, string Tipo)
        {
            using (SqlCommand command = new SqlCommand("UPDATE LMedicion SET  Descripcion='" + Descripcion + "', Cantidad ='" + Cantidad + "', Longitud='" + Longitud + "', Ancho='" + Ancho + "', Alto='" + Alto + "', Total='" + Total + "', Detalle='" + Detalle + "', Tipo='" + Tipo + "' WHERE CodMedicion='" + CodMedicion + "';", cn))
                command.ExecuteNonQuery();
        }


        public void EliminarHijosMedicion(string CodPadre)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LMedicion WHERE PhantomParentId='" + CodPadre + "';", cn))
                command.ExecuteNonQuery();
        }

        public void EliminarTodoMedicion(string Presupuesto_actual, string SubPresupuesto_actual, string Item_actual)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LMedicion WHERE CodPresupuesto='" + Presupuesto_actual + "' and CodSubpresupuesto='" + SubPresupuesto_actual + "' and Item='" + Item_actual + "';", cn))
                command.ExecuteNonQuery();
        }
        public void EliminarItemMedicion(string CodMedicion)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LMedicion WHERE CodMedicion='" + CodMedicion + "';", cn))
            command.ExecuteNonQuery();
        }

        public void GuardarPlano(string CodPlano, string NombreArchivoRvt, string RutaArchivoRvt, string UrnAddIn, string UrnWeb, String EmailUsuario)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "SELECT * FROM Lplano where CodPlano='" + CodPlano + "'";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch { }
            //using (SqlCommand command = new SqlCommand("CREATE TABLE LConfCalculoDetalle(LConfCalculoDetalle char(25), CodConfCalculo char(25), TipoCampo char(10),Campo char(35),Operacion char(2),Posicion int primary key (LConfCalculoDetalle) FOREIGN KEY (CodConfCalculo) REFERENCES LConfCalculo(CodConfCalculo));", cn))
            if (dt.Rows.Count == 0)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO LPlano VALUES('" + CodPlano + "','" + NombreArchivoRvt + "','" + RutaArchivoRvt + "','" + UrnAddIn + "','" + UrnWeb + "','" + EmailUsuario + "');", cn))
                    command.ExecuteNonQuery();

            }
            else
            {
                using (SqlCommand command = new SqlCommand("UPDATE LPlano SET UrnAddIn ='" + UrnAddIn + "', UrnWeb ='" + UrnWeb + "'  WHERE CodPlano='" + CodPlano + "';", cn))
                    command.ExecuteNonQuery();
            }


        }

        public DataTable LmedicionesSubPresupuesto(string Presupuesto_actual, string SubPresupuesto_actual, string Item_actual)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "Select * from LMedicion where CodPresupuesto='" + Presupuesto_actual + "' and CodSubPresupuesto='" + SubPresupuesto_actual + "' and Item='" + Item_actual + "' order by Descripcion";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch
            {
                //error
            }

            return dt;
        }

        public DataTable LmedicionesSubPresupuesto1(string Presupuesto_actual, string SubPresupuesto_actual)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "Select DISTINCT Vinculo from LMedicion where CodPresupuesto='" + Presupuesto_actual + "' and CodSubPresupuesto='" + SubPresupuesto_actual + "' and Vinculo <> 'Personalizado'";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch
            {
                //error
            }

            return dt;
        }

        public DataTable LmedicionesSubPresupuesto2(string Presupuesto_actual, string SubPresupuesto_actual, string Vinculo)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "Select DISTINCT Item from LMedicion where CodPresupuesto='" + Presupuesto_actual + "' and CodSubPresupuesto='" + SubPresupuesto_actual + "' and Vinculo='" + Vinculo + "'";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch
            {}
            return dt;
        }

        public DataTable LmedicionesPresVinculo(string Vinculo)
        {
            DataTable dt = new DataTable();
            try
            {
                comando.Connection = cn;
                comando.CommandText = "Select DISTINCT CodPresupuesto from LMedicion where Vinculo='" + Vinculo + "'";
                SqlDataAdapter da = new SqlDataAdapter(comando);
                da.Fill(dt);
            }
            catch
            { }
            return dt;
        }

        public void DeleteMedicionesVinculo(string Presupuesto_actual, string SubPresupuesto_actual, string Item_actual, string Vinculo)
        {
            using (SqlCommand command = new SqlCommand("DELETE FROM LMedicion WHERE CodPresupuesto='" + Presupuesto_actual + "' and CodSubPresupuesto='" + SubPresupuesto_actual + "' and Item='" + Item_actual + "' and Vinculo='" + Vinculo + "'", cn))
                command.ExecuteNonQuery();
        }





        /*public class LPlano
        {
            public string[25] CodPlano
            public string[7] CodPresupuesto
            public string[3] CodSubpresupuesto
            public string[20] Item
            public string[50] NombreArchivoRvt
            public string[200] UrnAddIn
            public string[200] UrnWeb
  }*/



        /*Dim myConnection As New SqlConnection()
        Dim myData As SqlDataReader
        Dim cmd As New SqlCommand
        Dim strConexionPresupuestos As String = "Data Source=LAPC-002;Initial Catalog=Proyectos;Persist Security Info=True;User ID=sa;password= Flipo123t"*/

    }
}
