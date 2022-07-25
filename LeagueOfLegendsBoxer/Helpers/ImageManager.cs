using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LeagueOfLegendsBoxer.Helpers
{
    public class ImageManager
    {
        bool ColorAEqualColorB(Color colorA, Color colorB, byte errorRange = 10)
        {
            return colorA.A <= colorB.A + errorRange && colorA.A >= colorB.A - errorRange &&
                colorA.R <= colorB.R + errorRange && colorA.R >= colorB.R - errorRange &&
                colorA.G <= colorB.G + errorRange && colorA.G >= colorB.G - errorRange &&
                colorA.B <= colorB.B + errorRange && colorA.B >= colorB.B - errorRange;

        }
        bool ListContainsPoint(List<Point> listPoint, Point point, double errorRange = 10)
        {
            bool isExist = false;
            foreach (var item in listPoint)
            {
                if (item.X <= point.X + errorRange && item.X >= point.X - errorRange && item.Y <= point.Y + errorRange && item.Y >= point.Y - errorRange)
                {
                    isExist = true;
                }
            }
            return isExist;
        }

        #region 找图

        /// <summary>
        /// 查找图片，不能镂空
        /// </summary>
        /// <param name="subPic"></param>
        /// <param name="parPic"></param>
        /// <param name="searchRect">如果为empty，则默认查找整个图像</param>
        /// <param name="errorRange">容错，单个色值范围内视为正确0~255</param>
        /// <param name="matchRate">图片匹配度，默认90%</param>
        /// <param name="isFindAll">是否查找所有相似的图片</param>
        /// <returns>返回查找到的图片的中心点坐标</returns>
        public List<Point> FindPicture(string subPic, string parPic, Rectangle searchRect, byte errorRange, double matchRate = 0.9, bool isFindAll = false)
        {
            List<Point> ListPoint = new List<Point>();
            var subBitmap = new Bitmap(subPic);
            var parBitmap = new Bitmap(parPic);
            int subWidth = subBitmap.Width;
            int subHeight = subBitmap.Height;
            int parWidth = parBitmap.Width;
            int parHeight = parBitmap.Height;
            if (searchRect.IsEmpty)
            {
                searchRect = new Rectangle(0, 0, parBitmap.Width, parBitmap.Height);
            }

            var searchLeftTop = searchRect.Location;
            var searchSize = searchRect.Size;
            Color startPixelColor = subBitmap.GetPixel(0, 0);
            var subData = subBitmap.LockBits(new Rectangle(0, 0, subBitmap.Width, subBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var parData = parBitmap.LockBits(new Rectangle(0, 0, parBitmap.Width, parBitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var byteArrarySub = new byte[subData.Stride * subData.Height];
            var byteArraryPar = new byte[parData.Stride * parData.Height];
            Marshal.Copy(subData.Scan0, byteArrarySub, 0, subData.Stride * subData.Height);
            Marshal.Copy(parData.Scan0, byteArraryPar, 0, parData.Stride * parData.Height);

            var iMax = searchLeftTop.Y + searchSize.Height - subData.Height;//行
            var jMax = searchLeftTop.X + searchSize.Width - subData.Width;//列

            int smallOffsetX = 0, smallOffsetY = 0;
            int smallStartX = 0;
            int pointX = -1; int pointY = -1;
            for (int i = searchLeftTop.Y; i < iMax; i++)
            {
                for (int j = searchLeftTop.X; j < jMax; j++)
                {
                    //大图x，y坐标处的颜色值
                    int x = j, y = i;
                    int parIndex = i * parWidth * 4 + j * 4;
                    var colorBig = Color.FromArgb(byteArraryPar[parIndex + 3], byteArraryPar[parIndex + 2], byteArraryPar[parIndex + 1], byteArraryPar[parIndex]);
                    ;
                    if (ColorAEqualColorB(colorBig, startPixelColor, errorRange))
                    {
                        smallStartX = x - smallOffsetX;//待找的图X坐标
                        int smallStartY = y - smallOffsetY;
                        int sum = 0;//所有需要比对的有效点
                        int matchNum = 0;//成功匹配的点
                        for (int m = 0; m < subHeight; m++)
                        {
                            for (int n = 0; n < subWidth; n++)
                            {
                                int x1 = n, y1 = m;
                                int subIndex = m * subWidth * 4 + n * 4;
                                var color = Color.FromArgb(byteArrarySub[subIndex + 3], byteArrarySub[subIndex + 2], byteArrarySub[subIndex + 1], byteArrarySub[subIndex]);

                                sum++;
                                int x2 = smallStartX + x1, y2 = smallStartY + y1;
                                int parReleativeIndex = y2 * parWidth * 4 + x2 * 4;//比对大图对应的像素点的颜色
                                var colorPixel = Color.FromArgb(byteArraryPar[parReleativeIndex + 3], byteArraryPar[parReleativeIndex + 2], byteArraryPar[parReleativeIndex + 1], byteArraryPar[parReleativeIndex]);
                                if (ColorAEqualColorB(colorPixel, color, errorRange))
                                {
                                    matchNum++;
                                }
                            }
                        }
                        if ((double)matchNum / sum >= matchRate)
                        {
                            Console.WriteLine((double)matchNum / sum);
                            pointX = smallStartX + (int)(subWidth / 2.0);
                            pointY = smallStartY + (int)(subHeight / 2.0);
                            var point = new Point(pointX, pointY);
                            if (!ListContainsPoint(ListPoint, point, 10))
                            {
                                ListPoint.Add(point);
                            }
                            if (!isFindAll)
                            {
                                goto FIND_END;
                            }
                        }
                    }
                    //小图x1,y1坐标处的颜色值
                }
            }
        FIND_END:
            subBitmap.UnlockBits(subData);
            parBitmap.UnlockBits(parData);
            subBitmap.Dispose();
            parBitmap.Dispose();
            GC.Collect();
            return ListPoint;
        }
        #endregion
    }
}
