using Godot;
using System;

public class Intro : Control
{
    private void _on_BtnStart_pressed()
    {
        GetTree().ChangeScene("res://level/TestLevel.tscn");
    }

    private void _on_BtnQuit_pressed()
    {
        GetTree().Quit();
    }
}
