using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System;
using System.Diagnostics;
using WinFormsApp1;
using Emgu.CV;
using System.Windows.Forms;
using Emgu.CV.Dnn;
using Emgu.CV.Models;
using System.Drawing;
using System.Collections.Generic;
using System.Web.Helpers;
using System.Linq;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;

namespace WinFormsAppTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestFileType()
        {
            var mc = new Processing();
            FileType res;
            string path = "C:\\VKR\\WinFormsApp1\\test_database\\Check_file_type\\";

            Console.Write($"Process file: {".BmP"} - "); 
            res = mc.Check_file_type(path+ ".BmP");
            Assert.AreEqual(FileType.FILE_PHOTO, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"test1.jpg"} - ");
            res = mc.Check_file_type(path + "test1.jpg");
            Assert.AreEqual(FileType.FILE_PHOTO, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"test2.png"} - ");
            res = mc.Check_file_type(path + "test2.png");
            Assert.AreEqual(FileType.FILE_PHOTO, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"test3.jPg"} - ");
            res = mc.Check_file_type(path + "test3.jPg");
            Assert.AreEqual(FileType.FILE_PHOTO, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"test4.bmp"} - ");
            res = mc.Check_file_type(path + "test4.bmp");
            Assert.AreEqual(FileType.FILE_PHOTO, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"test5.png.JPG.bmp"} - ");
            res = mc.Check_file_type(path + "test5.png.JPG.bmp");
            Assert.AreEqual(FileType.FILE_PHOTO, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"1.mp4"} - ");
            res = mc.Check_file_type(path + "1.mp4");
            Assert.AreEqual(FileType.FILE_VIDEO, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"2.Mov"} - ");
            res = mc.Check_file_type(path + "2.Mov");
            Assert.AreEqual(FileType.FILE_VIDEO, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"3.AVI"} - ");
            res = mc.Check_file_type(path + "3.AVI");
            Assert.AreEqual(FileType.FILE_VIDEO, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"4.aVi.mkv"} - ");
            res = mc.Check_file_type(path + "4.aVi.mkv");
            Assert.AreEqual(FileType.FILE_VIDEO, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"5.FlV"} - ");
            res = mc.Check_file_type(path + "5.FlV");
            Assert.AreEqual(FileType.FILE_VIDEO, res);
            Console.Write($"{res}\n");
            //
            Console.Write($"Process file: {".png....script.as"} - ");
            res = mc.Check_file_type(path + ".png....script.as");
            Assert.AreEqual(FileType.FILE_ERROR, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"~.png.rar"} - ");
            res = mc.Check_file_type(path + "~.png.rar");
            Assert.AreEqual(FileType.FILE_ERROR, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"w.docx"} - ");
            res = mc.Check_file_type(path + "w.docx");
            Assert.AreEqual(FileType.FILE_ERROR, res);
            Console.Write($"{res}\n");

            Console.Write($"Process file: {"й.txt"} - ");
            res = mc.Check_file_type(path + "й.txt");
            Assert.AreEqual(FileType.FILE_ERROR, res);
            Console.Write($"{res}\n");
        }
        [TestMethod]
        public unsafe void TestImageProcessing()
        {
            var mc = new Processing();
            mc.NetInit();
            for (int k = 1; k < mc.classes.Count; k++)
            {
                string classname = mc.classes[k];
                string classnameRUS = mc.Translate_to_rus(classname);

                string path = "C:\\VKR\\WinFormsApp1\\test_database\\new\\" + classname + "\\";
                string fileName = "";
                string openPath = "";
                string savePath = "";

                float test_num = Directory.GetFiles(path)
                            .Where(file => !file.Contains("save"))
                            .Count();

                float err = 0;

                for (int t = 1; t < test_num; t++)
                {
                    List<string> detected_objects = new List<string>();
                    TypedReference p_detected_objects = __makeref(detected_objects);
                    fileName = classname + t.ToString() + ".jpg";
                    openPath = path + fileName;
                    savePath = path + "save_" + fileName;
                    Bitmap img = mc.Image_processing(mc.model, mc.classes, openPath, p_detected_objects);
                    if (img != null) img.Save(savePath);
                    Console.Write($"Process file: {fileName}\n");
                    Console.Write($"\tNum objects: { __refvalue(p_detected_objects, List<string>).ToArray().Length}\n");
                    if (__refvalue(p_detected_objects, List<string>).ToArray().Length == 0) err++;
                    else
                    {
                        for (int i = 0; i < __refvalue(p_detected_objects, List<string>).ToArray().Length; i++)
                        {
                            Console.Write($"\tClass[{i}]: { __refvalue(p_detected_objects, List<string>).ToArray()[i]}\n");
                            if (__refvalue(p_detected_objects, List<string>).ToArray()[i] != classnameRUS) err++;
                        }
                    }

                }
                float conf = 1 - (err / test_num);
                Console.Write(classnameRUS + $" tests:{test_num}\n");
                Console.Write($"Success: {test_num - err}\n");
                Console.Write($"Not found: {err}\n");
                Console.Write($"Confidence: {conf}\n");
            }
        }
        [TestMethod]
        public unsafe void TestImageProcessing2obj()
        {
            var mc = new Processing();
            mc.NetInit();

            string classname = "test";
            string path = "C:\\VKR\\WinFormsApp1\\test_database\\no_objs\\";
            string fileName = "";
            string openPath = "";
            string savePath = "";

            float test_num = Directory.GetFiles(path)
                        .Where(file => !file.Contains("save"))
                        .Count();

            float err = 0;

            for (int t = 1; t <= test_num; t++)
            {
                List<string> detected_objects = new List<string>();
                TypedReference p_detected_objects = __makeref(detected_objects);
                fileName = classname + t.ToString() + ".jpg";
                openPath = path + fileName;
                savePath = path + "save_" + fileName;
                Bitmap img = mc.Image_processing(mc.model, mc.classes, openPath, p_detected_objects);
                if (img != null) img.Save(savePath);
                Console.Write($"Process file: {fileName}\n");
                Console.Write($"\tNum objects: { __refvalue(p_detected_objects, List<string>).ToArray().Length}\n");
                if (__refvalue(p_detected_objects, List<string>).ToArray().Length == 0) err++;
                else
                {
                    for (int i = 0; i < __refvalue(p_detected_objects, List<string>).ToArray().Length; i++)
                    {
                        Console.Write($"\tClass[{i}]: { __refvalue(p_detected_objects, List<string>).ToArray()[i]}\n");
                    }
                }

            }
            float conf = 1 - (err / test_num);
            Console.Write($" tests:{test_num}\n");
            Console.Write($"Success: {test_num - err}\n");
            Console.Write($"Not found: {err}\n");
            Console.Write($"Confidence: {conf}\n");

        }
        [TestMethod]
        public unsafe void TestImageProcessingAll()
        {
            var mc = new Processing();
            mc.NetInit();

            string classname = "test";

            string path = "C:\\VKR\\WinFormsApp1\\test_database\\all\\";
            string fileName = "";
            string openPath = "";

            float test_num = Directory.GetFiles(path)
                        .Where(file => !file.Contains("save"))
                        .Count();

           
            for (int t = 1; t <= test_num; t++)
            {
                List<string> detected_objects = new List<string>();
                TypedReference p_detected_objects = __makeref(detected_objects);
                VectorOfRect bboxes = new VectorOfRect();
                VectorOfFloat scores = new VectorOfFloat();
                VectorOfInt indices = new VectorOfInt();
                fileName = classname + t.ToString() + ".jpg";
                openPath = path + fileName;
                Image<Bgr, byte> img = new Image<Bgr, byte>(openPath);

                Console.Write($"Номер теста:{t}\n");


                Size new_size = new Size(416, 416);
                var blob = DnnInvoke.BlobFromImage(img, 1 / 255.0, size: new_size, swapRB: true, crop: false);

                mc.model.SetInput(blob);
                VectorOfMat vectorOfMat = new VectorOfMat();
                mc.model.Forward(vectorOfMat, mc.model.UnconnectedOutLayersNames);

                for (int k = 0; k < vectorOfMat.Size; k++)
                {
                    var mat = vectorOfMat[k];
                    var data = Processing.ArrayTo2DList(mat.GetData());

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
                            float ImgWidth = img.Width;
                            float ImgHeight = img.Height;
                            Console.Write($"Найден класс: {classId}\n");
                            Console.Write($"Коэффициент уверенности: {confidence}\n"); 
                            Console.Write($"Координата X: {center_x / ImgWidth}\n");
                            Console.Write($"Координата Y: {center_y / ImgHeight}\n");
                            Console.Write("------------------------\n");
                            bboxes.Push(new Rectangle[] { new Rectangle(x, y, width, height) });
                            indices.Push(new int[] { classId });
                            scores.Push(new float[] { confidence });
                        }
                    }
                }
                var idx = DnnInvoke.NMSBoxes(bboxes.ToArray(), scores.ToArray(), 0.1f, 0.3f); 


                for (int i = 0; i < idx.Length; i++)
                {
                    int index = idx[i];
                    var bbox = bboxes[index];
                    float X = bbox.X;
                    float Y = bbox.Y;
                    float Width = bbox.Width;
                    float Height = bbox.Height;
                    float ImgWidth = img.Width;
                    float ImgHeight = img.Height;
                    
                    Console.Write($"Ширина итогового разметочного прямоугольника: {Width / ImgWidth}\n");
                    Console.Write($"Высота итогового разметочного прямоугольника: {Height / ImgHeight}\n");
                }

            }
        }
    }
}