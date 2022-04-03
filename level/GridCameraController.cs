using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GridCameraController : Control
{
    private const int FocusChildIndex = 1;

    private const String Right = "Right";
    private const String Rear = "Rear";
    private const String Left = "Left";
    private const String Front = "Front";

    private IDictionary<String, ViewportContainer> _viewports;
    private Control _v_split;
    private Control _split_view;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {            
        _viewports = new Dictionary<String, ViewportContainer> 
        {
            {Right, GetNode<ViewportContainer>("VSplit/SplitView/ViewportContainerRight")},
            {Rear, GetNode<ViewportContainer>("VSplit/SplitView/ViewportContainerRear")},
            {Left, GetNode<ViewportContainer>("VSplit/SplitView/ViewportContainerLeft")},
            {Front, GetNode<ViewportContainer>("VSplit/ViewportContainerFront")},
        };
        
        _v_split = GetNode<Control>("VSplit");
        _split_view = GetNode<Control>("VSplit/SplitView");
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
        ChangeFocusView(mouse_event);
    }
    
    private void ChangeFocusView(InputEventMouseButton mouse_event)
    {
        ViewportContainer to_focus = GetViewportContainerFromMousePosition();
        
        if (to_focus != null && !IsViewportContainerFocused(to_focus))
        {
            var focused = GetFocusedView();
            
            _v_split.RemoveChild(focused);
            _split_view.RemoveChild(to_focus);
            
            _v_split.AddChild(to_focus);
            _split_view.AddChild(focused);
        }
    }
    
    private ViewportContainer GetViewportContainerFromMousePosition()
    {
        foreach (var kvp in _viewports)
        {
            var v = kvp.Value;
            var viewport = v.GetNode<Viewport>("Viewport");
            var mouse_pos = viewport.GetMousePosition();
            var is_point_in_viewport = viewport.GetVisibleRect().HasPoint(mouse_pos);
            
            if (is_point_in_viewport)
            {
                return v;
            }
        }
        
        return null;
    }
    
    private bool IsViewportContainerFocused(ViewportContainer viewport_container)
    {
        return viewport_container.GetParent() == _v_split;
    }
    
    private ViewportContainer GetFocusedView()
    {
        return _v_split.GetChild(FocusChildIndex) as ViewportContainer;
    }
}
