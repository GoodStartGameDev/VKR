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
        bool condition = true;
        
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
        private void ChangeCondition_Click(object sender, EventArgs e)
        {
            condition = !condition;
        }

        private void timer_Tick(object sender, EventArgs e,List<string> detectedObjects,ListBox listBox1)
        {
            if (detectedObjects.Count > 0)
            {
                string className = detectedObjects[0];
                detectedObjects.RemoveAt(0);
                listBox1.Items.Remove(className);
            }
        }
        private bool IsDetected(List<string> detectedObjects, List<string> classes, VectorOfInt indices, ListBox listBox1, int index)
        {
           
            if (detectedObjects.Contains(classes[indices[index]]))
            {
                return true;
            }
            else return false;
             // var rect1 = GetObjectRect(obj1);
             //  var rect2 = GetObjectRect(obj2);
              
             //  var distance = Math.Sqrt(Math.Pow(rect1.X - rect2.X, 2) + Math.Pow(rect1.Y - rect2.Y, 2));
             //  return distance < 10; // если расстояние между центрами меньше 10 пикселей, то объекты считаем одинаковыми
         
         }
        //VectorOfRect bboxes
        private bool IsSameObject(VectorOfRect bboxes1, int index1, VectorOfRect bboxes2, int index2)
        {
            var bbox1 = bboxes1[index1];
            double distance;
            try
            {
                var bbox2 = bboxes2[index2];
                distance = Math.Sqrt(Math.Pow(bbox1.X - bbox2.X, 2) + Math.Pow(bbox1.Y - bbox2.Y, 2));
            }
            catch (Exception ex)
            {
                return false;
            }
            return distance < 100; // если расстояние между центрами меньше 10 пикселей, то объекты считаем одинаковыми
        }
        private Rectangle GetObjectRect(Mat obj)
        {
            VectorOfRect bboxes = new VectorOfRect(); // перенёс
            VectorOfFloat scores = new VectorOfFloat();

            var rects = DnnInvoke.NMSBoxes(bboxes.ToArray(), scores.ToArray(), 0.1f, 0.3f); // Точность
            int index = rects[0];
            var bbox = bboxes[index];
            return bbox;
        }



        /*  private void SendNotification(Mat detectedObject)
        {
            // Реализация метода зависит от конкретной задачи 
            // В данном примере просто выводим информацию об объекте в консоль
            Console.WriteLine($"Обнаружен объект: {detectedObject}");
        }
        */


       
        private string Object_to_delete_from_listbox(string classes, int fr)
        {
                if (fr < 5)
                {
                    return classes;
                }
            return null;
        }

        private void Delete_from_listbox(string classes,ListBox listBox1)
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
            if (classes!=null)
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
                fr[classes[indices[index]]] = 15; // Делаем запас 5 кадров, для потверждения того, что объект остаётся в кадре
                return classes[indices[index]];
            }
            return null;
            }   
    public void Process(PictureBox pictureBox1, string fileName, CheckBox checkBox, ListBox listBox1, Label label3)
    {
        Net model = DnnInvoke.ReadNet("1_last_new2.weights", "1.cfg");
        List<string> classes = File.ReadAllLines("classes.txt").ToList();

        List<string> detectedObjects = new List<string>();  //NEW
        List<string> deleteObjects = new List<string>();
        int imgDefaultSize = 416;
        Mat blob1 = null;

            if (fileName.ToLower().EndsWith(".mp4") || fileName.ToLower().EndsWith(".mov") || fileName.ToLower().EndsWith(".avi") || fileName.ToLower().EndsWith(".wmv") || fileName.ToLower().EndsWith(".mkv") || fileName.ToLower().EndsWith(".flv"))
            { // ВЫНЕСТИ В ОТДЕЛЬНУЮ ФУНКЦИЮ!

                /*
                Timer timer = new Timer();
                timer.Interval = 5000; // интервал в миллисекундах
                timer.Tick += (s, e) => timer_Tick(s, e, detectedObjects, listBox1);
                timer.Start();
                */
                VideoCapture capture = new VideoCapture(fileName);
                Dictionary<string, int> fr = new Dictionary<string, int>();
                checkBox.Visible = true;
                
                for (int i = 0; i < classes.Count(); i++)
                {
                    fr[classes[i]] = 0;         // Счётчик количества кадров, в которых объект считается найденным
                }
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
                        if (fr[classes[i]]>0) fr[classes[i]]--;         // Счётчик количества кадров, в которых объект считается найденным
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
                        Delete_from_listbox(Object_to_delete_from_listbox(classes[i], fr[classes[i]]),listBox1);
                    }

                    // Read the next frame from the video
                    Mat img1 = capture.QueryFrame();
                    Image<Bgr, byte> img = null;
                    try
                    {
                        img = img1.ToImage<Bgr, byte>();
                    }
                    catch (Exception ex)
                    {
                        checkBox.Checked = false;
                        capture.Dispose();
                        Application.DoEvents();
                        pictureBox1.Image = null;
                        checkBox.Visible = false;
                        break;
                    }
                    // If the frame is null, we've reached the end of the video
                    if (img == null)
                    {
                        break;
                    }

                    try
                    {
                        // Create a 4D blob from the frame and set it as the input to the DNN
                        blob1 = DnnInvoke.BlobFromImage(img, 1 / 255.0, new Size(416, 416), new MCvScalar(0, 0, 0), swapRB: true, crop: false);
                    }
                    catch (Exception ex)
                    {
                        checkBox.Checked = false;
                        capture.Dispose();
                        Application.DoEvents();
                        pictureBox1.Image = null;
                        checkBox.Visible = false;
                        break;
                    }


                    // Extract the detected objects from the output
                    
                    model.SetInput(blob1);
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
                    var idx = DnnInvoke.NMSBoxes(bboxes.ToArray(), scores.ToArray(), 0.2f, 0.001f); // Точность

                    VectorOfRect tbboxes = new VectorOfRect();
                    var tidx = idx;

                    var imgOutput = img.Clone();

                    for (int i = 0; i < idx.Length; i++)
                    {
                        int index = idx[i];
                        var bbox = bboxes[index];
                        //imgOutput.ToImage<Bgr, byte>().Draw(bbox, new Bgr(255.0, 255.0, 255.0), 2);
                       // imgOutput.Draw(bbox, new Bgr(255.0, 255.0, 255.0), 2);
                        //CvInvoke.PutText(imgOutput, classes[indices[index]], new System.Drawing.Point(bbox.X, bbox.Y + 20),
                                        //FontFace.HersheySimplex, 0.7, new MCvScalar(0, 0, 255), 1);
                        Application.DoEvents();
                        
                        if (!IsSameObject(bboxes, index, tbboxes, tidx[i]))
                        {
                            imgOutput.Draw(bbox, new Bgr(0, 255, 0), 3);
                        

                            CvInvoke.PutText(imgOutput, classes[indices[index]], new System.Drawing.Point(bbox.X, bbox.Y + 20),
                                    FontFace.HersheySimplex, 0.7, new MCvScalar(0, 0, 255), 1);
                            if (fr[classes[indices[index]]] < 15) fr[classes[indices[index]]]+=2; // Увеличиваем счётчик количества кадров, в которых объект считается найденным
                        }
                        //if (!IsDetected(detectedObjects, classes, indices, listBox1, index))



                        //Output_in_listbox(classes, indices, listBox1, index, fr);
                        for (int j = 0; j < classes.Count(); j++)
                        {
                            Add_to_listbox(Object_to_add_to_listbox(classes, indices, index, fr),listBox1);
                        }

                    }
                    //label3.Text = string.Join(Environment.NewLine, detectedObjects);
                    pictureBox1.Image = imgOutput.ToBitmap();
                    Application.DoEvents();
                    if (!checkBox.Checked)
                    {
                        capture.Dispose();
                        Application.DoEvents();
                        pictureBox1.Image = null;
                        checkBox.Visible = false;
                        break;
                    }
                }

                // Release the VideoCapture object and close the window
                capture.Dispose();
                CvInvoke.DestroyAllWindows();
               
            }
            else
            {
                VectorOfRect bboxes = new VectorOfRect(); // перенёс
                VectorOfFloat scores = new VectorOfFloat();
                VectorOfInt indices = new VectorOfInt();
                Image<Bgr, byte> img = null;
                try
                {
                    img = new Image<Bgr, byte>(fileName).Resize(imgDefaultSize, imgDefaultSize, Inter.Cubic);// для фото  test(1-15)
                }
                catch (Exception ex)
                {
                    return;
                }

                var blob = DnnInvoke.BlobFromImage(img, 1 / 255.0, swapRB: true, crop: false);

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
                    CvInvoke.PutText(imgOutput, classes[indices[index]], new System.Drawing.Point(bbox.X, bbox.Y + 20),
                        FontFace.HersheySimplex, 0.7, new MCvScalar(0, 0, 255), 1);
                }
                imgOutput = imgOutput.Resize(782, 555, Inter.Cubic);
                pictureBox1.Image = imgOutput.ToBitmap();
                
            }
    }
        //Bitmap bitmap = new Bitmap(pictureBox1.Image);
        //pictureBox1.Image = bitmap;

        //Graphics graphics = Graphics.FromImage(bitmap);
        //Mat frame = new Mat();
        //pictureBox1.Image = frame.ToBitmap();
        //capture = new OpenCvSharp.VideoCapture(fileName);
        /*
        if (capture != null)
        {
            OpenCvSharp.Mat frame = new OpenCvSharp.Mat();
            capture.Read(frame);

            pictureBox1.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(frame); ;
            Emgu.CV.BackgroundSubtractorMOG2 backgroundSubtractor = new Emgu.CV.BackgroundSubtractorMOG2();

           // backgroundSubtractor = new Emgu.CV.BgSegm.BackgroundSubtractorGMG(200,20);
            //Application.Idle += Application_Idle;
        }
        */

                    }
                }
