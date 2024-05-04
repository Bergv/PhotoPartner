using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Encoder = System.Drawing.Imaging.Encoder;
using PhotoPartner.Utils;

//using Encoder = System.Drawing.Imaging.Encoder;

namespace PhotoPartner
{
    public partial class Form1 : Form
    {
        private Bitmap combinedImage;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private Bitmap CreateCombinedImage(string path)
        {
            // 导入要放置在中心的照片
            string photoPath = @path; // 替换为你的照片路径
            Image photo = Image.FromFile(photoPath);

            // 创建一个200x200像素的白色背景图
            int widthAdd = 200;
            int heightAdd = 200;
            Bitmap whiteBackground = new Bitmap(photo.Width + widthAdd, photo.Height + heightAdd);
            using (Graphics g = Graphics.FromImage(whiteBackground))
            {
                g.Clear(Color.Aqua);
            }

            // 计算照片在白色背景图上的位置，使其居中
            int photoX = widthAdd / 2;
            int photoY = heightAdd / 2;

            // 在白色背景图上绘制照片
            using (Graphics g = Graphics.FromImage(whiteBackground))
            {
                g.DrawImage(photo, photoX, photoY, photo.Width, photo.Height);
            }

            // 释放照片图像资源
            photo.Dispose();

            return whiteBackground;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
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


        private string photoPath = String.Empty;

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
                // 获取选中的文件路径
                photoPath = openFileDialog.FileName;
                combinedImage = CreateCombinedImage(photoPath);
                pictureBox1.Image = combinedImage;
            }
        }


        private void exportButton_Click(object sender, EventArgs e)
        {
            // 导出合成的图片为JPG格式
            var outputPath = photoPath.GetNewPath(); //"H:\\photo\\output.jpg"; // 输出文件的路径

            //combinedImage.Save(outputPath, ImageFormat.Jpeg);
            //ExportImage(combinedImage, outputPath);
            ExportImage2(pictureBox1.Image, outputPath);

            // 显示消息框通知用户导出完成
            MessageBox.Show("合成图片已保存为: " + outputPath);
        }
        

        private void button3_Click(object sender, EventArgs e)
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
                // 获取选中的文件路径
                photoPath = openFileDialog.FileName;
                pictureBox1.Image = Image.FromFile(photoPath);
            }
        }

        
        private void ExportImage(Image image, string outputPath)
        {
            image.Save(outputPath, ImageFormat.Jpeg);
        }

        /// <summary>
        /// 导出合成的图片为jpg格式，质量为100%
        /// </summary>
        /// <param name="image"></param>
        /// <param name="outputPath"></param>
        private void ExportImage2(Image image, string outputPath)
        {
            // 设置JPEG编码器以最大质量导出
            ImageCodecInfo jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            System.Drawing.Imaging.Encoder encoder = System.Drawing.Imaging.Encoder.Quality;
            EncoderParameter encoderParameter = new EncoderParameter(encoder, 100L); // 100 is the maximum quality
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = encoderParameter;

            // 将图像导出为JPG格式
            image.Save(outputPath, jpgEncoder, encoderParameters);
        }
        
    }
}