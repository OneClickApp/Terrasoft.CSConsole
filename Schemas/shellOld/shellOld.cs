namespace Terrasoft.Configuration
{
	using System;
	using System.Text;
	using System.Web;
	using System.ServiceModel;
	using System.ServiceModel.Web;
	using System.ServiceModel.Activation;
	using Terrasoft.Core;
	using Terrasoft.Core.DB;
	using System.Collections.Generic;

	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ShellExecutorOld
	{
		private UserConnection userConnection = null;

		public UserConnection UserConnection
		{
			get
			{
				if (this.userConnection == null)
				{
					this.userConnection = (UserConnection)HttpContext.Current.Session[@"UserConnection"];
				}

				return this.userConnection;
			}
		}

		[OperationContract]
		[WebInvoke(Method = @"POST", RequestFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
		public string Execute(string UserCode)
		{
			string ReturnValueString = "";
			try
			{
				var codeArr = new List<string>();
				foreach (var userCode in UserCode.Split(';'))
				{
					byte[] data = Convert.FromBase64String(userCode);
					string code = Encoding.UTF8.GetString(data);
					codeArr.Add(code);
				}

				Microsoft.CSharp.CSharpCodeProvider provider = new Microsoft.CSharp.CSharpCodeProvider();
				System.CodeDom.Compiler.CompilerParameters parameters = new System.CodeDom.Compiler.CompilerParameters();

				parameters.ReferencedAssemblies.Add("System.dll");
				parameters.ReferencedAssemblies.Add("System.Xml.dll");
				parameters.ReferencedAssemblies.Add("System.Core.dll");
				parameters.ReferencedAssemblies.Add("System.Data.dll");
				parameters.ReferencedAssemblies.Add("System.Drawing.dll");
				parameters.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
				parameters.ReferencedAssemblies.Add("System.Xml.Linq.dll");
				parameters.ReferencedAssemblies.Add("System.Management.dll");
				parameters.ReferencedAssemblies.Add("System.Configuration.dll");
				parameters.ReferencedAssemblies.Add("System.ServiceModel.Web.dll");
				parameters.ReferencedAssemblies.Add("System.ServiceModel.Activation.dll");
				parameters.ReferencedAssemblies.Add("System.ServiceModel.dll");
				parameters.ReferencedAssemblies.Add("System.Runtime.Serialization.dll");
				parameters.ReferencedAssemblies.Add("System.Web.dll");
				parameters.ReferencedAssemblies.Add(System.AppDomain.CurrentDomain.BaseDirectory + "Terrasoft.Configuration\\bin\\Terrasoft.Configuration.dll");

				System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "bin");

				foreach (var file in d.GetFiles("*.dll"))
				{
					parameters.ReferencedAssemblies.Add(file.FullName);
				}

				parameters.GenerateInMemory = true;
				parameters.GenerateExecutable = true;

				System.CodeDom.Compiler.CompilerResults results = provider.CompileAssemblyFromSource(parameters, codeArr.ToArray());


				if (results.Errors.HasErrors)
				{
					System.Text.StringBuilder sb = new System.Text.StringBuilder();

					foreach (System.CodeDom.Compiler.CompilerError error in results.Errors)
					{
						sb.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
					}

					return sb.ToString();
				}

				System.Reflection.Assembly assembly = results.CompiledAssembly;
				Type program = assembly.GetType("UserScript.Program");
				System.Reflection.MethodInfo main = program.GetMethod("Script");
				object[] arguments = new object[] { UserConnection };
				object ret = main.Invoke(null, arguments);

				ReturnValueString = ret.ToString();
			}
			catch (Exception e)
			{
				ReturnValueString = e.ToString();
			}

			return ReturnValueString;
		}
	}
}
