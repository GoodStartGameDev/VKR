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
using System.Media;

using System.Speech.Synthesis;

namespace WinFormsApp1
{
    public enum FileType: byte
    {
        FILE_ERROR = 0,
        FILE_PHOTO = 1,
        FILE_VIDEO = 2
    }
    class Processing
    {
        private string Translate_to_rus(string name)
        {
            if (name == "pothole") return "яма";
            if (name == "sign") return "знак пешeходного перехода";
            if (name == "tram_sign") return "знак трамвая";
            if (name == "bus_sign") return "знак автобуса";
            if (name == "crosswalk") return "пешeходный переход";
            if (name == "red") return "красный цвет светофора";
            if (name == "green") return "зелёный цвет светофора";
            if (name == "down") return "знак подземного перехода";
            if (name == "up") return "знак надземного перехода";
            if (name == "metro") return "метро";

            return null;
        }

        private string Translate_to_eng(string name)
        {
            if (name == "яма") return "pothole";
            if (name == "знак пешeходного перехода") return "sign";
            if (name == "знак трамвая") return "tram_sign";
            if (name == "знак автобуса") return "bus_sign";
            if (name == "пешeходный переход") return "crosswalk";
            if (name == "красный цвет светофора") return "red";
            if (name == "зелёный цвет светофора") return "green";
            if (name == "знак подземного перехода") return "down";
            if (name == "знак надземного перехода") return "up";
            if (name == "метро") return "metro";

            return null;
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

        private void Delete_from_listbox(string classes, ListBox listBox_found_objects)
        {
            const string filePath = @"C:\Users\Иван\Documents\VKR\OTLADKA.txt"; // путь к файлу
            FileStream fileStream = null;
            byte[] buffer;
            if (classes != null)
            {
                if (listBox_found_objects.Items.Contains(Translate_to_rus(classes)))
                {
                    fileStream = new FileStream(filePath, FileMode.Append);
                    buffer = System.Text.Encoding.UTF8.GetBytes($"!!Объект {classes} Удалён из списка\n"); // конвертируем строку в байты
                    fileStream.Write(buffer, 0, buffer.Length); // записываем байты в файл
                    fileStream.Close();

                    listBox_found_objects.Items.Remove(Translate_to_rus(classes));
                    Application.DoEvents();
                }
            }
        }

        private void Add_to_listbox(string classes, ListBox listBox_found_objects)
        {
            const string filePath = @"C:\Users\Иван\Documents\VKR\OTLADKA.txt"; // путь к файлу
            FileStream fileStream = null;
            byte[] buffer;
            if (classes != null)
            {
                if (!listBox_found_objects.Items.Contains(classes))
                {
                    listBox_found_objects.Items.Add(classes); // нет в списке - добавить
                    SoundMessage(classes);

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

        private void Video_output(PictureBox pictureBox1, string fileName, CheckBox checkBox, ListBox listBox_found_objects, List<string> classes, TypedReference model)
        {
            VideoCapture capture = new VideoCapture(fileName);
            TypedReference p_capture = __makeref(capture);
            Dictionary<string, int> fr = new Dictionary<string, int>();

            TypedReference p_listBox_found_objects = __makeref(listBox_found_objects);
            checkBox.Checked = true;
            for (int i = 0; i < classes.Count(); i++)
            {
                fr[classes[i]] = 0;         // Счётчик количества кадров, в которых объект считается найденным
            }

            while (checkBox.Checked)
            {
                checkBox.Visible = true;
                pictureBox1.Image = Video_processing(fr, p_capture, fileName, p_listBox_found_objects, classes, model);
                Application.DoEvents();
            }
            if (!checkBox.Checked)
            {
                pictureBox1.Image = null;
                checkBox.Visible = false;
            }
        }
       
        unsafe private int[] Deep_proccessing(TypedReference model, TypedReference img, TypedReference bboxes, TypedReference scores, TypedReference indices)
        {
            Size new_size = new Size(416,416);
            var blob = DnnInvoke.BlobFromImage(__refvalue(img, Image<Bgr, byte>), 1 / 255.0,size: new_size, swapRB: true, crop: false);

            __refvalue(model, Net).SetInput(blob);
            VectorOfMat vectorOfMat = new VectorOfMat();
            __refvalue(model, Net).Forward(vectorOfMat, __refvalue(model, Net).UnconnectedOutLayersNames);

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
                        var center_x = (int)(row[0] * __refvalue(img, Image<Bgr, byte>).Width);
                        var center_y = (int)(row[1] * __refvalue(img, Image<Bgr, byte>).Height);

                        var width = (int)(row[2] * __refvalue(img, Image<Bgr, byte>).Width);
                        var height = (int)(row[3] * __refvalue(img, Image<Bgr, byte>).Height);

                        var x = (int)(center_x - (width / 2));
                        var y = (int)(center_y - (height / 2));

                        __refvalue(bboxes, VectorOfRect).Push(new Rectangle[] { new Rectangle(x, y, width, height) });
                        __refvalue(indices, VectorOfInt).Push(new int[] { classId });
                        __refvalue(scores, VectorOfFloat).Push(new float[] { confidence });
                    }
                }
            }
            var idx = DnnInvoke.NMSBoxes(__refvalue(bboxes, VectorOfRect).ToArray(), __refvalue(scores, VectorOfFloat).ToArray(), 0.1f, 0.3f); // Точность

            return idx;
            //return new Processing_variables { bboxes = __refvalue(bboxes, VectorOfRect), scores = __refvalue(scores, VectorOfFloat), indices = __refvalue(indices, VectorOfInt), idx = idx };
        }

        unsafe private Bitmap Video_processing(Dictionary<string, int> fr, TypedReference capture, string fileName, TypedReference listBox_found_objects, List<string> classes, TypedReference model)
        {

            
            //checkBox.Visible = true;
            //Mat blob1 = null;
           
            // Loop through the frames of the video
            //while (checkBox.Checked)
            //{
                VectorOfRect bboxes = new VectorOfRect(); // перенёс из глобального объявления функции
                VectorOfFloat scores = new VectorOfFloat();
                VectorOfInt indices = new VectorOfInt();
                

                TypedReference p_bboxes = __makeref(bboxes);
                TypedReference p_scores = __makeref(scores);
                TypedReference p_indices = __makeref(indices);

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
                    Delete_from_listbox(Object_to_delete_from_listbox(classes[i], fr[classes[i]]), __refvalue(listBox_found_objects, ListBox));
                }

                // Read the next frame from the video
                Mat img1 = __refvalue(capture, VideoCapture).QueryFrame();
                if (img1 == null)
                {
                    return null;
                    //break;
                }
                Image<Bgr, byte> img = img1.ToImage<Bgr, byte>();
               
                TypedReference p_img=__makeref(img);

                int[] idx = Deep_proccessing(model, p_img, p_bboxes, p_scores, p_indices);

                //var imgOutput = img.Clone();

                for (int i = 0; i < idx.Length; i++)
                {
                    int index = idx[i];
                    var bbox = bboxes[index];

                    //Application.DoEvents();


                    img.Draw(bbox, new Bgr(0, 255, 0), 3);


                    CvInvoke.PutText(img, classes[indices[index]], new System.Drawing.Point(bbox.X, bbox.Y + 20),
                                FontFace.HersheySimplex, 0.7, new MCvScalar(0, 0, 255), 1);
                    if (fr[classes[indices[index]]] < 15) fr[classes[indices[index]]] += 2; // Увеличиваем счётчик количества кадров, в которых объект считается найденным

                    for (int j = 0; j < classes.Count(); j++)
                    {
                        Add_to_listbox(Translate_to_rus(Object_to_add_to_listbox(classes, indices, index, fr)), __refvalue(listBox_found_objects, ListBox));
                    }

                }

                /*if (!checkBox.Checked)
                {
                    __refvalue(capture, VideoCapture).Dispose();
                    Application.DoEvents();
                    //pictureBox1.Image = null;
                    checkBox.Visible = false;
                    return null;
                    //break;
                }
                */
                //Application.DoEvents();
                return img.ToBitmap();
            //}

            // Release the VideoCapture object and close the window
            //__refvalue(capture, VideoCapture).Dispose();
            //CvInvoke.DestroyAllWindows();
            //return null;

        }

      
        private Bitmap Image_processing(TypedReference model, List<string> classes, string fileName, TypedReference p_detected_objects)
        {
            //int imgDefaultSize = 416;
            
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
            
            TypedReference p_img = __makeref(img);
            TypedReference p_bboxes = __makeref(bboxes);
            TypedReference p_scores = __makeref(scores);
            TypedReference p_indices = __makeref(indices);
            
            int[] idx = Deep_proccessing(model, p_img, p_bboxes, p_scores, p_indices);
            var imgOutput = img.Clone();
            
            for (int i = 0; i < idx.Length; i++)
            {
                int index = idx[i];
                var bbox = bboxes[index];
                imgOutput.Draw(bbox, new Bgr(0, 255, 0), 3);
                CvInvoke.PutText(imgOutput, classes[indices[index]], new System.Drawing.Point(bbox.X, bbox.Y + 20),
                    FontFace.HersheySimplex, 0.7, new MCvScalar(0, 0, 255), 1);
                __refvalue(p_detected_objects, List<string>).Add(Translate_to_rus(classes[indices[index]]));
            }
            imgOutput = imgOutput.Resize(782, 555, Inter.Cubic);
            return imgOutput.ToBitmap();
        }

        private FileType Check_file_type(string fileName)
        {
            if (fileName == null) return FileType.FILE_ERROR;
            if (fileName.ToLower().EndsWith(".mp4") ||
                fileName.ToLower().EndsWith(".mov") ||
                fileName.ToLower().EndsWith(".avi") ||
                fileName.ToLower().EndsWith(".wmv") ||
                fileName.ToLower().EndsWith(".mkv") ||
                fileName.ToLower().EndsWith(".flv"))
                return FileType.FILE_VIDEO;
            if (fileName.ToLower().EndsWith(".jpeg") ||
                fileName.ToLower().EndsWith(".jpg") ||
                fileName.ToLower().EndsWith(".png") ||
                fileName.ToLower().EndsWith(".gif") ||
                fileName.ToLower().EndsWith(".bmp") ||
                fileName.ToLower().EndsWith(".tiff") ||
                fileName.ToLower().EndsWith(".psd") ||
                fileName.ToLower().EndsWith(".raw") ||
                fileName.ToLower().EndsWith(".cr2"))
                return FileType.FILE_PHOTO;
            else return FileType.FILE_ERROR;
        }

    public void Save_Processed_Image(Image img)
        {
            if (img != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "JPEG Image|*.jpg|PNG Image|*.png|BMP Image|*.bmp";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    img.Save(saveFileDialog.FileName);
                }
            }
        }

        private void SoundMessage(string text)
        {
            //SoundPlayer soundPlayer= new SoundPlayer ("1.wav");
            //soundPlayer.Play();
            SpeechSynthesizer bot = new SpeechSynthesizer();
           // bot.Speak("Обнаружено:");
            if (text!=null) bot.Speak(text);
        }
        public void Process(PictureBox pictureBox1, string fileName, CheckBox checkBox, ListBox listBox_found_objects)
        {
            checkBox.Checked = false; Application.DoEvents();
            const string cfg = "1.cfg";
            const string weights = "1_last_new2.weights";
            Net model = DnnInvoke.ReadNet(weights, cfg);
            TypedReference p_model = __makeref(model);
            List<string> classes = File.ReadAllLines("classes.txt").ToList();

            List<string> detected_objects = new List<string>();
            TypedReference p_detected_objects = __makeref(detected_objects);
            

            if (Check_file_type(fileName) == FileType.FILE_VIDEO)
            {
                Video_output(pictureBox1,fileName,checkBox,listBox_found_objects,classes, p_model);
            }
            if (Check_file_type(fileName) == FileType.FILE_PHOTO)
            {
                listBox_found_objects.Items.Clear();
                pictureBox1.Image = Image_processing(p_model, classes, fileName, p_detected_objects);
                listBox_found_objects.Items.AddRange(detected_objects.ToArray());
                for (int i = 0; i < detected_objects.ToArray().Length; i++)
                {
                    SoundMessage(detected_objects.ToArray()[i]);
                }
            }
            if (Check_file_type(fileName) == FileType.FILE_ERROR)
            {
                MessageBox.Show("ОШИБКА: Неверный тип файла");
            }
        }
       
    }
}
