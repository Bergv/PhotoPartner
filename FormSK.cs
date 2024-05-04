using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using PhotoPartner.Utils;
using SkiaSharp;

namespace PhotoPartner
{
    public partial class FormSK : Form
    {
        public FormSK()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Task.Run((DrawRandom));
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
                var image = System.Drawing.Image.FromFile(photoPath);
                pictureBox1.Image = image;
                Task.Run((() =>
                {
                    DrawPic1(photoPath);
                    MessageBox.Show("合成图片已保存");
                }));
            }
        }


        private void DrawPic1(string photoPath)
        {
            // 加载原始图片
        using (var image = SKBitmap.Decode(photoPath))
        {
            // 提取原始图像的 EXIF 信息
            // var metadata = ImageMetadataReader.ReadMetadata(originalImage);

            // 获取图片的尺寸
            var width = image.Width;
            var height = image.Height;

            // 创建一个比原始图片大100px的新图片
            using (var skBitmap = new SKBitmap(width + 100, height + 100))
            using (var skCanvas = new SKCanvas(skBitmap))
            {
                // 清除画布为白色背景（或你想要的任何颜色）
                skCanvas.Clear(SKColors.Fuchsia);

                // 在新图片的中心绘制原始图片
                var destRect = new SKRect(50, 50, width + 50, height + 50); // 50是额外的100px的一半
                skCanvas.DrawImage(SKImage.FromBitmap(image), destRect);

                // 在图片底部写入文件名
                using (var paint = new SKPaint())
                {
                    paint.Color = SKColors.Black;
                    paint.TextSize = 20; // 字体大小可以根据需要调整
                    paint.IsAntialias = true;
                    var text = Path.GetFileNameWithoutExtension(photoPath); // 获取文件名，不包含扩展名
                    var textBounds = new SKRect();
                    paint.MeasureText(text, ref textBounds);
                    var textX = (width + 100 - textBounds.Width) / 2; // 水平居中
                    var textY = height + 50; // 距离底部一定的距离，比如50px
                    skCanvas.DrawText(text, textX, textY, paint);
                }

                // 导出为 JPG，质量100%
                using var jpgData = skBitmap.Encode(SKEncodedImageFormat.Jpeg, 100);

                // 创建新的输出文件流
                using (var outputStream = new FileStream(photoPath.GetNewPath(), FileMode.Create))
                {
                    // 写入 JPG 数据到文件
                    var bytes = jpgData.ToArray();
                    outputStream.Write(bytes, 0, bytes.Length);

                    // 将原始的 EXIF 信息写入输出文件
                    // foreach (var directory in metadata.Directories)
                    // {
                    //     directory.WriteTo(outputStream);
                    // }
                }
            }
        }
        }
        

        private void DrawRandom()
        {
            var width = pictureBox1.Width;
            var height = pictureBox1.Height;
            var imageInfo = new SKImageInfo(width: width, height: height, colorType: SKColorType.Rgba8888, alphaType: SKAlphaType.Premul);
    
            var surface = SKSurface.Create(imageInfo);

            var canvas = surface.Canvas;
            
            canvas.Clear(SKColor.Parse("#003366"));
            
            var rand = new Random();
            int.TryParse(textBox1.Text, out var n);
            
            long lineCount = 1000 * n;
            for (var i = 0; i < lineCount; i++)
            {
                float lineWidth = rand.Next(1, 20);
                var lineColor = new SKColor(
                    red: (byte)rand.Next(255), green: (byte)rand.Next(255),
                    blue: (byte)rand.Next(255), alpha: (byte)rand.Next(255));

                var linePaint = new SKPaint
                {
                    Color = lineColor,
                    StrokeWidth = lineWidth,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                };

                int x1 = rand.Next(imageInfo.Width);
                int y1 = rand.Next(imageInfo.Height);
                int x2 = rand.Next(imageInfo.Width);
                int y2 = rand.Next(imageInfo.Height);
                canvas.DrawLine(x1, y1, x2, y2, linePaint);
            }
            
            using (SKImage image = surface.Snapshot())
            using (SKData data = image.Encode())
            using (System.IO.MemoryStream mStream = new System.IO.MemoryStream(data.ToArray()))
            {
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = new Bitmap(mStream, false);
            }
        }

        
    }
}