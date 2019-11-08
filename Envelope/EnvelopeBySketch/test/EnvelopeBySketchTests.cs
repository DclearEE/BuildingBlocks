﻿using Xunit;
using System.Linq;
using Newtonsoft.Json;
using Elements;
using Hypar.Functions.Execution.Local;
using Elements.Serialization.glTF;
using Elements.Serialization.JSON;
using System.Collections.Generic;
using Elements.Geometry;


namespace EnvelopeBySketch.Tests
{
    public class EnvelopeBySketchTests
    {
        [Fact]
        public void EnvelopeBySketchTest()
        {
            var model = new Model();
            var polygon =
                new Polygon(
                    new[]
                    {
                        new Vector3(-46.0, -29.0, 0.0),
                        new Vector3(-10.0, -43.0, -0.0),
                        new Vector3(33.0, -40.0, -0.0),
                        new Vector3(36.0, 71.0, 0.0)
                    });
            var inputs = 
                new EnvelopeBySketchInputs (polygon, 100.0, 10.0, "", "", new Dictionary<string, string>(), "", "", "");
            var outputs = EnvelopeBySketch.Execute(new Dictionary<string, Model> { { "Envelope", model } }, inputs);
            System.IO.File.WriteAllText("../../../../../../TestOutput/EnvelopeBySketch.json", outputs.model.ToJson());
            outputs.model.ToGlTF("../../../../../../TestOutput/EnvelopeBySketch.glb");
        }
    }
}