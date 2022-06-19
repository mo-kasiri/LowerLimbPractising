using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;
using Coding4Fun.Kinect.Wpf;

namespace ShapeNinjaWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        

        //here is my kinect code
        private KinectSensor sensor;
        private Shape currentShape;
        private ColorImagePoint lastPoint;
        private int recognizeCount = 0;
        private Shape lastMarker;
        private int scoreCounter = 0;
        private int number = 1;
        private int LevelCounterCounter = 1;
        private int popUpShowUpCounter = 0;

        //sound variables
        private MediaPlayer playScoreSound = new MediaPlayer();
        private MediaPlayer playBackgroundSound = new MediaPlayer();

        Uri scoreSound;
        Uri backgroundSound;
        

        public MainWindow()
        {
            InitializeComponent();
            //startGame();
            scoreSound = new Uri("pack://siteoforigin:,,,/sounds/ScoreSound.wav");
            backgroundSound = new Uri("pack://siteoforigin:,,,/sounds/BackgroundMusic.mp3");

           
            

        }
        
        private void StartStopButton_Click(object sender, RoutedEventArgs e)
        {
            playBackgroundSound.Open(backgroundSound);
            playBackgroundSound.Play();
            if (StartStopButton.Content.ToString() == "Start")
            {
                if (KinectSensor.KinectSensors.Count > 0)
                {
                    KinectSensor.KinectSensors.StatusChanged += (o, args) =>
                                                                    {
                                                                        Status.Content = args.Status.ToString();
                                                                    };
                    sensor = KinectSensor.KinectSensors[0];
                }
                Console.WriteLine("hello the sensor gonna be started soon");
                sensor.Start();
                ConnectionID.Content = sensor.DeviceConnectionId;
                sensor.ColorStream.Enable();
                //currentShape = MakeRectangle();
                MakeCoin();
                //ImageCanvas.Children.Add(currentShape);
                sensor.ColorFrameReady += SensorOnColorFrameReady;
                //here is my code
                sensor.SkeletonStream.Enable();
                /*sensor.SkeletonStream.TrackingMode = SkeletonTrackingMode.Seated;*/
                sensor.AllFramesReady += sensor_AllFramesReady;
                StartStopButton.Content = "Stop";
            }
            else
            {
                if (sensor != null && sensor.IsRunning)
                {
                    scoreCounter = 0;
                    scoreCounterIndicator.Content = scoreCounter;
                    sensor.Stop();
                    StartStopButton.Content = "Start";
                }

                ImageCanvas.Children.Remove(currentShape);
            }
        }


        void sensor_AllFramesReady(object sender, AllFramesReadyEventArgs e)
        {
            using (var frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;
                var skeletons = new Skeleton[frame.SkeletonArrayLength];
                frame.CopySkeletonDataTo(skeletons);
                var skeleton = skeletons.FirstOrDefault(s => s.TrackingState == SkeletonTrackingState.Tracked);
                if (skeleton == null)
                    return;

                //Left foot
                var LeftFootPosition = skeleton.Joints[JointType.FootLeft].Position;
                var LeftFootMapper = new CoordinateMapper(sensor);
                var colorPointLeftFoot = LeftFootMapper.MapSkeletonPointToColorPoint(LeftFootPosition, ColorImageFormat.RgbResolution640x480Fps30);
                //Right foot
                var RightFootPosition = skeleton.Joints[JointType.FootRight].Position;
                var RightFootMapper = new CoordinateMapper(sensor);
                var colorPointRightFoot = LeftFootMapper.MapSkeletonPointToColorPoint(RightFootPosition, ColorImageFormat.RgbResolution640x480Fps30);
                //Right Hand
                var RightHandPosition = skeleton.Joints[JointType.HandRight].Position;
                var RightHandMapper = new CoordinateMapper(sensor);
                var colorPointRightHand = LeftFootMapper.MapSkeletonPointToColorPoint(RightHandPosition, ColorImageFormat.RgbResolution640x480Fps30);
                //Left Hand
                var LeftHandPosition = skeleton.Joints[JointType.HandLeft].Position;
                var LeftHandMapper = new CoordinateMapper(sensor);
                var colorPointLeftHand = LeftFootMapper.MapSkeletonPointToColorPoint(LeftHandPosition, ColorImageFormat.RgbResolution640x480Fps30);
                //Spine
                var spinePosition = skeleton.Joints[JointType.Spine].Position;
                var spineMapper = new CoordinateMapper(sensor);
                var colorPointSpine = LeftFootMapper.MapSkeletonPointToColorPoint(spinePosition, ColorImageFormat.RgbResolution640x480Fps30);
                //Knee Left
                var kneeLeftPosition = skeleton.Joints[JointType.KneeLeft].Position;
                var kneeLeftMapper = new CoordinateMapper(sensor);
                var colorPointKneeLeft = LeftFootMapper.MapSkeletonPointToColorPoint(kneeLeftPosition, ColorImageFormat.RgbResolution640x480Fps30);
                //Knee right
                var kneeRightPosition = skeleton.Joints[JointType.KneeRight].Position;
                var kneeRightMapper = new CoordinateMapper(sensor);
                var colorPointKneeRight = LeftFootMapper.MapSkeletonPointToColorPoint(kneeRightPosition, ColorImageFormat.RgbResolution640x480Fps30);
                //Hip Left
                var hipLeftPosition = skeleton.Joints[JointType.HipLeft].Position;
                var hipLeftMapper = new CoordinateMapper(sensor);
                var colorPointHipLeft = LeftFootMapper.MapSkeletonPointToColorPoint(hipLeftPosition, ColorImageFormat.RgbResolution640x480Fps30);
                //Hip Left
                var hipRightPosition = skeleton.Joints[JointType.HipRight].Position;
                var hipRightMapper = new CoordinateMapper(sensor);
                var colorPointHipRight = LeftFootMapper.MapSkeletonPointToColorPoint(hipRightPosition, ColorImageFormat.RgbResolution640x480Fps30);
                //head
                var position = skeleton.Joints[JointType.Head].Position;
                var mapper = new CoordinateMapper(sensor);
                var colorPoint = mapper.MapSkeletonPointToColorPoint(position, ColorImageFormat.RgbResolution640x480Fps30);
                //Console.WriteLine("Left foot position: {0},{1}", colorPointLeftFoot.X*2, colorPointLeftFoot.Y*2);

                var circle = CreateCircle(colorPointSpine);
                //ImageCanvas.Children.Add(circle);
                

                //my code for skeleton mappping for character******************************************
                Canvas.SetLeft(head,colorPoint.X*2 - 60);
                Canvas.SetTop(head,colorPoint.Y*2 - 55);
                Canvas.SetLeft(spine, colorPointSpine.X * 2 - 150);
                Canvas.SetTop(spine, colorPointSpine.Y * 2 - 150);
                Canvas.SetLeft(rightHand, colorPointRightHand.X * 2 - 25);
                Canvas.SetTop(rightHand, colorPointRightHand.Y * 2 - 25);
                Canvas.SetLeft(leftHand, colorPointLeftHand.X * 2 - 25);
                Canvas.SetTop(leftHand, colorPointLeftHand.Y * 2 - 25);
                Canvas.SetLeft(leftFoot, colorPointLeftFoot.X * 2 - 25);
                Canvas.SetTop(leftFoot, colorPointLeftFoot.Y * 2 - 25);
                Canvas.SetLeft(rightFoot, colorPointRightFoot.X * 2 - 25);
                Canvas.SetTop(rightFoot, colorPointRightFoot.Y * 2 - 25);

                Canvas.SetLeft(kneeRight, colorPointKneeRight.X * 2 - 75);
                Canvas.SetTop(kneeRight, colorPointKneeRight.Y * 2);
                Canvas.SetLeft(kneeLeft, colorPointKneeLeft.X * 2 - 75);
                Canvas.SetTop(kneeLeft, colorPointKneeLeft.Y * 2);

                Canvas.SetLeft(hipRight, colorPointHipRight.X * 2 - 55);
                Canvas.SetTop(hipRight, colorPointHipRight.Y * 2 + 50);
                Canvas.SetLeft(hipLeft, colorPointHipLeft.X * 2 - 95);
                Canvas.SetTop(hipLeft, colorPointHipLeft.Y * 2 + 50);

                //my code for skeleton mappping for character******************************************

                //DetectChop(colorPoint, circle);
                DetectStrike(colorPoint, circle);
                
            }
        }

        private void DetectStrike(ColorImagePoint colorPoint, Shape circle)
        {
            //detectig strike of ship to the coins in lvl 1
            if (colorPoint.X*2 > Canvas.GetLeft(coinName)
                && (colorPoint.X*2 < Canvas.GetLeft(coinName) + coinName.Width)
                && colorPoint.Y*2 < Canvas.GetTop(coinName) + 120 && LevelCounterCounter == 1)
            {
                playScoreSound.Open(scoreSound);
                playScoreSound.Play();
                scoreCounter++;
                scoreCounterIndicator.Content = scoreCounter;
                //ImageCanvas.Children.Remove(currentShape);
                MakeCoin();
            }
            //detecting strike for level 2
            if (colorPoint.X * 2 > Canvas.GetLeft(coinName)
                && (colorPoint.X * 2 < Canvas.GetLeft(coinName) + coinName.Width)
                && colorPoint.Y * 2 < Canvas.GetTop(coinName) + 120 && LevelCounterCounter == 2)
            {
                playScoreSound.Open(scoreSound);
                playScoreSound.Play();
                scoreCounter++;
                scoreCounterIndicator.Content = scoreCounter;
                //ImageCanvas.Children.Remove(currentShape);
                MakeCoin();
                if(number % 2 != 0)
                    Canvas.SetLeft(coinName, 1100);
                else
                    Canvas.SetLeft(coinName, 100);
            }

            //detecting strike of ship to the stars lvl3 
            if (colorPoint.X * 2 > Canvas.GetLeft(star) && colorPoint.X * 2 < Canvas.GetLeft(star)+150 &&
                colorPoint.Y*2 > Canvas.GetTop(star) && colorPoint.Y * 2 < Canvas.GetTop(star)+150 && LevelCounterCounter == 3)
            {
                playScoreSound.Open(scoreSound);
                playScoreSound.Play();
                scoreCounter++;
                scoreCounterIndicator.Content = scoreCounter;
                //ImageCanvas.Children.Remove(currentShape);
                makeStar();
            }

            //dectiong strike for level 4
            if (colorPoint.X * 2 > Canvas.GetLeft(star) && colorPoint.X * 2 < Canvas.GetLeft(star) + 150 &&
                colorPoint.Y * 2 > Canvas.GetTop(star) && colorPoint.Y * 2 < Canvas.GetTop(star) + 150 && LevelCounterCounter == 4)
            {
                playScoreSound.Open(scoreSound);
                playScoreSound.Play();
                scoreCounter++;
                scoreCounterIndicator.Content = scoreCounter;
                //ImageCanvas.Children.Remove(currentShape);
                makeStar();
            }

        }

        private void DetectChop(ColorImagePoint colorPoint, Shape circle)
        {
            if (lastMarker != null)
            {
                ImageCanvas.Children.Remove(lastMarker);
            }
            if (recognizeCount == 0 && colorPoint.X > Canvas.GetLeft(currentShape)
                && (colorPoint.X < Canvas.GetLeft(currentShape) + currentShape.Width)
                && colorPoint.Y > Canvas.GetTop(currentShape))
            {
                lastPoint = colorPoint;
                recognizeCount = 1;
                lastMarker = circle;
                ImageCanvas.Children.Add(circle);
                return;
            }
            if (recognizeCount > 0 && colorPoint.X > Canvas.GetLeft(currentShape)
                && (colorPoint.X < Canvas.GetLeft(currentShape) + currentShape.Width)
                && colorPoint.Y > lastPoint.Y)
            {
                recognizeCount++;
                ImageCanvas.Children.Add(circle);
                lastMarker = circle;
            }
            else
            {
                recognizeCount = 0;
            }
            if (recognizeCount > 4)
            {

                ImageCanvas.Children.Remove(currentShape);
                scoreCounter++;
                scoreCounterIndicator.Content = scoreCounter;
                currentShape = MakeRectangle();
                ImageCanvas.Children.Add(currentShape);
                recognizeCount = 0;
            }
        }

        private Shape CreateCircle(ColorImagePoint colorPoint)
        {
            var circle = new Ellipse();
            circle.Fill = Brushes.Red;
            circle.Height = 10;
            circle.Width = 10;
            circle.Opacity = 0.2;
            circle.Stroke = Brushes.Red;
            circle.StrokeThickness = 2;

            Canvas.SetLeft(circle, colorPoint.X*2);
            Canvas.SetTop(circle, colorPoint.Y*2);
            return circle;
        }

        //create coin shape
        private void MakeCoin()
        {
            var random = new Random();
            int randomLeftMargin = random.Next((int)(ImageCanvas.Width - 40));
            
                    //centeralizing the coin
                    if (randomLeftMargin > 1000)
                    {
                        Canvas.SetLeft(coinName, randomLeftMargin - 200);
                        Canvas.SetTop(coinName, 80);
                        
                    }
                    else if (randomLeftMargin < 200)
                    {
                        Canvas.SetLeft(coinName, randomLeftMargin + 200);
                        Canvas.SetTop(coinName, 80);
                        
                    }
                    else
                    {
                        Canvas.SetLeft(coinName, randomLeftMargin);
                        Canvas.SetTop(coinName, 80);
                        
                    }
        }

        private void makeStar()
        {
            if (number%2 == 0 && LevelCounterCounter == 3)
            {
                Canvas.SetLeft(star,750);
                Canvas.SetTop(star, -150);
                number++;
            }
            else if(number%2 == 1 && LevelCounterCounter == 3)
            {
                Canvas.SetLeft(star, 350);
                Canvas.SetTop(star, -150);
                number++;
            }
            else if(number%2 == 0 && LevelCounterCounter == 4)
            {
                Canvas.SetLeft(star, 10);
                Canvas.SetTop(star, 200);
                number++;
            }
            else if(number % 2 == 1 && LevelCounterCounter == 4)
            {
                Canvas.SetLeft(star, 1000);
                Canvas.SetTop(star, 500);
                number++;
            }
            
        }

        private Rectangle MakeRectangle()
        {
            var rectangle = new Rectangle();
            rectangle.Stroke = Brushes.Blue;
            rectangle.Width = 120;
            rectangle.Height = 120;
            rectangle.StrokeThickness = 1;
            rectangle.Fill = Brushes.Yellow;          
            

            var random = new Random();
            int randomLeftMargin = random.Next((int)(ImageCanvas.Width - 40));
           // if(randomLeftMargin )
            Console.WriteLine("The Left margin is:{0}",randomLeftMargin);
            //Canvas.SetTop(rectangle, random.Next((int) (ImageCanvas.Height - 100)));

            //centeralizing the rectangle
            if (randomLeftMargin > 1000)
            {
                Canvas.SetLeft(rectangle, randomLeftMargin - 200);
                Canvas.SetTop(rectangle, 80);
                return rectangle;
            }
            else if(randomLeftMargin < 200)
            {
                Canvas.SetLeft(rectangle, randomLeftMargin + 200);
                Canvas.SetTop(rectangle, 80);
                return rectangle;
            }
            else
            {
                Canvas.SetLeft(rectangle, randomLeftMargin);
                Canvas.SetTop(rectangle, 80);
                return rectangle;
            }
            
        }

        public async void popUpShowUp()
        {
            //level 2
            if (popUpShowUpCounter == 0 && scoreCounter == 2)
            {
                popUpShowUpCounter++;
                sensor.Stop();
                popUp.Opacity = 1;
                await Task.Delay(2000);
                popUp.Opacity = 0;
                sensor.Start();
            }
            //level 3
            if (popUpShowUpCounter == 1 && scoreCounter == 5)
            {
                ImageCanvas.Children.Remove(coinName);
                makeStar();
                popUpShowUpCounter++;
                sensor.Stop();
                popUp.Opacity = 1;
                await Task.Delay(2000);
                popUp.Opacity = 0;
                sensor.Start();
            }
            if(popUpShowUpCounter == 2 && scoreCounter == 10)
            {
                //ImageCanvas.Children.Remove(star);
                makeStar();
                popUpShowUpCounter++;
                sensor.Stop();
                popUp.Opacity = 1;
                await Task.Delay(2000);
                popUp.Opacity = 0;
                sensor.Start();
            }
        }

        private void SensorOnColorFrameReady(object sender, ColorImageFrameReadyEventArgs colorImageFrameReadyEventArgs)
        {
            using (var frame = colorImageFrameReadyEventArgs.OpenColorImageFrame())
            {


                //*************************************************************************my code

                //initiating the levels
                if(scoreCounter == 2)
                {
                    popUpShowUp();
                    popUpText.Content = "Level 2 is starting";         
                    LevelCounterCounter = 2;
                    LevelCounter.Content = "Level 2";

                }
                if(scoreCounter == 5)
                {
                    popUpShowUp();
                    popUpText.Content = "Level 3 is starting";
                    LevelCounterCounter = 3;
                    LevelCounter.Content = "Level 3";
                }
                if (scoreCounter == 10)
                {
                    popUpShowUp();
                    popUpText.Content = "Level 4 is starting";
                    LevelCounterCounter = 4;
                    LevelCounter.Content = "Level 4";
                }
                if (scoreCounter == 20)
                {
                    LevelCounterCounter = 5;
                    LevelCounter.Content = "Level 5";
                }


                //defining levels

                //level 2
                if (LevelCounterCounter == 2 && number % 2 == 0)
                {
                    //Console.WriteLine(Canvas.GetLeft(coinName));
                    Canvas.SetLeft(coinName, Canvas.GetLeft(coinName) - 20);
                }else if (LevelCounterCounter == 2  && number % 2 != 0)
                {
                    Canvas.SetLeft(coinName, Canvas.GetLeft(coinName) + 20);
                }
                
                //level 3
                if (LevelCounterCounter == 3)
                {                
                    Canvas.SetTop(star, Canvas.GetTop(star) + 11);
                    if(Canvas.GetTop(star) > 900)
                    {
                        makeStar();
                    }
                }

                //level 4
                if (LevelCounterCounter == 4)
                {
                    if(number % 2 == 1)
                    {
                        Canvas.SetLeft(star, Canvas.GetLeft(star) + 11);
                        if (Canvas.GetLeft(star) < 0)
                        {
                            makeStar();
                        }else if(Canvas.GetLeft(star) > 1000)
                        {
                            makeStar();
                        }
                    }
                    else
                    {
                        Canvas.SetLeft(star, Canvas.GetLeft(star) - 11);
                        if (Canvas.GetLeft(star) > 1200)
                        {
                            makeStar();
                        }
                        else if (Canvas.GetLeft(star) < 0)
                        {
                            makeStar();
                        }
                    }
                    
                }
                //Console.WriteLine("**************"+LevelCounterCounter);


                //setting the position of coin in level 1
                if (Canvas.GetLeft(coinName) > 1100)
                {
                    Canvas.SetLeft(coinName, 1100);
                    number++;
                }
                if (Canvas.GetLeft(coinName) < 100)
                {
                    Canvas.SetLeft(coinName, 100);
                    number++;
                }

               










                //*************************************************************************my code

                var bitmap = CreateBitmap(frame);
                //ImageCanvas.Background = new ImageBrush(frame.ToBitmapSource());
                ImageCanvas.Background = new ImageBrush(bitmap);
            }
        }

        private static BitmapSource CreateBitmap(ColorImageFrame frame)
        {
            var pixelData = new byte[frame.PixelDataLength];
            frame.CopyPixelDataTo(pixelData);

            //GrayscaleData(pixelData);

            var stride = frame.Width * frame.BytesPerPixel;
            var bitmap = BitmapSource.Create(frame.Width, frame.Height, 96, 96, PixelFormats.Bgr32, null, pixelData,
                                             stride);
            return bitmap;
        }
    }
}
