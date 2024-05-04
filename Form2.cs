using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PhotoPartner.Utils;
using Encoder = System.Drawing.Imaging.Encoder;


namespace PhotoPartner
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private string photoPath = string.Empty;
        
        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            var image = Image.FromFile(photoPath);
            pictureBox1.Image = image;
            foreach (var propItem in image.PropertyItems)
            {
                richTextBox1.AppendText($"ID:{propItem.Id},Value:{Encoding.UTF8.GetString(propItem.Value)},Type:{propItem.Type}");
                richTextBox1.AppendText(Environment.NewLine);
            }
        }
        
        
        private void Process2(string photo)
        {
            string sourceImagePath = photo;  //"path_to_your_image_with_exif.jpg"; // 替换为你的图片路径
            string destinationImagePath = photo.GetNewPath();  // "path_to_output_image.jpg"; // 替换为你想要保存的输出图片路径

            // 加载图片并获取EXIF信息
            Image sourceImage = Image.FromFile(sourceImagePath);
            ImageCodecInfo jpgEncoder = Utils.Utils.GetEncoder(ImageFormat.Jpeg);

            // 添加白色边框
            int borderWidth = 100;
            Bitmap borderedBitmap = new Bitmap(sourceImage.Width + 2 * borderWidth, sourceImage.Height + 2 * borderWidth);
            using (Graphics graphics = Graphics.FromImage(borderedBitmap))
            {
                // 设置高质量渲染
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                // 填充白色背景
                graphics.Clear(Color.Goldenrod);

                // 绘制原始图片到带有边框的位图上
                graphics.DrawImage(sourceImage, new Rectangle(borderWidth, borderWidth, sourceImage.Width, sourceImage.Height));

                // 在底部边框中间写上照片文件名
                string fileName = Path.GetFileNameWithoutExtension(sourceImagePath);
                using (Font font = new Font("Arial", 12, FontStyle.Bold))
                {
                    SizeF textSize = graphics.MeasureString(fileName, font);
                    float x = borderedBitmap.Width - (float)textSize.Width - borderWidth;
                    float y = borderedBitmap.Height - (float)textSize.Height - borderWidth;
                    graphics.DrawString(fileName, font, Brushes.Black, x, y);
                }
            }

            // 保存图片，同时保留EXIF信息
            FileInfo fileInfo = new FileInfo(sourceImagePath);
            using (FileStream fs = fileInfo.OpenRead())
            {
                PropertyItem[] propItems = sourceImage.PropertyItems;
                var encoderParams = new EncoderParameters(1)
                {
                    Param = new[] { new EncoderParameter(Encoder.Quality, 100L) }
                };
                
                // 保存带有EXIF信息的JPG
                borderedBitmap.Save(destinationImagePath, jpgEncoder, encoderParams);

                // 将原始的EXIF信息写入新的图片
                if (propItems.Length > 0)
                {
                    using (Image newImage = Image.FromFile(destinationImagePath))
                    {
                        // PropertyItem[] newPropItems = newImage.PropertyItems;

                        // 删除新图片的EXIF信息
                        // newImage.PropertyItems.Clear(); //没有Clear()
                        // newImage.PropertyItems = new PropertyItem[0]; //没有setter 

                        // 添加原始图片的EXIF信息
                        // newImage.PropertyItems = propItems.Concat(newPropItems).ToArray();
                        
                        foreach (var propItem in propItems)
                        {
                            newImage.SetPropertyItem(propItem);
                        }

                        // 保存图片
                        newImage.Save(destinationImagePath, jpgEncoder, encoderParams);
                    }
                }
            }

            // 释放资源
            sourceImage.Dispose();
            borderedBitmap.Dispose();
        }
    }
    
    
    
}