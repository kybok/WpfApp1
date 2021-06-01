using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public void MainMethod()
        {
            var _listDll = new List<string>();
            var result = MessageBoxResult.Yes;
            while(result == MessageBoxResult.Yes)
            {
                if (GetFiles(out _listDll))
                {
                    result = MessageBoxResult.No;
                }
                else
                {
                    result = MessageBox.Show("В этой папке DLL сборок не найдено!", "Сообщение", MessageBoxButton.YesNo, MessageBoxImage.Question);                    
                }
            }
            AssemblyDlls(_listDll);
        }
        public bool GetFiles(out List<string> filesList)
        {
            filesList = null;
            var dlg = new CommonOpenFileDialog()
            {
                Title = "Выберите папку с файлами DLL сборок",
                IsFolderPicker = true,
                Multiselect = false
            };


            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                filesList = new DirectoryInfo(dlg.FileName).GetFiles()
                    .Where(d => System.IO.Path.GetExtension(d.FullName)
                    .Contains("dll")).Select(d => d.FullName).ToList();
                if (filesList.Count == 0) return false;
            }
            return true;
        }

        public void AssemblyDlls(List<string> dlls)
        {
            foreach (var dll in dlls)
            {
                var assembly = Assembly.LoadFile(dll);
                var treeDll = new TreeViewItem() {Header = assembly.ManifestModule.Name };
                try
                {
                    var getClasses = assembly.GetTypes().Where(c => c.IsClass).Select(c => c);
                    foreach (var _class in getClasses)
                    {
                        var treeClass = new TreeViewItem() { Header = _class.Name };
                        treeDll.Items.Add(treeClass);
                        var publicMethods = _class.GetMethods().Where(m => m.IsPublic).Select(m => m);
                        var treePublic = new TreeViewItem() { Header = "Public методы" };
                        foreach (var method in publicMethods)
                        {
                            treePublic.Items.Add(method.Name);
                        }
                        var protectedMethods = _class.GetMethods().Where(m => m.IsFamily).Select(m => m);
                        var treeProtected = new TreeViewItem() { Header = "Protected методы" };
                        foreach (var method in protectedMethods)
                        {
                            treeProtected.Items.Add(method.Name);
                        }
                        treeClass.Items.Add(treePublic);
                        treeClass.Items.Add(treeProtected);
                    }
                    treeView1.Items.Add(treeDll);
                }
                catch (Exception)
                {

                }        
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainMethod();
        }
    }
}
