using System;
using System.Collections.Generic;
using System.Linq;
using Elements;
using Elements.Geometry;
using Elements.Geometry.Solids;

namespace Facade
{
    internal class LevelComparer : IComparer<Level>
    {
        public int Compare(Level x, Level y)
        {
            if(x.Elevation > y.Elevation)
            {
                return 1;
            }
            else if(x.Elevation < y.Elevation)
            {
                return -1;
            }
            return 0;
        }
    }

    public static class Facade
	{
        private const int Elevation = 10;
        private static string ENVELOPE_MODEL_NAME = "Envelope";
        private static string LEVELS_MODEL_NAME = "Levels";

        private static Material _glazing = new Material("Glazing", new Color(1.0,1.0,1.0, 0.7), 0.8f, 1.0f);
        private static Material _nonStandardPanel = new Material(Colors.Orange, 0.0, 0.0, Guid.NewGuid(), "Non-standard Panel");

        /// <summary>
        /// Adds facade Panels to one or more Masses named 'envelope'.
        /// </summary>
        /// <param name="model">The model. 
        /// Add elements to the model to have them persisted.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A FacadeOutputs instance containing computed results.</returns>
        public static FacadeOutputs Execute(Dictionary<string, Model> models, FacadeInputs input)
		{
            List<Envelope> envelopes;
            List<Level> levels = null;
            var model = new Model();

            if(models.ContainsKey(ENVELOPE_MODEL_NAME))
            {
                var envelopeModel = models[ENVELOPE_MODEL_NAME];
                envelopes = envelopeModel.AllElementsOfType<Envelope>().Where(e=>e.Elevation >= 0.0).ToList();
                var levelsModel = models[LEVELS_MODEL_NAME];
                levels = levelsModel.AllElementsOfType<Level>().ToList();
            }
            else
            {
                var envMaterial = BuiltInMaterials.Void;
                var p1 = Polygon.Ngon(3, 10);
                var p2 = p1.Offset(-1)[0];
                var env1 = new Envelope(p1,
                                        0,
                                        10,
                                        Vector3.ZAxis,
                                        0.0,
                                        new Transform(),
                                        envMaterial,
                                        new Representation(new List<SolidOperation>() { new Extrude(p1, 10, Vector3.ZAxis, 0, false) }),
                                        Guid.NewGuid(),
                                        "envelope");
                var env2 = new Envelope(p2,
                                        Elevation,
                                        20,
                                        Vector3.ZAxis,
                                        0.0,
                                        new Transform(0,0,10),
                                        envMaterial,
                                        new Representation(new List<SolidOperation>() { new Extrude(p2, 10, Vector3.ZAxis, 0, false) }),
                                        Guid.NewGuid(),
                                        "envelope");
                envelopes = new List<Envelope>(){env1, env2};
                levels = new List<Level>();
                for(var i=0; i<20; i+=3)
                {
                    levels.Add(new Level(i, Guid.NewGuid(), $"Level {i}"));
                }
                // model.AddElements(envelopes);
            }

            levels.Sort(new LevelComparer());

            var panelCount = 0;

            var panelMat = new Material("envelope", new Color(1.0, 1.0, 1.0, 1), 0.5f, 0.5f);
            List<Level> envLevels = null;
            foreach(var envelope in envelopes)
            {
                var boundarySegments = envelope.Profile.Perimeter.Segments();
                Level last = null;
                if(envLevels != null)
                {
                    // If levels don't correspond exactly with the change
                    // in envelopes, then we need the last level of the previous
                    // set to become the first level of the next set.
                    last = envLevels.Last();
                }
                envLevels = levels.Where(l=>l.Elevation >= envelope.Elevation && l.Elevation <= envelope.Elevation + envelope.Height).ToList();
                if(last != null)
                {
                    envLevels.Insert(0, last);
                }

                foreach(var s in boundarySegments)
                {
                    var d = s.Direction();
                    for(var i=0; i<envLevels.Count-1; i++)
                    {
                        try{
                            var level1 = envLevels[i];
                            var level2 = envLevels[i+1];
                            var bottom = new Line(new Vector3(s.Start.X, s.Start.Y, level1.Elevation), new Vector3(s.End.X, s.End.Y, level1.Elevation));
                            var top = new Line(new Vector3(s.Start.X, s.Start.Y, level2.Elevation), new Vector3(s.End.X, s.End.Y, level2.Elevation));
                            var topSegments = top.DivideByLength(input.PanelWidth);
                            var bottomSegments = bottom.DivideByLength(input.PanelWidth);
                            for(var j=0; j<bottomSegments.Count(); j++)
                            {
                                var bs = bottomSegments[j];
                                var ts = topSegments[j];
                                var t = new Transform(bs.Start, d, d.Cross(Vector3.ZAxis));
                                var panel = CreateFacadePanel($"FP_{i}_{j}",
                                                            bs.Length(),
                                                            level2.Elevation - level1.Elevation,
                                                            input.GlassLeftRightInset,
                                                            input.GlassTopBottomInset,
                                                            0.1,
                                                            input.PanelWidth,
                                                            panelMat,
                                                            t,
                                                            model);
                                // var panel = CreateBBFacadePanel($"FP_{i}_{j}",
                                //                             bs.Length(),
                                //                             level2.Elevation - level1.Elevation,
                                //                             d1: 0.1,
                                //                             d2: 0.4,
                                //                             h1: 0.3,
                                //                             h2: 0.05,
                                //                             gap: 0.05,
                                //                             panelMat,
                                //                             t,
                                //                             model);
                                panelCount++;
                            }

                            if(i == envLevels.Count - 2)
                            {
                                var parapet= new StandardWall(new Line(new Vector3(s.Start.X, s.Start.Y, level2.Elevation), new Vector3(s.End.X, s.End.Y, level2.Elevation)), 0.1, 0.9, panelMat);
                                model.AddElement(parapet);
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex);
                            continue;
                        }
                    }
                }
            }
            
            var output = new FacadeOutputs(panelCount);
            output.model = model;
			return output;
		}

        private static FacadePanel CreateBBFacadePanel(string name,
                                                     double width,
                                                     double height,
                                                     double d1,
                                                     double d2,
                                                     double h1,
                                                     double h2,
                                                     double gap,
                                                     Material material,
                                                     Transform lowerLeft,
                                                     Model model)
        {
            var ll = new Vector3(0,0,0);
            var lr = new Vector3(width,0,0);
            var ur = new Vector3(width, height, 0);
            var ul = new Vector3(0, height, 0);
            
            // ---- d1
            // |   \   
            // |    \   h1
            // |     \  
            // |      | h2 
            // o-------
            //    d2
            var o = Vector3.Origin;
            var a = o + new Vector3(d2, 0);
            var b = a + new Vector3(0, h2);
            var c = b + new Vector3(-(d2-d1), h1);
            var d = c + new Vector3(-d1,0);

            var p = new Polygon(new[]{o,a,b,c,d});
            var solidOps = new List<SolidOperation>();
            var profile = new Profile(p);
            
            var bottom = new Sweep(p, new Line(lr, ll), 0, 0, 0, false);
            var right = new Sweep(p, new Line(lr + new Vector3(gap,0),ur + new Vector3(gap,0)), 0, 0, 0, false);
            var top = new Sweep(p, new Line(ul, ur), 0, 0, 0, false);
            var left = new Sweep(p, new Line(ul - new Vector3(gap,0), ll - new Vector3(gap,0)), 0, 0, 0, false);
            solidOps.AddRange(new []{bottom, right, top, left});
            // var poly = new Polygon(new[]{ll,lr,ur,ul});
            // var sweep = new Sweep(profile, poly, 0, 0, 0, false);
            var rep = new Representation(solidOps);
            var panel = new FacadePanel(d1, lowerLeft, material, rep, Guid.NewGuid(), name);
            model.AddElement(panel);
            
            return panel;

        }

        private static FacadePanel CreateFacadePanel(string name,
                                                     double width,
                                                     double height,
                                                     double leftRightInset,
                                                     double topBottomInset,
                                                     double thickness,
                                                     double defaultWidth,
                                                     Material material,
                                                     Transform lowerLeft,
                                                     Model model)
        {
            var a = new Vector3(0,0,0);
            var b = new Vector3(width,0,0);
            var c = new Vector3(width, height, 0);
            var d = new Vector3(0, height, 0);

            Profile profile;
            var mat = material;
            //if(Math.Abs(width - defaultWidth) > Vector3.Epsilon)
            //{
            //    mat = _nonStandardPanel;
            //    profile = new Profile(new Polygon(new[]{a,b,c,d}.Shrink(0.01)));
            //}
            //else
            //{
                var a1 = new Vector3(leftRightInset, topBottomInset, 0);
                var b1 = new Vector3(width-leftRightInset, topBottomInset,0);
                var c1 = new Vector3(width-leftRightInset, height-topBottomInset, 0);
                var d1 = new Vector3(leftRightInset, height-topBottomInset, 0);
                var inner = new Polygon(new[]{d1,c1,b1,a1});
                profile = new Profile(new Polygon(new[]{a,b,c,d}.Shrink(0.01)), inner);
                var glazing = new Panel(inner, _glazing, lowerLeft);
                model.AddElement(glazing);
            //}
            
            var solidOps = new List<SolidOperation>(){new Extrude(profile, thickness, Vector3.ZAxis, 0.0, false)};
            var representation = new Representation(solidOps);
            var panel = new FacadePanel(thickness, lowerLeft, mat, representation, Guid.NewGuid(), name);
            model.AddElement(panel);

            return panel;
        }
  	}
}