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

namespace WinFormsApp1
{
    public partial class YOLO : Form
    {
        string fileName;
        Processing processing_class = new Processing();
        Import importFile = new Import();
        Net model = new Net();
        List<string> classes = new List<string>();

        public YOLO()
        {
            InitializeComponent();
        }

        public void btn_Processing_Click(object sender, EventArgs e)
        {
            processing_class.Process(model, classes, pictureBox, fileName, checkBox_proccessing, listBox_found_objects);
        }

        private void btn_import_Click(object sender, EventArgs e)
        {
            fileName = importFile.Import_file();
            label_file_imported.Text = "Импортирован файл: " + fileName;
        }

        private void btn_cloud_import_Click(object sender, EventArgs e)
        {
            importFile.Cloud_import(textBox_cloud_import ,label_download_from_cloud);
        }

        private void button_save_processed_image_Click(object sender, EventArgs e)
        {
            processing_class.Save_Processed_Image(pictureBox.Image);
        }

        private void YOLO_Load(object sender, EventArgs e)
        {
            processing_class.NetInit();
        }

        private void YOLO_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
    
