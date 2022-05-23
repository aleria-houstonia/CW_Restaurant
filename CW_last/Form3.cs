using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CW_last
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string jsonFile = "database.json";
            var jsonObj = JObject.Parse(File.ReadAllText(jsonFile));
            string username = textBox1.Text;
            string psw = textBox2.Text;
            var hasNumber = new Regex(@"[0-9]+");
            var hasMinimum8Chars = new Regex(@".{8,}");
            var validUsername = new Regex(@"^[a-zA-Z0-9]+$");
            var hasMinimum4Symb = new Regex(@".{4,}");
            bool doRender = true;
            JArray usersArr = (JArray)jsonObj["users"];

            int equalUser = 0;
            equalUser= usersArr.Where(obj => obj.SelectToken("username").ToString()== username).Count();
            if (equalUser > 0) { label9.Visible = true; doRender = false; } else label9.Visible = false;

            if (!hasNumber.IsMatch(psw) || !hasMinimum8Chars.IsMatch(psw)) { label6.Visible = true; doRender = false; }
            else { label6.Visible = false; }

            if (!validUsername.IsMatch(username)||!hasMinimum4Symb.IsMatch(username)) { label1.Visible = true; doRender = false; }
            else { label1.Visible = false; }

            if (doRender)
            {
                 var newUser = JObject.Parse("{ 'username': '" + username + "', 'psw': '" + psw + "', 'role':  'user','condition':'false', 'order': []}");
                try
                {
                    var users = jsonObj.GetValue("users") as JArray;
                    users.Add(newUser);
                    jsonObj["users"] = users;
                    string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(jsonFile, newJsonResult);

                    Form1 form1 = new Form1();
                    form1.Show();
                    this.Hide();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message.ToString(), "Ошибка");
                }
            }

        }


    }
}
