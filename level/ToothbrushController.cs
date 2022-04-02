using Godot;
using Godot.Collections;
using System;

public class ToothbrushController : Spatial
{
    [Export]
    private NodePath _toothbrush_path;
    private Spatial _toothbrush;
    
    private Vector3 _previous_brush_target;
    
    private const float RayLength = 1000;
    
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _toothbrush = GetNode<Spatial>(_toothbrush_path);
    }
    
    public override void _PhysicsProcess(float delta)
    {
        var r = CastRayFromCamera();
        
        if (r.Contains("position") && r.Contains("normal"))
        {
            var brush_target = (Vector3) r["position"];
            var tooth_normal = (Vector3) r["normal"];
            var target_delta = brush_target - _previous_brush_target;
            
            if (brush_target.IsEqualApprox(_previous_brush_target))
            {
                return;
            }
            
            _toothbrush.Translation = brush_target;
            
            var x = tooth_normal;
            
            if (_toothbrush.Transform.basis.z.Dot(target_delta) >= 0)
            {
                var z = _toothbrush.Transform.basis.z.LinearInterpolate(target_delta.Normalized(), 0.25f).Normalized();
                var y = z.Cross(x);
                
                _toothbrush.Transform = new Transform(
                    x,
                    y,
                    z,
                    _toothbrush.Transform.origin
                );
            }
            else
            {
                 _toothbrush.Transform = new Transform(
                    x,
                    _toothbrush.Transform.basis.y,
                    _toothbrush.Transform.basis.z,
                    _toothbrush.Transform.origin
                );   
            }
            
            _previous_brush_target = brush_target;
        }
    }
    
    private Dictionary CastRayFromCamera()
    {
        var mouse_pos = GetViewport().GetMousePosition();
        var camera = GetViewport().GetCamera();
        
        var from = camera.ProjectRayOrigin(mouse_pos);
        var to = from + camera.ProjectRayNormal(mouse_pos) * RayLength;
        
        var result = GetWorld().DirectSpaceState.IntersectRay(from, to);
        
        return result;
    }
}
