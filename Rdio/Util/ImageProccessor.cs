using ImageProcessor.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using ImageProcessor.Web.Processors;
using ImageProcessor.Processors;
using System.Text.RegularExpressions;
using ImageProcessor.Web.HttpModules;
using ImageProcessor.Web.Helpers;
using System.IO;
using System.Web.Helpers;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace Rdio.Util
{
    public class ImageProccessor
    {
         public static async void WatermarkHandler(object sender, PostProcessingEventArgs e)
        {
            //var d = ((System.Reflection.TypeInfo)(((ImageProcessor.Web.HttpModules.ImageProcessingModule)sender).GetType())).DeclaredFields.ToList()[1];
            //var h = ((ImageProcessor.Web.Caching.ImageCacheBase)(d.GetValue(sender)));
            //BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
            //FieldInfo finfo = h.GetType().GetField("FullPath", bindingFlags);
            //var FileName = finfo.GetValue(h).ToString().Split('/').LastOrDefault().Split('.').FirstOrDefault();
            //var ProductInfo = Rdio.Modules.Product.Common.ProductInfo(FileName);

            //if (bool.Parse(Common.GetSettingValue("applywatermark").AsString))
            //{
            //    var SourceStream = e.ImageStream;
            //    using (Image image = Image.FromStream(SourceStream))
            //    using (Image watermarkImage = Image.FromFile(HttpContext.Current.Server.MapPath(Common.GetSettingValue("watermarkpicture").AsString)))
            //    using (Graphics imageGraphics = Graphics.FromImage(image))
            //    {
            //        var Percent = (float)(Common.GetSettingValue("watermarkpercent").AsInt32 / 100.0);
            //        var Padding = Common.GetSettingValue("watermarkpadding").AsInt32;
            //        var ImageWidth = image.Width;
            //        var ImageHeight = image.Height;

            //        var OrginalWatermarkContainerWidth = 3500;
            //        var OrginalWatermarkContainerHeight = 1000;
            //        var WatermarkContainerWidth = ImageWidth * Percent;
            //        var WatermarkContainerHeight = (WatermarkContainerWidth * OrginalWatermarkContainerHeight) / OrginalWatermarkContainerWidth;
            //        var Position = Common.GetSettingValue("watermarkposition").AsString;
            //        var WatermarkXY = GetWatermarkXY(ImageWidth, ImageHeight, (int)WatermarkContainerWidth, (int)WatermarkContainerHeight, 0,Padding, Position);
            //        var WatermarkContainerRectangle = new Rectangle(WatermarkXY.Item1, WatermarkXY.Item2, (int)WatermarkContainerWidth, (int)WatermarkContainerHeight);
            //        Brush brush = new SolidBrush(Color.FromArgb(40, 255, 255, 255));
            //        imageGraphics.FillRectangle(brush, WatermarkContainerRectangle);

            //        var innerPercent = (float)(40 / 100.0);
            //        var innerPaddingY = 10* innerPercent;

            //        var OrginalWatermarkWidth = watermarkImage.Width;
            //        var OrginalWatermarkHeight = watermarkImage.Height;
            //        var WatermarkWidth = WatermarkContainerWidth * innerPercent;
            //        var WatermarkHeight = (WatermarkWidth * OrginalWatermarkHeight)/OrginalWatermarkWidth;
            //        imageGraphics.DrawImage(watermarkImage,
            //            WatermarkContainerRectangle.X+((WatermarkContainerWidth- WatermarkWidth) / 2), 
            //            WatermarkContainerRectangle.Y+ ((WatermarkContainerRectangle.Height- WatermarkHeight)/2)- WatermarkHeight/2, 
            //            WatermarkWidth, WatermarkHeight);
                   
            //        string text = ProductInfo.GetValue("owner","").AsString;
            //        Font font = new Font("Adobe Arabic", 1,FontStyle.Bold);
            //        Font goodFont = FindFont(imageGraphics, text, new Size((int)(WatermarkContainerRectangle.Width*innerPercent), (int)(WatermarkContainerRectangle.Height*innerPercent)), font, FontStyle.Regular);
            //        SolidBrush drawBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 255));
            //        var fsize=imageGraphics.MeasureString(text, goodFont);
            //        imageGraphics.DrawString(text, goodFont, drawBrush,
            //            WatermarkContainerRectangle.X + ((WatermarkContainerWidth - WatermarkWidth) / 2),
            //            WatermarkContainerRectangle.Y + ((WatermarkContainerHeight - fsize.Height) / 2)+ fsize.Height/2);

            //        var myEncoderParameters = new EncoderParameters(2);
            //        myEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
            //        myEncoderParameters.Param[1] = new EncoderParameter(Encoder.ColorDepth, 100L);
            //        ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);
            //        var WatermarkStream = new MemoryStream();
            //        image.Save(WatermarkStream, jgpEncoder, myEncoderParameters);
            //        WatermarkStream.Position = 0;
            //        e.ImageStream = WatermarkStream;
            //    }
            //}
        }

        public static Font FindFont(System.Drawing.Graphics g, string longString, Size Room, Font PreferedFont,FontStyle fontstyle=FontStyle.Regular)
        {
            SizeF RealSize = g.MeasureString(longString, PreferedFont);
            float HeightScaleRatio = Room.Height / RealSize.Height;
            float WidthScaleRatio = Room.Width / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = PreferedFont.Size * ScaleRatio;
            return new Font(PreferedFont.FontFamily, ScaleFontSize,fontstyle);
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
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
        
        public static Tuple<int,int> GetWatermarkXY(int imagewidth,int imageheight,int watermarkwidth,int watermarkheight,int paddingx, int paddingy,string position)
        {
            var result = new Tuple<int, int>(0, 0);
            switch (position)
            {
                case "top|right":
                    result = new Tuple<int, int>(imagewidth-watermarkwidth-paddingx, paddingy);
                    break;
                case "top|center":
                    result = new Tuple<int, int>((imagewidth - watermarkwidth - paddingx)/2, paddingy);
                    break;
                case "top|left":
                    result = new Tuple<int, int>(paddingx, paddingy);
                    break;
                case "bottom|left":
                    result = new Tuple<int, int>(paddingx, imageheight-watermarkheight- paddingy);
                    break;
                case "bottom|center":
                    result = new Tuple<int, int>((imagewidth - watermarkwidth - paddingx) / 2, imageheight - watermarkheight - paddingy);
                    break;
                case "bottom|right":
                    result = new Tuple<int, int>(imagewidth - watermarkwidth - paddingx, imageheight - watermarkheight - paddingy);
                    break;
                case "center|center":
                    result = new Tuple<int, int>((imagewidth - watermarkwidth - paddingx) / 2, (imageheight - watermarkheight - paddingy) /2);
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}