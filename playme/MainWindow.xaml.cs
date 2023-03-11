using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace playme
{
    
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void musSlid_Changed(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            music.Position = new TimeSpan(Convert.ToInt64(musSlid.Value));
            
        }
        private void mus_Opened(object sender, RoutedEventArgs e)
        {
            int old = play.SelectedIndex;
            bool play_bool = false;
            musSlid.Maximum = music.NaturalDuration.TimeSpan.Ticks;
            Thread thread = new Thread(_ =>
            {
                while (play_bool != true)
                {
                    Application.Current.Dispatcher.Invoke(new Action(() =>
                    {
                        musSlid.Value = music.Position.Ticks;
                        if (musSlid.Value == musSlid.Maximum)
                        {
                            play_bool = true;
                            musSlid.Value = 0;
                            if (play.SelectedIndex == -1)
                            {
                                play.SelectedIndex += 2;
                            }
                            else
                            {
                                play.SelectedIndex += 1;
                            }
                        }
                        if (play.SelectedIndex != old)
                        {
                            play_bool = true;
                            musSlid.Value = 0;
                        }
                    }));
                    Thread.Sleep(100);
                }
            });
            thread.Start();

        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog {IsFolderPicker = true};
            var result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                string[] files = Directory.GetFiles(dialog.FileName);
                play.Items.Clear();
                string[] musics = new string[files.Length-1];
                int y = 0;
                for (int i=1; i < musics.Length; i++)
                {
                    if ((files[i])[files[i].Length-3].ToString() + (files[i])[files[i].Length - 2].ToString() + 
                        (files[i])[files[i].Length - 1].ToString() == "mp3" || 
                        (files[i])[files[i].Length - 3].ToString() + (files[i])[files[i].Length - 2].ToString() + 
                        (files[i])[files[i].Length - 1].ToString() == "mp4a" || (files[i])[files[i].Length - 3].ToString() + 
                        (files[i])[files[i].Length - 2].ToString() + (files[i])[files[i].Length - 1].ToString() == "wav")
                    {
                        musics[y] = files[i];
                        y++;
                    }
                }
                play.ItemsSource = musics;
                
                music.Source = new Uri(play.Items[0].ToString());
                music.Play();
            }
        }

        private void play_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = play.SelectedItem.ToString();
            music.Source = new Uri(selected);
            music.Play();
        }
    }
}
