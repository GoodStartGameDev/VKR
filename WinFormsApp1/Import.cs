using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using System.Windows.Forms;
using Google.Apis;
using Google.Apis.Auth.OAuth2;
using System.IO;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Download;

using YandexDiskNET;


namespace WinFormsApp1
{
    // Class to demonstrate use-case of drive's download file.
    public class DownloadFile
    {
        /// <summary>
        /// Download a Document file in PDF format.
        /// </summary>
        /// <param name="fileId">file ID of any workspace document format file.</param>
        /// <returns>byte array stream if successful, null otherwise.</returns>
        public static MemoryStream DriveDownloadFile(string fileId)
        {
            try
            {
                /* Load pre-authorized user credentials from the environment.
                 TODO(developer) - See https://developers.google.com/identity for 
                 guides on implementing OAuth2 for your application. */
                GoogleCredential credential = GoogleCredential
                    .GetApplicationDefault()
                    .CreateScoped(DriveService.Scope.Drive);

                // Create Drive API service.
                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Drive API Snippets"
                });

                var request = service.Files.Get(fileId);
                var stream = new MemoryStream();

                // Add a handler which will be notified on progress changes.
                // It will notify on each chunk download and when the
                // download is completed or failed.
                request.MediaDownloader.ProgressChanged +=
                    progress =>
                    {
                        switch (progress.Status)
                        {
                            case DownloadStatus.Downloading:
                                {
                                    Console.WriteLine(progress.BytesDownloaded);
                                    break;
                                }
                            case DownloadStatus.Completed:
                                {
                                    Console.WriteLine("Download complete.");
                                    break;
                                }
                            case DownloadStatus.Failed:
                                {
                                    Console.WriteLine("Download failed.");
                                    break;
                                }
                        }
                    };
                request.Download(stream);

                return stream;
            }
            catch (Exception e)
            {
                // TODO(developer) - handle error appropriately
                if (e is AggregateException)
                {
                    Console.WriteLine("Credential Not found");
                }
                else
                {
                    throw;
                }
            }
            return null;
        }
    }
    class Import
    {
        public String Import_file()
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Title = "Select file";
            openFileDialog1.InitialDirectory = "C:\\VKR"; // заглушка ;
            openFileDialog1.Filter = "All files(*.*)|*.*|JPG|*.jpg|JPEG|*.jpeg|PNG|*.png|TIFF|*.tiff";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "" && openFileDialog1.CheckFileExists)
            { return openFileDialog1.FileName; }
            else
            return null;
        }

        public async void buttonTest_Click(Label label)
        {
            string oauth = "y0_AgAAAABLox37AAn22QAAAADj_Usy-ORM0OOFTS2K2U2d4mErxcZDQZE";
            string sourceFileName = "1.jpg";
            string destFileName = @"C:\VKR\1.jpg";
            //https://github.com/GoodStartGameDev/VKR#access_token=y0_AgAAAABLox37AAn22QAAAADj_Usy-ORM0OOFTS2K2U2d4mErxcZDQZE&token_type=bearer&expires_in=31535989

            YandexDiskRest disk = new YandexDiskRest(oauth);

            var err = await disk.DownloadResourceAcync(sourceFileName, destFileName);
            if (err.Message == null)
                label.Text = string.Format("Success downloaded \n{0}", Path.GetFileName(sourceFileName)) + "\r\n";
            else
                label.Text = string.Format(err.Message) + "\r\n";

          
        }

        //progress view for win form
        public Progress<double> SetProgressChange(ProgressBar progressBar)
        {
            Progress<double> progressChange = new Progress<double>();
            progressChange.ProgressChanged += (send, value) => progressBar.Value = (int)value;
            return progressChange;
        }
    }
}
