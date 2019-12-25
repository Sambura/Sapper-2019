using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Stuff
{
	public class ImagingPlus
	{
		public static Bitmap GetClip(Bitmap source, int x, int y, int w, int h)
		{
			return GetClip(source, new Rectangle(x, y, w, h));
		}

		public static Bitmap GetClip(Bitmap source, Rectangle clip)
		{
			var sourceData = source.LockBits(clip, System.Drawing.Imaging.ImageLockMode.ReadWrite, source.PixelFormat);
			Bitmap result = new Bitmap(clip.Width, clip.Height, sourceData.Stride, source.PixelFormat, sourceData.Scan0);
			source.UnlockBits(sourceData);
			return result;
		}
	}
}
