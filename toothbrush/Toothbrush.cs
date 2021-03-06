using Godot;
using System;
using System.Collections.Generic;

public class Toothbrush : Spatial, IBrush
{
    [Export]
    private float _transform_interpolation_time = 0.05f;
    private float _transform_interpolation_timer = 0.0f;
    
    [Export]
    private Godot.Collections.Array _strength_progression;
    [Export]
    private Godot.Collections.Array _color_progression;
    
    [Export]
    private int _level = 0;
    
    private ISet<IBrushable> _cleaned_cache = new HashSet<IBrushable>();
    
    private CSGBox _handle;
    private Area _brush_area;
    
    private Transform _previous_transform;
    private Transform _target_transform;
    
    private RandomNumberGenerator _rng = new RandomNumberGenerator();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _previous_transform = this.Transform;
        _target_transform = this.Transform;
        
        _handle = GetNode<CSGBox>("Handle");
        _brush_area = GetNode<Area>("BrushArea");
    }

    public void SetTargetTransform(Transform target_transform)
    {
        if (IsNewTargetTransformDirectionChange(target_transform))
        {
            _cleaned_cache.Clear();
            
            var i = _rng.RandiRange(1, 4);
            GetNode<AudioStreamPlayer>($"Audio/BrushPlayer{i}").Play();
        }
    
        _transform_interpolation_timer = 0.0f;
        _previous_transform = this.Transform;
        _target_transform = target_transform;
    }
    
    public override void _Process(float delta)
    {
        UpdateTransformTimer(delta);
        UpdateTransform(delta);
        UpdateColor();
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
    
    private void UpdateColor()
    {
        if (_handle.Material is SpatialMaterial sm)
        {
            sm.AlbedoColor = GetColor();
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
    
    public void LevelUp()
    {
        _level = Math.Min(_level + 1, _strength_progression.Count - 1);
    }
    
    public float GetStrength()
    {
        return (float) _strength_progression[_level];
    }
    
    public Color GetColor()
    {
        return (Color) _color_progression[_level];
    }
}
