using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageMagick;
using PhotoPartner.Utils;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using ExifProfile = SixLabors.ImageSharp.Metadata.Profiles.Exif.ExifProfile;

namespace PhotoPartner
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private string photoPath = string.Empty;

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置对话框的属性
            // openFileDialog.InitialDirectory = "c:\\"; // 设置初始目录
            openFileDialog.Filter = "所有文件(*.jpg)|*.jpg"; // 设置文件过滤器
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            // 显示对话框
            if (openFileDialog.ShowDialog() != DialogResult.OK) return;
            // 获取选中的文件路径
            photoPath = openFileDialog.FileName;
            var image = System.Drawing.Image.FromFile(photoPath);
            pictureBox1.Image = image;
            Task.Run((() =>
            {
                ResizePic(photoPath);
            }));
            
            
        }

        
        /// <summary>
        /// ImageSharp Resize不会清除exif信息
        /// </summary>
        /// <param name="inpath"></param>
        private void ResizePic(string inpath)
        {
            using (var image = SixLabors.ImageSharp.Image.Load(inpath))
            {
                int width = image.Width + 200;
                int height = image.Height + 200;
                image.Mutate(x => x.Resize(width, height));

                var newPath = inpath.GetNewPath();
                image.Save(newPath);
                MessageBox.Show("合成图片已保存为: " + newPath);
            }
        }
        
        
        
        
        
        
        /*private void Mainsss()
        {
            string sourceImagePath = "path_to_your_image_with_exif.jpg";
            string destinationImagePath = "path_to_output_image.jpg";
            int borderWidth = 100;

            // 加载图片
            using (var image = SixLabors.ImageSharp.Image.Load(sourceImagePath))
            {
                // 添加白色边框和文字
                image.Mutate(x =>
                {
                    x.Resize(image.Width + 2 * borderWidth, image.Height + 2 * borderWidth);
                    x.BackgroundColor(SixLabors.ImageSharp.Color.White);
                    x.DrawText(Path.GetFileNameWithoutExtension(sourceImagePath), new Font("Arial", 12, FontStyle.Bold), Color.Black, new PointF(image.Width + borderWidth - 5, 5));
                });
                
                // 保存图片到内存流中
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.Save(memoryStream, new JpegEncoder());
                    memoryStream.Position = 0;

                    // 加载图片到ImageSharp的Image对象中
                    using (var imageSharp = SixLabors.ImageSharp.Image.Load(memoryStream))
                    {
                        // 从原始图片中读取EXIF信息
                        ExifProfile exifProfile = image.PropertyItems.GetExifProfile();

                        // 将EXIF信息写入ImageSharp的Image对象中
                        imageSharp.Metadata.ExifProfile = exifProfile;

                        // 保存图片到文件，保留EXIF信息
                        imageSharp.Save(destinationImagePath);
                    }
                }
            }
        }*/
        
        
    }
    
    
    
    
    
    public static class ImageExtensions
    {
        public static ExifProfile GetExifProfile(this PropertyItem[] propItems)
        {
            ExifProfile exifProfile = null;

            foreach (PropertyItem propItem in propItems)
            {
                if (propItem.Id == 0x9000)
                {
                    exifProfile = new ExifProfile(propItem.Value);
                    break;
                }
            }

            return exifProfile;
        }
    }
}