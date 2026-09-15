using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel;

namespace Game.Server.WebLogin;

[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[DebuggerStepThrough]
[MessageContract(IsWrapped = false)]
[EditorBrowsable(EditorBrowsableState.Advanced)]
public class ChenckValidateResponse
{
	[MessageBodyMember(Name = "ChenckValidateResponse", Namespace = "dandantang", Order = 0)]
	public ChenckValidateResponseBody Body;

	public ChenckValidateResponse()
	{
	}

	public ChenckValidateResponse(ChenckValidateResponseBody Body)
	{
		this.Body = Body;
	}
}
