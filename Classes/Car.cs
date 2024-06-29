using Godot;
using System;

public partial class Car : CharacterBody2D
{
    Vector2 carLocation;
    float carHeading;
    float carSpeed;
    float steerAngle = 15;
    float steerDirection;
    float wheelBase = 70; // the distance between the two axles

    public override void _Ready()
    {
        base._Ready();
        Velocity = Vector2.Zero;
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        //Get Input
        GetInput();

        //Calculate Steering
        CalculateSteering(delta);
        MoveAndSlide();
    }

    public void GetInput()
    {
        var turn = 0;
        if (Input.IsActionPressed("turnRight"))
        {
            ++turn;
        }

        if (Input.IsActionPressed("turnLeft"))
        {
            --turn;
        }

        steerDirection = turn * Mathf.DegToRad(steerAngle);
        Velocity = Vector2.Zero;

        if(Input.IsActionPressed("accelerate"))
        {
            Velocity = Transform.X * 500;
        }
    }

    public void CalculateSteering(double delta)
    {
        //1. Find the wheel positions
        var rearWheel = Position - Transform.X * wheelBase / 2;
        var frontWheel = Position + Transform.X * wheelBase / 2;

        //2. Move the wheels forward
        rearWheel += Velocity * (float)delta;
        frontWheel += Velocity.Rotated(steerDirection) * (float)delta;
        
        //3. Find the new direction vector
        var newHeading = (frontWheel - rearWheel).Normalized();
        
        //4. Set the velocity and rotation to the new direction
        Velocity = newHeading * Velocity.Length();
        Rotation = newHeading.Angle();
    }

}
