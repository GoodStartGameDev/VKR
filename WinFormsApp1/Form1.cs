using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using Emgu.CV;
using Emgu.CV.Features2D;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.Flann;
using Emgu.CV.Cuda;
using System.Runtime.InteropServices;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.ML;
using Emgu.CV.ML.MlEnum;
using Emgu.CV.Face;
using Emgu.CV.Stitching;
using Emgu.CV.Dnn;

using YandexDiskNET;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        string fileName;
        Processing processing_class = new Processing(); // IS IT OKAY?
        Import importFile = new Import();

        // bool IsRunning = true;

        public Form1()
        {
            InitializeComponent();
        }

        public void button1_Click(object sender, EventArgs e)
        {

            processing_class.Process(pictureBox1, fileName, checkBox1);
            label2.Text = "YOLO DONE!";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            fileName = importFile.Import_file();
            //processing_class.ShowImport(pictureBox1, fileName);
            label1.Text = "Imported: " + fileName;
            label2.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // DownloadFile downloadFile = new DownloadFile();
            //DownloadFile.DriveDownloadFile("https://drive.google.com/file/d/1v8brGFHvLidP-0WiTqyPadY--BrYKHuJ/view?usp=share_link");
            importFile.buttonTest_Click(textBox1, progressBar1);
        }
    }
}
    
