using System;
using System.Drawing;
using UMapx.Core;

namespace ObjectONNX
{
    /// <summary>
    /// Defines cloth segmentator interface.
    /// </summary>
    public interface IClothSegmentator : IDisposable
    {
        #region Interface

        /// <summary>
        /// Returns cloth segmentation results.
        /// </summary>
        /// <param name="image">Bitmap</param>
        /// <param name="interpolationMode">Interpolation mode</param>
        /// <returns>Result</returns>
        float[,] Forward(Bitmap image, InterpolationMode interpolationMode = InterpolationMode.Bilinear);

        /// <summary>
        /// Returns cloth segmentation results.
        /// </summary>
        /// <param name="image">Image in BGR terms</param>
        /// <param name="interpolationMode">Interpolation mode</param>
        /// <returns>Result</returns>
        float[,] Forward(float[][,] image, InterpolationMode interpolationMode = InterpolationMode.Bilinear);

        #endregion
    }
}
