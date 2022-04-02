using Godot;
using Godot.Collections;
using System;

public class ToothbrushController : Spatial
{
    private const float RayLength = 1000;
    
    [Export]
    private NodePath _toothbrush_path;
    private Toothbrush _toothbrush;
    
    private Vector3 _previous_brush_target;
    private Transform? _target_transform;
    
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _toothbrush = GetNode<Toothbrush>(_toothbrush_path);
    }
    
    public override void _PhysicsProcess(float delta)
    {
        var r = CastRayFromCamera();
        var t = CalculateTargetTransform(r);
        
        if (t.HasValue)
        {
            _toothbrush.SetTargetTransform(t.Value);
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
    
    private Transform? CalculateTargetTransform(Dictionary raycast_result)
    {
        if (!(raycast_result.Contains("position") && raycast_result.Contains("normal")))
        {
            return null;
        }
        
        var brush_target = (Vector3) raycast_result["position"];
        var tooth_normal = (Vector3) raycast_result["normal"];
        var target_delta = brush_target - _previous_brush_target;
            
        if (brush_target.IsEqualApprox(_previous_brush_target))
        {
            return null;
        }
        
        Vector3 x, y, z;
        x = tooth_normal;
        _previous_brush_target = brush_target;
        
        if (_toothbrush.Transform.basis.z.Dot(target_delta) >= 0)
        {
            z = _toothbrush.Transform.basis.z.LinearInterpolate(target_delta.Normalized(), 0.25f).Normalized();
            y = z.Cross(x);
        }
        else
        {
            y = _toothbrush.Transform.basis.y;
            z = _toothbrush.Transform.basis.z;
        }
            
        return new Transform(x, y, z, brush_target);
    }
}
