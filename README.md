
# **电波字幕组字幕格式校对器**

**一个简单的C#桌面应用，实现文本打开，校对，勾选，保存**
***
作为一枚电波组的小翻译，希望能有效的避免格式错误增加校对不必要的工作量，因此诞生了这个格式校对的小应用，主要规则为：**英文字幕行如有多人说话，每个人的话前加破折号 - ，破则号前后各一个空格，两句话之间也为一个空格，而相应的中文行则是两句话之间两个空格。**

所以本应用实现功能为：
- 点击按钮打开文本文件（txt以及srt格式）
- 校对字幕格式（主要围绕破折号前后空格数量的规则）
- 对根据规则找出的错误行进行修改，并显示勾选框
- 点击按钮保存新文件，更改勾选框为”选定“状态的错句

平台：Visual studio 2017
***
## 1. 新建项目

打开VS编辑器，分别点击FILE->NEW->Project，然后在弹出的复选框中选择Visual C#->Window Forms Application，在底部设置工程的名称还有保存的路径。 
![新建project](https://upload-images.jianshu.io/upload_images/42676-22323ec90b82546f.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
如果你找不到windows Forms Application.
点击左下角的Open Visual Studio Installer
![installer](https://upload-images.jianshu.io/upload_images/42676-b84e170b4b01b9fa.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
![Modify](https://upload-images.jianshu.io/upload_images/42676-4c878331d21e492d.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
![install](https://upload-images.jianshu.io/upload_images/42676-4d473d1cf375a884.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
安装需要关闭VS。
完成后你就可以在新建项目菜单中看到Window Forms Application了。
***
## 2. 应用功能介绍
![应用界面](https://upload-images.jianshu.io/upload_images/42676-ad3b5170714a14d9.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

点击按钮选择泽字幕文件，打开windows资源管理器进行文件选择。
点击确定后，程序自动比对每一行字幕行是否符合格式规则，如果不符合，则会在左侧文本框显示错误行信息，包括文本文件中的对应行号和字幕句的编号，以及错误原因提示。
在右侧对应出现系统修改结果和一个默认为勾选状态的选择框。

![应用界面 - 输出样例](https://upload-images.jianshu.io/upload_images/42676-a2821f3966788a07.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

这时界面右上角会出现一个保存按钮，点击按钮会用勾选框为选定状态的修改句子替换原始错句新存一个文件。
这个功能是因为有一些特殊情况在规则判断外（比如在不明情况下连续出现了两个破则号），可能会产生错误的修改提示，这个时候校对就可以手动取消选择框，输出其他项后再手动修改特殊错句。

有时候翻译删掉一轴后会忘记自己还留有空行，很难直接进行规则核对，也会给后期造成不便，所以本应用在首次打开一个字幕文件的时候会先对行号进行审核。
一个完整的字幕文件应当严格遵守【编号】-【时间轴】-【中文行】-【英文行】-【空行】的循环。

![正确的字幕文件行循环示例](https://upload-images.jianshu.io/upload_images/42676-7b76cfe4c667cc47.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

如果第一次打开的文件有缺行或多行的情况，应用会弹出一个提示框，给出行号和字幕句编号，使用者这时请手动修改并保存，然后再打开此字幕文件。

![当打开出现缺行或多行的文件，出现提示](https://upload-images.jianshu.io/upload_images/42676-0a4141e193297760.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)
***
## 3.代码块讲解
### 3.1Windows资源管理器openFileDialog
字行在Form1.cs的设计视图里面调整窗口大小。
拖动工具箱Toolbox内的button到合适位置。

![工具箱Toolbox](https://upload-images.jianshu.io/upload_images/42676-aa268cbb3feb86a2.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

给我们的第一个打开文件button设置name，即是类似ID的存在，通过这个name找到对应的控件。

![image.png](https://upload-images.jianshu.io/upload_images/42676-da9738db39271dec.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

属性右侧的闪电图标是此控件绑定的各个事件，在这里，我们需要“点击”事件，给它起个名字叫Open_click。双击此事件即会自动命名为【控件名_事件】并在代码底部生成event框架。事件名可自己修改。

![Events窗口](https://upload-images.jianshu.io/upload_images/42676-7f6f19e3000dd6a6.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

![自动生成的事件代码框架](https://upload-images.jianshu.io/upload_images/42676-3677ec9c0191141d.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

在我们按下这个按钮的时候，打开Windows文件窗口。

![windows文件窗口](https://upload-images.jianshu.io/upload_images/42676-8e69c7b82c3fdeb6.png?imageMogr2/auto-orient/strip%7CimageView2/2/w/1240)

这里使用openFileDialog类。
当文件窗口的结果是OK（即当用户点击了open按钮），我们获取到选择的文件名和其路径。在这里，`openFileDialog.FileName`返回的是带路径的文件名，如果只想返回文件名，则使用`openFileDialog.SafeFileName`. 这里我单独设定了一个全局公共类Myclass，在里面设定了全局变量origionname用来保存文件的名称，便于后来输出的时候根据原文件名生成新的默认文件名。

```
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
                Myclass.origionname = openFileDialog.SafeFileName;
                Read1(path);
            }
        }
```
这里要强调C#其实没有全局变量一说，但有时候确实需要这么个东西，所以为了（图省事）实现，便可以这样操作：在一个public类里面定义一个`public static`的变量即可. 
使用方法如上，类名.变量名：`Myclass.origionname`.
```
 //定义全局变量
        public class Myclass
        {
            public static String origionname = "";
            public static List<string> thissubfile = new List<string>();
            public static List<List<string>> suggestionline = new List<List<string>>();
        }
```
### 3.2逐行读文件，缺行多行查找
在Open_click方法最后我们调用了Read1()方法读文件，并将获取到的文件路径path当作参数传递。
`System.IO.StreamReader sr = new System.IO.StreamReader(path, Encoding.Default);`文件输入流，参数为文件路径和编码模式，这里是默认编码.
检查缺行多行的方法为，逐行读文件，根据行号和读到的行的内容进行数学判断.
如果有错误，直接停止循环.

```
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
```
检查的具体数学方法是行号循环。
如果行号是5的倍数，这一行就应当为空白行（这里为了用户使用方便，行号跟文本文件保持一致，从1而不是0开始计）
如果行号是5n+1，这一行就应该是数字行；
如果行号是5n+2,，这一行就应该是时间轴行；
如果行号是5n+3,，这一行就应该是中文行；
如果行号是5n+4,，这一行就应该是英文行；
如果判读遇到不符合的情况，弹出信息框，显示出错的行号，（在这里我没有判断这么多情况，因为判断是不是数字行和空行比较简单，剩下几个判断比较难）
```
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
```
### 3.3格式判断与checkbox动态生成
当字幕文件检查过空行和多行后就可以进行破折号前后空格数的检查了。
根据两条规则建立以下判断。

-->逐行读行，跳过间隔行，数字行和时间轴行，只比对中文行和英文行
-->再依次比对句子中的字符
-->如果当前字符为破折号 “-”
    ---->如果破折号在句首，检查其后是不是有且只有一个空格
    ---->如果不在句首
------>如果破则号前后都不为空格，则很可能是名词中连接用破则，保持不变
       --------->如果是中文行，破折号前要有且有两个空格，其后有且只有一个空格
        --------->如果是英文行，破折号前要有且有一个空格，其后有且只有一个空格

大概就是这样一个比对逻辑，还有一些很小的细节和如何根据错误情况进行修改就不在此列举了。
我在比较的时候将错误的句子加上错误提示，行号，加入一个预设的`wrongline`列表，再将根据规则修改后的句子加入另一个列表`suggestionline`，这是一个嵌套数组 
`public static List<List<string>> suggestionline = new List<List<string>>();`
其中每一个元素的格式是**{"修改后的行" , "true" , "行号" }**，因为后面需要替换错句和勾选框功能，这里附加了行号和状态（状态预设为true）。
```
wrongline.Add("文本第"+linenumber+ "行，字幕第"+srtnumber+"句:" + modified);
modifiedline.Add(modified);
List<string> choice = new List<string>();
choice.Add(suggestion);
choice.Add("true");
choice.Add((linenumber-1).ToString());
Myclass.suggestionline.Add(choice);
```
当文件读行到底的时候，我就有了一个装满错句的列表和一个装着正确句子的列表了。
将所有的错句行添加到我们拖拽的文本框Textbox控件里面，左侧就会显示错句表。
```
string combine_wrongline = string.Join("\r\n\r\n", wrongline.ToArray());
Text1.Text = combine_wrongline;
```
根据我的suggestionlist生成checkbox的代码块如下：
```
CheckBox cb = new CheckBox();

......

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
```
得到一共要生成几个checkbox，开始循环，每次循环新生产一个checkbox，设定text为我每个元素的[0]位，放置位置为5 + i * 24像素，默认勾选状态为true。
因为这些checkbox是动态生成的，所以为了知道用户到底取消勾选了哪一个，就需要给每个checkbox绑定一个checkedchanged事件：
`cb.CheckedChanged += new EventHandler(CheckedChanged);`
这个事件有个参数为`object sender`，通过它就可以知道具体是哪一个checkbox被触发了，赋值给一个新的checkbox变量即可`CheckBox cb = sender as CheckBox;`
然后我们就可以通过`cb.Name`属性得到这个checkbox的name了，而又因为我们生成checkbox的时候给他们的name依次编了号，这时候就可以准确的定位到suggestionline里面的某一个元素了，当用户取消点选了这一个checkbox，我们就定位到这个元素，将[1]位的 "true "改为 "false "（这里的"true ""false "并不是布尔型数据，就是一般的string形，因为嵌套list不允许多数据类型）
```
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
```

到此为止，我们就有了一个用户同意改正的推荐改正列表了。
### 3.4文件保存 SaveFileDialog()
我的保存按钮设置的是当完成比对后才会出现，这个只用设置一下visible属性就可以。
最开始在全局变量中保存了原始文件的名字，这里用它来生成保存的新文件的默认名为`savefile.FileName = "【修改】" + Myclass.origionname ;`

```
//保存文件点击事件
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
```
这里首先执行了一个`Linereplace();`方法是用来替换用户勾选通过的改正句子的，改正后的整个文档是存在全局变量`Myclass.thissubfile`列表里的。
```
//替换选择通过的字幕行
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
```


这个应用里面还加了一个进度条提示文件处理进度。
在第二次开始比对格式的方法最开头添加进度条，总长度就是文件总行数，步长为1
```
//初始化进度条
            progressBar1.Visible = true;
            progressBar1.Minimum = 1;
            progressBar1.Maximum = totalline;
            progressBar1.Step = 1;
```
然后在每读一句`line = sr.ReadLine()`以后使步子加一即可：
```
//计算进度条进度
 progressBar1.PerformStep();
```

以上，就是电波字幕组字幕格式校对器的全部编写过程讲解了。
