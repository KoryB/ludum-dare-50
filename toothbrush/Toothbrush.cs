using Godot;
using System;

public class Toothbrush : Spatial
{
    [Export]
    private float _transform_interpolation_time = 0.05f;
    private float _transform_interpolation_timer = 0.0f;
    
    
    private Transform _previous_transform;
    private Transform _target_transform;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _previous_transform = this.Transform;
        _target_transform = this.Transform;
    }

    public void SetTargetTransform(Transform target_transform)
    {
        _transform_interpolation_timer = 0.0f;
        _previous_transform = this.Transform;
        _target_transform = target_transform;
    }
    
    public override void _Process(float delta)
    {
        _transform_interpolation_timer += delta;
        _transform_interpolation_timer = Math.Min(_transform_interpolation_timer, _transform_interpolation_time);
        
        var i = _transform_interpolation_timer / _transform_interpolation_time;
        this.Transform = _previous_transform.InterpolateWith(_target_transform, i);
    }
}
