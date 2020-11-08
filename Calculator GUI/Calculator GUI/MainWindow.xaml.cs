using NCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml.Serialization;

namespace Calculator_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string formula = "";
        string expression = "";

        public MainWindow()
        {
            InitializeComponent();
        }
        //Turn minimized and close
        private void DockPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();

        }
        private void Image_PreviewMouseDown_Minimized(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }
        private void Image_PreviewMouseDown_Close(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        //Sign
        private void BackSpace_Click(object sender, RoutedEventArgs e)
        {
            ResetFormula();
            if (!String.IsNullOrEmpty(expression))
            {
                expression = expression.Substring(0, expression.Length - 1);
            }
            DisplayToTextBox();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            this.expression = "";
            this.formula = "";
            this.txtDisplay.Text = "";
            this.txtResult.Text = "";
        }

        private void ClearEmpty_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Positive_Negative_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //1. check chuỗi hợp lệ 
                string validExp = new NCalc.Expression(expression).Evaluate().ToString();
                //2. clone chuỗi
                string cloneExp = expression;
                // 3. 23+(-14)/9-9*2
                cloneExp = cloneExp
                    .Replace("(-", ":") //thay thế(- thành dấu :  23+(-14)/9 => 23+:14)/9-9*2
                    .Replace(")", ""); //xóa dấu )   23+:14) => 23+:14/9-9*2
                cloneExp = cloneExp
                    .Replace("*", ",") //thay dấu * thành dấu , 23+:14/9-9*2 => 23+:14/9-9,2
                    .Replace("/", ",") //thay dấu * thành dấu ,  23+:14/9-9,2 => 23+:14,9-9,2
                    .Replace("+", ",") //thay dấu * thành dấu ,  23+:14,9-9,2 => 23,:14,9-9,2
                    .Replace("-", ","); //thay dấu * thành dấu ,  23,:14,9-9,2 => 23,:14,9,9,2
                string[] exp = cloneExp.Split(','); // cắt chuỗi string thành array string phân cách = dấu ,    23,:14,9,9,2 => [23, :14, 9, 9, 2]
                string lastString = exp[exp.Length - 1]; // lấy ra string cuối cùng. 2
                float lastNumber = float.Parse(exp[exp.Length - 1].Replace(":", "-")); //Thay dấu : thành dấu -. Và parse string sang float
                int length = lastString.Length; // lấy độ dài của string cuối cùng
                if (lastNumber > 0)
                {
                    //Số dương thì chuyển thành số âm = cách thay thế số cuối cùng của chuỗi expression = số hiện tại thêm đóng mở ngoặc và dấu -. 
                    //23+(-14)/9-9*2 => 23+(-14)/9-9*(-2)
                    expression = expression.Substring(0, expression.Length - length) + "(-" + lastNumber + ")";
                }
                else
                {
                    //Số âm thì chuyển thành số dương = cách thay thế số cuối cùng của chuỗi expression = và số hiện tại. Xóa dấu đóng mở ngoặc và dấu - 
                    //23+(-14)/9-9*(-2) => 23+(-14)/9-9*2
                    expression = expression.Substring(0, expression.Length - (length + 2)) + lastNumber.ToString().Replace("-", "");
                }
                DisplayToTextBox();
            }
            catch (Exception ex)
            {
                //lỗi xảy ra không thao tác gì
                Console.WriteLine(ex.Message.ToString());
            }
        }

        //Result
        private void DisplayToResult()
        {
            txtResult.Text = new NCalc.Expression(expression).Evaluate().ToString();
        }

        private void DisplayToTextBox(string text = "")
        {
            txtDisplay.Text = String.IsNullOrEmpty(text) ? expression : text;
        }
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                expression = expression.EndsWith(".") ? expression + "0" : expression;
                string validResult = new NCalc.Expression(expression).Evaluate().ToString();
                DisplayToResult();
            } catch (Exception ex)
            {

            }
        }

        //Number
        private void Number_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string number = btn.Content.ToString();

            ResetFormula();

            expression = ConcatNumber(number);

            DisplayToTextBox();
        }

        //Formula
        private void Formula_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string formula = btn.Content.ToString();

             expression =  ConcatFormula(formula);

            DisplayToTextBox();
        }

        private string ConcatNumber(string number)
        {
            return expression.EndsWith(")") ? expression : expression + number;
        }

        private string ConcatFormula(string formula)
        {
            if (String.IsNullOrEmpty(expression)) 
            {
                return expression;
            }

            if (String.IsNullOrEmpty(this.formula))
            {
                this.formula = formula;
                return expression + formula;
            }

            this.formula = formula;
            return  expression.Substring(0,  expression.Length - 1) + formula;
        }

        private void ResetFormula()
        {
            formula = "";
        }

        private void Dot_Click(object sender, RoutedEventArgs e)
        {
            string expDot = expression.EndsWith(".") ? expression : expression + ".0";
            try
            {
                string validDot = new NCalc.Expression(expDot).Evaluate().ToString();
                expression += ".";
                DisplayToTextBox();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error");
            }
        }
    }
}
