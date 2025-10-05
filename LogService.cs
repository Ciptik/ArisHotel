using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ArisHotel
{
	public enum LogLevel
	{
		Info,
		Warning,
		Error
	}

	public class LogEntry
	{
		public DateTime Timestamp { get; set; }
		public string UserName { get; set; }
		public LogLevel Level { get; set; }
		public string Action { get; set; }
		public string Details { get; set; }

		public override string ToString()
		{
			var user = string.IsNullOrWhiteSpace(UserName) ? "<system>" : UserName;
			return $"[{Timestamp:yyyy-MM-dd HH:mm:ss}] [{Level}] [{user}] {Action}{(string.IsNullOrWhiteSpace(Details) ? string.Empty : " — " + Details)}";
		}
	}

	public sealed class LogService
	{
		private static readonly Lazy<LogService> _instance = new Lazy<LogService>(() => new LogService());
		private readonly List<LogEntry> _entries = new List<LogEntry>();
		private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

		public static LogService Instance => _instance.Value;

		public event Action<LogEntry> LogAdded;

		private LogService() { }

		public IReadOnlyList<LogEntry> GetSnapshot()
		{
			_lock.EnterReadLock();
			try
			{
				return new List<LogEntry>(_entries);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}

		public void Log(LogLevel level, string action, string details = null)
		{
			var userName = Session.currentUser != null ? Session.currentUser.UserName : null;
			var entry = new LogEntry
			{
				Timestamp = DateTime.Now,
				UserName = userName,
				Level = level,
				Action = action,
				Details = details
			};

			_lock.EnterWriteLock();
			try
			{
				_entries.Add(entry);
			}
			finally
			{
				_lock.ExitWriteLock();
			}

			LogAdded?.Invoke(entry);
		}

		public void Info(string action, string details = null) => Log(LogLevel.Info, action, details);
		public void Warn(string action, string details = null) => Log(LogLevel.Warning, action, details);
		public void Error(string action, string details = null) => Log(LogLevel.Error, action, details);

		public void SaveToFile(string filePath)
		{
			_lock.EnterReadLock();
			try
			{
				var sb = new StringBuilder();
				foreach (var entry in _entries)
				{
					sb.AppendLine(entry.ToString());
				}
				File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
			}
			finally
			{
				_lock.ExitReadLock();
			}
		}
	}
}


