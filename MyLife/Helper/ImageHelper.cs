using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyLife.Helper
{
    /// <summary>
    /// 图片压缩
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// 指定缩放类型
        /// </summary>
        public enum ImgThumbnailType
        {
            /// <summary>
            /// 指定高宽缩放（可能变形）
            /// </summary>
            WH = 0,
            /// <summary>
            /// 指定宽，高按比例
            /// </summary>
            W = 1,
            /// <summary>
            /// 指定高，宽按比例
            /// </summary>
            H = 2,
            /// <summary>
            /// 指定高宽裁减（不变形）
            /// </summary>
            Cut = 3
        }
        #region Thumbnail
        /// <summary>
        /// 无损压缩图片
        /// </summary>
        /// <param name="sFile">原图片</param>
        /// <param name="dFile">压缩后保存位置</param>
        /// <param name="height">高度</param>
        /// <param name="width">宽度</param>
        /// <param name="flag">压缩质量 1-100</param>
        /// <param name="type">压缩缩放类型</param>
        /// <returns></returns>
        private static bool Thumbnail(System.Drawing.Image iSource, string dFile, int height, int width, int flag, ImgThumbnailType type)
        {
            //System.Drawing.Image iSource = System.Drawing.Image.FromFile(sFile);
            ImageFormat tFormat = iSource.RawFormat;

            //缩放后的宽度和高度
            int towidth = width;
            int toheight = height;
            //
            int x = 0;
            int y = 0;
            int ow = iSource.Width;
            int oh = iSource.Height;

            switch (type)
            {
                case ImgThumbnailType.WH://指定高宽缩放（可能变形）           
                    {
                        break;
                    }
                case ImgThumbnailType.W://指定宽，高按比例     
                    {
                        toheight = iSource.Height * width / iSource.Width;
                        break;
                    }
                case ImgThumbnailType.H://指定高，宽按比例
                    {
                        towidth = iSource.Width * height / iSource.Height;
                        break;
                    }
                case ImgThumbnailType.Cut://指定高宽裁减（不变形）     
                    {
                        if ((double)iSource.Width / (double)iSource.Height > (double)towidth / (double)toheight)
                        {
                            oh = iSource.Height;
                            ow = iSource.Height * towidth / toheight;
                            y = 0;
                            x = (iSource.Width - ow) / 2;
                        }
                        else
                        {
                            ow = iSource.Width;
                            oh = iSource.Width * height / towidth;
                            x = 0;
                            y = (iSource.Height - oh) / 2;
                        }
                        break;
                    }
                default:
                    break;
            }

            Bitmap ob = new Bitmap(towidth, toheight);
            Graphics g = Graphics.FromImage(ob);
            g.Clear(System.Drawing.Color.WhiteSmoke);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource
              , new Rectangle(x, y, towidth, toheight)
              , new Rectangle(0, 0, iSource.Width, iSource.Height)
              , GraphicsUnit.Pixel);
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            long[] qy = new long[1];
            qy[0] = flag;//设置压缩的比例1-100
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qy);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
                ImageCodecInfo jpegICIinfo = null;
                for (int i = 0; i < arrayICI.Length; i++)
                {
                    if (arrayICI[i].FormatDescription.Equals("JPEG"))
                    {
                        jpegICIinfo = arrayICI[i];
                        break;
                    }
                }
                if (jpegICIinfo != null)
                {
                    ob.Save(dFile, jpegICIinfo, ep);//dFile是压缩后的新路径
                }
                else
                {
                    ob.Save(dFile, tFormat);
                }
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                iSource.Dispose();

                ob.Dispose();

            }
        }
        #endregion

        private static bool IsPicture(string filePath)//filePath是文件的完整路径 
        {
            try
            {
                FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader reader = new BinaryReader(fs);
                string fileClass;
                byte buffer;
                byte[] b = new byte[2];
                buffer = reader.ReadByte();
                b[0] = buffer;
                fileClass = buffer.ToString();
                buffer = reader.ReadByte();
                b[1] = buffer;
                fileClass += buffer.ToString();

                reader.Close();
                fs.Close();
                if (fileClass.Equals("255216") || fileClass.Equals("7173") || fileClass.Equals("6677") || fileClass.Equals("13780"))//255216是jpg;7173是gif;6677是BMP,13780是PNG;7790是exe,8297是rar 
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static void InsertImg(RichTextBox rtb, string filepath)
        {
            if (IsPicture(filepath))
            {
                //创建临时文件
                string tempFile = Path.GetTempFileName();

                //读取图片过宽就压缩并保存在临时文件中
                System.Drawing.Image iSource = System.Drawing.Image.FromFile(filepath);
                if (iSource.Width > 800.0)
                {
                    Thumbnail(iSource, tempFile, 0, 800, 80, ImageHelper.ImgThumbnailType.W);
                }
                else
                {
                    iSource.Save(tempFile);
                    iSource.Dispose();
                }

                // 读取文件到文件流中并释放占用
                BinaryReader binReader = new BinaryReader(File.Open(tempFile, FileMode.Open));
                FileInfo fileInfo = new FileInfo(tempFile);
                byte[] bytes = binReader.ReadBytes((int)fileInfo.Length);
                binReader.Close();
                File.Delete(tempFile);

                //初始化bitmap
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(bytes);
                bitmap.EndInit();

                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Source = bitmap;
                img.Stretch = (bitmap.PixelWidth < 800) ? Stretch.None : Stretch.Uniform;

                //插入到richtextbox内
                new InlineUIContainer(img, rtb.Selection.End);
                rtb.Selection.ApplyPropertyValue(Block.TextAlignmentProperty, TextAlignment.Center);
                rtb.Document.Blocks.Add(new Paragraph());
                rtb.CaretPosition = rtb.CaretPosition.DocumentEnd;

            }
        }
    }
}