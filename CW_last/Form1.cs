using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace CW_last
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // System.Threading.Thread.Sleep(15000);
        }


        private void label4_Click(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            string jsonFile = "database.json";
            try
            {
                var jsonObj = JObject.Parse(File.ReadAllText(jsonFile));
                var users = jsonObj.GetValue("users") as JArray;
                if (users.Count < 1) label7.Visible = true;
                if (textBox1.Text.Length < 1) label8.Visible = true;
                else label8.Visible = true;
                if (textBox2.Text.Length < 1) label9.Visible = true;
                else label9.Visible = false;
                bool isAuthorized = false;
                foreach (var user in users)
                    if (user["username"].ToString() == textBox1.Text && user["psw"].ToString() == textBox2.Text)
                        isAuthorized = true;
                if (isAuthorized)
                {
                    foreach (var user in users.Where(obj => obj["username"].Value<string>() == textBox1.Text))
                    {
                        user["condition"] = "true";
                    }
                    jsonObj["users"] = users;
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText("database.json", output);
                    Form2 form2 = new Form2();
                    form2.Show();
                    this.Hide();
                }
                else
                {
                    if (!label8.Visible)
                        label6.Visible = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message.ToString(), "Ошибка");
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            label7.Visible = false;
            Form3 form3 = new Form3();
            form3.Show();
            this.Hide();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "Тема: Ресторан\n Кубанычбекова Айпери 20ВП1\n ", "Курсовой проект");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            panel2.Visible = false;

        }

        void t_Tick(object sender, EventArgs e)
        {
            panel2.Visible = false;
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Timer t = new Timer();
            t.Interval = 5000;
            t.Start();
            t.Tick += new EventHandler(t_Tick);
        }
    }
}
