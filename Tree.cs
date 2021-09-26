using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;

namespace Tree
{
    public partial class Form : System.Windows.Forms.Form
    {
        public Form()
        {
            InitializeComponent();
            InitializeTreeView();
        }

        private void InitializeTreeView()
        {
            var connStr = new NpgsqlConnectionStringBuilder
            {
                Host = "localhost",
                Port = 5432,
                Username = "postgres",
                Password = "1211",
                Database = "Pharmacy"
            }.ConnectionString;

            using (var conn = new NpgsqlConnection(connStr))
            {

                conn.Open();

                var command = new NpgsqlCommand
                {
                    Connection = conn,
                    CommandText = @"SELECT drug_category.drug_category as drugCat, active_substance.substance_name as subName, drug.drug_name as drugName
                                    FROM drug_category
                                        LEFT JOIN active_substance ON active_substance.category_id = drug_category.id
                                        LEFT JOIN drug ON drug.substance_id = active_substance.id
                                    ORDER BY drug_category.drug_category, active_substance.substance_name, drug.drug_name"

                };

                using (var reader = command.ExecuteReader())
                {

                    var lastAddedNode = new TreeNode();

                    while (reader.Read())
                    {
                        string drugCat = (string)reader["drugCat"];
                        string subName;
                        string drugName;

                        if (lastAddedNode.Text != drugCat)
                            lastAddedNode = treeView.Nodes.Add(drugCat);
                        try
                        {
                            subName = (string)reader["subName"];
                            if (lastAddedNode.Nodes.Count == 0)
                                lastAddedNode.Nodes.Add(subName);
                            else
                            {
                                if (lastAddedNode.Nodes[lastAddedNode.Nodes.Count - 1].Text != subName)
                                    lastAddedNode.Nodes.Add(subName);
                            }

                            try
                            {
                                drugName = (string)reader["drugName"];
                                lastAddedNode.Nodes[lastAddedNode.Nodes.Count - 1].Nodes.Add(drugName);
                            }
                            catch { }
                           
                        }
                        catch { }
                        
                    }
                
                }
            }
        }
    }
}

