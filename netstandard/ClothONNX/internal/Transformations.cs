namespace ClothONNX
{
    /// <summary>
    /// Using for UNet transformations.
    /// </summary>
    internal static class Transformations
    {
        /// <summary>
        /// Returns resized matrix.
        /// </summary>
        /// <param name="input">Matrix</param>
        /// <param name="h">Height</param>
        /// <param name="w">Width</param>
        /// <returns>Matrix</returns>
        public static float[,] ResizeBilinear(this float[,] input, int h, int w)
        {
            // get source size
            int width = input.GetLength(1);
            int height = input.GetLength(0);

            float xFactor = (float)width / w;
            float yFactor = (float)height / h;

            // width and height decreased by 1
            int ymax = height - 1;
            int xmax = width - 1;

            // output
            float[,] H = new float[h, w];

            // for each line
            for (int y = 0; y < h; y++)
            {
                // Y coordinates
                double oy = (double)y * yFactor;
                int oy1 = (int)oy;
                int oy2 = (oy1 == ymax) ? oy1 : oy1 + 1;
                double dy1 = oy - (double)oy1;
                double dy2 = 1.0 - dy1;

                // for each pixel
                for (int x = 0; x < w; x++)
                {
                    // X coordinates
                    double ox = (double)x * xFactor;
                    int ox1 = (int)ox;
                    int ox2 = (ox1 == xmax) ? ox1 : ox1 + 1;
                    double dx1 = ox - (double)ox1;
                    double dx2 = 1.0 - dx1;

                    // get four points
                    var p1 = input[oy1, ox1];
                    var p2 = input[oy1, ox2];
                    var p3 = input[oy2, ox1];
                    var p4 = input[oy2, ox2];

                    // interpolate using 4 points
                    H[y, x] = (float)(
                            dy2 * (dx2 * p1 + dx1 * p2) +
                            dy1 * (dx2 * p3 + dx1 * p4));
                }
            }

            return H;
        }
    }
}
