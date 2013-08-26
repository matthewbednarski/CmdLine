/*
 * Matthew Carl Bednarski <matthew.bednarski@ekr.it>
 * 25/05/2012 - 10.07
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CmdLine
{
	public class Args
	{
		#region Command Line Options
		private List<Option> _options;
		public List<Option> Options
		{
			get{
				if(_options == null)
				{
					_options = new List<Option>();
					_options.Add(new Option('?', "help", "print usage", true));
				}
				return _options;
			}
		}
		private StringBuilder _usage;
		public virtual string Usage()
		{
			if(_usage == null)
			{
				_usage = new StringBuilder();
			}else{
				_usage.Clear();
			}
			_usage.AppendFormat("{0} [options]", System.IO.Path.GetFileName( System.Reflection.Assembly.GetEntryAssembly().Location)).AppendLine();;
			var flags = Options.Where(xx => xx.IsFlag == true);
			_usage.Append("Flags").AppendLine();
			foreach(var flag in flags)
			{
				_usage.AppendFormat("    {0}", flag.GetHelpLine()).AppendLine();
			}
			_usage.AppendLine();
			var opts = Options.Where(xx => xx.IsFlag == false);
			if(opts != null && opts.Count() > 0)
			{
				_usage.Append("Options").AppendLine();
				foreach(var opt in opts)
				{
					_usage.AppendFormat("    {0}", opt.GetHelpLine()).AppendLine();
				}
				_usage.AppendLine();
			}
			return _usage.ToString();
		}
		#endregion //Command Line Options
		
		
		private Dictionary<string, string> _paramsDict;
		public Dictionary<string, string> ParamsDict
		{
			get{
				return _paramsDict;
			}
		}
		private List<string> _argsList;
		public List<string> ArgsList
		{
			get{
				return _argsList;
			}
			set{
				_argsList = value;
			}
		}
		public String this [string index]
		{
			get{
				
				if(ParamsDict.ContainsKey(index))
				{
					return ParamsDict[index];
				}else{
					return "";
				}
			}
		}
		public Args(string[] args)
		{
			ArgsList = new List<string>();
			foreach(string s in args)
			{
				ArgsList.Add(s.TrimStart('-','/'));
			}
			SetParamsDict(args);
		}
		public void SetParamsDict(IEnumerable<string> args)
		{
			_paramsDict = new Dictionary<string, string>();
			List<string> t = new List<string>(args);
			for(int i = 0; i < args.Count(); i++)
			{
				if(args.ArgIsParameter(i) && args.ArgParameterHasValue(i))
				{
					try{
						string key = t[i].ArgAsParameter();
						string val =  args.ArgParameterValue(i);
						ParamsDict.Add(key, val);
						Console.WriteLine("{0} : -> : {1}", key, val);
					}catch(Exception ex)
					{
						ex.ToString();
						Console.WriteLine("{0} : -> : {1}", t[i].ArgAsParameter(), args.ArgParameterHasValue(i));
					}
				}
			}
		}
	}
	public static class ArgsExtensions
	{
		public static Object GetOptionValue(this Option option, Args args)
		{
			return Option.GetValue(args, option);
			
		}
		#region Command Line Helpers
		public static bool ArgParameterHasValue(this IEnumerable<string> args, int index)
		{
			if(index + 2 <= args.Count())
			{
				List<string> t = new List<string>(args);
				if(!String.IsNullOrWhiteSpace(t[index + 1]) && !args.ArgIsParameter(index + 1))
				{
					return true;
				}
				else{
					return false;
				}
			}
			else
			{
				return false;
			}
		}
		public static string ArgParameterValue(this IEnumerable<string> args, int index)
		{
			string r = null;
			if(index + 2 <= args.Count())
			{
				List<string> t = new List<string>(args);
				if(!String.IsNullOrWhiteSpace(t[index + 1]) && !args.ArgIsParameter(index + 1))
				{
					r = t[index + 1];
					Console.WriteLine("Value of r: \"{0}\"...", r);
				}
			}
			return r;
		}
		public static bool ArgIsParameter(this IEnumerable<string> args, int index)
		{
			if(index + 1 <= args.Count())
			{
				List<string> t = new List<string>(args);
				if(t[index].StartsWith("-") || t[index].StartsWith("/"))
				{
					return true;
				}
				else{
					return false;
				}
			}else{
				return false;
			}
		}
		public static string ArgAsParameter(this string arg)
		{
			string r = arg;
			r = r.TrimStart('-','/');
			return r;
		}
		public static bool ArgIsParameter(this string arg)
		{
			if(arg.StartsWith("-") || arg.StartsWith("/"))
			{
				return true;
			}
			else{
				return false;
			}
		}
		public static bool ArgUsed(this IEnumerable<string> args, string arg)
		{
			string argMinus = String.Format("-{0}", arg);
			string argBar = string.Format("/{0}", arg);
			bool r = false;
			foreach(string targ in args)
			{
				if(targ.EndsWith(argMinus) || targ.EndsWith(argBar))
				{
					if(targ.Length == argMinus.Length + 1)
					{
						r = true;
						break;
					}
				}else if(targ.Equals(arg))
				{
					r = true;
					break;
				}
			}
			return r;
		}
		public static bool ArgUsed(this Args args, string arg)
		{
			bool r = false;
			if(args.ParamsDict.ContainsKey(arg))
			{
				r = true;
			}
			else if(args.ArgsList.Contains(arg))
			{
				r = true;
			}
			return r;
		}
//		public static string GetArgParameter(this IEnumerable<string> args, string arg)
//		{
//			string argMinus = String.Format("-{0}", arg);
//			string argBar = string.Format("/{0}", arg);
//			string param = "";
//			int farg = 0;
//			List<string> t = new List<string>(args);
//			for(int i = 0; i < t.Count; i++)
//			{
//				if((t[i] == argMinus || t[i] == argBar) && t.Count > i + 1)
//				{
//					param = t[i + 1];
//					break;
//				}
//			}
//			return param;
//		}
		#endregion
		
	}
}