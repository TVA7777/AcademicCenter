﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using AcademicCenter;

namespace AcademicCenter.Forms
{
    public partial class FormDocView : Form
    {
        public FormDocView(List<Document> doc, string typeDoc)
        {
            InitializeComponent();
            Text += typeDoc;
            listBox1.SuspendLayout();
            foreach(Document d in doc)
            {
                if (doc != null && !string.IsNullOrEmpty(d.Path))
                    listBox1.Items.Add(new Item(){Document = d}); 
            }
            listBox1.PerformLayout();

        }

        private void FormDocView_Load(object sender, EventArgs e)
        {

        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            if (e.Index < 0) return;
            Item i = (Item)listBox1.Items[e.Index];
            if(i.Icon!=null)
                e.Graphics.DrawIcon(i.Icon, new Rectangle(e.Bounds.X + 3, e.Bounds.Y + 3, 34, 34));
            e.Graphics.DrawString(i.Document.Name, listBox1.Font, new SolidBrush(listBox1.ForeColor),
                new RectangleF(e.Bounds.X + 50, e.Bounds.Y, e.Bounds.Width - 50, e.Bounds.Height),
                new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new FormName().ShowDialog();
            if (FormName.Value == "") return;
            openFileDialog1.InitialDirectory =
                Path.GetDirectoryName(Application.StartupPath+@"\dat\");
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            if (Path.GetDirectoryName(openFileDialog1.FileName) != Application.StartupPath + @"\dat\")
            {
                File.Copy(openFileDialog1.FileName,
                    Application.StartupPath + @"\dat\"+ Path.GetFileName(openFileDialog1.FileName));
            }
            openFileDialog1.FileName = Path.GetFileName(openFileDialog1.FileName);
            Document d = new Document
            {
                Name = FormName.Value,
                Path = openFileDialog1.FileName
            };
            Configuration.Disciplines[0].Docs.Add(d);
            listBox1.Items.Add(new Item(d));
            Configuration.Disciplines[0].Save(JsonSettings.FolderPath + "\\discipline\\default.json");
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0) return;
            Item i = (Item)listBox1.Items[listBox1.SelectedIndex];
            if(i.Document.Path.StartsWith(Application.StartupPath + @"\dat\"))
                Process.Start(i.Document.Path);
            else
                Process.Start(Application.StartupPath + @"\dat\" + i.Document.Path);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
            {
                MessageBox.Show(@"Документ не выделен");
                return;
            }
            Document d = ((Item)listBox1.Items[listBox1.SelectedIndex]).Document;
            
            if (d == null)
            {
                MessageBox.Show(@"Нет подходящнго документа");
                return;
            }

            if (MessageBox.Show($"Удалить '{d.Name}' документ?", @"Внимание",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            Configuration.Disciplines[0].Docs.Remove(d);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            Configuration.Disciplines[0].Save(JsonSettings.FolderPath + "\\discipline\\default.json");
        }
    }
    

    public struct Item
    {
        public Icon Icon;
        public Document Document;

        public Item(Document doc)
        {
            Document = doc;
            Icon = Icon.ExtractAssociatedIcon(doc?.Path);
        }
    }
}
