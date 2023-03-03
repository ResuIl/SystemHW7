using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SystemHW7;

public partial class MainWindow : Window
{
    public DriveInfo[] drives = DriveInfo.GetDrives();

    public DirectoryInfo driverFolder { get; set; }
    public FileInfo[] driverFiles { get; set; }

    public DirectoryInfo[] directories { get; set; }

    public string[] dir { get; set; }

    public MainWindow()
    {
        InitializeComponent();

        foreach (DriveInfo drive in drives)
        {
            cBox_Drives.Items.Add(drive.Name);
            cBox_Drives2.Items.Add(drive.Name);
        }
    }

    private void FillListBox(ListBox listBox)
    {
        listBox.Items.Clear();
        directories = driverFolder.GetDirectories();
        driverFiles = driverFolder.GetFiles();

        listBox.Items.Add("<<<<<<<<<<<<<<<<< Folders >>>>>>>>>>>>>>>>>");
        foreach (var directory in directories)
            listBox.Items.Add(directory);

        listBox.Items.Add("<<<<<<<<<<<<<<<<<< Files >>>>>>>>>>>>>>>>>>");
        foreach (var file in driverFiles)
            listBox.Items.Add(file);
    }

    private void cBox_Drives2_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        driverFolder = new DirectoryInfo(cBox_Drives2.SelectedValue.ToString());
        tBox_First.Text = cBox_Drives2.SelectedValue.ToString();
        FillListBox(lBox_First);

    }

    private void cBox_Drives_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        driverFolder = new DirectoryInfo(cBox_Drives.SelectedValue.ToString());
        tBox_Second.Text = cBox_Drives.SelectedValue.ToString();
        FillListBox(lBox_Second);
    }

    private void lBox_First_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (lBox_First.SelectedItem is DirectoryInfo)
        {
            tBox_First.Text = (lBox_First.SelectedItem as DirectoryInfo).FullName;
            dir = Directory.GetFiles((lBox_First.SelectedItem as DirectoryInfo).FullName);
            driverFolder = new DirectoryInfo((lBox_First.SelectedItem as DirectoryInfo).FullName);
            FillListBox(lBox_First);
        }
        else if (lBox_First.SelectedItem is FileInfo)
        {
            var fileInfo = lBox_First.SelectedItem as FileInfo;
            Process.Start(new ProcessStartInfo { FileName = fileInfo.FullName, UseShellExecute = true });
        }
    }


    private void lBox_Second_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        if (lBox_Second.SelectedItem is DirectoryInfo)
        {
            tBox_Second.Text = (lBox_Second.SelectedItem as DirectoryInfo).FullName;
            dir = Directory.GetFiles((lBox_Second.SelectedItem as DirectoryInfo).FullName);
            driverFolder = new DirectoryInfo((lBox_Second.SelectedItem as DirectoryInfo).FullName);
            FillListBox(lBox_Second);
        }
        else if (lBox_Second.SelectedItem is FileInfo)
        {
            var fileInfo = lBox_Second.SelectedItem as FileInfo;
            Process.Start(new ProcessStartInfo { FileName = fileInfo.FullName, UseShellExecute = true });
        }
    }

    private void ButtonBackClick(object sender, RoutedEventArgs e)
    {
        Button clickedButton = (Button)sender;
        if (!string.IsNullOrEmpty(clickedButton?.Name))
        {
            var temp = "";
            TextBox textBox;
            ListBox listBox;

            switch (clickedButton.Name)
            {
                case "btn_First":
                    temp = tBox_First.Text;
                    textBox = tBox_First;
                    listBox = lBox_First;
                    break;
                case "btn_Second":
                    temp = tBox_Second.Text;
                    textBox = tBox_Second;
                    listBox = lBox_Second;
                    break;
                default:
                    return;
            }

            int tempLength = temp.Length;
            int startindex = temp.LastIndexOf(@"\");
            int count = tempLength - startindex;

            string directoryPath = Path.GetDirectoryName(temp);
            if (directoryPath != null)
            {
                dir = Directory.GetFiles(directoryPath);
                driverFolder = new DirectoryInfo(directoryPath);
                textBox.Text = directoryPath;
                FillListBox(listBox);
            }
        }
    }


    private void ButtonReloadClick(object sender, RoutedEventArgs e)
    {
        Button clickedButton = (Button)sender;
        if (!string.IsNullOrEmpty(clickedButton?.Name) && !string.IsNullOrEmpty(tBox_First.Text) || !string.IsNullOrEmpty(tBox_Second.Text))
        {
            if (clickedButton.Name == "btn_FirstReload")
            {
                dir = Directory.GetFiles(tBox_First.Text);
                driverFolder = new DirectoryInfo(tBox_First.Text);
                FillListBox(lBox_First);
            }
            else
            {
                dir = Directory.GetFiles(tBox_Second.Text);
                driverFolder = new DirectoryInfo(tBox_Second.Text);
                FillListBox(lBox_Second);
            }
        }
    }

    private async void ButtonDelete_Click(object sender, RoutedEventArgs e)
    {
        if (lBox_First.SelectedIndex != -1)
        {
            try
            {
                if (lBox_First.SelectedItem is DirectoryInfo && Directory.Exists((lBox_First.SelectedItem as DirectoryInfo).FullName))
                    (lBox_First.SelectedItem as DirectoryInfo).Delete(true);
                else if (lBox_First.SelectedItem is FileInfo && File.Exists((lBox_First.SelectedItem as FileInfo).FullName))
                    (lBox_First.SelectedItem as FileInfo).Delete();
                await Task.Delay(3000);
                MessageBox.Show("Delete Done!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else
        {
            MessageBox.Show("Select Item From ListBox");
        }
    }

    private async void btn_Move_Click(object sender, RoutedEventArgs e)
    {
        if (lBox_First.SelectedIndex != -1)
        {
            try
            {
                if (lBox_First.SelectedItem is FileInfo)
                    File.Move((lBox_First.SelectedItem as FileInfo).FullName, tBox_Second.Text + "\\" + (lBox_First.SelectedItem as FileInfo).Name);
                else if (lBox_First.SelectedItem is DirectoryInfo)
                    Directory.Move((lBox_First.SelectedItem as DirectoryInfo).FullName, tBox_Second.Text + "\\" + (lBox_First.SelectedItem as DirectoryInfo).Name);

                await Task.Delay(3000);
                MessageBox.Show("Sucessfully Moved!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else
        {
            MessageBox.Show("Select Item From ListBox");
        }
    }

    private string SizeFormat(long _size)
    {
        if (_size >= 1073741824) // GB
            return (_size /= 1073741824).ToString("0.00") + " GB";
        else if (_size < 1073741824 && _size >= 1048576) // MB
            return (_size /= 1048576).ToString("0.00") + " MB";
        else if (_size < 1048576 && _size >= 1024) // KB
            return (_size /= 1024).ToString("0.00") + " KB";
        else if (_size < 1024) // bytes
            return _size.ToString("0.00") + " bytes";

        return null;
    }

    private async void btn_Properties_Click(object sender, RoutedEventArgs e)
    {
        if (lBox_First.SelectedIndex == -1)
        {
            MessageBox.Show("Selected First Listbox!");
            return;
        }

        try
        {
            if (lBox_First.SelectedItem is DirectoryInfo dir)
            {
                var sizeFolders = await Task.Run(() => dir.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));
                var countFiles = await Task.Run(() => dir.EnumerateFiles("*", SearchOption.AllDirectories).Count());
                var countFolders = await Task.Run(() => dir.EnumerateDirectories("*", SearchOption.AllDirectories).Count());

                ShowProperties($"{dir.Name} Proporties", $@"
Folder name:   {dir.Name}
Type:               File folder
Location:         {dir.Root.FullName}
Size:                {SizeFormat(sizeFolders)} ({(double)sizeFolders} bytes) 
Contains:        {countFiles} Files, {countFolders} Folders
Created:          {dir.CreationTime.ToString("dddd, dd MMMM yyyy HH:mm:ss")}");
            }
            else if (lBox_First.SelectedItem is FileInfo file)
            {
                ShowProperties($"{file.Name} Proporties", $@"
File name:       {file.Name}
Type of file:      {file.Extension}
Location:         {System.IO.Path.GetDirectoryName(file.FullName)}
Size:                {SizeFormat(file.Length)} ({(double)file.Length} bytes) 
Created:          {file.CreationTime.ToString("dddd, dd MMMM yyyy HH:mm:ss")}
Modified:         {file.LastWriteTime.ToString("dddd, dd MMMM yyyy HH:mm:ss")}
Accessed:         {file.LastAccessTime.ToString("dddd, dd MMMM yyyy HH:mm:ss")}");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void ShowProperties(string title, string message)
    {
        MessageBox.Show(message, title, MessageBoxButton.OK);
    }



    public void CopyFolder(string sourceFolder, string destFolder)
    {
        if (!Directory.Exists(destFolder))
            Directory.CreateDirectory(destFolder);
        string[] files = Directory.GetFiles(sourceFolder);
        foreach (string file in files)
        {
            string name = System.IO.Path.GetFileName(file);
            string dest = System.IO.Path.Combine(destFolder, name);
            File.Copy(file, dest);
        }
        string[] folders = Directory.GetDirectories(sourceFolder);
        foreach (string folder in folders)
        {
            string name = System.IO.Path.GetFileName(folder);
            string dest = System.IO.Path.Combine(destFolder, name);
            CopyFolder(folder, dest);
        }
    }

    private async void btn_Copy_Click(object sender, RoutedEventArgs e)
    {
        if (lBox_First.SelectedIndex != -1)
        {
            try
            {
                if (lBox_First.SelectedItem is FileInfo)
                {
                    string sourcePath = (lBox_First.SelectedItem as FileInfo).FullName;
                    string targetPath = tBox_Second.Text + "\\" + (lBox_First.SelectedItem as FileInfo).Name;

                    File.Copy(sourcePath, targetPath, true);
                }
                else if (lBox_First.SelectedItem is DirectoryInfo)
                {
                    string sourcePath = (lBox_First.SelectedItem as DirectoryInfo).FullName;
                    string targetPath = tBox_Second.Text + "\\" + (lBox_First.SelectedItem as DirectoryInfo).Name;
                    CopyFolder(sourcePath, targetPath);
                }
                await Task.Delay(3000);
                MessageBox.Show("Sucessfully Copied!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        else
        {
            MessageBox.Show("Select Item From ListBox");
        }
    }
}
