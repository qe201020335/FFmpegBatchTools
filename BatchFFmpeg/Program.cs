using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FFMpegCore;
using FFMpegCore.Enums;
using Newtonsoft.Json;

namespace BatchFFmpeg
{
    internal class Program
    {
        private const string SUFFIX = "_extracted";
        private static bool _useSelectedDir = false;
        private static string _selectedDir;
        private static FolderBrowserDialog _outputDirDialog = new FolderBrowserDialog();

        private static async Task Run(string path)
        {
            var nameWithoutExtension = Path.GetFileNameWithoutExtension(path);
            var parent = Path.GetDirectoryName(path);
            var extension = Path.GetExtension(path);
            string newpath;
            if (_useSelectedDir)
            {
                newpath = _selectedDir + "\\" + nameWithoutExtension + (_selectedDir == parent ? SUFFIX : "") + extension;
            }
            else
            {
                newpath = parent + "\\" + nameWithoutExtension + SUFFIX + extension;
            }


            try
            {
                await FFMpegArguments.FromFileInput(path)
                    .OutputToFile(newpath, true,
                        options => options.CopyChannel(Channel.Audio)
                            .WithVideoCodec("libx264").WithConstantRateFactor(23).WithSpeedPreset(Speed.VerySlow))
                    .ProcessAsynchronously();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [STAThread]
        public static void Main(string[] args)
        {
            var ass = Assembly.GetExecutingAssembly();
            var prgname = ass.GetName().Name;

            _outputDirDialog.SelectedPath = Path.GetDirectoryName(ass.Location) ?? "";
            if (_outputDirDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(_outputDirDialog.SelectedPath);
                _useSelectedDir = true;
                _selectedDir = _outputDirDialog.SelectedPath;
            }

            int len = args.Length;
            if (len == 0)
            {
                MessageBox.Show("Drag video files on this tool.", prgname);
                return;
            }

            foreach (string arg in args)
            {
                try
                {
                    if (!File.Exists(arg))
                    {
                        Console.WriteLine(arg + " DOES NOT EXIST");
                        continue; // not exist
                    }

                    var fullpath = Path.GetFullPath(arg);
                    Run(fullpath).Wait();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            Console.Write("\a");
            MessageBox.Show("Done.", prgname);
        }
    }
}