using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

using System.Threading;
using System.Threading.Tasks;



namespace HelloService
{
	class MainClass
	{
		public static void RunTcpService(){

			//Set the base Adress
			Uri baseAddress = new Uri("tcp.net://localhost:8080/hello");

			// Create the ServiceHost.
			using (ServiceHost host = new ServiceHost(typeof(HelloWorldService)))
			{
				// Enable metadata publishing.
				ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
				smb.HttpGetEnabled = true;

							
				var myTcpBinding = new NetTcpBinding();
				host.AddServiceEndpoint (typeof(IHelloWorldService), myTcpBinding, baseAddress);


				//smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				host.Description.Behaviors.Add(smb);

				// Open the ServiceHost to start listening for messages. Since
				// no endpoints are explicitly configured, the runtime will create
				// one endpoint per base address for each service contract implemented
				// by the service.
				host.Open();

				Console.WriteLine("The service is ready at {0}", baseAddress);
				// Console.WriteLine("Press <Enter> to stop the service.");

				Console.ReadLine ();
				// Close the ServiceHost.
				Console.WriteLine ("Host is shutting down...");
				host.Close();
				Console.WriteLine ("Host is down.");

			}		
		}

		public static void RunHttpService(){

			//Set the base Adress
			Uri baseAddress = new Uri("http://localhost:8080/hello");

			// Create the ServiceHost.
			using (WebServiceHost host = new WebServiceHost(typeof(HelloWorldService), baseAddress))
			{
				// Enable metadata publishing.
				ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
				smb.HttpGetEnabled = true;

				//smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
				host.Description.Behaviors.Add(smb);

				// Open the ServiceHost to start listening for messages. Since
				// no endpoints are explicitly configured, the runtime will create
				// one endpoint per base address for each service contract implemented
				// by the service.
				host.Open();

				Console.WriteLine("The service is ready at {0}", baseAddress);
				// Console.WriteLine("Press <Enter> to stop the service.");

				Console.ReadLine ();
				// Close the ServiceHost.
				Console.WriteLine ("Host is shutting down...");
				host.Close();
				Console.WriteLine ("Host is down.");

			}		
		}

		public static void Main (string[] args)
		{
				//RunHttpService();
			RunTcpService ();
	
		}
	}
}
