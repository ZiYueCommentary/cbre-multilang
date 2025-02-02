﻿using CBRE.Common.Mediator;
using CBRE.Editor.Rendering;
using CBRE.Editor.UI.Layout;
using CBRE.Graphics.Helpers;
using CBRE.Graphics.Renderables;
using CBRE.Localization;
using CBRE.Settings;
using CBRE.UI;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CBRE.Editor.UI
{
    public static class ViewportManager
    {
        private static TableSplitControl MainWindowGrid { get; set; }
        public static List<ViewportBase> Viewports { get; private set; }
        public static List<ViewportWindow> Windows { get; private set; }

        static ViewportManager()
        {
            Viewports = new List<ViewportBase>();
            Windows = new List<ViewportWindow>();
        }

        private static ViewportBase CreateViewport(string setting, Viewport3D.ViewType preferred3D)
        {
            return CreateViewport(setting, true, preferred3D, Viewport2D.ViewDirection.Top);
        }

        private static ViewportBase CreateViewport(string setting, Viewport2D.ViewDirection preferred2D)
        {
            return CreateViewport(setting, false, Viewport3D.ViewType.Lightmapped, preferred2D);
        }

        private static ViewportBase CreateViewport(string setting, bool prefer3D, Viewport3D.ViewType preferred3D, Viewport2D.ViewDirection preferred2D)
        {
            if (setting != null)
            {
                string[] spl = setting.ToLowerInvariant().Split('.');
                if (spl.Length == 2)
                {
                    switch (spl[0])
                    {
                        case "viewport3d":
                            prefer3D = true;
                            Viewport3D.ViewType vt;
                            if (Enum.TryParse(spl[1], true, out vt)) preferred3D = vt;
                            break;
                        case "viewport2d":
                            prefer3D = false;
                            Viewport2D.ViewDirection vd;
                            if (Enum.TryParse(spl[1], true, out vd)) preferred2D = vd;
                            break;
                    }
                }
            }

            if (prefer3D) return Create3D(preferred3D);
            else return Create2D(preferred2D);
        }

        public static void Init(TableSplitControl tlp)
        {
            MainWindowGrid = tlp;

            List<ViewportWindowConfiguration> configuration = SettingsManager.GetAdditionalData<List<ViewportWindowConfiguration>>("ViewportManagerWindowConfiguration")
                                ?? new List<ViewportWindowConfiguration>();
            ViewportWindowConfiguration main = configuration.FirstOrDefault(x => x.WindowID == 0) ?? GetDefaultWindowConfiguration();
            MainWindowGrid.Configuration = main.Configuration;
            LoadViewports(MainWindowGrid, main);
            if (!main.Size.IsEmpty)
            {
                Editor.Instance.Location = main.Size.Location;
                Editor.Instance.Size = main.Size.Size;
                Editor.Instance.WindowState = main.Maximised ? FormWindowState.Maximized : FormWindowState.Normal;
            }

            foreach (ViewportWindowConfiguration config in configuration.Where(x => x.WindowID > 0))
            {
                CreateViewportWindow(config);
            }
        }

        public static void CreateNewWindow()
        {
            CreateViewportWindow(new ViewportWindowConfiguration
            {
                Size = Rectangle.Empty,
                WindowID = Windows.Count + 1,
                Configuration = TableSplitConfiguration.Default()
            });
        }

        private static void CreateViewportWindow(ViewportWindowConfiguration config)
        {
            ViewportWindow win = new ViewportWindow(config.Configuration);
            win.Text += " - Window " + config.WindowID;
            LoadViewports(win.TableSplitControl, config);
            Windows.Add(win);
            win.Closed += (s, e) =>
            {
                Windows.Remove(win);
                win.Dispose();
            };
            win.Show(Editor.Instance);
            if (!config.Size.IsEmpty)
            {
                win.Location = config.Size.Location;
                win.Size = config.Size.Size;
                win.WindowState = config.Maximised ? FormWindowState.Maximized : FormWindowState.Normal;
            }
        }

        private static void LoadViewports(TableSplitControl tableSplitControl, ViewportWindowConfiguration config)
        {
            Tuple<bool, Viewport3D.ViewType, Viewport2D.ViewDirection>[] defaultViewports = new[]
            {
                Tuple.Create(true, Viewport3D.ViewType.Lightmapped, Viewport2D.ViewDirection.Top),
                Tuple.Create(false, Viewport3D.ViewType.Lightmapped, Viewport2D.ViewDirection.Top),
                Tuple.Create(false, Viewport3D.ViewType.Lightmapped, Viewport2D.ViewDirection.Front),
                Tuple.Create(false, Viewport3D.ViewType.Lightmapped, Viewport2D.ViewDirection.Side)
            };

            List<string> viewports = config.Viewports ?? new List<string>();

            for (int i = 0; i < config.Configuration.Rectangles.Count; i++)
            {
                string viewport = viewports.Count > i ? viewports[i] : "";
                Tuple<bool, Viewport3D.ViewType, Viewport2D.ViewDirection> def = defaultViewports[(i % defaultViewports.Length)];
                ViewportBase vp = CreateViewport(viewport, def.Item1, def.Item2, def.Item3);
                Viewports.Add(vp);
                SubscribeExceptions(vp);
                tableSplitControl.Controls.Add(vp);
                Mediator.Publish(EditorMediator.ViewportCreated, vp);
                vp.Run();
            }
        }

        private static ViewportWindowConfiguration GetDefaultWindowConfiguration()
        {
            return new ViewportWindowConfiguration
            {
                WindowID = 0,
                Size = Rectangle.Empty,
                Configuration = TableSplitConfiguration.Default(),
                Maximised = true
            };
        }

        private static string SerialiseViewport(Control vp)
        {
            Viewport2D vp2 = vp as Viewport2D;
            Viewport3D vp3 = vp as Viewport3D;
            if (vp2 != null) return "Viewport2D." + vp2.Direction;
            if (vp3 != null) return "Viewport3D." + vp3.Type;
            return "";
        }

        public static void SaveLayout()
        {
            SettingsManager.SetAdditionalData("ViewportManagerWindowConfiguration", GetWindowConfigurations());
        }

        public static List<ViewportWindowConfiguration> GetWindowConfigurations()
        {
            List<ViewportWindowConfiguration> list = new List<ViewportWindowConfiguration>();
            list.Add(new ViewportWindowConfiguration
            {
                WindowID = 0,
                Size = new Rectangle(Editor.Instance.Location, Editor.Instance.Size),
                Configuration = MainWindowGrid.Configuration,
                Viewports = GetViewportsForTableSplitControl(MainWindowGrid).Select(SerialiseViewport).ToList(),
                Maximised = Editor.Instance.WindowState != FormWindowState.Normal
            });
            for (int i = 0; i < Windows.Count; i++)
            {
                list.Add(new ViewportWindowConfiguration
                {
                    WindowID = i + 1,
                    Size = new Rectangle(Windows[i].Location, Windows[i].Size),
                    Configuration = Windows[i].TableSplitControl.Configuration,
                    Viewports = GetViewportsForTableSplitControl(Windows[i].TableSplitControl).Select(SerialiseViewport).ToList(),
                    Maximised = Windows[i].WindowState != FormWindowState.Normal
                });
            }
            return list;
        }

        private static void SetConfiguration(TableSplitControl control, ViewportWindowConfiguration configuration)
        {
            if (control == null || configuration == null) return;

            control.Configuration = configuration.Configuration;
            List<ViewportBase> viewports = GetViewportsForTableSplitControl(control).ToList();

            control.Controls.Clear();
            foreach (ViewportBase vp in viewports)
            {
                vp.Dispose();
                Viewports.Remove(vp);
            }

            LoadViewports(control, configuration);
        }

        public static void SetWindowConfigurations(List<ViewportWindowConfiguration> configurations)
        {
            SetConfiguration(MainWindowGrid, configurations.FirstOrDefault(x => x.WindowID == 0));
            for (int i = 0; i < Windows.Count; i++)
            {
                ViewportWindow win = Windows[i];
                SetConfiguration(win.TableSplitControl, configurations.FirstOrDefault(x => x.WindowID == i + 1));
            }
        }

        private static IEnumerable<ViewportBase> GetViewportsForTableSplitControl(TableSplitControl control)
        {
            return Viewports.Where(x => GetParentSplitControl(x) == control);
        }

        public static PointF GetSplitterPosition()
        {
            int[] w = MainWindowGrid.GetColumnWidths();
            int[] h = MainWindowGrid.GetRowHeights();
            return new PointF(MainWindowGrid.ColumnStyles[0].Width, MainWindowGrid.RowStyles[0].Height);
        }

        public static void SetSplitterPosition(PointF pos)
        {
            if (pos.X <= 0 || pos.Y <= 0) return;
            MainWindowGrid.ColumnStyles[0].Width = pos.X;
            MainWindowGrid.ColumnStyles[1].Width = 100 - pos.X;
            MainWindowGrid.RowStyles[0].Height = pos.Y;
            MainWindowGrid.RowStyles[1].Height = 100 - pos.Y;
        }

        private static void SubscribeExceptions(ViewportBase vp)
        {
            vp.ListenerException += (sender, ex) => Logging.Logger.ShowException(ex, Local.LocalString("exception.viewport_listener"));
            vp.RenderException += (sender, ex) => Logging.Logger.ShowException(ex, Local.LocalString("exception.viewport_render"));
        }

        private static void RunAll()
        {
            Viewports.ForEach(v => v.Run());
        }

        public static void ClearContexts()
        {
            Viewports.ForEach(v => v.RenderContext.Clear());
        }

        public static void AddContextAll(IRenderable r)
        {
            Viewports.ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContextAll(IRenderable r)
        {
            Viewports.ForEach(v => v.RenderContext.Remove(r));
        }

        public static void AddContext3D(IRenderable r)
        {
            Viewports.OfType<Viewport3D>().ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContext3D(IRenderable r)
        {
            Viewports.OfType<Viewport3D>().ToList().ForEach(v => v.RenderContext.Remove(r));
        }

        public static void AddContext2D(IRenderable r)
        {
            Viewports.OfType<Viewport2D>().ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContext2D(IRenderable r)
        {
            Viewports.OfType<Viewport2D>().ToList().ForEach(v => v.RenderContext.Remove(r));
        }

        public static void AddContext2D(IRenderable r, Viewport2D.ViewDirection dir)
        {
            Viewports.OfType<Viewport2D>().Where(v => v != null && v.Direction == dir).ToList().ForEach(v => v.RenderContext.Add(r));
        }

        public static void RemoveContext2D(IRenderable r, Viewport2D.ViewDirection dir)
        {
            Viewports.OfType<Viewport2D>().Where(v => v != null && v.Direction == dir).ToList().ForEach(v => v.RenderContext.Remove(r));
        }

        public static void RefreshClearColour(bool disabled = false)
        {
            foreach (ViewportBase vp in Viewports)
            {
                vp.MakeCurrent();
                if (!disabled)
                {
                    GL.ClearColor(vp is Viewport3D ? CBRE.Settings.View.ViewportBackground : CBRE.Settings.Grid.Background);
                }
                else
                {
                    Color gray = Color.FromArgb(150, 150, 150);
                    GL.ClearColor(gray);
                }
            }
        }

        private static Viewport3D Create3D(Viewport3D.ViewType type)
        {
            Viewport3D viewport = new Viewport3D(type)
            {
                Dock = DockStyle.Fill,
                Camera =
                {
                    Location = new Vector3(0, 0, 0),
                    LookAt = new Vector3(0, 1, 0),
                    FOV = CBRE.Settings.View.CameraFOV,
                    ClipDistance = CBRE.Settings.View.BackClippingPane
                },
                VSync = false
            };
            viewport.MakeCurrent();
            GraphicsHelper.InitGL3D();
            GL.ClearColor(CBRE.Settings.View.ViewportBackground);
            viewport.Listeners.Add(new ViewportLabelListener(viewport));
            viewport.Listeners.Add(new Camera3DViewportListener(viewport));
            viewport.Listeners.Add(new ToolViewportListener(viewport));
            return viewport;
        }

        private static Viewport2D Create2D(Viewport2D.ViewDirection direction)
        {
            Viewport2D viewport = new Viewport2D(direction)
            {
                Dock = DockStyle.Fill,
                VSync = false
            };
            viewport.MakeCurrent();
            GraphicsHelper.InitGL2D();
            GL.ClearColor(CBRE.Settings.Grid.Background);
            viewport.Listeners.Add(new ViewportLabelListener(viewport));
            viewport.Listeners.Add(new Camera2DViewportListener(viewport));
            viewport.Listeners.Add(new Grid2DEventListener(viewport));
            viewport.Listeners.Add(new ToolViewportListener(viewport));
            return viewport;
        }

        private static TableSplitControl GetParentSplitControl(ViewportBase viewport)
        {
            Control par = viewport.Parent;
            while (par != null && !(par is TableSplitControl))
            {
                par = par.Parent;
            }
            return par as TableSplitControl;
        }

        public static Viewport2D Make2D(ViewportBase viewport, Viewport2D.ViewDirection direction)
        {
            TableSplitControl parent = GetParentSplitControl(viewport);
            if (parent == null) return null;

            Viewports.Remove(viewport);

            Viewport2D newViewport = Create2D(direction);
            SubscribeExceptions(newViewport);

            parent.ReplaceControl(viewport, newViewport);

            Viewports.Add(newViewport);

            viewport.Dispose();
            newViewport.Run();

            return newViewport;
        }

        public static Viewport3D Make3D(ViewportBase viewport, Viewport3D.ViewType type)
        {
            TableSplitControl parent = GetParentSplitControl(viewport);
            if (parent == null) return null;

            Viewports.Remove(viewport);

            Viewport3D newViewport = Create3D(type);
            SubscribeExceptions(newViewport);

            parent.ReplaceControl(viewport, newViewport);

            Viewports.Add(newViewport);

            viewport.Dispose();
            newViewport.Run();

            return newViewport;
        }

        public static Image CreateScreenshot(ViewportBase viewport, int width, int height)
        {
            ScreenshotViewportListener shot = new ScreenshotViewportListener(viewport);
            TableSplitControl parent = GetParentSplitControl(viewport);
            if (parent == null) return null;

            TableLayoutPanelCellPosition pos = parent.GetPositionFromControl(viewport);
            Form form = new Form();
            //form.FormBorderStyle = FormBorderStyle.None;
            form.TopMost = true;
            form.Width = width;
            form.Height = height;
            form.WindowState = FormWindowState.Maximized;
            Panel panel = new Panel { Width = width, Height = height };
            form.Controls.Add(panel);
            panel.Controls.Add(viewport);

            viewport.Dock = DockStyle.Top | DockStyle.Left;
            viewport.Width = width;
            viewport.Height = height;

            //form.ShowDialog();
            viewport.Listeners.Add(shot);
            viewport.UpdateNextFrameImmediately();
            viewport.Listeners.Remove(shot);

            viewport.Dock = DockStyle.Fill;
            parent.Controls.Add(viewport, pos.Column, pos.Row);
            return shot.Screenshot;
        }
    }
}
