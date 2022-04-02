using Godot;
using System;
using System.Collections.Generic;

public class Toothbrush : Spatial, IBrush
{
    [Export]
    private float _transform_interpolation_time = 0.05f;
    private float _transform_interpolation_timer = 0.0f;
    
    [Export]
    private float _strength = 1.0f;
    
    private ISet<IBrushable> _cleaned_cache = new HashSet<IBrushable>();
    
    private Area _brush_area;
    
    private Transform _previous_transform;
    private Transform _target_transform;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _previous_transform = this.Transform;
        _target_transform = this.Transform;
        _brush_area = GetNode<Area>("BrushArea");
    }

    public void SetTargetTransform(Transform target_transform)
    {
        if (IsNewTargetTransformDirectionChange(target_transform))
        {
            GD.Print("Direction Changed! ", _cleaned_cache.Count);
            _cleaned_cache.Clear();
        }
    
        _transform_interpolation_timer = 0.0f;
        _previous_transform = this.Transform;
        _target_transform = target_transform;
        
        
    }
    
    public override void _Process(float delta)
    {
        UpdateTransformTimer(delta);
        UpdateTransform(delta);
    }
    
    public override void _PhysicsProcess(float delta)
    {
        UpdateBrushing(delta);    
    }
    
    private void UpdateTransformTimer(float delta)
    {
        _transform_interpolation_timer += delta;
        _transform_interpolation_timer = Math.Min(_transform_interpolation_timer, _transform_interpolation_time);
    }
    
    private void UpdateTransform(float delta)
    {
        var i = GetTransformInterpolationValue();
        
        try
        {
            this.Transform = _previous_transform.InterpolateWith(_target_transform, i);
        }
        catch (System.ArgumentException e)
        {
            // pass
        }        
    }
    
    private void UpdateBrushing(float delta)
    {
        foreach (var node in _brush_area.GetOverlappingBodies())
        {
            if (node is IBrushable brushable)
            {
                if (! _cleaned_cache.Contains(brushable))
                {
                    brushable.Brush(this);
                    _cleaned_cache.Add(brushable);   
                }
            }
        }
    }
    
    private bool IsNewTargetTransformDirectionChange(Transform target_transform)
    {
        var current_direction = this.Transform.origin - _previous_transform.origin;
        var new_direction = target_transform.origin - this.Transform.origin;
        
        return current_direction.Dot(new_direction) <= 0.0f;
    }
    
    private float GetTransformInterpolationValue()
    {
        return _transform_interpolation_timer / _transform_interpolation_time;
    }
    
    private bool IsBrushMoving()
    {
        return GetTransformInterpolationValue() < 1.0;
    }
    
    public float GetStrength()
    {
        return _strength;
    }
}
