using System.Reflection;
using System.Runtime.InteropServices;

namespace endpointgenerator;

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
				foreach (var controller in types.Where(x =>
					         x.Name.Contains("controller", StringComparison.InvariantCultureIgnoreCase))) {
					Console.WriteLine(controller.FullName);
					foreach (var method in controller.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)) {
						Console.WriteLine(method.Name);
					}
				}

			}
			
		}
	}
}