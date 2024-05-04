using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PhotoPartner.Utils
{
    
    //https://www.cnblogs.com/sandea/p/3294254.html
    public class PhotoUtil
    {
        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);
        private const String configFile = "Config.ini";
 
        public static void Main2(string[] args)
        {
            DirectoryInfo workingDir = new DirectoryInfo(ReadConfig("General", "WorkingDir",Environment.CurrentDirectory));
            if (!workingDir.Exists)
            {
                workingDir = new DirectoryInfo(Environment.CurrentDirectory);
            }
            int quality=int.Parse(ReadConfig("General", "Quality", "85"));
            bool needResize = Boolean.Parse(ReadConfig("ResizeImage", "Enable", "false"));
            int newWidth = int.Parse(ReadConfig("ResizeImage", "NewWidth", "800"));
            int newHeight = int.Parse(ReadConfig("ResizeImage", "NewHeight", "600"));
            bool padding = Boolean.Parse(ReadConfig("ResizeImage", "Padding", "false"));
            bool needRotate = Boolean.Parse(ReadConfig("RotateImage", "Enable", "true"));
            FileInfo[] files = workingDir.GetFiles();
            DirectoryInfo output = workingDir.CreateSubdirectory(DateTime.Now.ToString("yyyyMMdd"));
            foreach (FileInfo i in files)
            {
                String type = i.Extension.ToLower();
                if (type.Contains("jpg") || type.Contains("jpeg") || (type.Contains("png")) || type.Contains("tif") || type.Contains("bmp"))
                {
                    Image img = Image.FromFile(i.FullName);
                    if (needResize)
                    {
                        Console.WriteLine("Resizing " + i.FullName);
                        ResizeImage(ref img, newWidth, newHeight, padding);
                    }
                    if (needRotate)
                    {
                        Console.WriteLine("Rotating " + i.FullName);
                        RotateImage(img);
                    }
                    SaveAs(img, output.FullName+"\\\\"+i.Name, quality);
                }
            }
            Console.ReadLine();
        }
 
        private static void SaveAs(Image img, string dest, long quality)
        {
            if (quality > 100 || quality < 1)
            {
                quality = 85;
            }
            EncoderParameters para = new EncoderParameters();
            para.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            String extension = new FileInfo(dest).Extension;
            ImageCodecInfo info = GetImageCodecInfoByExtension(extension);
            if (info != null)
            {
                img.Save(dest, info, para);
            }
            else
            {
                throw new Exception("Unrecognized format + extension ");
            }
        }
 
        private static void ResizeImage(ref Image image, int expectDestWidth, int expectDestHeight,bool padding)
        {
            PropertyItem[] exif = image.PropertyItems;
            int targetWidth = 0;
            int targetHeight = 0;
            double srcHWRate = (double)image.Width / (double)image.Height;
            double expectHWRate = (double)expectDestWidth / (double)expectDestHeight;
            if (srcHWRate > expectHWRate)
            {
                targetWidth = expectDestWidth;
                targetHeight = System.Convert.ToInt32(Math.Round(expectDestWidth / srcHWRate, 0));
            }
            else
            {
                targetHeight = expectDestHeight;
                targetWidth = System.Convert.ToInt32(Math.Round(expectDestHeight * srcHWRate, 0));
            }
 
            Image bitmap = null;
            if (!padding)
            {
                bitmap = new Bitmap(targetWidth, targetHeight);
            }
            else
            {
                bitmap = new Bitmap(expectDestWidth, expectDestHeight);
            }
            Graphics g = Graphics.FromImage(bitmap);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height), new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);
            foreach (PropertyItem i in exif)
            {
                if (i.Id == 40962)
                {
                    i.Value = BitConverter.GetBytes(targetWidth);
                }
                else if (i.Id == 40963)
                {
                    i.Value = BitConverter.GetBytes(targetHeight);
                }
                bitmap.SetPropertyItem(i);
            }
            g.Dispose();
            image.Dispose();
            image = bitmap;
        }
 
        private static string ReadConfig(String Section, String Key, String defaultValue)
        {
            if (File.Exists(configFile))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, String.Empty, temp, 1024, new FileInfo(configFile).FullName);
                if (!String.IsNullOrEmpty(temp.ToString()))
                {
                    return temp.ToString();
                }
                else
                {
                    return defaultValue;
                }
            }
            else
            {
                return defaultValue;
            }
        }
 
        public static void RotateImage(Image img)
        {
            PropertyItem[] exif = img.PropertyItems;
            byte orientation = 0;
            foreach (PropertyItem i in exif)
            {
                if (i.Id == 274)
                {
                    orientation = i.Value[0];
                    i.Value[0] = 1;
                    img.SetPropertyItem(i);
                }
            }
 
            switch (orientation)
            {
                case 2:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;
                case 5:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 7:
                    img.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
                default:
                    break;
            }
            foreach (PropertyItem i in exif)
            {
                if (i.Id == 40962)
                {
                    i.Value = BitConverter.GetBytes(img.Width);
                }
                else if (i.Id == 40963)
                {
                    i.Value = BitConverter.GetBytes(img.Height);
                }
            }
        }
 
        private static ImageCodecInfo GetImageCodecInfoByExtension(String extension)
        {
            ImageCodecInfo[] list = ImageCodecInfo.GetImageEncoders();
            foreach(ImageCodecInfo i in list)
            {
                if (i.FilenameExtension.ToLower().Contains(extension.ToLower()))
                {
                    return i;
                }
            }
            return null;
        }
    }
}