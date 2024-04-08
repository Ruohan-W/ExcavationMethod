using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ExcavationMethod.Revit.Utilities
{
    public static class ImageUtils
    {
        public static BitmapImage LoadImage(Assembly assembly, string name)
        {
            BitmapImage img = new BitmapImage();
            try
            {
                string resourceName = assembly
                    .GetManifestResourceNames()
                    .FirstOrDefault(n => n.Contains(name));
                Stream stream = assembly.GetManifestResourceStream(resourceName);
                img.BeginInit();
                img.StreamSource = stream;
                img.EndInit();
            }
            catch (Exception ex){ }
            {
                //ignore
            }
            return img;
        }

        public static BitmapImage LoadImage(Assembly assembly, Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.StreamSource = memory;
                img.CacheOption = BitmapCacheOption.OnLoad;
                img.EndInit();
                img.Freeze();

                return img;
            }
        }
    }
}
