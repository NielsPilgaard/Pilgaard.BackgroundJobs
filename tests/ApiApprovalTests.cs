using System.Diagnostics;
using System.Reflection;
using PublicApiGenerator;
using Shouldly;

/// <see href="https://github.com/JakeGinnivan/ApiApprover"/>
public class ApiApprovalTests
{
	[Fact]
	[Trait("Category", "API Approval")]
	public void public_api_should_not_change_unintentionally()
	{
		const string testFolderName = "tests";

		var executingAssembly = Assembly.GetExecutingAssembly();
		var dependencies = executingAssembly.GetReferencedAssemblies();
		var nameToFind = executingAssembly.GetName().Name!.Split('.')[1]; // 0 is always "Pilgaard"

		var assemblyToTest = dependencies
							 .Select(Assembly.Load)
							 .Single(assembly =>
										 assembly
											 .GetTypes()
											 .Any(type => type.Name == "ApiMarker") &&
										 assembly.GetName().Name!
												 .Contains(nameToFind, StringComparison.InvariantCultureIgnoreCase));

		var publicApi = assemblyToTest.GeneratePublicApi(new ApiGeneratorOptions
		{
			IncludeAssemblyAttributes = false,
			AllowNamespacePrefixes = ["Microsoft.Extensions.DependencyInjection"]
		});

		var location = executingAssembly.Location;
		var pathItems = location.Split(Path.DirectorySeparatorChar);
		var index = Array.IndexOf(pathItems, testFolderName);
		Debug.Assert(index > 0 && index < pathItems.Length - 1);

		// See: https://shouldly.readthedocs.io/en/latest/assertions/shouldMatchApproved.html
		// Note: If the AssemblyName.approved.txt file doesn't match the latest publicApi value,
		// this call will try to launch a diff tool to help you out but that can fail on
		// your machine if a diff tool isn't configured/setup.
		publicApi.ShouldMatchApproved(options =>
			options.SubFolder(pathItems[index + 1])
				.WithFilenameGenerator((_, _, fileType, fileExtension)
					=> $"{assemblyToTest.GetName().Name}.{fileType}.{fileExtension}"));
	}
}
