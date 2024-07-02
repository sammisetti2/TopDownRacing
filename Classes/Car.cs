using Godot;
using System;

public partial class Car : CharacterBody2D
{
    Vector2 CarLocation;
    float CarHeading;
    float CarSpeed;
    float SteerAngle = 15;
    float SteerDirection;
    float WheelBase = 70; // the distance between the two axles

    int EnginePower = 800;
    float Friction = -0.9f;
    float Drag = -0.001f;
    Vector2 Acceleration = Vector2.Zero;
    int Braking = -450;
    int MaxSpeedReverse = 250;

    public override void _Ready()
    {
        base._Ready();
        Velocity = Vector2.Zero;
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Acceleration = Vector2.Zero;

        //Get Input
        GetInput();

        //Apply Friction
        ApplyFriction();

        //Calculate Steering
        CalculateSteering(delta);

        Velocity += Acceleration * (float)delta;

        MoveAndSlide();
    }

    public void ApplyFriction()
    {
        if (Velocity.Length() < 5)
        {
            Velocity = Vector2.Zero;
        }
        var frictionForce = Velocity * Friction;
        var dragForce = Velocity * Velocity.Length() * Drag;
        Acceleration += dragForce + frictionForce;
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

        SteerDirection = turn * Mathf.DegToRad(SteerAngle);

        if (Input.IsActionPressed("accelerate"))
        {
            Acceleration = Transform.X * EnginePower;
        }

        if (Input.IsActionPressed("brake"))
        {
            Acceleration = Transform.X * Braking;
        }
    }

    public void CalculateSteering(double delta)
    {
        //1. Find the wheel positions
        var rearWheel = Position - Transform.X * WheelBase / 2;
        var frontWheel = Position + Transform.X * WheelBase / 2;

        //2. Move the wheels forward
        rearWheel += Velocity * (float)delta;
        frontWheel += Velocity.Rotated(SteerDirection) * (float)delta;

        //3. Find the new direction vector
        var newHeading = (frontWheel - rearWheel).Normalized();

        //4. Set the Velocity to new direction depending on if you're acceleration or reversing
        var dotProduct = newHeading.Dot(Velocity.Normalized());

        if (dotProduct > 0)
        {
            Velocity = newHeading * Velocity.Length();
        }
        if (dotProduct < 0)
        {
            Velocity = -newHeading * Math.Min(Velocity.Length(), MaxSpeedReverse);
        }

        //4. Set the rotation to the new direction
        Rotation = newHeading.Angle();
    }

}
