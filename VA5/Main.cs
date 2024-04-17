
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace VA5
{
    public partial class Main : Form
    {
        double minValue = 0;
        double maxValue = 5;
        public Main()
        {
            InitializeComponent();
        }
        double[] XLagrVal;
        double[] XNewVal;
        double[] YLagrVal;
        double[] YNewVal;

        private void Main_Load(object sender, EventArgs e)
        {

        }
        double find(double x)
        {
            return Math.Pow(Math.E,Math.Sin(x));
        }
        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series[0].Points.Clear();
            chart1.Series[1].Points.Clear();
            chart1.Series[2].Points.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            int numOfNode = Convert.ToInt32(numericUpDown1.Value);
            
            double len = (maxValue - minValue)/numOfNode;
            for (int i = 0; i <=numOfNode; i++) 
            {
                dataGridView1.Columns.Add("X" + i, "");
                dataGridView1.Columns[i].Width = 80;
                dataGridView2.Columns.Add("X" + i, "");
                dataGridView2.Columns[i].Width =80;
            }
            dataGridView1.Columns[0].Width = 20;
            dataGridView1.Rows.Add(3);
            dataGridView1.Rows[0].Cells[0].Value = "X";
            dataGridView1.Rows[1].Cells[0].Value = "Y";
            double[] XN=new double[numOfNode];
            double[] YN = new double[numOfNode];
            for (int i = 1; i <= numOfNode; i++)
            {
                double num = i * len;
                double valtmp = find(num);
                dataGridView1.Rows[0].Cells[i].Value = Convert.ToString(num);
                dataGridView1.Rows[1].Cells[i].Value = Convert.ToString(valtmp);
                XN[i - 1] = num;
                YN[i - 1] =valtmp;
                chart1.Series[0].Points.AddXY(num, valtmp);
            }
            double[] r = Newton(XN, YN);
            dataGridView1.Rows[2].Cells[0].Value = "Ньютон";
            for (int i = 0; i < r.Length; i++)
            {
                dataGridView1.Rows[2].Cells[i+1].Value = Convert.ToString(r[i]);
                chart1.Series[1].Points.AddXY(XN[i], r[i]);
            }

            dataGridView2.Rows.Add(3);
            dataGridView2.Columns[0].Width = 20;
            dataGridView2.Rows[0].Cells[0].Value = "X";
            dataGridView2.Rows[1].Cells[0].Value = "Y";
            Random rnd = new Random();
            SortedSet<double> nodes = new SortedSet<double>();
            while (nodes.Count < numOfNode)
            {
                nodes.Add(rnd.NextDouble()+rnd.Next(0, 5));
            }
            double[] XL = new double[numOfNode];
            double[] YL = new double[numOfNode];
            for (int i = 1; i <= numOfNode; i++)
            {
                double num = nodes.ElementAt(i - 1);
                double valtmp = find(num);
                dataGridView2.Rows[0].Cells[i].Value = Convert.ToString(num);
                dataGridView2.Rows[1].Cells[i].Value = Convert.ToString(valtmp);
                XL[i - 1] = num;
                YL[i - 1] = valtmp;
            }
            r = Lagrange(XL, YL);
            dataGridView2.Rows[2].Cells[0].Value = "Лагранж";
            for (int i = 0; i < r.Length; i++)
            {
                dataGridView2.Rows[2].Cells[i+1].Value = Convert.ToString(r[i]);
                chart1.Series[2].Points.AddXY(XL[i], r[i]);
            }
            XLagrVal = XL;
            XNewVal= XN;
            YLagrVal=YL;
            YNewVal= YN;

        }

        double lang(double[] X, int num, double x)
        {
            double up = 1;
            double down = 1;
            for(int i = 0; i < X.Length; i++) 
            {
                if (i == num) continue;
                up *= x - X[i];
                down *= X[num] - X[i];
            }
            return up/down;
        }
        double Lagrange(double[] XL, double[] YL,double x )
        {
            double tmp = 0;
            for (int i = 0; i < YL.Length; i++)
            {
                tmp += YL[i] * lang(XL, i, x);
            }
            return tmp;
        }
        double[] Lagrange(double[] XL, double[] YL)
        {
            if (XL.Length != YL.Length) throw new Exception("Некорректные входные данные");
            double[] val = new double[XL.Length];
            for (int j = 0; j < XL.Length; j++)
            {

                val[j] = Lagrange(XL,YL,XL[j]);
            }
            return val;
        }

        double Minus(double[] Yarray, int pow, int ind)
        {
            double sum = Yarray[ind + pow];
            double tmp = 1;
            int i = 0;
            while (i<pow+1)
            {
                double up = pow;
                for (int j = 1; j <= i; j++)
                {
                    up *= (pow - j);
                }
                if (up == 0) break;
                up *= Yarray[ind + pow - (i + 1)];
                double down =1;
                for (int j = 2; j <= i+1; j++)
                {
                    down *= j;
                }
                tmp = up/down;
                if(i%2== 0) 
                    sum -= tmp;
                else 
                    sum += tmp;
                i++;
            }
            return sum;
        }
        double Newton(double[] Xarray,double[] Yarray,double x) 
        {
            double val = Yarray[0];
            double h = Xarray[1] - Xarray[0];
            double tmp = 1;
            double t = (x - Xarray[0]) / h;
            int i = 0;
            while (i<t)
            {
                tmp = Minus(Yarray, i+1 , 0);
                
                double up = t;
                for (int j = 1; j <= i; j++)
                {
                    up *= t - j;
                }
                double down = 1;
                for (int j = 2; j <= i + 1; j++)
                {
                    down *= j;
                }
                tmp *= up / down;
                val += tmp;
                i++;
            }
            return val;
        }
        double NewtonB(double[] Xarray, double[] Yarray, double x)
        {
            int n = Xarray.Length - 1;
            double val = Yarray[n];
            double h = Xarray[1] - Xarray[0];
            double t = (x - Xarray[n]) / h;
            double tmp = 1;
            int i = 0;
            while (i<Math.Abs( t))
            {
                tmp = Minus(Yarray, i+1, n-(i+1));
                double up = t;
                for (int j = 1; j <= i; j++)
                {
                    up *= t + j;
                }
                double down = 1;
                for (int j = 2; j <= i + 1; j++)
                {
                    down *= j;
                }
                tmp *= up / down;
                val += tmp;
                i++;
            }
            return val;
        }
        double[] Newton(double[] Xarray, double[] Yarray)
        {
            double[] rezult = new double[Yarray.Length];
            for (int k = 0; k < Yarray.Length/2; k++)
            {
                rezult[k] = Newton(Xarray,Yarray,Xarray[k]);
                //if (k != 0) rezult[k] -= rezult[k-1];
            }
            for (int k = Yarray.Length / 2; k < Yarray.Length ; k++)
            {
                rezult[k] = NewtonB(Xarray, Yarray, Xarray[k]);
            }
            return rezult;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (XNewVal == null || YNewVal == null)
            {
                MessageBox.Show("Нет промежутков", "Warning");
                return;
            }
            double num = Convert.ToDouble(numericUpDown2.Value);
            double tmpFind = find(num);
            labelFind.Text =tmpFind.ToString();
            double tmpNew = Newton(XNewVal, YNewVal, num);
            labelValueNewton.Text =Convert.ToString(tmpNew);
            double tmpLag =Lagrange(XLagrVal, YLagrVal,num);
            labelValueLangange.Text = tmpLag.ToString();
            double PerNew = Math.Abs((tmpFind - tmpNew)/tmpFind*100);
            labelPerErrNewton.Text = PerNew.ToString() + "%";
            double PerLag =Math.Abs( (tmpFind - tmpLag)/tmpFind*100);
            labelPerErrLangrange.Text = PerLag.ToString()+"%";
        }
    }
}
