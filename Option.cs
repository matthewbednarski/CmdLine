/*
 * Created by SharpDevelop.
 * User: ekr
 * Date: 23/08/2013
 * Time: 17:45
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;

namespace CmdLine
{
	/// <summary>
	/// A class for managing CMD line Args options
	/// </summary>
	public class Option
	{
		char short_name;
		public char Short_name {
			get { return short_name; }
		}
		string name;
		public string Name {
			get { return name; }
			set { name = value; }
		}
		string description;
		public string Description {
			get { return description; }
		}
		bool isFlag;
		public bool IsFlag {
			get { return isFlag; }
			set {  this.isFlag = value; }
		}
		string default_value;
		public string DefaultValue {
			get { return default_value; }
		}
		string actual_value;
		public string ActualValue {
			get {
				if(actual_value == null && default_value != null)
				{
					return default_value;
				}
				return actual_value;
			}
			set {
				actual_value = value;
			}
		}
		public bool ActualValue_IsFlag
		{
			get{
				bool r = false;
				if(!String.IsNullOrEmpty(this.actual_value))
				{
					if(this.actual_value.ToLower().Equals(Boolean.TrueString.ToLower()) || this.actual_value.ToLower().Equals(Boolean.FalseString.ToLower()))
					{
						r = true;
					}
				}
				return r;
			}
		}
		public bool HasActualValue
		{
			get{ return ( !String.IsNullOrEmpty(actual_value) );}
		}
		public Option(string name, bool isFlag = false):this(name[0], name, "", isFlag, "")
		{
		}
		public Option(char short_name, bool isFlag = false):this(short_name, "", "", isFlag, "")
		{
		}
		public Option(char short_name, string name, bool isFlag = false):this(short_name, name, "", isFlag, "")
		{
		}
		public Option(char short_name, string name, string description, bool isFlag = false):this(short_name, name, description, isFlag, "")
		{
		}
		public Option(char short_name, string name, string description, bool isFlag, string default_value = "")
		{
			this.short_name = short_name;
			this.name = name;
			this.description = description;
			this.isFlag = isFlag;
			this.default_value = default_value;
		}
		
		public string GetHelpLine()
		{
			if(!String.IsNullOrEmpty(this.Name))
			{
				if(isFlag)
				{
					return String.Format("    -{0}, --{1}    {2}.", this.Short_name, this.Name, this.Description);
				}
				else if(!String.IsNullOrEmpty(this.DefaultValue))
				{
					return String.Format("    -{0} [value], --{1}=[value]    {2}. Default: {3}", this.Short_name, this.Name, this.Description, this.DefaultValue);
				}
				else
				{
					return String.Format("    -{0} [value], --{1}=[value]    {2}.", this.Short_name, this.Name, this.Description);
				}
			}else{
				if(isFlag)
				{
					return String.Format("    -{0}    {1}.", this.Short_name, this.Description);
				}
				else if(!String.IsNullOrEmpty(this.DefaultValue))
				{
					return String.Format("    -{0} [value]    {1}. Default: {2}", this.Short_name, this.Description, this.DefaultValue);
				}
				else
				{
					return String.Format("    -{0} [value]    {1}.", this.Short_name, this.Description);
				}
			}
		}
		
		public override string ToString()
		{
			return string.Format("[Option short_name={0}, name={1}, description={2}, isFlag={3}]", short_name, name, description, isFlag);
		}

		internal static Object GetValue(Args args, Option option)
		{
			if(option.IsFlag)
			{
				if(args.ParamsDict.ContainsKey(option.Name) || args.ParamsDict.ContainsKey(option.Short_name.ToString()))
				{
					return true;
				}
				else
				{
					return false;
				}
			}else{
				if(args.ParamsDict.ContainsKey(option.Name) )
				{
					return args.ParamsDict[option.Name];
				}
				else if(args.ParamsDict.ContainsKey(option.Short_name.ToString()))
				{
					return args.ParamsDict[option.Short_name.ToString()];
				}else{
					return option.DefaultValue;
				}
			}
		}
		public Object GetOptionValue(Args args)
		{
			return GetValue(args, this);
		}
		public Boolean GetOptionValueAsFlag(Args args)
		{
			return (bool)GetValue(args, this);
		}
		public String GetOptionValueAsString(Args args)
		{
			return GetValue(args, this).ToString();
		}
	}
	
}
