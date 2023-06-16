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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Drawing;

namespace WinFormsApp1
{
    class Processing
    {
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
        public void ShowImport(PictureBox pictureBox1, string fileName)
        {
            int imgDefaultSizeWidth = pictureBox1.Width;
            int imgDefaultSizeHeight = pictureBox1.Height;
            if (!(fileName.ToLower().EndsWith(".mp4") || fileName.ToLower().EndsWith(".mov") || fileName.ToLower().EndsWith(".avi") || fileName.ToLower().EndsWith(".wmv") || fileName.ToLower().EndsWith(".mkv") || fileName.ToLower().EndsWith(".flv")))
            {
                Image<Bgr, byte> img = new Image<Bgr, byte>(fileName).Resize(imgDefaultSizeWidth, imgDefaultSizeHeight, Inter.Cubic);  // SIZE?
                pictureBox1.Image = img.ToBitmap();
            }
        }


        private string Object_to_delete_from_listbox(string classes, int fr)
        {
            if (fr < 5)
            {
                return classes;
            }
            else return null;
        }

        private void Delete_from_listbox(string classes, ListBox listBox1)
        {
            const string filePath = @"C:\Users\Иван\Documents\VKR\OTLADKA.txt"; // путь к файлу
            FileStream fileStream = null;
            byte[] buffer;
            if (classes != null)
            {
                if (listBox1.Items.Contains(classes))
                {
                    fileStream = new FileStream(filePath, FileMode.Append);
                    buffer = System.Text.Encoding.UTF8.GetBytes($"!!Объект {classes} Удалён из списка\n"); // конвертируем строку в байты
                    fileStream.Write(buffer, 0, buffer.Length); // записываем байты в файл
                    fileStream.Close();

                    listBox1.Items.Remove(classes);
                    Application.DoEvents();
                }
            }
        }

        private void Add_to_listbox(string classes, ListBox listBox1)
        {
            const string filePath = @"C:\Users\Иван\Documents\VKR\OTLADKA.txt"; // путь к файлу
            FileStream fileStream = null;
            byte[] buffer;
            if (classes != null)
            {
                if (!listBox1.Items.Contains(classes))
                {
                    listBox1.Items.Add(classes); // нет в списке - добавить

                    fileStream = new FileStream(filePath, FileMode.Append);
                    buffer = System.Text.Encoding.UTF8.GetBytes($"!Объект {classes} Добавлен в список\n"); // конвертируем строку в байты
                    fileStream.Write(buffer, 0, buffer.Length); // записываем байты в файл
                    fileStream.Close();
                }
            }
        }

        private string Object_to_add_to_listbox(List<string> classes, VectorOfInt indices, int index, Dictionary<string, int> fr)
        {
            if (fr[classes[indices[index]]] >= 5)
            {
                fr[classes[indices[index]]] = 15; // Делаем запас 10 кадров, для потверждения того, что объект остаётся в кадре
                return classes[indices[index]];
            }
            else return null;
        }

        private void Video_output(PictureBox pictureBox1, string fileName, CheckBox checkBox, ListBox listBox1, List<string> classes, Net model)
        {
            VideoCapture capture = new VideoCapture(fileName);
            
            Dictionary<string, int> fr = new Dictionary<string, int>();
            for (int i = 0; i < classes.Count(); i++)
            {
                fr[classes[i]] = 0;         // Счётчик количества кадров, в которых объект считается найденным
            }

            while (checkBox.Checked)
            {
               // listBox1.BeginUpdate();
                pictureBox1.Image = Video_processing(fr,capture, fileName, checkBox, listBox1, pictureBox1, classes, model);

                Application.DoEvents();
               // listBox1.EndUpdate();
            }
        }

        class Processing_variables
        {
            public VectorOfRect bboxes;
            public VectorOfFloat scores;
            public VectorOfInt indices;
            public int[] idx;
        }

        private Processing_variables Deep_proccessing(Net model, Image<Bgr, byte> img, VectorOfRect bboxes, VectorOfFloat scores, VectorOfInt indices)
        {
            Size new_size = new Size(416,416);
            var blob = DnnInvoke.BlobFromImage(img, 1 / 255.0,size: new_size, swapRB: true, crop: false);

            model.SetInput(blob);
            VectorOfMat vectorOfMat = new VectorOfMat();
            model.Forward(vectorOfMat, model.UnconnectedOutLayersNames);

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

                    if (confidence > 0.15f)
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

            return new Processing_variables { bboxes = bboxes, scores = scores, indices = indices, idx = idx };
        }

        private Bitmap Video_processing(Dictionary<string, int> fr,VideoCapture capture, string fileName, CheckBox checkBox, ListBox listBox1, PictureBox pictureBox1, List<string> classes, Net model)
        {

            
            checkBox.Visible = true;
            Mat blob1 = null;
           
            // Loop through the frames of the video
            while (checkBox.Checked)
            {
                VectorOfRect bboxes = new VectorOfRect(); // перенёс из глобального объявления функции
                VectorOfFloat scores = new VectorOfFloat();
                VectorOfInt indices = new VectorOfInt();

                const string filePath = @"C:\Users\Иван\Documents\VKR\OTLADKA.txt"; // путь к файлу
                FileStream fileStream = null;

                byte[] buffer;

                for (int i = 0; i < classes.Count(); i++)
                {
                    if (fr[classes[i]] > 0) fr[classes[i]]--;         // Счётчик количества кадров, в которых объект считается найденным
                    fileStream = new FileStream(filePath, FileMode.Append);
                    buffer = System.Text.Encoding.UTF8.GetBytes($"Объект {classes[i]} найден {fr[classes[i]]}-раз\n"); // конвертируем строку в байты
                    fileStream.Write(buffer, 0, buffer.Length); // записываем байты в файл
                    fileStream.Close();
                }
                fileStream = new FileStream(filePath, FileMode.Append);
                buffer = System.Text.Encoding.UTF8.GetBytes("------------------------------------\n"); // конвертируем строку в байты
                fileStream.Write(buffer, 0, buffer.Length); // записываем байты в файл
                fileStream.Close();


                for (int i = 0; i < classes.Count(); i++)
                {
                    Delete_from_listbox(Object_to_delete_from_listbox(classes[i], fr[classes[i]]), listBox1);
                }

                // Read the next frame from the video
                Mat img1 = capture.QueryFrame();
                Image<Bgr, byte> img = null;
                try
                {
                    //img = img1.ToImage<Bgr, byte>().Resize(416,416,Inter.Linear);
                    img = img1.ToImage<Bgr, byte>();
                }
                catch (Exception ex)
                {
                    checkBox.Checked = false;
                    capture.Dispose();
                    Application.DoEvents();

                    checkBox.Visible = false;
                    return null;
                    break;
                }
                // If the frame is null, we've reached the end of the video
                if (img == null)
                {
                    return null;
                    break;
                }

                bboxes = Deep_proccessing(model, img, bboxes, scores, indices).bboxes;
                scores = Deep_proccessing(model, img, bboxes, scores, indices).scores;
                indices = Deep_proccessing(model, img, bboxes, scores, indices).indices;
                var idx = Deep_proccessing(model, img, bboxes, scores, indices).idx;

                var imgOutput = img.Clone();

                for (int i = 0; i < idx.Length; i++)
                {
                    int index = idx[i];
                    var bbox = bboxes[index];

                    Application.DoEvents();


                    imgOutput.Draw(bbox, new Bgr(0, 255, 0), 3);


                    CvInvoke.PutText(imgOutput, classes[indices[index]], new System.Drawing.Point(bbox.X, bbox.Y + 20),
                                FontFace.HersheySimplex, 0.7, new MCvScalar(0, 0, 255), 1);
                    if (fr[classes[indices[index]]] < 15) fr[classes[indices[index]]] += 2; // Увеличиваем счётчик количества кадров, в которых объект считается найденным

                    for (int j = 0; j < classes.Count(); j++)
                    {
                        Add_to_listbox(Object_to_add_to_listbox(classes, indices, index, fr), listBox1);
                    }

                }

               
                if (!checkBox.Checked)
                {
                    capture.Dispose();
                    Application.DoEvents();
                    pictureBox1.Image = null;
                    checkBox.Visible = false;
                    return null;
                    break;
                }
                Application.DoEvents();
                return imgOutput.ToBitmap();
            }

            // Release the VideoCapture object and close the window
            capture.Dispose();
            CvInvoke.DestroyAllWindows();
            return null;

        }

      
        private Bitmap Image_processing(Net model, List<string> classes, string fileName)
        {
            int imgDefaultSize = 416;
            VectorOfRect bboxes = new VectorOfRect(); // перенёс
            VectorOfFloat scores = new VectorOfFloat();
            VectorOfInt indices = new VectorOfInt();
            Image<Bgr, byte> img = null;
            try
            {
                //img = new Image<Bgr, byte>(fileName).Resize(imgDefaultSize, imgDefaultSize, Inter.Cubic);// для фото  test(1-15)
                img = new Image<Bgr, byte>(fileName);
            }
            catch (Exception ex)
            {
                return null;
            }

            bboxes = Deep_proccessing(model, img, bboxes, scores, indices).bboxes;
            scores = Deep_proccessing(model, img, bboxes, scores, indices).scores;
            indices = Deep_proccessing(model, img, bboxes, scores, indices).indices;
            var idx = Deep_proccessing(model, img, bboxes, scores, indices).idx;

            var imgOutput = img.Clone();

            for (int i = 0; i < idx.Length; i++)
            {
                int index = idx[i];
                var bbox = bboxes[index];
                imgOutput.Draw(bbox, new Bgr(0, 255, 0), 2);
                CvInvoke.PutText(imgOutput, classes[indices[index]], new System.Drawing.Point(bbox.X, bbox.Y + 20),
                    FontFace.HersheySimplex, 0.7, new MCvScalar(0, 0, 255), 1);
            }
            imgOutput = imgOutput.Resize(782, 555, Inter.Cubic);
            return imgOutput.ToBitmap();
        }

        private byte Check_file_type(string fileName)
        {
            if (fileName.ToLower().EndsWith(".mp4") || fileName.ToLower().EndsWith(".mov") || fileName.ToLower().EndsWith(".avi") || fileName.ToLower().EndsWith(".wmv") || fileName.ToLower().EndsWith(".mkv") || fileName.ToLower().EndsWith(".flv"))
                return 2;
            if (fileName.ToLower().EndsWith(".jpeg") || fileName.ToLower().EndsWith(".jpg") || fileName.ToLower().EndsWith(".png") || fileName.ToLower().EndsWith(".gif") || fileName.ToLower().EndsWith(".bmp") || fileName.ToLower().EndsWith(".tiff") || fileName.ToLower().EndsWith(".psd") || fileName.ToLower().EndsWith(".raw") || fileName.ToLower().EndsWith(".cr2"))
                return 1;
            else return 0;
        }
    public void Process(PictureBox pictureBox1, string fileName, CheckBox checkBox, ListBox listBox1, Label label3)
    {
            const string cfg = "1.cfg";
            const string weights = "1_last_new2.weights";
            Net model = DnnInvoke.ReadNet(weights, cfg);
            List<string> classes = File.ReadAllLines("classes.txt").ToList();

        //List<string> detectedObjects = new List<string>();  //NEW
        //List<string> deleteObjects = new List<string>();
           // int imgDefaultSize = 416;
           // Mat blob1 = null;

            if (Check_file_type(fileName) == 2)
            {
                Video_output(pictureBox1,fileName,checkBox,listBox1,classes,model);
            }
            if (Check_file_type(fileName) == 1)
            {
                pictureBox1.Image = Image_processing(model, classes, fileName);
            }
    }
       
    }
}
