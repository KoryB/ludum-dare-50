using Godot;
using System;

public class MathUtils : Godot.Object
{
    public static float DegreesToRadians(float degrees) 
    {
        return ((float) Math.PI / 180.0f) * degrees;
    }
}
