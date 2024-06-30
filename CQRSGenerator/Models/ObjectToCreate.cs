using CQRSGenerator.Enums;
using CQRSGenerator.Shared;
using System.Collections.Generic;
using System.IO;

namespace CQRSGenerator.Models;

public class ObjectToCreate
{
    public CreateType Type { get; set; }
    public FileTypes? FileType { get; set; }
    public string Name { get; set; }
    public bool HasChilds => Childs != null && Childs.Count > 0;
    public List<ObjectToCreate> Childs { get; set; }
    public void Create(EnvDTE.Project project, string entity, string path)
    {
        string fullPath =path;
        switch (Type)
        {
            case CreateType.Folder:
                fullPath = Path.Combine(path, Name);
                CreateFolder(fullPath);
                break;
            case CreateType.File:
                CreateFile(project,entity, fullPath,"./Resources");
                break;
        }
        if (HasChilds)
        {
            foreach (var child in Childs)
            {
                child.Create(project,entity, fullPath);
            }
        }
    }

    private void CreateFolder(string path)
    {
        Directory.CreateDirectory(path);
    }

    private void CreateFile(EnvDTE.Project project,string entity, string path,string resourcesPath)
    {
        var namespaceName = path.Substring(path.IndexOf(project.Name));
        var responseNamespace = Path.Combine(project.Name, Pluralizer.Pluralize(entity));
        string templatePath = "";
        string templateHandlerPath = "";
        switch (FileType)
        {
            case FileTypes.Command:
                templatePath = Path.Combine(resourcesPath, "Command.txt");
                templateHandlerPath = Path.Combine(resourcesPath, "CommandHandler.txt");
                break;
            //case FileTypes.CommandHandler:
            //    templatePath = Path.Combine(resourcesPath, "CommandHandler.txt");
            //    break;
            case FileTypes.Query:
                templatePath = Path.Combine(resourcesPath, "Query.txt");
                templateHandlerPath = Path.Combine(resourcesPath, "QueryHandler.txt");
                break;
            //case FileTypes.QueryHandler:
            //    templatePath = Path.Combine(resourcesPath, "QueryHandler.txt");
            //    break;
            case FileTypes.Mapper:
                templatePath = Path.Combine(resourcesPath, "Mapper.txt");
                break;
            case FileTypes.Response:
                templatePath = Path.Combine(resourcesPath, "Response.txt");
                break;
            default:
                throw new ArgumentNullException(nameof(Name));
        }

        var filePath = Path.Combine(path, $"{Name}.cs");
        var templateContent = File.ReadAllText(templatePath);
        var  content = templateContent
             .Replace("{responseNamespace}", responseNamespace.Replace('\\', '.'))
             .Replace("{namespace}", namespaceName.Replace('\\', '.'))
             .Replace("{entity}", entity)
             .Replace("{name}", Name);
        File.WriteAllText(filePath, content);
        project.ProjectItems.AddFromFile(filePath);
        if (!string.IsNullOrEmpty(templateHandlerPath))
        {
            var fileHandlerPath = Path.Combine(path, $"{Name}Handler.cs");
            var handlerContent = File.ReadAllText(templateHandlerPath);
            var hContent = handlerContent
                 .Replace("{responseNamespace}", responseNamespace.Replace('\\', '.'))
                 .Replace("{namespace}", namespaceName.Replace('\\', '.'))
                 .Replace("{entity}", entity)
                 .Replace("{name}", Name);
            File.WriteAllText(fileHandlerPath, hContent);
            project.ProjectItems.AddFromFile(fileHandlerPath);

        }
    }
}
