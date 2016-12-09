using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FaceRecognition1.Helper
{
    public class ImageLoader
    {
        public static List<List<String>> GetImages()
        {
            List<List<string>> imagesList = new List<List<string>>();
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            if(folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var path = folderBrowserDialog.SelectedPath;
                foreach( var dir in Directory.GetDirectories(path))
                {
                    List<string> images = new List<string>();
                    foreach ( var file in Directory.GetFiles(dir))
                    {
                        images.Add(file);
                    }
                    imagesList.Add(images);
                }
            }
            return imagesList;
        }
    }
}
