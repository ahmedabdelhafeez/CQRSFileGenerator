using CQRSGenerator.Enums;
using CQRSGenerator.Models;
using CQRSGenerator.Shared;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.OLE.Interop;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Windows;
using System.Windows.Controls;
using MessageBox = System.Windows.MessageBox;

namespace CQRSGenerator;

public partial class CQRSWindowControl : UserControl
{
    public List<EnvDTE.Project> Projects;
    public CQRSWindowControl()
    {
        InitializeComponent();
        Projects = new List<EnvDTE.Project>();
        Loaded += CQRSWindowControl_Loaded;

    }
    private async void CQRSWindowControl_Loaded(object sender, RoutedEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
        await PopulateProjectComboBoxAsync();
    }
    public async Task PopulateProjectComboBoxAsync()
    {
        var dte = (DTE2)await ServiceProvider.GetGlobalServiceAsync(typeof(DTE));
        var solution = dte.Solution;
        Projects = GetAllProjects(solution.Projects);

        foreach (EnvDTE.Project project in Projects)
        {
            ProjectComboBox.Items.Add(project.Name);
        }
    }
    private List<EnvDTE.Project> GetAllProjects(Projects projects)
    {
        var projectList = new List<EnvDTE.Project>();

        foreach (EnvDTE.Project project in projects)
        {
            if (project.Kind == ProjectKinds.vsProjectKindSolutionFolder)
            {
                projectList.AddRange(GetSolutionFolderProjects(project));
            }
            else
            {
                projectList.Add(project);
            }
        }

        return projectList;
    }

    private List<EnvDTE.Project> GetSolutionFolderProjects(EnvDTE.Project solutionFolder)
    {
        var projectList = new List<EnvDTE.Project>();

        var solutionFolderProjects = solutionFolder.ProjectItems;
        if (solutionFolderProjects == null) return projectList;

        foreach (ProjectItem projectItem in solutionFolderProjects)
        {
            var subProject = projectItem.SubProject;
            if (subProject != null)
            {
                if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                {
                    projectList.AddRange(GetSolutionFolderProjects(subProject));
                }
                else
                {
                    projectList.Add(subProject);
                }
            }
        }

        return projectList;
    }
    private async void GenerateBtnClick(object sender, RoutedEventArgs e)
    {
        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

        var selectedProjectName = ProjectComboBox.SelectedValue as string;
        var entityName = EntityBox.Text;

        if (string.IsNullOrEmpty(selectedProjectName) ||
            string.IsNullOrEmpty(entityName))
        {
            MessageBox.Show("Please select a project and enter an entity name.");
            return;
        }

        var selectedProject = Projects.Cast<EnvDTE.Project>()
            .FirstOrDefault(p => p.Name == selectedProjectName);
        if (selectedProject == null)
        {
            MessageBox.Show("Selected project not found.");
            return;
        }
        var pluralizerName = Pluralizer.Pluralize(entityName);

        var createList = new[]
        {
            new ObjectToCreate
            {
                Name = "Commands",
                Type = CreateType.Folder,
                Childs = new List<ObjectToCreate>
                {
                    new ObjectToCreate
                    {
                        Name = "Create" + entityName,
                        Type = CreateType.Folder,
                        Childs = new List<ObjectToCreate>
                        {
                            new ObjectToCreate
                            {
                                Name = $"Create{entityName}Command", 
                                Type = CreateType.File,
                                FileType=FileTypes.Command,

                            }
                        }
                    },
                    new ObjectToCreate
                    {
                        Name = "Update" + entityName,
                        Type = CreateType.Folder,
                        Childs = new List<ObjectToCreate>
                        {
                            new ObjectToCreate 
                            { 
                                Name = $"Update{entityName}Command",
                                Type = CreateType.File,
                                FileType=FileTypes.Command,
                            },
                        }
                    },
                    new ObjectToCreate
                    {
                        Name = "Delete" + entityName,
                        Type = CreateType.Folder,
                        Childs = new List<ObjectToCreate>
                        {
                            new ObjectToCreate 
                            { 
                                Name = $"Delete{entityName}Command",
                                Type = CreateType.File,
                                FileType=FileTypes.Command,
                            }
                        }
                    }
                }
            },
            new ObjectToCreate
            {
                Name = "Queries",
                Type = CreateType.Folder,
                Childs = new List<ObjectToCreate>
                {
                    new ObjectToCreate
                    {
                        Name = $"Get{entityName}ById",
                        Type = CreateType.Folder,
                        Childs = new List<ObjectToCreate>
                        {
                            new ObjectToCreate 
                            { 
                                Name = $"Get{entityName}ByIdQuery",
                                Type = CreateType.File,
                                FileType=FileTypes.Query,
                            }
                        }
                    },
                    new ObjectToCreate
                    {
                        Name = $"GetAll{pluralizerName}",
                        Type = CreateType.Folder,
                        Childs = new List<ObjectToCreate>
                        {
                            new ObjectToCreate 
                            {
                                Name = $"GetAll{pluralizerName}Query",
                                Type = CreateType.File,
                                FileType=FileTypes.Query,
                            }
                        }
                    }
                }
            },
            new ObjectToCreate
            {
                Name = "Responses",
                Type = CreateType.Folder,
                Childs = new List<ObjectToCreate>
                {
                    new ObjectToCreate 
                    { 
                        Name = entityName+"Response", 
                        Type = CreateType.File,
                        FileType=FileTypes.Response,
                    }
                }
            },
            new ObjectToCreate
            {
                Name = "Mappig",
                Type = CreateType.Folder,
                Childs = new List<ObjectToCreate>
                {
                    new ObjectToCreate 
                    { 
                        Name = entityName+"Mapping",
                        Type = CreateType.File,
                        FileType=FileTypes.Mapper,
                    }
                }
            }
        };

        new CreaterHandler(
        entityName,
        selectedProject,
        createList
        ).Start();
        CoseWindow();
        MessageBox.Show($"created successfully");
    }

    private void CancellBtnClick(object sender, RoutedEventArgs e) {
        CoseWindow();
    }
    private void CoseWindow()
    {
        EntityBox.Text = "";
        System.Windows.Window.GetWindow(this)?.Close();
    }

}
