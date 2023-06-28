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
    public enum FileType : byte
    {
        FILE_ERROR = 0,
        FILE_PHOTO = 1,
        FILE_VIDEO = 2
    }
    public class Processing
    {
        public Net model;
        public List<string> classes;
        public List<string> detected_objects;
        public string Translate_to_rus(string name)
        {
            if (name == "pothole") return "яма";
            if (name == "sign") return "знак пешеходного перехода";
            if (name == "tram_sign") return "знак трамвайной остановки";
            if (name == "bus_sign") return "знак автобусной остановки";
            if (name == "crosswalk") return "пешeходный переход";
            if (name == "red") return "красный цвет светофора";
            if (name == "green") return "зелёный цвет светофора";
            if (name == "down") return "знак подземного перехода";
            if (name == "up") return "знак надземного перехода";
            if (name == "metro") return "метро";

            return null;
        }

        public string Translate_to_eng(string name)
        {
            if (name == "яма") return "pothole";
            if (name == "знак пешеходного перехода") return "sign";
            if (name == "знак трамвайной остановки") return "tram_sign";
            if (name == "знак автобусной остановки") return "bus_sign";
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
            if (classes != null)
            {
                if (listBox_found_objects.Items.Contains(Translate_to_rus(classes)))
                {
                    listBox_found_objects.Items.Remove(Translate_to_rus(classes));
                    Application.DoEvents();
                }
            }
        }

        private void Add_to_listbox(string classes, ListBox listBox_found_objects)
        {
            if (classes != null)
            {
                if (!listBox_found_objects.Items.Contains(classes))
                {
                    listBox_found_objects.Items.Add(classes); // нет в списке - добавить
                    SoundMessage(classes);
                }
            }
        }

        private string Object_to_add_to_listbox(List<string> classes, VectorOfInt indices, int index, Dictionary<string, int> fr)
        {
            if (fr[classes[indices[index]]] >= 5)
            {
                fr[classes[indices[index]]] = 15; 
                return classes[indices[index]];
            }
            else return null;
        }

        private void Video_output(PictureBox pictureBox1, string fileName, CheckBox checkBox, ListBox listBox_found_objects, List<string> classes, Net model)
        {
            VideoCapture capture = new VideoCapture(fileName);
            TypedReference p_capture = __makeref(capture);
            Dictionary<string, int> fr = new Dictionary<string, int>();

            TypedReference p_listBox_found_objects = __makeref(listBox_found_objects);
            checkBox.Checked = true;
            for (int i = 0; i < classes.Count(); i++)
            {
                fr[classes[i]] = 0;
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

        public int[] Deep_proccessing(Net model, TypedReference img, TypedReference bboxes, TypedReference scores, TypedReference indices)
        {
            Size new_size = new Size(416, 416);
            var blob = DnnInvoke.BlobFromImage(__refvalue(img, Image<Bgr, byte>), 1 / 255.0, size: new_size, swapRB: true, crop: false);

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
            var idx = DnnInvoke.NMSBoxes(__refvalue(bboxes, VectorOfRect).ToArray(), __refvalue(scores, VectorOfFloat).ToArray(), 0.1f, 0.3f); 

            return idx;
        }

        unsafe private Bitmap Video_processing(Dictionary<string, int> fr, TypedReference capture, string fileName, TypedReference listBox_found_objects, List<string> classes, Net model)
        {
            VectorOfRect bboxes = new VectorOfRect();
            VectorOfFloat scores = new VectorOfFloat();
            VectorOfInt indices = new VectorOfInt();

            TypedReference p_bboxes = __makeref(bboxes);
            TypedReference p_scores = __makeref(scores);
            TypedReference p_indices = __makeref(indices);

            for (int i = 0; i < classes.Count(); i++)
            {
                if (fr[classes[i]] > 0) fr[classes[i]]--; 
            }
            for (int i = 0; i < classes.Count(); i++)
            {
                Delete_from_listbox(Object_to_delete_from_listbox(classes[i], fr[classes[i]]), __refvalue(listBox_found_objects, ListBox));
            }

            Mat img1 = __refvalue(capture, VideoCapture).QueryFrame();
            if (img1 == null)
            {
                return null;
            }
            Image<Bgr, byte> img = img1.ToImage<Bgr, byte>();

            TypedReference p_img = __makeref(img);

            int[] idx = Deep_proccessing(model, p_img, p_bboxes, p_scores, p_indices);

            for (int i = 0; i < idx.Length; i++)
            {
                int index = idx[i];
                var bbox = bboxes[index];

                img.Draw(bbox, new Bgr(0, 255, 0), 3);
                CvInvoke.PutText(img, classes[indices[index]], new System.Drawing.Point(bbox.X, bbox.Y + 20),
                            FontFace.HersheySimplex, 0.7, new MCvScalar(0, 0, 255), 1);

                if (fr[classes[indices[index]]] < 15) fr[classes[indices[index]]] += 2; 

                for (int j = 0; j < classes.Count(); j++)
                {
                    Add_to_listbox(Translate_to_rus(Object_to_add_to_listbox(classes, indices, index, fr)), __refvalue(listBox_found_objects, ListBox));
                }

            }
            return img.ToBitmap();
        }

        public Bitmap Image_processing(Net model, List<string> classes, string fileName, TypedReference p_detected_objects)
        {
            VectorOfRect bboxes = new VectorOfRect(); 
            VectorOfFloat scores = new VectorOfFloat();
            VectorOfInt indices = new VectorOfInt();
            Image<Bgr, byte> img = null;
            try
            {
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

        public FileType Check_file_type(string fileName)
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
            SpeechSynthesizer bot = new SpeechSynthesizer();
            if (text != null) bot.SpeakAsync(text);
        }

        public void NetInit()
        {
            const string cfg = "C:\\VKR\\WinFormsApp1\\WinFormsApp1\\bin\\Debug\\net5.0-windows\\1.cfg";
            const string weights = "C:\\VKR\\WinFormsApp1\\WinFormsApp1\\bin\\Debug\\net5.0-windows\\1_last_new2.weights";
            this.model = DnnInvoke.ReadNet(weights, cfg);
            this.classes = File.ReadAllLines("C:\\VKR\\WinFormsApp1\\WinFormsApp1\\bin\\Debug\\net5.0-windows\\classes.txt").ToList();
            this.detected_objects = new List<string>();
        }
  
        public void Process(Net model, List<string> classes, PictureBox pictureBox1, string fileName, CheckBox checkBox, ListBox listBox_found_objects)
        {
            checkBox.Checked = false; Application.DoEvents();
            List<string> detected_objects = new List<string>();
            TypedReference p_detected_objects = __makeref(detected_objects);
            if (Check_file_type(fileName) == FileType.FILE_VIDEO)
            {
                Video_output(pictureBox1, fileName, checkBox, listBox_found_objects, this.classes, this.model);
            }
            if (Check_file_type(fileName) == FileType.FILE_PHOTO)
            {
                listBox_found_objects.Items.Clear();
                pictureBox1.Image = Image_processing(this.model, this.classes, fileName, p_detected_objects);
                Application.DoEvents();
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
