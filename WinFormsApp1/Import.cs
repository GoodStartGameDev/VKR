using System;
using System.Windows.Forms;
using System.Net;

namespace WinFormsApp1
{
    class Import
    {
        public String Import_file()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select file";
            openFileDialog1.InitialDirectory = "C:\\"; 
            openFileDialog1.Filter = "All files(*.*)|*.*|JPG|*.jpg|JPEG|*.jpeg|PNG|*.png|TIFF|*.tiff";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "" && openFileDialog1.CheckFileExists)
            { return openFileDialog1.FileName; }
            else
            return null;
        }

        public void Cloud_import(TextBox textBox,Label label)
        {
            string url = textBox.Text;
            try
            {
                using (var client = new WebClient())
                {
                    var head = client.DownloadData(url);
                    var contentLength = client.ResponseHeaders["Content-Length"];
                    var contentType = client.ResponseHeaders["Content-Type"];

                    using (var dialog = new SaveFileDialog())
                    {
                        dialog.FileName = "download";
                        dialog.Filter = $"{contentType} ({contentLength} bytes)|*.{contentType.Split('/')[1]}";
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            client.DownloadFile(url, dialog.FileName);
                            label.Text = "Успешно";
                        }
                        else label.Text = "Ошибка загрузки";
                    }
                }
            }
            catch (Exception ex)
            {
                label.Text = "Ошибка загрузки";
            }
        }
    }
}
