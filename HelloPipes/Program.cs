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

		public void StartWriting (){
			String line; // = streamReader.ReadLine();
			var rnd = new Random ();

			var myPipeOutName = "/home/stijn/sandbox/myPipeOut";
			var myDummyPipeStream = new FileInfo (myPipeOutName).Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
			var dummyWriter = new StreamWriter (myDummyPipeStream);

			//streamWriter.NewLine = Environment.NewLine;
			//streamWriter.AutoFlush = true;

			Console.WriteLine("Start writing...");


			var i = 0;

			while (true) {


				var myPipeStream = new FileInfo (myPipeOutName).Open(FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

				//var pipeOutStreamDummy = new FileStream (myPipeOutName, FileMode.Create);
				//We do not need to read the pipe, but there must be at least one reader to write.
				//Console.WriteLine ("Open Dummy");




			
				//var pipeOutStream = new FileStream (myPipeOutName, FileMode.Open);

				//Console.WriteLine ("Creating StreamWriter");
				using (var streamPipeWriter = new StreamWriter (myPipeStream)) {
					line = "Line " + i++;
					Console.WriteLine ("Writing to stream: " + line);
					streamPipeWriter.WriteLine (line);

				}	
				Thread.Sleep (rnd.Next (500) + 1000);
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



			var pipeStreamIn= new FileStream (myPipeInName, FileMode.Open);
			//var pipeStreamOut= new FileStream (myPipeOutName, FileMode.Open);

			//var streamWriterIn = new StreamWriter (pipeStreamIn);
			var streamReaderIn = new StreamReader (pipeStreamIn);





			var myPipeReader = new MyPipeReader {
				streamReader = streamReaderIn
			};
			var myPipeWriter = new MyPipeWriter ();

			new Thread (new ThreadStart(myPipeReader.OnStreamReadLine)).Start ();			         
			new Thread (new ThreadStart(myPipeWriter.StartWriting)).Start ();			         

			Console.ReadLine ();

		}



		

		public static void DoWrite(){
			Console.WriteLine("Begin");
			var myPipeName = "/home/stijn/sandbox/myPipeOut";

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
			var myPipeName = "/home/stijn/sandbox/myPipeIn";

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
