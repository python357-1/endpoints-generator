using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace endpointgenerator;

public class ControllerData {
	private Type Controller { get; init; }
	public string Name => Controller.FullName;

	private IEnumerable<MethodInfo> Methods =>
		Controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
	public IEnumerable<string> MethodNames { get; set; }

	public ControllerData(Type controller) {
		Controller = controller;
	}
}

internal class Program {
	static void Main(string[] args) {
		if (args.Length == 0) {
			Console.WriteLine("no");
			return;
		}

		if (args.Length == 1) {
			var runtimeAssemblies = Directory.GetFiles(RuntimeEnvironment.GetRuntimeDirectory(), "*.dll");
			var paths = new List<string>(runtimeAssemblies);
			paths.Add(Path.Join(Directory.GetCurrentDirectory(), args[0]));
			paths.Add("Microsoft.AspNetCore.Mvc.ViewFeatures");
			paths.Add("Microsoft.AspNetCore");
			paths.Add("Microsoft.AspNetCore.Mvc");
			var resolver = new PathAssemblyResolver(paths);

			var mlc = new MetadataLoadContext(resolver);

			using (mlc) {
				var assm = mlc.LoadFromAssemblyPath(Path.GetFileName(args[0]));
				var types = assm.GetTypes();
				var controllerDatas = Enumerable.Empty<ControllerData>();
				foreach (var controller in types.Where(x =>
					         x.Name.Contains("controller", StringComparison.InvariantCultureIgnoreCase))) {
					controllerDatas = controllerDatas.Append(new ControllerData(controller));
				}

				Console.WriteLine(JsonSerializer.Serialize(controllerDatas));
			}
			
		}
	}
}