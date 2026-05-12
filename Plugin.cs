using QuickLook.Common.Plugin;
using SharpMap.Data.Providers;
using SharpMap.Forms;
using SharpMap.Layers;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using static SharpMap.Forms.MapBox;

namespace QuickLook.Plugin.ShapefileViewer;

public class Plugin : IViewer
{
    public int Priority => 0;

    public void Init()
    {
    }

    public bool CanHandle(string path)
    {
        return !Directory.Exists(path) && path.ToLower().EndsWith(".shp");
    }

    public void Prepare(string path, ContextObject context)
    {
        context.SetPreferredSizeFit(new Size { Width = 1920, Height = 1440 }, 0.9d);
    }

    public void View(string path, ContextObject context)
    {
        context.Title = $"{Path.GetFileName(path)}";
        try
        {
            MapBox mapBox = new()
            {
                Dock = DockStyle.Fill,
                ActiveTool = Tools.Pan,
                PreviewMode = PreviewModes.Fast
            };
            WindowsFormsHost host = new()
            {
                Child = mapBox,
            };
            VectorLayer layer = new(Path.GetFileName(path))
            {
                DataSource = new ShapeFile(path, false, true)
            };
            mapBox.Map.Layers.Add(layer);
            mapBox.Map.ZoomToExtents();
            mapBox.Refresh();

            context.ViewerContent = host;
        }
        catch (Exception ex)
        {
            context.ViewerContent = new System.Windows.Controls.Label
            {
                Content = $"Can not open shapefile because of: {ex.Message}"
            };
        }
        finally
        {
            context.IsBusy = false;
        }
    }

    public void Cleanup()
    {
    }
}
