using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GridCameraController : GridContainer
{
    private const int ColumnsGridView = 2;
    private const int ColumnsFullscreen = 1;

    private const String TopLeft = "TL";
    private const String TopRight = "TR";
    private const String BottomLeft = "BL";
    private const String BottomRight = "BR";


    private bool _is_fullscreen = false;
    private IDictionary<String, ViewportContainer> _viewports;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _viewports = new List<String> {TopLeft, TopRight, BottomLeft, BottomRight}
            .ToDictionary(
                s => s,
                s => GetNode<ViewportContainer>($"ViewportContainer{s}")
            );
    }
    
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouse_event)
        {
            if (mouse_event.Pressed && mouse_event.Doubleclick)
            {
                OnDoubleClick(mouse_event);
            }
        }
    }
    
    private void OnDoubleClick(InputEventMouseButton mouse_event)
    {
        if (_is_fullscreen)
        {
            ReturnToGridView();
        }
        else
        {
            FullscreenFromEvent(mouse_event);
        }
    }
    
    private void ReturnToGridView()
    {
        _is_fullscreen = false;
        Columns = ColumnsGridView;
    
        foreach (var kvp in _viewports)
        {
            var vc = kvp.Value;
            
            vc.Visible = true;
        }
        
        TriggerContainerUpdate();
    }
    
    private void FullscreenFromEvent(InputEventMouseButton mouse_event)
    {
        _is_fullscreen = true;
        Columns = ColumnsFullscreen;
        ViewportContainer v = GetViewportContainerFromScreenPosition(mouse_event.Position);    
       
        foreach (var kvp in _viewports)
        {
            var vc = kvp.Value;
            
            vc.Visible = vc == v;
        }
        
        TriggerContainerUpdate();
    }
    
    private ViewportContainer GetViewportContainerFromScreenPosition(Vector2 screen_position)
    {
        var container_size = RectSize / 2.0f;
        
        var is_left = screen_position.x / container_size.x < 1.0;
        var is_top = screen_position.y / container_size.y < 1.0;
        
        return is_top?
            (is_left? _viewports[TopLeft] : _viewports[TopRight]) :
            (is_left? _viewports[BottomLeft] : _viewports[BottomRight]);
    }
    
    private void TriggerContainerUpdate()
    {
        SetSize(RectSize);
    }
}
