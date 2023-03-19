using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;
using System.Data;
using Emgu.CV;

using System.Collections.Generic;
using System.ComponentModel;

using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
using System.IO;

using Emgu.CV.Face;
using Emgu.CV.Stitching;
using Emgu.CV.Dnn;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public static List<float[]> ArrayTo2DList(Array array)
        {
            System.Collections.IEnumerator enumerator = array.GetEnumerator();
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            List<float[]> list = new List<float[]>();
            List<float> temp = new List<float>();

            for (int i = 0; i < rows; i++)
            {
                temp.Clear();
                for (int j = 0; j < cols; j++)
                {
                    temp.Add(float.Parse(array.GetValue(i, j).ToString()));
                }
                list.Add(temp.ToArray());
            }

            return list;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            String win1 = "Image"; //The name of the window
            CvInvoke.NamedWindow(win1); //Create the window using the specific name

            Net model = DnnInvoke.ReadNet("1_last_new2.weights", "1.cfg");
           
            List<string> classes = File.ReadAllLines("classes.txt").ToList();
            int imgDefaultSize = 416;
            string fileName = "test3"; // ввод названия фото
            Image<Bgr, byte> img = new Image<Bgr, byte>(fileName + ".jpg").Resize(imgDefaultSize, imgDefaultSize, Inter.Cubic);// для фото  test(1-15)


            var blob = DnnInvoke.BlobFromImage(img, 1 / 255.0, swapRB: true, crop: false);
            model.SetInput(blob);
            VectorOfMat vectorOfMat = new VectorOfMat();
            model.Forward(vectorOfMat, model.UnconnectedOutLayersNames);

            VectorOfRect bboxes = new VectorOfRect();
            VectorOfFloat scores = new VectorOfFloat();
            VectorOfInt indices = new VectorOfInt();
            for (int k = 0; k < vectorOfMat.Size; k++)
            {
                var mat = vectorOfMat[k];
                var data = ArrayTo2DList(mat.GetData());

                for (int i = 0; i < data.Count; i++)
                {
                    var row = data[i];
                    var rowsscores = row.Skip(5).ToArray();
                    var classId = rowsscores.ToList().IndexOf(rowsscores.Max());
                    var confidence = rowsscores[classId];

                    if (confidence > 0.2f)
                    {
                        var center_x = (int)(row[0] * img.Width);
                        var center_y = (int)(row[1] * img.Height);

                        var width = (int)(row[2] * img.Width);
                        var height = (int)(row[3] * img.Height);

                        var x = (int)(center_x - (width / 2));
                        var y = (int)(center_y - (height / 2));

                        bboxes.Push(new Rectangle[] { new Rectangle(x, y, width, height) });
                        indices.Push(new int[] { classId });
                        scores.Push(new float[] { confidence });
                    }
                }
            }
            var idx = DnnInvoke.NMSBoxes(bboxes.ToArray(), scores.ToArray(), 0.1f, 0.3f); // Точность

            var imgOutput = img.Clone();
            for (int i = 0; i < idx.Length; i++)
            {
                int index = idx[i];
                var bbox = bboxes[index];
                imgOutput.Draw(bbox, new Bgr(0, 255, 0), 2);
                CvInvoke.PutText(imgOutput, classes[indices[index]], new Point(bbox.X, bbox.Y + 20),
                    FontFace.HersheySimplex, 0.7, new MCvScalar(0, 0, 255), 1);
            }

            while (CvInvoke.WaitKey(1) == -1)
            {
                CvInvoke.Imshow(win1, imgOutput); //вывод изображения
            }
        }
    }
}
    
