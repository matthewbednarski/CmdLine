CmdLine
=======

command line arguments parser utilities

.NET 2.0 compatable


<pre>
public static void Main(string[] arguments)
{
  Option o1 = new Option('f', "file", "File parameter.", false );
  Option flag1 = new Option('v', "verbose", "Print verbose output." , true, "FALSE");

  CmdLine.Args args = new CmdLine.Args(arguments, new Option[]{o1, flag1});

  Option help = args.GetOption('?');
  if(help.ActualValue_AsFlag)
  {
    Console.WriteLine(args.Usage());
    return;
  }

}
</pre>
