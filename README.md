CmdLine
=======

command line arguments parser utilities

.NET 2.0 compatable


<pre>
public static void Main(string[] arguments)
{
  CmdLine.Option o1 = new CmdLine.Option('f', "file", "File parameter.", false );
  CmdLine.Option flag1 = new CmdLine.Option('v', "verbose", "Print verbose output." , true, "FALSE");

  CmdLine.Args args = new CmdLine.Args(arguments, new CmdLine.Option[]{o1, flag1});

  CmdLine.Option help = args.GetOption('?');
  if(help.ActualValue_AsFlag)
  {
    Console.WriteLine(args.Usage());
    return;
  }

}
</pre>
