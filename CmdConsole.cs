/*
 * Matthew Carl Bednarski <matthew.bednarski@ekr.it>
 * 06/07/2012 - 14.44
 */

using System;

namespace CmdLine
{
	/// <summary>
	/// Description of Console.
	/// </summary>
	public class CmdConsole
	{
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
	}
}
