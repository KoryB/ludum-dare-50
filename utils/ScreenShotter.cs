using Godot;
using System;

public class ScreenShotter : Node
{
    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("ui_accept"))
        {
            SaveScreenshot(GD.Randi());
        }
    }


    private void SaveScreenshot(uint id)
    {
        var image = GetViewport().GetTexture().GetData();
        image.FlipY();

        var e = image.SavePng($"res://screenshots/screenshot-{id}.png");

        if (Error.Ok == e)
        {
            GD.Print("Screenshot saved!");
        }
        else
        {
            GD.Print("Error! ", e);
        }
    }
}
