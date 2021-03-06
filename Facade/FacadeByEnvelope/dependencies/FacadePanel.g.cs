//----------------------
// <auto-generated>
//     Generated using the NJsonSchema v10.1.21.0 (Newtonsoft.Json v12.0.0.0) (http://NJsonSchema.org)
// </auto-generated>
//----------------------
using Elements;
using Elements.GeoJSON;
using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements.Validators;
using Elements.Serialization.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using Line = Elements.Geometry.Line;
using Polygon = Elements.Geometry.Polygon;

namespace Elements
{
    #pragma warning disable // Disable all warnings

    /// <summary>A single panel within a facade.</summary>
    [Newtonsoft.Json.JsonConverter(typeof(Elements.Serialization.JSON.JsonInheritanceConverter), "discriminator")]
    [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.21.0 (Newtonsoft.Json v12.0.0.0)")]
    public partial class FacadePanel : GeometricElement
    {
        [Newtonsoft.Json.JsonConstructor]
        public FacadePanel(double @thickness, Transform @transform, Material @material, Representation @representation, bool @isElementDefinition, System.Guid @id, string @name)
            : base(transform, material, representation, isElementDefinition, id, name)
        {
            var validator = Validator.Instance.GetFirstValidatorForType<FacadePanel>();
            if(validator != null)
            {
                validator.PreConstruct(new object[]{ @thickness, @transform, @material, @representation, @isElementDefinition, @id, @name});
            }
        
            this.Thickness = @thickness;
            
            if(validator != null)
            {
                validator.PostConstruct(this);
            }
        }
    
        /// <summary>The total thickness of the panel.</summary>
        [Newtonsoft.Json.JsonProperty("Thickness", Required = Newtonsoft.Json.Required.Always)]
        public double Thickness { get; set; }
    
    
    }
}