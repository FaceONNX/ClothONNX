using ClothONNX.Properties;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using ObjectONNX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UMapx.Core;
using UMapx.Imaging;

namespace ClothONNX
{
    /// <summary>
    /// Defines cloth segmentator.
    /// </summary>
    public class ClothSegmentator : IClothSegmentator
    {
        #region Private data

        /// <summary>
        /// Inference session.
        /// </summary>
        private readonly InferenceSession _session;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes cloth segmentator.
        /// <param name="clothSegmentatorQuality">Cloth segmentator quality</param>
        /// </summary>
        public ClothSegmentator(ClothSegmentatorQuality clothSegmentatorQuality = ClothSegmentatorQuality.Medium)
        {
            ClothSegmentatorQuality = clothSegmentatorQuality;
            _session = new InferenceSession(Resources.cloth_segmentation_unet);
        }

        /// <summary>
        /// Initializes cloth segmentator.
        /// </summary>
        /// <param name="clothSegmentatorQuality">Cloth segmentator quality</param>
        /// <param name="options">Session options</param>
        public ClothSegmentator(SessionOptions options, ClothSegmentatorQuality clothSegmentatorQuality = ClothSegmentatorQuality.Medium)
        {
            ClothSegmentatorQuality = clothSegmentatorQuality;
            _session = new InferenceSession(Resources.cloth_segmentation_unet, options);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets cloth segmentator quality.
        /// </summary>
        public ClothSegmentatorQuality ClothSegmentatorQuality { get; set; }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public float[,] Forward(Bitmap image, InterpolationMode interpolationMode = InterpolationMode.Bilinear)
        {
            var rgb = image.ToRGB(false);
            return Forward(rgb, interpolationMode);
        }

        /// <inheritdoc/>
        public float[,] Forward(float[][,] image, InterpolationMode interpolationMode = InterpolationMode.Bilinear)
        {
            if (image.Length != 3)
                throw new ArgumentException("Image must be in BGR terms");

            var width = image[0].GetLength(1);
            var height = image[0].GetLength(0);
            var length = (int)ClothSegmentatorQuality;
            var size = new Size(length, length);

            var resized = new float[3][,];

            for (int i = 0; i < image.Length; i++)
            {
                resized[i] = image[i].ResizePreserved(size.Height, size.Width, 0.0f, interpolationMode);
            }

            var dimentions = new int[] { 1, 3, size.Width, size.Height };
            var inputMeta = _session.InputMetadata;
            var name = inputMeta.Keys.ToArray()[0];

            // preprocessing
            var tensor = new DenseTensor<float>(dimentions);
            var mean = new[] { 0.485f, 0.456f, 0.406f }.Flip();
            var stddev = new[] { 0.229f, 0.224f, 0.225f }.Flip();

            // do job
            for (int i = 0; i < resized.Length; i++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    for (int x = 0; x < size.Width; x++)
                    {
                        // bgr to rgb and apply transform
                        tensor[0, resized.Length - i - 1, x, y] = (resized[i][y, x] - mean[i]) / stddev[i];
                    }
                }
            }

            // session run
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(name, tensor) };
            using var sessionResults = _session.Run(inputs);
            var results = sessionResults?.ToArray();

            // post-proccessing
            var mask = new float[size.Height, size.Width];
            var output = results[0].AsTensor<float>();

            // do job
            for (int j = 0; j < size.Height; j++)
            {
                for (int i = 0; i < size.Width; i++)
                {
                    // apply
                    mask[j, i] = output[0, 0, i, j];
                }
            }

            return mask.ResizePreserved(height, width, interpolationMode);
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _session?.Dispose();
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~ClothSegmentator()
        {
            Dispose(false);
        }

        #endregion
    }
}
