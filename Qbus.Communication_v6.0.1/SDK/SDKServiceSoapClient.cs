// Decompiled with JetBrains decompiler
// Type: Qbus.Communication.SDK.SDKServiceSoapClient
// Assembly: Qbus.Communication, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 1C9BB5DA-FD58-4B4B-BB43-F21C0096E93E
// Assembly location: C:\Users\Stijn\Downloads\Qbus.Communication_v6.0.1.dll

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading;

namespace Qbus.Communication.SDK
{
  [GeneratedCode("System.ServiceModel", "4.0.0.0")]
  [DebuggerStepThrough]
  public class SDKServiceSoapClient : ClientBase<SDKServiceSoap>, SDKServiceSoap
  {
    private ClientBase<SDKServiceSoap>.BeginOperationDelegate onBeginGetActivationStringDelegate;
    private ClientBase<SDKServiceSoap>.EndOperationDelegate onEndGetActivationStringDelegate;
    private SendOrPostCallback onGetActivationStringCompletedDelegate;
    private ClientBase<SDKServiceSoap>.BeginOperationDelegate onBeginGetDeactivationStringDelegate;
    private ClientBase<SDKServiceSoap>.EndOperationDelegate onEndGetDeactivationStringDelegate;
    private SendOrPostCallback onGetDeactivationStringCompletedDelegate;

    public event EventHandler<GetActivationStringCompletedEventArgs> GetActivationStringCompleted;

    public event EventHandler<GetDeactivationStringCompletedEventArgs> GetDeactivationStringCompleted;

    public SDKServiceSoapClient()
    {
    }

    public SDKServiceSoapClient(string endpointConfigurationName)
      : base(endpointConfigurationName)
    {
    }

    public SDKServiceSoapClient(string endpointConfigurationName, string remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public SDKServiceSoapClient(string endpointConfigurationName, EndpointAddress remoteAddress)
      : base(endpointConfigurationName, remoteAddress)
    {
    }

    public SDKServiceSoapClient(Binding binding, EndpointAddress remoteAddress)
      : base(binding, remoteAddress)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    GetActivationStringResponse SDKServiceSoap.GetActivationString(GetActivationStringRequest request)
    {
      return this.Channel.GetActivationString(request);
    }

    public string GetActivationString(string CTL, string SN, string name, string email, string installer)
    {
      GetActivationStringRequest request = new GetActivationStringRequest();
      request.Body = new GetActivationStringRequestBody();
      request.Body.CTL = CTL;
      request.Body.SN = SN;
      request.Body.name = name;
      request.Body.email = email;
      request.Body.installer = installer;
      return ((SDKServiceSoap) this).GetActivationString(request).Body.GetActivationStringResult;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    IAsyncResult SDKServiceSoap.BeginGetActivationString(GetActivationStringRequest request, AsyncCallback callback, object asyncState)
    {
      return this.Channel.BeginGetActivationString(request, callback, asyncState);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IAsyncResult BeginGetActivationString(string CTL, string SN, string name, string email, string installer, AsyncCallback callback, object asyncState)
    {
      GetActivationStringRequest request = new GetActivationStringRequest();
      request.Body = new GetActivationStringRequestBody();
      request.Body.CTL = CTL;
      request.Body.SN = SN;
      request.Body.name = name;
      request.Body.email = email;
      request.Body.installer = installer;
      return ((SDKServiceSoap) this).BeginGetActivationString(request, callback, asyncState);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    GetActivationStringResponse SDKServiceSoap.EndGetActivationString(IAsyncResult result)
    {
      return this.Channel.EndGetActivationString(result);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public string EndGetActivationString(IAsyncResult result)
    {
      return ((SDKServiceSoap) this).EndGetActivationString(result).Body.GetActivationStringResult;
    }

    private IAsyncResult OnBeginGetActivationString(object[] inValues, AsyncCallback callback, object asyncState)
    {
      return this.BeginGetActivationString((string) inValues[0], (string) inValues[1], (string) inValues[2], (string) inValues[3], (string) inValues[4], callback, asyncState);
    }

    private object[] OnEndGetActivationString(IAsyncResult result)
    {
      return new object[1]
      {
        (object) this.EndGetActivationString(result)
      };
    }

    private void OnGetActivationStringCompleted(object state)
    {
      if (this.GetActivationStringCompleted == null)
        return;
      ClientBase<SDKServiceSoap>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<SDKServiceSoap>.InvokeAsyncCompletedEventArgs) state;
      this.GetActivationStringCompleted((object) this, new GetActivationStringCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public void GetActivationStringAsync(string CTL, string SN, string name, string email, string installer)
    {
      this.GetActivationStringAsync(CTL, SN, name, email, installer, (object) null);
    }

    public void GetActivationStringAsync(string CTL, string SN, string name, string email, string installer, object userState)
    {
      if (this.onBeginGetActivationStringDelegate == null)
        this.onBeginGetActivationStringDelegate = new ClientBase<SDKServiceSoap>.BeginOperationDelegate(this.OnBeginGetActivationString);
      if (this.onEndGetActivationStringDelegate == null)
        this.onEndGetActivationStringDelegate = new ClientBase<SDKServiceSoap>.EndOperationDelegate(this.OnEndGetActivationString);
      if (this.onGetActivationStringCompletedDelegate == null)
        this.onGetActivationStringCompletedDelegate = new SendOrPostCallback(this.OnGetActivationStringCompleted);
      this.InvokeAsync(this.onBeginGetActivationStringDelegate, new object[5]
      {
        (object) CTL,
        (object) SN,
        (object) name,
        (object) email,
        (object) installer
      }, this.onEndGetActivationStringDelegate, this.onGetActivationStringCompletedDelegate, userState);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    GetDeactivationStringResponse SDKServiceSoap.GetDeactivationString(GetDeactivationStringRequest request)
    {
      return this.Channel.GetDeactivationString(request);
    }

    public string GetDeactivationString(string CTL)
    {
      GetDeactivationStringRequest request = new GetDeactivationStringRequest();
      request.Body = new GetDeactivationStringRequestBody();
      request.Body.CTL = CTL;
      return ((SDKServiceSoap) this).GetDeactivationString(request).Body.GetDeactivationStringResult;
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    IAsyncResult SDKServiceSoap.BeginGetDeactivationString(GetDeactivationStringRequest request, AsyncCallback callback, object asyncState)
    {
      return this.Channel.BeginGetDeactivationString(request, callback, asyncState);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public IAsyncResult BeginGetDeactivationString(string CTL, AsyncCallback callback, object asyncState)
    {
      GetDeactivationStringRequest request = new GetDeactivationStringRequest();
      request.Body = new GetDeactivationStringRequestBody();
      request.Body.CTL = CTL;
      return ((SDKServiceSoap) this).BeginGetDeactivationString(request, callback, asyncState);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    GetDeactivationStringResponse SDKServiceSoap.EndGetDeactivationString(IAsyncResult result)
    {
      return this.Channel.EndGetDeactivationString(result);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public string EndGetDeactivationString(IAsyncResult result)
    {
      return ((SDKServiceSoap) this).EndGetDeactivationString(result).Body.GetDeactivationStringResult;
    }

    private IAsyncResult OnBeginGetDeactivationString(object[] inValues, AsyncCallback callback, object asyncState)
    {
      return this.BeginGetDeactivationString((string) inValues[0], callback, asyncState);
    }

    private object[] OnEndGetDeactivationString(IAsyncResult result)
    {
      return new object[1]
      {
        (object) this.EndGetDeactivationString(result)
      };
    }

    private void OnGetDeactivationStringCompleted(object state)
    {
      if (this.GetDeactivationStringCompleted == null)
        return;
      ClientBase<SDKServiceSoap>.InvokeAsyncCompletedEventArgs completedEventArgs = (ClientBase<SDKServiceSoap>.InvokeAsyncCompletedEventArgs) state;
      this.GetDeactivationStringCompleted((object) this, new GetDeactivationStringCompletedEventArgs(completedEventArgs.Results, completedEventArgs.Error, completedEventArgs.Cancelled, completedEventArgs.UserState));
    }

    public void GetDeactivationStringAsync(string CTL)
    {
      this.GetDeactivationStringAsync(CTL, (object) null);
    }

    public void GetDeactivationStringAsync(string CTL, object userState)
    {
      if (this.onBeginGetDeactivationStringDelegate == null)
        this.onBeginGetDeactivationStringDelegate = new ClientBase<SDKServiceSoap>.BeginOperationDelegate(this.OnBeginGetDeactivationString);
      if (this.onEndGetDeactivationStringDelegate == null)
        this.onEndGetDeactivationStringDelegate = new ClientBase<SDKServiceSoap>.EndOperationDelegate(this.OnEndGetDeactivationString);
      if (this.onGetDeactivationStringCompletedDelegate == null)
        this.onGetDeactivationStringCompletedDelegate = new SendOrPostCallback(this.OnGetDeactivationStringCompleted);
      this.InvokeAsync(this.onBeginGetDeactivationStringDelegate, new object[1]
      {
        (object) CTL
      }, this.onEndGetDeactivationStringDelegate, this.onGetDeactivationStringCompletedDelegate, userState);
    }
  }
}
