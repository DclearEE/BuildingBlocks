//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.1.21.0 (Newtonsoft.Json v12.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------
using Elements;
using Elements.GeoJSON;
using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements.Properties;
using Elements.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using Line = Elements.Geometry.Line;
using Polygon = Elements.Geometry.Polygon;

namespace Elements
{
    #pragma warning disable // Disable all warnings

    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.21.0 (Newtonsoft.Json v12.0.0.0)")]
    [UserElement]
	public partial class Mesh 
    {
        [Newtonsoft.Json.JsonConstructor]
        public Mesh(IList<triangles> @triangles, IList<vertices> @vertices)
        {
            var validator = Validator.Instance.GetFirstValidatorForType<Mesh>
            ();
            if(validator != null)
            {
                validator.PreConstruct(new object[]{ @triangles, @vertices});
            }
        
                this.Triangles = @triangles;
                this.Vertices = @vertices;
            
            if(validator != null)
            {
            validator.PostConstruct(this);
            }
            }
    
        [Newtonsoft.Json.JsonProperty("triangles", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IList<triangles> Triangles { get; set; }
    
        [Newtonsoft.Json.JsonProperty("vertices", Required = Newtonsoft.Json.Required.DisallowNull, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
        public IList<vertices> Vertices { get; set; }
    
    
    }
}