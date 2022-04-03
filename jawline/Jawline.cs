using Godot;
using System;
using System.Linq;

public class Jawline : Spatial
{
    private static PackedScene ParticleScene = ResourceLoader.Load("res://particle/Food.tscn") as PackedScene;
    private const float RayLength = 1000.0f;
    
    [Export]
    private float _radius = 1.0f;
    [Export]
    private float half_inflation_angle_range_degrees = 45.0f;
    [Export]
    private float half_forward_angle_range_degrees = 20.0f;
    [Export]
    private bool _is_negate_up = false;
    
    private Path _path;
    private Curve3D _curve;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _path = GetNode<Path>("Path");
        _curve = _path.Curve;
    }
    

    public Node SpawnParticleRandom()
    {
        var path_location = _rng.Randf();
        var inflation_angle = MathUtils.DegreesToRadians(_rng.RandfRange(-half_inflation_angle_range_degrees, half_inflation_angle_range_degrees));
        var forward_angle = MathUtils.DegreesToRadians(_rng.RandfRange(-half_forward_angle_range_degrees, half_forward_angle_range_degrees));
        
        var particle = SpawnParticle(path_location, inflation_angle, forward_angle);
        
        while (particle == null)
        {
            path_location += _rng.RandfRange(-0.3f, 0.3f);
            inflation_angle += _rng.RandfRange(-0.3f, 0.3f);
            forward_angle += _rng.RandfRange(-0.3f, 0.3f);
            
            particle = SpawnParticle(path_location, inflation_angle, forward_angle);
        }
        
        return particle;
    }
    
    
    public Node SpawnParticle(float path_location, float inflation_angle, float forward_angle)
    {
        var final_position = GetToothPosition(
            path_location, 
            inflation_angle,
            forward_angle
        );
        
        if (final_position.HasValue)
        {
            var particle = (Particle) ParticleScene.Instance();
            AddChild(particle);
            
            particle.Translation = final_position.Value;
            return particle;
        }
        else
        {
            return null;
        }
    }
    
    
    public Vector3? GetToothPosition(float path_location, float inflation_angle, float forward_angle)
    {
        var forward_location = path_location >= 1.0f? 
            path_location - 0.01f : 
            Math.Min(path_location + 0.01f, 1.0f);
        
        var path_position = _curve.InterpolateBaked(path_location * _curve.GetBakedLength());
        var forward_path_position = _curve.InterpolateBaked(forward_location * _curve.GetBakedLength());
        
        var forward = (forward_path_position - path_position).Normalized();
        var up = (_is_negate_up? -1 : 1) * 
            _curve.InterpolateBakedUpVector(path_location * _curve.GetBakedLength())
            .Normalized();
        var normal = forward.Cross(up).Normalized();
        
        GD.Print(Name, " Up: ", up);
        
        var inflation_direction = up
            .Rotated(forward, inflation_angle)
            .Rotated(normal, forward_angle);
        var inflated_position = path_position + _radius * inflation_direction;
        
        var r = RaycastToTeeth(inflated_position, -inflation_direction);
        
        return r.Contains("position")?
            (Vector3?) r["position"] :
            (Vector3?) null;
    }
    
    
    private Godot.Collections.Dictionary RaycastToTeeth(Vector3 from, Vector3 direction)
    {
        return GetWorld().DirectSpaceState.IntersectRay(from, from + RayLength * direction);
    }
}
