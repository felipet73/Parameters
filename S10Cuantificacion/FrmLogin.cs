using Newtonsoft.Json.Linq;
using RestSharp;
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
    public partial class FrmLogin : Form
    {
        public string Token="";
        public String[] Tokens = new string[50];
        public string Empresa="";
        public string Email = "";

        public FrmLogin()
        {
            InitializeComponent();
        }

        private void LabelX4_Click(object sender, EventArgs e)
        {

        }

        private void LabelX5_Click(object sender, EventArgs e)
        {

        }

        private void TextBoxX1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBoxX2_TextChanged(object sender, EventArgs e)
        {

        }

        private async void autentificar_S10()
        {
            dynamic responseDynamic=null;
            RestClient client = new RestClient("http://200.48.100.203:5033/api");
            RestRequest request = new RestRequest("/SecurityAuthApi/LogonApp", RestSharp.Method.POST);
            request.AddParameter("ModuleId", "11");
            request.AddParameter("AccessTypeId", "2");
            request.AddParameter("UserName", TxtUsuario.Text);
            request.AddParameter("Password", TxtPassword.Text);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //IRestResponse response = await client.ExecuteTaskAsync(request);
            IRestResponse response = await client.ExecuteAsync(request);
            //if (response.Content != "")
                responseDynamic = JObject.Parse(response.Content);
            //textBox1.Text = JObject.Parse(response.Content).ToString();
            try
            {

                //if ((responseDynamic.Value.Companies))
                //JArray =
                int length = ((JArray)responseDynamic.Value["Companies"]).Count;
                //MessageBox.Show("info", length.ToString());
                CmbEmpresa.Items.Clear();
                for (int i = 0; i < length; i++) {

                    CmbEmpresa.Items.Add(responseDynamic.Value.Companies[i].Name.ToString());
                    Tokens[i] = responseDynamic.Value.Companies[i].Token.ToString();
                    //Token = responseDynamic.Value.Companies[0].Token.ToString();
                }


                
            }
            catch {
                MessageBox.Show("Error  al validar usuario o contraseña", "Error");
            }

            
        }

        private void ButtonItem3_Click(object sender, EventArgs e)
        {
            Empresa = "";
            Email = "";
            this.Close();
        }

        private void ButtonItem2_Click(object sender, EventArgs e)
        {
            
            //MessageBox.Show(CmbEmpresa.SelectedIndex.ToString(), "dato");

            if (CmbEmpresa.Text == "") {
                MessageBox.Show("Seleccione una empresa valida", "Error");
                return;
            }
            Empresa = CmbEmpresa.Text;
            Token = Tokens[CmbEmpresa.SelectedIndex].ToString();
            Email = TxtUsuario.Text;
            //MessageBox.Show(Token.ToString(), "dato");
            this.Close();
        }

        private void CmbEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void CmbEmpresa_Click(object sender, EventArgs e)
        {
            
        }

        private void ButtonItem18_Click(object sender, EventArgs e)
        {
            autentificar_S10();
        }

        private void FrmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}
