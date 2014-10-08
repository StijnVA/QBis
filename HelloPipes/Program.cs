using System;
using System.IO;
using System.Text;
using System.Threading;

namespace HelloPipes
{
	class MyPipeReader {
		public StreamReader streamReader { set; get; }

		public void OnStreamReadLine (){
			String line; // = streamReader.ReadLine();
			while (! streamReader.EndOfStream) {
				line = streamReader.ReadLine ();
				Console.WriteLine ("Read from stream: " + line);
			}
        }
	}

	class MyPipeWriter{
		public StreamWriter streamWriter{ set; get; }

		public void StartWriting (){
			String line; // = streamReader.ReadLine();
			var rnd = new Random ();

			//streamWriter.NewLine = Environment.NewLine;
			//streamWriter.AutoFlush = true;

			var i = 0;

			while (true) {
				line = "Line " + i++;
				Console.WriteLine ("Writing to stream: " + line);
				streamWriter.WriteLine (line);
				streamWriter.Flush ();
				Thread.Sleep (rnd.Next (500) + 1300);
			}
		}
	}

	class MainClass
	{
		public static void Main (string[] args)
		{
		// 	DoRead ();
		//	DoWrite ();
			DoReadAndWrite ();

		}

		public static void DoReadAndWrite(){

			Console.WriteLine ("Begin");
			var myPipeInName = "/home/stijn/sandbox/myPipeIn";
			var myPipeOutName = "/home/stijn/sandbox/myPipeOut";


			var pipeStreamIn= new FileStream (myPipeInName, FileMode.Open);
			var pipeStreamOut= new FileStream (myPipeOutName, FileMode.Open);

			//var streamWriterIn = new StreamWriter (pipeStreamIn);
			var streamReaderIn = new StreamReader (pipeStreamIn);

			var streamWriterOut = new StreamWriter (pipeStreamOut);



			var myPipeReader = new MyPipeReader {
				streamReader = streamReaderIn
			};
			var myPipeWriter = new MyPipeWriter {
				streamWriter = streamWriterOut
			};

			new Thread (new ThreadStart(myPipeReader.OnStreamReadLine)).Start ();			         
			new Thread (new ThreadStart(myPipeWriter.StartWriting)).Start ();			         

			Console.ReadLine ();

		}



		

		public static void DoWrite(){
			Console.WriteLine("Begin");
			var myPipeName = "/home/stijn/sandbox/myPipe";

			var pipeStream = File.OpenWrite (myPipeName);

			Console.WriteLine ("Pipe opened.");

			var streamWriter= new StreamWriter (pipeStream);

			Console.WriteLine ("Stream opened.");

			Console.WriteLine ("Reading from stream...");

			for(int i = 0; i < 10; i++){

				Console.WriteLine ("Hello pipe " + i);
				streamWriter.WriteLine ("Hello pipe " + i);

				streamWriter.Flush ();
				Thread.Sleep(1000);
			}
			streamWriter.Close ();

			Console.WriteLine ("Streamwriter closed.");

			Console.WriteLine ("Consider it done");

		}

		 public static void DoRead(){
			Console.WriteLine("Begin");
			var myPipeName = "/home/stijn/sandbox/myPipe";

			var pipeStream = new FileInfo (myPipeName).OpenRead();

			Console.WriteLine ("Pipe opened.");

			var streamReader= new StreamReader (pipeStream);

			Console.WriteLine ("Stream opened.");

			Console.WriteLine ("Reading from stream...");

			String line; // = streamReader.ReadLine();
			while(! streamReader.EndOfStream){
				line = streamReader.ReadLine();
				Console.WriteLine ("Read from stream: "+ line);
			}

			streamReader.Close ();

			Console.WriteLine ("Streamwriter closed.");

			Console.WriteLine ("Consider it done");

		}
	}
}
