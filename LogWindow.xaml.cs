using Microsoft.Win32;
using System;
using System.Linq;
using System.Text;
using System.Windows;

namespace ArisHotel
{
	public partial class LogWindow : Window
	{
		public LogWindow()
		{
			InitializeComponent();
			LoadExisting();
			LogService.Instance.LogAdded += OnLogAdded;
			this.Closed += (s, e) => LogService.Instance.LogAdded -= OnLogAdded;
		}

		private void LoadExisting()
		{
			var snapshot = LogService.Instance.GetSnapshot();
			foreach (var entry in snapshot)
			{
				lstLogs.Items.Add(entry.ToString());
			}
			ScrollToEnd();
		}

		private void OnLogAdded(LogEntry entry)
		{
			Dispatcher.Invoke(() =>
			{
				lstLogs.Items.Add(entry.ToString());
				ScrollToEnd();
			});
		}

		private void ScrollToEnd()
		{
			if (lstLogs.Items.Count > 0)
			{
				lstLogs.ScrollIntoView(lstLogs.Items[lstLogs.Items.Count - 1]);
			}
		}

		private void BtnCopy_Click(object sender, RoutedEventArgs e)
		{
			var text = string.Join(Environment.NewLine, lstLogs.Items.Cast<object>().Select(x => x.ToString()));
			Clipboard.SetText(text);
		}

		private void BtnSave_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new SaveFileDialog
			{
				Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
				FileName = $"logs_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
			};
			if (dialog.ShowDialog() == true)
			{
				LogService.Instance.SaveToFile(dialog.FileName);
			}
		}

		private void BtnClose_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}
	}
}


