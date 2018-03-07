using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Mail;
using System.IO;

/**电波字幕组，字幕格式检查器
 * created by LSD猴子**/

namespace wave
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            savefile.Visible = false;
           


        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //CheckBox chb = new CheckBox();
            //chb.Name = "checkbox"; //设置按钮文字
            //chb.Text = "number";
            //chb.Location = new Point(400, 70); //设置他的坐标
            //chb.AutoCheck = true;
            //panel1.Controls.Add(chb); //把btn对象添加到该窗体的控件集合内

            


        }

        public class Myclass
        {
            public static String origionname = "";
            public static List<string> thissubfile = new List<string>();
            public static List<List<string>> suggestionline = new List<List<string>>();
        }

        //检查是否有空行和多行
        public bool Linecheck(string thisline, int linenumber)
        {
            if (linenumber % 5 == 0)
            {
                //this is a blank line 5n
                if (thisline.Length != 0)
                {
                    int srtline =1 + linenumber / 5; 
                    string errormessage = "检查文本第" + linenumber + "行(字幕第"+srtline+"句)前是否缺行或多行";
                    MessageBox.Show(errormessage);
                    return false;
                }
                
            }
            else if (linenumber % 5 == 1)
            {
                //this is a number line 5n+1
                int number;
                bool result = Int32.TryParse(thisline, out number);
                if (result)
                {

                }
                else
                {
                    int srtline =1 + linenumber / 5;
                    string errormessage = "检查文本第" + linenumber + "行(字幕第" + srtline + "句)前是否缺行或多行";
                    MessageBox.Show(errormessage);
                    return false;
                }

            }
            return true;
        }

        // 加载第一次 检查文件是否存在空行或多行
        public void Read1(string path)
        {   
            System.IO.StreamReader sr = new System.IO.StreamReader(path, Encoding.Default);
            String line;
            int linenumber = 0;
            bool linemiss = true;
            while ((line = sr.ReadLine()) != null)
            {
                linenumber += 1;
                if (Linecheck(line, linenumber) == false)
                {
                    linemiss = false;
                    break;

                }
            }
            if (linemiss)
            {
                Read2(path, linenumber);

            }
        }

        CheckBox cb = new CheckBox();

        //确定Read1检查通过 开始逐行检查符号
        public void Read2(string path, int totalline)
        { 
            
                                    
            //初始化进度条
            progressBar1.Visible = true;
            progressBar1.Minimum = 1;
            progressBar1.Maximum = totalline;
            progressBar1.Step = 1;

            System.IO.StreamReader sr = new System.IO.StreamReader(path, Encoding.Default);
            String line;
            //存储错误行的list
            List<string> wrongline = new List<string>();
            //存储检查结果的列表
            List<string> modifiedline = new List<string>();
            //存储修改建议的列表
            //List<List<string>> suggestionline = new List<List<string>>();
            //List<string> suggestionline = new List<string>();
            
            int linenumber = 0;
            while ((line = sr.ReadLine()) != null)
            {
                Myclass.thissubfile.Add(line);
                //计算进度条进度
                progressBar1.PerformStep();

                //Text1.Text = "read: " + line;
                linenumber += 1;
                Console.WriteLine("line=" + linenumber+ " "+line);

                int length_line = line.Length;
                //检查结果
                string modified=line;
                //修改建议
                string suggestion = line;

                //当加入了空格或删除后，后面字符的index会变化，因此用count进行调整
                int count = 0;

                //跳过数字行，轴行和空格行
                if ((linenumber - 2) % 5 == 0 || (linenumber - 1) % 5 == 0 || linenumber % 5 == 0)  {
                    Console.WriteLine("无关行！跳过！" + linenumber + " " + line);
                    continue;
                }

                //在这里添加需要检查的符号
                if (line.Contains('“'))//中文引号“” 英文引号""
                {
                    suggestion = suggestion.Replace('“', '"');
                    modified = line + "【中文引号】";
                   
                }
                if (line.Contains('”'))
                {
                    suggestion = suggestion.Replace('”', '"');
                    modified = line + "【中文引号】";
                }

                try {
                //一行逐个检查字符是否为破折号
                for (int i = 0; i < length_line; i++)
                {
                    char this_character = line[i];
                        
                    if ( this_character == '-')
                    {
                        //如果-在句首的情况，只考虑其后
                        if (i==0)
                        {
                            if(line[1]!= ' ')
                            {
                                suggestion = suggestion.Insert((i + 1 + count), " ");
                                modified = line+ "【少】";
                                count += 1;
                                Console.Write("句首，缺空格，在后加一个空格");
                                Console.WriteLine("new: "+ suggestion);
                                    


                                    Console.WriteLine("index="+i);
                            }
                            if (suggestion[2] == ' ')
                            {
                                suggestion = suggestion.Remove( 1 + count, 1);
                                modified = line + "【多】";
                                count -= 1;
                                Console.Write("句首，多空格，在后删除一个空格");
                                Console.WriteLine("index=" + i);
                            }
                        }
                        
                        else if(i==1 && line[0] == ' ')
                        {
                            suggestion = suggestion.Remove(0, 1);
                            modified = line + "【多】";
                            count -= 1;
                            Console.Write("句首多一个空格，删除");
                            Console.WriteLine("index=" + i);

                        }
                        //不在句首
                        else
                        {
                                Console.WriteLine("into else: " + suggestion);
                                Console.WriteLine("current i: " + suggestion[i]);

                                //中文行
                                if ((linenumber - 3) % 5 == 0)
                            {
                                //破折号前后都没空格，应为词中连接线，忽略
                                if (line[i - 1] != ' ' && suggestion[i + 1] != ' ')
                                {
                                    break;
                                }
                                if (line[i - 1] != ' ')
                                {
                                    suggestion = suggestion.Insert((i  + count), " ");
                                    modified = line + "【前少】";
                                    count += 1;
                                    Console.Write("前1缺空格，前1加空格");
                                    Console.WriteLine("原始" + line);
                                    Console.WriteLine("修改" + suggestion);
                                    Console.WriteLine("提示" + modified);
                                    }
                                if (line[i - 2] != ' ')
                                {
                                    suggestion = suggestion.Insert((i -1 + count), " ");
                                    modified = line + "【前少】";
                                    count += 1;
                                    Console.Write("前2缺空格，前2加空格");
                                        Console.WriteLine("原始" + line);
                                        Console.WriteLine("修改" + suggestion);
                                        Console.WriteLine("提示" + modified);
                                    }
                                if (line[i + 1] != ' ')
                                {
                                    suggestion = suggestion.Insert((i + 1 + count), " ");
                                    modified = line + "【后少】";
                                    count += 1;
                                    Console.Write("后1少空格，后1加空格");
                                        Console.WriteLine("原始" + line);
                                        Console.WriteLine("修改" + suggestion);
                                        Console.WriteLine("提示" + modified);
                                    }

                            }
                                //英文行
                                else
                                {
                                //破折号前后都没空格，应为词中连接线，忽略
                                if (line[i - 1]!= ' '&& line[i + 1] != ' ')
                                {

                                        Console.WriteLine("qian " + suggestion[i - 1]);
                                        Console.WriteLine("hou " + suggestion[i + 1]);
                                        break;
                                }
                                if (line[i - 1] != ' ')
                                {
                                    suggestion = suggestion.Insert((i + count), " ");
                                    modified = line + "【前少】";
                                    count += 1;
                                    Console.Write("前面缺空格，前面加一个空格");
                                        Console.WriteLine("原始" + line);
                                        Console.WriteLine("修改" + suggestion);
                                        Console.WriteLine("提示" + modified);
                                    }
                                if (line[i + 1] != ' ')
                                {

                                    suggestion = suggestion.Insert((i + 1 + count), " ");
                                    modified = line + "【后少】";
                                    count += 1;
                                    Console.Write("后面少空格，后面加一个空格");
                                        Console.WriteLine("原始" + line);
                                        Console.WriteLine("修改" + suggestion);
                                        Console.WriteLine("提示" + modified);
                                    }
                                if (line[i - 2] == ' ')
                                {
                                    suggestion = suggestion.Remove(i - 1, 1);
                                    modified = line + "【前多】";
                                    count -= 1;
                                    Console.Write("前1多空格，删除一个空格");
                                        Console.WriteLine("原始" + line);
                                        Console.WriteLine("修改" + suggestion);
                                        Console.WriteLine("提示" + modified);
                                    }
                                if (line[i + 2] == ' ')
                                {
                                    suggestion = suggestion.Remove(i + 1, 1);
                                    modified = line + "【后多】";
                                    count -= 1;
                                    Console.Write("后1多空格，删除一个空格");
                                        Console.WriteLine("原始" + line);
                                        Console.WriteLine("修改" + suggestion);
                                        Console.WriteLine("提示" + modified);
                                    }

                            }

                            

                        }
                    }
                }
                }
                catch(IndexOutOfRangeException)
                {
                    //如果出现意外情况请求人工核查， 输出原句
                    suggestion = line;
                    modified = "【检查此行是否有多余破折号】";
                }



                int srtnumber = 1 + linenumber / 5;
                if (line != modified) {
                    wrongline.Add("文本第"+linenumber+ "行，字幕第"+srtnumber+"句:" + modified);
                    modifiedline.Add(modified);

                    List<string> choice = new List<string>();
                    choice.Add(suggestion);
                    choice.Add("true");
                    choice.Add((linenumber-1).ToString());
                                                           
                    Myclass.suggestionline.Add(choice);
                    //Console.Write("XXXXXX: "+choice[0] + choice[1] + choice[2]);


                }
                
            }
            string combine_wrongline = string.Join("\r\n\r\n", wrongline.ToArray());
            string combine_modifiedline = string.Join("\r\n\r\n", modifiedline.ToArray());
            Text1.Text = combine_wrongline;

          
            int checkboxnumber = Myclass.suggestionline.Count();
            
            for (int i = 0; i < checkboxnumber; i++)
            {
                cb = new CheckBox();
                cb.Text = Myclass.suggestionline[i][0];
                cb.Location = new Point(5, 5 + i * 24);
                cb.BackColor = Color.White;
                cb.Name = i + "checkbox";
                cb.AutoSize = true;
                cb.Checked = true;
                cb.CheckedChanged += new EventHandler(CheckedChanged);
                panel1.Controls.Add(cb);
            };

            savefile.Visible = true;

            

       




            }

        private void CheckedChanged(object sender, EventArgs e)
        {
            //通过sender得到具体哪个checkbox被触发
            CheckBox cb = sender as CheckBox;
            if (!cb.Checked)
            {
                String thisname = cb.Name.Replace("checkbox", "");
                int thisindex = Int32.Parse(thisname);
                Myclass.suggestionline[thisindex][1] = "false";
                Console.WriteLine(cb.Text +" unchecked");

            }
            if (cb.Checked)
            {
                String thisname = cb.Name.Replace("checkbox", "");
                int thisindex = Int32.Parse(thisname);
                Myclass.suggestionline[thisindex][1] = "true";
                Console.WriteLine(cb.Text + " checked");

            }
        }


        //选择文件
        private void Open_click(object sender, EventArgs e)
        {
            

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            //文件格式
            openFileDialog.Filter = "所有文件|*.*";
            //还原当前目录
            openFileDialog.RestoreDirectory = true;
            //默认的文件格式
            openFileDialog.FilterIndex = 1;

            Text1.Text = String.Empty;
            panel1.Controls.Clear();
            Myclass.suggestionline.Clear();
            Myclass.thissubfile.Clear();
            savefile.Visible = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                

                string path = openFileDialog.FileName;
                Myclass.origionname = openFileDialog.SafeFileName;
                Read1(path);

            }



       

        }

        private void Output_click(object sender, EventArgs e)
        {
            Linereplace();


            SaveFileDialog savefile = new SaveFileDialog();
            // set a default file name
            savefile.FileName = "【修改】" + Myclass.origionname ;
            // set filters - this can be done in properties as well
            savefile.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (savefile.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(savefile.FileName))
                {
                    foreach (String s in Myclass.thissubfile)
                        sw.WriteLine(s);
                }
            }

        }

        public void Linereplace()
        {
            int len1 = Myclass.suggestionline.Count();

            for (int i = 0; i < len1; i++)
            {
                if (Myclass.suggestionline[i][1] == "true")
                {
                    int rownumber = Int32.Parse(Myclass.suggestionline[i][2]);
                    Myclass.thissubfile[rownumber] = Myclass.suggestionline[i][0];
                }


            }
        }
    }
}
