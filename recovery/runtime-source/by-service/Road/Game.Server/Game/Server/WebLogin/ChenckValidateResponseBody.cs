using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Game.Server.WebLogin;

[DebuggerStepThrough]
[DataContract(Namespace = "dandantang")]
[GeneratedCode("System.ServiceModel", "4.0.0.0")]
[EditorBrowsable(EditorBrowsableState.Advanced)]
public class ChenckValidateResponseBody
{
	[DataMember(EmitDefaultValue = false, Order = 0)]
	public string ChenckValidateResult;

	public ChenckValidateResponseBody()
	{
	}

	public ChenckValidateResponseBody(string ChenckValidateResult)
	{
		this.ChenckValidateResult = ChenckValidateResult;
	}
}
