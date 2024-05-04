using System.Drawing.Imaging;
using System.IO;

namespace PhotoPartner.Utils
{
    public static class Utils
    {
        public static string GetNewPath(this string photoPath)
        {
            var p1 = Path.GetDirectoryName(photoPath);
            var p3 = Path.GetFileNameWithoutExtension(photoPath);
            var p4 = Path.GetExtension(photoPath);
            var newFileName = Path.Combine(p1, p3 + "_new" + p4);
            var i = 1;
            while (File.Exists(newFileName))
            {
                newFileName = Path.Combine(p1, p3 + "_new" + i + p4);
                i++;
            }

            return newFileName;
        }


        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
    }
    
}