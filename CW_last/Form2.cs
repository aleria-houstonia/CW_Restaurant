using System;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;
using System.Linq;
using System.Collections.Generic;
using CW_last.Models;
using System.Drawing;
//using System.Drawing;

namespace CW_last
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            this.FormClosed += MyClosedHandler;
            InitializeComponent();
            totalRender();
            renderCategory();
        }
        string orders = "";
        private void totalRender()
        {
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var users = jsonObj.GetValue("users") as JArray;
            foreach (var user in users)//.Where(obj => obj["condition"].Value<string>() == "true"))
            {
                if (user["condition"].Value<string>() == "true")
                {
                    JArray cart = (JArray)user["order"];
                    button8.Visible = (user["role"].ToString() == "admin");
                    renderCartItems(cart);
                }
            }
            JArray productsArr = (JArray)jsonObj["products"];
            renderCards(productsArr);
            totalSum();
        }
        private void renderCards(JArray productsArr)
        {
            int height = 33;
            this.panel4.Controls.Clear();
            if (productsArr.Count == 0)
            {
                Label label15 = new Label();
                label15.AutoSize = true;
                label15.BackColor = System.Drawing.Color.Transparent;
                label15.Font = new System.Drawing.Font("Segoe Script", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
                label15.ForeColor = System.Drawing.Color.Gray;
                label15.Location = new System.Drawing.Point(120, 136);
                label15.Name = "label15";
                label15.Size = new System.Drawing.Size(266, 57);
                label15.TabIndex = 1;
                label15.Text = "Не найдено...";
                label15.Visible = true;
                this.panel4.Controls.Add(label15);
            }
            else
            {
                foreach (var prod in productsArr)
                {
                    renderCard(prod["id"].ToString(), prod["name"].ToString(), prod["price"].ToString(), prod["image"].ToString(), height);
                    height += 115;
                }

            }
        }

        private void addProductToCart(string id, CartItem cartProd)
        {
            JToken currUser = "";
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var users = jsonObj.GetValue("users") as JArray;
            foreach (var user in users) if (user["condition"].ToString() == "true") currUser = user;
            JArray cart = (JArray)currUser["order"];
            bool duplicate = false;

            if (cart.Count() > 0)
                foreach (var product in cart)
                    if (product["productId"].ToString() == id)
                    {
                        duplicate = true;
                        product["count"] = (Convert.ToInt32(product["count"]) + 1).ToString();
                    }

            if (!duplicate) cart.Add(JObject.Parse(cartProd.returnJsonObject()));

            foreach (var item in users)
            {
                if (item["username"].ToString() == currUser["username"].ToString())
                {
                    item["order"] = cart;
                }
            }
            jsonObj["users"] = users;

            string newJsonResult = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj,
                                       Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("database.json", newJsonResult);
            totalRender();
        }
        private void deleteProdFromCart(string id)
        {
         
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var users = jsonObj.GetValue("users") as JArray;
            foreach (var user in users)//.Where(obj => obj["condition"].Value<string>() == "true"))
            {
                if (user["condition"].Value<string>() == "true")
                {
                    JArray cart = (JArray)user["order"];

                    var prodToDeleted = cart.FirstOrDefault(obj => obj["productId"].Value<string>() == id);
                    cart.Remove(prodToDeleted);
                    string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText("database.json", output);
                    renderCartItems(cart);
                }
            }
            totalSum();
        }
        private void renderCard(string id, string name, string price, string loc, int height)
        {
            CartItem myCart = new CartItem(name, price, 1, id);
            Panel panel = new Panel();
            Button mybutton = new Button();
            mybutton.BackColor = System.Drawing.Color.Maroon;
            mybutton.Font = new System.Drawing.Font("Segoe Script", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            mybutton.ForeColor = System.Drawing.Color.White;
            mybutton.Location = new System.Drawing.Point(197, 55);
            mybutton.Name = myCart.returnJsonObject();
            mybutton.Size = new System.Drawing.Size(157, 35);
            mybutton.TabIndex = 3;
            mybutton.Text = "Добавить в корзину";
            mybutton.Click += delegate { addProductToCart(id, myCart); };
            mybutton.UseVisualStyleBackColor = false;

            Label mylabel = new Label();
            mylabel.AutoSize = true;
            mylabel.Font = new System.Drawing.Font("Segoe Script", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            mylabel.ForeColor = System.Drawing.Color.Maroon;
            mylabel.Location = new System.Drawing.Point(201, 0);
            mylabel.Name = "label" + name;
            mylabel.Size = new System.Drawing.Size(57, 25);
            mylabel.TabIndex = 1;
            mylabel.Text = name;

            Label mylabel2 = new Label();
            mylabel2.AutoSize = true;
            mylabel2.Font = new System.Drawing.Font("Segoe Script", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            mylabel2.ForeColor = System.Drawing.Color.Black;
            mylabel2.Location = new System.Drawing.Point(254, 25);
            mylabel2.Name = $"label{price}";
            mylabel2.Size = new System.Drawing.Size(56, 27);
            mylabel2.TabIndex = 2;
            mylabel2.Text = price;

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            PictureBox mypictureBox = new PictureBox();
            mypictureBox.Name = "pictureBox" + name;
            mypictureBox.ImageLocation = loc;
            mypictureBox.Location = new System.Drawing.Point(3, -18);
            mypictureBox.Size = new System.Drawing.Size(149, 129);
            mypictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            mypictureBox.TabIndex = 0;
            mypictureBox.TabStop = false;

            panel.BackColor = System.Drawing.Color.Gainsboro;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Controls.Add(mybutton);
            panel.Controls.Add(mylabel);
            panel.Controls.Add(mylabel2);
            panel.Controls.Add(mypictureBox);
            panel.Location = new System.Drawing.Point(46, height);
            panel.Name = name + "picture";
            panel.Size = new System.Drawing.Size(380, 95);
            panel.TabIndex = 4;
            this.panel4.Controls.Add(panel);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form4 form4 = new Form4();
            form4.Show();
            this.Hide();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var users = jsonObj.GetValue("users") as JArray;
            foreach (var user in users.Where(obj => obj["condition"].Value<string>() == "true"))
            {
                user["condition"] = "false";
            }
            jsonObj["users"] = users;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("database.json", output);
            Form1 form1 = new Form1();
            form1.Show();
            this.Hide();
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            string toSearch = textBox1.Text;
            if (toSearch.Length > 0)
            {
                var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
                var products = jsonObj.GetValue("products") as JArray;
                JArray searchArr = new JArray();
                foreach (var prod in products)
                    if (prod["name"].ToString().ToLowerInvariant().Contains(toSearch.ToLowerInvariant()))
                        searchArr.Add(prod);
                renderCards(searchArr);
            }
            else
            {
                var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
                var products = jsonObj.GetValue("products") as JArray;
                renderCards(products);
            }
        }
        private List<int> getMinMaxPrice()
        {
            List<int> minMax = new List<int>();
            List<int> arrOfPrices = new List<int>();
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            JArray productsArr = (JArray)jsonObj["products"];
            foreach (var product in productsArr)
            {
                arrOfPrices.Add(Int32.Parse(product["price"].ToString()));
            }
            minMax.Add(arrOfPrices.Min());
            minMax.Add(arrOfPrices.Max());
            //label10.Text = progressBar1.Maximum.ToString() + "р.";
            return minMax;

        }
        int step = 10;
        int cur = 0;
        private void button1_Click(object sender, EventArgs e)
        {

            progressBar1.Minimum = getMinMaxPrice()[0];
            progressBar1.Maximum = getMinMaxPrice()[1];
            if (progressBar1.Value < progressBar1.Maximum) progressBar1.Value += step;
            label4.Text = progressBar1.Value.ToString() + "р.";
            label5.Text = progressBar1.Maximum.ToString() + "р.";
            cur = progressBar1.Value;
            filterByPrice();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = getMinMaxPrice()[0];
            progressBar1.Maximum = getMinMaxPrice()[1];
            if (progressBar1.Value > progressBar1.Minimum && progressBar1.Value > step) progressBar1.Value -= step;
            label4.Text = progressBar1.Value.ToString() + "р.";
            label5.Text = progressBar1.Maximum.ToString() + "р.";
            cur = progressBar1.Value;
            filterByPrice();
        }
        private void renderCart(string name, int height, string count, string price, string id)
        {
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            Panel myPan = new Panel();
            Button button = new Button();
            button.Location = new System.Drawing.Point(193, 7);
            button.Name = "buttonCart" + name + height;
            button.Size = new System.Drawing.Size(29, 23);
            button.TabIndex = 9;
            button.Text = "X";
            button.UseVisualStyleBackColor = true;
            button.Click += delegate { deleteProdFromCart(id); };

            Label nameLabel = new Label();
            nameLabel.AutoSize = true;
            nameLabel.Font = new System.Drawing.Font("Segoe Script", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            nameLabel.ForeColor = System.Drawing.Color.RosyBrown;
            nameLabel.Location = new System.Drawing.Point(3, 0);
            nameLabel.Name = "label7Cart" + height + name;
            nameLabel.Size = new System.Drawing.Size(48, 20);
            nameLabel.TabIndex = 6;
            nameLabel.Text = name;

            Label priceLabel = new Label();
            priceLabel.AutoSize = true;
            priceLabel.Font = new System.Drawing.Font("Segoe Script", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            priceLabel.ForeColor = System.Drawing.Color.Gray;
            priceLabel.Location = new System.Drawing.Point(115, 19);
            priceLabel.Name = "label8Cart" + height;
            priceLabel.Size = new System.Drawing.Size(45, 20);
            priceLabel.TabIndex = 7;
            priceLabel.Text = price;


            Label productCountLabel = new Label();
            productCountLabel.AutoSize = true;
            productCountLabel.Font = new System.Drawing.Font("Segoe Script", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            productCountLabel.ForeColor = System.Drawing.Color.Teal;
            productCountLabel.Location = new System.Drawing.Point(131, -1);
            productCountLabel.Name = "labeCartl" + height;
            productCountLabel.Size = new System.Drawing.Size(28, 20);
            productCountLabel.TabIndex = 8;
            productCountLabel.Text = count + "x";

            myPan.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            myPan.Controls.Add(productCountLabel);
            myPan.Controls.Add(priceLabel);
            myPan.Controls.Add(button);
            myPan.Controls.Add(nameLabel);
            myPan.Location = new System.Drawing.Point(12, height);
            myPan.Name = "myPan" + name + height;
            myPan.Size = new System.Drawing.Size(229, 42);
            myPan.TabIndex = 9;
            panel5.Controls.Add(myPan);
        }
        int totalSumVar = 0;
        private void totalSum()
        {
            totalSumVar = 0;
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var users = jsonObj.GetValue("users") as JArray;
            foreach (var user in users)
            {
                if (user["condition"].Value<string>() == "true")
                {
                    JArray cart = (JArray)user["order"];
                    foreach (var item in cart)
                    {
                        totalSumVar += Convert.ToInt32(item["price"]) * Convert.ToInt32(item["count"]);
                    }
                }
            }
            label18.Text = totalSumVar.ToString();
        }
        private void renderCartItems(JArray cartArr)
        {
            int height = 22;
            panel5.Controls.Clear();
            if (cartArr.Count != 0)
                foreach (var cartItem in cartArr)
                {
                    renderCart(cartItem["name"].ToString(), height, cartItem["count"].ToString(), cartItem["price"].ToString(), cartItem["productId"].ToString());
                    height += 48;
                }


        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text.Length > 0 && Int32.TryParse(textBox2.Text, out int x)) step = x;
        }
        private void filterByPrice()
        {
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var products = jsonObj.GetValue("products") as JArray;
            JArray priceArr = new JArray();
            foreach (var prod in products)
                if (Int32.Parse(prod["price"].ToString()) >= cur)
                    priceArr.Add(prod);
            renderCards(priceArr);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            JArray productsArr = (JArray)jsonObj["products"];
            JArray sorted = new JArray(productsArr.OrderBy(obj => (string)obj["name"]));
            renderCards(sorted);
        }

        private void Form2_Load(object sender, EventArgs e) { }

        protected void MyClosedHandler(object sender, EventArgs e)
        {
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var users = jsonObj.GetValue("users") as JArray;
            foreach (var user in users)
            {
                user["condition"] = "false";
            }
            jsonObj["users"] = users;
            string output = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObj, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText("database.json", output);

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string toFilter = comboBox1.SelectedItem.ToString();
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var products = jsonObj.GetValue("products") as JArray;
            JArray filteredArr = new JArray();
            JArray empty = new JArray();
            foreach (var prod in products) { if (prod["category"].ToString() == toFilter) filteredArr.Add(prod); }
            if (filteredArr.Count > 0) renderCards(filteredArr);
            if (filteredArr.Count == 0) renderCards(empty);
            if ("не выбрано" == toFilter) renderCards(products);
        }
        private void renderCategory()
        {
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var category = jsonObj.GetValue("category") as JArray;
            comboBox1.Items.Clear();
            foreach (var c in category)
                comboBox1.Items.Add(c);
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            e.Graphics.DrawString("Bruno Buccellati ", new Font("Arial", 25, FontStyle.Bold), Brushes.Black, new Point(10, 10));
            e.Graphics.DrawString("Ваш заказ ", new Font("Arial", 17, FontStyle.Bold), Brushes.Black, new Point(10, 60));
            e.Graphics.DrawString(orders, new Font("Arial", 10, FontStyle.Regular), Brushes.Black, new Point(10, 100));

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var jsonObj = JObject.Parse(File.ReadAllText("database.json"));
            var users = jsonObj.GetValue("users") as JArray;
            foreach (var user in users)//.Where(obj => obj["condition"].Value<string>() == "true"))
            {
                if (user["condition"].Value<string>() == "true")
                {
                    JArray cart = (JArray)user["order"];
                    int i = 1;
                    foreach (var item in cart)
                    {
                        orders += i.ToString() + ")  Название:   " + item["name"].ToString() + "     Цена:   " + item["price"].ToString() +  "  Количество:    " + item["count"].ToString() + "\r\n"; i++; ; 
                                            }
                    orders += "Итого :  "+ totalSumVar.ToString();
                }
            }
            PrintDialog printDialog = new PrintDialog();
            printDialog.Document = printDocument1;
            DialogResult result = printDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                printDocument1.Print();
            }
        }
    }
}
