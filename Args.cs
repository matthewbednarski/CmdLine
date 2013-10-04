/**
 * Matthew Carl Bednarski <matthew.bednarski@ekr.it>
 * 25/05/2012 - 10.07
 */

using System;
using System.Collections.Generic;

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
				_usage.Length = 0;
				_usage.Capacity = 1;
			}
			_usage.AppendLine();
			_usage.AppendFormat("{0} [options]", System.IO.Path.GetFileName( System.Reflection.Assembly.GetEntryAssembly().Location)).AppendLine();
			List<Option> flags = new List<Option>();
			List<Option> opts = new List<Option>();
			foreach(Option o in Options)
			{
				if(o.IsFlag)
				{
					flags.Add(o);
				}else{
					opts.Add(o);
				}
			}
			
			_usage.AppendLine();
			_usage.Append("Flags").AppendLine();
			foreach(var flag in flags)
			{
				_usage.AppendFormat("{0}", flag.GetHelpLine()).AppendLine();
			}
			_usage.AppendLine();
			if(opts != null && opts.Count > 0)
			{
				_usage.AppendLine();
				_usage.Append("Options").AppendLine();
				foreach(var opt in opts)
				{
					_usage.AppendFormat("{0}", opt.GetHelpLine()).AppendLine();
				}
				_usage.AppendLine();
			}
			_usage.AppendLine();
			string r = _usage.ToString();
			return r;
		}
		#endregion //Command Line Options
		
		private string[] args_original;
		
		private Dictionary<string, string> _paramsDict;
		public Dictionary<string, string> ParamsDict
		{
			get{
				return _paramsDict;
			}
		}
		private List<string> _argsList;
		private List<string> ArgsList
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
		public String this [int index]
		{
			get{
				
				if(ArgsList != null && ArgsList.Count > index)
				{
					return ArgsList[index];
				}else{
					return "";
				}
			}
		}
		public Args(string[] args)
		{
			args_original = args;
			ArgsList = new List<string>();
			foreach(string s in args)
			{
				ArgsList.Add(s);
			}
			SetParamsDict(args);
		}
		private void SetParamsDict( IList<string> args)
		{
			_paramsDict = new Dictionary<string, string>();
			for(int i = 0; i < args.Count; i++)
			{
				if(ArgIsParameter(args, i) && ArgParameterHasValue(args, i))
				{
					try{
						string key = ArgAsParameter(args[i]);
						string val =  ArgParameterValue(args, i);
						ParamsDict.Add(key, val);
						#if DEBUG
						Console.WriteLine("{0} : -> : {1}", key, val);
						#endif
					}catch(Exception ex)
					{
						ex.ToString();
						Console.WriteLine("{0} : -> : {1}",ArgAsParameter( args[i]), ArgParameterHasValue(args, i));
					}
				}
				else if( ArgIsParameter(args, i))
				{
					try{
						string key = ArgAsParameter(args[i]);
						string val =  Convert.ToString(true);
						ParamsDict.Add(key, val);
						#if DEBUG
						Console.WriteLine("{0} : -> : {1}", key, val);
						#endif
					}catch(Exception ex)
					{
						ex.ToString();
						Console.WriteLine("{0} : -> : {1}",ArgAsParameter( args[i]), ArgParameterHasValue(args, i));
					}
				}
			}
		}
		public Option GetOption(char where)
		{
			return GetOption(where.ToString());
		}
		public Option GetOption(String where)
		{
			IList<Option> options = this.GetOptions(where);
			if(options.Count > 0)
			{
				return options[0];
			}else {
				return null;
			}
		}
		public IList<Option> GetOptions(String where)
		{
			List<Option> options = new List<Option>();
			foreach(Option o in this.Options)
			{
				if(o.Name.ToLower().Equals(where.ToLower()))
				{
					options.Add(o);
				} else if(where.Length == 1)
				{
					if(o.Short_name != null && where[0].Equals(o.Short_name))
					{
						options.Add(o);
					}
				}
			}
			foreach(Option o in this.Options)
			{
				if(o.Name.ToLower().Contains(where.ToLower()))
				{
					options.Add(o);
				} else if(where.Length == 1)
				{
					if(o.Short_name != null)
					{
						string tocomp = o.Short_name.ToString();
						if(where.ToLower()[0].Equals(tocomp.ToLower()))
						{
							options.Add(o);
						}
					}
				}
			}
			return options;
		}
		public void AddOption(Option to_add)
		{
			this.Options.Add(to_add);
			Object val = to_add.GetOptionValue(this);
			if(val != null)
			{

				to_add.ActualValue = to_add.GetOptionValue(this).ToString();
			}
		}
		
		private static string ArgAsParameter(string arg)
		{
			string r = arg;
			r = r.TrimStart('-','/');
			if(r.Contains("=")){
				r = r.Split(new char[]{'='}, 2)[0];
			}
			return r;
		}
		private static bool ArgIsParameter(string arg)
		{
			if(arg.StartsWith("-") || arg.StartsWith("/"))
			{
				return true;
			}
			else{
				return false;
			}
		}
		private static bool ArgIsParameter( IList<string> args, int index)
		{
			bool r = false;
			if(index + 1 <= args.Count)
			{
				return ArgIsParameter(args[index]);
			}
			return r;
		}
		private static bool ArgParameterHasValue(IList<string> args, int index)
		{
			bool r = false;
			if(ArgIsParameter(args, index))
			{
				string part = args[index];
				if(!String.IsNullOrEmpty(part) && (part.StartsWith("-") || part.StartsWith("/")))
				{
					if(part.Contains("="))
					{
						string[] parts = part.Split(new char[]{ '='}, 2);
						if(parts != null && parts.Length == 2)
						{
							return true;
						}
					}else{
						if(index + 2 <= args.Count)
						{
							if(!String.IsNullOrEmpty(args[index + 1]) && !ArgIsParameter(args, index + 1))
							{
								return true;
							}
						}
					}
				}
			}
			return r;
		}
		private static string ArgParameterValue( IList<string> args, int index)
		{
			string r = null;
			if(index + 1 <= args.Count)
			{
				string arg = args[index];
				if(ArgIsParameter(arg))
				{
					if(arg.Contains("="))
					{
						r = arg.Split(new char[]{'='}, 2)[1];
						#if DEBUG
						Console.WriteLine("Value of r: \"{0}\"...", r);
						#endif
					}else if(!String.IsNullOrEmpty(args[index + 1]) && !ArgIsParameter(args, index + 1))
					{
						r = args[index + 1];
						#if DEBUG
						Console.WriteLine("Value of r: \"{0}\"...", r);
						#endif
					}
				}
			}
			return r;
		}
		
		public static bool ArgUsed(string[] args, string arg)
		{
			string argMinus = String.Format("-{0}", arg);
			string argBar = string.Format("/{0}", arg);
			bool r = false;
			foreach(string targ in args)
			{
				if(targ.EndsWith(argMinus) || targ.EndsWith(argBar))
				{
					if(targ.Length == argMinus.Length)
					{
						r = true;
						break;
					}
				}
				if(targ.Equals(arg))
				{
					r = true;
					break;
				}
				if(targ.TrimStart(new char[]{'-', '/'}).StartsWith(arg.TrimStart(new char[]{'-', '/'} ) + "=" ) )
				{
					r = true;
					break;
				}
				if(targ.TrimStart(new char[]{'-', '/'}).Equals(arg.TrimStart(new char[]{'-', '/'} ) ) )
				{
					r = true;
					break;
				}
			}
			return r;
		}
		public bool ArgUsed(string arg)
		{
			bool r = false;
			r = ArgUsed(this.args_original, arg);
			return r;
		}
		public bool ArgUsed(Option option)
		{
			bool r = false;
			if(option.Short_name != null)
			{
				r = ArgUsed(option.Short_name.ToString());
			}
			if(!r)
			{
				r = ArgUsed(option.Name);
			}
			return r;
		}
	}
}
