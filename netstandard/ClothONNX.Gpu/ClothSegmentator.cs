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
        /// </summary>
        public ClothSegmentator()
        {
            _session = new InferenceSession(Resources.cloth_segmentation_unet);
        }

        /// <summary>
        /// Initializes cloth segmentator.
        /// </summary>
        /// <param name="options">Session options</param>
        public ClothSegmentator(SessionOptions options)
        {
            _session = new InferenceSession(Resources.cloth_segmentation_unet, options);
        }

        #endregion

        #region Methods

        /// <inheritdoc/>
        public float[,] Forward(Bitmap image)
        {
            var rgb = image.ToRGB(false);
            return Forward(rgb);
        }

        /// <inheritdoc/>
        public float[,] Forward(float[][,] image)
        {
            if (image.Length != 3)
                throw new ArgumentException("Image must be in BGR terms");

            var width = image[0].GetLength(1);
            var height = image[0].GetLength(0);
            var size = new Size(768, 768);

            var resized = new float[3][,];

            for (int i = 0; i < image.Length; i++)
            {
                resized[i] = image[i].Resize(size.Height, size.Width, InterpolationMode.Bilinear);
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

            return mask.Resize(height, width, InterpolationMode.Bilinear);
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
