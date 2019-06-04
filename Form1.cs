using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Shell;

namespace PlaylistCreator
{
    public partial class Form1 : Form
    {
        static string m3uFile = "";
        List<Song> songs = new List<Song>();

        public Form1()
        {
            InitializeComponent();
            this.listBox1.AllowDrop = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            load();
        }

        private void load()
        {
            songs = LoadPlaylist();
            listBox1.Items.Clear();
            for (int i = 0; i < songs.Count; i++)
            {
                listBox1.Items.Add(songs[i].name);
            }
        }

        private List<Song> LoadPlaylist()
        {
            if (m3uFile != "")
            {
                songs = new List<Song>();
                StreamReader sr = new StreamReader(m3uFile);

                while (!sr.EndOfStream)
                {
                    int fileNameLength = m3uFile.Split('\\').Last().Length;
                    string absolute = m3uFile.Remove(m3uFile.Length - fileNameLength, fileNameLength);
                    string path = sr.ReadLine();
                    if (path.Contains(".mp3") && !path.Contains("#EXTINF"))
                    {
                        int duration = GetDSSDuration(path);

                        String name = path.Split('\\').Last();
                        name = name.Substring(0, name.Length - 4);

                        Song song = new Song(duration, name, path);
                        songs.Add(song);
                    }
                }
                sr.Close();
            }
            return songs;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int time = 0;
            for (int i = 0; i <= listBox1.SelectedIndex; i++)
            {
                time += songs[i].seconds;
            }

            TimeSpan t = new TimeSpan(0, 0, time);
            label1.Text = t.ToString();
        }

        public static int GetDSSDuration(string fileName)
        {
            ShellFile so = ShellFile.FromFilePath(fileName);
            double nanoseconds;
            double.TryParse(so.Properties.System.Media.Duration.Value.ToString(), out nanoseconds);

            // double milliseconds = nanoseconds * 0.000001;
            double seconds = Convert100NanosecondsToMilliseconds(nanoseconds) / 1000;


            return (int)seconds;
        }
        public static double Convert100NanosecondsToMilliseconds(double nanoseconds)
        {
            // One million nanoseconds in 1 millisecond, but we are passing in 100ns units...
            return nanoseconds * 0.0001;
        }

        private void button1_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem != null && listBox1.SelectedIndex > 0)
            {
                int index = listBox1.SelectedIndex;

                Song temp = songs[index];
                songs[index] = songs[index - 1];
                songs[index - 1] = temp;
                move(-1, index);
            }
        }

        private void button2_MouseClick(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedItem != null && listBox1.SelectedIndex < songs.Count)
            {
                int index = listBox1.SelectedIndex;

                Song temp = songs[index];
                songs[index] = songs[index + 1];
                songs[index + 1] = temp;
                move(1, index);
            }
        }

        public void move(int direction, int index)
        {
            string s = listBox1.Items[index].ToString();
            listBox1.Items.RemoveAt(index);
            listBox1.Items.Insert(index + direction, s);
            listBox1.SelectedIndex = index + direction;
        }

        private void button3_MouseClick(object sender, MouseEventArgs e)
        {
            if (m3uFile == "")
            {
                SaveFileDialog svd = new SaveFileDialog();
                svd.DefaultExt = "m3u";
                if (svd.ShowDialog() == DialogResult.OK)
                {
                    m3uFile = svd.FileName;
                }
            }

            if (m3uFile != "")
            {
                StreamWriter sw = new StreamWriter(m3uFile, false);

                sw.WriteLine("#EXTM3U");

                for (int i = 0; i < songs.Count; i++)
                {
                    sw.WriteLine(songs[i].path);
                }
                sw.Close();
            }
        }

        private void button4_MouseClick(object sender, MouseEventArgs e)
        {
            songs.RemoveAt(listBox1.SelectedIndex);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);
        }

        private void scegliFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            if (opf.ShowDialog() == DialogResult.OK)
            {
                if (opf.FileName.Contains(".m3u"))
                {
                    m3uFile = opf.FileName;
                    load();
                }
            }
        }

        private void button5_MouseClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog opf = new OpenFileDialog();
            opf.Multiselect = true;
            if (opf.ShowDialog() == DialogResult.OK)
            {
                for (int i = 0; i < opf.FileNames.Length; i++)
                {
                    if (opf.FileNames[i].Contains(".mp3"))
                    {
                        Song s = new Song();
                        s.name = opf.FileNames[i].Split('\\').Last();
                        s.path = opf.FileNames[i];
                        s.seconds = GetDSSDuration(opf.FileNames[i]);
                        songs.Add(s);

                        listBox1.Items.Add(s.name);
                    }
                }
            }
        }

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.listBox1.SelectedItem == null) return;
            this.listBox1.DoDragDrop(this.listBox1.SelectedItem, DragDropEffects.Move);
        }

        private void listBox1_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {

            Point point = listBox1.PointToClient(new Point(e.X, e.Y));

            int from = listBox1.SelectedIndex;
            int to = listBox1.IndexFromPoint(point);

            if (to < 0) to = listBox1.Items.Count - 1;

            object data = listBox1.SelectedItem;

            Song s = songs[from];
            songs.RemoveAt(from);
            songs.Insert(to, s);

            listBox1.Items.Remove(data);
            listBox1.Items.Insert(to, data);

            listBox1.SetSelected(to, true);
        }
    }
}
