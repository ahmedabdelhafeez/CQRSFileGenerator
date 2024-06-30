using CQRSGenerator.Shared;
using System.IO;

namespace CQRSGenerator.Models;
public class CreaterHandler(
    string Entity,
    EnvDTE.Project Project,
    ObjectToCreate[] ObjectsToCreate
    )
{
    public void Start()
    {
        var projectDir = Path.GetDirectoryName(Project.FullName);
        var entityFolder = Path.Combine(projectDir, Pluralizer.Pluralize(Entity));
        var resourcesPath = Path.Combine("Resources");
        Directory.CreateDirectory(entityFolder);
        foreach (var create in ObjectsToCreate)
        {
            create.Create(Project,Entity,entityFolder);
        }
    }
}
