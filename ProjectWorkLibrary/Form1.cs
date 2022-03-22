using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace ProjectWorkLibrary
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<User> users;
        Dictionary<string, User> usersDict = new Dictionary<string, User>();
       
        private void Form1_Load(object sender, EventArgs e)
        {
            users = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("users.json"));
            foreach(var user in users)
            {
                usersDict.Add(user.CF.ToString(), user);
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if(usersDict.ContainsKey(tbxUsername.Text) && usersDict[tbxUsername.Text.ToUpper()].Password == tbxPw.Text)
            {
                if(usersDict[tbxUsername.Text.ToUpper()].Role == "admin")
                {
                    
                }
            }
            
        }
    }
}
