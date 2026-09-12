using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace Calculator_Project
{
    public partial class Form1 : Form
    {
        string sOperation = "";
        
        public Form1()
        {

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            lblOperations.Text = string.Empty;
            lblResult.RightToLeft = RightToLeft.No;
            lblResult.TextAlign = ContentAlignment.MiddleRight;
        }
        void PlayButton(Button Btn)
        {
            if(Btn.Tag.ToString()=="C" )
            {
                lblOperations.Text = string.Empty;
                lblResult.Text ="0";

                sOperation = string.Empty;
            }
            else if(Btn.Tag.ToString() == "Del")
            {
                if (lblOperations.Text != string.Empty)
                {
                    sOperation = sOperation.Remove(sOperation.Length - 1);

                    lblOperations.Text = sOperation;
                }
            }
            else if (Btn.Tag.ToString() == "=")
            {
                Queue<string> Q=ConvertToPostfix(lblOperations.Text);
                
                lblResult.Text = CalculatePostfix(Q).ToString();
            }

            else
            {
                sOperation += Btn.Tag.ToString();
                lblOperations.Text = sOperation;
            }
           
        }
        private void Clickbtn(object sender, EventArgs e)
        {
           
            PlayButton((Button)(sender));
        }
        Queue<string> SplitExpression(string text)
        {
            Queue<string> parts = new Queue<string>();

            string number = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == ' ')
                {
                    continue;
                }
                if ((i == 0 && text[i] == '-') || (i > 0 && text[i] == '-' && (text[i - 1] == '-' || text[i - 1] == '+' || text[i - 1] == '*' || text[i - 1] == '/' || text[i - 1] == '(')))
                {
                    number += "-";
                }
                else if (char.IsDigit(text[i]) || text[i] == '.')
                {
                    number += text[i];

                }


                else
                {
                    if (number != "")
                    {
                        parts.Enqueue(number);
                    }

                    number = "";
                    parts.Enqueue(text[i].ToString());
                }

            }
            if (number != "")
            {
                parts.Enqueue(number);
            }

            return parts;
        }
        public void test()
        {
            Queue<string> result = SplitExpression("12.5+3*(4.2-2)");

            while (result.Count > 0)
            {
                Console.WriteLine(result.Dequeue());
            }
        }

        public Queue<string> ConvertToPostfix(string s)
        {
            Queue<string> parts = SplitExpression(s);
            Queue<string> Qoperations = new Queue<string>();
            Stack<string> StTemp = new Stack<string>();
            string st = "";
            while (parts.Count > 0)
            {

                st = parts.Dequeue();
                if (CheckDouble(st))
                {
                    Qoperations.Enqueue(st);
                }
                else if (st == "(")
                {
                    StTemp.Push(st);
                }
                else if (st == ")")
                {
                    while (StTemp.Count > 0)
                    {
                        if (StTemp.Peek() == "(")
                        {
                            StTemp.Pop();
                            break;
                        }
                        else
                        {
                            Qoperations.Enqueue(StTemp.Pop());
                        }


                    }

                }
                else
                {

                    while (StTemp.Count > 0 && StTemp.Peek() != "(" && (Levelop(st) <= Levelop(StTemp.Peek())))
                    {

                        Qoperations.Enqueue(StTemp.Pop());

                    }

                    StTemp.Push(st);

                }
            }
            while (StTemp.Count > 0)
            {
                if (StTemp.Peek() != "(")
                {
                    Qoperations.Enqueue(StTemp.Pop());
                }
                else
                {
                    StTemp.Pop();
                }
            }

            return Qoperations;
        }

        int Levelop(string s1)
        {
            if (s1 == "(" || s1 == ")") return 0;
            if (s1 == "+" || s1 == "-") return 1;
            if (s1 == "/" || s1 == "*") return 2;
            else return -1;
        }

        public bool CheckDouble(string s)
        {
            if (s == "" || s == "-" || s == "." || s == "-.") return false;

            int count = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if ((!char.IsDigit(s[i]) && (s[i] != '.')) || (i > 0 && s[i] == '-'))
                {
                    return false;
                }
                if (s[i] == '.')
                {
                    count++;
                    if (count > 1) return false;
                }
            }

            return true;
        }
        public double ConvertStringToFloat(string s)
        {
            if (!CheckDouble(s)) return 0;
            double f = 0;
            double count = 1;
            bool Point = false;
            double PointPart = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '-') continue;
                else if (s[i] == '.') { Point = true; }
                else
                {
                    if (Point)
                    {
                        count *= 0.1;
                        PointPart += (s[i] - '0') * count;
                    }

                    else
                    {
                        f *= 10;

                        f = f + (s[i] - '0');
                    }

                }

            }
            f = f + PointPart;
            if (s[0] == '-')
            {
                f *= -1;
            }
            return f;
        }
        double Calculate(double a, double b, string s)
        {
            switch (s)
            {
                case "+":
                    return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/":
                    {
                        if (b == 0) return 0;
                        return a / b;
                    }
                default: return 1;
            }

        }
        double CalculatePostfix(Queue<string> Q)
        {
            Stack<double> st = new Stack<double>();
            double a = 0;
            double b = 0;
            while (Q.Count > 0)
            {
                string s = Q.Dequeue();
                if (CheckDouble(s))
                {
                    st.Push(ConvertStringToFloat(s));
                }
                else
                {

                    if (st.Count >= 2)
                    {
                        b = st.Pop();
                        a = st.Pop();
                        st.Push(Calculate(a, b, s));
                    }



                }
            }


            if (st.Count > 0)
            {
                return st.Pop();
            }
            return 0;
        }


    }
    }


