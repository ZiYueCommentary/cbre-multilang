﻿using CBRE.DataStructures.Geometric;
using CBRE.DataStructures.MapObjects;
using CBRE.DataStructures.Transformations;
using CBRE.Localization;
using CBRE.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CBRE.Editor.Tools.VMTool
{
    public class EditFaceTool : VMSubTool
    {
        private List<Face> _selection;

        public EditFaceTool(VMTool mainTool) : base(mainTool)
        {
            EditFaceControl ef = new EditFaceControl();
            ef.Poke += Poke;
            ef.Bevel += Bevel;
            Control = ef;
        }

        private void Poke(object sender, int num)
        {
            foreach (Face face in _selection.ToArray())
            {
                PokeFace(face, num);
            }
        }

        private void Bevel(object sender, int num)
        {
            foreach (Face face in _selection.ToArray())
            {
                BevelFace(face, num);
            }
        }

        private void PokeFace(Face face, int num)
        {
            Solid solid = face.Parent;
            // Remove the face
            solid.Faces.Remove(face);
            face.Parent = null;
            _selection.Remove(face);

            Coordinate center = face.BoundingBox.Center + face.Plane.Normal * num;
            foreach (Line edge in face.GetEdges())
            {
                Vertex v1 = face.Vertices.First(x => x.Location == edge.Start);
                Vertex v2 = face.Vertices.First(x => x.Location == edge.End);
                Coordinate[] verts = new[] { v1.Location, v2.Location, center };
                Face f = new Face(Document.Map.IDGenerator.GetNextFaceID())
                {
                    Parent = solid,
                    Plane = new Plane(verts[0], verts[1], verts[2]),
                    Colour = solid.Colour,
                    Texture = face.Texture.Clone()
                };
                f.Vertices.AddRange(verts.Select(x => new Vertex(x, face)));
                f.UpdateBoundingBox();
                f.AlignTextureToFace();
                solid.Faces.Add(f);
                _selection.Add(f);
            }
            solid.UpdateBoundingBox();
            UpdateSelection();

            MainTool.SetDirty(true, true);
        }

        private void BevelFace(Face face, int num)
        {
            Solid solid = face.Parent;
            Dictionary<Vertex, Coordinate> vertexCoordinates = face.Vertices.ToDictionary(x => x, x => x.Location);
            // Scale the face a bit and move it away by the bevel distance
            face.Transform(new UnitScale(Coordinate.One * 0.9m, face.BoundingBox.Center), TransformFlags.TextureLock);
            face.Transform(new UnitTranslate(face.Plane.Normal * num), TransformFlags.TextureLock);
            foreach (Line edge in face.GetEdges())
            {
                Vertex v1 = face.Vertices.First(x => x.Location == edge.Start);
                Vertex v2 = face.Vertices.First(x => x.Location == edge.End);
                Coordinate[] verts = new[] { vertexCoordinates[v1], vertexCoordinates[v2], v2.Location, v1.Location };
                Face f = new Face(Document.Map.IDGenerator.GetNextFaceID())
                {
                    Parent = solid,
                    Plane = new Plane(verts[0], verts[1], verts[2]),
                    Colour = solid.Colour,
                    Texture = face.Texture.Clone()
                };
                f.Vertices.AddRange(verts.Select(x => new Vertex(x, face)));
                f.UpdateBoundingBox();
                f.AlignTextureToFace();
                solid.Faces.Add(f);
                _selection.Add(f);
            }
            solid.UpdateBoundingBox();
            UpdateSelection();

            MainTool.SetDirty(true, true);
        }

        public override string GetName()
        {
            return Local.LocalString("tool.face");
        }

        public override string GetContextualHelp()
        {
            return Local.LocalString("tool.face.help");
        }

        public override void ToolSelected(bool preventHistory)
        {
            _selection = new List<Face>();
            UpdateSelection();
        }

        public override void ToolDeselected(bool preventHistory)
        {
            _selection.Clear();
            UpdateSelection();
        }

        public override List<VMPoint> GetVerticesAtPoint(int x, int y, Viewport2D viewport)
        {
            return new List<VMPoint>();
        }

        public override List<VMPoint> GetVerticesAtPoint(int x, int y, Viewport3D viewport)
        {
            return new List<VMPoint>();
        }

        public override void DragStart(List<VMPoint> clickedPoints)
        {

        }

        public override void DragMove(Coordinate distance)
        {

        }

        public override void DragEnd()
        {

        }

        public override void MouseEnter(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void MouseLeave(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void MouseDown(ViewportBase viewport, ViewportEvent e)
        {
            Viewport3D vp = viewport as Viewport3D;
            if (vp == null || e.Button != MouseButtons.Left) return;

            // Do selection
            e.Handled = true;
            Line ray = vp.CastRayFromScreen(e.X, e.Y);
            IEnumerable<Solid> hits = MainTool.GetCopies().Where(x => x.BoundingBox.IntersectsWith(ray));
            Face clickedFace = hits.SelectMany(f => f.Faces)
                .Select(x => new { Item = x, Intersection = x.GetIntersectionPoint(ray) })
                .Where(x => x.Intersection != null)
                .OrderBy(x => (x.Intersection - ray.Start).VectorMagnitude())
                .Select(x => x.Item)
                .FirstOrDefault();

            List<Face> faces = new List<Face>();
            if (clickedFace != null)
            {
                if (KeyboardState.Shift) faces.AddRange(clickedFace.Parent.Faces);
                else faces.Add(clickedFace);
            }

            if (!KeyboardState.Ctrl) _selection.Clear();
            _selection.AddRange(faces);

            UpdateSelection();
        }

        private void UpdateSelection()
        {
            foreach (Solid s in MainTool.GetCopies())
            {
                s.IsSelected = false;
                foreach (Face f in s.Faces) f.IsSelected = _selection.Contains(f);
            }
        }

        public override void MouseClick(ViewportBase viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseDoubleClick(ViewportBase viewport, ViewportEvent e)
        {
            // Not used
        }

        public override void MouseUp(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void MouseWheel(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void MouseMove(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void KeyPress(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void KeyDown(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void KeyUp(ViewportBase viewport, ViewportEvent e)
        {

        }

        public override void UpdateFrame(ViewportBase viewport, FrameInfo frame)
        {

        }

        public override void Render(ViewportBase viewport)
        {

        }

        public override void Render2D(Viewport2D viewport)
        {

        }

        public override void Render3D(Viewport3D viewport)
        {

        }

        public override void SelectionChanged()
        {

        }

        public override bool ShouldDeselect(List<VMPoint> vtxs)
        {
            return true;
        }

        public override bool NoSelection()
        {
            return true;
        }

        public override bool No3DSelection()
        {
            return true;
        }

        public override bool DrawVertices()
        {
            return false;
        }
    }
}
