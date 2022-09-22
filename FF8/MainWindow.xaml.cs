using System;
using System.IO;
using System.Linq;
using System.Windows;
using Microsoft.Win32;

namespace FF8
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void ButtonPath_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog dlg = new OpenFileDialog();
			if (dlg.ShowDialog() == false) return;

			FilePath.Text = dlg.FileName;
		}

		private void ButtonRun_Click(object sender, RoutedEventArgs e)
		{
			String filePath = FilePath.Text;
			if (Export.IsChecked == true && chkTinfoil.IsChecked == true)
			{
				if (Directory.Exists(filePath))
				{
					var rslt = ExportFromTinfoil(filePath);
					if (rslt)
					{
						MessageBox.Show("Exported");
					}
					return;
				}
			}

			if (!System.IO.File.Exists(filePath))
			{
				MessageBox.Show("Please select a file");
				return;
			}

			if (Export.IsChecked == true)
			{
				SaveFileDialog dlg = new SaveFileDialog();
				dlg.Filter = "PC Save|*.ff8";
				if (dlg.ShowDialog() == false) return;

				ExportFile(filePath, dlg.FileName);
			}
			else
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Filter = "PC Save|*.ff8";
				if (dlg.ShowDialog() == false) return;
				Byte[] input = File.ReadAllBytes(dlg.FileName);
				Byte[] buffer = File.ReadAllBytes(filePath);

				Byte[] output = new Byte[buffer.Length - BitConverter.ToUInt32(buffer, 0) + input.Length];
				int count = 0;
				foreach (var val in BitConverter.GetBytes(input.Length))
				{
					output[count++] = val;
				}
				Array.Copy(input, 0, output, 4, input.Length);
				Array.Copy(buffer, BitConverter.ToUInt32(buffer, 0) + 4, output, input.Length + 4, buffer.Length - BitConverter.ToUInt32(buffer, 0) - 4);
				File.WriteAllBytes(filePath, output);
			}
		}

		private bool ExportFromTinfoil(string filePath)
		{
			var newPath = System.IO.Path.Combine(filePath, "exported");
			if (!Directory.Exists(newPath))
			{
				Directory.CreateDirectory(newPath);
			}
			var files = Directory.GetFiles(filePath);

			if (files.Count() > 60)
			{
				MessageBox.Show("More than 60 files detected. This is not supported");
				return false;
			}

			var x = 2;
			var slot = "slot1";

			foreach (var file in files)
			{
				var fi = new FileInfo(file);

				var newFileName = System.IO.Path.Combine(newPath, $"{slot}_save{x.ToString().PadLeft(2, '0')}.ff8");
				newFileName = newFileName.Replace("ff8slot", $"{slot}_save");

				ExportFile(file, newFileName);
				x++;
				if (x > 30)
				{
					x = 2; slot = "slot2";
				}
			}

			return true;
		}

		private void ExportFile(string input, string output)
		{
			Byte[] buffer = System.IO.File.ReadAllBytes(input);
			uint fileSize = BitConverter.ToUInt32(buffer, 4) + 4;
			Byte[] content = new Byte[fileSize];
			Array.Copy(buffer, 4, content, 0, content.Length);

			System.IO.File.WriteAllBytes(output, content);
		}
	}
}
