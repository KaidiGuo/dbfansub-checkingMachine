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


/**电波字幕组，字幕格式检查器
 * created by LSD猴子**/

namespace wave
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Text2.Visible = false;

            TextBox myText = new TextBox();
            myText.Location = new Point(765, 147);
            this.Controls.Add(myText);


        }

        private void Form1_Load(object sender, EventArgs e)
        {
           


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
            ArrayList suggestionline = new ArrayList();
            int linenumber = 0;
            while ((line = sr.ReadLine()) != null)
            {   
                //计算进度条进度
                progressBar1.PerformStep();

                //Text1.Text = "read: " + line;
                linenumber += 1;
                Console.WriteLine("line=" + linenumber+ " "+line);

                int length_line = line.Length;
                //检查结果
                string modified=line;
                //修改建议
                string suggestion = "";

                int count = 0;

                //跳过数字行，轴行和空格行
                if ((linenumber - 2) % 5 == 0 || (linenumber - 1) % 5 == 0 || linenumber % 5 == 0)  {
                    Console.WriteLine("无关行！跳过！" + linenumber + " " + line);
                    continue;
                }

                //在这里添加需要检查的符号
                if (line.Contains('“'))//中文引号“” 英文引号""
                {
                    suggestion = line.Replace('“', '"');
                    modified = suggestion + "【中文引号】";
                   
                }
                if (line.Contains('”'))
                {
                    suggestion = line.Replace('”', '"');
                    modified = suggestion + "【中文引号】";
                }

                try {
                //一行逐个检查字符是否为破折号
                for (int i = 0; i < length_line; i++)
                {
                    char this_character = line[i];
                        
                    if ( this_character == '-')
                    {
                        if (i==0)//如果-在句首的情况，只考虑其后
                        {
                            if(line[i+1]!= ' ')
                            {
                                suggestion = line.Insert((i + 1 + count), " ");
                                modified = suggestion+ "【少空格】";
                                count += 1;
                                Console.Write("句首，缺空格，在后加一个空格");
                                Console.WriteLine("index="+i);
                            }
                            if (line[i + 2] == ' ')
                            {
                                suggestion = line.Remove(i + 1, 1);
                                modified = suggestion + "【多空格】";
                                count += -1;
                                Console.Write("句首，多空格，在后删除一个空格");
                                Console.WriteLine("index=" + i);
                            }
                        }
                        else if(i==1 && line[0] == ' ')
                        {
                            suggestion = line.Remove(0, 1);
                            modified = suggestion + "【多空格】";
                            count -= 1;
                            Console.Write("句首多一个空格，删除");
                            Console.WriteLine("index=" + i);

                        }
                        else//不在句首
                        {
                            //中文行
                            if ((linenumber - 3) % 5 == 0)
                            {
                                //破折号前后都没空格，应为词中连接线，忽略
                                if (line[i - 1] != ' ' && line[i + 1] != ' ')
                                {
                                    break;
                                }
                                if (line[i - 1] != ' ')
                                {
                                    suggestion = line.Insert((i + count), " ");
                                    modified = suggestion + "【少空格】";
                                    count += 1;
                                    Console.Write("前1缺空格，前1加空格");
                                    Console.WriteLine("index" + i);
                                }
                                if (line[i - 2] != ' ')
                                {
                                    suggestion = line.Insert((i + count), " ");
                                    modified = suggestion + "【少空格】";
                                    count += 1;
                                    Console.Write("前2缺空格，前2加空格");
                                    Console.WriteLine("index" + i);
                                }
                                if (line[i + 1] != ' ')
                                {
                                    suggestion = line.Insert((i + 1 + count), " ");
                                    modified = suggestion + "【少空格】";
                                    count += 1;
                                    Console.Write("后1少空格，后1加空格");
                                    Console.WriteLine("index" + i);
                                }

                            }
                            else//英文行
                            {
                                //破折号前后都没空格，应为词中连接线，忽略
                                if (line[i - 1]!= ' '&& line[i + 1] != ' ')
                                {
                                        break;
                                }
                                if (line[i - 1] != ' ')
                                {
                                    suggestion = line.Insert((i + count), " ");
                                    modified = suggestion + "【少空格】";
                                    count += 1;
                                    Console.Write("前面缺空格，前面加一个空格");
                                    Console.WriteLine("index" + i);
                                }
                                if (line[i + 1] != ' ')
                                {

                                    suggestion = line.Insert((i + 1 + count), " ");
                                    modified = suggestion + "【少空格】";
                                    count += 1;
                                    Console.Write("后面少空格，后面加一个空格");
                                    Console.WriteLine("index" + i);
                                }
                                if (line[i - 2] == ' ')
                                {
                                    suggestion = line.Remove(i - 1, 1);
                                    modified = suggestion + "【多空格】";
                                    count -= 1;
                                    Console.Write("前1多空格，删除一个空格");
                                    Console.WriteLine("index" + i+ "line"+linenumber);
                                }
                                if (line[i + 2] == ' ')
                                {
                                    suggestion = line.Remove(i + 1, 1);
                                    modified = suggestion + "【多空格】";
                                    count -= 1;
                                    Console.Write("后1多空格，删除一个空格");
                                    Console.WriteLine("index" + i + "line" + linenumber);
                                }

                            }

                            

                        }
                    }
                }
                }
                catch(System.IndexOutOfRangeException e)
                {
                    //如果出现意外情况请求人工核查， 输出原句
                    suggestion = line;
                    modified = "【检查此行是否有多余破折号】";
                }



                int srtnumber = 1 + linenumber / 5;
                if (line != modified) {
                    wrongline.Add("文本第"+linenumber+ "行，字幕第"+srtnumber+"句:" + line);
                    modifiedline.Add(modified);

                    ArrayList choice = new ArrayList();
                    choice.Add(suggestion);
                    choice.Add(false);
                    choice.Add(linenumber-1);
                                                           
                    suggestionline.AddRange(choice);
                    //Console.Write("XXXXXX: "+choice[0] + choice[1] + choice[2]);


                }
                
            }
            string combine_wrongline = string.Join("\r\n\r\n", wrongline.ToArray());
            string combine_modifiedline = string.Join("\r\n\r\n", modifiedline.ToArray());
            Text1.Text = combine_wrongline;

            

            //Console.Write("ssssss: "+suggestionline[0]);


            //Text2.Text = combine_modifiedline;



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

            if (openFileDialog.ShowDialog() == DialogResult.OK)

            {

                string path = openFileDialog.FileName;
                Read1(path);


            }



            ////选择文件夹

            //FolderBrowserDialog dialog = new FolderBrowserDialog();

            //dialog.Description = "请选择文件路径";

            //if (dialog.ShowDialog() == DialogResult.OK)

            //{

            //    string foldPath = dialog.SelectedPath;

            //    MessageBox.Show("已选择文件夹:" + foldPath, "选择文件夹提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

            //}

        }

        

       
    }
}
