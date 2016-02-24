using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        Graphics graph;
        Random rnd = new Random();
        List<Points> pointList = new List<Points>();
        List<Points> classList = new List<Points>();
        List<Points>[] pointInClassList = new List<Points>[20];

        public Form1()
        {
            InitializeComponent();
            graph = pictureBox1.CreateGraphics();
        }

        private void RandomPoints(int obj, int cls)
        {
            int j = 0;
            for (int i = 0; i < obj; i++)
            {
                pointList.Add(new Points(rnd.Next(0, this.pictureBox1.Width), rnd.Next(0, this.pictureBox1.Height)));

                if (j < cls)
                {
                    j++;
                    classList.Add(new Points(rnd.Next(0, this.pictureBox1.Width), rnd.Next(0, this.pictureBox1.Height)));
                }
            }
        }

        private void RandomColors(Points[] p)
        {
            var j = 0;
            foreach (var temp in classList)
            {
                temp.drawPoint(graph, rnd.Next(0, 255));
                p[j] = temp;
                j++;
            }
        }

        private void FillK(Points[] k)
        {
            int ind = 0;
            foreach (var temp in classList)
            {
                k[ind] = temp;
                ind++;
            }
        }

        public double EnterPointLenght(Point point1, Point point2)
        {
            double Lenght;
            var katets = new double[2];
            katets[0] = Math.Abs(point1.Y - point2.Y);
            katets[1] = Math.Abs(point1.X - point2.X);
            Lenght = Math.Sqrt(Math.Pow(katets[0], 2) + Math.Pow(katets[1], 2));
            return Lenght;
        }

        private void RecountKCenter(int n, Points[] k)
        {
            float sumOfElemX, sumOfElemY, sum;
            for (int i = 0; i < n; i++)
            {
                sum = sumOfElemX = sumOfElemY = 0;
                foreach (var temp in pointInClassList[i])
                {
                    sum++;
                    sumOfElemX += temp.X;
                    sumOfElemY += temp.Y;
                }
                k[i].X = (int)(sumOfElemX / sum);
                k[i].Y = (int)(sumOfElemY / sum);
            }
        }

        int ind;

        public void ReplacePoints(int numberOfClasses, int screenWidth, int screenHeight, Graphics graph, Points[] k)
        {
            double minDistance;
            Points point_0 = new Points(0, 0);
            ind = 0;
            foreach (var temp in classList)
            {
                if (pointInClassList[ind] != null)
                    pointInClassList[ind].Clear();
                pointInClassList[ind] = new List<Points>();
                ind++;
            }
            ind = 1;
            foreach (var temp in pointList)
            {
                minDistance = Math.Sqrt((Math.Pow(screenWidth, 2) + Math.Pow(screenHeight, 2)));
                for (int i = 0; i < numberOfClasses; i++)
                {
                    if (Math.Sqrt(Math.Pow((temp.X - k[i].X), 2) + Math.Pow((temp.Y - k[i].Y), 2)) <= minDistance)
                    {
                        minDistance = Math.Sqrt(Math.Pow((temp.X - k[i].X), 2) + Math.Pow((temp.Y - k[i].Y), 2));
                        point_0 = k[i];
                        ind = i;
                    }
                }
                pointInClassList[ind].Add(temp);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int numberOfObjects = int.Parse(this.numericUpDown2.Text);
            int numberOfClasses = int.Parse(this.numericUpDown1.Text);
            int count = 0;
            bool notFinished = true;
            
            var k = new Points[numberOfClasses];
            var k0 = new tempvalue[numberOfClasses];
            var brush = new SolidBrush(Color.Black);
            var point_0 = new Points[numberOfClasses];
            var pt1 = new Point(0, 0);
            var pt2 = new Point(0, 0);
            
            graph.FillRectangle(brush, 0, 0, this.pictureBox1.Width, this.pictureBox1.Height);

            pointList.Clear();
            classList.Clear();

            RandomPoints(numberOfObjects, numberOfClasses);
            RandomColors(point_0);

            FillK(k);

            while (notFinished == true) //k
            {
                count++;

                ReplacePoints(numberOfClasses, this.pictureBox1.Width, this.pictureBox1.Height, graph, k);

                for (int i = 0; i < numberOfClasses; i++) //prev centers
                {
                    k0[i].X = k[i].X;
                    k0[i].Y = k[i].Y;
                }

                RecountKCenter(numberOfClasses, k);
                notFinished = false;

                for (int i = 0; i < numberOfClasses; i++)
                    if (Math.Sqrt(Math.Pow((k0[i].X - k[i].X), 2) + Math.Pow((k0[i].Y - k[i].Y), 2)) > 2)
                        notFinished = true;
   
                for (int i = 0; i < numberOfClasses; i++)
                {
                    if ((count % 8 == 0) || (count == 1))
                        foreach (var temp in pointInClassList[i])
                            temp.drawLink(graph, temp, point_0[i]);
                        
                    graph.DrawEllipse(new Pen(Color.Black, 3), k0[i].X, k0[i].Y, 5, 5);
                    graph.DrawEllipse(new Pen(Color.White, 3), k[i].X, k[i].Y, 5, 5);
                }
            }

            for (int i = 0; i < numberOfClasses; i++)
            {
                if (count % 8 != 0)
                    foreach (var temp in pointInClassList[i])
                        temp.drawLink(graph, temp, point_0[i]);

                graph.DrawEllipse(new Pen(Color.Black, 3), k0[i].X, k0[i].Y, 5, 5);
                graph.DrawEllipse(new Pen(Color.White, 3), k[i].X, k[i].Y, 5, 5);
            }
            MessageBox.Show("Done!");
        }
    }
}