using System.Drawing;

namespace Stuff
{
	/// <summary>
	/// Some methods to work with images
	/// </summary>
	public class ImagingPlus
	{
		/// <summary>
		/// Get a Bitmap object that represents area in source that bounded with given rectangle
		/// Note: You need to save source bitmap not to lose outputted image
		/// </summary>
		/// <param name="source">Source bitmap</param>
		/// <param name="x">Left bound of rectangle</param>
		/// <param name="y">Top bound of rectangle</param>
		/// <param name="w">Width of rectangle</param>
		/// <param name="h">Height of rectangle</param>
		/// <returns>Target bitmap</returns>
		public static Bitmap GetClip(Bitmap source, int x, int y, int w, int h)
		{
			return GetClip(source, new Rectangle(x, y, w, h));
		}
		/// <summary>
		/// Get a Bitmap object that represents area in source that bounded with given rectangle
		/// Note: You need to save source bitmap not to lose outputted image
		/// </summary>
		/// <param name="source">Source bitmap</param>
		/// <param name="clip">Rectangle that need to be clipped</param>
		/// <returns>Target bitmap</returns>
		public static Bitmap GetClip(Bitmap source, Rectangle clip)
		{
			var sourceData = source.LockBits(clip, System.Drawing.Imaging.ImageLockMode.ReadWrite, source.PixelFormat);
			Bitmap result = new Bitmap(clip.Width, clip.Height, sourceData.Stride, source.PixelFormat, sourceData.Scan0);
			source.UnlockBits(sourceData);
			return result;
		}
	}
}
