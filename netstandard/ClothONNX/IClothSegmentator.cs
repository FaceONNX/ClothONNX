using System;
using System.Drawing;

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
        /// <returns>Result</returns>
        float[,] Forward(Bitmap image);

        /// <summary>
        /// Returns cloth segmentation results.
        /// </summary>
        /// <param name="image">Image in BGR terms</param>
        /// <returns>Result</returns>
        float[,] Forward(float[][,] image);

        #endregion
    }
}
