using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ImageMagick;
using PhotoPartner.Utils;

namespace PhotoPartner
{
    public partial class FormMagick : Form
    {
        public FormMagick()
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
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var photoPath = openFileDialog.FileName;
                var image = System.Drawing.Image.FromFile(photoPath);
                pictureBox1.Image = image;
                Task.Run((() =>
                {
                    // Hanndle1(photoPath);
                    DrawClaerTest(photoPath);
                    // ResizeToFixedSize(photoPath);
                    MessageBox.Show("合成图片已保存");
                }));
            }
        }
        
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            // 设置对话框的属性
            // openFileDialog.InitialDirectory = "c:\\"; // 设置初始目录
            openFileDialog.Filter = "所有文件(*.jpg)|*.jpg"; // 设置文件过滤器
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            // 显示对话框
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                var photoPath = openFileDialog.FileName;
                
                using var magickImage = new MagickImage(photoPath);
                using var memoryStream = magickImage.ToMemoryStream();
                
                // var image = System.Drawing.Image.FromFile(photoPath);
                var image2 = System.Drawing.Image.FromStream(memoryStream);
                pictureBox1.Image = image2;
                
            }
        }

        private void DrawClaerTest(string photoPath)
        {
            // Read from file
            using var image = new MagickImage(photoPath);

            var size = new MagickGeometry(image.Width+200, image.Height + 200);
            // This will resize the image to a fixed size without maintaining the aspect ratio.
            // Normally an image will be resized to fit inside the specified size.
            size.IgnoreAspectRatio = true;  //忽略长宽比例

            image.Resize(size);

            int weith = image.Width;
            int height = image.Height;
            
            // 定义一个矩形，位置和大小可以根据需要调整
            //MagickGeometry rectangleGeometry = new MagickGeometry(0, 0, weith, height);

            /*new Drawables()
                // Draw text on the image
                .FontPointSize(72)
                .Font("Comic Sans")
                .StrokeColor(new MagickColor("yellow"))
                .FillColor(MagickColors.Orange)
                .TextAlignment(TextAlignment.Center)
                .Text(256, 64, "Magick.NET")
                // Add an ellipse
                .StrokeColor(new MagickColor(0, Quantum.Max, 0))
                .FillColor(MagickColors.SaddleBrown)
                .Ellipse(256, 96, 192, 8, 0, 360)
                .Draw(image);*/



            var drawables = new Drawables()
                .Rectangle(0, 0, weith, height)
                .FillColor(MagickColors.Yellow);

            image.Draw(drawables);

            // Save the result
            var newPath = photoPath.GetNewPath();
            image.Write(newPath);
        }

        
        //加载图片
        private (MagickImage, MagickImage) LoadImage(string photoPath)
        {
            var image = new MagickImage(photoPath);
            return (image, (MagickImage)image.Clone());
        }


        //清除像素
        private MagickImage DrawClaer(MagickImage image)
        {
            var drawables = new Drawables()
                .Rectangle(0, 0, image.Width, image.Height)
                .FillColor(MagickColors.Transparent);

            image.Draw(drawables);
            
            return image;
        }
        
        //重置大小
        private MagickImage DrawResize(MagickImage image, int width, int height)
        {
            var size = new MagickGeometry(width, height);
            // This will resize the image to a fixed size without maintaining the aspect ratio.
            // Normally an image will be resized to fit inside the specified size.
            size.IgnoreAspectRatio = true;  //忽略长宽比例
            image.Resize(size);
            return image;
        }

        //合并
        private MagickImage DrawCombine(MagickImage bkimage, MagickImage image)
        {

            // 将照片放置在白底图片的中心
            int x = (bkimage.Width - image.Width) / 2;
            int y = (bkimage.Height - image.Height) / 2;
            bkimage.Composite(image, x, y, CompositeOperator.Over);
            
            //var exifProfile = image.GetExifProfile();
            //保留原始exif信息
            // if (exifProfile != null) bkim
            // ge.SetProfile(exifProfile); 
            
            return bkimage;
        }

        //
        private MagickImage DrawText(MagickImage image)
        {
            /*new Drawables()
                // Draw text on the image
                .FontPointSize(72)
                .Font("Comic Sans")
                .StrokeColor(new MagickColor("yellow"))
                .FillColor(MagickColors.Orange)
                .TextAlignment(TextAlignment.Center)
                .Text(256, 64, "Magick.NET")
                // Add an ellipse
                .StrokeColor(new MagickColor(0, Quantum.Max, 0))
                .FillColor(MagickColors.SaddleBrown)
                .Ellipse(256, 96, 192, 8, 0, 360)
                .Draw(image);*/



            var drawables = new Drawables()
                .FontPointSize(72)
                .Font("Comic Sans")
                .StrokeColor(MagickColors.Black) //MagickColor.FromRgba(1,1,1,1)
                .FillColor(MagickColors.Black)
                .TextAlignment(TextAlignment.Center)
                .Text(256, 64, "Magick.NET")
                .Rectangle(0, 0, 10, 10)
                .FillColor(MagickColors.Yellow);

            image.Draw(drawables);
            return image;
        }
        
        //
        private MagickImage DrawLogo(MagickImage image)
        {
            /*new Drawables()
                // Draw text on the image
                .FontPointSize(72)
                .Font("Comic Sans")
                .StrokeColor(new MagickColor("yellow"))
                .FillColor(MagickColors.Orange)
                .TextAlignment(TextAlignment.Center)
                .Text(256, 64, "Magick.NET")
                // Add an ellipse
                .StrokeColor(new MagickColor(0, Quantum.Max, 0))
                .FillColor(MagickColors.SaddleBrown)
                .Ellipse(256, 96, 192, 8, 0, 360)
                .Draw(image);*/



            var drawables = new Drawables()
                .Rectangle(0, 0, 10, 10)
                .FillColor(MagickColors.Yellow);

            image.Draw(drawables);
            return image;
        }
        
        public static void AddTextToExistingImage(string photo)
        {
            var textToWrite = "Insert This Text Into Image";

            // These settings will create a new caption
            // which automatically resizes the text to best
            // fit within the box.

            var settings = new MagickReadSettings
            {
                Font = "Calibri",
                TextGravity = Gravity.Center,
                BackgroundColor = MagickColors.Transparent,
                Height = 250, // height of text box
                Width = 680 // width of text box
            };

            using var image = new MagickImage(photo);
            using var caption = new MagickImage($"caption:{textToWrite}", settings);

            // Add the caption layer on top of the background image
            // at position 590,450
            image.Composite(caption, 590, 450, CompositeOperator.Over);

            image.Write(photo.GetNewPath());
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            using var image = new MagickImage(new MagickColor("#ff00ff"), 512, 128);

            new Drawables()
                // Draw text on the image
                .FontPointSize(72)
                .Font("Comic Sans")
                .StrokeColor(new MagickColor("yellow"))
                .FillColor(MagickColors.Orange)
                .TextAlignment(TextAlignment.Center)
                .Text(256, 64, "Magick.NET")
                // Add an ellipse
                .StrokeColor(new MagickColor(0, Quantum.Max, 0))
                .FillColor(MagickColors.SaddleBrown)
                .Ellipse(256, 96, 192, 8, 0, 360)
                .Draw(image);

            using var stream = image.ToMemoryStream();
            pictureBox1.Image = System.Drawing.Image.FromStream(stream);
        }
        

        private void Hanndle1(string photoPath)
        {
            // 加载照片并获取其尺寸
            using (var photoImage = new MagickImage(photoPath))
            {
                photoImage.Quality = 100;
                int photoWidth = photoImage.Width;
                int photoHeight = photoImage.Height;
                
                var exifProfile = photoImage.GetExifProfile();

                // 创建一个比照片大100像素的白底图片
                int whiteBackgroundWidth = photoWidth + 100;
                int whiteBackgroundHeight = photoHeight + 100;
                using (var bkImg = new MagickImage(MagickColors.Blue, whiteBackgroundWidth,whiteBackgroundHeight))
                {
                    // 将照片放置在白底图片的中心
                    int x = (whiteBackgroundWidth - photoWidth) / 2;
                    int y = (whiteBackgroundHeight - photoHeight) / 2;
                    bkImg.Composite(photoImage, x, y, CompositeOperator.Over);

                    //保留原始exif信息
                    // if (exifProfile != null) whiteBackground.SetProfile(exifProfile); 

                    bkImg.Quality = 100;
                    
                    // 保存输出图片
                    var newPath = photoPath.GetNewPath();
                    Console.WriteLine(newPath);
                    bkImg.Write(newPath);
                    
                    
                    // 创建一个 MagickWriteSettings 对象并设置相关属性
                    // MagickWriteSettings settings = new MagickWriteSettings();
                    // settings.Quality = 90; // 设置 JPEG 压缩质量为 90
                    // settings.PreserveExifProfile = true; // 设置保留 EXIF 信息
                    
                    bkImg.Write(newPath,MagickFormat.Jpeg);
                    
                    
                    MessageBox.Show("合成图片已保存为: " + newPath);
                }
            }
        }
        
        public static void ResizeToFixedSize(string photoPath)
        {
            // Read from file
            using var image = new MagickImage(photoPath);

            var size = new MagickGeometry(image.Width+500, 0);
            // This will resize the image to a fixed size without maintaining the aspect ratio.
            // Normally an image will be resized to fit inside the specified size.
            size.IgnoreAspectRatio = false;  //忽略长宽比例

            image.Resize(size);

            // Save the result
            var newPath = photoPath.GetNewPath();
            image.Write(newPath);
        }
        
        public static void ReadExifData(string photoPath)
        {
            // Read image from file
            using var image = new MagickImage(photoPath);

            // Retrieve the exif information
            var profile = image.GetExifProfile();

            // Check if image contains an exif profile
            if (profile is null)
            {
                Console.WriteLine("Image does not contain exif information.");
            }
            else
            {
                // Write all values to the console
                foreach (var value in profile.Values)
                {
                    Console.WriteLine("{0}({1}): {2}", value.Tag, value.DataType, value.ToString());
                }
            }
        }

        
    }

    public static class MyExtensions
    {
        public static MemoryStream ToMemoryStream(this MagickImage image)
        {
            var memoryStream = new MemoryStream();
            image.Write(memoryStream, MagickFormat.Bmp);
            return memoryStream;
        }
    }
}