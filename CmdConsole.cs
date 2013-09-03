/*
 * Matthew Carl Bednarski <matthew.bednarski@ekr.it>
 * 06/07/2012 - 14.44
 */

using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CmdLine
{
	/// <summary>
	/// Description of Console.
	/// </summary>
	public class CmdConsole
	{
		private static Type t = Type.GetType ("Mono.Runtime");
		public static Boolean IsMono
		{
			get{
				if (t != null)
					return true;
				else
					return false;
			}
		}
		/**
		 * http://sanity-free.org/143/csharp_dotnet_single_instance_application.html
		 * */
		private static Mutex mutex;
		public static Mutex Mutex
		{
			get{
				if(mutex == null)
				{
					mutex  = new Mutex(true, "{9D4F5184-8ED4-4417-921C-2D4E4C537382}");
				}
				return mutex;
			}
		}
		private static string lock_file_pid;
		private static Boolean _hasLock;
		public static Boolean HasLock
		{
			get{
				LockFile.ToString();
				return _hasLock;
			}
		}
		public static Boolean GetLock()
		{
			bool r = false;
			if(CmdLine.CmdConsole.IsMono)
			{
				if(CmdLine.CmdConsole.HasLock)
				{
					r = true;
				}
			} else {
				if(CmdLine.CmdConsole.Mutex.WaitOne(TimeSpan.Zero, true)) {
					r = true;
				}else{
					r = false;
				}
			}
			return r;
		}
		
		public static void ReleaseLock()
		{
			if(CmdLine.CmdConsole.IsMono)
			{
				File.Delete(LockFile.FullName);
			} else {
				try{
				}finally{
					CmdLine.CmdConsole.Mutex.ReleaseMutex();
				}
			}
		}
		private static FileInfo lock_file;
		private static FileInfo LockFile
		{
			get{
				if(lock_file == null)
				{
					lock_file  = new FileInfo(Path.Combine(System.Environment.SpecialFolder.Personal.ToString(), ".mt.lock"));
					if(lock_file.Exists)
					{
						if(lock_file_pid == null)
						{
							using(FileStream fs = new FileStream(lock_file.FullName, FileMode.Open, FileAccess.Read))
							{
								using(StreamReader sr = new StreamReader(fs))
								{
									
									lock_file_pid = sr.ReadToEnd();
									lock_file_pid = lock_file_pid.Trim();
								}
							}
							if(isInCurrentProcess(lock_file_pid))
							{
								_hasLock = true;
								return lock_file;
							}
							else if(!processRunning(lock_file_pid))
							{
								lock_file_pid = Process.GetCurrentProcess().Id.ToString();
								File.Delete(lock_file.FullName);
								using(FileStream fs = new FileStream(lock_file.FullName, FileMode.Create, FileAccess.Write))
								{
									using(StreamWriter sw = new StreamWriter(fs))
									{
										sw.Write(lock_file_pid);
									}
								}
								_hasLock = true;
							}else{
								_hasLock = false;
							}
						}
						return lock_file;
					} else {
						lock_file_pid = Process.GetCurrentProcess().Id.ToString();
						using(FileStream fs = new FileStream(lock_file.FullName, FileMode.Create, FileAccess.Write))
						{
							using(StreamWriter sw = new StreamWriter(fs))
							{
								sw.Write(lock_file_pid);
							}
						}
						_hasLock = true;
					}
					return lock_file;
				}
				return lock_file;
			}
		}
		private static Boolean isInCurrentProcess(string s_pid)
		{
			int pid = Convert.ToInt32(s_pid.Trim());
			int c_p = Process.GetCurrentProcess().Id;
			if(c_p.Equals(pid))
			{
				return true;
			}else{
				return false;
			}
		}
		private static Boolean processRunning(string s_pid)
		{
			int pid = Convert.ToInt32(s_pid.Trim());
			Process p = Process.GetProcessById(pid);
			if(p != null)
			{
				return true;
			}else{
				return false;
			}
		}
		
		public CmdConsole()
		{
		}
		public static void Info(string s, params Object[] o)
		{
			WriteLine("Info", s, o);
		}
		public static void Warn(string s, params Object[] o)
		{
			WriteLine("Warn", s, o);
		}
		public static void Error(string s, params Object[] o)
		{
			WriteLine("Error", s, o);
		}
		private static void WriteLine(string pref, string s, params Object[] o)
		{
			s = pref + ": " + s;
			System.Console.Out.WriteLine(s, o);
		}
		/*
		 * <see cref="http://stackoverflow.com/questions/8946808/can-console-clear-be-used-to-only-clear-a-line-instead-of-whole-console"></see>
		 * */
		public static void ClearCurrentConsoleLine()
		{
			Console.Write(new string(' ', Console.WindowWidth));
			Console.SetCursorPosition(0, Console.CursorTop);
		}
		public static void ClearLastNConsoleLines(int n, bool toWindowTopOnly = true)
		{
			int currentLineCursor = Console.CursorTop;
			int windowTop = 0;
			if(toWindowTopOnly)
			{
				windowTop = Console.WindowTop;
			}
			for(int i = n ; i >= windowTop;i--)
			{
				Console.SetCursorPosition(0, i);
				ClearCurrentConsoleLine();

			}
		}
	}
}
