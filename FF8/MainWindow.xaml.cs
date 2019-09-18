using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			if (!System.IO.File.Exists(filePath)) return;

			if (Export.IsChecked == true)
			{
				Byte[] buffer = System.IO.File.ReadAllBytes(filePath);
				uint fileSize = BitConverter.ToUInt32(buffer, 4) + 4;
				Byte[] output = new Byte[fileSize];
				Array.Copy(buffer, 4, output, 0, output.Length);

				SaveFileDialog dlg = new SaveFileDialog();
				dlg.Filter = "PC Save|*.ff8";
				if (dlg.ShowDialog() == false) return;
				System.IO.File.WriteAllBytes(dlg.FileName, output);
			}
			else
			{
				OpenFileDialog dlg = new OpenFileDialog();
				dlg.Filter = "PC Save|*.ff8";
				if (dlg.ShowDialog() == false) return;
				Byte[] input = System.IO.File.ReadAllBytes(dlg.FileName);
				Byte[] buffer = System.IO.File.ReadAllBytes(filePath);

				Byte[] output = new Byte[buffer.Length - BitConverter.ToUInt32(buffer, 0) + input.Length];
				int count = 0;
				foreach (var val in BitConverter.GetBytes(input.Length))
				{
					output[count++] = val;
				}
				Array.Copy(input, 0, output, 4, input.Length);
				Array.Copy(buffer, BitConverter.ToUInt32(buffer, 0) + 4, output, input.Length + 4, buffer.Length - BitConverter.ToUInt32(buffer, 0) - 4);
				System.IO.File.WriteAllBytes(filePath, output);
			}
		}
	}
}
