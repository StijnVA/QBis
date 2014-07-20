using System;
using Qbus.Communication;

namespace HalloQBus
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Qbus.Communication.Controller controller = new Qbus.Communication.Controller ();
			Console.WriteLine ("Controller initializeren ...");
			
			controller.Address = "192.168.1.102";
			controller.Login = "admin";
			controller.Password = "Pixy";

			ConnectionManager.Instance.CommandReceived += (object sender, CommandEventArgs e) => {
				Console.WriteLine ("Command Received: " + e.Command.ToString ());
			};

			ConnectionManager.Instance.ConnectionChanged += (ControllerCommunication cc) => {
				Console.WriteLine ("Connection Changed");

				foreach (ControllerCommunication coco in ConnectionManager.Instance.ActiveConnections) {
					TcpCommunication tc = (TcpCommunication)coco;
					Console.WriteLine ("Connected: " + coco.Controller.Connected);
					Console.WriteLine ("Hostname: " + coco.Controller.HostName);
					Console.WriteLine ("TcpPort: " + coco.Controller.TcpPort);
					Console.WriteLine ("TcpComm Status: " + tc.Status);
				}
				Console.WriteLine ("---");
			};

			Console.WriteLine ("Druk ENTER");
			Console.ReadLine ();

			Console.WriteLine ("Bezig met verbinding maken...");
			ConnectionManager.Instance.Connect (controller);

			Console.WriteLine ("Druk ENTER");
			Console.ReadLine ();

			Console.WriteLine ("Bezig met ophalen modules");
			ConnectionManager.Instance.Modules.GetModules += (System.Collections.Generic.List<Qbus.Communication.Protocol.Module> modules) => {
				Console.WriteLine ("GetModulesAsyncResult");
				foreach(var m in modules){
					Console.WriteLine("   " + m.Name);
				}

			};

			ConnectionManager.Instance.Modules.GetModulesAsync ();

			Console.WriteLine ("Druk ENTER");
			Console.ReadLine ();


			Console.WriteLine ("Bezig met sluiten...");

			ConnectionManager.Instance.DisconnectAll ();

			Console.WriteLine ("Gesloten.");
		}
	}
}
	
	

