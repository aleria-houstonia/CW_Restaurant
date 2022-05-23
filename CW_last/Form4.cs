using CW_last.Models;
using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace CW_last
{
    public partial class Form4 : Form
    {
        String imageLoc = "";
        public Form4()
        {
            InitializeComponent();
            string jsonFile = "database.json";
            var jsonObj = JObject.Parse(File.ReadAllText(jsonFile));
            renderItem("category", comboBox1);
            renderItem("products", comboBox2);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "jpg files(*.jpg)|*.jpg| PNG files(*.png)|*.png| All Files(*.*)|*.*";
                if (ofd.ShowDialog() == DialogResult.OK) imageLoc = ofd.FileName;

            }
            catch (Exception) { }
            //img1.ImageLocation=imageLoc;
        }

        private void button2_Click(object sender, EventArgs e)
        {

            label6.Visible = comboBox1.SelectedItem == null || textBox1.Text.Length < 1 || textBox2.Text.Length < 1 || imageLoc.Length < 1;//|| !(textBox1.Text.All(char.IsDigit)
            if (!label6.Visible)
            {
                string jsonFile = "database.json";
                var jsonObj = JObject.Parse(File.ReadAllText(jsonFile));
                JArray productsArr = (JArray)jsonObj["products"];
                MyProduct newFood = new MyProduct(textBox1.Text, textBox2.Text, imageLoc, comboBox1.SelectedItem.ToString());
                string toChange = @newFood.returnJsonObject();
                string pattern = @"(\\[^bfrnt\\/'\""])";
                toChange = Regex.Replace(toChange, pattern, "\\$1");
                var jo = JsonConvert.DeserializeObject<JObject>(toChange);
                productsArr.Add(JObject.Parse(jo.ToString()));
                jsonObj["products"] = productsArr;
                string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj,
                                       Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(jsonFile, newJsonResult);
                renderItem("products", comboBox2);
            }
            textBox1.Text = "";
            textBox2.Text = "";
            imageLoc = "";
            comboBox1.Text = "";
        }
        private void renderItem(string nameField, ComboBox cb)
        {
            cb.Text = "";
            string jsonFile = "database.json";
            var jsonObj = JObject.Parse(File.ReadAllText(jsonFile));
            JArray arrOfData = (JArray)jsonObj[nameField];
            List<string> listData = new List<string>();
            if (nameField == "category") { listData = arrOfData.Select(item => (string)item).ToList(); }
            if (nameField == "products")
            {
                listData = arrOfData.Select(item =>
                 item.SelectToken(@"id").Value<string>() + ")" + item.SelectToken(@"name").Value<string>() + " | " + item.SelectToken(@"price").Value<string>() + " | " + item.SelectToken(@"category").Value<string>()).ToList();
            }
            cb.Items.Clear();
            foreach (string data in listData)
                cb.Items.Add(data);

        }
        //добавление категорий
        private void button3_Click(object sender, EventArgs e)
        {
            var newCategory = textBox3.Text;
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var categoryArr = jsonObj.GetValue("category") as JArray;
            int equalCat = categoryArr.Where(obj => obj.ToString() == newCategory).Count();
            label8.Visible = (equalCat > 0);
            if (!label8.Visible)
            {
                categoryArr.Add(newCategory);
                jsonObj["category"] = categoryArr;
                string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj,
                                       Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText("database.json", newJsonResult);
                renderItem("category", comboBox1);
                textBox3.Text= "";
            }

        }
        //удалить продукт
        private void button6_Click(object sender, EventArgs e)
        {
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            JArray categoryArr = (JArray)jsonObj["products"];
            string idStr = comboBox2.SelectedItem.ToString();
            string id = idStr.Substring(0, idStr.IndexOf(')'));
            var prodToDeleted = categoryArr.FirstOrDefault(obj => obj["id"].Value<string>() == id);
            categoryArr.Remove(prodToDeleted);
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("database.json", output);
            renderItem("products", comboBox2);
        }
        //изменить продукт
        private void button5_Click(object sender, EventArgs e)
        {
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            JArray productArr = (JArray)jsonObj["products"];
            string idStr = comboBox2.SelectedItem.ToString();
            string id = idStr.Substring(0, idStr.IndexOf(')'));
            string toUpdate=checkedListBox1.SelectedItem.ToString();
            string key = "";
            switch (toUpdate)
            {
                case "название": key = "name"; break;
                case "цена": key = "price"; break;
                case "категория": key = "category"; break;
                case "картинка": key = "image"; break;
                default: key = "name";break;
            }
            foreach (var product in productArr.Where(obj => obj["id"].Value<string>() == id))
            {
                product[key] = !string.IsNullOrEmpty(textBox4.Text) ? textBox4.Text : "";
            }

            jsonObj["products"] = productArr;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("database.json", output);
            renderItem("products", comboBox2);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }
    }
}
