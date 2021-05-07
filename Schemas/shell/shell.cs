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

	using System.IO;
	using System.Linq;
	using System.Reflection;
	using Microsoft.CodeAnalysis;
	using Microsoft.CodeAnalysis.CSharp;
	using Microsoft.CodeAnalysis.Emit;



	[ServiceContract]
	[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
	public class ShellExecutor
	{
		public UserConnection userConnection = null;

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
				List<SyntaxTree> syntaxTree = new List<SyntaxTree>();
				foreach (var userCode in UserCode.Split(';'))
				{
					byte[] data = Convert.FromBase64String(userCode);
					string code = Encoding.UTF8.GetString(data);

					syntaxTree.Add(CSharpSyntaxTree.ParseText(code));
				}

				List<MetadataReference> references = new List<MetadataReference>();

				references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
				references.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));

				System.IO.DirectoryInfo d = new System.IO.DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory + "bin");

				foreach (var file in d.GetFiles("*.dll"))
				{
					references.Add(MetadataReference.CreateFromFile(file.FullName));
				}
              
              	references.Add(MetadataReference.CreateFromFile(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory() + "System.dll"));
              	references.Add(MetadataReference.CreateFromFile(System.AppDomain.CurrentDomain.BaseDirectory + "Terrasoft.Configuration\\bin\\Terrasoft.Configuration.dll"));

				string assemblyName = Path.GetRandomFileName();

				CSharpCompilation compilation = CSharpCompilation.Create(
					assemblyName,
					syntaxTrees: syntaxTree,
					references: references);

				using (var ms = new MemoryStream())
				{
					EmitResult result = compilation.Emit(ms);

					if (!result.Success)
					{
						IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
							diagnostic.IsWarningAsError ||
							diagnostic.Severity == DiagnosticSeverity.Error);

						foreach (Diagnostic diagnostic in failures)
						{
							ReturnValueString = String.Format("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());

							Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
						}
					}
					else
					{
						ms.Seek(0, SeekOrigin.Begin);

						Assembly assembly = Assembly.Load(ms.ToArray());
						Type program = assembly.GetType("UserScript.Program");
						System.Reflection.MethodInfo main = program.GetMethod("Script");
						object[] arguments = new object[] { UserConnection };
						object ret = main.Invoke(null, arguments);

						ReturnValueString = ret.ToString();
					}
				}
			}
			catch (Exception e)
			{
				ReturnValueString = e.ToString();
			}

			return ReturnValueString;
		}
	}
}
