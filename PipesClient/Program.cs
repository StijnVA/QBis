using System;
using System.IO;

namespace PipesClient
{
	class MainClass
	{
		private const string myPipeInName = "/home/stijn/sandbox/myPipeIn";
		private const string myPipeOutName = "/home/stijn/sandbox/myPipeOut";

		public static void Main (string[] args)
		{
			var command = args [0];
			switch (command.ToLower()) {
			case "log":
				log ();
				break;
			case "send":
				send ();
				break;
			default:
				Console.Error.WriteLine(String.Format("The command '{0}' is not vallid", command));
				break;
			}

		}
		private static void log(){
			//The name's In and Out are viewd from the server application side.
			//In our app we need to write to In, and read from Out.
			var pipeStreamOut= new FileStream (myPipeOutName, FileMode.Open);
			var streamReaderOut = new StreamReader (pipeStreamOut);

			while (!streamReaderOut.EndOfStream) {
				Console.WriteLine(streamReaderOut.ReadLine());
			}

		}

		/*
		  private static void send(){
			//The name's In and Out are viewd from the server application side.
			//In our app we need to write to In, and read from Out.
			var pipeStreamIn= new FileStream (myPipeInName, FileMode.Open);
			var streamWriterIn = new StreamWriter (pipeStreamIn);

			//streamWriterIn.Write(new StreamReader(Console.OpenStandardInput ()).ReadToEnd());
			var stInStream = Console.OpenStandardInput ();
			var stInReader = new StreamReader (stInStream);
			while (!stInReader.EndOfStream) {
				var line = stInReader.ReadLine ();
				Console.WriteLine("Sending: "+ line); 
				streamWriterIn.WriteLine (line);
			}

			streamWriterIn.Flush ();
			streamWriterIn.Close ();
	

		}
*/

		private static void send(){
			//The name's In and Out are viewd from the server application side.
			//In our app we need to write to In, and read from Out.


			//streamWriterIn.Write(new StreamReader(Console.OpenStandardInput ()).ReadToEnd());
			var stInStream = Console.OpenStandardInput ();
			var stInReader = new StreamReader (stInStream);
			while (!stInReader.EndOfStream) {
				var pipeStreamIn= new FileStream (myPipeInName, FileMode.Open);
				var streamWriterIn = new StreamWriter (pipeStreamIn);

				var line = stInReader.ReadLine ();
				Console.WriteLine("Sending: "+ line); 
				streamWriterIn.WriteLine (line);
			

				streamWriterIn.Flush ();
				streamWriterIn.Close ();
			}
	

		}
	}
}
