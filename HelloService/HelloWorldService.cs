using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;


[ServiceContract]
public interface IHelloWorldService
{
	[OperationContract]
	[WebInvoke(Method="GET", UriTemplate = "{name}" , ResponseFormat = WebMessageFormat.Json  )]
	string HelloWorld ();

	[OperationContract]
	[WebInvoke(Method="POST"
	           , UriTemplate = "{name}" 
	           , BodyStyle = WebMessageBodyStyle.WrappedRequest
	           , ResponseFormat = WebMessageFormat.Json
	           , RequestFormat=WebMessageFormat.Json
	           )]
	object SayHello(string name, string status);


}

public class HelloWorldService : IHelloWorldService
{
	private string lastSubmittedName;

	public string HelloWorld(){
		Console.WriteLine ("Hello GET world.");
		return "Hello, " + lastSubmittedName ;
	}

	public object SayHello(string name, string status)
	{
		WebOperationContext.Current.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.Redirect;
		WebOperationContext.Current.OutgoingResponse.Headers.Add ("Location", name);

		Console.WriteLine ("Hello POST world.");

		Console.WriteLine ("name: " + name);
		Console.WriteLine ("status: " + status);

		this.lastSubmittedName = name;
		//this.lastSubmittedName = name;
		return new { msg  = "hello post", code = 0 };
	}

}


