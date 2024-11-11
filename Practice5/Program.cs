using OpenCvSharp;
using ScottPlot;
using ScottPlot.AxisPanels;
using Generate = MathNet.Numerics.Generate;

namespace Practice5;

public static class MpegEncoder
{
    public static (Mat, Mat) GetFrames(string filename, int firstFrame, int secondFrame)
    {
        var cap = new VideoCapture(filename);
        if (!cap.IsOpened())
            throw new Exception("Unable to open the video file.");

        cap.Set(VideoCaptureProperties.PosFrames, firstFrame);
        var frame1 = new Mat();
        if (!cap.Read(frame1) || frame1.Empty())
            throw new Exception($"Failed to read frame at position {firstFrame}.");

        cap.Set(VideoCaptureProperties.PosFrames, secondFrame);
        var frame2 = new Mat();
        if (!cap.Read(frame2) || frame2.Empty())
            throw new Exception($"Failed to read frame at position {secondFrame}.");

        cap.Release();
        return (frame1, frame2);
    }

    public static double GetBitsPerPixel(Mat img)
    {
        if (img.Empty())
            throw new ArgumentException("Image cannot be empty.");

        var height = img.Height;
        var width = img.Width;
        double bits = 0;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                bits += Math.Log2(Math.Abs(img.At<byte>(y, x)) + 1);
            }
        }

        return bits / (height * width);
    }

    public static Mat GetResidual(Mat target, Mat predicted)
    {
        if (target.Size() != predicted.Size())
            throw new ArgumentException("Target and predicted frames must be the same size.");

        return target - predicted;
    }

    public static Mat GetReconstructTarget(Mat residual, Mat predicted)
    {
        if (residual.Size() != predicted.Size())
            throw new ArgumentException("Residual and predicted frames must be the same size.");

        return residual + predicted;
    }

    public static Mat BlockSearchBody(Mat anchor, Mat target, int blockSize, int searchArea = 7)
    {
        var height = anchor.Rows;
        var width = anchor.Cols;
        var predicted = new Mat(height, width, anchor.Type(), new Scalar(255));

        for (var y = 0; y <= height - blockSize; y += blockSize)
        {
            for (var x = 0; x <= width - blockSize; x += blockSize)
            {
                var targetBlock = target.SubMat(y, y + blockSize, x, x + blockSize);
                var anchorSearchArea = GetAnchorSearchArea(x, y, anchor, blockSize, searchArea);
                var bestMatch = GetBestMatch(targetBlock, anchorSearchArea, blockSize);

                bestMatch.CopyTo(predicted.SubMat(y, y + blockSize, x, x + blockSize));
            }
        }

        return predicted;
    }

    private static Mat GetAnchorSearchArea(int x, int y, Mat anchor, int blockSize, int searchArea)
    {
        var startX = Math.Max(0, x - searchArea);
        var startY = Math.Max(0, y - searchArea);
        var endX = Math.Min(anchor.Cols, x + blockSize + searchArea);
        var endY = Math.Min(anchor.Rows, y + blockSize + searchArea);

        return anchor.SubMat(startY, endY, startX, endX);
    }

    private static Mat GetBestMatch(Mat targetBlock, Mat searchArea, int blockSize)
    {
        var minMad = double.MaxValue;
        var bestMatch = new Mat();

        for (var y = 0; y <= searchArea.Rows - blockSize; y++)
        {
            for (var x = 0; x <= searchArea.Cols - blockSize; x++)
            {
                var anchorBlock = searchArea.SubMat(y, y + blockSize, x, x + blockSize);
                var mad = GetMad(targetBlock, anchorBlock);

                if (mad < minMad)
                {
                    minMad = mad;
                    bestMatch = anchorBlock.Clone();
                }
            }
        }

        return bestMatch;
    }

    private static double GetMad(Mat block1, Mat block2)
    {
        if (block1.Size() != block2.Size())
            throw new ArgumentException("Blocks must be the same size.");

        double sum = 0;
        for (var y = 0; y < block1.Rows; y++)
        {
            for (var x = 0; x < block1.Cols; x++)
            {
                sum += Math.Abs(block1.At<byte>(y, x) - block2.At<byte>(y, x));
            }
        }
        return sum / (block1.Rows * block1.Cols);
    }
    
    public static double[] GetBitsPerPixelPerChannel(Mat img)
    {
        if (img.Empty())
            throw new ArgumentException("Image cannot be empty.");

        var channels = Cv2.Split(img);
        var bppPerChannel = new double[channels.Length];

        for (var i = 0; i < channels.Length; i++)
        {
            bppPerChannel[i] = GetBitsPerPixel(channels[i]);
        }

        return bppPerChannel;
    }
}

internal static class Program
{
    private static void Main()
    {
            var (frame1, frame2) = MpegEncoder.GetFrames(@"C:\Users\leviaweek\Downloads\sample.avi", 400, 401);
            Cv2.ImWrite("Frame1.png", frame1);
            Cv2.ImWrite("Frame2.png", frame2);

            var framesResidual = MpegEncoder.GetResidual(frame2, frame1);
            Cv2.ImWrite("FramesResidual.png", framesResidual);
            
            var predictedFrame = MpegEncoder.BlockSearchBody(frame1, frame2, blockSize: 16, searchArea: 7);
            Cv2.ImWrite("Predicted.png", predictedFrame);

            var residual = MpegEncoder.GetResidual(frame2, predictedFrame);
            Cv2.ImWrite("Residual.png", residual);

            var reconstructed = MpegEncoder.GetReconstructTarget(residual, predictedFrame);
            Cv2.ImWrite("Reconstructed.png", reconstructed);

            var frame1Pixels = MpegEncoder.GetBitsPerPixel(frame1);
            var predictedPixels = MpegEncoder.GetBitsPerPixel(predictedFrame);
            var residualPixels = MpegEncoder.GetBitsPerPixel(residual);
            Console.WriteLine($"Bits per pixel for Frame1: {frame1Pixels}");
            Console.WriteLine($"Bits per pixel for Frame2: {MpegEncoder.GetBitsPerPixel(frame2)}");
            Console.WriteLine($"Bits per pixel for Predicted Frame: {predictedPixels}");
            Console.WriteLine($"Bits per pixel for Residual: {residualPixels}");
            Console.WriteLine($"Bits per pixel for Reconstructed: {MpegEncoder.GetBitsPerPixel(reconstructed)}");

            var bitsAnchor = MpegEncoder.GetBitsPerPixelPerChannel(frame1);
            var bitsDiff = MpegEncoder.GetBitsPerPixelPerChannel(framesResidual);
            var bitsPredicted = MpegEncoder.GetBitsPerPixelPerChannel(residual);

            const double barWidth = 0.25;
            
            double[] p1 = [bitsAnchor.Sum(), bitsAnchor[0], bitsAnchor[1], bitsAnchor[2]];
            double[] diff = [bitsDiff.Sum(), bitsDiff[0], bitsDiff[1], bitsDiff[2]];
            double[] mpeg = [bitsPredicted.Sum(), bitsPredicted[0], bitsPredicted[1], bitsPredicted[2]];

            var br1 = Generate.LinearRange(0, p1.Length);
            var br2 = br1.Select(x => x + barWidth).ToArray();
            var br3 = br2.Select(x => x + barWidth).ToArray();
            
            var fig = new Plot();
            fig.Add.Bars(HistogramToBars(br1, p1, 0.25, Colors.Red));
            
            fig.Add.Bars(HistogramToBars(br2, diff, 0.25, Colors.Green));
            
            fig.Add.Bars(HistogramToBars(br3, mpeg, 0.25, Colors.Blue));

            fig.Legend.Axes = new Axes
            {
                YAxis = new LeftAxis
                {
                    LabelText = "Біт на піксель"
                }
            };

            fig.SavePng("file.png", 1920, 1080);

    }
    
    private static List<Bar> HistogramToBars(double[] bins, double[] counts, double binSize, Color fillColor)
    {
        return counts.Select((t, i) => new Bar
            {
                Position = bins[i],
                Value = t,
                FillColor = fillColor,
                Size = binSize
                
            })
            .ToList();
    }
}
